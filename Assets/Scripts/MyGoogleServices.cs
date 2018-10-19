using UnityEngine;
using GooglePlayGames;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class MyGoogleServices : MonoBehaviour
{

    public static MyGoogleServices Instance;
    public Button achievButton;
    public Button leaderButton;
    public bool isAuthenticated;

    void Awake()
    {
        // there is one single public static instance of one class
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public string achSWinner; //
    public string achMWinner; //
    public string achLWinner; //
    public string achXLWinner; //
    public string achWin; //
    public string achLose; //
    public string achSincr; //
    public string achMincr; //
    public string achLincr; //
    public string achXLincr; //
    public string achLoseincr; //
    public string achWinWithLifeincr; //
    public string achAddincr; //
    public string achLifeUserincr; //
    public string achGoXL; //
    public string achGoCustom; //
    public string achGoSky; //

    public string leaderboardScore; //
    public string leaderboardS; //
    public string leaderboardM; //
    public string leaderboardL; //
    public string leaderboardXL; //

    void Start()
    {
        // Select the Google Play Games platform as our social platform implementation
        GooglePlayGames.PlayGamesPlatform.Activate();

        // authenticate user:
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                // enable these buttons after logged in
                achievButton.interactable = true;
                leaderButton.interactable = true;

                isAuthenticated = true;
                

                // register probable records from offline mode at login
                RegisterRecord(0);
            }
            else
            {
                isAuthenticated = false;
            }
        });
    }


    void OnApplicationQuit()
    {
        // when quit application sync score and record to google play
        RegisterFullScore(GlobalControl.Instance.LocalCopyOfData.fullScore);
        RegisterRecord(0);
    }


    /********************************************
     *  SHOW GOOGLE BOARDS
     * *****************************************/

    // Show Achievment UI
    public void ShowAchievmentUI()
    {
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            // show achievements UI
            Social.ShowAchievementsUI();
        }
    }

    // Show Leaderboard UI
    public void ShowLeaderboardUI()
    {
        // show leaderboard UI
        Social.ShowLeaderboardUI();
    }



    /*********************************
    * Listeners
    *********************************/

    void OnEnable()
    {
        EventManager.OnWonEvent         += GameWon;

        EventManager.OnRecordEvent      += RegisterRecord;

        EventManager.OnLoseEvent        += GameLose;

        EventManager.OnLifeUsedEvent    += GameLifeUsed;

        EventManager.OnAddWatchedEvent  += GameAddWatched;
    }

    void OnDisable()
    {
        EventManager.OnWonEvent         -= GameWon;

        EventManager.OnRecordEvent      -= RegisterRecord;

        EventManager.OnLoseEvent        -= GameLose;

        EventManager.OnLifeUsedEvent    -= GameLifeUsed;

        EventManager.OnAddWatchedEvent  -= GameAddWatched;
    }


    /************************************
     * Functions
     *************************************/

    // GameWon function - no need to cut part
    public void GameWon(int difficulty, int life, int rank)
    {
        // Register score when won a game
        RegisterFullScore(GlobalControl.Instance.LocalCopyOfData.fullScore);

        // Unlock Gamewon Achievement
        Social.ReportProgress(achWin, 100.0f, (bool success) => {
            // handle success or failure
        });

        switch (difficulty)
        {
            case 1:
                // Unlock S Winner Achievement
                Social.ReportProgress(achSWinner, 100.0f, (bool success) => {
                    // handle success or failure
                });
                // Increment S-incr Achievment by 1
                PlayGamesPlatform.Instance.IncrementAchievement(
                    achSincr, 1, (bool success) => {
                // handle success or failure
                });
                break;
            case 2:
                // Unlock M Winner Achievement
                Social.ReportProgress(achMWinner, 100.0f, (bool success) => {
                    // handle success or failure
                });
                // Increment M-incr Achievment by 1
                PlayGamesPlatform.Instance.IncrementAchievement(
                    achMincr, 1, (bool success) => {
                        // handle success or failure
                    });
                break;
            case 3:
                // Unlock L Winner Achievement
                Social.ReportProgress(achLWinner, 100.0f, (bool success) => {
                    // handle success or failure
                });
                // Increment L-incr Achievment by 1
                PlayGamesPlatform.Instance.IncrementAchievement(
                    achLincr, 1, (bool success) => {
                        // handle success or failure
                    });
                break;
            case 4:
                // Unlock XL Winner Achievement
                Social.ReportProgress(achXLWinner, 100.0f, (bool success) => {
                    // handle success or failure
                });
                // Increment XL-incr Achievment by 1
                PlayGamesPlatform.Instance.IncrementAchievement(
                    achXLincr, 1, (bool success) => {
                        // handle success or failure
                    });
                break;
            default:
                
                break;
        }


        // if life == 1 then increment this achievement
        if (life == 1)
        {
            // Increment If Won without using life Achievment by 1
            PlayGamesPlatform.Instance.IncrementAchievement(
                achWinWithLifeincr, 1, (bool success) => {
                    // handle success or failure
                });
        }

        
        // Check rank to unlock ranking achievments
        if(rank >= 30)
        {
            // Unlock Go Big Achievement
            Social.ReportProgress(achGoXL, 100.0f, (bool success) => {
                // handle success or failure
            });
        }
        if (rank >= 60)
        {
            // Unlock Custom Board Achievement
            Social.ReportProgress(achGoCustom, 100.0f, (bool success) => {
                // handle success or failure
            });
        }
        if (rank >= 120)
        {
            // Unlock Sky style Achievement
            Social.ReportProgress(achGoSky, 100.0f, (bool success) => {
                // handle success or failure
            });
        }


    }

    public void RegisterRecord(int difficulty)
    {
        // Register all record if any record, so this function can register the records any time  - google play expect milisec so *1000
        Social.ReportScore(GlobalControl.Instance.LocalCopyOfData.bestTimeEasy * 1000, leaderboardS, (bool success) => {
            // handle success or failure
        });
        Social.ReportScore(GlobalControl.Instance.LocalCopyOfData.bestTimeMedium * 1000, leaderboardM, (bool success) => {
            // handle success or failure
        });
        Social.ReportScore(GlobalControl.Instance.LocalCopyOfData.bestTimeHard * 1000, leaderboardL, (bool success) => {
            // handle success or failure
        });
        Social.ReportScore(GlobalControl.Instance.LocalCopyOfData.bestTimeVeryHard * 1000, leaderboardXL, (bool success) => {
            // handle success or failure
        });
    }

    public void GameLose()
    {
        // Unlock GameLose Achievement
        Social.ReportProgress(achLose, 100.0f, (bool success) => {
            // handle success or failure
        });

        // Increment Lose Achievment by 1
        PlayGamesPlatform.Instance.IncrementAchievement(
            achLoseincr, 1, (bool success) => {
            // handle success or failure
        });
    }

    public void GameLifeUsed()
    {
        // Increment Game Life Used Achievment by 1
        PlayGamesPlatform.Instance.IncrementAchievement(
            achLifeUserincr, 1, (bool success) => {
                // handle success or failure
            });
    }

    public void GameAddWatched()
    {
        // Increment Add watched Achievment by 1
        PlayGamesPlatform.Instance.IncrementAchievement(
            achAddincr, 1, (bool success) => {
                // handle success or failure
            });
    }


    // Register the fullscore
    public void RegisterFullScore(int fullscore)
    {
        Social.ReportScore(fullscore, leaderboardScore, (bool success) => {
            // handle success or failure
        });
    }


}
