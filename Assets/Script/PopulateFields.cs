using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopulateFields : MonoBehaviour {

	GraphData graphData;
	Text txt;
	public GameObject eq;
	public GameObject minx;
	public GameObject miny;
	public GameObject minz;
	public GameObject maxx;
	public GameObject maxy;
	public GameObject maxz;


	// Use this for initialization
	void Start () {
		graphData = GraphData.gd;
	}
	
	// Update is called once per frame
	void Update () {
        if (graphData != null)
        {
            txt = eq.GetComponent<Text>();
            txt.text = graphData.Fn;

            txt = minx.GetComponent<Text>();
            txt.text = graphData.MinX.ToString();

            txt = miny.GetComponent<Text>();
            txt.text = graphData.MinY.ToString();

            txt = minz.GetComponent<Text>();
            txt.text = graphData.MinZ.ToString();

            txt = maxx.GetComponent<Text>();
            txt.text = graphData.MaxX.ToString();

            txt = maxy.GetComponent<Text>();
            txt.text = graphData.MaxY.ToString();

            txt = maxz.GetComponent<Text>();
            txt.text = graphData.MaxZ.ToString();
        }

	}
}
