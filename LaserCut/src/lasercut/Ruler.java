/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.scene.Group;
import javafx.scene.paint.Color;
import javafx.scene.shape.Line;

/**
 *
 * @author gmein
 */
public class Ruler extends Group {

    double originX;
    double originY;
    double length;
    double overhangX;
    double overhangY;
    double tickDistance;
    Line line;
    Line[] ticks;

    enum Orientation {
        ORIENTATION_HORIZONTAL,
        ORIENTATION_VERTICAL
    }

    Ruler(double oX, double oY, double l, double ohX, double ohY, double td, Orientation o) {
        this.originX = oX;
        this.originY = oY;
        this.length = l;
        this.overhangX = ohX;
        this.overhangY = ohY;
        this.tickDistance = td;

        if (o == Orientation.ORIENTATION_HORIZONTAL) {
            line = new Line(-overhangX, 0, length, 0);
        } else {
            line = new Line(0, -overhangY, 0, length);
        }
        line.setStroke(Color.GREEN);
        super.getChildren().add(line);

        int numTicks = (int) (length / tickDistance);
        ticks = new Line[numTicks];
        for (int i = 0; i < numTicks; i++) {
            if (o == Orientation.ORIENTATION_HORIZONTAL) {
                ticks[i] = new Line(tickDistance * (i + 1), 0, tickDistance * (i + 1), (i+1)%10==0?-overhangY:((i+1)%5==0?-3*overhangY/4:-overhangY/2));
            } else {
                ticks[i] = new Line(0, tickDistance * (i + 1), (i+1)%10==0?-overhangX:((i+1)%5==0?-3*overhangX/4:-overhangX/2), tickDistance * (i + 1));
            }
            ticks[i].setStroke(Color.GREEN);
            super.getChildren().add(ticks[i]);
        }
    }
}
