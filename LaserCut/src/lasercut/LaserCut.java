/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import com.sun.jna.Platform;
import javafx.application.Application;
import static javafx.application.Application.launch;
import javafx.collections.ObservableSet;
import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.print.PageLayout;
import javafx.print.PageOrientation;
import javafx.print.Paper;
import javafx.print.Printer;
import javafx.print.PrinterJob;
import javafx.scene.Node;
import javafx.scene.Scene;
import javafx.scene.canvas.GraphicsContext;
import javafx.scene.control.Button;
import javafx.scene.layout.StackPane;
import javafx.scene.paint.Color;
import javafx.scene.shape.ArcTo;
import javafx.scene.shape.ArcType;
import javafx.scene.shape.HLineTo;
import javafx.scene.shape.LineTo;
import javafx.scene.shape.MoveTo;
import javafx.scene.shape.Path;
import javafx.scene.shape.QuadCurveTo;
import javafx.scene.transform.Scale;
import javafx.stage.Stage;
import com.sun.jna.platform.win32.User32;
import com.sun.jna.platform.win32.WinDef;
import com.sun.jna.platform.win32.WinDef.HDC;

/**
 *
 * @author gmein
 */
public class LaserCut extends Application {
    
    static GraphicsInterface gi = GraphicsInterface.INSTANCE;

    @Override
    public void start(Stage primaryStage) {
        Button btn = new Button();
        btn.setText("Say 'Hello World'");
        btn.setOnAction(new EventHandler<ActionEvent>() {

            @Override
            public void handle(ActionEvent event) {
                System.out.println("Hello World!");
            }
        });

        StackPane root = new StackPane();
        root.getChildren().add(btn);

        Scene scene = new Scene(root, 300, 250);

        primaryStage.setTitle("Hello World!");
        primaryStage.setScene(scene);
        primaryStage.show();
        primaryStage.setTitle("EPS Laser Cutting Tool");
        WinDef.HWND hWnd = User32.INSTANCE.FindWindow(null, "EPS Laser Cutting Tool");
    }

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        System.out.println(gi.test());
        launch(args);
    }

    private void drawShapes(GraphicsContext gc) {
        gc.setFill(Color.GREEN);
        gc.setStroke(Color.BLUE);
        gc.setLineWidth(5);
        gc.strokeLine(40, 10, 10, 40);
        gc.fillOval(10, 60, 30, 30);
        gc.strokeOval(60, 60, 30, 30);
        gc.fillRoundRect(110, 60, 30, 30, 10, 10);
        gc.strokeRoundRect(160, 60, 30, 30, 10, 10);
        gc.fillArc(10, 110, 30, 30, 45, 240, ArcType.OPEN);
        gc.fillArc(60, 110, 30, 30, 45, 240, ArcType.CHORD);
        gc.fillArc(110, 110, 30, 30, 45, 240, ArcType.ROUND);
        gc.strokeArc(10, 160, 30, 30, 45, 240, ArcType.OPEN);
        gc.strokeArc(60, 160, 30, 30, 45, 240, ArcType.CHORD);
        gc.strokeArc(110, 160, 30, 30, 45, 240, ArcType.ROUND);
        gc.fillPolygon(new double[]{10, 40, 10, 40},
                new double[]{210, 210, 240, 240}, 4);
        gc.strokePolygon(new double[]{60, 90, 60, 90},
                new double[]{210, 210, 240, 240}, 4);
        gc.strokePolyline(new double[]{110, 140, 110, 140},
                new double[]{210, 210, 240, 240}, 4);

    }

    private Path makePath() {
        Path path = new Path();

        MoveTo moveTo = new MoveTo();
        moveTo.setX(0.0f);
        moveTo.setY(0.0f);

        HLineTo hLineTo = new HLineTo();
        hLineTo.setX(70.0f);

        QuadCurveTo quadCurveTo = new QuadCurveTo();
        quadCurveTo.setX(120.0f);
        quadCurveTo.setY(60.0f);
        quadCurveTo.setControlX(100.0f);
        quadCurveTo.setControlY(0.0f);

        LineTo lineTo = new LineTo();
        lineTo.setX(175.0f);
        lineTo.setY(55.0f);

        ArcTo arcTo = new ArcTo();
        arcTo.setX(50.0f);
        arcTo.setY(50.0f);
        arcTo.setRadiusX(50.0f);
        arcTo.setRadiusY(50.0f);

        path.getElements().add(moveTo);
        path.getElements().add(hLineTo);
        path.getElements().add(quadCurveTo);
        path.getElements().add(lineTo);
        path.getElements().add(arcTo);

        path.setStroke(Color.rgb(255, 0, 0));
        path.setStrokeWidth(1);
        path.setFill(null);

        return path;

    }

    public void print(final Node node) {
        Printer printer = Printer.getDefaultPrinter();
        ObservableSet<Printer> allPrinters = Printer.getAllPrinters();
        for (Printer p : allPrinters) {
            if (p.getName().equalsIgnoreCase("Microsoft Print to PDF")) {
                printer = p;
            }
        }

        PageLayout pageLayout = printer.createPageLayout(Paper.NA_LETTER, PageOrientation.PORTRAIT, Printer.MarginType.DEFAULT);
        double scaleX = pageLayout.getPrintableWidth() / node.getBoundsInParent().getWidth();
        double scaleY = pageLayout.getPrintableHeight() / node.getBoundsInParent().getHeight();
        node.getTransforms().add(new Scale(scaleX, scaleY));

        PrinterJob job = PrinterJob.createPrinterJob();
        if (job != null) {
            boolean success = job.printPage(node);
            if (success) {
                job.endJob();
            }
        }
    }

    void draw(HDC hdc) {
//        Graphics * graphics = new Graphics(hdc);
//        Pen * pen = new Pen(Color(255, 0, 0, 0));
//        graphics -> DrawLine(pen, 50, 50, 350, 550);
//        graphics -> DrawRectangle(pen, 50, 50, 300, 500);
//        graphics -> DrawEllipse(pen, 50, 50, 300, 500);
//        delete pen;
//        delete graphics;
    }

    /*
    

void print(void) {
	// Get a device context for the printer.
	HDC hdc = getPrinterDC(NULL);
	DOCINFO docInfo;
	ZeroMemory(&docInfo, sizeof(docInfo));
	docInfo.cbSize = sizeof(docInfo);
	docInfo.lpszDocName = TEXT("GdiplusPrint");

	StartDoc(hdc, &docInfo);
	StartPage(hdc);

	draw(hdc);

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



     */
}
