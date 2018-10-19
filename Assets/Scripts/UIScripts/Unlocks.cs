using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Unlocks : MonoBehaviour {

    public Button buttonXL;
    public Button buttonCustomGrid;
    public Toggle bonusStyle;
    public int playerRank;


    public void EnableButton()
    {
        buttonXL.interactable = false;
        buttonCustomGrid.interactable = false;
        bonusStyle.interactable = false;

        if (playerRank >= 30 || SIS.DBManager.isPurchased("ms_pro"))  // enable XL map if rank is enough OR purchased
        {
            buttonXL.interactable = true;
        }
        if (playerRank >= 60 || SIS.DBManager.isPurchased("ms_pro"))  // enable custom map if rank is enough OR purchased
        {
            buttonCustomGrid.interactable = true;
        }
        if (playerRank >= 120 || SIS.DBManager.isPurchased("ms_pro"))  // enable custom map if rank is enough OR purchased
        {
            bonusStyle.interactable = true;
        }
    }


	// Use this for initialization
	void Start () {
        playerRank = GlobalControl.Instance.LocalCopyOfData.playerRank;
        EnableButton();
    }

}
