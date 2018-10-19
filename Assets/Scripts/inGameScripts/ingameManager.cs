using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class ingameManager : MonoBehaviour
{
    public Grid grid;
    private float time;

    // top panel
    public Text mineCountText;
    public Text timerText;

    // win panel settings
    public GameObject popupWin;
    public Text winText;
    public Text winTimerText;
    public Text secPerBomb;
    public Text scoreText;
    public Text rankText;
    public int score;
    public int scorePlus;
    public int rank;
    public Button winAdButton;

    // lose panel settings
    public GameObject popupLose;
    public Text loseText;
    public Text loseTimerText;

    // pending panel settings
    public GameObject popupPending;
    public Button pendingAdButton;

    private bool isFirstFrame = true;
    private bool isRecord = false;
    private int difficulty;  // check difficulty level
    private bool isBonusAdded = false;


    public void restart()  // restart level
    {
        Grid.state = "loading";

        Grid.tilesAll.Clear();
        Grid.tilesMined.Clear();
        Grid.tilesUnmined.Clear();

        Grid.minesMarkedCorrectly = 0;
        Grid.tilesUncovered = 0;
        Grid.minesRemaining = 0;

        Grid.isFirstClick = false;

        Grid.life = 1;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // reload current scene
    }


    public void continueGame()
    {
        Grid.state = "inGame";
        hideIngameContPanel();
        isFirstFrame = true;  // for the later windows to calculate only one case

        EventManager.OnLifeUsed();
    }


    // manage the main ingame UI, and ingame popup panel - called in update
    public void manageUI()  
    {
        // write to panel count of flags/mines
        mineCountText.text = Grid.minesMarkedCorrectly + "/" + Grid.numberOfMines.ToString();

        // count - write time
        timer();

         // the popup panel texts and behaviours
        if (Grid.state == "gameOver")
        {
            if(isFirstFrame == true) // calculate only once - spare resources
            {
                SoundManager.Instance.loosingSounds();  // play the loosing sounds
                Invoke("showIngameLosePanel", 2);
                losingTexts();
                isFirstFrame = false; // dont calculate in next frame

                EventManager.OnLose();  // fire the OnLoseEvent
            }
        }
        else if (Grid.state == "gameWon")
        {
            if (isFirstFrame == true) // calculate only once
            {
                SoundManager.Instance.winningSounds();  // play the winning sounds
                Invoke("showIngameWinPanel", 1);
                winnigTexts();
                addScores(score, rank); // add the scores to saveable

                isFirstFrame = false; // dont calculate in next frame

                EventManager.OnWon(difficulty, Grid.life, GlobalControl.Instance.LocalCopyOfData.playerRank);  // fire the OnWonEvent
            }
        }
        else if (Grid.state == "pending")
        {
            if (isFirstFrame == true) // calculate only once
            {
                showIngameContPanel();

                isFirstFrame = false; // dont calculate in next frame
            }
        }
    }


    // popup panel data when loosing
    public void losingTexts()
    {
        loseText.text = QuoteList.loseQuotes[Random.Range(0, QuoteList.loseQuotes.Count)];  // get a random item from the loseQuotes
        loseTimerText.text = "Time: " + timerText.text;
        secPerBomb.text = "";
        scoreText.text = "";
    }


    // popup panel data when winning
    public void winnigTexts()
    {
        winText.text = QuoteList.winQuotes[Random.Range(0, QuoteList.winQuotes.Count)];  // get a random item from the winQuotes

        checkRecords(); // check if there is a record, and set the record
        if (isRecord == true)
        {
            winTimerText.text = "New Record: " + timerText.text;
            EventManager.OnRecord(difficulty);  // fire the OnRecordEvent  // the checkRecords() set the time on localcopyofdata, so use that to find the record time to prevent different datas
            isRecord = false;
        }
        else
        {
            winTimerText.text = "Time: " + timerText.text;
        }
        secPerBomb.text = Mathf.Round(time / Grid.numberOfMines * 100.0f) / 100.0f + " sec/bomb";
        scoreCounter(); // sets the score variable
        rankCounter(); // sets the rank
        scoreText.text = "Score: " + score.ToString();
        rankText.text = "Rank: +" + rank.ToString();
    }


    // addscores
    public void addScores(int scoreToAdd, int rankToAdd)
    {
        GlobalControl.Instance.LocalCopyOfData.fullScore += scoreToAdd;  //  add score to full score
        GlobalControl.Instance.LocalCopyOfData.playerRank += rankToAdd;  //  add rank to player rank
        GlobalControl.Instance.SaveData();  // save the data
    }


    // check if best time is beaten
    public void checkRecords()
    {
        switch(difficulty)
        {
            case 1:   // easy
                if (GlobalControl.Instance.LocalCopyOfData.bestTimeEasy > (int)time || GlobalControl.Instance.LocalCopyOfData.bestTimeEasy == 0)
                {
                    isRecord = true;
                    GlobalControl.Instance.LocalCopyOfData.bestTimeEasy = (int)time;
                }
                break;
            case 2:   // medium
                if (GlobalControl.Instance.LocalCopyOfData.bestTimeMedium > (int)time || GlobalControl.Instance.LocalCopyOfData.bestTimeMedium == 0)
                {
                    isRecord = true;
                    GlobalControl.Instance.LocalCopyOfData.bestTimeMedium = (int)time;
                }
                break;
            case 3:   // hard
                if (GlobalControl.Instance.LocalCopyOfData.bestTimeHard > (int)time || GlobalControl.Instance.LocalCopyOfData.bestTimeHard == 0)
                {
                    isRecord = true;
                    GlobalControl.Instance.LocalCopyOfData.bestTimeHard = (int)time;
                }
                break;
            case 4:   // very hard
                if (GlobalControl.Instance.LocalCopyOfData.bestTimeVeryHard > (int)time || GlobalControl.Instance.LocalCopyOfData.bestTimeVeryHard == 0)
                {
                    isRecord = true;
                    GlobalControl.Instance.LocalCopyOfData.bestTimeVeryHard = (int)time;
                }
                break;
            default:   // custom
                break;
        }
    }


    public void timer()
    {
        if (Grid.state == "inGame")  // timer only inGame
        {
            time += Time.deltaTime;

            int minutes = (int)time / 60; //Divide the guiTime by sixty to get the minutes.
            int seconds = (int)time % 60;//Use the euclidean division for the seconds.

            //update the label value
            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
    }

    
    public void scoreCounter()  // count the score for the game - score penalty is slownig with time
    {
        int maxScore = Grid.tilesPerRow * Grid.tilesPerColumn + (3 * Grid.numberOfMines);
        int tableSize = Grid.tilesPerRow * Grid.tilesPerColumn;

        float step = tableSize * 0.5f;
        float penalty;
        
        if(time <= step)  
        {
            penalty = time * 0.5f;
            score = maxScore - (int)penalty;
        }
        else if (step < time && time <= 2 * step)
        {
            penalty = step * 0.5f + (time - step) * 0.25f;
            score = maxScore - (int)penalty;
        }
        else if (2 * step < time)
        {
            penalty = step * 0.5f + step * 0.25f + (time - 2 * step) * 0.1f;
            score = maxScore - (int)penalty;
        }

        if(score < Grid.numberOfMines)  // score cant be smaller than the number of mines
        {
            score = Grid.numberOfMines;
        }
    }


    public void rankCounter()  // on smaller grida +1, on larger +2 rank when winning
    {
        if (Grid.tilesPerColumn * Grid.tilesPerRow < 200)
        {
            rank = 1;
        }
        else
        {
            rank = 2;
        }
    }


    // get bonus score after winning without life loss
    public void scoreBonus()
    {
        if (isBonusAdded == false)
        {        
            scorePlus = (int)((float)score * 0.1);
            scoreText.text = "Score: " + (score + scorePlus).ToString();
            rankText.text = "Rank: +" + (rank + 1).ToString();
            addScores(scorePlus, 1); // add the scorePlus to saveable
        }
        isBonusAdded = true;
        winAdButton.interactable = false;  // disable the bonus multiple times
    }


    // Lose panel 
    public void showIngameLosePanel()
    {
        popupLose.SetActive(true);
    }
    public void hideIngameLosePanel()
    {
        popupLose.SetActive(false);
    }


    // Win panel
    public void showIngameWinPanel()
    {
        popupWin.SetActive(true);
        if ((Advertisement.IsReady() || SIS.DBManager.isPurchased("ms_pro")) && Grid.life == 1)  // enable adButton onlye if the (add is ready OR purchased) and life = 1
        {
            winAdButton.interactable = true;
        }
        else
        {
            winAdButton.interactable = false;
        }
    }
    public void hideIngameWinPanel()
    {
        popupWin.SetActive(false);
    }


    // Continue panel
    public void showIngameContPanel()
    {
        popupPending.SetActive(true);
        if (Advertisement.IsReady() || SIS.DBManager.isPurchased("ms_pro"))  // enable adButton onlye if the add is ready OR purchased
        {
            pendingAdButton.interactable = true;
        }
        else
        {
            pendingAdButton.interactable = false;
        }
    }
    public void hideIngameContPanel()
    {
        popupPending.SetActive(false);
    }



    // Use this for initialization *************************
    void Start()
    {
        difficulty = GlobalControl.Instance.difficulty;  // check difficulty level
        popupWin.SetActive(false);  // hide panel at the beginning of level
        popupLose.SetActive(false);
        popupPending.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        manageUI();
    }
}
