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


                reportPortal.AddLogItem("LogLevel.Info;", LogLevel.Info);
                reportPortal.AddLogItem("LogLevel.Warning;", LogLevel.Warning);
                reportPortal.AddLogItem("LogLevel.Error;", LogLevel.Error);
                reportPortal.AddLogItem("LogLevel.Trace;", LogLevel.Trace);
                reportPortal.AddLogItem("LogLevel.Debug;", LogLevel.Trace);

                reportPortal.StartTest("TestG1:TestG2:Test2");
                reportPortal.FinishTest(Status.Passed, "TestG1:TestG2:Test2");

                reportPortal.FinishTest(Status.Passed);
                reportPortal.FinishLaunch();
            }
        }
    }
}