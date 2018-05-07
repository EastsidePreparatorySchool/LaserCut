/* Author: Evren DAGLIOGLU
 * E-mail: evrenda@yahoo.com
 * This software is copyrighted to the author himself. It can be used freely for educational purposes.
 * For commercial usage written consent of the author must be taken and a reference to the author should be provided. 
 * No responsibility will be taken for any loss or damage that will occur as a result of the usage of this code. 
 * 
 * Please feel free to inform me about any bugs, problems, ideas etc.
*/

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;


namespace DXFImporter
{
	/// <summary>
	/// Summary description for Canvas.
	/// </summary>
	public class Canvas : System.Windows.Forms.Form
	{
		private bool multipleSelect = false;
		private bool clicked = false;
				
		private double XMax, XMin;
		private double YMax, YMin;

		private double scaleX = 1;
		private double scaleY = 1;
		private double mainScale = 1;

		private Point aPoint;
		private bool sizeChanged = false;
		
		private Point startPoint;
		private Point endPoint;

		private static Point exPoint;

		private ArrayList drawingList;
		private ArrayList objectIdentifier;

		public bool onCanvas = false;
		private polyline thePolyLine = null;
		
		private bool polyLineStarting = true;
		private bool CanIDraw = false;

		private FileInfo theSourceFile;

