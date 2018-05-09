/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.scene.Group;
import javafx.scene.Node;
import javafx.scene.control.ScrollPane;

/**
 *
 * @author gmein
 */
public class Preview extends ScrollPane {

    Node target;

    Preview(Group g) {
        Group outer = new Group();
        Ruler r = new Ruler(0, 0, 500, 10, 10, 10.0, Ruler.Orientation.ORIENTATION_HORIZONTAL);
        Ruler r2 = new Ruler(0, 0, 300, 10, 10, 10.0, Ruler.Orientation.ORIENTATION_VERTICAL);

        outer.getChildren().addAll(r, r2, g);

        super.setContent(outer);
        super.setMinSize(500, 300);
        super.setHbarPolicy(ScrollBarPolicy.AS_NEEDED);
        super.setVbarPolicy(ScrollBarPolicy.AS_NEEDED);
    }

    void zoomIn() {
        Group g = (Group) getContent();
        g.setScaleX(g.getScaleX() * 1.3);
        g.setScaleY(g.getScaleY() * 1.3);
        layout();
    }

    void zoomOut() {
        Group g = (Group) getContent();

        System.out.println("Scale x " +g.getScaleX());
        if (g.getScaleX() > 1.1) {
            g.setScaleX(g.getScaleX() / 1.3);
            g.setScaleY(g.getScaleY() / 1.3);
            layout();
        }
    }

}
