using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("0723B714-DB43-40E2-B258-491ABB0FB4FE")
    ]
    public interface IReportPortalPublisher
    {
        [DispId(1)]
        bool Init(bool isTestNestingEnabled);
        [DispId(2)]
        void AddLogItem(string logMessage, int logLevel);
        [DispId(3)]
        void StartLaunch();

        [DispId(4)]
        void StartTest(string testFullName);
        [DispId(5)]
        void FinishLaunch();

        [DispId(6)]
        void FinishTest(int testOutcome, string testFullName = null);
    }
}