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
		result = ReportPortalPublisherComWrapper.Initialize() == true;

		break;
	case DLL_PROCESS_DETACH:
		result = ReportPortalPublisherComWrapper.Deinitialize() == true;
		break;
	}
	return result;
}
