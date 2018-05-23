// CppTestApp.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "../ReportPortal.SilkTest.Adapter/SilkTestFunctions.h"
#include <Windows.h>
#include <thread>
#include <iostream>

using namespace std;

int main()
{
	
	bool result = Init(false);
	cout << "init return result: " << result << endl;
    return 0;
}




