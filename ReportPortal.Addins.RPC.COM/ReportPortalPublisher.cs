using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Net;
using System.Diagnostics;
using RGiesecke.DllExport;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ReportPortal.Addins.RPC.COM
{

    public enum Status
    {
        None,
        InProgress,
        Passed,
        Failed,
        Skipped,
        Interrupted
    }

    public enum LogLevel
    {
        None = 0,
        Trace = 1,//
        Debug = 2,//
        Info = 3,//
        Warning = 4,//
        Error = 5//
    }

    public class ReportPortalPublisher
    {
        static Dictionary<int, Status> _statusMap = new Dictionary<int, Status>();
        static Dictionary<int, LogLevel> _logLevelMap = new Dictionary<int, LogLevel>();
        static SortedDictionary<string, string> _suiteMap;
        static List<string> garbageList = new List<string>();
        static object service;
        static Assembly assemblyReportPortalClient;
        static string lId;
        static string testID;
        static bool testNestingEnabled = false;

        [DllExport]
        public static void Init(bool isTestNestingEnabled)
        {
            try
            {
                DebugLogger.SetupLogger(Configuration.ReportPortalConfiguration.GeneralConfiguration.LogFile, Configuration.ReportPortalConfiguration.GeneralConfiguration.DebugMode);
                DebugLogger.Message("init executed");
                DebugLogger.Message("Library path: "+ Configuration.ReportPortalConfiguration.GeneralConfiguration.LibraryPath);
                assemblyReportPortalClient = Assembly.LoadFrom(Configuration.ReportPortalConfiguration.GeneralConfiguration.LibraryPath);
                testNestingEnabled = isTestNestingEnabled;
                DebugLogger.Message("testNestingEnabled: " + testNestingEnabled);
                lId = null;
                testID = null;

                _statusMap[3] = Status.Failed;
                _statusMap[1] = Status.Passed;
                _statusMap[2] = Status.Passed;
                _statusMap[9] = Status.None;
                _statusMap[0] = Status.None;

                _logLevelMap[1] = LogLevel.Info;
                _logLevelMap[2] = LogLevel.Warning;
                _logLevelMap[3] = LogLevel.Error;
                _logLevelMap[6] = LogLevel.Trace;
                _logLevelMap[7] = LogLevel.Debug;

                _suiteMap = new SortedDictionary<string, string>(new LengthComparer());

                IWebProxy proxy = null;
                var proxyConfig = Configuration.ReportPortalConfiguration.GeneralConfiguration.ProxyConfiguration;
                if (proxyConfig != null)
                {
                    proxy = new WebProxy(proxyConfig.Server);
                    if (!String.IsNullOrEmpty(proxyConfig.Username) && !String.IsNullOrEmpty(proxyConfig.Password))
                    {
                        proxy.Credentials = String.IsNullOrEmpty(proxyConfig.Domain) == false
                            ? new NetworkCredential(proxyConfig.Username, proxyConfig.Password, proxyConfig.Domain)
                            : new NetworkCredential(proxyConfig.Username, proxyConfig.Password);
                    }
                }
                try
                {
                    List<object> constructorParameters = new List<object>() {
                    new Uri(Configuration.ReportPortalConfiguration.ServerConfiguration.Url),
                    Configuration.ReportPortalConfiguration.ServerConfiguration.Project,
                    Configuration.ReportPortalConfiguration.ServerConfiguration.Password };

                    if (proxy != null)
                    {
                        constructorParameters.Add(proxy);
                    }

                    Type tService = assemblyReportPortalClient.GetType("EPAM.ReportPortal.Client.Service");
                    service = Activator.CreateInstance(tService, constructorParameters.ToArray());
                }
                catch (Exception ex)
                {
                    DebugLogger.Message(ex.Message);
                }
            }
            catch (Exception e)
            {
                DebugLogger.Message(e.Message);
            }
        }

        private static void _AddLogItem(string logMessage, int logLevel)
        {
            DebugLogger.Message("AddLogItem: " + logMessage.ToString() + ". logLevel: " + logLevel);
            try
            {
                var propValue = new Dictionary<string, object>()
                {
                    { "TestItemId", testID},
                    { "Level",  _logLevelMap[logLevel]},
                    { "Time" , DateTime.UtcNow },
                    { "Text" , logMessage }
                };

                object[] parameters = new object[1];
                parameters[0] = requestWithProperties("EPAM.ReportPortal.Client.Requests.AddLogItemRequest", propValue); ;
                MethodInfo methodInfo = service.GetType().GetMethod("AddLogItem");
                var res = methodInfo.Invoke(service, parameters);
            }
            catch (Exception ex)
            {
                DebugLogger.Message("AddLogItem: " + logMessage.ToString() + ". logLevel: " + logLevel + ". Exception: " + ex.Message);
            }
        }

        [DllExport]
        public static void AddLogItem([MarshalAs(UnmanagedType.LPWStr)] string logMessage, int logLevel)
        {
            _AddLogItem(logMessage, logLevel);
        }

        [DllExport]
        public static void StartLaunch()
        {
            DebugLogger.Message("StartLaunch");
            try {
                Type tRequest = assemblyReportPortalClient.GetType("EPAM.ReportPortal.Client.Requests.StartLaunchRequest");
                object requestNewLaunch = Activator.CreateInstance(tRequest);

                PropertyInfo propName = requestNewLaunch.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                if (null != propName && propName.CanWrite)
                {
                    propName.SetValue(requestNewLaunch, Configuration.ReportPortalConfiguration.LaunchConfiguration.LaunchName, null);
                }
                PropertyInfo propTime = requestNewLaunch.GetType().GetProperty("StartTime", BindingFlags.Public | BindingFlags.Instance);
                if (null != propTime && propTime.CanWrite)
                {
                    propTime.SetValue(requestNewLaunch, DateTime.UtcNow, null);
                }

                object[] parameters = new object[1];
                parameters[0] = requestNewLaunch;
                MethodInfo methodInfo = service.GetType().GetMethod("StartLaunch");
                var res = methodInfo.Invoke(service, parameters);
                lId = (string)res.GetType().GetProperty("Id").GetValue(res);
                DebugLogger.Message("StartLaunch. ID: " + lId.ToString());
            }
            catch (Exception ex)
            {
                DebugLogger.Message("StartLaunch\r\n" + ex.Message + "\r\n" + ex.InnerException.Message);

            }
        }

        public static string StartSuite(string suiteName, DateTime time, string parentSuite = null)
        {
            DebugLogger.Message("StartSuite: " + suiteName.ToString() + ". parentSuite: " + parentSuite);
            var propValue = new Dictionary<string, object>()
                {
                    { "LaunchId", lId},
                    { "Name",  suiteName},
                    { "StartTime" , DateTime.UtcNow },
                    { "Type" , 1 }
                };

            object[] parameters = new object[2];
            parameters[0] = parentSuite;
            parameters[1] = requestWithProperties("EPAM.ReportPortal.Client.Requests.StartTestItemRequest", propValue);
            MethodInfo methodInfo = service.GetType().GetMethods().Where(p => p.Name == "StartTestItem" && p.GetParameters().Length == 2).First();
            var res = methodInfo.Invoke(service, parameters);

            var suiteID = (string)res.GetType().GetProperty("Id").GetValue(res);
            DebugLogger.Message("StartSuite: " + suiteName.ToString() + " (" + suiteID + "). parentSuite: " + parentSuite );
            return suiteID;

        }

        [DllExport]
        public static void StartTest([MarshalAs(UnmanagedType.LPWStr)] string testFullName)
        {
            DebugLogger.Message("StartTest: " + testFullName );
            if (!String.IsNullOrEmpty(testID) && testNestingEnabled)
            {
                _AddLogItem("Starting nested test case: " + testFullName, 1);
                return;
            }

            try
            {
                var path = testFullName.Split(':').ToList();
                var testName = path[path.Count - 1];
                path = path.GetRange(0, path.Count - 1);
                string parentSuite = null;
                for (var i = 0; i < path.Count; i++)
                {
                    var currentSuiteName = String.Join(":", path.GetRange(0, i + 1));
                    if (!_suiteMap.ContainsKey(currentSuiteName))
                    {
                        var suiteID = StartSuite(path[i], DateTime.UtcNow, parentSuite);
                        _suiteMap[currentSuiteName] = suiteID;
                        garbageList.Add(suiteID);
                    }
                    parentSuite = _suiteMap[currentSuiteName];
                }

                var propValue = new Dictionary<string, object>()
                {
                    {"LaunchId", lId},
                    {"Name",  testName},
                    { "StartTime" , DateTime.UtcNow },
                    { "Type" , 3 }
                };

                object[] parameters = new object[2];
                parameters[0] = parentSuite;
                parameters[1] = requestWithProperties("EPAM.ReportPortal.Client.Requests.StartTestItemRequest", propValue);
                var methodsInfo = service.GetType().GetMethods().Where(p => p.Name == "StartTestItem" && p.GetParameters().Length == 2);
                DebugLogger.Message(methodsInfo.Select(x => x.Name));
                MethodInfo methodInfo = methodsInfo.First();
                var res = methodInfo.Invoke(service, parameters);
                var _newtestID = (string)res.GetType().GetProperty("Id").GetValue(res);

                garbageList.Add(_newtestID);
                _suiteMap[testFullName] = _newtestID;
                if (String.IsNullOrEmpty(testID))
                {
                    testID = _newtestID;
                    testName = testFullName;
                }
                DebugLogger.Message("StartTest: " + testFullName + "(" + testID + ")" );
            }
            catch (Exception ex)
            {

                DebugLogger.Message("StartTest\r\n" + ex.Message );
            }
        }

        [DllExport]
        public static void FinishLaunch()
        {
            foreach (KeyValuePair<string, string> entry in _suiteMap)
            {
                Console.WriteLine(entry);
                FinishSuite(entry.Value, (int)Status.Passed);
            }

            var propValue = new Dictionary<string, object>()
            {
                { "EndTime" , DateTime.UtcNow }
            };

            object[] parameters = new object[3];
            parameters[0] = lId;
            parameters[1] = requestWithProperties("EPAM.ReportPortal.Client.Requests.FinishLaunchRequest", propValue); ;
            parameters[2] = false;
            MethodInfo methodInfo = service.GetType().GetMethod("FinishLaunch");
            var res = methodInfo.Invoke(service, parameters);
        }

        public static void FinishSuite(String suiteId, int testOutcome)
        {
            if (garbageList.Contains(suiteId))
            {
                var propValue = new Dictionary<string, object>()
            {
                { "EndTime" , DateTime.UtcNow },
                { "Status" , testOutcome }
            };

                object[] parameters = new object[2];
                parameters[0] = suiteId;
                parameters[1] = requestWithProperties("EPAM.ReportPortal.Client.Requests.FinishTestItemRequest", propValue);
                MethodInfo methodInfo = service.GetType().GetMethod("FinishTestItem");
                var res = methodInfo.Invoke(service, parameters);
                garbageList.Remove(suiteId);
            }
        }



        private static object requestWithProperties(string typeName, Dictionary<string, object> propValue)
        {
            Type tRequest = assemblyReportPortalClient.GetType(typeName);
            object _requestWithProperties = Activator.CreateInstance(tRequest);
            foreach (KeyValuePair<string, object> entry in propValue)
            {
                PropertyInfo prop = _requestWithProperties.GetType().GetProperty(entry.Key, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite)
                {
                    prop.SetValue(_requestWithProperties, entry.Value, null);
                }
            }
            return _requestWithProperties;
        }

        [DllExport]
        public static void FinishTest(/*[MarshalAs(UnmanagedType.LPWStr)]*/ int testOutcome, [MarshalAs(UnmanagedType.LPWStr)] string testFullName = null)
        {
            if (garbageList.Contains(testID))
            {
                var testIDtoFinish = testID;
                if (!String.IsNullOrEmpty(testFullName))
                {
                    if (_suiteMap.ContainsKey(testFullName))
                    {
                        testIDtoFinish = _suiteMap[testFullName];
                    }
                    else
                    {
                        _AddLogItem("Finish nested test case: " + testFullName, 1);
                        return;
                    }
                }

                DebugLogger.Message("FinishTest: " + testIDtoFinish + ". Outcome:" + testOutcome.ToString());
                try
                {
                    var propValue = new Dictionary<string, object>()
                {
                    { "EndTime" , DateTime.UtcNow },
                    { "Status" , _statusMap[testOutcome] }
                };

                    object[] parameters = new object[2];
                    parameters[0] = testIDtoFinish;
                    parameters[1] = requestWithProperties("EPAM.ReportPortal.Client.Requests.FinishTestItemRequest", propValue);

                    MethodInfo methodInfo = service.GetType().GetMethod("FinishTestItem");
                    var res = methodInfo.Invoke(service, parameters);
                    garbageList.Remove(testIDtoFinish);
                    if (String.IsNullOrEmpty(testFullName) || testID == testIDtoFinish)
                    {
                        testID = null;
                    }
                    else
                    {
                        _suiteMap.Remove(testFullName);
                    }

                }
                catch (Exception ex)
                {
                    DebugLogger.Message("FinishTest\r\n" + ex.InnerException.Message );

                }
            }
        }
    }
    //Rest

}
