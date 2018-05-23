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

	void AddLogItem(wchar_t* logMessage, int logLevel);

	void StartTest(wchar_t* testFullName);
	void FinishTest(int testOutcome, wchar_t* testFullName);

	void StartLaunch();
	void FinishLaunch();
};

extern CReportPortalPublisher ReportPortalPublisherComWrapper;
