using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.Detectors;

// TARGET_OS_IPHONE
using UnityEngine.SocialPlatforms;
#if UNITY_ANDROID
#endif

public class MainUIEntityManager : BaseTransitionPopupEntity {

	private static MainUIEntityManager _instance = null;//new MainUIEntityManager();
	public  static MainUIEntityManager instance
	{
		get
		{
			return _instance;
		}
	}

	private MainUIEntityManager() 			
	{
		_instance = this;
	}

	/* Top right*/
	private UILabel 					currentJelly;

	private bool 						isUIInitialized 			= false;

	private Transform 					newCharacterSprite;
	private Transform 					newSpinSprite;
	private UILabel 					newSpinCountLabel;

	private Transform 					buddyTutorialTarget;
	private Transform 					characterTutorialTarget;

	private UILabel 					labelCurrentDiamond;
	private UILabel 					labelPlayCount;

	//<<-----------------------------------------------------------------------

    public INGAME_STATE currentState = INGAME_STATE.NOTREADY;	

    float startVolume = 0.2f;
    float volumeTimeSpan = 5;
    float volumeTimer = 5;

	[HideInInspector]
	public 	GamePlayerEntity 		player;

	private Transform 				newCharacterAvailable;
	private Transform 				spinBonusCount;

	private void LinkUI()
	{
		this.isUIInitialized 		= true;

        this.AddOnClickListener("Start",        			OnGameStartClick);

        this.AddOnClickListener("Button Character",        	OnCharacterClick);
        this.AddOnClickListener("Button Help",        		OnHelpClick);
        this.AddOnClickListener("Button Mission",        	OnMissionClick);
        this.AddOnClickListener("Button Shop",        		OnShopClick);
        
        // this.AddOnClickListener("Button Ranking",        	OnRankingClick);
        // 
        // this.AddOnClickListener("Button Bonus",        		OnBonusSpinClick);
        
    //     
    //     this.AddOnClickListener("Button Diamond",        	OnShopClick);
    //     this.AddOnClickListener("Button More Games",        OnMoreGameClick);
// 
        this.newCharacterSprite = this.GetComponent<Transform>("Button Character", "New");

        this.labelCurrentDiamond = this.GetComponent<UILabel>("Button Diamond", "Label Diamond");
        //this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();

	}

	public  void  		SetSoundButton( bool status )
	{
    	// UIToggle toggle = this.GetComponent<UIToggle>("Button Option");
    	// toggle.Set( status );
	}

	private void 		OnMoreGameClick(GameObject go)
	{
		SocialManager.instance.OpenCompanyLink();
	}

	private void 		OnHelpClick(GameObject go)
	{
		//MyAdManager.instance.HideBanner();
		UserInfo.instance.gameplayTutorialInitiated = true;
        GamePlayerEntity.instance.RestartGame();
        //GoogleAnalytics.Client.SendScreenHit("GamePlay-Tutorial");
	}

	private void 		OnGameStartClick(GameObject go)
	{
		UserInfo.instance.CheckTutorialTreshold();
		UserInfo.instance.CheckAndUpdateBuffBonus();

		UserInfo.SpinBonusBuffStat buffStat = UserInfo.instance.currentBuffStat;
		// Debug.Log("CheckAndUpdateBuffBonus: + " + Newtonsoft.Json.JsonConvert.SerializeObject(buffStat));

		
        GamePlayerEntity.instance.RestartGame();
        //GoogleAnalytics.Client.SendScreenHit("GamePlay-InGame");
	}

    public  void 	   	OnCharacterClick( GameObject go)
    {
    	this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Character UI") );
    }

    public  void 	  	OnFollowerClick( GameObject go)
    {
    	this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Follower UI") );
    }

    public  void 	 	OnMissionClick( GameObject go)
    {
    	this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Mission UI") );
    }

    public  void 		OnShopClick( GameObject go)
    {
    	this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Shop UI") );
    }

    public  void 	 	OnRankingClick( GameObject go)
    {
    	SocialManager.instance.ShowLeaderboard();
    }

    public  void 		OnBonusSpinClick( GameObject go)
    {
    	this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Spin UI") );
    }

