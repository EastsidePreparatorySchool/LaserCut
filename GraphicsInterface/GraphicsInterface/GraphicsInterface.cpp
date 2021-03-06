// GraphicsInterface.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"


#include "stdafx.h"
using namespace Gdiplus;



// Global Variables:

Graphics* graphics;
Pen* pen;
HDC hdc;



// Forward declarations of functions included in this code module:
extern "C" __declspec(dllexport) void drawLine(double x1, double y1, double x2, double y2);
extern "C" __declspec(dllexport) int  startPrint(char *printerName);
extern "C" __declspec(dllexport) void  endPrint(void);
extern "C" __declspec(dllexport) int test(char * str);
HDC getPrinterDC(TCHAR *printerName);


GdiplusStartupInput gdiplusStartupInput;
ULONG_PTR           gdiplusToken;



extern "C" __declspec(dllexport) int test(char *str) {
	if (strcmp(str, "Test") == 0)
		return 123456;
	else
		return 0;
}



extern "C" __declspec(dllexport) void drawLine(double x1, double y1, double x2, double y2)
{
	graphics->DrawLine(pen, (REAL) x1, (REAL) y1, (REAL) x2, (REAL) y2);
}




extern "C" __declspec(dllexport) int startPrint(char *printerName) {
	// Get a device context for the printer.
	TCHAR *pname;

	if (printerName != NULL) {
		pname = new TCHAR[1024];
		size_t count;
		mbstowcs_s(&count, pname, 1024, printerName, 1023);
	}
	else {
		pname = NULL;
	}

	hdc = getPrinterDC(pname);
	if (hdc == NULL) {
		return -1;
	}
	DOCINFO docInfo;
	ZeroMemory(&docInfo, sizeof(docInfo));
	docInfo.cbSize = sizeof(docInfo);
	docInfo.lpszDocName = TEXT("EPS DXF Cutting Tool");

	StartDoc(hdc, &docInfo);
	StartPage(hdc);
	// Initialize GDI+.
	GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);

	graphics = new Graphics(hdc);
	if (graphics == NULL) {
		return -2;
	}
	graphics->SetPageUnit(UnitMillimeter);
	pen = new Pen(Color(255, 255, 0, 0));
	if (pen == NULL) {
		return -3;
	}
	pen->SetWidth(0.01F);

	return 0;
}

extern "C" __declspec(dllexport) void endPrint(void) {
	delete pen;
	delete graphics;
	GdiplusShutdown(gdiplusToken);
	EndPage(hdc);
	EndDoc(hdc);
	DeleteDC(hdc);
}

HDC getPrinterDC(TCHAR *printerName) {
	DWORD numprinters;
	DWORD defprinter = 0;
	DWORD               dwSizeNeeded = 0;
	DWORD               dwItem;
	LPPRINTER_INFO_2    printerinfo = NULL;

	// Get buffer size

	EnumPrinters(PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS, NULL, 2, NULL, 0, &dwSizeNeeded, &numprinters);

	// allocate memory
	//printerinfo = (LPPRINTER_INFO_2)HeapAlloc ( GetProcessHeap (), HEAP_ZERO_MEMORY, dwSizeNeeded );
	printerinfo = (LPPRINTER_INFO_2)new char[dwSizeNeeded];

	if (EnumPrinters(PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS,      // what to enumerate
		NULL,           // printer name (NULL for all)
		2,              // level
		(LPBYTE)printerinfo,        // buffer
		dwSizeNeeded,       // size of buffer
		&dwSizeNeeded,      // returns size
		&numprinters            // return num. items
	) == 0)
	{
		numprinters = 0;
	}

	{
		DWORD size = 0;

		// Get the size of the default printer name.
		GetDefaultPrinter(NULL, &size);
		if (size)
		{
			// Allocate a buffer large enough to hold the printer name.
			TCHAR* buffer = new TCHAR[size];

			// Get the printer name.
			GetDefaultPrinter(buffer, &size);

			for (dwItem = 0; dwItem < numprinters; dwItem++)
			{
				if (!wcscmp(buffer, printerinfo[dwItem].pPrinterName))
					defprinter = dwItem;
			}
			delete buffer;
		}
	}

	return CreateDC(NULL, printerName == NULL ? printerinfo[defprinter].pPrinterName : printerName, NULL, NULL);
}




