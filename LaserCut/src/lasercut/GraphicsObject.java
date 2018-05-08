/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import javafx.scene.Node;

/**
 *
 * @author gmein
 */
abstract public class GraphicsObject {
    Class nodeClass;
    Node node;
    
    abstract void permanentTranslate(double x, double y);
    abstract void flip90();
}
