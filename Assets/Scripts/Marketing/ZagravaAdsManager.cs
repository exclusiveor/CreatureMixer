
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tapdaq;

public class ZagravaAdsManager : MonoBehaviour
{
    public bool Initialized = false;

    public void Debugger()
    {
        AdManager.LaunchMediationDebugger();
    }

    public bool NoAdsPurchased()
    {
        return GameManager.Instance.Settings.User.NoAds;
    }

    public void InitAds()
    {
        if (!NoAdsPurchased() && !Initialized)
        {
            TDCallbacks.RewardVideoValidated += OnRewardVideoValidated;
            TDCallbacks.AdClosed += OnAdClosed;
            TDCallbacks.AdAvailable += OnAdAvailable;
            TDCallbacks.AdNotAvailable += OnAdNotAvailable;
            TDCallbacks.TapdaqConfigLoaded += OnTapdaqConfigLoaded;

            AdManager.Init();
            //Debugger();
        }
    }

    private void OnTapdaqConfigLoaded()
    {
        TDCallbacks.TapdaqConfigLoaded -= OnTapdaqConfigLoaded;
        Initialized = true;
        Invoke("LoadBannerAndInterstitial", 1.0f);
    }

    public void LoadBannerAndInterstitial()
    {
        RequestBanner();
        PrepareInterstitial();
        //PrepareRewardedVideo();
    }

    public void RequestBanner()
    {
        if (!NoAdsPurchased())
        {
            AdManager.RequestBanner(TDMBannerSize.TDMBannerStandard);
        }
    }

    public void PrepareInterstitial()
    {
        if (!NoAdsPurchased())
        {
            AdManager.LoadVideo("maininterstitial");
        }
    }

    public void PrepareRewardedVideo()
    {
        AdManager.LoadRewardedVideo();
    }

    public void ShowInterstitial()
    {
        if (NoAdsPurchased())
        {
            GameManager.Instance.EventManager.CallOnInterstitialClosedEvent();
            return;
        }

        if (Initialized)
        {
            if (AdManager.IsVideoReady("maininterstitial"))
            {
                AdManager.ShowVideo("maininterstitial");
            }
            else
            {
                GameManager.Instance.EventManager.CallOnInterstitialClosedEvent();
            }
        }     
        else
        {
            GameManager.Instance.EventManager.CallOnInterstitialClosedEvent();
        }
    }

    public void ShowRewarded()
    {
        if (Initialized)
        {
            if (AdManager.IsRewardedVideoReady())
            {
                Debug.Log("Ready - showing rewarded video.");
                AdManager.ShowRewardVideo();
            }
            else
            {
                Debug.Log("Rewarded video is not ready");
                // show message when we haven't loaded video
            }
        }
    }

    private void OnRewardVideoValidated(TDVideoReward reward)
    {
        if (reward.RewardValid)
        {
            GameManager.Instance.EventManager.CallOnRewardedVideoClosedEvent();
        }
    }

    private void OnAdClosed(TDAdEvent e)
    {
        if (e.IsVideoEvent())
        {
            GameManager.Instance.EventManager.CallOnInterstitialClosedEvent();
            AdManager.LoadVideo("maininterstitial");
        }
        else if (e.IsRewardedVideoEvent())
        {
            AdManager.LoadRewardedVideo();
        }
    }

    public void SetBannerVisible(bool visible)
    {
        if (visible)
        {
            AdManager.ShowBanner(TDBannerPosition.Bottom);
        }
        else
        {
            AdManager.HideBanner();
        }
    }

    private void OnAdAvailable(TDAdEvent e)
    {
        if (e.adType == "BANNER")
        {
            AdManager.ShowBanner(TDBannerPosition.Bottom);
        }
    }

	private void OnAdNotAvailable(TDAdEvent e)
	{
		if (e.adType == "VIDEO" && e.tag == "maininterstitial")
		{
            // Video has failed to load
            Invoke("PrepareInterstitial", 2.0f);
		}	
        else if (e.adType == "REWARD_AD")
        {
            // Video has failed to load
            Invoke("PrepareRewardedVideo", 2.0f);
        }
		else if (e.adType == "BANNER")
		{
            //RequestBanner();
		}
	}
}

