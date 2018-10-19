using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AndroidBackButton : MonoBehaviour {

    private ShowPanels showPanels;

    // we are in an object, which is permanent, so not permanent objects must be reached at the start and 
    // at level loading as well, so we need two functions : start and onlevelwasloaded
    // ugly
    void Start()
    {
        showPanels = GameObject.Find("UIstart").GetComponent<ShowPanels>();
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            showPanels = GameObject.Find("UIstart").GetComponent<ShowPanels>();
        }
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackButtonActions();
        }
    }

    public void BackButtonActions()
    {
        int sceneID = SceneManager.GetActiveScene().buildIndex;  // get active scene
        
        // if not start scene => load home  (from grids and shop)
        if (sceneID != 0)
        {
            SceneManager.LoadScene(0);
        }
        else if (sceneID == 0 && showPanels.isMenuOpen)
        {
            // QuitApplication is appended to this go
            gameObject.GetComponent<QuitApplication>().Quit();
            Debug.Log("quit time");
        }
        else
        {
            // on start go back to main menu
            showPanels.HideOptionsPanel();
            showPanels.HideStylePanel();
            showPanels.HideCustomPanel();

            showPanels.ShowMenu();
        }
    }


}
