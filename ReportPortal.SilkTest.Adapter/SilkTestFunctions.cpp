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

bool AddLogItem(wchar_t* logMessage, int logLevel)
{
	return ReportPortalPublisherComWrapper.AddLogItem(logMessage, logLevel);
}

bool StartTest(wchar_t* testFullName)
{
	return ReportPortalPublisherComWrapper.StartTest(testFullName);
}
bool FinishTest(int testOutcome, wchar_t* testFullName)
{
	return ReportPortalPublisherComWrapper.FinishTest(testOutcome, testFullName);
}

bool StartLaunch()
{
	return ReportPortalPublisherComWrapper.StartLaunch();
}
bool FinishLaunch()
{
	return ReportPortalPublisherComWrapper.FinishLaunch();
}

int GetErrorDescription(wchar_t* message, int maxMessageSize)
{
	std::wstring lastError = ReportPortalPublisherComWrapper.GetLastError();
	std::wstring& err = errorMessage.empty() ? lastError : errorMessage;

	wcsncpy_s(message, maxMessageSize, err.c_str(), err.length());
	return min(maxMessageSize, (int)err.length());
}