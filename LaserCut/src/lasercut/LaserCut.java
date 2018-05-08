/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.application.Application;
import static javafx.application.Application.launch;
import javafx.scene.Scene;
import javafx.scene.control.Button;
import javafx.scene.layout.StackPane;
import javafx.stage.Stage;
import com.sun.jna.platform.win32.User32;
import com.sun.jna.platform.win32.WinDef;
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
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.geometry.Pos;
import javafx.scene.Group;
import javafx.scene.control.ListView;
import javafx.scene.layout.AnchorPane;
import javafx.scene.layout.Background;
import javafx.scene.layout.BackgroundFill;
import javafx.scene.layout.Border;
import javafx.scene.layout.BorderStroke;
import javafx.scene.layout.BorderStrokeStyle;
import javafx.scene.layout.VBox;
import javafx.scene.paint.Color;
import javafx.scene.shape.Line;

/**
 *
 * @author gmein
 */
public class LaserCut extends Application {
    
    static LaserCut instance;
    static GraphicsInterface gi = GraphicsInterface.INSTANCE;
    static Group group;
    static List<GraphicsObject> lgo;
    static ListView<String> files;
    static double margin = 5;
    static double scaleZing = 1.0068;
    static String folder = "c:\\users\\laser cutter\\desktop\\laser dropbox";
//    static String folder = "c:\\users\gmein\desktop";
    static WatchDir wd;
    static Thread t = null;
    
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

        // preview
        AnchorPane preview = new AnchorPane();
        preview.setBackground(new Background(new BackgroundFill(Color.WHITE, null, null)));
        group = new Group();
        AnchorPane.setTopAnchor(group, 10.0);
        AnchorPane.setLeftAnchor(group, 10.0);
//        preview.setBorder(new Border(new BorderStroke(Color.BLACK,
//                BorderStrokeStyle.SOLID,
//                null,
//                BorderStroke.THIN)));
        
        preview.getChildren().addAll(group/*, h, v*/);

        // buttons
//        Button btn = new Button("Preview");
//        btn.setOnAction((e) -> {
//            parse();
//            p.getChildren().add(preview);
//        });
//        btn.setAlignment(Pos.BOTTOM_CENTER);
        Button btn2 = new Button("<- Print to Epilog Zing");
        btn2.setOnAction((e) -> print("Epilog Engraver WinX64 Zing", scaleZing));
        btn2.setAlignment(Pos.BOTTOM_LEFT);
        btn2.setDisable(true);
        
        Button btn3 = new Button("Print to Universal ->");
//        btn3.setOnAction((e) -> print("Microsoft Print to PDF", 1.0));
        btn3.setOnAction((e) -> print("PLS6.150D", 1.0));
        btn3.setAlignment(Pos.BOTTOM_RIGHT);
        btn3.setDisable(true);
        
        AnchorPane btns = new AnchorPane();
        btns.setMinWidth(500);
        btns.getChildren().addAll(btn2, btn3);
        AnchorPane.setLeftAnchor(btn2, 0.0);
        AnchorPane.setRightAnchor(btn3, 0.0);

        // events
        preview.setOnMouseClicked((e) -> {
            p.getChildren().remove(preview);
            btn2.setDisable(true);
            btn3.setDisable(true);
        });

        // listview
        files = new ListView<>();
        files.setOnMouseClicked((click) -> {
            
            if (click.getClickCount() == 2) {
                parse();
                p.getChildren().add(preview);
                btn2.setDisable(false);
                btn3.setDisable(false);
                
            }
        });
        p.getChildren().add(files);
        populateFileList(files);
        p.setMinSize(500, 300);

        // all together
        VBox vb = new VBox();
        vb.getChildren().addAll(p, btns);
        
        Scene scene = new Scene(vb, 500, 350);
        scene.setOnKeyTyped((e) -> {
            p.getChildren().remove(preview);
        });
        
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
    
    public void print(String printerName, double scale) {
        String fname = this.files.getSelectionModel().getSelectedItem();
        fname = folder + "\\" + fname.substring(0, fname.indexOf("\t"));
        parseStream(openFile(fname));
        
        int i = gi.startPrint(printerName);
        if (i != 0) {
            System.out.println("startPrint failed with code " + i);
            return;
        }
        for (GraphicsObject go : lgo) {
            if (go.nodeClass == Line.class) {
                Line line = (Line) go.node;
                gi.drawLine(line.getStartX() * scale + margin, line.getStartY() * scale + margin, line.getEndX() * scale + margin, line.getEndY() * scale + margin);
            }
        }
        gi.endPrint();
        
    }
    
    void parse() {
        String fname = this.files.getSelectionModel().getSelectedItem();
        //fname = "c:\\users\\laser cutter\\desktop\\laser dropbox\\" + fname.substring(0, fname.indexOf("\t"));
        fname = folder + "\\" + fname.substring(0, fname.indexOf("\t"));
        System.out.println(fname);
        parseStream(openFile(fname));
        this.group.getChildren().clear();
        Line h = new Line(-margin - 10.0, -margin, 500.0, -margin);
        h.setStroke(Color.RED);
        h.setStrokeWidth(1);
        Line v = new Line(-margin, -margin - 10.0, -margin, 300.0);
        v.setStroke(Color.RED);
        v.setStrokeWidth(1);
        this.group.getChildren().addAll(h, v);
        
        for (GraphicsObject go : this.lgo) {
            this.group.getChildren().add(go.node);
        }
    }
    
    void parseStream(InputStream is) {
        DXFReader dfr = new DXFReader();
        this.lgo = dfr.Read(is);
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
