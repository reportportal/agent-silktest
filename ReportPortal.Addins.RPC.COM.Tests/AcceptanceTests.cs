using System.Threading;
using NUnit.Framework;

namespace ReportPortal.Addins.RPC.COM.Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        [Test]
        public void StressTest01()
        {
            using (var rp = RpProxy.CreateValidPortal("StressTest01"))
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");

                for (int i = 0; i < 100 && result; i++)
                {
                    result &= rp.Publisher.AddLogItem($"Foo {i}", LogLevel.Info);
                }

                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true);    // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true);    // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true);    // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true);  // finish plan

                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }

        [Test]
        public void UseCase01()    // single req ID -> passed
        {
            using (var rp = RpProxy.CreateValidPortal("UseCase01"))
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");
                result &= result && rp.Publisher.AddLogItem("foo", LogLevel.Info);
                result &= result && rp.Publisher.AddLogItem("bar", LogLevel.Warning);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true);    // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true);    // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true);    // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true);  // finish plan
                
                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }
        [Test]
        public void UseCase02()    // two req IDs in same test case -> passed
        {
            using (var rp = RpProxy.CreateValidPortal("UseCase02"))
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");
                result &= result && rp.Publisher.AddLogItem("foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true); // finish req ID

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId02");
                result &= result && rp.Publisher.AddLogItem("bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId02", Status.Passed, true); // finish req ID

                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true); // finish plan

                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }

        [Test]
        public void UseCase03()    // two req IDs in same test case used in parallel -> failed
        {
            using (var rp = RpProxy.CreateValidPortal("UseCase03"))
            {

                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");
                result &= result && rp.Publisher.AddLogItem("foo", LogLevel.Info);

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId02");

                result &= result && rp.Publisher.AddLogItem("bar", LogLevel.Info);

                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId02", Status.Passed, true); // finish req ID

                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true); // finish plan

                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }
        [Test]
        public void UseCase04()    // two req IDs in two test cases -> passed
        {
            using (var rp = RpProxy.CreateValidPortal("UseCase04"))
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");
                result &= result && rp.Publisher.AddLogItem("foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseYYY:ReqId02");
                result &= result && rp.Publisher.AddLogItem("bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseYYY:ReqId02", Status.Passed, true); // finish req ID
                          
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseYYY", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true); // finish plan

                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }
        [Test]
        public void UseCase05()    // two req IDs in two test scripts -> passed
        {
            using (var rp = RpProxy.CreateValidPortal("UseCase04"))
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");
                result &= result && rp.Publisher.AddLogItem("foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptBlue:TestCaseYYY:ReqId02");
                result &= result && rp.Publisher.AddLogItem("bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptBlue:TestCaseYYY:ReqId02", Status.Passed, true); // finish req ID

                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptBlue:TestCaseYYY", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptBlue", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true); // finish plan

                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }
        [Test]
        public void UseCase06()    // two req IDs in two test plans -> passed
        {
            using (var rp = RpProxy.CreateValidPortal("UseCase04"))
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch();

                result &= result && rp.Publisher.StartTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01");
                result &= result && rp.Publisher.AddLogItem("foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX:ReqId01", Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true); // finish plan

                result &= result && rp.Publisher.StartTest("PlanTwo:ScriptBlue:TestCaseYYY:ReqId02");
                result &= result && rp.Publisher.AddLogItem("bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest("PlanTwo:ScriptBlue:TestCaseYYY:ReqId02", Status.Passed, true); // finish req ID

                result &= result && rp.Publisher.FinishTest("PlanTwo:ScriptBlue:TestCaseYYY", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanTwo:ScriptBlue", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanTwo", Status.Passed, true); // finish plan

                if (!result)
                {
                    rp.FailWithError();
                }
            }
        }
    }
}