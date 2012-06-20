#include "stdafx.h"
#include <stdlib.h>
#include <Windows.h>

void StartGame()
{
	system("\"Cloudberry Kingdom.exe\"");
}

void InstallDotNet()
{
	system("\"Loader\\dotNetFx40_Full_x86_x64.exe\"");
}

void InstallXna()
{
	system("\"Loader\\xnafx40_redist.msi\"");
}

void DoNotInstall()
{
	if(MessageBox(0, TEXT("XNA 4.0 and .NET Framework 4.0 are required to run this program. You have opted not to install them. Would you like to try running Cloudberry Kingdom anyway?"), TEXT("Error"), MB_YESNO + MB_ICONWARNING) == IDYES)
		StartGame();
}

int main() 
{ 
    HKEY hKey; 

	if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("Software\\Microsoft\\NET Framework Setup\\NDP\\v4"), 0, KEY_READ, &hKey) != ERROR_SUCCESS)
	{
        if(MessageBox(0, TEXT(".NET Framework 4.0 is required to launch this game, but not installed. Do you want to install it now?"), TEXT("Error"), MB_YESNO + MB_ICONWARNING) == IDYES)
			InstallDotNet();
		else
		{
			DoNotInstall();
			return 0;
		}
	}

    if (RegOpenKeyEx(HKEY_LOCAL_MACHINE, TEXT("SOFTWARE\\Microsoft\\XNA\\Framework\\v4.0"), 0, KEY_READ, &hKey) != ERROR_SUCCESS) 
    { 
        if(MessageBox(0, TEXT("XNA Framework 4.0 is required to launch this game, but not installed. Do you want to install it now?"), TEXT("Error"), MB_YESNO + MB_ICONWARNING) == IDYES) 
			InstallXna();
		else
		{
			DoNotInstall();
			return 0;
		}
    } 
 
	StartGame();

    return 0; 
} 
 
int APIENTRY _tWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPTSTR    lpCmdLine,
                     int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	main();

	return 0;
}