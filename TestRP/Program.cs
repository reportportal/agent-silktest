using ReportPortal.Addins.RPC.COM;

namespace TestRP
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var reportPortal = new ReportPortalPublisher();
            if (reportPortal.Init(false))
            {
                reportPortal.StartLaunch();
                reportPortal.StartTest("TestG1:TestG2:Test");


                reportPortal.AddLogItem("_logLevelMap[1] = LogLevel.Info;", 1);
                reportPortal.AddLogItem("_logLevelMap[2] = LogLevel.Warning;", 2);
                reportPortal.AddLogItem("_logLevelMap[3] = LogLevel.Error;", 3);
                reportPortal.AddLogItem("_logLevelMap[6] = LogLevel.Trace;", 6);
                reportPortal.AddLogItem("_logLevelMap[7] = LogLevel.Debug;", 7);
                reportPortal.StartTest("TestG1:TestG2:Test2");
                reportPortal.FinishTest(1, "TestG1:TestG2:Test2");

                reportPortal.FinishTest(1);
                reportPortal.FinishLaunch();
            }
        }
    }
}