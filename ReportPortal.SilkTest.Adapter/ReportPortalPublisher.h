#pragma once

using namespace ReportPortal_Addins_RPC_COM;

class CReportPortalPublisher
{
	IReportPortalPublisherPtr _reportPortalPublisherComPtr;

public:
	CReportPortalPublisher();
	CReportPortalPublisher(IReportPortalPublisherPtr);
	~CReportPortalPublisher();

public: // wrap methods
	bool Init(bool isTestNestingEnabled);

	bool AddLogItem(wchar_t* logMessage, int logLevel);

	bool StartTest(wchar_t* testFullName);
	bool FinishTest(int testOutcome, wchar_t* testFullName);

	bool StartLaunch();
	bool FinishLaunch();

	std::wstring GetLastError();

};

extern CReportPortalPublisher ReportPortalPublisherComWrapper;
