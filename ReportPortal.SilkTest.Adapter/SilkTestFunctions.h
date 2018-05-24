#pragma once

extern "C"
{
	__declspec (dllexport) bool Init(bool isTestNestingEnabled);

	__declspec (dllexport) bool AddLogItem(wchar_t* logMessage, int logLevel);

	__declspec (dllexport) bool StartTest(wchar_t* testFullName);
	__declspec (dllexport) bool FinishTest(int testOutcome, wchar_t* testFullName);

	__declspec (dllexport) bool StartLaunch();
	__declspec (dllexport) bool FinishLaunch();

	__declspec (dllexport) int GetErrorDescription(wchar_t* message, int maxMessageSize);

}
