#include "stdafx.h"
#include <stdlib.h>
#include <Windows.h>

bool IsInstalled_DotNet4()
{
	HKEY hKey;
	return RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("Software\\Microsoft\\NET Framework Setup\\NDP\\v4"), 0, KEY_READ, &hKey) == ERROR_SUCCESS;
}

bool IsInstalled_Xna4()
{
	HKEY hKey;
	return RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Microsoft\\XNA\\Framework\\v4.0"), 0, KEY_READ, &hKey) == ERROR_SUCCESS;
}

void InstallDotNet()
{
	system("\"..\\Support\\dotNetFx40_Full_x86_x64.exe\"");
}

void InstallDotNet_Silent()
{
	system("\"..\\Support\\dotNetFx40_Full_x86_x64.exe\" /q /norestart");
}

void InstallXna()
{
	system("\"..\\Support\\xnafx40_redist.msi\"");
}

void InstallXna_Silent()
{
	system("\"..\\Support\\xnafx40_redist.msi\" /quiet /passive /norestart");
}

void DidNotInstall_DotNet()
{
	MessageBox(0, TEXT("Installation of the .NET Framework 4.0 has failed. Please install this dependency in order to play Cloudberry Kingdom."), TEXT("Please install dependency."), MB_OK);
}

void DidNotInstall_Xna()
{
	MessageBox(0, TEXT("Installation of the XNA Framework 4.0 has failed. Please install this dependency in order to play Cloudberry Kingdom."), TEXT("Please install dependency."), MB_OK);
}

int InstallDependencies_Silent()
{
	//InstallXna_Silent();
	//InstallXna();

	if (!IsInstalled_DotNet4())
	{
		InstallDotNet_Silent();
	}

    if (!IsInstalled_Xna4()) 
    { 
		InstallXna_Silent();
    }

	if (!IsInstalled_DotNet4())
	{
		DidNotInstall_DotNet();
		return 1;
	}

	if (!IsInstalled_Xna4())
	{
		DidNotInstall_Xna();
		return 1;
	}

	return 0;
}

int InstallDependencies() 
{ 
	if (!IsInstalled_DotNet4())
	{
        if(MessageBox(0, TEXT(".NET Framework 4.0 is required to launch this game, but not installed. Do you want to install it now?"), TEXT("Missing .NET 4.0"), MB_YESNO + MB_ICONWARNING) == IDYES)
			InstallDotNet();
		else
		{
			DidNotInstall_DotNet();
			return 1;
		}
	}

    if (!IsInstalled_Xna4()) 
    { 
        if(MessageBox(0, TEXT("XNA Framework 4.0 is required to launch this game, but not installed. Do you want to install it now?"), TEXT("Missing XNA 4.0."), MB_YESNO + MB_ICONWARNING) == IDYES) 
			InstallXna();
		else
		{
			DidNotInstall_Xna();
			return 1;
		}
    }

	if (!IsInstalled_DotNet4())
	{
		DidNotInstall_DotNet();
		return 1;
	}

	if (!IsInstalled_Xna4()) 
	{
		DidNotInstall_Xna();
		return 1;
	}
	
    return 0; 
}

int _tmain(int argc, _TCHAR* argv[])
{
	//int result = InstallDependencies();
	int result = InstallDependencies_Silent();
	return result;
}

