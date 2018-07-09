using System.Linq;
using FakeItEasy;
using NUnit.Framework;

namespace ReportPortal.Addins.RPC.COM.Tests
{
    [TestFixture]
    public class ReportPortalPublisherTests
    {
        public class Init
        {
            IConfiguration _config;
            ReportPortalPublisher _publisher;

            [SetUp]
            public void Setup()
            {
                _config = A.Fake<IConfiguration>();
                _publisher = new ReportPortalPublisher(_config);
            }

            [Test]
            [TestCase("https://rp.epam.com/api/v1/", "default_project", "7853c7a9-7f27-43ea-835a-cab01355fd17", true)]
            [TestCase("http://google.com", "default_project", "7853c7a9-7f27-43ea-835a-cab01355fd17", false)]
            [TestCase("https://rp.epam.com/api/v1/", "default_project", "", false)]
            public void EnsureThatInitReturnExpectedResultWith(string url, string name, string password, bool expectedResult)
            {
                ProvideParametersToConfig(url, name, password);

                bool res = _publisher.Init();

                Assert.IsTrue(res);
            }

            private void ProvideParametersToConfig(string url, string name, string password)
            {
                A.CallTo(() => _config.ServerUrl).Returns(url);
                A.CallTo(() => _config.ServerProjectName).Returns(name);
                A.CallTo(() => _config.ProxyPassword).Returns(password);
            }

            [Test]
            public void InitShouldReportSuccessWithEpamDefaultProject()
            {
                ProvideParametersToConfig("https://rp.epam.com/api/v1/", "default_project", "7853c7a9-7f27-43ea-835a-cab01355fd17");
                _publisher.Init();

                string res = _publisher.GetLastError();

                Assert.AreEqual(res, "Init: success");
            }


            [Test]
            [TestCase("https://rp.epam.com/api/v1/blablabla", "default_project", "7853c7a9-7f27-43ea-835a-cab01355fd17")]
            [TestCase("", "default_project", "7853c7a9-7f27-43ea-835a-cab01355fd17")]
            [TestCase("https://rp.epam.com/api/v1/blablabla", "", "7853c7a9-7f27-43ea-835a-cab01355fd17")]
            [TestCase("https://rp.epam.com/api/v1/blablabla", "default_project", "")]
            [TestCase("https://rp.epam.com/api/v1/blablabla", "default_project", "11111111-1111-1111-1111-111111111111")]

            public void InitShouldReportAnErrorWithInvalidParameters(string url, string name, string password)
            {
                ProvideParametersToConfig(url, name, password);

                _publisher.Init();

                string res = _publisher.GetLastError();

                Assert.That(res, Is.Not.EqualTo("success").And.Not.Empty);
            }
        }


        public class StartLaunch
        {
            private readonly string _launchName = RpProxy.GetLaunchName(nameof(StartLaunch));
            [Test]
            public void EnsureThatReturnTrueWithValidParameters()
            {
                using (var proxy = RpProxy.CreateValidPortal())
                {
                    bool res = proxy.Publisher.StartLaunch(_launchName, Mode.Debug, "tag1;tag2");

                    Assert.IsTrue(res);
                }
            }

            [Test]
            public void EnsureThatReturnFalseIfInvalidCall()
            {
                var publisher = new ReportPortalPublisher(A.Dummy<IConfiguration>());

                bool res = publisher.StartLaunch(_launchName, Mode.Debug, "tag1;tag2");

                Assert.IsFalse(res);
            }

            [Test]
            public void ItShouldReportSuccessWithValidParameters()
            {
                using (var proxy = RpProxy.CreateValidPortal())
                {
                    proxy.Publisher.StartLaunch(_launchName, Mode.Debug, "tag1;tag2");

                    string res = proxy.Publisher.GetLastError();

                    Assert.That(res, Is.EqualTo("StartLaunch: success"));
                }
            }


            [Test]
            public void ItShouldReportAnErrorWithInvalidParameters()
            {
                var publisher = new ReportPortalPublisher(A.Dummy<IConfiguration>());
                publisher.StartLaunch(_launchName, Mode.Debug, "tag1;tag2");

                string res = publisher.GetLastError();

                Assert.That(res, Is.Not.EqualTo("success").And.Not.Empty);
            }
        }


