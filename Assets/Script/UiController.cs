using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiController : MonoBehaviour {
	// prefabs of ui objects
	public Transform uiSelectionPlane;
	public Transform[] uiCirclePlanes = new Transform[5];
    public GameObject textBox;
    public Text textVal;
	public string builtEq;
	GraphData eqInfo;
    public GameObject manageSource;
    uiManager manager;


	// transparency and scale config
	float[] uiAlphas = new float[] {0.0f, 0.2f, 0.9f, 0.3f, 0.0f};
	float[] uiScales = new float[] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f}; //{4.0f, 2.0f, 1.0f, 0.5f, 0.25f};
	float[] uiZpos = new float[] {-10.0f, -5.0f, 0.0f, 5.0f, 10.0f};

	// manager for instanced ui objects
	Transform[] circlePlanes = new Transform[5];
	Transform selectionPlane;
	Renderer selectionPlaneRenderer;

	// controller button config
	string confirmBtn = "Fire1";
	string cancelBtn = "Fire2";
	string buildBtn = "Fire3";
	string leftBtn = "LeftBumper";
	string rightBtn = "RightBumper";
	string startBtn = "StartBtn";

	// joystick config
	string xAxisName = "LA_h";
	string yAxisName = "LA_v";
	Vector2 joystickAngleReference = Vector2.up;
	// joystick internal variables
	Vector2 joystickRaw = Vector2.zero;
	float joystickAngle = 0.0f;
	bool isSelected = false;

	// animation config
	float animationDuration = 0.5f;
	// animation internal variables
	float animationStartTime = 0.0f;
	bool isAnimating = false;

	void Start () {
		SetupUi();
		OVRDevice.ResetOrientation ();
		// gets the renderer of the child, parent is an empty controller
		selectionPlaneRenderer = selectionPlane.GetChild(0).renderer;
		// get instance of output box.
        manager = (uiManager)manageSource.GetComponent(typeof(uiManager));
        textVal = textBox.GetComponent<Text>();
        textVal.text = "";
        eqInfo = GraphData.gd;
        transform.parent.gameObject.SetActive(false);

	}

	//make sure to keep the data from this scene (IE the GraphData Class)
    public void becomeActive()
    {
        if (GraphData.gd.EditVar == 0)
            builtEq = GraphData.gd.Fn;
        else if (GraphData.gd.EditVar == 1)
            builtEq = GraphData.gd.MinX.ToString();
        else if (GraphData.gd.EditVar == 2)
            builtEq = GraphData.gd.MinY.ToString();
        else if (GraphData.gd.EditVar == 3)
            builtEq = GraphData.gd.MinZ.ToString();
        else if (GraphData.gd.EditVar == 4)
            builtEq = GraphData.gd.MaxX.ToString();
        else if (GraphData.gd.EditVar == 5)
            builtEq = GraphData.gd.MaxY.ToString();
        else if (GraphData.gd.EditVar == 6)
            builtEq = GraphData.gd.MaxZ.ToString();

        textVal.text = builtEq;
    }
	void Update () {
		if (!isAnimating) {
			// joystick input
			joystickRaw.x = Input.GetAxis(xAxisName);
			joystickRaw.y = Input.GetAxis(yAxisName);

			if ( joystickRaw != Vector2.zero ) {
				isSelected = true;
				joystickAngle = GetJoyAngle(joystickRaw);
				selectionPlane.localEulerAngles = new Vector3(0, 0, joystickAngle);
			} else {
				isSelected = false;
			}

			// button input
			// Press 'X' to view the equation.
			// Press 'A' to build the equation and send it to the GUI viewer
			if (isSelected && Input.GetButtonDown(confirmBtn)) {
				builtEq = UpdateEquation (textVal.text,circlePlanes[2].name.ToString(),1);
				textVal.text = builtEq;
			}//Press 'B' to delete the equation one character at a time.
			if (Input.GetButtonDown(cancelBtn)) {
				builtEq = UpdateEquation (textVal.text,circlePlanes[2].name.ToString(),0);
				textVal.text = builtEq;
			} //Press 'start' to build graph data object and go to graphing scene
			if (Input.GetButtonDown(buildBtn)) {
				if(GraphData.gd.EditVar == 0)
					GraphData.gd.Fn = builtEq;
				else if(GraphData.gd.EditVar == 1)
                    GraphData.gd.MinX = float.Parse(builtEq);
				else if(GraphData.gd.EditVar == 2)
                    GraphData.gd.MinY = float.Parse(builtEq);
				else if(GraphData.gd.EditVar == 3)
                    GraphData.gd.MinZ = float.Parse(builtEq);
				else if(GraphData.gd.EditVar == 4)
                    GraphData.gd.MaxX = float.Parse(builtEq);
				else if(GraphData.gd.EditVar == 5)
                    GraphData.gd.MaxY = float.Parse(builtEq);
				else if(GraphData.gd.EditVar == 6)
                    GraphData.gd.MaxZ = float.Parse(builtEq);

				//Application.LoadLevel(1);
                manager.circleOff();

			}
			if (Input.GetButtonDown(leftBtn)) {
				CycleCarousel(-1);
				isSelected = false;
			}
			if (Input.GetButtonDown(rightBtn)) {
				CycleCarousel(1);
				isSelected = false;
			}

			selectionPlaneRenderer.enabled = isSelected;

		} else if( Time.time > animationStartTime + animationDuration) {
			isAnimating = false;
		}
	}

	/****************************************
	 * 
	 *	creates all ui objects 
	 * 
	 ****************************************/
	void SetupUi () {



		// create circle objects
		for (var i = 0; i < circlePlanes.Length; i++) {
			circlePlanes[i] = (Transform)Instantiate(uiCirclePlanes[i]);
			circlePlanes[i].transform.parent = transform;
			circlePlanes[i].SendMessage("SetAlpha", uiAlphas[i]);
			circlePlanes[i].SendMessage("SetScale", uiScales[i]);
			circlePlanes[i].SendMessage("SetZPos", uiZpos[i]);
			circlePlanes[i].SendMessage("SetAnimationDuration", animationDuration);
		}
		// create selection object
		Vector3 selectionPlaneLocation = circlePlanes[2].position;
		selectionPlaneLocation.z = selectionPlaneLocation.z-.2f;

		selectionPlane = (Transform)Instantiate(uiSelectionPlane, 
		                                        selectionPlaneLocation, 
		                                        transform.rotation);
		selectionPlane.transform.parent = transform;

		// rotate root object to correctly orient children
		transform.localRotation = Quaternion.identity;
	}

	/****************************************
	 * 
	 *	returns angle as value 0, 36, 72...324 (counter-clockwise) with respects to joystickAngleReference
	 * 
	 ****************************************/
	float GetJoyAngle (Vector2 rawJoy) {
		float angle = Vector2.Angle(joystickAngleReference, rawJoy);

		// determine if angle should be 0 - 180 or 180 - 360
		Vector3 direction = Vector3.Cross(joystickAngleReference, rawJoy);
		angle = (direction.z < 0) ? 360 - angle : angle;

		return angle - (angle % 36);
	}

	/****************************************
	 * 
	 *	 cycles the circle carousel, increment should be 1 or -1
	 * 
	 ****************************************/
	void CycleCarousel ( int increment ) {
		Transform[] tempArray = new Transform[circlePlanes.Length];
		// send all circle objects new transparency and scale
		for (int i = 0; i < circlePlanes.Length; i++) {
			int newPos = i + increment;
			// check for edge cases
			if (newPos > circlePlanes.Length - 1) {
				newPos = 0;
			} else if (newPos < 0) {
				newPos = circlePlanes.Length - 1;
			}
			circlePlanes[i].SendMessage("AnimateAlpha", uiAlphas[newPos]);
			circlePlanes[i].SendMessage("AnimateScale", uiScales[newPos]);
			circlePlanes[i].SendMessage("AnimateZPos", uiZpos[newPos]);
			tempArray[newPos] = circlePlanes[i];
		}
		circlePlanes = tempArray;
		animationStartTime = Time.time;
		isAnimating = true;
	}

	/****************************************
	 * 
	 *	 Upon pressing A, get the correct input into the GUI window.
	 * 
	 ****************************************/
	string UpdateEquation(string equationString,string name, int updateType){
		if (updateType == 0){
			equationString = equationString.Substring(0,equationString.Length - 1);
			return equationString;
		}
		else{
			if(name == "UiSymbols(Clone)"){
				int choice = (int)(joystickAngle / 36);
				switch (choice)
				{
				case 0:
					equationString += "exp(";
					break;
				case 1:
					equationString += "pi";
					break;
				default:
					break;
				}
				return equationString;
			}
			else if(name == "UiNumbers(Clone)"){
				int choice = (int)(joystickAngle / 36);
				return equationString += choice;
			}
			else if(name == "UiConstants(Clone)"){
				int choice = (int)(joystickAngle / 36);
				switch (choice)
				{
				case 0:
					equationString += "+";
					break;
				case 1:
					equationString += "-";
					break;
				case 2:
					equationString += "*";
					break;
				case 3:
					equationString += "/";
					break;
				case 4:
					equationString += "(";
					break;
				case 5:
					equationString += ")";
					break;
				case 6:
					equationString += "^";
					break;
				case 7:
					equationString += "sqrt(";
					break;
				case 9:
					equationString += ".";
					break;
				default:
					break;
				}
				return equationString;
			}
			else if(name == "UiVars(Clone)"){
				int choice = (int)(joystickAngle / 36);
				switch (choice)
				{
				case 0:
					equationString += "x";
					break;
				case 1:
					equationString += "y";
					break;
				default:
					break;
				}
				return equationString;
			}
			else if(name == "UiTrig(Clone)"){
				int choice = (int)(joystickAngle / 36);
				switch (choice)
				{
				case 0:
					equationString += "sin(";
					break;
				case 1:
					equationString += "cos(";
					break;
				case 2:
					equationString += "tan(";
					break;
				default:
					break;
				}
				return equationString;
			}
			else{return "'" + name + "'";}

		}
	}


}
