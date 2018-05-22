#include "StdAfx.h"
#include "ReportPortalPublisher.h"
#include "SilkTestFunctions.h"

bool Init(bool isTestNestingEnabled)
{
	return ReportPortalPublisherComWrapper.Init(isTestNestingEnabled);
}