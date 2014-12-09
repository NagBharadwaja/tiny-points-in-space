using UnityEngine;
using System.Collections;

public class drawLine : MonoBehaviour {
	private LineRenderer line;

	//load the new line to draw
	public void lineDraw(Vector3 pos1, Vector3 pos2)
	{
		line = GetComponent<LineRenderer>();
		//line.enabled = true;
		line.SetPosition (0, pos1);
		line.SetPosition (1, pos2);
	}
	//disable the line
	public void enableLine(bool val)
	{
		line = GetComponent<LineRenderer>();
		line.enabled = val;
	}
	// Use this for initialization
	void Start () 
	{
		//line = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
