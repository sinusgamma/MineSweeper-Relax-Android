using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class StartOptions : MonoBehaviour
{
    public ShowPanels showPanels;  //Reference to ShowPanels script on UI GameObject, to show and hide panels
    public ToggleGroup styleToggleGroup;  // the togglegroup to toggle style

    public Text bestTimeS;
    public Text bestTimeM;
    public Text bestTimeL;
    public Text bestTimeXL;

    public Text totalScore;
    public Text playerRank;

    public Button achievButton;
    public Button leaderButton;

    // get active toggle from a group  -  called on the style toggle buttons
    public void GetActiveToggle()
    {
        string styleGroupName = "";
        // get first active toggle (and actually there should be only one in a group)
        foreach (var item in styleToggleGroup.ActiveToggles())
        {
            styleGroupName = item.name;
            break;
        }
        GlobalControl.Instance.LocalCopyOfData.styleGroupName = styleGroupName;  // set the style for the global control
        GlobalControl.Instance.SaveData();
    }


    // set the best level times in main menu
    public void showBestTimeTx()
    {
        bestTimeS.text = timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeEasy);
        bestTimeM.text = timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeMedium);
        bestTimeL.text = timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeHard);
        bestTimeXL.text = timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeVeryHard);
    }


    // set the main menu level time records - from save to string
    public string timeChanger(int timeInSec)
    {
        string timeString = "--";  // default
        int minutes;
        int seconds;
        if(timeInSec >= 99999 || timeInSec <= 0)  // if data isn't reasonable
        {
            timeString = "--";
        }
        else
        {
            minutes = (int)timeInSec / 60; //Divide the guiTime by sixty to get the minutes.
            seconds = (int)timeInSec % 60;//Use the euclidean division for the seconds.

            //update the label value
            timeString = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        return timeString;
    }


    // set the total score in main menu
    public void showTotalScore()
    {
        totalScore.text = GlobalControl.Instance.LocalCopyOfData.fullScore.ToString();
    }


    // set the player rank in main menu
    public void showPlayerRank()
    {
        playerRank.text = GlobalControl.Instance.LocalCopyOfData.playerRank.ToString();
    }

    // set the google play buttons behaviour
    public void GoogleButtonsBehaviour()
    {
        if (MyGoogleServices.Instance.isAuthenticated == true)
        {
            // enable these buttons if logged in
            achievButton.interactable = true;
            leaderButton.interactable = true;
        }
    }

    /*****************************************************************************/

    void Awake()
    {
        //Get a reference to ShowPanels attached to UI object
        showPanels = GetComponent<ShowPanels>();
    }


    void Start()
    {
        showBestTimeTx();
        showTotalScore();
        showPlayerRank();

        GoogleButtonsBehaviour();
    }

}