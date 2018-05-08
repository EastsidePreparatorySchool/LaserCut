/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import java.io.InputStream;
import java.util.LinkedList;
import java.util.List;
import java.util.Scanner;
import javafx.geometry.Bounds;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;

/**
 *
 * @author gmein
 */
public class DXFReader {

    double minX = 0;
    double minY = 0;
    double maxX = 0;
    double maxY = 0;
    boolean initialized = false;

    List<GraphicsObject> Read(InputStream is) {
        LinkedList<GraphicsObject> objects = new LinkedList<>();
        Scanner sc = new Scanner(is);

        // read first line code
        if (!sc.hasNextLine()) {
            return null;
        }
        int nextCode = Integer.parseInt(sc.nextLine().trim());

        int n = 0;
        while (sc.hasNextLine()) {
            String value = sc.nextLine();
            switch (value) {
                case "LINE":
                    LineObject line = new LineObject();
                    nextCode = line.parse(sc);
                    objects.add(line);
//                    if (++n == 1) {
//                        return objects;
//                    }
                    break;
                default:
                    //System.out.println("Unknown token '" + value + "' ignored");
                    if (sc.hasNext()) {
                        nextCode = Integer.parseInt(sc.nextLine().trim());
                    }
                    break;
            }
            if (!objects.isEmpty()) {
                Bounds b = objects.getLast().node.boundsInLocalProperty().get();
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
        }

        if (!objects.isEmpty()) {
            Rectangle r = new Rectangle(minX, minY, maxX-minX, maxY-minY);
            r.setStroke(Color.AZURE);
            r.setFill(null);
            objects.add(new BoundsRectangle(r));
            
            for (GraphicsObject go:objects) {
                go.permanentTranslate(-minX, -minY);
            }
        }
        return objects;
    }
}
