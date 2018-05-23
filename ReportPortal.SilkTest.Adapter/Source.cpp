#include "StdAfx.h"
#include "ReportPortalPublisher.h"
#include "SilkTestFunctions.h"
#include <exception>
#include <iostream>

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
		cerr << hex << "HRESULT: 0x" << ex.Error() << " Message:"<< ex.ErrorMessage() << endl;
	}
	catch (exception& ex)
	{
		cerr << ex.what() << endl;
	}
	catch (...)
	{
	}
	return false;
}