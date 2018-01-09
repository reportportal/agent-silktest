using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportPortal.Addins.RPC.COM;

namespace TestRP
{
    class Program
    {
        static void Main(string[] args)
        {
            var RP = new ReportPortalPublisher();
            ReportPortalPublisher.Init(false);
            ReportPortalPublisher.StartLaunch();
            ReportPortalPublisher.StartTest("TestG1:TestG2:Test");


            ReportPortalPublisher.AddLogItem("_logLevelMap[1] = LogLevel.Info;", 1);
            ReportPortalPublisher.AddLogItem("_logLevelMap[2] = LogLevel.Warning;", 2);
            ReportPortalPublisher.AddLogItem("_logLevelMap[3] = LogLevel.Error;", 3);
            ReportPortalPublisher.AddLogItem("_logLevelMap[6] = LogLevel.Trace;", 6);
            ReportPortalPublisher.AddLogItem("_logLevelMap[7] = LogLevel.Debug;", 7);
            ReportPortalPublisher.StartTest("TestG1:TestG2:Test2");
            ReportPortalPublisher.FinishTest(1,"TestG1:TestG2:Test2");

            ReportPortalPublisher.FinishTest(1);
            ReportPortalPublisher.FinishLaunch();
        }
    }
}
