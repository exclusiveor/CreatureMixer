using UnityEngine;
using UnityEngine.Purchasing;

public class ZIAPManager : IStoreListener {

	private IStoreController controller;
	private IExtensionProvider extensions;
	private bool _initialized;
	private bool _restoreStarted;

	private ProductCatalog catalog;

	public ZIAPManager () {
		_initialized = false;
		_restoreStarted = false;
		
		catalog = ProductCatalog.LoadDefaultCatalog();

		StandardPurchasingModule module = StandardPurchasingModule.Instance();
		module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;

		ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
		foreach (var product in catalog.allProducts) {
			if (product.allStoreIDs.Count > 0) {
				var ids = new IDs();
				foreach (var storeID in product.allStoreIDs) {
					ids.Add(storeID.id, storeID.store);
				}
				builder.AddProduct(product.id, product.type, ids);
			} else {
				builder.AddProduct(product.id, product.type);
			}
		}
		UnityPurchasing.Initialize (this, builder);
	}

	/// <summary>
	/// Called when Unity IAP is ready to make purchases.
	/// </summary>
	public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
	{
		this.controller = controller;
		this.extensions = extensions;
		_initialized = true;
		#if UNITY_IOS
		RestorePurchases ();
		#endif
	}

	/// <summary>
	/// Called when Unity IAP encounters an unrecoverable initialization error.
	///
	/// Note that this will not be called if Internet is unavailable; Unity IAP
	/// will attempt initialization until it becomes available.
	/// </summary>
	public void OnInitializeFailed (InitializationFailureReason error)
	{
		Debug.Log ("ZIAPManager.OnInitializeFailed=" + error);
	}

	/// <summary>
	/// Called when a purchase completes.
	///
	/// May be called at any time after OnInitialized().
	/// </summary>
	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e)
	{
		Debug.Log ("ZIAPManager.ProcessPurchase=" + e.purchasedProduct.definition.id);
        OnPurchaseProduct(e.purchasedProduct);
		return PurchaseProcessingResult.Complete;
	}

	/// <summary>
	/// Called when a purchase fails.
	/// </summary>
	public void OnPurchaseFailed (Product i, PurchaseFailureReason p)
	{
		Debug.Log ("ZIAPManager.OnPurchaseFailed=" + p);
	}

    public void OnPurchaseProduct(Product product)
    {
        if (product.definition.id.Contains("removeads"))
        {
            Debug.Log("ZIAPManager.ProcessPurchase=Restore ShowAds");
            GameManager.Instance.Settings.User.NoAds = true;
            GameManager.Instance.Ads.SetBannerVisible(false);
        }
        EventData e = new EventData("");
        e.Data["Product"] = product.definition.id;
        GameManager.Instance.EventManager.CallOnPurchaseCompleteEvent(e);
    }

	public bool Initialized() {
		return _initialized;
	}	

	public void RestorePurchases() {
		if (!_restoreStarted) {

			if (Initialized ()) {
				_restoreStarted = true;

				if (Application.platform == RuntimePlatform.IPhonePlayer || 
					Application.platform == RuntimePlatform.OSXPlayer || 
					Application.platform == RuntimePlatform.tvOS) {
					extensions.GetExtension<IAppleExtensions> ().RestoreTransactions (OnTransactionsRestored);
				}

			}
		}
	}

	void OnTransactionsRestored(bool success)
	{
		//Debug.Log("Transactions restored: " + success);
	}

	public void InitiatePurchase(string productID) {
		if (controller != null) {
			if (HasProductInCatalog (productID)) {
				#if UNITY_IOS
//				GameManager.Instance.EventManager.CallOnShowPurchaseFaderEvent ();
				#endif
				controller.InitiatePurchase (productID);
			} else {
				Debug.LogWarning("The product catalog has no product with the ID \"" + productID + "\"");
			}
		} else {
			Debug.LogWarning ("IAP controller not found");
		}
	}

	public bool HasProductInCatalog(string productID)
	{
		foreach (var product in catalog.allProducts) {
			if (product.id.ToLower() == productID.ToLower()) {
				return true;
			}
		}
		return false;
	} 

	public Product GetProduct(string productID)
	{
		if (controller != null) {
			return controller.products.WithID(productID);

		}
		return null;
	}

	public string GetFullProductName (string shortName) {
		string result = "";

		foreach (var product in catalog.allProducts)
		{
			if (product.id.ToLower().Contains(shortName.ToLower()))
			{
				result = product.id;
				break;
			}
		}
		return result;
	}
}
