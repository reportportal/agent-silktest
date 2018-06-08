#pragma once

#include "SilkTestFunctions.h"

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

	bool AddLogItem(wchar_t* logMessage, SilkTestLogLevel logLevel);

	bool StartTest(wchar_t* testFullName);
	bool FinishTest(SilkTestTestStatus testOutcome, wchar_t* testFullName);

	bool StartLaunch();
	bool FinishLaunch();

	std::wstring GetLastError();

};

extern CReportPortalPublisher ReportPortalPublisherComWrapper;
