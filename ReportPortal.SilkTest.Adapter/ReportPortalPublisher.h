#pragma once

using namespace ReportPortal_Addins_RPC_COM;

class CReportPortalPublisher
{
	IReportPortalPublisherPtr _reportPortalPublisherComPtr;

public:
	CReportPortalPublisher();
	CReportPortalPublisher(IReportPortalPublisherPtr);
	~CReportPortalPublisher();

public: // wrap methods
	bool Init(bool isTestNestingEnabled);

};

extern CReportPortalPublisher ReportPortalPublisherComWrapper;
