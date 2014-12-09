using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class uiManager : MonoBehaviour {

    public GameObject[] ScreenList;
    public GameObject controllerSource;
    private UiController controller;
    public GameObject circles;
    private int textEntrySource = 0;
    private int currentScreen;
    private Animator[] animatorList;
    private List<Button> buttonList;
    private EventSystem es;


    private void Start() {
        currentScreen = 0;

        // build collections of animators and buttons
        animatorList = new Animator[ScreenList.Length];
        buttonList = new List<Button>();
        controller = (UiController)controllerSource.GetComponent(typeof(UiController));
        for (int i = 0; i < ScreenList.Length; i++) {
            animatorList[i] = ScreenList[i].GetComponent<Animator>();
            buttonList.AddRange(ScreenList[i].GetComponentsInChildren<Button>());
        }

        // find event system
        es = GameObject.FindGameObjectWithTag("EventSystem").GetComponent<EventSystem>();
 
        // set initial button
        SetSelectedButton();
    }

    public void ChangeScreen(int id) {
        foreach (Button b in buttonList) {
            if (b.transform.parent.gameObject == ScreenList[currentScreen]) { // turn off old buttons
                b.interactable = false;
            } else if (b.transform.parent.gameObject == ScreenList[id]) { // turn on new buttons
                b.interactable = true;
            }
        }

        // determine direction and animate away old menu
        animatorList[currentScreen].SetBool("OnStack", (id > currentScreen) ? true : false);
        animatorList[currentScreen].SetBool("IsOpen", false);

        // set new screen
        currentScreen = id;
        animatorList[currentScreen].SetBool("IsOpen", true);
        SetSelectedButton();
    }

    // transitions to next level
    public void GoToNextLevel(int i) {
		GraphData.gd.EditVar = i;
        Application.LoadLevel(Application.loadedLevel + 1);
    }

	//transition to circle screen
	public void LoadCircleMenu(int sceneNumber, int varChange){
		GraphData.gd.EditVar = varChange;
		Application.LoadLevel(sceneNumber);
	}
    public void circleOn(int source)
    {
      // canvas.SetActive(false);
        //eventSytem.SetActive(false);
        textEntrySource = source;
        circles.SetActive(true);
        controller.becomeActive();
        

    }
    public void setEditVar(int var)
    {
        GraphData.gd.EditVar = var;
    }
    public void circleOff()
    {
        circles.SetActive(false);
        ChangeScreen(textEntrySource);
    }

    //transition to circle screen
    public void LoadGraph()
    {
        Application.LoadLevel(3);
    }



    // quits
    public void Quit() {
        Application.Quit();
    }

    // sets the initial selected button (needed for controller support)
    private void SetSelectedButton() {
        foreach (Button b in buttonList) {
            if (b.transform.parent.gameObject == ScreenList[currentScreen]) {
                es.SetSelectedGameObject(b.gameObject);
                return;
            }
        }
    }
}