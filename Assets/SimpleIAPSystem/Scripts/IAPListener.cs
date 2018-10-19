/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Purchasing;

namespace SIS
{
    /// <summary>
    /// script that listens to purchases and other IAP events,
    /// here we tell our game what to do when these events happen
    /// <summary>
    public class IAPListener : MonoBehaviour
    {
        //subscribe to the most important IAP events
        public void Init()
        {
            IAPManager.inventoryRequestFailedEvent += HandleFailedInventory;
            IAPManager.purchaseSucceededEvent += HandleSuccessfulPurchase;
            IAPManager.purchaseFailedEvent += HandleFailedPurchase;
            ShopManager.itemSelectedEvent += HandleSelectedItem;
            ShopManager.itemDeselectedEvent += HandleDeselectedItem;
        }


        /// <summary>
        /// handle purchases, for real money or ingame currency
        /// </summary>
        public void HandleSuccessfulPurchase(string id)
        {
            //differ between ids set in the IAP Settings editor
            if (IAPManager.isDebug) Debug.Log("HandleSuccessfulPurchase: " + id);
            IAPObject obj = IAPManager.GetIAPObject(id);

            //get instantiated shop item based on the IAP id
            IAPItem item = null;
            if (ShopManager.GetInstance())
                item = ShopManager.GetIAPItem(id);

            //if the purchased item was non-consumable,
            //additionally block further purchase of the shop item
            if (item != null && obj != null && obj.type != ProductType.Consumable)
                item.Purchased(true);

            switch (id)
            {
                //section for in app purchases
                case "coins":
                    //the user bought the item "coins",
                    //increase coins by 1000 and show appropriate feedback
                    DBManager.IncreaseFunds("coins", 1000);
                    ShowMessage("1000 coins were added to your balance!");
                    break;
                case "coin_pack":
                    DBManager.IncreaseFunds("coins", 2500);
                    ShowMessage("2500 coins were added to your balance!");
                    break;
                case "big_coin_pack":
                    DBManager.IncreaseFunds("coins", 6000);
                    ShowMessage("6000 coins were added to your balance!");
                    break;
                case "huge_coin_pack":
                    DBManager.IncreaseFunds("coins", 12000);
                    ShowMessage("12000 coins were added to your balance!");
                    break;
                case "no_ads":
                    //no_ads purchased. You can now check DBManager.isPurchased("no_ads")
                    //before showing ads and block them
                    ShowMessage("Ads disabled!");
                    break;
                case "abo_monthly":
                    //same here - your code to unlock subscription content
                    ShowMessage("Subscribed to monthly abo!");
                    break;
                case "restore":
                    //nothing else to call here,
                    //the actual restore is handled by IAPManager
                    ShowMessage("Restored transactions!");
                    break;

                //section for in game content
                case "bullets":
                    //for virtual items, you could use DBManager's custom data option in order
                    //to save amounts of virtual products. E.g. increasing bullets by 100:
                    //int bullets = DBManager.GetPlayerData("bullets").AsInt;
                    //DBManager.SetPlayerData("bullets", new SimpleJSON.JSONData(bullets + 100));
                    ShowMessage("Bullets were added to your inventory!");
                    break;
                case "health":
                    ShowMessage("Medikits were added to your inventory!");
                    break;
                case "energy":
                    ShowMessage("Energy was added to your inventory!");
                    break;
                case "speed":
                    ShowMessage("Speed boost unlocked!");
                    break;
                case "speed_1":
                case "speed_2":
                case "speed_3":
                    ShowMessage("Speed boost upgraded!");
                    break;
                case "bonus":
                    ShowMessage("Bonus level unlocked!");
                    break;

                case "uzi":
                    ShowMessage("Uzi unlocked!");
                    break;
                case "ak47":
                    ShowMessage("AK47 unlocked!");
                    break;
                case "m4":
                    ShowMessage("M4 unlocked!");
                    break;

                case "hat":
                    ShowMessage("Hat unlocked!");
                    break;
                case "backpack":
                    ShowMessage("Backpack unlocked!");
                    break;
                case "belt":
                    ShowMessage("Ammo belt unlocked!");
                    break;
                case "jetpack":
                    ShowMessage("Jetpack unlocked!");
                    break;
                case "booster":
                    ShowMessage("Double XP unlocked!");
                    break;
            }
        }

        //just shows a message via our ShopManager component,
        //but checks for an instance of it first
        void ShowMessage(string text)
        {
            if (ShopManager.GetInstance())
                ShopManager.ShowMessage(text);
        }

        //called when an purchaseFailedEvent happens, here we forward
        //the error message to ShopManager's error window (if present)
        void HandleFailedInventory(string error)
        {
            if (ShopManager.GetInstance())
                ShopManager.ShowMessage(error);
        }

        //called when an purchaseFailedEvent happens,
        //we do the same here
        void HandleFailedPurchase(string error)
        {
            if (ShopManager.GetInstance())
                ShopManager.ShowMessage(error);
        }


        //called when a purchased shop item gets selected
        void HandleSelectedItem(string id)
        {
            if (IAPManager.isDebug) Debug.Log("Selected: " + id);
        }


        //called when a selected shop item gets deselected
        void HandleDeselectedItem(string id)
        {
            if (IAPManager.isDebug) Debug.Log("Deselected: " + id);
        }
    }
}