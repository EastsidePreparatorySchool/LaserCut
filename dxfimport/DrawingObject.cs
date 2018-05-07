using System;

namespace LaserCut
{
	/// <summary>
	/// Summary description for DrawingObject.
	/// </summary>
	public class DrawingObject
	{
		public int shapeType;
		public int indexNo;

		public DrawingObject (int shapeID, int ix)
		{
			shapeType = shapeID;
			indexNo = ix;
			
		}
	}
}
