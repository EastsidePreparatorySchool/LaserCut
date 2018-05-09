/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import java.util.List;
import javafx.geometry.Bounds;
import javafx.scene.Group;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;
import static lasercut.LaserCut.marginX;
import static lasercut.LaserCut.marginY;

/**
 *
 * @author gmein
 */
public class Cuts extends Group {

    List<GraphicsObject> objects;
    BoundsRectangle bounds;

    Cuts(List<GraphicsObject> list) {
        this.objects = list;

        super.getChildren().clear();
        for (GraphicsObject go : objects) {
            super.getChildren().add(go.node);
        }
        normalizeToOrigin();
        super.getChildren().add(bounds.node);
        updateMargins();
    }

    void updateMargins() {
        super.setTranslateX(marginX);
        super.setTranslateY(marginY);
    }

    void flip90() {
        super.getChildren().remove(bounds.node);
        for (GraphicsObject go : objects) {
            go.flip90();
        }
        normalizeToOrigin();
        super.getChildren().add(bounds.node);
    }

    private void normalizeToOrigin() {

        if (objects.isEmpty()) {
            return;
        }
        double minX = 0;
        double minY = 0;
        double maxX = 0;
        double maxY = 0;
        boolean initialized = false;

        for (GraphicsObject go : objects) {
            Bounds b = go.node.boundsInLocalProperty().get();
            if (initialized) {
                maxX = Math.max(maxX, b.getMaxX());
                maxY = Math.max(maxY, b.getMaxY());
                minX = Math.min(minX, b.getMinX());
                minY = Math.min(minY, b.getMinY());
            } else {
                maxX = b.getMaxX();
                maxY = b.getMaxY();
                minX = b.getMinX();
                minY = b.getMinY();
                initialized = true;
            }
        }

        Rectangle r = new Rectangle(0, 0, maxX - minX, maxY - minY);
        r.setStroke(Color.AZURE);
        r.setFill(null);
        bounds = new BoundsRectangle(r);

//        System.out.println("bounds: (" + minX + "," + minY + "):(" + maxX + "," + maxY + ")");
        // adjust everything to 0,0
        for (GraphicsObject go : objects) {
            go.permanentTranslate(-minX, -minY);
        }
    }
}
