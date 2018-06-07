﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

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
        private object _service;
        private Assembly _assemblyReportPortalClient;
        private string _lId;
        private string _testId;
        private bool _testNestingEnabled;
        private string _lastError = string.Empty;

        private SortedDictionary<string, string> SuiteMap { get; set; }
        

        public bool Init(bool isTestNestingEnabled)
        {
            try
            {
                DebugLogger.SetupLogger(Configuration.ReportPortalConfiguration.GeneralConfiguration.LogFile,
                    Configuration.ReportPortalConfiguration.GeneralConfiguration.DebugMode);
                DebugLogger.Message("init executed");
                DebugLogger.Message("Library path: " +
                                    Configuration.ReportPortalConfiguration.GeneralConfiguration.LibraryPath);
                _assemblyReportPortalClient =
                    Assembly.LoadFrom(Configuration.ReportPortalConfiguration.GeneralConfiguration.LibraryPath);
                _testNestingEnabled = isTestNestingEnabled;
                DebugLogger.Message("testNestingEnabled: " + _testNestingEnabled);
                _lId = null;
                _testId = null;
                
                SuiteMap = new SortedDictionary<string, string>(new LengthComparer());

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
                try
                {
                    var constructorParameters = new List<object>
                    {
                        new Uri(Configuration.ReportPortalConfiguration.ServerConfiguration.Url),
                        Configuration.ReportPortalConfiguration.ServerConfiguration.Project,
                        Configuration.ReportPortalConfiguration.ServerConfiguration.Password
                    };

                    if (proxy != null)
                        constructorParameters.Add(proxy);

                    var tService = _assemblyReportPortalClient.GetType("EPAM.ReportPortal.Client.Service");
                    _service = Activator.CreateInstance(tService, constructorParameters.ToArray());
                    ReportSuccess();
                    return true;
                }
                catch (Exception ex)
                {
                    ReportError("Init", ex);
                    return false;
                }
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
                var tRequest =
                    _assemblyReportPortalClient.GetType("EPAM.ReportPortal.Client.Requests.StartLaunchRequest");
                var requestNewLaunch = Activator.CreateInstance(tRequest);

                var propName = requestNewLaunch.GetType().
                    GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                if (null != propName && propName.CanWrite)
                    propName.SetValue(requestNewLaunch,
                        Configuration.ReportPortalConfiguration.LaunchConfiguration.LaunchName, null);
                var propTime = requestNewLaunch.GetType().
                    GetProperty("StartTime", BindingFlags.Public | BindingFlags.Instance);
                if (null != propTime && propTime.CanWrite)
                    propTime.SetValue(requestNewLaunch, DateTime.UtcNow, null);

                var parameters = new object[1];
                parameters[0] = requestNewLaunch;
                var methodInfo = _service.GetType().GetMethod("StartLaunch");
                var res = methodInfo?.Invoke(_service, parameters);
                _lId = (string) res?.GetType().GetProperty("Id")?.GetValue(res);
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

                var propValue = new Dictionary<string, object>
                {
                    {"LaunchId", _lId},
                    {"Name", testName},
                    {"StartTime", DateTime.UtcNow},
                    {"Type", 3}
                };

                var parameters = new object[2];
                parameters[0] = parentSuite;
                parameters[1] = RequestWithProperties("EPAM.ReportPortal.Client.Requests.StartTestItemRequest", propValue);
                var methodsInfo = _service.GetType().GetMethods().
                    Where(p => p.Name == "StartTestItem" && p.GetParameters().Length == 2).ToArray();

                DebugLogger.Message(methodsInfo.Select(x => x.Name));
                var methodInfo = methodsInfo.First();
                var res = methodInfo.Invoke(_service, parameters);
                var newtestId = (string) res.GetType().GetProperty("Id")?.GetValue(res);

                _garbageList.Add(newtestId);
                SuiteMap[testFullName] = newtestId;
                if (string.IsNullOrEmpty(_testId))
                {
                    _testId = newtestId;
                    //testName = testFullName;
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
                    var propValue = new Dictionary<string, object>
                    {
                        {"EndTime", DateTime.UtcNow},
                        {"Status", testOutcome}
                    };

                    var parameters = new object[2];
                    parameters[0] = testIDtoFinish;
                    parameters[1] = RequestWithProperties("EPAM.ReportPortal.Client.Requests.FinishTestItemRequest",
                        propValue);

                    var methodInfo = _service.GetType().GetMethod("FinishTestItem");
                    methodInfo?.Invoke(_service, parameters);
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
                    FinishSuite(entry.Value, (int) Status.Passed);
                }

                var propValue = new Dictionary<string, object>
                {
                    {"EndTime", DateTime.UtcNow}
                };

                var parameters = new object[3];
                parameters[0] = _lId;
                parameters[1] =
                    RequestWithProperties("EPAM.ReportPortal.Client.Requests.FinishLaunchRequest", propValue);

                parameters[2] = false;
                var methodInfo = _service.GetType().GetMethod("FinishLaunch");
                methodInfo?.Invoke(_service, parameters);

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
                var propValue = new Dictionary<string, object>
                {
                    {"TestItemId", _testId},
                    {"Level", logLevel},
                    {"Time", DateTime.UtcNow},
                    {"Text", logMessage}
                };

                var parameters = new object[1];
                parameters[0] = RequestWithProperties("EPAM.ReportPortal.Client.Requests.AddLogItemRequest", propValue);

                var methodInfo = _service.GetType().GetMethod("AddLogItem");
                methodInfo?.Invoke(_service, parameters);
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
            var propValue = new Dictionary<string, object>
            {
                {"LaunchId", _lId},
                {"Name", suiteName},
                {"StartTime", DateTime.UtcNow},
                {"Type", 1}
            };

            var parameters = new object[2];
            parameters[0] = parentSuite;
            parameters[1] = RequestWithProperties("EPAM.ReportPortal.Client.Requests.StartTestItemRequest", propValue);
            var methodInfo = _service.GetType().GetMethods().
                First(p => p.Name == "StartTestItem" && p.GetParameters().Length == 2);
            var res = methodInfo.Invoke(_service, parameters);

            var suiteId = (string)res.GetType().GetProperty("Id")?.GetValue(res);
            DebugLogger.Message("StartSuite: " + suiteName + " (" + suiteId + "). parentSuite: " + parentSuite);
            return suiteId;
        }
        
        private void FinishSuite(string suiteId, int testOutcome)
        {
            if (_garbageList.Contains(suiteId))
            {
                var propValue = new Dictionary<string, object>
                {
                    {"EndTime", DateTime.UtcNow},
                    {"Status", testOutcome}
                };

                var parameters = new object[2];
                parameters[0] = suiteId;
                parameters[1] =
                    RequestWithProperties("EPAM.ReportPortal.Client.Requests.FinishTestItemRequest", propValue);
                var methodInfo = _service.GetType().GetMethod("FinishTestItem");
                methodInfo?.Invoke(_service, parameters);
                _garbageList.Remove(suiteId);
            }
        }


        private object RequestWithProperties(string typeName, Dictionary<string, object> propValue)
        {
            var tRequest = _assemblyReportPortalClient.GetType(typeName);
            var requestWithProperties = Activator.CreateInstance(tRequest);
            foreach (var entry in propValue)
            {
                var prop = requestWithProperties.GetType().
                    GetProperty(entry.Key, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                    prop.SetValue(requestWithProperties, entry.Value, null);
            }
            return requestWithProperties;
        }
    }

    //Rest
}