#pragma strict

var mytime : float = 0.0;
var mytwo : float = 0.0;

function Start () {

}

function Update () {
	mytime++;
}

function OnGUI () {
	mytwo++;
	GUI.Label(Rect(0,0,200,20), mytime.ToString());
	GUI.Label(Rect(0,30,200,20), mytwo.ToString());
}