	void Awake()
	{	
		ObscuredCheatingDetector.StartDetection(this.OnObscuredTypeCheatingDetected);
		SpeedHackDetector.StartDetection(OnSpeedHackDetected);

#if !UNITY_WEBGL
         InjectionDetector.StartDetection(OnInjectionDetected);
#endif

        _instance = this;
        base.Awake();
        this.gameObject.SetActive(true);

		this.SetTestValue();
		// SocialManager.InitSocial(null);
		// GameSystemInfo.instance.SetLocaleCode( (LanguageType)UserInfo.instance.GetLocaleCode() );

		GameInfo.instance.Reset();
		GameInfo.instance.LoadStageParameters();
		this.LinkUI();
	 	
	 	this.OnDataReset();
		SoundController.instance.Initialize();
	}

	private void OnInjectionDetected()
	{ 
		MyLogger.Log("Gotcha!", "OnInjectionDetected"); 
	}

	private void OnSpeedHackDetected() 
	{ 
		MyLogger.Log("Gotcha!", "OnSpeedHackDetected"); 
	}

	private void OnObscuredTypeCheatingDetected()
	{
		MyLogger.Log("Gotcha",  "OnObscuredTypeCheatingDetected!"); 
	}

	public void OnDataReset()
	{
	}

    public void StartFadeIn()
    {
    }

	private void SetTestValue()
	{		
		GameObject reporter = GameObject.Find("Reporter");

#if USE_TESTLOG
			MyLogger.logEnabled = true;
			reporter.SetActive(true);
#else
			//MyLogger.logEnabled = false;
			reporter.SetActive(false);
#endif
	}

	public  void SetIngameActive( bool enable )
	{
		this.GetComponent<Transform>("base").gameObject.SetActive(enable);
	}

	void Update()
	{
        this.volumeTimer += Time.deltaTime;
        if ( this.volumeTimer < this.volumeTimeSpan )
        {
            float volume = Mathf.Lerp( this.startVolume, 1f, this.volumeTimer/this.volumeTimeSpan );
            /// SoundManager.SetVolumeMusic(volume);
        }

	    if (Input.GetKeyDown(KeyCode.Escape)) 
	    	this.CloseByAndroidBack();

	    // this.isPopupExist = PopupController.instance.IsExistPopup();

		if( GameInfo.instance.IsGamePaused() )
			return;

		if( this.isUIInitialized == false )
		{
			this.LinkUI();
			return;
		}

		// this.UpdatePerFrame();
	}	

	void OnEnable()
	{
        GamePlayerEntity.instance.transform.localPosition    = Vector3.zero;
        // Camera.main.GetComponent<SmoothFollowEntity>().OnMainMenu();
        GamePlayerEntity.instance.ResetValueForAuto();
        
        if ( this.isUIInitialized == false )
        	return;
        	
        bool anySpecialCharacterUnlocked = false;

		for ( int index = 60 ; index < GameInfo.instance.GetCharacterTotal() ; index++ )
		{
			if ( UserInfo.instance.IsCharacterUnlockedRecently( index ))
			{
				anySpecialCharacterUnlocked = true;
			}
		}
        this.newCharacterSprite.gameObject.SetActive( anySpecialCharacterUnlocked );

        this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();

#if !UNITY_EDITOR
            //SocialManager.instance.CheckIfUserLikeUs(false);
#endif

		Screen.orientation = ScreenOrientation.AutoRotation;
		Screen.autorotateToLandscapeLeft 	= false;
		Screen.autorotateToLandscapeRight 	= false;
		Screen.autorotateToPortrait 		= true;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	public void OnMainMenuOpened()
	{
        this.gameObject.SetActive(true);
        // MyLogger.Red(this.gameObject.name, "TileController.instance.OnRestart");
        GamePlayerEntity.instance.SendEvent("ON_MAINMENU_AUTO");
        TileController.instance.OnRestart();

        //GoogleAnalytics.Client.SendScreenHit("MainMenu");
	}

    public void ShowWorldTransitionAnimation()
    {    	
    }

    private void CloseByAndroidBack()
    {
    }
}
