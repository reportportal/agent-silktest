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
            [Test]
            public void EnsureThatReturnTrueWithValidParameters()
            {
                using (var proxy = RpProxy.CreateValidPortal(nameof(StartLaunch)))
                {
                    bool res = proxy.Publisher.StartLaunch();

                    Assert.IsTrue(res);
                }
            }

            [Test]
            public void EnsureThatReturnFalseIfInvalidCall()
            {
                var publisher = new ReportPortalPublisher(A.Dummy<IConfiguration>());

                bool res = publisher.StartLaunch();

                Assert.IsFalse(res);
            }

            [Test]
            public void ItShouldReportSuccessWithValidParameters()
            {
                using (var proxy = RpProxy.CreateValidPortal(nameof(StartLaunch)))
                {
                    proxy.Publisher.StartLaunch();

                    string res = proxy.Publisher.GetLastError();

                    Assert.That(res, Is.EqualTo("StartLaunch: success"));
                }
            }


            [Test]
            public void ItShouldReportAnErrorWithInvalidParameters()
            {
                var publisher = new ReportPortalPublisher(A.Dummy<IConfiguration>());
                publisher.StartLaunch();

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
                _rpProxy = RpProxy.CreateValidPortal(nameof(StartTest));
                _rpProxy.Publisher.StartLaunch();
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
                var res = _rpProxy.Publisher.StartTest(name);

                Assert.IsTrue(res);
                Assert.AreEqual(name, _rpProxy.TestMe.FullTestName);
            }

            [Test]
            [TestCase("A:B:C:D")]
            public void EnsureThatHierarchyIsTheSame(string name)
            {
                var res = _rpProxy.Publisher.StartTest(name);

                Assert.IsTrue(res);
                Assert.That(name.Split(':'), Is.EquivalentTo(_rpProxy.TestMe.Hierarchy));
            }


            [Test]
            [TestCase("A:B", "A:B:C")]
            [TestCase("A:B", "A:B:C:D")]
            public void EnsureThatIsPossibleToAddNestedNodes(string parent, string child)
            {
                _rpProxy.Publisher.StartTest(parent);

                _rpProxy.Publisher.StartTest(child);
                    
                Assert.AreEqual(child, _rpProxy.TestMe.FullTestName);
            }

            [Test]
            [TestCase("A:B:C", "A:B:C")]
            [TestCase("A:B:C", "A:B:D")]
            [TestCase("A:B:C", "A:D")]
            [TestCase("A:B:C", "A:D:E")]
            [TestCase("A:B:C", "D")]
            [TestCase("A:B:C", "D:E")]
            public void EnsureThatIsNotPossibleToSiblins(string parent, string child)
            {
                _rpProxy.Publisher.StartTest(parent);

                var res = _rpProxy.Publisher.StartTest(child);

                Assert.IsFalse(res);
                Assert.AreEqual(parent, _rpProxy.TestMe.FullTestName);
            }
        }

        public class FinishTest
        {
            RpProxy _rpProxy;

            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal(nameof(FinishTest));
                _rpProxy.Publisher.StartLaunch();
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            [TestCase("A", "")]
            [TestCase("A:B", "A")]
            [TestCase("A:B:C:D", "A:B:C")]
            public void EnsureThatTestIsFinished(string name, string expectedName)
            {
                _rpProxy.Publisher.StartTest(name);

                var res = _rpProxy.Publisher.FinishTest(name, Status.Passed, false);

                Assert.IsTrue(res);
                Assert.AreEqual(expectedName, _rpProxy.TestMe.FullTestName);
            }


            [Test]
            [TestCase("A:B", "A", true, true)]
            [TestCase("A:B", "A", false, false)]
            [TestCase("A:B:C:D", "A:B", true, true)]
            [TestCase("A:B:C:D", "A:B", false, false)]
            public void EnsureThatParentSuiteFinishedWithStatus(string name, string suite, bool forceClose, bool status)
            {
                _rpProxy.Publisher.StartTest(name);

                var res = _rpProxy.Publisher.FinishTest(suite, Status.Passed, forceClose);

                Assert.AreEqual(res, status);
            }

            [Test]
            [TestCase("A:B", "A", true, "")]
            [TestCase("A:B", "A", false, "A:B")]
            [TestCase("A:B:C:D", "A:B", true, "A")]
            [TestCase("A:B:C:D", "A:B", false, "A:B:C:D")]
            public void EnsureThatParentSuiteFinishedCorrectly(string testName, string suiteToClose, bool forceClose, string expectedResult)
            {
                _rpProxy.Publisher.StartTest(testName);

                _rpProxy.Publisher.FinishTest(suiteToClose, Status.Passed, forceClose);

                Assert.AreEqual(expectedResult, _rpProxy.TestMe.FullTestName);
            }
        }

        public class FinishLaunch
        {
            RpProxy _rpProxy;
            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal(nameof(FinishLaunch));
                _rpProxy.Publisher.StartLaunch();
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            public void EnsureThatLaunchIsFinished()
            {
                _rpProxy.Publisher.StartTest("A:B:C:D");

                var res = _rpProxy.Publisher.FinishLaunch();

                Assert.IsTrue(res);
                Assert.IsEmpty(_rpProxy.TestMe.FullTestName);

                _rpProxy.Reset();
            }
        }

        public class AddLogItem
        {
            RpProxy _rpProxy;
            [SetUp]
            public void Setup()
            {
                _rpProxy = RpProxy.CreateValidPortal(nameof(AddLogItem));
                _rpProxy.Publisher.StartLaunch();
            }

            [TearDown]
            public void TearDown()
            {
                _rpProxy.Dispose();
            }

            [Test]
            public void EnsureThatLogItemSentToReportPortal()
            {
                _rpProxy.Publisher.StartTest("A:B:C:D");

                var res = _rpProxy.Publisher.AddLogItem("message", LogLevel.Info);

                Assert.IsTrue(res);
            }

            [Test]
            public void EnsureThatLogItemIsNotSentIfNoReporters()
            {
                var res = _rpProxy.Publisher.AddLogItem("message", LogLevel.Info);

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
