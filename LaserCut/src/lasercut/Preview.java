/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.scene.Group;
import javafx.scene.control.ScrollPane;
import javafx.scene.layout.Background;
import javafx.scene.layout.BackgroundFill;
import javafx.scene.paint.Color;

/**
 *
 * @author gmein
 */
public class Preview extends ScrollPane {

    Group group;

    Preview(Group g) {
        Group outer = new Group();
        
        super.setMinSize(500, 300);
        super.setHbarPolicy(ScrollBarPolicy.AS_NEEDED);
        super.setVbarPolicy(ScrollBarPolicy.AS_NEEDED);

        Ruler r = new Ruler(0, 0, 500, 10, 10, 10.0, Ruler.Orientation.ORIENTATION_HORIZONTAL);
        Ruler r2 = new Ruler(0, 0, 300, 10, 10, 10.0, Ruler.Orientation.ORIENTATION_VERTICAL);
        
        outer.getChildren().addAll(r, r2, g);

        super.setContent(outer);
    }

}
