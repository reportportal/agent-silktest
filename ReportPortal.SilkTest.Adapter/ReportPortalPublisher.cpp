#include "StdAfx.h"
#include "ReportPortalPublisher.h"

CReportPortalPublisher ReportPortalPublisherComWrapper;

CReportPortalPublisher::CReportPortalPublisher()
{
}


CReportPortalPublisher::~CReportPortalPublisher()
{
}

bool CReportPortalPublisher::Initialize()
{
	HRESULT hr = CoInitialize(NULL);
	if (hr == S_OK)
	{
		_reportPortalPublisherComPtr = IReportPortalPublisherPtr(__uuidof(IReportPortalPublisherPtr));
		return true;
	}
	return false;
}

bool CReportPortalPublisher::Deinitialize()
{
	CoUninitialize();
	return true;
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


