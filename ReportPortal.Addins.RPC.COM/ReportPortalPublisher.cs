using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
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
    public class ReportPortalPublisher : IReportPortalPublisher
    {
        private const char Separator = ':';
        private string _lastError = string.Empty;
        private LaunchReporter _launchReporter;
        private readonly List<Tuple<string, TestReporter>> _reporters = new List<Tuple<string, TestReporter>>();
        

        public bool Init()
        {
            try
            {                
                var reportPortalService = new Service(
                    new Uri(Configuration.ReportPortalConfiguration.ServerConfiguration.Url),
                    Configuration.ReportPortalConfiguration.ServerConfiguration.Project,
                    Configuration.ReportPortalConfiguration.ServerConfiguration.Password,
                    TryToCreateProxyServer());

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

        public bool StartLaunch()
        {
            try
            {
                var launchRequest = new StartLaunchRequest()
                {
                    Name = Configuration.ReportPortalConfiguration.LaunchConfiguration.LaunchName,
                    StartTime = DateTime.UtcNow,
                    Mode = Configuration.ReportPortalConfiguration.LaunchConfiguration.DebugMode? LaunchMode.Debug : LaunchMode.Default
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
        public bool StartTest(string testFullName)
        {
            try
            {
                var path = testFullName.Split(Separator).ToList();
                var testName = path[path.Count - 1];

                var suiteLength = path.Count - 1;
                if (_reporters.Count == 0)
                {
                    for (int i = 0; i < suiteLength; ++i)
                    {
                        AddTestReporter(path[i], TestItemType.Suite);
                    }
                }
                else
                {
                    Func<string> makePath = () => string.Join(Separator.ToString(), _reporters.Select(x => x.Item1));
                    Func<string, string, bool> isEqual = (l, r) => string.Compare(l, r, StringComparison.CurrentCultureIgnoreCase) == 0;

                    if (_reporters.Count > suiteLength)
                    {
                        ReportError(nameof(StartTest), $"Unfinished test suite was found. Please review: <{makePath()}>");
                        return false;
                    }
                    var currentSuite = _reporters[_reporters.Count - 1].Item1;
                    var requestedSuite = path[suiteLength - 1];

                    if (_reporters.Count < suiteLength)
                    {
                        var parentPathSuite = path[_reporters.Count - 1];

                        if (isEqual(currentSuite, parentPathSuite))
                        {
                            for (int i = _reporters.Count; i < suiteLength; ++i)
                            {
                                AddTestReporter(path[i], TestItemType.Suite);
                            }
                        }
                    }else if (!isEqual(currentSuite, requestedSuite))
                    {
                        ReportError(nameof(StartTest), $"Cannot start new test suite as long as the previous one is not finished <{makePath()}>.");
                        return false;
                    }
                }


                AddTestReporter(testName, TestItemType.Step); 
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
                string executingTestName = string.Join(Separator.ToString(), _reporters.Select(x => x.Item1));
                if (string.Compare(executingTestName, testFullName, StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    DeleteTestReporter(_reporters.Count - 1, withStatus);
                    ReportSuccess(nameof(FinishTest));
                    return true;
                }

                var path = testFullName.Split(Separator).ToList();

                if (path.Count >= _reporters.Count)
                {
                    ReportError(nameof(FinishTest), $"<{testFullName}> is not found. <{executingTestName}> is running.");
                    return false;
                }
                if (!forceToFinishNestedSteps)
                {
                    ReportError(nameof(FinishTest), $"Unfinished nested steps found, please finish them before: <{executingTestName}>");
                    return false;
                }

                if (executingTestName.ToLower().Contains(testFullName.ToLower()))
                {
                    FinishSuite(path.Count - 1, withStatus);
                }

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
                FinishSuite(0, Status.Passed);

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

        public bool AddLogItem(string logMessage, LogLevel logLevel)
        {
            try
            {
                AddLogItemToReportPortal(logMessage, logLevel);
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

        private void ReportError(string failedFunction, string message)
        {
            var text = new StringBuilder();
            text.Append($"{failedFunction} is failed").AppendLine();
            text.Append("Message: ").Append(message).AppendLine();
            _lastError = text.ToString();
        }


        private void AddLogItemToReportPortal(string logMessage, LogLevel logLevel)
        {
            var addLogItemRequest = new AddLogItemRequest()
            {
                Level = (Client.Models.LogLevel)logLevel,
                Time =  DateTime.UtcNow,
                Text = logMessage
            };
            _reporters[_reporters.Count - 1].Item2.Log(addLogItemRequest);
        }
        
        private static IWebProxy TryToCreateProxyServer()
        {
            IWebProxy proxy = null;
            var proxyConfig = Configuration.ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration;
            if (proxyConfig != null)
            {
                proxy = new WebProxy(proxyConfig.Server);
                if (!string.IsNullOrEmpty(proxyConfig.Username) && !string.IsNullOrEmpty(proxyConfig.Password))
                    proxy.Credentials = string.IsNullOrEmpty(proxyConfig.Domain) == false
                        ? new NetworkCredential(proxyConfig.Username, proxyConfig.Password, proxyConfig.Domain)
                        : new NetworkCredential(proxyConfig.Username, proxyConfig.Password);
            }
            return proxy;
        }

        private void FinishSuite(int index, Status withStatus)
        {
            for (int i = _reporters.Count - 1; i >= index; --i)
                DeleteTestReporter(i, withStatus);
        }

        private void AddTestReporter(string name, TestItemType testItemType)
        {
            var startTestItemRequest = new StartTestItemRequest()
            {
                Name = name,
                StartTime = DateTime.UtcNow,
                Type = testItemType
            };

            var suite = _reporters.Count != 0
                ? _reporters.Last().Item2.StartNewTestNode(startTestItemRequest)
                : _launchReporter.StartNewTestNode(startTestItemRequest);
            var reporter = new Tuple<string, TestReporter>(name, suite);
            _reporters.Add(reporter);
        }

        private void DeleteTestReporter(int index, Status withStatus)
        {
            var finishTestItemRequest = new FinishTestItemRequest()
            {
                EndTime = DateTime.UtcNow,
                Status = (Client.Models.Status)withStatus
            };
            _reporters[index].Item2.Finish(finishTestItemRequest);
            _reporters.RemoveAt(index);
        }
    }
}