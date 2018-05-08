/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import java.io.FileInputStream;
import java.io.InputStream;
import java.util.LinkedList;
import java.util.List;
import java.util.Scanner;
import javafx.scene.Group;

/**
 *
 * @author gmein
 */
public class DXFReader {

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
        }

        return objects;
    }

}
