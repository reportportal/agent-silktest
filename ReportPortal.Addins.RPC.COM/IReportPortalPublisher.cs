using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("02DD346D-B00F-4A04-8E6F-FC795328A009")
    ]
    public interface IReportPortalPublisher
    {
        [DispId(1)]
        bool Init(bool isTestNestingEnabled);
        [DispId(2)]
        bool AddLogItem(string logMessage, LogLevel logLevel);
        [DispId(3)]
        bool StartLaunch();

        [DispId(4)]
        bool StartTest(string testFullName);
        [DispId(5)]
        bool FinishLaunch();

        [DispId(6)]
        bool FinishTest(Status testOutcome, string testFullName = null);

        [DispId(7)]
        string GetLastError();
    }
}