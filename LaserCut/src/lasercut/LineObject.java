/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package lasercut;

import java.util.Scanner;
import javafx.scene.shape.Line;

/**
 *
 * @author gmein
 */
public class LineObject extends GraphicsObject {


    LineObject() {
        nodeClass = Line.class;
    }

    int parse(Scanner sc) {
        double x1 = 0;
        double x2 = 0;
        double y1 = 0;
        double y2 = 0;

        String line;

        while (sc.hasNextLine()) {
            line = sc.nextLine();
            int code = Integer.parseInt(line.trim());
            if (code == 0) {
                this.node = new Line(x1, y1, x2, y2);
                //System.out.println("Line ("+x1+","+y1+"),("+x2+","+y2+")");
                return 0;
            }

            double value = 0;
            if (sc.hasNextLine()) {
                value = Double.parseDouble(sc.nextLine().trim());
            }

            switch (code) {
                case 10:
                    x1 = value;
                    break;

                case 20:
                    y1 = value;
                    break;

                case 11:
                    x2 = value;
                    break;

                case 21:
                    y2 = value;
                    break;

                default:
                    break;
            }
        }

        // should not get here
        return 0;
    }
    
    @Override
    void permanentTranslate(double x, double y){
        Line line = (Line) this.node;
        line.setStartX(line.getStartX()+x);
        line.setStartY(line.getStartY()+y);
        line.setEndX(line.getEndX()+x);
        line.setEndY(line.getEndY()+y);
    }
}
