/*  This file is part of the "Simple IAP System" project by Rebound Games.
 *  You are only allowed to use these resources if you've bought them from the Unity Asset Store.
 * 	You shall not license, sublicense, sell, resell, transfer, assign, distribute or
 * 	otherwise make available to any third party the Service or the Content. */

using UnityEngine;
using UnityEngine.Purchasing;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SIS
{
    /// <summary>
    /// Unity IAP cross-platform wrapper for real money purchases,
    /// as well as for virtual ingame purchases (for virtual currency)
    /// </summary>
	public class IAPManager : MonoBehaviour, IStoreListener
	{
        /// <summary>
        /// Debug messages are enabled in Development build
        /// </summary>
        public static bool isDebug = false;

        /// <summary>
        /// your Google Play license key
        /// </summary>
        public string googleStoreKey;

        /// <summary>
        /// your server url for online IAP verification or remote config
        /// </summary>
        public string serverUrl;

        /// <summary>
        /// type for processing remotely hosted configs
        /// </summary>
        public RemoteType remoteType = RemoteType.none;

        /// <summary>
        /// relative url to your remotely hosted config file
        /// </summary>
        public string remoteFileName;

        /// <summary>
        /// static reference to this script
        /// </summary>
        private static IAPManager instance;

        /// <summary>
        /// object for downloading hosted configs
        /// </summary>
        private WWW request;

        /// <summary>
        /// array of real money IAP ids
        /// </summary>
        private string[] realIDs = null;

        /// <summary>
        /// client/server IAP receipt verificator
        /// </summary>
        private ReceiptValidator validator;

        /// <summary>
        /// In app products, set in the IAP Settings editor
        /// </summary>
        [HideInInspector]
        public List<IAPGroup> IAPs = new List<IAPGroup>();

        /// <summary>
        /// list of virtual currency,
        /// set in the IAP Settings editor
        /// </summary>
        [HideInInspector]
        public List<IAPCurrency> currency = new List<IAPCurrency>();

        /// <summary>
        /// dictionary of product ids,
        /// mapped to the corresponding IAPObject for quick lookup
        /// </summary>
        public Dictionary<string, IAPObject> IAPObjects = new Dictionary<string, IAPObject>();

        /// <summary>
        /// fired when a purchase succeeds, delivering its product id
        /// </summary>
        public static event Action<string> purchaseSucceededEvent;

        /// <summary>
        /// fired when a purchase fails, delivering its product id
        /// </summary>
        public static event Action<string> purchaseFailedEvent;

        /// <summary>
        /// fired when a server request fails, delivering the error message
        /// </summary>
        public static event Action<string> inventoryRequestFailedEvent;

        //disable platform specific warnings, because Unity throws them
        //for unused variables however they are used in this context
        #pragma warning disable 0414
        public static IStoreController controller;
		private static IExtensionProvider extensions;
        #pragma warning restore 0414

        // initialize IAPs, billing systems and database,
        // as well as shop components in this order
        void Awake()
        {
            //make sure we keep one instance of this script in the game
            if (instance)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(this);
            isDebug = Debug.isDebugBuild;

            //set static reference
            instance = this;
            //populate IAP dictionary and arrays with product ids
            InitIds();

			var module = StandardPurchasingModule.Instance();
			var builder = ConfigurationBuilder.Instance(module);
			RequestProductData(builder);

            builder.Configure<IGooglePlayConfiguration>().SetPublicKey(googleStoreKey);
            if(isDebug)
            {
                builder.Configure<IMicrosoftConfiguration>().useMockBillingSystem = true;
                // Write out our Amazon Sandbox JSON file.
                // This has no effect when the Amazon billing service is not in use.
                builder.Configure<IAmazonConfiguration>().WriteSandboxJSON(builder.products);
            }

            // Now we're ready to initialize Unity IAP.
            UnityPurchasing.Initialize(this, builder);

            //initialize database, remote and shop managers
            GetComponent<IAPListener>().Init();
            GetComponent<DBManager>().Init();
            StartCoroutine(RemoteDownload());
            validator = GetComponent<ReceiptValidator>();
            OnLevelWasLoaded(-1);
        }


        /// <summary>
        /// Initiate shop manager initialization on scene change.
        /// </summary>
        public void OnLevelWasLoaded(int level)
        {
            if (instance != this)
                return;

            ShopManager shop = null;
            GameObject shopGO = GameObject.Find("ShopManager");
            if (shopGO) shop = shopGO.GetComponent<ShopManager>();
            if (shop)
            {
                shop.Init();
                #if !UNITY_EDITOR
                    if(controller != null)
                        ShopManager.OverwriteWithFetch(controller.products.all);
                #endif
            }
        }


        /// <summary>
        /// Returns a static reference to this script.
        /// </summary>
        public static IAPManager GetInstance()
        {
            return instance;
        }


        // initialize IAP ids:
        // populate IAP dictionary and arrays with product ids
        private void InitIds()
        {
            //create a list only for real money purchases
            List<string> ids = new List<string>();

            if (IAPs.Count == 0)
                Debug.LogError("Initializing IAPManager, but IAP List is empty."
                               + " Did you set up IAPs in the IAP Settings?");

            //loop over all groups
            for (int i = 0; i < IAPs.Count; i++)
            {
                //cache current group
                IAPGroup group = IAPs[i];
                //loop over items in this group
                for (int j = 0; j < group.items.Count; j++)
                {
                    //cache item
                    IAPObject obj = group.items[j];

                    if (String.IsNullOrEmpty(obj.id) || IAPObjects.ContainsKey(obj.id))
                    {
                        Debug.LogError("Found IAP Object in IAP Settings without an identifier "
                                        + " or " + obj.id + " does exist already. Skipping product.");
                        continue;
                    }

                    //add this IAPObject to the dictionary of id <> IAPObject
                    IAPObjects.Add(obj.id, obj);
                    //if it's an IAP for real money, add it to the id list
                    if (!obj.isVirtual) ids.Add(obj.id);
                }
            }

            //don't add the restore button to the list of online purchases
            if (ids.Contains("restore")) ids.Remove("restore");
            //convert and store list of real money IAP ids as string array 
            realIDs = ids.ToArray();
        }


        // Construct IAP product data with their App Store identifiers
        private void RequestProductData(ConfigurationBuilder builder)
        {
			for(int i = 0; i < realIDs.Length; i++)
			{
				IAPObject obj = GetIAPObject(realIDs[i]);
				builder.AddProduct(obj.id, obj.type, obj.GetIDs());
			}
        }


        // Optionally: verify old purchases online.
        // Once we've received the product list, we overwrite
        // the existing shop item values with this online data
        public void OnInitialized(IStoreController ctrl, IExtensionProvider ext)
        {
            controller = ctrl;
            extensions = ext;

            if (validator && validator.shouldValidate(VerificationType.onStart))
                validator.Validate();

            if (ShopManager.GetInstance())
                ShopManager.OverwriteWithFetch(controller.products.all);
        }
		
			
        /// <summary>
        /// Purchase product based on its product id.
        /// If the productId matches "restore", we restore transactions instead.
        /// Our delegates then fire the appropriate succeeded/fail/restore event.
        /// </summary>
        public static void PurchaseProduct(string productId)
        {
            if (productId == "restore")
                RestoreTransactions();
            else
            {
				IAPObject obj = GetIAPObject(productId);
				if(obj == null) 
				{
					if(isDebug) Debug.LogError("Product " + productId + " not found in IAP Settings.");
					return;
				}

                //distinguish between virtual and real products
				if(obj.isVirtual) PurchaseProduct(obj);
				else controller.InitiatePurchase(controller.products.WithID(productId));
            }
        }
		
		
		/// <summary>
        /// Overload for purchasing virtual product based on its product identifier.
        /// </summary>
        public static void PurchaseProduct(IAPObject obj)
        {	
			string productId = obj.id;
			//product is set to already owned, do nothing
			if(DBManager.isPurchased(productId))
			{
				OnPurchaseFailed("Product already purchased.");
                return;
			}
		
            //check whether the player has enough funds
            bool didSucceed = DBManager.VerifyVirtualPurchase(obj);
            if (isDebug) Debug.Log("Purchasing virtual product " + productId + ", result: " + didSucceed);
            //on success, non-consumables are saved to the database. This automatically
			//saves the new substracted fund value, then and fire the succeeded event
            if (didSucceed)
            {
                if (obj.type != ProductType.Consumable)
					DBManager.SetToPurchased(productId);
                purchaseSucceededEvent(productId);
            }
            else
                OnPurchaseFailed("Insufficient funds.");
        }

		
		/// <summary>
		/// This will be called when a purchase completes.
		/// Optional: verify new product receipt.
		/// </summary>
		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
            string id = e.purchasedProduct.definition.id;

            if (validator && validator.shouldValidate(VerificationType.onPurchase))
            {
                validator.Validate(id, e.purchasedProduct.receipt);
                if (!(validator is ReceiptValidatorClient))
                    return PurchaseProcessingResult.Pending;
            }
            else
            {
                PurchaseVerified(id);
            }
                
            // Indicate we have handled this purchase, we will not be informed of it again
            return PurchaseProcessingResult.Complete;
		}


        // set product to purchased after successful verification (or without)
        // For non consumable IAPs or subscriptions, alter database entry
        public void PurchaseVerified(string id)
        {
            if (!IAPObjects.ContainsKey(id))
                id = GetIAPIdentifier(id);
            if (!IAPObjects.ContainsKey(id)) return;
            IAPObject obj = IAPObjects[id];

            //don't continue if the product is already purchased,
            //for example if we just want to verify an existing product again
            if (DBManager.isPurchased(id)) return;
            if (obj.type != ProductType.Consumable)
				DBManager.SetToPurchased(id);

            purchaseSucceededEvent(id);
        }


        /// <summary>
        /// Restore already purchased user's transactions for non consumable IAPs.
        /// For Android we use the received list for detecting previous purchases.
        /// </summary>
        public static void RestoreTransactions()
        {
			#if UNITY_IOS
				extensions.GetExtension<IAppleExtensions>().RestoreTransactions(OnTransactionsRestored);
			#else
			
			Product[] purchasedProducts = controller.products.all;
            foreach (Product product in purchasedProducts)
            {
                string id = product.definition.id;
				if (DBManager.isPurchased(id) || product.definition.type == ProductType.Consumable
					|| !product.hasReceipt || String.IsNullOrEmpty(product.receipt))
					continue;
				
                DBManager.SetToPurchased(id);
            }
            
			OnTransactionsRestored(true);
            #endif

            //update ShopManager GUI items
            if (ShopManager.GetInstance())
                ShopManager.SetItemState();
        }


        //initiates the download process of your remotely hosted
        //config file for virtual products. Differs between types:
        //cached: stores config on the device, changes on next bootup
        //overwrite: only preserves changes in the current session
        private IEnumerator RemoteDownload()
        {
            //build file url
            string url = serverUrl + remoteFileName;

            switch (remoteType)
            {
                case RemoteType.cached:
                    //load cached file string and overwrite virtual IAPs
                    DBManager.LoadRemoteConfig();
                    //download new config
                    yield return StartCoroutine(Download(url));
                    //save downloaded file
                    DBManager.SaveRemoteConfig(request.text);
                    break;
                case RemoteType.overwrite:
                    //download new config 
                    yield return StartCoroutine(Download(url));
                    //parse string and overwrite virtual IAPs
                    DBManager.ConvertToIAPs(request.text);
                    break;
            }
        }


        //downloads the remotely hosted config file
        private IEnumerator Download(string url)
        {
            request = new WWW(url);
            yield return request;

            if (!string.IsNullOrEmpty(request.error))
                Debug.Log("Failed remote config download with error: " + request.error);
            else if (isDebug)
                Debug.Log("Downloaded remotely hosted config file: \n" + request.text);
        }


        // method that fires a product request error
		public void OnInitializeFailed(InitializationFailureReason error)
		{
			switch (error)
			{
				case InitializationFailureReason.AppNotKnown:
					Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");
					break;
				case InitializationFailureReason.PurchasingUnavailable:
					// Ask the user if billing is disabled in device settings.
					Debug.LogWarning("Billing disabled!");
					break;
				case InitializationFailureReason.NoProductsAvailable:
					// Developer configuration error; check product metadata.
					Debug.LogWarning("No products available for purchase!");
					break;
			}
		}

		
		/// <summary>
		/// This will be called when an attempted purchase fails.
		/// </summary>
		public void OnPurchaseFailed(Product item, PurchaseFailureReason reason)
		{
            if (isDebug) Debug.Log("IAPManager reports: PurchaseFailed. Error: " + reason);
            if (purchaseFailedEvent != null)
                purchaseFailedEvent(reason.ToString());
		}
		
		
		/// <summary>
		/// Overload for failed purchases with string error message.
		/// </summary>
        private static void OnPurchaseFailed(string error)
        {
            if (isDebug) Debug.Log("IAPManager reports: PurchaseFailed. Error: " + error);
            if (purchaseFailedEvent != null)
                purchaseFailedEvent(error);
        }
		

		/// <summary>
        /// This will be called after a restore attempt.
		/// </summary>
        private static void OnTransactionsRestored(bool success)
        {
			if(!success)
			{
				string error = "Restore failed.";
                if (isDebug) Debug.Log("IAPManager reports: " + error);
				if (purchaseFailedEvent != null)
					purchaseFailedEvent(error);
				return;
			}
			
            purchaseSucceededEvent("restore");
        }

	
		/// <summary>
        /// Returns a list of all upgrade ids associated to a product.
        /// </summary>
        public static List<string> GetIAPUpgrades(string productId)
        {
            List<string> list = new List<string>();
            IAPObject obj = GetIAPObject(productId);

            if (obj == null)
            {
                if (isDebug)
                    Debug.LogError("Product " + productId + " not found in IAP Settings. Make sure "
                                   + "to remove your app from the device before deploying it again!");
            }
            else
            {
                while (obj != null && !string.IsNullOrEmpty(obj.req.nextId))
                {
                    list.Add(obj.req.nextId);
                    obj = GetIAPObject(obj.req.nextId);
                }
            }
           
            return list;
        }


        /// <summary>
        /// Returns the last purchased upgrade id of a product,
        /// or the main product itself if it hasn't been purchased yet.
        /// </summary>
        public static string GetCurrentUpgrade(string productId)
        {
            if (!DBManager.isPurchased(productId))
                return productId;

            string id = productId;
            List<string> upgrades = GetIAPUpgrades(productId);

            for (int i = upgrades.Count - 1; i >= 0; i--)
            {
                if (DBManager.isPurchased(upgrades[i]))
                {
                    id = upgrades[i];
                    break;
                }
            }

            return id;
        }


        /// <summary>
        /// Returns the next unpurchased upgrade id of a product.
        /// </summary>
        public static string GetNextUpgrade(string productId)
        {
            string id = GetCurrentUpgrade(productId);
            IAPObject obj = GetIAPObject(id);

            if (!DBManager.isPurchased(id) || obj == null || string.IsNullOrEmpty(obj.req.nextId)) return id;
            else return obj.req.nextId;
        }
		

        /// <summary>
        /// Returns the global identifier of an in-app product,
        /// specified in the IAP Settings editor.
        /// </summary>
        public static string GetIAPIdentifier(string id)
        {
			//better check controller.products.all
			//and search for ProductDefinition.storeSpecificId?
		
            foreach (IAPObject obj in instance.IAPObjects.Values)
            {
                if (obj.isVirtual) continue;
				string local = "";
				
				for(int i = 0; i < obj.localIDs.Count; i++)
				{
					local = obj.localIDs[i].GetIdentifier();
					if(!string.IsNullOrEmpty(local) && local == id)
						return obj.id;
				}
            }

            return id;
        }


        /// <summary>
        /// Returns the list of currencies defined in the IAP Settings editor.
        /// </summary>
        public static List<IAPCurrency> GetCurrency()
        {
            return instance.currency;
        }


        /// <summary>
        /// Returns a string array of all IAP ids.
        /// Used by DBManager
        /// </summary>
        public static string[] GetIAPKeys()
        {
            string[] ids = new string[instance.IAPObjects.Count];
            instance.IAPObjects.Keys.CopyTo(ids, 0);
            return ids;
        }


        /// <summary>
        /// Returns the IAPObject with a specific id.
        /// </summary>
        public static IAPObject GetIAPObject(string id)
        {
            if (!instance || !instance.IAPObjects.ContainsKey(id))
                return null;
            return instance.IAPObjects[id];
        }


        /// <summary>
        /// Returns the group name of a specific product id.
        /// Used by DBManager
        /// <summary>
        public static string GetIAPObjectGroupName(string id)
        {
            if (instance.IAPObjects.ContainsKey(id))
            {
                IAPObject obj = GetIAPObject(id);
                //loop over groups to find the product id,
                //then return the name of the group
                for (int i = 0; i < instance.IAPs.Count; i++)
                    if (instance.IAPs[i].items.Contains(obj))
                        return instance.IAPs[i].name;
            }
			
            //if the corresponding group has not been found
            return null;
        }
    }


    /// <summary>
    /// Supported billing platforms.
    /// </summary>
    public enum IAPPlatform
    {
        GooglePlay = 0,
		IOSAppStore = 1,
		MacAppStore = 2,
		WindowsStore = 3,
        AmazonApps = 4
    }


    /// <summary>
    /// Remotely hosted config type.
    /// </summary>
    public enum RemoteType
    {
        none,
        cached,
        overwrite
    }


    /// <summary>
    /// IAP group properties.
    /// Each group holds a list of IAPObject.
    /// </summary>
    [System.Serializable]
    public class IAPGroup
    {
        public string id;
        public string name;
        public List<IAPObject> items = new List<IAPObject>();
    }


    /// <summary>
    /// IAP object properties.
    /// This is a meta-class for an IAP item.
    /// </summary>
    [System.Serializable]
    public class IAPObject
    {
        public string id;
        public List<IAPIdentifier> localIDs = new List<IAPIdentifier>();
        public bool fetch = false;
        public ProductType type = ProductType.Consumable;
        public string title;
        public string description;
        public string realPrice;
        public Sprite icon;
        public List<IAPCurrency> virtualPrice = new List<IAPCurrency>();
        public IAPRequirement req = new IAPRequirement();

		public bool isVirtual = false;
        public bool platformFoldout = false;

        public IDs GetIDs()
        {
			IDs productIDs = new IDs();
			
			for(int i = 0; i < localIDs.Count; i++)
			{
				string localId = localIDs[i].GetIdentifier();
				if(string.IsNullOrEmpty(localId))
					localId = id;
						
				productIDs.Add(localId, ((IAPPlatform)i).ToString());
			}
			
			return productIDs;
        }
    }


    /*
    /// <summary>
    /// ...
    /// </summary>
    [System.Serializable]
    public class VirtualPrice
    {
        public IAPCurrency currency = new IAPCurrency();
        public int amount;
    }
    */


    /// <summary>
    /// Local identifier for per store definitions.
    /// </summary>
    [System.Serializable]
    public class IAPIdentifier
    {
        public bool overridden = false;
        public string id;

        public string GetIdentifier()
        {
            if (overridden) return id;
            else return null;
        }
    }


    /// <summary>
    /// IAP currency, stored in the database.
    /// </summary>
    [System.Serializable]
    public class IAPCurrency
    {
        public string name;
        public int amount;
    }


    /// <summary>
    /// IAP unlock requirement, stored in the database.
    /// </summary>
    [System.Serializable]
    public class IAPRequirement
    {
		/// <summary>
		/// Database key name for the target value.
		/// </summary>
        public string entry;
		
		/// <summary>
		/// Value to reach for unlocking this requirement.
		/// </summary>
        public int target;
		
		/// <summary>
		/// Optional label text that describes the requirement.
		/// </summary>
        public string labelText;
		
		/// <summary>
		///	Product identifier for the following upgrade.
		/// </summary>
        public string nextId;
    }
}