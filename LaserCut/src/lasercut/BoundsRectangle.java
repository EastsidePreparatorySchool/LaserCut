/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.scene.shape.Rectangle;

/**
 *
 * @author gmein
 */
public class BoundsRectangle extends GraphicsObject {

    BoundsRectangle(Rectangle r) {
        this.node = r;
    }

    @Override
    void permanentTranslate(double x, double y) {
        Rectangle r = (Rectangle) this.node;
        r.setX(r.getX() + x);
        r.setY(r.getY() + y);
    }
}
