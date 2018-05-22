#pragma once

using namespace ReportPortal_Addins_RPC_COM;


class CReportPortalPublisher
{
	IReportPortalPublisherPtr _reportPortalPublisherComPtr;

public:
	CReportPortalPublisher();
	~CReportPortalPublisher();

public:
	bool Initialize();
	bool Deinitialize();

public: // wrap methods
	bool Init(bool isTestNestingEnabled);

};

extern CReportPortalPublisher ReportPortalPublisherComWrapper;
