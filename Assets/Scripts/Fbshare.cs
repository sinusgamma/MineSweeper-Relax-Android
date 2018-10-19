using UnityEngine;
using System.Collections;

public class Fbshare : MonoBehaviour
{

    public string AppID = "282461685419526";
    public string Link = "https://play.google.com/store/apps/details?id=com.sinusgamma.msrelax";
    // The picture's URL and the picture must be at least 200px by 200px.
    public string Picture = "https://lh3.googleusercontent.com/GciNhMfTqxFaXw7mNLRp6ZfXnII6vLFKjgDOEirk_YLq4363pf-khJcEepmdW9ScQROL=w300-rw";

    // The name of your app or game.
    public string Name = "Minesweeper Relax";

    // The caption of your game or app.
    public string Caption = "Give it a try! Beautiful Minesweeper game.";

    // The description of your game or app.
    public string Description;

    public StartOptions startOptions;

    public void FacebookShare()
    {
        // Make the list to build the description
        Description = "Full Score: " + GlobalControl.Instance.LocalCopyOfData.fullScore
                    + " Rank: " + GlobalControl.Instance.LocalCopyOfData.playerRank
                    + " Best S: " + startOptions.timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeEasy)
                    + " Best M: " + startOptions.timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeMedium)
                    + " Best L: " + startOptions.timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeHard)
                    + " Best XL: " + startOptions.timeChanger(GlobalControl.Instance.LocalCopyOfData.bestTimeVeryHard);

        Application.OpenURL("https://www.facebook.com/dialog/feed?" +
                "app_id=" + AppID +
                "&link=" + Link +
                "&picture=" + Picture +
                "&name=" + SpaceHere(Name) +
                "&caption=" + SpaceHere(Caption) +
                "&description=" + SpaceHere(Description) +
                "&redirect_uri=https://facebook.com/");
    }
    string SpaceHere(string val)
    {
        return val.Replace(" ", "%20"); // %20 is only used for space
    }
}
