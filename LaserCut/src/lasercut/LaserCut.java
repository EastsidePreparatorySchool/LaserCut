/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import com.sun.jna.platform.win32.WinDef.ULONG;
import javafx.application.Application;
import static javafx.application.Application.launch;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.layout.StackPane;
import javafx.stage.Stage;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.List;
import java.util.Scanner;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.geometry.Pos;
import javafx.scene.control.ListView;
import javafx.scene.control.ScrollPane;
import javafx.scene.layout.AnchorPane;
import javafx.scene.layout.VBox;
import javafx.scene.shape.Line;

/**
 *
 * @author gmein
 */
public class LaserCut extends Application {

    static LaserCut instance;
    static GraphicsInterface gi = GraphicsInterface.INSTANCE;

    static Cuts cuts;
    static ListView<String> files;

    static double marginX = 5;
    static double marginY = 5;
    static double scaleX = 1.0;
    static double scaleY = 1.0;

    static double scaleZing = 1.0019;
    static double scaleUniversal = 1.0;

    static String folder = "c:\\users\\laser cutter\\desktop\\laser dropbox";
//    static String folder = "c:\\users\gmein\desktop";
    static WatchDir wd;
    static Thread t = null;
    
    static Preview pv;

    @Override
    public void start(Stage primaryStage) {

        instance = this;

        // watch folder
        try {
            Path dir = Paths.get(folder);
            wd = new WatchDir(dir);
            t = new Thread(() -> wd.processEvents());
            t.start();
        } catch (IOException ex) {
            System.out.println("Could not watch folder");
        }

        StackPane p = new StackPane();

        // buttons
        Button btn2 = new Button("<- Print to Epilog Zing");
        btn2.setOnAction((e) -> print("Epilog Engraver WinX64 Zing", scaleZing));
        btn2.setAlignment(Pos.BOTTOM_LEFT);
        btn2.setDisable(true);

        Button btn3 = new Button("Print to Universal ->");
//        btn3.setOnAction((e) -> print("Microsoft Print to PDF", 1.0));
        btn3.setOnAction((e) -> print("PLS6.150D", scaleUniversal));
        btn3.setAlignment(Pos.BOTTOM_RIGHT);
        btn3.setDisable(true);

        AnchorPane btns = new AnchorPane();
        btns.setMinWidth(500);
        btns.getChildren().addAll(btn2, btn3);
        AnchorPane.setLeftAnchor(btn2, 0.0);
        AnchorPane.setRightAnchor(btn3, 0.0);

        // listview
        files = new ListView<>();

        p.getChildren().add(files);
        populateFileList(files);
        p.setMinSize(500, 300);

        // all together
        VBox vb = new VBox();
        vb.getChildren().addAll(p, new UI_FudgeParams(), btns);
        Scene scene = new Scene(vb, 500, 400);

        // all events other than buttons
        // display preview on double click, enable printing
        files.setOnMouseClicked((click) -> {
            if (click.getClickCount() == 2) {
                parse();
                btn2.setDisable(false);
                btn3.setDisable(false);
                pv = new Preview(cuts);
//                pv = new ZoomableScrollPane(cuts);
                p.getChildren().add(pv);

            }
        });

   
        // on pressing ESC in preview, go back to list, disable printing
        scene.setOnKeyTyped((e) -> {
            if (e.getCharacter().equals("\u001b")) {
                if (pv != null) {
                    p.getChildren().remove(pv);
                    btn2.setDisable(true);
                    btn3.setDisable(true);
                    pv = null;
                }
            }
        });

        primaryStage.setTitle("EPS DXF Laser Cutting Tool");
        primaryStage.setScene(scene);
        primaryStage.show();
    }

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        if (gi.test("Test") != 123456) {
            System.out.println("Could not load graphics DLL <press enter to quit>");
            Scanner sc = new Scanner (System.in);
            sc.nextLine();
            return;
        }
        
        System.out.println("Initializing, please ignore this window ...");
        
        launch(args);
        if (t != null) {
            t.interrupt();
        }
    }

    InputStream openFile(String name) {
        InputStream is = null;
        try {
            is = new FileInputStream(name);
        } catch (FileNotFoundException ex) {
            System.out.println("File not found");
        }

        return is;
    }

    public void print(String printerName, double printerScale) {
        String fname = this.files.getSelectionModel().getSelectedItem();
        fname = folder + "\\" + fname.substring(0, fname.indexOf("\t"));
        parseStream(openFile(fname));

        int i = gi.startPrint(printerName);
        if (i != 0) {
            System.out.println("startPrint failed with code " + i);
            return;
        }
        for (GraphicsObject go : this.cuts.objects) {
            if (go.nodeClass == Line.class) {
                Line line = (Line) go.node;
                gi.drawLine(line.getStartX() * printerScale * scaleX + marginX,
                        line.getStartY() * printerScale * scaleY + marginY,
                        line.getEndX() * printerScale * scaleX + marginX,
                        line.getEndY() * printerScale * scaleY + marginY);
            }
        }
        gi.endPrint();

    }

    void parse() {
        String fname = this.files.getSelectionModel().getSelectedItem();
        //fname = "c:\\users\\laser cutter\\desktop\\laser dropbox\\" + fname.substring(0, fname.indexOf("\t"));
        fname = folder + "\\" + fname.substring(0, fname.indexOf("\t"));
        System.out.println(fname);
        List<GraphicsObject> l = parseStream(openFile(fname));
        this.cuts = new Cuts(l);

    }

    List<GraphicsObject> parseStream(InputStream is) {
        DXFReader dfr = new DXFReader();
        return dfr.Read(is);
    }

    static void refreshFiles() {
        LaserCut.instance.populateFileList(LaserCut.files);
    }

    void populateFileList(ListView<String> lv) {
        //String myDirectoryPath = "c:\\users\\laser cutter\\desktop\\laser dropbox";
        File dir = new File(folder);
        File[] directoryListing = dir.listFiles();
        Arrays.sort(directoryListing, (a, b) -> (int) (a.lastModified() - b.lastModified()));
        ObservableList<String> items = FXCollections.observableArrayList();
        DateFormat df = new SimpleDateFormat("MM/dd/yyyy hh:mm a");

        if (directoryListing != null) {
            for (File child : directoryListing) {
                String name = child.getName().toLowerCase();
                if (name.endsWith(".dxf")) {
                    name += "\t\t" + df.format(child.lastModified());
                    items.add(name);
                }
            }
        } else {
            // Handle the case where dir is not really a directory.
            // Checking dir.isDirectory() above would not be sufficient
            // to avoid race conditions with another process that deletes
            // directories.
        }

        lv.setItems(items);
        Platform.runLater(() -> {
            lv.scrollTo(0);
            lv.getFocusModel().focus(0);
            lv.getSelectionModel().select(0);
        });
    }

}
