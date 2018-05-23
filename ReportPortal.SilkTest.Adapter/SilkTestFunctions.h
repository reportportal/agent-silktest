#pragma once

extern "C"
{
	__declspec (dllexport) bool Init(bool isTestNestingEnabled);

	__declspec (dllexport) void AddLogItem(wchar_t* logMessage, int logLevel);

	__declspec (dllexport) void StartTest(wchar_t* testFullName);
	__declspec (dllexport) void FinishTest(int testOutcome, wchar_t* testFullName);

	__declspec (dllexport) void StartLaunch();
	__declspec (dllexport) void FinishLaunch();
}
