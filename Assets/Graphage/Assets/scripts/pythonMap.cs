using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;


/* This script assumes A: the python path variable is set and
 * B: The user enters the function correctly*/

public class pythonMap : MonoBehaviour {

	//these are used for the debug submissions and are not required


	//status text and status bool to confirm that status should be updated in the ui
	public string statusText;
	private bool statusUpdate=false;
    private GraphData data;

	public string LowerX = "1";		//lower x boundary
	public string UpperX = "2";		//upper x boundary
	public string LowerY = "3";		//lower y boundary
	public string UpperY = "4";		//upper y boundary
	public string UpperZ = "100";
	public string LowerZ = "-100";

	public string res = "10";		//resolution (points to draw per dimension)

	public string function = "x+y";	//function to send in to sympy

	//the graph object
	public GameObject graph;
	public GameObject plane1;
	public GameObject plane2;
	public GameObject plane3;

	private ProcGrid grid1;
	private ProcGrid grid2;
	private ProcGrid grid3;
	//the backgroundworker
	private BackgroundWorker bw = new BackgroundWorker();

	//upgrade the graph!
	private bool updateGraph = false;

	private List<Vector3> verts= new List<Vector3>();


	private void Start()
	{
        data = GraphData.gd;
		grid1=(ProcGrid)plane1.GetComponent(typeof(ProcGrid));
		grid2=(ProcGrid)plane2.GetComponent(typeof(ProcGrid));
		grid3=(ProcGrid)plane3.GetComponent(typeof(ProcGrid));
		//add a listener for the inputfield and button. you can remove it if you have to

		//prepare the backgroundworker for battle!
		bw.WorkerReportsProgress = true;
		bw.DoWork += new DoWorkEventHandler (bw_DoWork);
		bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
		bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler (bw_RunWorkerCompleted);
        runTest();
	}

	//this is used for the submitbutton, remove if you need to
	private void SubmitFunction(string func)
	{
		function = func;
		runTest ();
	}

	//progress changed from worker.ReportProgress calls
	private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		if(e.ProgressPercentage==1)
		{
			statusText="computing equation";
		} else if(e.ProgressPercentage==2)
		{
			statusText="waiting for Sympy";
		} else if(e.ProgressPercentage==3)
		{
			statusText="parsing data";
		}
		statusUpdate = true;
	}

	private void bw_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker worker = sender as BackgroundWorker;
		worker.ReportProgress(1);
		try
		{
		Process myProcess = new Process();
		myProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		myProcess.StartInfo.CreateNoWindow = true; 					//hide new process
		myProcess.StartInfo.UseShellExecute = false;
		myProcess.StartInfo.FileName = "python"; 					//process executable
        myProcess.StartInfo.Arguments = data.toArgList(); //"calculateZ.py " + LowerX + " " + UpperX + " " + LowerY + " " + UpperY + " " + res + " " + function;
		myProcess.EnableRaisingEvents = true;
		worker.ReportProgress(2);
		myProcess.Start(); 											//start
		myProcess.WaitForExit();//wait for exit
		int ExitCode = myProcess.ExitCode;
		
		}	catch (Exception ex){
		print(ex);       
	}
		worker.ReportProgress(3);
		verts= new List<Vector3>();
		string[] vertImport= (File.ReadAllLines ("temp.txt"));
		//print (vertImport.Length);

		//we must check for unrenderables
		for (int i=0; i<vertImport.Length; i++) 
		{
			string[] coords=vertImport[i].Split(","[0]);
			if(coords[2]=="nan"||coords[2]=="zoo"||coords[2]=="+inf"||coords[2]=="-inf")
			{
				verts.Add(new Vector3(float.Parse (coords[0]),Mathf.Infinity,float.Parse (coords[1])));
			} else {

				verts.Add(new Vector3(float.Parse (coords[0]),float.Parse (coords[2]),float.Parse (coords[1])));
			}
		}
	}
	private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		//print ("ended");
		updateGraph = true;
	}

	//i dont remember why this is its own thing.
	void runTest () {
		bw.RunWorkerAsync ();
	}
	
	void Update () {
	
		if (Input.GetButtonDown ("SelectBtn")) {
			Application.LoadLevel(1);
		}


		if(updateGraph)
		{
			updateGraph=false;
			//data.MinZ=float.Parse(LowerZ);
			//data.MinY=float.Parse(LowerY);
			//data.MinX=float.Parse(LowerX);
			//data.MaxZ=float.Parse(UpperZ);
			//data.MaxY=float.Parse(UpperY);
			//data.MaxX=float.Parse(UpperX);
			//data.Fn=function;
			//data.Res=int.Parse (res);
			ProcGraph script =(ProcGraph)graph.GetComponent(typeof(ProcGraph));
			script.createMesh(verts,data);
			float mY=(Mathf.Abs (data.MinY)>Mathf.Abs (data.MaxY) ? Mathf.Abs (data.MinY) : Mathf.Abs (data.MaxY));
			float mX=(Mathf.Abs (data.MinX)>Mathf.Abs (data.MaxX) ? Mathf.Abs (data.MinX) : Mathf.Abs (data.MaxX));
			float m = mX>mY ? mX: mY;
			grid1.rebuildMesh(2*(int)m+1);
			grid2.rebuildMesh(2*(int)m+1);
			grid3.rebuildMesh(2*(int)m+1);
		}
		if(statusUpdate)
		{
			statusUpdate=false;
		//	statusfield.text=statusText;
            print(statusText);
		}

	}
}
