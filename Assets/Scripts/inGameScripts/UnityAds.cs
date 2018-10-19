using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour
{


    // normal skippable ads
    public void ShowAd()
    {
        if (Advertisement.IsReady() && !SIS.DBManager.isPurchased("ms_pro"))
        {
            Advertisement.Show();
            EventManager.OnAddWatched(); // event when add is watched
        }
    }


    // rewarded ads
    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo")  && !SIS.DBManager.isPurchased("ms_pro"))  // don't show add to pro users
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                EventManager.OnAddWatched();  // event when add is watched
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
