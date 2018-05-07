/* Author: Evren DAGLIOGLU
 * E-mail: evrenda@yahoo.com
 * This software is copyrighted to the author himself. It can be used freely for educational purposes.
 * For commercial usage written consent of the author must be taken and a reference to the author should be provided. 
 * No responsibility will be taken for any loss or damage that will occur as a result of the usage of this code. 
 * 
 * Please feel free to inform me about any bugs, problems, ideas etc.
*/

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LaserCut
{
    #region Shape class - abstract
    public abstract class Shape
    {
        protected Color contourColor;
        protected Color fillColor;
        protected float lineWidth;

        public int shapeIdentifier;
        public int rotation;
        public bool highlighted;

        public abstract Color AccessContourColor
        {
            get;
            set;
        }

        public abstract Color AccessFillColor
        {
            get;
            set;
        }

        public abstract float AccessLineWidth
        {
            get;
            set;
        }

        public abstract int AccessRotation
        {
            get;
            set;
        }

        public abstract void Draw(Pen pen, Graphics g);

    }
    #endregion

    #region Line class
    public class Line : LaserCut.Shape
    {
        protected PointF startPoint;
        protected PointF endPoint;

        public Line(PointF start, PointF end, Color color)
        {
            startPoint = start;
            endPoint = end;
            contourColor = color;
            lineWidth = 0.01F;
            shapeIdentifier = 1;
            rotation = 0;

        }

        public Line()
        {

        }


        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        public override float AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public override void Draw(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            g.DrawLine(pen, startPoint, endPoint);
        }

        public void Draw(Pen pen, Graphics g, double scale)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            //g.DrawLine(pen, startPoint, endPoint);
            g.DrawLine(pen, (float)startPoint.X * (float)scale, (float)startPoint.Y * (float)scale, (float)endPoint.X * (float)scale, (float)endPoint.Y * (float)scale);
        }

        public virtual PointF GetStartPoint
        {
            get
            {
                return startPoint;
            }

        }

        public virtual PointF GetEndPoint
        {
            get
            {
                return endPoint;
            }
        }


    }
    #endregion

    #region Rectangle class
    public class rectangle : LaserCut.Line
    {
        public rectangle(Point start, Point end, Color color, Color fill, int angle)
        {
            startPoint = start;
            endPoint = end;
            contourColor = color;
            fillColor = fill;
            lineWidth = 0.01F;
            shapeIdentifier = 2;
            rotation = angle;

        }

        public override void Draw(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            if (AccessRotation != 0)
            {
                DrawRotatedRectangle(pen, g);
                return;
            }

            g.DrawLine(pen, GetStartPoint.X, GetStartPoint.Y, GetEndPoint.X, GetStartPoint.Y);
            g.DrawLine(pen, GetEndPoint.X, GetStartPoint.Y, GetEndPoint.X, GetEndPoint.Y);
            g.DrawLine(pen, GetEndPoint.X, GetEndPoint.Y, GetStartPoint.X, GetEndPoint.Y);
            g.DrawLine(pen, GetStartPoint.X, GetEndPoint.Y, GetStartPoint.X, GetStartPoint.Y);

            return;
        }

        private void DrawRotatedRectangle(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            PointF P1 = GetStartPoint;
            PointF P2 = GetEndPoint;

            PointF P3 = new PointF(P2.X, P1.Y);
            PointF P4 = new PointF(P1.X, P2.Y);


            PointF center = new PointF(P1.X + (P3.X - P1.X) / 2, P1.Y + (P4.Y - P1.Y) / 2);

            int angle = AccessRotation;

            if (angle != 0)
            {

                P1 = CalculateRotatedNewPoint(P1, center, angle);   //Top left
                P3 = CalculateRotatedNewPoint(P3, center, angle);   //Bottom right

                P2 = CalculateRotatedNewPoint(P2, center, angle);   //Top right
                P4 = CalculateRotatedNewPoint(P4, center, angle);   //Bottom left


                g.DrawLine(pen, P1, P3);
                g.DrawLine(pen, P3, P2);
                g.DrawLine(pen, P2, P4);
                g.DrawLine(pen, P4, P1);

                return;

            }

        }

        private PointF CalculateRotatedNewPoint(PointF P, PointF center, int angle)
        {
            double angleRad = angle * 1 / 57.2957;

            PointF tempPoint = new PointF(P.X - center.X, P.Y - center.Y);

            double radius = Math.Sqrt((tempPoint.X * tempPoint.X) + (tempPoint.Y * tempPoint.Y));


            double radiant1 = Math.Acos(tempPoint.X / radius);

            if (tempPoint.X < 0 && tempPoint.Y < 0)
                radiant1 = -radiant1;

            if (tempPoint.X > 0 && tempPoint.Y < 0)
                radiant1 = -radiant1;

            double radiant2 = Math.Asin(tempPoint.Y / radius);

            radiant1 = radiant1 + angleRad;
            radiant2 = radiant2 + angleRad;

            double temp;
            temp = radius * Math.Cos(radiant1);
            P.X = (int)temp + center.X;



            temp = radius * Math.Sin(radiant1);
            P.Y = (int)temp + center.Y;


            return P;
        }

        /*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point check1 = GetStartPoint;
					Point check2 = GetEndPoint;
					
					Point check1a = new Point (check2.X, check1.Y);

					Point check2a = new Point (check1.X, check2.Y);

					Point center = new Point ( check1.X + (check1a.X - check1.X)/2, check1.Y + (check2a.Y - check1.Y)/2);

					check1 = CalculateRotatedNewPoint (check1, center, AccessRotation);

					check2 = CalculateRotatedNewPoint (check2, center, AccessRotation);
					
					check1a = CalculateRotatedNewPoint (check1a, center, AccessRotation);

					check2a = CalculateRotatedNewPoint (check2a, center, AccessRotation);


					GraphicsPath areaPath;
					Pen areaPen;
					Region areaRegion;

					// Create path which contains wide line
					// for easy mouse selection
					areaPath = new GraphicsPath();
					areaPen = new Pen(Color.Red, 2);

					areaPath.AddLine(check1, check1a);
					areaPath.AddLine(check1a, check2);
					areaPath.AddLine(check2, check2a);
					areaPath.AddLine(check2a, check1);

					areaPath.CloseFigure();

					// startPoint and EndPoint are class members of type Point
					areaPath.Widen(areaPen);

					// Create region from the path
					areaRegion = new Region(areaPath);

					RectangleF arect = areaRegion.GetBounds(g);
					
					if (arect.Contains(point))
					{
				
						g.FillPath(Brushes.DarkOrange, areaPath);
						
						//g.FillRectangle(Brushes.Green, arect);

						areaPath.Dispose();
						areaPen.Dispose();
						areaRegion.Dispose();


						return true;

					}
			
					return false;
				}
		
		*/
        /*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point P1 = GetStartPoint;
					Point P2 = GetEndPoint;

					Point P3 = new Point (P2.X, P1.Y);
					Point P4 = new Point (P1.X, P2.Y);

					if (AccessRotation != 0)
					{
						Point center = new Point ( P1.X + (P3.X - P1.X)/2, P1.Y + (P4.Y - P1.Y)/2);

						P1 = CalculateRotatedNewPoint(P1, center, AccessRotation);
						P2 = CalculateRotatedNewPoint(P2, center, AccessRotation);
						P3 = CalculateRotatedNewPoint(P3, center, AccessRotation);
						P4 = CalculateRotatedNewPoint(P4, center, AccessRotation);
					}

					int maxX = Math.Max(P1.X, P2.X);
					maxX = Math.Max(maxX, P3.X);
					maxX = Math.Max(maxX, P4.X);

					int minX = Math.Min(P1.X, P2.X);
					minX = Math.Min(minX, P3.X);
					minX = Math.Min(minX, P4.X);

					int maxY = Math.Max(P1.Y, P2.Y);
					maxY = Math.Max(maxY, P3.Y);
					maxY = Math.Max(maxY, P4.Y);

					int minY = Math.Min(P1.Y, P2.Y);
					minY = Math.Min(minY, P3.Y);
					minY = Math.Min(minY, P4.Y);

					if (point.X > minX && point.X < maxX && point.Y > minY && point.Y < maxY)
					{
						pen.Color = Color.LightGreen;

						Draw(pen, g);

						return true;
					}

					return false;
				}
		
		*/


        private double checkPosition(Point P1, Point P2, Point current)
        {
            double m = (double)(P2.Y - P1.Y) / (P2.X - P1.X);
            return ((current.Y - P1.Y) - (m * (current.X - P1.X)));
        }

    }
    #endregion

    #region Circle Class
    public class circle : LaserCut.Shape
    {
        private PointF centerPoint;
        private double radius;

        public circle(PointF center, double r, Color color1, Color color2)
        {
            centerPoint = center;
            radius = r;
            contourColor = color1;
            fillColor = color2;
            lineWidth = 0.01F;
            shapeIdentifier = 3;
            rotation = 0;
        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        public override float AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }


        public PointF AccessCenterPoint
        {
            get
            {
                return centerPoint;
            }
            set
            {
                centerPoint = value;
            }
        }

        public double AccessRadius
        {
            get
            {
                return radius;
            }
        }

        public override void Draw(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            g.DrawEllipse(pen, centerPoint.X - (int)radius, centerPoint.Y - (int)radius, (int)radius * 2, (int)radius * 2);
        }

        public void Draw(Pen pen, Graphics g, double scale)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            g.DrawEllipse(pen, (float)centerPoint.X * (float)scale - (float)radius * (float)scale, (float)centerPoint.Y * (float)scale - (float)radius * (float)scale, (float)radius * 2 * (float)scale, (float)radius * 2 * (float)scale);
        }

        /*		public override bool Highlight(Pen pen, Graphics g, Point point)
				{
					Point circular = AccessCenterPoint;
					int abc = (int) AccessRadius;
	
					GraphicsPath areaPath;
					Pen areaPen;
					Region areaRegion;

					// Create path which contains wide line
					// for easy mouse selection
					areaPath = new GraphicsPath();
					areaPen = new Pen(Color.Red, 4);
					areaPath.AddEllipse (circular.X - abc, circular.Y - abc, abc * 2, abc*2);
					// startPoint and EndPoint are class members of type Point
					areaPath.Widen(areaPen);

					areaRegion = new Region(areaPath);

					RectangleF arect = areaRegion.GetBounds(g);
					
					if (arect.Contains(point))
					{
				
						g.FillPath(Brushes.DarkOrange, areaPath);
						
						//g.FillRectangle(Brushes.Green, arect);

						areaPath.Dispose();
						areaPen.Dispose();
						areaRegion.Dispose();

		
						return true;

					}
					return false;
				}
		
		*/

      

    }
    #endregion

    #region Freehand Class - Not Completed Yet
    public class FreehandTool : LaserCut.Shape
    {
        private ArrayList linePoint;

        public FreehandTool(ArrayList points, Color color)
        {

            contourColor = color;
            lineWidth = 0.01F;
            shapeIdentifier = 4;
            rotation = 0;
            linePoint = points;

        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }

        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }


        public override float AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }
        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public override void Draw(Pen pen, Graphics g)
        {

        }

    

    }
    #endregion

    #region Polyline Class

    public class polyline : LaserCut.Shape
    {
        private ArrayList listOfLines;

        public polyline(Color color)
        {
            listOfLines = new ArrayList();

            contourColor = color;
            lineWidth = 0.01F;
        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }


        public override float AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        public override void Draw(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            foreach (Line obj in listOfLines)
            {
                obj.Draw(pen, g);
            }

        }

        public void Draw(Pen pen, Graphics g, double scale)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            foreach (Line obj in listOfLines)
            {
                obj.Draw(pen, g, scale);
            }

        }

    

     

        public void AppendLine(Line theLine)
        {
            listOfLines.Add(theLine);
        }

    }

    #endregion

    #region Arc Class

    public class arc : LaserCut.Shape
    {
        private PointF centerPoint;
        private double radius;

        private double startAngle;
        private double sweepAngle;

        public arc(PointF center, double r, double startangle, double sweepangle, Color color1, Color color2)
        {
            centerPoint = center;
            radius = r;
            startAngle = startangle;
            sweepAngle = sweepangle;
            contourColor = color1;
            fillColor = color2;
            lineWidth = 0.01F;
            shapeIdentifier = 3;
            rotation = 0;
        }

        public double AccessStartAngle
        {
            get
            {
                return startAngle;
            }

        }

        public double AccessSweepAngle
        {
            get
            {
                return sweepAngle;
            }
        }

        public override Color AccessContourColor
        {
            get
            {
                return contourColor;
            }
            set
            {
                contourColor = value;
            }
        }

        public override Color AccessFillColor
        {
            get
            {
                return fillColor;
            }
            set
            {
                fillColor = value;
            }
        }

        public override float AccessLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }

        }

        public override int AccessRotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }


        public PointF AccessCenterPoint
        {
            get
            {
                return centerPoint;
            }
            set
            {
                centerPoint = value;
            }
        }

        public double AccessRadius
        {
            get
            {
                return radius;
            }
        }

        public override void Draw(Pen pen, Graphics g)
        {
            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            g.DrawArc(pen, (float)centerPoint.X - (float)radius, (float)centerPoint.Y - (float)radius, (float)radius * 2, (float)radius * 2, -(float)startAngle, -360 + (float)startAngle - (float)sweepAngle);
        }

        public void Draw(Pen pen, Graphics g, double scale)
        {
            //g.DrawEllipse(pen, (float) centerPoint.X* (float)scale - (float) radius* (float)scale, (float)centerPoint.Y * (float)scale - (float)radius* (float)scale, (float)radius*2* (float)scale, (float)radius*2* (float)scale);

            if (highlighted)
            {
                pen.Color = Color.Red;
                highlighted = false;
            }

            float tempAngle = 0;




            if (sweepAngle < startAngle)
            {
                tempAngle = -360 + (float)startAngle - (float)sweepAngle;

            }
            /*else if (startAngle > 180 && sweepAngle > 180)
            {
                tempAngle = startAngle - sweepAngle;


            }*/
            else
                tempAngle = (float)startAngle - (float)sweepAngle;



            g.DrawArc(pen, (float)centerPoint.X * (float)scale - (float)radius * (float)scale, (float)centerPoint.Y * (float)scale - (float)radius * (float)scale, (float)radius * 2 * (float)scale, (float)radius * 2 * (float)scale, -(float)startAngle, tempAngle);
        }

        #endregion
    }

}
