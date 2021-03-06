/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import com.sun.jna.Library;
import com.sun.jna.Native;
import com.sun.jna.platform.win32.WinDef.ULONG;

/**
 *
 * @author gmein
 */
public interface GraphicsInterface extends Library {

    GraphicsInterface INSTANCE = (GraphicsInterface) Native.loadLibrary("GraphicsInterface.dll", GraphicsInterface.class);

    void drawLine(double x1, double y1, double x2, double y2);

    int startPrint(String printerName);

    void endPrint();

    int test(String str);

}
