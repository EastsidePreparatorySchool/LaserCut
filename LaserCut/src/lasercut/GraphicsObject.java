/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import com.sun.jna.platform.win32.WinDef.HDC;
import javafx.scene.Node;

/**
 *
 * @author gmein
 */
abstract public class GraphicsObject {
    Class nodeClass;
    Node node;
    abstract void draw(HDC hdc);
    
}
