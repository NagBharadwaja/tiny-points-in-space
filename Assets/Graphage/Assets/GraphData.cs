using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System;


namespace Graphing
{
	public class GraphData{
		
		private float minX;  // lower range for X
		private float maxX;  // upper range for X
		private float minY;  // lower range for Y
		private float maxY;  // upper range for Y
		private float maxZ;
		private float minZ;
		private int res;  	  // fidelity of graph
		private string fn;    // fn concatenated to one string
		private float x0;    // x coord to evaluate
		private float y0;    // y coord to evaluate
		private float x1;	  // x coord used in integration
		private float y1;	  // y coord used in integration

		private int evalChoice;//integer representation of what the python script is supposed to evaluate for
		/* 1 = regular graph
		 * 2 = graph indefinite integral w/ respect to x
		 * 3 = graph indefinite integral w/ respect to y
		 * 4 = graph indefinite integral w/ respect to x and y (aka map the new graph of the integral)
		 * 5 = graph definite integral w/ respect to x
		 * 6 = graph definite integral w/ respect to y
		 * 7 = graph definite integral w/ respect to x and y
		 * 8 = graph tangent line at point w/ respect to x
		 * 9 = graph tangent line at point w/ respect to y
		 * 10 = graph derivative w/ respect to x
		 * 11 = graph derivative w/ respect to y
		 * 12 = graph derivative w/ respect to x and y */


		// get/set properties for the variables
		public float MinX {
			get { return minX; }
			set { minX = value; }
		}
		public float MaxX {
			get { return maxX; }
			set { maxX = value; }
		}
		public float MinY {
			get { return minY; }
			set { minY = value; }
		}
		public float MaxY {
			get { return maxY; }
			set { maxY = value; }
		}
		public float MinZ {
			get { return minZ; }
			set { minZ = value; }
		}
		public float MaxZ {
			get { return maxZ; }
			set { maxZ = value; }
		}
		public int Res {
			get { return res; }
			set { res = value; }
		}
		public string Fn {
			get { return fn; }
			set { fn = value;}
		}
		public float X0 {
			get { return x0; }
			set { x0 = value;}
		}
		public float Y0 {
			get { return y0; }
			set { y0 = value;}
		}
		
		public float X1 {
			get { return x1; }
			set { x1 = value;}
		}
		public float Y1 {
			get { return y1; }
			set { y1 = value;}
		}
		public int EvalChoice {
			get { return evalChoice; }
			set { evalChoice = value; }
		}


		//default constructor 
		public GraphData() {
			minX = -1.0f;
		    maxX = 1.0f;
		    minY = -1.0f;
		    maxY = 1.0f;
		    res = 10;
			fn = "";
			x0 = -999;
			y0 = 999;
			x1 = -999;
			y1 = 999;
		}

		// public function that converts the class into a string that can be passed
		// to python as an argument list. 
		public string toArgList(){
			string pythonCommand;

			pythonCommand = getPythonScript () + " "
							+ minX + " "
							+ maxX + " "
							+ minY + " "
							+ maxY + " "
							+ res + " "
							+ fn + " ";

			if (x0 != -999) {  					//if x0,y0,y1,x1 were included in the class
				pythonCommand += x0 + " ";     //attach them to the python-execute string
			}
			if (y0 != 999) {				
				pythonCommand += y0 + " ";
			}
			if (x1 != -999) {  
				pythonCommand += x1 + " ";    
			}
			if (y1 != -999) {
				pythonCommand += y1 + " ";
			}
			
			return pythonCommand;

		}

		//converts the evaluation choice from an integer to the name of a python script.
		private string getPythonScript()
		{
			switch (evalChoice) {
			case 1: 
				return "calculateZ.py";
			case 2:
				return "integral.py";
			case 3:
				return "integral.py";
			case 4:
				return "integral.py";
			case 5:
				return "integralX.py";
			case 6:
				return "integralY.py";
			case 7:
				return "integralXY.py";
			case 8:
				return "tangentX.py";
			case 9:
				return "tangentY.py";
			case 10:
				return "derivX.py";
			case 11:
				return "derivY.py";
			case 12:
				return "derivXY.py";	
			default:
				return "ERROR";
			}			
		}
	}
}

