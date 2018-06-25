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

bool CReportPortalPublisher::Init()
{
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->Init(&ret);
	if (hr == S_OK)
	{
		return ret != 0;
	}

	return false;
}

bool CReportPortalPublisher::AddLogItem(wchar_t* testFullName, wchar_t* logMessage, SilkTestLogLevel silkTestLogLevel)
{
	LogLevel logLevel;
	switch (silkTestLogLevel)
	{
		case SilkTestLogLevel_Info:		logLevel = LogLevel_Info;   break;
		case SilkTestLogLevel_Warning:	logLevel = LogLevel_Warning;break;
		case SilkTestLogLevel_Error:	logLevel = LogLevel_Error;	break;
		case SilkTestLogLevel_Trace:	logLevel = LogLevel_Trace;	break;
		case SilkTestLogLevel_Debug:	logLevel = LogLevel_Debug;	break;
		default: throw std::exception("Unknown log level");
	}										
	
	_bstr_t comString = logMessage;
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->AddLogItem(testFullName, comString, logLevel, &ret);
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

bool CReportPortalPublisher::FinishTest(wchar_t* testFullName, SilkTestTestStatus testOutcome, bool forceToFinishNestedSteps)
{
	Status status;
	switch (testOutcome)
	{
		case SilkTestTestStatus_Failed:		status = Status_Failed; break; 
		case SilkTestTestStatus_Passed1:	status = Status_Passed; break;
		case SilkTestTestStatus_Passed2:	status = Status_Passed;	break;
		default: throw std::exception("Unknown test result");
	}
	
	_bstr_t comString = testFullName;
	VARIANT_BOOL ret;
	HRESULT hr = _reportPortalPublisherComPtr->FinishTest(comString, status, forceToFinishNestedSteps, &ret);
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



