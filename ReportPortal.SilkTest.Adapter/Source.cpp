#include "StdAfx.h"
#include "ReportPortalPublisher.h"
#include "SilkTestFunctions.h"
#include <exception>
#include <iostream>

bool Init(bool isTestNestingEnabled)
{
	using namespace std;
	try
	{
		ReportPortalPublisherComWrapper = CReportPortalPublisher(IReportPortalPublisherPtr(__uuidof(ReportPortalPublisher)));
		return ReportPortalPublisherComWrapper.Init(isTestNestingEnabled);
	}
	catch (_com_error& ex)
	{
		cerr << hex << "HRESULT: 0x" << ex.Error() << " Message:"<< ex.ErrorMessage() << endl;
	}
	catch (exception& ex)
	{
		cerr << ex.what() << endl;
	}
	catch (...)
	{
	}
	return false;
}

void AddLogItem(wchar_t* logMessage, int logLevel)
{
	ReportPortalPublisherComWrapper.AddLogItem(logMessage, logLevel);
}

void StartTest(wchar_t* testFullName)
{
	ReportPortalPublisherComWrapper.StartTest(testFullName);
}
void FinishTest(int testOutcome, wchar_t* testFullName)
{
	ReportPortalPublisherComWrapper.FinishTest(testOutcome, testFullName);
}

void StartLaunch()
{
	ReportPortalPublisherComWrapper.StartLaunch();
}
void FinishLaunch()
{
	ReportPortalPublisherComWrapper.FinishLaunch();
}