#include "StdAfx.h"
#include "ReportPortalPublisher.h"
#include "SilkTestFunctions.h"

std::wstring errorMessage;

inline std::wstring ConvertToWstring(const char* message)
{
	std::wstring_convert<std::codecvt_utf8_utf16<wchar_t>> converter;
	std::wstring wstr = converter.from_bytes(message);

	return wstr;
}

bool Init()
{
	using namespace std;
	try
	{
		ReportPortalPublisherComWrapper = CReportPortalPublisher(IReportPortalPublisherPtr(__uuidof(ReportPortalPublisher)));
		return ReportPortalPublisherComWrapper.Init();
	}
	catch (_com_error& ex)
	{
		std::wstring message = ConvertToWstring(ex.ErrorMessage());
		std::wstringstream ss;
		ss << hex << L"HRESULT: 0x" << ex.Error() << L" Message:" << message.c_str() << endl;
		errorMessage = ss.rdbuf()->str();
	}
	catch (exception& ex)
	{
		errorMessage = ConvertToWstring(ex.what());
	}
	catch (...)
	{
		errorMessage = L"Unknown error";
	}
	return false;
}

bool AddLogItem(wchar_t* logMessage, SilkTestLogLevel logLevel)
{
	try
	{
		return ReportPortalPublisherComWrapper.AddLogItem(logMessage, logLevel);
	}
	catch (const std::exception& ex)
	{
		errorMessage = ConvertToWstring(ex.what());
	}
	return false;
}

bool StartTest(wchar_t* testFullName)
{
	try
	{
		return ReportPortalPublisherComWrapper.StartTest(testFullName);
	}
	catch (const std::exception& ex)
	{
		errorMessage = ConvertToWstring(ex.what());
	}
	return false;
}
bool FinishTest(wchar_t* testFullName, SilkTestTestStatus testOutcome, bool forceToFinishNestedSteps)
{
	try
	{
		return ReportPortalPublisherComWrapper.FinishTest(testFullName, testOutcome, forceToFinishNestedSteps);
	}
	catch (const std::exception& ex)
	{
		errorMessage = ConvertToWstring(ex.what());
	}
	return false;
}

bool StartLaunch()
{
	try
	{
		return ReportPortalPublisherComWrapper.StartLaunch();
	}
	catch (const std::exception& ex)
	{
		errorMessage = ConvertToWstring(ex.what());
	}
	return false;
}
bool FinishLaunch()
{
	try
	{
		return ReportPortalPublisherComWrapper.FinishLaunch();
	}
	catch (const std::exception& ex)
	{
		errorMessage = ConvertToWstring(ex.what());
	}
	return false;
}

int GetErrorDescription(wchar_t* message, int maxMessageSize)
{
	std::wstring lastError = ReportPortalPublisherComWrapper.GetLastError();
	std::wstring& err = errorMessage.empty() ? lastError : errorMessage;

	wcsncpy_s(message, maxMessageSize, err.c_str(), err.length());
	return min(maxMessageSize, (int)err.length());
}