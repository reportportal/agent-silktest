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


