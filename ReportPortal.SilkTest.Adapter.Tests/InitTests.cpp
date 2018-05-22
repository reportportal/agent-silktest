#include "stdafx.h"
#include "CppUnitTest.h"
#include "../ReportPortal.SilkTest.Adapter/SilkTestFunctions.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace ReportPortalSilkTestAdapterTests
{		
	TEST_CLASS(Unit_InitTest)
	{
	public:
		
		TEST_METHOD(InitWithNestedTestingDisabled)
		{
			Init(false);
		}

	};
}