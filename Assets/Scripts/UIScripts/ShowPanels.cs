using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowPanels : MonoBehaviour {

    public GameObject menuPanel;                            // the main menu panel
    public GameObject customPanel;							// the custom grid settings
    public GameObject optionsPanel;							// the options panel - sounds
    public GameObject stylePanel;                         // the options panel - sounds
    public GameObject optionsTint;                          //Store a reference to the Game Object OptionsTint

    public bool isMenuOpen = true;


    //Call this function to activate and display the Style panel during the main menu
    public void ShowStylePanel()
    {
        stylePanel.SetActive(true);
        //optionsTint.SetActive(true);
        string activeToggleName = GlobalControl.Instance.LocalCopyOfData.styleGroupName;  // get the name of saved style
        if (GameObject.Find(activeToggleName) != null)
        {
            Toggle activeToggle = GameObject.Find(activeToggleName).GetComponent<Toggle>();
            activeToggle.isOn = true;  // set the toggle on
        }
    }

    //Call this function to deactivate and hide the Style panel during the main menu
    public void HideStylePanel()
    {
        stylePanel.SetActive(false);
    }

    //Call this function to activate and display the Custom Grid panel during the main menu
    public void ShowCustomPanel()
    {
        customPanel.SetActive(true);
    }

    //Call this function to deactivate and hide the Custom Grid panel during the main menu
    public void HideCustomPanel()
    {
        customPanel.SetActive(false);
    }


    //Call this function to activate and display the Options panel during the main menu
    public void ShowOptionsPanel()
	{
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		optionsTint.SetActive(false);
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.SetActive (true);
        isMenuOpen = true;
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
        isMenuOpen = false;
	}
}
