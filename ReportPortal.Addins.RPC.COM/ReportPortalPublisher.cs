using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using ReportPortal.Addins.RPC.COM.DataTypes;
using ReportPortal.Client;
using ReportPortal.Client.Models;
using ReportPortal.Client.Requests;
using ReportPortal.Shared;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("B872B4EF-2123-48C3-8D10-ACC9164FC8A4"),
        ProgId("ReportPortal.Publisher.API"),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IReportPortalPublisher))
    ]
    public class ReportPortalPublisher : IReportPortalPublisher, ITestable
    {
        private string _lastError = string.Empty;
        private LaunchReporter _launchReporter;
        private readonly IConfiguration _configuration;
        private readonly ConcurrentTree<TestReporter> _concurrentTree = new ConcurrentTree<TestReporter>();


        public ReportPortalPublisher() : this(new Configuration())
        {
        }

        public ReportPortalPublisher(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region ITestable

        public IEnumerable<IReadonlyNode<TestReporter>> RunningTests => _concurrentTree.RunningTests;
        #endregion

        public bool Init()
        {
            try
            {                
                var reportPortalService = new Service(new Uri(_configuration.ServerUrl), _configuration.ServerProjectName,
                    _configuration.ServerPassword, TryToCreateProxyServer());

                _launchReporter = new LaunchReporter(reportPortalService);

                ReportSuccess(nameof(Init));
                return true;
            }
            catch (Exception ex)
            {
                ReportError(nameof(Init), ex);
                return false;
            }
        }

        public bool StartLaunch(string launchName, Mode mode, string tags)
        {
            try
            {
                var launchRequest = new StartLaunchRequest()
                {
                    Name = launchName,
                    StartTime = DateTime.UtcNow,
                    Mode = (LaunchMode)mode,
                    Tags = SplitOnTags(tags)
                };
                
                _launchReporter.Start(launchRequest);
                ReportSuccess(nameof(StartLaunch));
                return true;
            }
            catch (Exception ex)
            {
                ReportError(nameof(StartLaunch), ex);
                return false;
            }
        }
        public bool StartTest(string testFullName, string tags)
        {
            try
            {
                _concurrentTree.AddPath(testFullName, (parent, name) => AddTestReporter(parent, name, tags));
                ReportSuccess(nameof(StartTest));
                return true;
            }
            catch (Exception ex)
            {
                ReportError(nameof(StartTest), ex);
                return false;
            }
        }

        public bool FinishTest(string testFullName, Status withStatus, bool forceToFinishNestedSteps)
        {
            try
            {
                var node = _concurrentTree.FindNode(testFullName);
                if (node == null)
                {
                    throw new Exception($"Test {testFullName} is not found");
                }

                if (!forceToFinishNestedSteps && node.Children.Count > 0)
                {
                    throw new Exception($"Unfinished nested steps found, please finish them before: <{node.FullName}>");
                }

                _concurrentTree.DeleteNodeWithChildren(node, current => DeleteTestReporter(current, withStatus));

                ReportSuccess(nameof(FinishTest));
                return true;
            }
            catch (Exception ex)
            {
                ReportError(nameof(FinishTest), ex);
                return false;
            }
        }

        public bool FinishLaunch()
        {
            try
            {
                _concurrentTree.Clear(current => DeleteTestReporter(current, Status.Passed));

                _launchReporter.Finish(new FinishLaunchRequest() {EndTime = DateTime.UtcNow});
                _launchReporter.FinishTask.Wait();
                ReportSuccess(nameof(FinishLaunch));
                return true;
            }
            catch (Exception ex)
            {
                ReportError(nameof(FinishLaunch), ex);
                return false;
            }
        }

        public bool AddLogItem(string testFullName, string logMessage, LogLevel logLevel)
        {
            try
            {
                var test = _concurrentTree.FindNode(testFullName);
                if (test == null)
                {
                    throw new Exception($"{testFullName} is not found");
                }

                var addLogItemRequest = new AddLogItemRequest()
                {
                    Level = (Client.Models.LogLevel)logLevel,
                    Time = DateTime.UtcNow,
                    Text = logMessage
                };
                test.Value.Log(addLogItemRequest);

                ReportSuccess(nameof(AddLogItem));
                return true;
            }
            catch (Exception ex)
            {
                ReportError(nameof(AddLogItem), ex);
                return false;
            }
        }
        public string GetLastError()
        {
            return _lastError;
        }

        private void ReportSuccess(string failedFunction)
        {
            _lastError = $"{failedFunction}: success";
        }
        private void ReportError(string failedFunction, Exception exception)
        {
            var text = new StringBuilder();
            text.Append($"{failedFunction} is failed").AppendLine();
            text.Append("Message: ").Append(exception.Message).AppendLine();

            int i = 1;
            for (var e = exception.InnerException; e != null; e = e.InnerException)
            {
                text.Append($"Inner exception #{i++}: {e.Message}").AppendLine();
            }

            _lastError = text.ToString();
        }
        
        private IWebProxy TryToCreateProxyServer()
        {
            IWebProxy proxy = null;
            if (_configuration.ProxyAvailable)
            {
                proxy = new WebProxy(_configuration.ProxyServer);
                if (!string.IsNullOrEmpty(_configuration.ProxyUser) && !string.IsNullOrEmpty(_configuration.ProxyPassword))
                    proxy.Credentials = !string.IsNullOrEmpty(_configuration.ProxyDomain)
                        ? new NetworkCredential(_configuration.ProxyUser, _configuration.ProxyPassword, _configuration.ProxyDomain)
                        : new NetworkCredential(_configuration.ProxyUser, _configuration.ProxyPassword);
            }
            return proxy;
        }

        private TestReporter AddTestReporter(IReadonlyNode<TestReporter> parent, string name, string tags)
        {
            var startTestItemRequest = new StartTestItemRequest()
            {
                Name = name,
                StartTime = DateTime.UtcNow,
                Type = TestItemType.Test,
                Tags = SplitOnTags(tags)
            };

            var suite = parent.Value != null
                ? parent.Value.StartNewTestNode(startTestItemRequest)
                : _launchReporter.StartNewTestNode(startTestItemRequest);
            return suite;
        }

        private void DeleteTestReporter(IReadonlyNode<TestReporter> node, Status withStatus)
        {
            var finishTestItemRequest = new FinishTestItemRequest()
            {
                EndTime = DateTime.UtcNow,
                Status = (Client.Models.Status)withStatus
            };

            node.Value.Finish(finishTestItemRequest);
        }

        private static List<string> SplitOnTags(string tags)
        {
            return string.IsNullOrEmpty(tags)
                ? new List<string>()
                : tags.Split(Constants.TagsSeparator).Select(x => x.Trim()).ToList();
        }
    }
}