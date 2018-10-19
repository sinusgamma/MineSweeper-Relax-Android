/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace SIS
{
    /// <summary>
    /// IAP receipt verification on the client (local, on the device)
    /// using Unity IAPs validator class
    /// </summary>
	public class ReceiptValidatorClient : ReceiptValidator
    {
		/*
        public override bool shouldValidate(VerificationType verificationType)
        {
            #if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX || UNITY_TVOS
            if (this.verificationType == verificationType || this.verificationType == VerificationType.both)
                return true;
            #endif

            return false;
        }


        public override void Validate()
        {
            //loop over all IAP items to check if a valid receipt exists
            string[] ids = IAPManager.GetIAPKeys();
            for (int i = 0; i < ids.Length; i++)
            {
                string id = ids[i];

                //do not validate virtual items and skip not purchased ones
                if (IAPManager.GetIAPObject(id).isVirtual || !DBManager.isPurchased(id))
                    continue;

                Product p = IAPManager.controller.products.WithID(id);

                //we found a receipt for this product on the device, initiate client receipt verification.
                //if we haven't found a receipt for this item, yet it is set to purchased. This can't be,
                //maybe the database contains fake data. Only pass the id to verification so it will fail
                if (p != null && p.hasReceipt) Validate(id, p.receipt);
                else Validate(id, string.Empty);
            }
        }


        public override void Validate(string id, string receipt)
        {
            CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                                                                          AppleTangle.Data(),
                                                                          Application.bundleIdentifier);

            try
            {
                // On Google Play, result will have a single product Id.
                // On Apple stores receipts contain multiple products.
                validator.Validate(receipt);
                
                //foreach (IPurchaseReceipt productReceipt in results)
                //    Debug.Log("RECEIPT PROCESSED: " + productReceipt.productID);
                
                IAPManager.GetInstance().PurchaseVerified(id);
            }
            catch (IAPSecurityException)
            {
                if (DBManager.isPurchased(id))
                {
                    IAPItem item = null;
                    if (ShopManager.GetInstance())
                        item = ShopManager.GetIAPItem(id);
                    if (item) item.Purchased(false);
                    DBManager.RemovePurchased(id);
                }
            };
        }
		*/
    }
}