		private Rectangle highlightedRegion = new Rectangle (0,0,0,0);


		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Canvas()
		{

			startPoint = new Point (0, 0);
			endPoint = new Point (0, 0);
			exPoint = new Point (0, 0);

			InitializeComponent();

			

			XMax = this.pictureBox1.Size.Width;
			YMax = this.pictureBox1.Size.Height /2;


			
			drawingList = new ArrayList ();
			objectIdentifier = new ArrayList ();


			//.Net Style Double Buffering/////////////////
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			//////////////////////////////////////////////
			//////////////////////////////////////////////

		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}

		            
            drawingList = null;
			objectIdentifier = null;
			base.Dispose(disposing);
		}



		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.pictureBox1.BackColor = System.Drawing.Color.SteelBlue;
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(304, 304);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Visible = false;
			// 
			// Canvas
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.SteelBlue;
			this.ClientSize = new System.Drawing.Size(320, 318);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.pictureBox1});
			this.MinimumSize = new System.Drawing.Size(300, 150);
			this.Name = "Canvas";
			this.ShowInTaskbar = false;
			this.Text = "Canvas";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CanvasRenewed_KeyDown);
			this.SizeChanged += new System.EventHandler(this.OnSizeChanged);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.CanvasRenewed_KeyUp);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseMoveCanvas);
			this.ResumeLayout(false);

		}
		#endregion

		#region Drawing and Highlighting Methods

		public void Draw (Graphics g)
		{
			Pen lePen = new Pen(Color.White, 3);
			
			g.TranslateTransform(this.pictureBox1.Location.X + 1, this.pictureBox1.Location.Y + this.pictureBox1.Size.Height - 1);

			if (YMin < 0)
				g.TranslateTransform(0, - (int)Math.Abs(YMin) );			//transforms point-of-origin to the lower left corner of the canvas.

			if (XMin < 0)
				g.TranslateTransform((int) Math.Abs(XMin), 0);
			
			//	g.SmoothingMode = SmoothingMode.AntiAlias; 

			foreach (DrawingObject obj in objectIdentifier)						//iterates through the objects
			{
				switch (obj.shapeType)
				{
					case 2:				//line
					{
						Line temp = (Line) drawingList[obj.indexNo];

						lePen.Color = temp.AccessContourColor;
						lePen.Width = temp.AccessLineWidth;

						
						highlightedRegion.Location = temp.GetStartPoint;

						highlightedRegion.Width = temp.GetStartPoint.X - temp.GetEndPoint.X;
						highlightedRegion.Height = temp.GetStartPoint.Y - temp.GetEndPoint.Y;

						if (mainScale == 0)
							mainScale = 1;

						temp.Draw(lePen, g, mainScale);

						break;
					}
					case 3:				//rectangle 
					{
						
						rectangle temp = (rectangle) drawingList[obj.indexNo];

						lePen.Color = temp.AccessContourColor;
						lePen.Width = temp.AccessLineWidth;


						temp.Draw(lePen, g);

						break;
					}
					case 4:				//circle
					{

						circle temp = (circle) drawingList[obj.indexNo];
					
						lePen.Color = temp.AccessContourColor;
						lePen.Width = temp.AccessLineWidth;

						if (mainScale == 0)
							mainScale = 1;

						temp.Draw(lePen, g, mainScale);

						break;
					}
					case 5:				//polyline
					{
						polyline temp = (polyline) drawingList[obj.indexNo];
					
						lePen.Color = temp.AccessContourColor;
						lePen.Width = temp.AccessLineWidth;

						if (mainScale == 0)
							mainScale = 1;

						temp.Draw(lePen, g, mainScale);

						break;
					}
					case 6:				//arc
					{
						arc temp = (arc) drawingList[obj.indexNo];

						lePen.Color = temp.AccessContourColor;
						lePen.Width = temp.AccessLineWidth;

						if (mainScale == 0)
							mainScale = 1;

						temp.Draw(lePen, g, mainScale);

						break;
					}
				}				
			}

						
			//	g.Dispose();		//not disposed because "g" is get from the paintbackground event..
			lePen.Dispose();
		}


		public bool HiglightObject ()			//Highlighting the object with mouse
		{
			Graphics daGe = this.CreateGraphics();


			foreach (DrawingObject obj in objectIdentifier)			//iterates through the objects and send the relevant info to the checkLineProximity(...) module
			{
				if (checkLineProximity(obj.indexNo, obj.shapeType, daGe) == true)		
				{
					this.Cursor = Cursors.Cross;

					CanIDraw = true;

					if (multipleSelect == false)
						return true;
				}
							
			}
			return false;

		}



		private bool checkLineProximity(int indexno, int identifier, Graphics daGe)	//checks whether if the mouse pointer is on an object (i.e. shape)
		{
			Graphics g = daGe;
			Pen lePen = new Pen(Color.Yellow, 1);
		
			g = pictureBox1.CreateGraphics();
			
			g.TranslateTransform(this.pictureBox1.Left+8, this.pictureBox1.Size.Height+8);	//transforms point-of-origin to the lower left corner of the canvas.

			g.SmoothingMode = SmoothingMode.HighQuality; 

			switch (identifier)											//depending on the "identifier" value, the relevant object will be highlighted
			{
				case 2:		//Line
				{
					Line line = (Line) drawingList[indexno];
					
					if (mainScale == 0)
						mainScale = 1;

					if (line.Highlight(lePen, g, aPoint, mainScale))
					{
						this.Cursor = Cursors.Hand;
						line.highlighted = true;
						return true;
					}
				
					break;			

				}
				case 3:		//rectangle
				{
					rectangle rect = (rectangle) drawingList[indexno];

					if (rect.Highlight(lePen, g, aPoint))
					{
						this.Cursor = Cursors.Hand;
						rect.highlighted = true;
						return true;
					}

					break;
				}
				case 4:		//circle
				{
					circle tempCircle= (circle) drawingList[indexno];

					if (mainScale == 0)
						mainScale = 1;

					if (tempCircle.Highlight(lePen, g, aPoint, mainScale))
					{
						this.Cursor = Cursors.Hand;
						tempCircle.highlighted = true;
						return true;
					}
				
					break;
				}
				case 5:		//polyline
				{
					polyline tempPoly = (polyline) drawingList[indexno];

					if (mainScale == 0)
						mainScale = 1;

					if (tempPoly.Highlight(lePen, g, aPoint, mainScale))
					{
						this.Cursor = Cursors.Hand;
						tempPoly.highlighted = true;
						return true;
					}
					break;
				}
				case 6:		//arc
				{
					arc tempArc = (arc) drawingList[indexno];

					if (mainScale == 0)
						mainScale = 1;

					if (tempArc.Highlight(lePen, g, aPoint, mainScale))
					{
						this.Cursor = Cursors.Hand;
						tempArc.highlighted = true;
						return true;
					}
					break;
				}
			}

			return false;
		}




		#endregion

		#region Helper Methods


		private double CalculateRadius()		//this helper function is used to calculate the radius for the circle-drawing mode.
		{
			double circleRadius = Math.Sqrt( (endPoint.X - startPoint.X)*(endPoint.X - startPoint.X) + (endPoint.Y - startPoint.Y)*(endPoint.Y - startPoint.Y) );
			return circleRadius;
		}


		public void RecalculateScale()
		{
			if (XMax > this.pictureBox1.Size.Width)
				scaleX = (double) (this.pictureBox1.Size.Width) / (double) XMax;
			
			if (YMax > this.pictureBox1.Size.Height)
				scaleY = (double) (this.pictureBox1.Size.Height) / (double) YMax;
			
			mainScale = Math.Min(scaleX, scaleY);
		}

		protected override void DefWndProc(ref Message m)		//DefWndProc is overriden to capture left mouse click on the title bar of the canvas...
		{

			
			const int WM_NCLBUTTONDOWN = 0x0A1;
			const int WM_NCLBUTTONUP = 0x0A0;
			const int WM_STYLECHANGED = 0x07D;

			
			switch (m.Msg)
			{
					
				case WM_NCLBUTTONDOWN:
				{

					clicked = true;
					break;
				}
				case WM_NCLBUTTONUP:
				{

					break;
				}

			}
            
			base.DefWndProc(ref m);
			
		}


		#endregion

		#region DXF Data Extraction and Interpretation

		public void ReadFromFile (string textFile)			//Reads a text file (in fact a DXF file) for importing an Autocad drawing.
															//In the DXF File structure, data is stored in two-line groupings ( or bi-line, coupling line ...whatever you call it)
															//in this grouping the first line defines the data, the second line contains the data value.
															//..as a result there is always even number of lines in the DXF file..
		{
			string line1, line2;							//these line1 and line2 is used for getting the a/m data groups...

			line1 = "0";									//line1 and line2 are are initialized here...
			line2 = "0";

			long position = 0;

			theSourceFile = new FileInfo (textFile);		//the sourceFile is set.

			StreamReader reader = null;						//a reader is prepared...

			try
			{
				reader = theSourceFile.OpenText();			//the reader is set ...
			}
			catch (FileNotFoundException e)
			{
				MessageBox.Show(e.FileName.ToString() + " cannot be found");
			}
			catch
			{
				MessageBox.Show("An error occured while opening the DXF file");
				return;
			}


			

			do
			{
				////////////////////////////////////////////////////////////////////
				//This part interpretes the drawing objects found in the DXF file...
				////////////////////////////////////////////////////////////////////

				if (line1 == "0" && line2 == "LINE")
					LineModule(reader);

				else if (line1 == "0" && line2 == "LWPOLYLINE")
					PolylineModule(reader);

				else if (line1 == "0" && line2 == "CIRCLE")
					CircleModule(reader);

				else if (line1 == "0" && line2 == "ARC")
					ArcModule(reader);

				////////////////////////////////////////////////////////////////////
				////////////////////////////////////////////////////////////////////


				GetLineCouple (reader, out line1, out line2);		//the related method is called for iterating through the text file and assigning values to line1 and line2...
				
			}
			while (line2 != "EOF");



			reader.DiscardBufferedData();							//reader is cleared...
			theSourceFile = null;
			

			reader.Close();											//...and closed.

		}


		private void GetLineCouple (StreamReader theReader, out string line1, out string line2)		//this method is used to iterate through the text file and assign values to line1 and line2
		{
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            string decimalSeparator = ci.NumberFormat.CurrencyDecimalSeparator;

			line1 = line2 = "";

			if (theReader == null)
				return;
            
			line1 = theReader.ReadLine();
			if (line1 != null)
			{
				line1 = line1.Trim();
                line1 = line1.Replace('.', decimalSeparator[0]);

			}
			line2 = theReader.ReadLine();
			if (line2 != null)
			{
				line2 = line2.Trim();
                line2 = line2.Replace('.', decimalSeparator[0]);
			}
		}


		private void LineModule (StreamReader reader)		//Interpretes line objects in the DXF file
		{
			string line1, line2;
			line1 = "0";
			line2 = "0";

			double x1= 0;
			double y1 = 0;
			double x2= 0;
			double y2 = 0;

			do
			{
				GetLineCouple (reader, out line1, out line2);

				if (line1 == "10")
				{
					x1 = Convert.ToDouble(line2); 
					
					if (x1>XMax)
						XMax = x1;

					if (x1 < XMin)
						XMin = x1;
				}
                
				if (line1 == "20")
				{
					y1 = Convert.ToDouble(line2); 
					if (y1 > YMax)
						YMax = y1;

					if (y1 < YMin)
						YMin = y1;
				}

				if (line1 == "11")
				{
					x2 = Convert.ToDouble(line2); 

					if (x2 > XMax)
						XMax = x2;

					if (x2 < XMin)
						XMin = x2;
				}
				
				if (line1 == "21")
				{
					y2 = Convert.ToDouble(line2); 
					
					if (y2 > YMax)
						YMax = y2;

					if (y2 < YMin)
						YMin = y2;
				}

				
			}
			while (line1 != "21");

			
		
			//****************************************************************************************************//
			//***************This Part is related with the drawing editor...the data taken from the dxf file******//
			//***************is interpreted hereinafter***********************************************************//

			if ((Math.Abs(XMax) - Math.Abs(XMin)) > this.pictureBox1.Size.Width)
			{
				scaleX = (double) (this.pictureBox1.Size.Width) / (double) (Math.Abs(XMax) - Math.Abs(XMin));
			}
			else
				scaleX = 1;


			if ((Math.Abs(YMax) - Math.Abs(YMin)) > this.pictureBox1.Size.Height)
			{
				scaleY = (double) (this.pictureBox1.Size.Height) / (double) (Math.Abs(YMax) - Math.Abs(YMin));
			}
			else
				scaleY = 1;

			mainScale = Math.Min(scaleX, scaleY);

			


			int ix = drawingList.Add(new Line (new Point((int)x1, (int) -y1), new Point((int)x2, (int)-y2) , Color.White, 1));
			objectIdentifier.Add (new DrawingObject (2, ix));

			///////////////////////////////////////////////////////////////////////////////////////////////////////
			///////////////////////////////////////////////////////////////////////////////////////////////////////
			
		}


		private void PolylineModule (StreamReader reader)	//Interpretes polyline objects in the DXF file
		{
			string line1, line2;
			line1 = "0";
			line2 = "0";

			double x1= 0;
			double y1 = 0;
			double x2= 0;
			double y2 = 0;
            			

			thePolyLine = new polyline (Color.White, 1);
			
			int ix = drawingList.Add(thePolyLine);
			objectIdentifier.Add (new DrawingObject (5, ix));

			int counter = 0;
			int numberOfVertices = 1;
			int openOrClosed = 0;
			ArrayList pointList = new ArrayList();
            
			
			do
			{
				GetLineCouple (reader, out line1, out line2);

				if (line1 == "90")
					numberOfVertices = Convert.ToInt32(line2);

				if (line1 == "70")
					openOrClosed = Convert.ToInt32(line2);
				

				if (line1 == "10")
				{
					x1 = Convert.ToDouble(line2); 
					if (x1 > XMax)
						XMax = x1;

					if	(x1 < XMin)
						XMin = x1;
				}
                
				if (line1 == "20")
				{
					y1 = Convert.ToDouble(line2); 
				
					if (y1 > YMax)
						YMax = y1;

					if (y1 < YMin)
						YMin = y1;

					pointList.Add(new Point((int)x1, (int)-y1));
					counter++;
				}

			}
			while(counter < numberOfVertices);
				
			//****************************************************************************************************//
			//***************This Part is related with the drawing editor...the data taken from the dxf file******//
			//***************is interpreted hereinafter***********************************************************//


			for (int i = 1; i<numberOfVertices; i++)
			{
				thePolyLine.AppendLine (new Line ( (Point)pointList[i-1], (Point)pointList[i],Color.White, 1));
			}

			if (openOrClosed == 1)
				thePolyLine.AppendLine (new Line ( (Point)pointList[numberOfVertices-1], (Point)pointList[0],Color.White, 1));

			if ((Math.Abs(XMax) - Math.Abs(XMin)) > this.pictureBox1.Size.Width)
			{
				scaleX = (double) (this.pictureBox1.Size.Width) / (double) (Math.Abs(XMax) - Math.Abs(XMin));
			}
			else
				scaleX = 1;


			if ((Math.Abs(YMax) - Math.Abs(YMin)) > this.pictureBox1.Size.Height)
			{
				scaleY = (double) (this.pictureBox1.Size.Height) / (double) (Math.Abs(YMax) - Math.Abs(YMin));
			}
			else
				scaleY = 1;

			mainScale = Math.Min(scaleX, scaleY);

			//////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////


		}


		private void CircleModule (StreamReader reader)		//Interpretes circle objects in the DXF file
		{
			string line1, line2;
			line1 = "0";
			line2 = "0";

			double x1= 0;
			double y1 = 0;

			double radius = 0;

			do
			{
				GetLineCouple (reader, out line1, out line2);

				if (line1 == "10")
				{
					x1 = Convert.ToDouble(line2);
					
				}


				if (line1 == "20")
				{
					y1 = Convert.ToDouble(line2);
					
				}


				if (line1 == "40")
				{
					radius = Convert.ToDouble(line2);

					if ( (x1 + radius) > XMax)
						XMax = x1 + radius;

					if ( (x1 - radius) < XMin)
						XMin = x1 - radius;

					if (y1 + radius > YMax)
						YMax = y1 + radius;

					if ( (y1 - radius) < YMin)
						YMin = y1 - radius;

				}



			}
			while(line1 != "40");

			//****************************************************************************************************//
			//***************This Part is related with the drawing editor...the data taken from the dxf file******//
			//***************is interpreted hereinafter***********************************************************//


			if ((Math.Abs(XMax) - Math.Abs(XMin)) > this.pictureBox1.Size.Width)
			{
				scaleX = (double) (this.pictureBox1.Size.Width) / (double) (Math.Abs(XMax) - Math.Abs(XMin));
			}
			else
				scaleX = 1;


			if ((Math.Abs(YMax) - Math.Abs(YMin)) > this.pictureBox1.Size.Height)
			{
				scaleY = (double) (this.pictureBox1.Size.Height) / (double) (Math.Abs(YMax) - Math.Abs(YMin));
			}
			else
				scaleY = 1;

			mainScale = Math.Min(scaleX, scaleY);


			int ix = drawingList.Add(new circle (new Point ((int)x1, (int)-y1), radius, Color.White, Color.Red, 1));
			objectIdentifier.Add (new DrawingObject (4, ix));

			//////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////
			
		}


		private void ArcModule (StreamReader reader)		//Interpretes arc objects in the DXF file
		{
			string line1, line2;
			line1 = "0";
			line2 = "0";

			double x1= 0;
			double y1 = 0;

			double radius = 0;
			double angle1 = 0;
			double angle2 = 0;

			do
			{
				GetLineCouple (reader, out line1, out line2);

				if (line1 == "10")
				{
					x1 = Convert.ToDouble(line2);
					if (x1 > XMax)
						XMax = x1;
					if (x1 < XMin)
						XMin = x1;

				}


				if (line1 == "20")
				{
					y1 = Convert.ToDouble(line2);
					if (y1 > YMax)
						YMax = y1;
					if (y1 < YMin)
						YMin = y1;
				}


				if (line1 == "40")
				{
					radius = Convert.ToDouble(line2);

					if ( (x1 + radius) > XMax)
						XMax = x1 + radius;

					if ( (x1 - radius) < XMin)
						XMin = x1 - radius;

					if (y1 + radius > YMax)
						YMax = y1 + radius;

					if ( (y1 - radius) < YMin)
						YMin = y1 - radius;
				}

				if (line1 == "50")
					angle1 = Convert.ToDouble(line2);

				if (line1 == "51")
					angle2 = Convert.ToDouble(line2);


			}
			while(line1 != "51");


			//****************************************************************************************************//
			//***************This Part is related with the drawing editor...the data taken from the dxf file******//
			//***************is interpreted hereinafter***********************************************************//


			if ((Math.Abs(XMax) - Math.Abs(XMin)) > this.pictureBox1.Size.Width)
			{
				scaleX = (double) (this.pictureBox1.Size.Width) / (double) (Math.Abs(XMax) - Math.Abs(XMin));
			}
			else
				scaleX = 1;


			if ((Math.Abs(YMax) - Math.Abs(YMin)) > this.pictureBox1.Size.Height)
			{
				scaleY = (double) (this.pictureBox1.Size.Height) / (double) (Math.Abs(YMax) - Math.Abs(YMin));
			}
			else
				scaleY = 1;

			mainScale = Math.Min(scaleX, scaleY);


			int ix = drawingList.Add(new arc (new Point ((int)x1, (int)-y1), radius, angle1, angle2, Color.White, Color.Red, 1));
			objectIdentifier.Add (new DrawingObject (6, ix));

			//////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////

		}


		#endregion

		#region Events
		
		private void OnSizeChanged(object sender, System.EventArgs e)
		{
			
			RecalculateScale();
			
			Refresh();
			
			sizeChanged = true;

			
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs e)		//all drawing is made here in OnPaintBackground...
		{
			
			base.OnPaintBackground(e);

			if (this.WindowState == FormWindowState.Minimized)
				return;

            
			Graphics g = e.Graphics;
		
			Rectangle rect = new Rectangle(this.pictureBox1.Location, this.pictureBox1.Size);
			

			System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
																											rect, 
																											Color.SteelBlue, 
																											Color.Black, 
																											System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);


			if (this.WindowState != FormWindowState.Minimized)
			{
				e.Graphics.FillRectangle(brush, rect);
				
				Draw(g);				//All drawing is made here...
			}
		
			g = null;
			brush.Dispose();
		}


		protected override void OnResize(EventArgs e)
		{
			
			if (this.Width < 500) 
			{
				this.Width = 500;
				return;
			}
			if (this.Height < 400)
			{
				this.Height = 400;
				return;
			}
			
            
			base.OnResize(e);
            
		}





		private void CanvasRenewed_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.Shift)
				multipleSelect = true;

		
		}

		private void CanvasRenewed_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			multipleSelect = false;
		
		}


		#endregion

		#region Mouse Events



		private void MouseMoveCanvas(object sender, System.Windows.Forms.MouseEventArgs e)		//mousemove event...while the "shift" button is pressed down, the shapes can be highlighted...
		{
			aPoint.X = e.X - this.pictureBox1.Location.X - (int) Math.Abs(XMin) - 1 ;
			aPoint.Y = e.Y - this.pictureBox1.Location.Y - this.pictureBox1.Size.Height + (int) Math.Abs(YMin) + 1;

			Rectangle rect = this.pictureBox1.ClientRectangle;
        
			if (rect.Contains(new Point(e.X - this.pictureBox1.Location.X, e.Y - this.pictureBox1.Location.Y)))
			{
				this.Cursor = Cursors.Cross;
				onCanvas = true;
			}
			else
			{
				this.Cursor = Cursors.Arrow;
				onCanvas = false;
			}

			if (onCanvas == true)
			{
				if (multipleSelect)
					HiglightObject();
				
				Refresh();
				
                	
			}
		}


		#endregion
	}
}
