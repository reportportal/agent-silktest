using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using EPAM.ReportPortal.Client;
using EPAM.ReportPortal.Client.Models;
using EPAM.ReportPortal.Client.Requests;

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
        private readonly List<string> _garbageList = new List<string>();
        private string _lId;
        private string _testId;
        private bool _testNestingEnabled;
        private string _lastError = string.Empty;
        private Service _reportPortal;

        private SortedDictionary<string, string> SuiteMap { get; set; }
        

        public bool Init(bool isTestNestingEnabled)
        {
            try
            {
                DebugLogger.SetupLogger(Configuration.ReportPortalConfiguration.GeneralConfiguration.LogFile,
                    Configuration.ReportPortalConfiguration.GeneralConfiguration.DebugMode);
                DebugLogger.Message("init executed");
                _testNestingEnabled = isTestNestingEnabled;
                DebugLogger.Message("testNestingEnabled: " + _testNestingEnabled);
                _lId = null;
                _testId = null;
                
                SuiteMap = new SortedDictionary<string, string>(new LengthComparer());

                _reportPortal = new Service(
                    new Uri(Configuration.ReportPortalConfiguration.ServerConfiguration.Url),
                    Configuration.ReportPortalConfiguration.ServerConfiguration.Project,
                    Configuration.ReportPortalConfiguration.ServerConfiguration.Password,
                    TryToCreateProxyServer());

                ReportSuccess();
                return true;
            }
            catch (Exception ex)
            {
                ReportError("Init", ex);
                return false;
            }
        }
        public bool AddLogItem(string logMessage, LogLevel logLevel)
        {
            try
            {
                AddLogItemToReportPortal(logMessage, logLevel);
                ReportSuccess();
                return true;
            }
            catch (Exception ex)
            {
                ReportError("AddLogItem", ex);
                return false;
            }
        }

        public bool StartLaunch()
        {
            DebugLogger.Message("StartLaunch");
            try
            {
                var launchRequest = new StartLaunchRequest()
                {
                    Name = Configuration.ReportPortalConfiguration.LaunchConfiguration.LaunchName,
                    StartTime = DateTime.UtcNow
                };
                
                var launch = _reportPortal.StartLaunch(launchRequest);
                _lId = launch?.Id;
                DebugLogger.Message("StartLaunch. ID: " + _lId);
                ReportSuccess();
                return true;
            }
            catch (Exception ex)
            {
                ReportError("StartLaunch", ex);
                return false;
            }
        }
        public bool StartTest(string testFullName)
        {
            DebugLogger.Message("StartTest: " + testFullName);
            try
            {
                if (!string.IsNullOrEmpty(_testId) && _testNestingEnabled)
                {
                    AddLogItemToReportPortal("Starting nested test case: " + testFullName, LogLevel.Info);
                    ReportSuccess();
                    return true;
                }

                var path = testFullName.Split(':').ToList();
                var testName = path[path.Count - 1];
                path = path.GetRange(0, path.Count - 1);
                string parentSuite = null;
                for (var i = 0; i < path.Count; i++)
                {
                    var currentSuiteName = string.Join(":", path.GetRange(0, i + 1));
                    if (!SuiteMap.ContainsKey(currentSuiteName))
                    {
                        var suiteId = StartSuite(path[i], parentSuite);
                        SuiteMap[currentSuiteName] = suiteId;
                        _garbageList.Add(suiteId);
                    }
                    parentSuite = SuiteMap[currentSuiteName];
                }


                var testItem = new StartTestItemRequest
                {
                    LaunchId = _lId,
                    Name = testName,
                    StartTime = DateTime.UtcNow,
                    Type = TestItemType.Step
                };
                var res = _reportPortal.StartTestItem(parentSuite, testItem);
                _garbageList.Add(res.Id);
                SuiteMap[testFullName] = res.Id;
                if (string.IsNullOrEmpty(_testId))
                {
                    _testId = res.Id;
                }
                DebugLogger.Message("StartTest: " + testFullName + "(" + _testId + ")");
                ReportSuccess();
                return true;
            }
            catch (Exception ex)
            {
                ReportError("StartTest", ex);
                return false;
            }
        }


        public bool FinishTest(Status testOutcome, string testFullName = null)
        {
            if (_garbageList.Contains(_testId))
            {
                var testIDtoFinish = _testId;
                if (!string.IsNullOrEmpty(testFullName))
                {
                    if (SuiteMap.ContainsKey(testFullName))
                    {
                        testIDtoFinish = SuiteMap[testFullName];
                    }
                    else
                    {
                        try
                        {
                            AddLogItemToReportPortal("Finish nested test case: " + testFullName, LogLevel.Info);
                            ReportSuccess();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            ReportError("FinishTest", ex);
                            return false;
                        }
                    }
                }

                DebugLogger.Message("FinishTest: " + testIDtoFinish + ". Outcome:" + testOutcome);
                try
                {
                    var finishTestItemRequest = new FinishTestItemRequest()
                    {
                        EndTime = DateTime.UtcNow,
                        Status = (EPAM.ReportPortal.Client.Models.Status) testOutcome
                    };
                    _reportPortal.FinishTestItem(testIDtoFinish, finishTestItemRequest);
                    _garbageList.Remove(testIDtoFinish);

                    if (string.IsNullOrEmpty(testFullName) || _testId == testIDtoFinish)
                        _testId = null;
                    else
                        SuiteMap.Remove(testFullName);
                }
                catch (Exception ex)
                {
                    ReportError("FinishTest", ex);
                    return false;
                }
            }
            ReportSuccess();
            return true;
        }

        public bool FinishLaunch()
        {
            try
            {
                foreach (var entry in SuiteMap)
                {
                    FinishSuite(entry.Value, Status.Passed);
                }

                _reportPortal.FinishLaunch(_lId, new FinishLaunchRequest() {EndTime = DateTime.UtcNow});

                ReportSuccess();
                return true;
            }
            catch (Exception ex)
            {
                ReportError("FinishLaunch", ex);
                return false;

            }
        }

        public string GetLastError()
        {
            return _lastError;
        }

        private void ReportSuccess()
        {
            _lastError = "success";
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
            DebugLogger.Message(_lastError);
        }


        private void AddLogItemToReportPortal(string logMessage, LogLevel logLevel)
        {
            var message = "AddLogItem: " + logMessage + ". logLevel: " + logLevel;
            DebugLogger.Message(message);
            try
            {
                var addLogItemRequest = new AddLogItemRequest()
                {
                    TestItemId =  _testId,
                    Level = (EPAM.ReportPortal.Client.Models.LogLevel)logLevel,
                    Time =  DateTime.UtcNow,
                    Text = logMessage
                };
                _reportPortal.AddLogItem(addLogItemRequest);
            }
            catch (Exception ex)
            {
                DebugLogger.Message(message + ". Exception: " + ex.Message);
                throw;
            }
        }

        private string StartSuite(string suiteName, string parentSuite = null)
        {
            DebugLogger.Message("StartSuite: " + suiteName + ". parentSuite: " + parentSuite);
            var startTestItemRequest = new StartTestItemRequest()
            {
                LaunchId = _lId,
                Name = suiteName,
                StartTime = DateTime.UtcNow,
                Type = TestItemType.Suite
            };
            var res = _reportPortal.StartTestItem(parentSuite, startTestItemRequest);
            var suiteId = res.Id;
            DebugLogger.Message("StartSuite: " + suiteName + " (" + suiteId + "). parentSuite: " + parentSuite);
            return suiteId;
        }
        
        private void FinishSuite(string suiteId, Status testOutcome)
        {
            if (_garbageList.Contains(suiteId))
            {
                var finishTestItemRequest = new FinishTestItemRequest()
                {
                    EndTime = DateTime.UtcNow,
                    Status = (EPAM.ReportPortal.Client.Models.Status) testOutcome
                };
                _reportPortal.FinishTestItem(suiteId, finishTestItemRequest);
                _garbageList.Remove(suiteId);
            }
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
    }
}