        public class StartTest
        {
            RpProxy _rpProxy;

            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal();
                _rpProxy.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(StartTest)), Mode.Debug, "StartTest;tag1;tag2");
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            [TestCase("A")]
            [TestCase("A:B")]
            [TestCase("A:B:C:D")]
            public void EnsureThatTestNameIsValid(string name)
            {
                var res = _rpProxy.Publisher.StartTest(name, "1");

                Assert.IsTrue(res);
                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(name));
            }


            [Test]
            [TestCase("A:B", "A:B:C")]
            [TestCase("A:B", "A:B:C:D")]
            public void EnsureThatIsPossibleToAddNestedNodes(string parent, string child)
            {
                _rpProxy.Publisher.StartTest(parent, "parent");

                _rpProxy.Publisher.StartTest(child, "parent;child");

                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(child));
            }

            [Test]
            [TestCase("A:B:C", "A:B:D")]
            [TestCase("A:B:C", "A:D")]
            [TestCase("A:B:C", "A:D:E")]
            [TestCase("A:B:C", "D")]
            [TestCase("A:B:C", "D:E")]
            public void EnsureThatIsPossibleToAddSiblins(string firstTest, string secondTest)
            {
                _rpProxy.Publisher.StartTest(firstTest, "");

                var res = _rpProxy.Publisher.StartTest(secondTest,"");

                Assert.IsTrue(res);
                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(firstTest));
                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(secondTest));
            }
            [Test]
            [TestCase("A", "A")]
            [TestCase("A:B", "A:B")]
            [TestCase("A:B:C", "A:B:C")]
            public void EnsureThatDublicatesAreReused(string firstTest, string secondTest)
            {
                _rpProxy.Publisher.StartTest(firstTest, "");

                var res = _rpProxy.Publisher.StartTest(secondTest, "");

                Assert.IsTrue(res);
                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(firstTest));
                Assert.AreEqual(1, _rpProxy.TestMe.RunningTests.Count());
            }

        }

        public class FinishTest
        {
            RpProxy _rpProxy;

            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal();
                _rpProxy.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(FinishTest)), Mode.Debug, "FinishTest");
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            [TestCase("A:B", "A")]
            [TestCase("A:B:C:D", "A:B:C")]
            public void EnsureThatTestIsFinished(string name, string expectedName)
            {
                _rpProxy.Publisher.StartTest(name, "");

                var res = _rpProxy.Publisher.FinishTest(name, Status.Passed, false);

                Assert.IsTrue(res);
                Assert.That(_rpProxy.TestMe.RunningTests.Select(x=>x.FullName), Has.No.Member(name));
                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(expectedName));
            }

            [Test]
            public void EnsureThatRootTestIsFinished()
            {
                const string name = "A";
                _rpProxy.Publisher.StartTest(name, "");

                var res = _rpProxy.Publisher.FinishTest(name, Status.Passed, false);

                Assert.IsTrue(res);
                Assert.IsEmpty(_rpProxy.TestMe.RunningTests);
            }


            [Test]
            [TestCase("A:B", "A", true, true)]
            [TestCase("A:B", "A", false, false)]
            [TestCase("A:B:C:D", "A:B", true, true)]
            [TestCase("A:B:C:D", "A:B", false, false)]
            public void EnsureThatParentSuiteFinishedWithStatus(string name, string suite, bool forceClose, bool status)
            {
                _rpProxy.Publisher.StartTest(name, "");

                var res = _rpProxy.Publisher.FinishTest(suite, Status.Passed, forceClose);

                Assert.AreEqual(res, status);
            }

            [Test]
            [TestCase("A:B", "A", new string[0])]
            [TestCase("A:B:C:D", "A:B", new[] {"A"})]
            public void EnsureThatSetIsClosed(string testName, string suiteToClose, string[] expectedResult)
            {
                _rpProxy.Publisher.StartTest(testName, "");

                _rpProxy.Publisher.FinishTest(suiteToClose, Status.Passed, true);

                Assert.That(_rpProxy.TestMe.RunningTests.Select(x=>x.FullName), Is.EquivalentTo(expectedResult));
            }
            [Test]
            [TestCase("A:B", "A")]
            [TestCase("A:B:C:D", "A:B")]
            public void EnsureThatSetIsNotClosed(string testName, string suiteToClose)
            {
                _rpProxy.Publisher.StartTest(testName, "");

                _rpProxy.Publisher.FinishTest(suiteToClose, Status.Passed, false);

                Assert.That(_rpProxy.TestMe.RunningTests.Select(x => x.FullName), Has.Member(testName));
            }

        }

        public class FinishLaunch
        {
            RpProxy _rpProxy;
            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal();
                _rpProxy.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(FinishLaunch)), Mode.Debug, "FinshLaunch");
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            public void EnsureThatLaunchIsFinished()
            {
                _rpProxy.Publisher.StartTest("A:B:C:D", "");

                var res = _rpProxy.Publisher.FinishLaunch();

                Assert.IsTrue(res);
                Assert.That(_rpProxy.TestMe.RunningTests, Is.Empty);

                _rpProxy.Reset();
            }
        }

        public class AddLogItem
        {
            RpProxy _rpProxy;
            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal();
                _rpProxy.Publisher.StartLaunch(RpProxy.GetLaunchName(nameof(AddLogItem)), Mode.Debug, "");
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            public void EnsureThatLogItemSentToReportPortal()
            {
                var testName = "A:B:C:D";
                _rpProxy.Publisher.StartTest(testName, "");

                var res = _rpProxy.Publisher.AddLogItem(testName, "message", LogLevel.Info);

                Assert.IsTrue(res);
            }

            [Test]
            public void EnsureThatLogItemIsNotSentIfNoReporters()
            {
                var res = _rpProxy.Publisher.AddLogItem("A", "message", LogLevel.Info);

                Assert.IsFalse(res);
            }

        }
        public class GetLastError
        {
            [Test]
            public void ItShouldBeEmptyInAColdStart()
            {
                var publisher = new ReportPortalPublisher(A.Dummy<IConfiguration>());

                var res = publisher.GetLastError();

                Assert.IsEmpty(res);
            }

        }

    }
}
