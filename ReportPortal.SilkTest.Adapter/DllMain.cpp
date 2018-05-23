#include "StdAfx.h"
#include <windows.h>
#include "ReportPortalPublisher.h"

BOOL APIENTRY DllMain(HANDLE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved)
{
	BOOL result = TRUE;
	switch (ul_reason_for_call) {
	case DLL_PROCESS_ATTACH:
	{
		HRESULT hr = CoInitialize(NULL);
		result = (hr == S_OK);
	}
		break;
	case DLL_PROCESS_DETACH:
		CoUninitialize();
		break;
	}
	return result;
}
