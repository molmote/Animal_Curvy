using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;

public enum DIA_TIER : int
{
    TIER1 = 2000,
    TIER2 = 10000,
    TIER3 = 30000,
    TOTAL = 50000    
};

public class ShopMenuEntity : BaseTransitionPopupEntity 
{
    private static ShopMenuEntity _instance = null;
    public static ShopMenuEntity instance 
    {
        get
        {
            return _instance;
        }
    }

    private string          bundleString = "test.ketchapp.animalrush.";
    private UILabel         labelCurrentDiamond;
    private AnimationEntity purchaseLoading;
    
    

    private int diamondBought;

    public void OnDiamondPurchased(int dia)
    {
        UserInfo.instance.BuyDiamond(dia);
        this.UpdateLabel();
    }

    public void ApplyBoost()
    {
        if ( GameSystemInfo.instance.IsAdfreeVersion() )
        {
            this.OnAdFreePurchased();
        }
        if ( GameSystemInfo.instance.IsMagneticForce() )
        {
            this.OnMagneticForcePurchased();
        }
        if ( GameSystemInfo.instance.IsDiamondBoosted() )
        {
            this.OnDoubleDiamondPurchased();
        }
        if ( GameSystemInfo.instance.IsMissionBoosted() )
        {
            this.OnMissionBoostPurchased();
        }
    }

    void Start()
    {
        this.ApplyBoost();
    }

    public void OnPurchaseFailed()
    {
        MyLogger.Red("OnPurchaseFailed", "OnPurchaseFailed");
        AnimationEntity.OnAnimationFinishDelegate OnFinishedDisappear = delegate(AnimationEntity ani)
        { 
            ani.gameObject.SetActive(false);
            PopupController.instance.Open("UI Global", new PopupParam( UserInfo.GetLocal("31") ) );
        };

        this.SetLoadingScreen(false, OnFinishedDisappear);
    }

    protected override void Awake()
    {
        _instance = this;
        base.Awake();
        this.LinkUI();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
        // this.ApplyBoost();
    }

    private void LinkUI()
    {
        this.labelCurrentDiamond = this.GetComponent<UILabel>("Label Diamond");

        this.AddOnClickListener("Button Back",                  OnMenuClick);
        this.AddOnClickListener("Button Restore",               OnRestorePurchases);
        this.AddOnClickListener("Remove Ad",           OnAdFreePurchaseClick);
        this.AddOnClickListener("Double Diamond",      OnDoubleDiamondPurchaseClick);
        this.AddOnClickListener("Mission Reward" ,     OnMissionRewardPurchaseClick);
        this.AddOnClickListener("Magnetic Power" ,     OnMagneticForcePurchaseClick);

        this.purchaseLoading    = this.GetComponent<AnimationEntity>("Loading UI");
        this.purchaseLoading.gameObject.SetActive(false);
    }

    public void OnRestorePurchases(GameObject go)
    {        
        #if UNITY_EDITOR
            this.Invoke("InvokeCloseLoading", 1f);
        #elif UNITY_IPHONE
            UM_InAppPurchaseManager.instance.RestorePurchases();
        #else
            this.Invoke("InvokeCloseLoading", 1f);
        #endif

        this.SetLoadingScreen(true, null);
    }

    private void InvokeCloseLoading()
    {
        this.SetLoadingScreen(false, null);
    }

    /* public  void OnResultRestore(ISN_Result res)
    {
        // GameSystemInfo.instance.ApplyBoost();
        this.SetLoadingScreen(false, null);
    }*/

    private void Purchase(string id)
    {
        AnimationEntity.OnAnimationFinishDelegate OnFinishedAppear = delegate(AnimationEntity ani)
        { 
            #if UNITY_EDITOR 
                this.OnPurchaseFailed();
            #endif
        };

        //OnFinishedAppear(null);
        this.SetLoadingScreen(true, OnFinishedAppear);
    }

    private void OnAdFreePurchaseClick( GameObject go )
    {
        this.Purchase(this.bundleString+"boostRemoveAds");
    }

    private void OnDoubleDiamondPurchaseClick( GameObject go )
    {
        this.Purchase(this.bundleString+"boostDoubleDiamond");
    }

    private void OnMissionRewardPurchaseClick( GameObject go )
    {
        this.Purchase(this.bundleString+"boostMissionReward");
    }

    private void OnMagneticForcePurchaseClick( GameObject go )
    {
        this.Purchase(this.bundleString+"boostMagnetic");
    }

    private void OnDiamondPurchaseBtnClick(GameObject diamond)
    {
        string id       = "";//this.itemList[index].id;
        string name     = diamond.name;
        if ( "01" == name )
        {
            this.diamondBought = (int)DIA_TIER.TIER1;
            id = this.bundleString+ "diamondPack1";
        }
        else if ( "02" == name )
        {
            this.diamondBought = (int)DIA_TIER.TIER2;
            id = this.bundleString+ "diamondPack2";
        }
        else if ( "03" == name )
        {
            this.diamondBought = (int)DIA_TIER.TIER3;
            id = this.bundleString+ "diamondPack3";
        }

        this.Purchase(id);
    }

    void UpdateLabel()
    {
        this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();
    }

    void OnEnable()
    {
        this.UpdateLabel();
    }

    public  void OnAdFreePurchased()
    {
        GameSystemInfo.instance.SetAdFree();

        this.SetLoadingScreen(false, null);

        GameObject purchased = this.FindChild("Remove Ad","Purchased");
        purchased.SetActive(true);
        purchased.transform.parent.GetComponent<BoxCollider>().enabled = false;

        if ( MyAdManager.instance != null )
        MyAdManager.instance.HideBanner();
    }

    public  void OnDoubleDiamondPurchased()
    {
        GameSystemInfo.instance.SetDiamondBoost();

        this.SetLoadingScreen(false, null);

        GameObject purchased = this.FindChild("Double Diamond","Purchased");
        purchased.SetActive(true);
        purchased.transform.parent.GetComponent<BoxCollider>().enabled = false;
    }

    public  void OnMissionBoostPurchased()
    {
        GameSystemInfo.instance.SetMissionBoost();

        this.SetLoadingScreen(false, null);

        GameObject purchased = this.FindChild("Mission Reward","Purchased");
        purchased.SetActive(true);
        purchased.transform.parent.GetComponent<BoxCollider>().enabled = false;
    }

    public  void OnMagneticForcePurchased()
    {
        GameSystemInfo.instance.SetMagneticForce();

        this.SetLoadingScreen(false, null);

        GameObject purchased = this.FindChild("Magnetic Power","Purchased");
        purchased.SetActive(true);
        purchased.transform.parent.GetComponent<BoxCollider>().enabled = false;
    }

    private void SetLoadingScreen(bool enable, AnimationEntity.OnAnimationFinishDelegate callback )
    {
        if ( enable )
        {
            this.purchaseLoading.gameObject.SetActive(true);

            this.purchaseLoading.Play("Loading Appear", callback);
        }
        else if ( callback == null)
        {            
            AnimationEntity.OnAnimationFinishDelegate OnFinishedDisAppear = delegate(AnimationEntity ani)
            { 
                ani.gameObject.SetActive(false);
                // PopupController.instance.Open("UI Global", new PopupParam( UserInfo.instance.GetLocal("30") ) );
            };
            this.purchaseLoading.Play("Loading Disappear", OnFinishedDisAppear);
        }
        else
        {
            this.purchaseLoading.Play("Loading Disappear", callback);
        }
    }
}

