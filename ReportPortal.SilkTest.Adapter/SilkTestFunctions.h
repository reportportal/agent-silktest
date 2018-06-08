#pragma once

typedef int SilkTestLogLevel;
const SilkTestLogLevel SilkTestLogLevel_Info	= 1;
const SilkTestLogLevel SilkTestLogLevel_Warning	= 2;
const SilkTestLogLevel SilkTestLogLevel_Error	= 3;
const SilkTestLogLevel SilkTestLogLevel_Trace	= 6;
const SilkTestLogLevel SilkTestLogLevel_Debug	= 7;

typedef int SilkTestTestStatus;
const SilkTestTestStatus SilkTestTestStatus_Failed	= 3;
const SilkTestTestStatus SilkTestTestStatus_Passed1 = 1;
const SilkTestTestStatus SilkTestTestStatus_Passed2 = 2;
const SilkTestTestStatus SilkTestTestStatus_None1	= 9;
const SilkTestTestStatus SilkTestTestStatus_None2	= 0;


extern "C"
{
	__declspec (dllexport) bool Init(bool isTestNestingEnabled);

	__declspec (dllexport) bool AddLogItem(wchar_t* logMessage, SilkTestLogLevel logLevel);

	__declspec (dllexport) bool StartTest(wchar_t* testFullName);
	__declspec (dllexport) bool FinishTest(SilkTestTestStatus testOutcome, wchar_t* testFullName);

	__declspec (dllexport) bool StartLaunch();
	__declspec (dllexport) bool FinishLaunch();

	__declspec (dllexport) int GetErrorDescription(wchar_t* message, int maxMessageSize);

}
