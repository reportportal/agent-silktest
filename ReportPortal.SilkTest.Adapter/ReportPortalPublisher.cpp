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

bool CReportPortalPublisher::AddLogItem(wchar_t* logMessage, int silkTestLogLevel)
{
	LogLevel logLevel;
	switch (silkTestLogLevel)
	{
		case 1: logLevel = LogLevel_Info;   break;
		case 2: logLevel = LogLevel_Warning;break;
		case 3: logLevel = LogLevel_Error;	break;
		case 6: logLevel = LogLevel_Trace;	break;
		case 7: logLevel = LogLevel_Debug;	break;
		default: throw std::exception("Unknown log level");
	}										
	
	_bstr_t comString = logMessage;
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->AddLogItem(comString, logLevel, &ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}

bool CReportPortalPublisher::StartTest(wchar_t* testFullName)
{
	_bstr_t comString = testFullName;
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->StartTest(comString, &ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}

bool CReportPortalPublisher::FinishTest(int testOutcome, wchar_t* testFullName)
{
	Status status;
	switch (testOutcome)
	{
		case 3: status = Status_Failed;
		case 1: status = Status_Passed;
		case 2: status = Status_Passed;
		case 9: status = Status_None;
		case 0: status = Status_None;
		default: throw std::exception("Unknown test result");
	}
	
	_bstr_t comString = testFullName;
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->FinishTest(status, comString, &ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}

bool CReportPortalPublisher::StartLaunch()
{
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->StartLaunch(&ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}

bool CReportPortalPublisher::FinishLaunch()
{
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->FinishLaunch(&ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}


std::wstring CReportPortalPublisher::GetLastError()
{
	using namespace std;

	BSTR errorMessage;
	HRESULT hr = _reportPortalPublisherComPtr->GetLastError(&errorMessage);
	_bstr_t bstr(errorMessage, false);

	if (hr == S_OK)
	{
		return std::wstring(bstr);
	}
	else
	{
		std::wstringstream ss;
		ss << hex << L"HRESULT: 0x" << hr << endl;
		return ss.rdbuf()->str();
	}
}



