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
	bool Init();

	bool AddLogItem(wchar_t* testFullName, wchar_t* logMessage, SilkTestLogLevel logLevel);

	bool StartTest(wchar_t* testFullName);
	bool FinishTest(wchar_t* testFullName, SilkTestTestStatus testOutcome, bool forceToFinishNestedSteps);

	bool StartLaunch();
	bool FinishLaunch();

	std::wstring GetLastError();

};

extern CReportPortalPublisher ReportPortalPublisherComWrapper;
