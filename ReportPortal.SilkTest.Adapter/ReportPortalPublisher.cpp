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
}

void CReportPortalPublisher::StartTest(wchar_t* testFullName)
{
}

void CReportPortalPublisher::FinishTest(int testOutcome, wchar_t* testFullName)
{
}

void CReportPortalPublisher::StartLaunch()
{
}

void CReportPortalPublisher::FinishLaunch()
{
}



