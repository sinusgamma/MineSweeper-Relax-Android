using UnityEngine;
using System;
using System.ComponentModel;

// Keeps data for saving between games and set the default values

[Serializable]
public class GameDataSaved
{
    private string styleName;
    public int fullScore;
    public int playerRank;

    private int btEasy = 99999;
    private int btMedium = 99999;
    private int btHard = 99999;
    private int btVeryHard = 99999;

    public string styleGroupName
    {
        get { return styleName; }
        set { styleName = value; }
    }

    public int bestTimeEasy // S
    {
        get { return btEasy; }
        set { btEasy = value; }
    }

    public int bestTimeMedium  // M
    {
        get { return btMedium; }
        set { btMedium = value; }
    }

    public int bestTimeHard  // L
    {
        get { return btHard; }
        set { btHard = value; }
    }

    public int bestTimeVeryHard  // XL
    {
        get { return btVeryHard; }
        set { btVeryHard = value; }
    }



}
