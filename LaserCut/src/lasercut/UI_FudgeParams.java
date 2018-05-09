/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.scene.control.Button;
import javafx.scene.control.Label;
import javafx.scene.control.TextField;
import javafx.scene.layout.GridPane;
import javafx.scene.layout.HBox;

/**
 *
 * @author gmein
 */
public class UI_FudgeParams extends GridPane {

    UI_FudgeParams() {
        Label label = new Label("Scale X:");
        TextField textField = new TextField("1.0");
        textField.setOnAction((e) -> {
        });
        super.add(label, 0, 0);
        super.add(textField, 1, 0);

        label = new Label("Scale Y:");
        textField = new TextField("1.0");
        textField.setOnAction((e) -> {
        });
        super.add(label, 3, 0);
        super.add(textField, 4, 0);

        label = new Label("Margin X:");
        textField = new TextField("5.0");
        textField.setOnAction((e) -> {
        });
        super.add(label, 0, 1);
        super.add(textField, 1, 1);

        label = new Label("Margin Y:");
        textField = new TextField("5.0");
        textField.setOnAction((e) -> {
        });
        super.add(label, 3, 1);
        super.add(textField, 4, 1);

        Button b = new Button("Zoom in");
        b.setOnAction((e) -> {
            if (LaserCut.pv != null) {
                LaserCut.pv.getContent().setScaleX(LaserCut.pv.getScaleX() * 1.3);
                LaserCut.pv.getContent().setScaleX(LaserCut.pv.getScaleX() * 1.3);
            }
        });
        super.add(b, 0, 2);

        b = new Button("Zoom out");
        b.setOnAction((e) -> {
            if (LaserCut.pv != null) {
                LaserCut.pv.getContent().setScaleX(LaserCut.pv.getScaleX() / 1.3);
                LaserCut.pv.getContent().setScaleX(LaserCut.pv.getScaleX() / 1.3);
            }
        });
        super.add(b, 2, 2);

        b = new Button("Rotate 90 deg");
        b.setOnAction((e) -> LaserCut.instance.cuts.flip90());
        super.add(b, 4, 2);
    }
}