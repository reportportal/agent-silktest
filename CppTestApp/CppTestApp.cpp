// CppTestApp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "../ReportPortal.SilkTest.Adapter/SilkTestFunctions.h"
#include <Windows.h>
#include <thread>
#include <iostream>

using namespace std;

void DisplayLastError();

int main()
{
	
	bool result = Init();
	if (!result)
	{
		DisplayLastError();

	}
    return 0;
}

void DisplayLastError()
{
	const int bufferSize = 1024;
	wchar_t message[bufferSize];

	int len = GetErrorDescription(message, bufferSize);

	wcout << message;
}




