﻿using System;
using FakeItEasy;
using NUnit.Framework;
using ReportPortal.Client.Models;

namespace ReportPortal.Addins.RPC.COM.Tests
{
    internal class RpProxy : IDisposable
    {
        public ReportPortalPublisher Publisher { get; private set; }
        public ITestable TestMe => Publisher;
        public static RpProxy CreateValidPortal(string method)
        {
            var config = A.Fake<IConfiguration>();
            A.CallTo(() => config.ServerUrl).Returns("https://rp.epam.com/api/v1/");
            A.CallTo(() => config.ServerProjectName).Returns("default_project");
            A.CallTo(() => config.ServerPassword).Returns("7853c7a9-7f27-43ea-835a-cab01355fd17");
            A.CallTo(() => config.LaunchMode).Returns(LaunchMode.Debug);
            A.CallTo(() => config.LaunchName).Returns($"SilkTest_{method}");


            var publisher = new ReportPortalPublisher(config);
            publisher.Init();

            return new RpProxy() { Publisher = publisher };
        }

        public void Reset()
        {
            Publisher = null;
        }

        public void FailWithError()
        {
            Assert.Fail(Publisher.GetLastError());
        }
        public void Dispose()
        {
            if (Publisher != null && !Publisher.FinishLaunch())
            {
                FailWithError();
            }
        }

    }
}