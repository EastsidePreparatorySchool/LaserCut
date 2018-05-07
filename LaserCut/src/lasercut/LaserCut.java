/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.application.Application;
import static javafx.application.Application.launch;
import javafx.collections.ObservableSet;
import javafx.print.Printer;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.layout.StackPane;
import javafx.stage.Stage;
import com.sun.jna.platform.win32.User32;
import com.sun.jna.platform.win32.WinDef;
import com.sun.jna.platform.win32.WinDef.HDC;
import java.io.InputStream;
import java.util.List;
import javafx.scene.Group;
import javafx.scene.layout.HBox;
import javafx.scene.layout.VBox;
import javafx.scene.shape.Line;

/**
 *
 * @author gmein
 */
public class LaserCut extends Application {

    static GraphicsInterface gi = GraphicsInterface.INSTANCE;
    static Group group;
    static List<GraphicsObject> lgo;

    @Override
    public void start(Stage primaryStage) {
        Button btn = new Button("Parse");
        btn.setOnAction((e) -> parse());
        Button btn2 = new Button("Print");
        btn2.setOnAction((e) -> print());
        HBox btns = new HBox(btn, btn2);

        VBox vb = new VBox();
        group = new Group();
        StackPane p = new StackPane(group);
        p.setMinSize(300, 300);
        vb.getChildren().addAll(p, btns);

        StackPane root = new StackPane();
        root.getChildren().add(vb);

        Scene scene = new Scene(root, 300, 350);

        primaryStage.setTitle("EPS DXF Laser Cutting Tool");
        primaryStage.setScene(scene);
        primaryStage.show();
        WinDef.HWND hWnd = User32.INSTANCE.FindWindow(null, "EPS Laser Cutting Tool");
    }

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        System.out.println(gi.test("Test"));
        launch(args);
    }

    public void print() {
//        Printer printer = Printer.getDefaultPrinter();
//        ObservableSet<Printer> allPrinters = Printer.getAllPrinters();
//        for (Printer p : allPrinters) {
//            if (p.getName().equalsIgnoreCase("Microsoft Print to PDF")) {
//                printer = p;
//            }
//        }

        int i = gi.startPrint(null);
        if (i != 0) {
            System.out.println("startPrint failed "+i);
            return;
        }
        for (GraphicsObject go : lgo) {
            if (go.nodeClass == Line.class) {
                Line line = (Line) go.node;
                gi.drawLine(line.getStartX(), line.getStartY(), line.getEndX(), line.getEndY());
            }
        }
        gi.endPrint();

    }

    void parse() {
        InputStream is = LaserCut.class.getResourceAsStream("test.dxf");
        DXFReader dfr = new DXFReader();
        this.lgo = dfr.Read(is);
        for (GraphicsObject go : this.lgo) {
            this.group.getChildren().add(go.node);  
        }
    }

}
