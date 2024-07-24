using System;
using UnityEngine;

using UnityEngine.Advertisements;
using CodeStage.AntiCheat.ObscuredTypes;

#if (UNITY_IPHONE && !UNITY_EDITOR)
    using System.Runtime.InteropServices;
#endif

public class MyAdManager : MonoBehaviour
{

    private static MyAdManager _instance = null;
    public static MyAdManager instance 
    {
        get
        {
            return _instance;
        }
    }

    private ObscuredInt             interstitialProb; 
    private int                     viedoProb;  

    public void OnPopupShown()
    {
        this.interstitialProb = 0;
    }

    public void OnVideoShown(bool shown)
    {
        this.interstitialProb = 0;
        if ( shown )
        {
            // UniversalPopupEntity.instance.ShowViedoReward(100);
            ResultPopupEntity.instance.OnRewardVideoClosed(true);
        }
        else
        {
            ResultPopupEntity.instance.OnRewardVideoClosed(false);
        }
    }

    public bool IsVideoAvailable()
    {
        #if UNITY_EDITOR 
            return false;
        #else

        //int saved = this.viedoProb;
        ////= Defines.VIDEO_RATE_BEFORE_REVIEW;
        
        //int prob = UnityEngine.Random.Range(0,100);
        this.viedoProb ++;
        
        if ( this.viedoProb >= Defines.AD_VIDEO_INTERVAL )
        {
            this.viedoProb = 0;
            if ( this.IsVideoAdLoaded() )
            {
                MyLogger.Log("IsVideoAvailable" , "Should show Video" ); 
                return true;
            }
        }
        return false;
        #endif
    }

    public void IncreasePopupProbability()
    {
        this.interstitialProb ++;
    }

    public bool IsPopupAvailable()
    {
        #if UNITY_EDITOR 
            return false;
        #endif

        if ( GameSystemInfo.instance.IsAdfreeVersion() )
            return false;

        if ( this.interstitialProb >= Defines.AD_INTERSTITIAL_INTERVAL )
        {
            return this.IsInterstitialLoaded();
        }
        MyLogger.RedAbsolute("No Popup", string.Format("count:{0}",this.interstitialProb) ); 
        return false;
    }

    void Awake() 
    {
        _instance = this;
    }

    private void FetchInterstitial()
    {
        if ( GameSystemInfo.instance.IsAdfreeVersion() )
            return;
    }

    private void FetchVideo()
    {
    }

    public  void ShowBanner()
    {
        if ( GameSystemInfo.instance.IsAdfreeVersion() )
            return;
    }

    public  void HideBanner()
    {
    }

    public  bool IsInterstitialLoaded()
    {
       return false;
    }

    public  void ShowInterstitial()
    {
    }

    private  bool IsVideoAdLoaded()
    {
       return true;
    }

    public  void RequestVideoAd()
    {
        #if UNITY_EDITOR
            this.OnVideoShown(true);
        #endif
    }

    public int videoFetchFailed;
    public int interstitialFetchFailed;
}
