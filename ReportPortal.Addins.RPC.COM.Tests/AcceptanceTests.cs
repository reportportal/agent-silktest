﻿using NUnit.Framework;

namespace ReportPortal.Addins.RPC.COM.Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        [Test]
        public void StressTest01()
        {
            using (var rp = RpProxy.CreateValidPortal())
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(StressTest01)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");

                for (int i = 0; i < 100 && result; i++)
                {
                    result &= rp.Publisher.AddLogItem(testName, $"Foo {i}", LogLevel.Info);
                }

                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true);    // finish req ID
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
            using (var rp = RpProxy.CreateValidPortal())
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(UseCase01)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");
                result &= result && rp.Publisher.AddLogItem(testName, "foo", LogLevel.Info);
                result &= result && rp.Publisher.AddLogItem(testName, "bar", LogLevel.Warning);
                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true);    // finish req ID
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
            using (var rp = RpProxy.CreateValidPortal())
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(UseCase02)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");
                result &= result && rp.Publisher.AddLogItem(testName, "foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true); // finish req ID

                var testName2 = "PlanOne:ScriptRed:TestCaseXXX:ReqId02";
                result &= result && rp.Publisher.StartTest(testName2, "");
                result &= result && rp.Publisher.AddLogItem(testName2, "bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName2, Status.Passed, true); // finish req ID

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
            using (var rp = RpProxy.CreateValidPortal())
            {

                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(UseCase03)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");
                result &= result && rp.Publisher.AddLogItem(testName, "foo", LogLevel.Info);

                var testName2 = "PlanOne:ScriptRed:TestCaseXXX:ReqId02";
                result &= result && rp.Publisher.StartTest(testName2, "");

                result &= result && rp.Publisher.AddLogItem(testName2, "bar", LogLevel.Info);

                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest(testName2, Status.Passed, true); // finish req ID

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
            using (var rp = RpProxy.CreateValidPortal())
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(UseCase04)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");
                result &= result && rp.Publisher.AddLogItem(testName, "foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case

                var testName2 = "PlanOne:ScriptRed:TestCaseYYY:ReqId02";
                result &= result && rp.Publisher.StartTest(testName2, "");
                result &= result && rp.Publisher.AddLogItem(testName2, "bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName2, Status.Passed, true); // finish req ID
                          
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
            using (var rp = RpProxy.CreateValidPortal())
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(UseCase05)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");
                result &= result && rp.Publisher.AddLogItem(testName, "foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script

                var testName2 = "PlanOne:ScriptBlue:TestCaseYYY:ReqId02";
                result &= result && rp.Publisher.StartTest(testName2, "");
                result &= result && rp.Publisher.AddLogItem(testName2, "bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName2, Status.Passed, true); // finish req ID

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
            using (var rp = RpProxy.CreateValidPortal())
            {
                bool result = rp.Publisher.Init();
                result &= result && rp.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(UseCase06)), Mode.Debug, "");

                var testName = "PlanOne:ScriptRed:TestCaseXXX:ReqId01";
                result &= result && rp.Publisher.StartTest(testName, "");
                result &= result && rp.Publisher.AddLogItem(testName, "foo", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName, Status.Passed, true); // finish req ID
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed:TestCaseXXX", Status.Passed, true); // finish test case
                result &= result && rp.Publisher.FinishTest("PlanOne:ScriptRed", Status.Passed, true); // finish script
                result &= result && rp.Publisher.FinishTest("PlanOne", Status.Passed, true); // finish plan

                var testName2 = "PlanTwo:ScriptBlue:TestCaseYYY:ReqId02";
                result &= result && rp.Publisher.StartTest(testName2, "");
                result &= result && rp.Publisher.AddLogItem(testName2, "bar", LogLevel.Info);
                result &= result && rp.Publisher.FinishTest(testName2, Status.Passed, true); // finish req ID

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