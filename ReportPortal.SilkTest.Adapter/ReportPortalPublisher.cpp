#include "StdAfx.h"
#include "ReportPortalPublisher.h"

CReportPortalPublisher ReportPortalPublisherComWrapper;


CReportPortalPublisher::CReportPortalPublisher()
{
}

CReportPortalPublisher::CReportPortalPublisher(IReportPortalPublisherPtr reportPortalPublisher) : 
	_reportPortalPublisherComPtr(reportPortalPublisher)
{

}


CReportPortalPublisher::~CReportPortalPublisher()
{
}

bool CReportPortalPublisher::Init(bool isTestNestingEnabled)
{
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->Init((VARIANT_BOOL)isTestNestingEnabled, &ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}

void CReportPortalPublisher::AddLogItem(wchar_t* logMessage, int logLevel)
{
	_bstr_t comString = logMessage;
	_reportPortalPublisherComPtr->AddLogItem(comString, logLevel);
}

void CReportPortalPublisher::StartTest(wchar_t* testFullName)
{
	_bstr_t comString = testFullName;
	_reportPortalPublisherComPtr->StartTest(comString);
}

void CReportPortalPublisher::FinishTest(int testOutcome, wchar_t* testFullName)
{
	_bstr_t comString = testFullName;
	_reportPortalPublisherComPtr->FinishTest(testOutcome, comString);
}

void CReportPortalPublisher::StartLaunch()
{
	_reportPortalPublisherComPtr->StartLaunch();
}

void CReportPortalPublisher::FinishLaunch()
{
	_reportPortalPublisherComPtr->FinishLaunch();
}



