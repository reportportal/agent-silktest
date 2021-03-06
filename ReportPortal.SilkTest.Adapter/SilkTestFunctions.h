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
//const SilkTestTestStatus SilkTestTestStatus_None1	= 9;
//const SilkTestTestStatus SilkTestTestStatus_None2	= 0;

typedef int LaunchMode;
const LaunchMode LaunchMode_Default = 0;
const LaunchMode LaunchMode_Debug = 1;

extern "C"
{
	__declspec (dllexport) bool Init();

	// testFullName - full path to the test, hierarchy is separated by ':' symbol
	// tags - list of tags that should be associated with launch. They are separated by ';' symbol
	__declspec (dllexport) bool StartLaunch(wchar_t* launchName, LaunchMode mode, wchar_t* tags);
	__declspec (dllexport) bool FinishLaunch();

	// testFullName - full path to the test, hierarchy is separated by ':' symbol
	// tags - list of tags that should be associated with launch. They are separated by ';' symbol
	__declspec (dllexport) bool StartTest(wchar_t* testFullName, wchar_t* tags);
	__declspec (dllexport) bool FinishTest(wchar_t* testFullName, SilkTestTestStatus testOutcome, bool forceToFinishNestedSteps);

	__declspec (dllexport) bool AddLogItem(wchar_t* testFullName, wchar_t* logMessage, SilkTestLogLevel logLevel);

	__declspec (dllexport) int GetErrorDescription(wchar_t* message, int maxMessageSize);

}
