using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System;

#if UNITY_ANDROID 
using UnityEngine.SocialPlatforms;
#endif

// this also includes analystics.
public class SocialManager : MonoBehaviour
{	
    private static SocialManager _instance = null;
    public static SocialManager instance 
    {
        get
        {
            return _instance;
        }
    }

    static bool isConnected = false;

    void Awake()
    {
    	_instance = this;

    	
    }

    void Start()
    {
    	#if UNITY_IPHONE
    	this.InitSocial();
    	#elif UNITY_ANDROID
    		this.Invoke("InitSocial", 0.5f);
    	#endif
    }

	void Update()
	{

	}
	
	public  delegate void OnInitFinishDelegate();
	static public  OnInitFinishDelegate onSocialInitFinish;


	private  void OnPlayerConnected() 
	{
		// UM_ExampleStatusBar.text = "Player Connected";
		MyLogger.Red("SocialManager","OnPlayerConnected");
		isConnected = true;

		if ( onSocialInitFinish != null )
		{
			onSocialInitFinish();
			onSocialInitFinish = null;
		}   	
	}	

	private  void OnPlayerDisconnected() 
	{
		// UM_ExampleStatusBar.text = "Player Disconnected";
		MyLogger.Red("SocialManager","OnPlayerDisconnected");
		isConnected = false;
	}

	public  void GameOverShare(int score)
	{
		string desc = UserInfo.GetLocal("40");
		desc = desc.Replace("(x)", ""+score);
		this.ShareScreenShot( "Animal Rush", 
			string.Format("{0}{1}{2}", desc, Environment.NewLine, this.GetAppStoreLink() ) );
	}

	public  void UnlockShare(string animal)
	{
		string desc = UserInfo.GetLocal("41");
		desc = desc.Replace("(x)", animal);
		this.ShareScreenShot( "Animal Rush", 
			string.Format("{0}{1}{2}", desc, Environment.NewLine, this.GetAppStoreLink() ) );
	}

	private void OnScreenshotReady( Texture2D shot, string caption, string message )
	{		
		// UM_ShareUtility.ShareMedia(caption, message, shot);
	}

	public void ShareScreenShot(string caption, string message)
	{
		StartCoroutine(SaveScreenshotForShare(caption, message));
	}

	private IEnumerator SaveScreenshotForShare(string caption, string message) 
	{		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();

		OnScreenshotReady(tex, caption, message);
	}
	
	private  void OnFocusChanged(bool focus) 
	{
		if (!focus)  
		{                       
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		} else  
		{                                                                                       
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}   
	}

	public   void InitSocial( )
	{
	}

	bool isAppTwitterInstalled;
	bool isAppFacebookInstalled;
    
    public void  OpenFacebookURL()
    {
        // Application.OpenURL(Defines.FACEBOOK_HOME_PROFILE);
        #if UNITY_IPHONE
        	if ( this.isAppFacebookInstalled)
        	{
            	IOSSharedApplication.instance.OpenUrl("fb://profile/503287153144438");
        	}
        	else
        	{
            	Application.OpenURL(Defines.FACEBOOK_HOME_URL);  
        	}
        #elif UNITY_ANDROID
        	if ( this.isAppFacebookInstalled )
        	{
        		Application.OpenURL("fb://page/503287153144438");  
        	}
        	else
        	{
            	Application.OpenURL(Defines.FACEBOOK_HOME_URL);  
        	}
        #endif
    }

    public void  OpenTwitterLink()
    {
    	string address = "twitter:///user?screen_name=ketchappgames";
        #if UNITY_IPHONE
        	if ( this.isAppTwitterInstalled )
            	IOSSharedApplication.instance.OpenUrl( address );
        	else 
        		Application.OpenURL ( "https://twitter.com/ketchappgames" );
        #elif UNITY_ANDROID
        	//AndroidNativeUtility.RunPackage("com.twitter.android");
        	if ( this.isAppTwitterInstalled )
        		Application.OpenURL ( "twitter://user?screen_name=ketchappgames" );
        	else 
        		Application.OpenURL ( "https://twitter.com/ketchappgames" );

        #endif
    }

    public void  OpenReviewLink()
    {
        #if UNITY_IPHONE
        MyLogger.Red("OpenReviewLink","OpenReviewLink");
        if ( this.CheckIfiOS7() )
        {
            //IOSSharedApplication.instance.OpenUrl("itms://itunes.apple.com/app/apple-store/id954809346?mt=8");
            IOSSharedApplication.instance.OpenUrl(this.GetAppStoreLink());
        }
        else
        {
            IOSSharedApplication.instance.OpenUrl("http://itunes.apple.com/WebObjects/MZStore.woa/wa/viewSoftware?id=954809346&mt=8");
        }
        #elif UNITY_ANDROID
        	Application.OpenURL ("market://details?id=com.molamola.molamolajump");
        #endif
    }

    public bool CheckIfiOS7()
    {
        float osVersion = -1f;
        string versionString = SystemInfo.operatingSystem.Replace("iPhone OS ", "");
        // MyLogger.Red("OpenReviewLink", versionString);
        float.TryParse(versionString.Substring(0, 2), out osVersion);

        // MyLogger.Red("OpenReviewLink", ""+osVersion);
    	return osVersion >= 7f;
    }

    public void OpenCompanyLink()
    {
        #if UNITY_IPHONE
        if ( this.CheckIfiOS7() )
        {
            IOSSharedApplication.instance.OpenUrl("itms-apps://itunes.apple.com/artist/ketchapp/id528065807");
        }
        else
        {
        	IOSSharedApplication.instance.OpenUrl("itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewArtist?id=528065807&mt=8");
        }
        #elif UNITY_ANDROID
        	Application.OpenURL ("https://play.google.com/store/apps/developer?id=Ketchapp");
        #endif
    }

    public string GetAppStoreLink()
    {
        #if UNITY_IPHONE            
        	return "https://itunes.apple.com/app/get-bigger!-mola/id954809346" ;
        #elif UNITY_ANDROID
        	return "https://play.google.com/store/apps/details?id=com.molamola.molamolajump";
        #endif
        	return "error";
    }

	public   void UpdateLeaderboardOnGameOver(int score, int combo, int playCount)
	{			
	}

	public   void ShowLeaderboard()
	{
	}

	public   void ShowAchievement()
	{		
	}

	public   void UpdateUnlockAchievements()
	{
		int unlocked = 0;
		for ( int i = 0 ; i < GameInfo.instance.GetCharacterInfoSize() ; i++ )
		{ 
			if ( false == UserInfo.instance.IsCharacterLocked(i) )
			unlocked ++;
		}

		ReportAchievements( unlocked >= 2,  "animalUnlockNew");
		ReportAchievements( unlocked >= 20, "animalUnlock20");
		ReportAchievements( unlocked >= 40, "animalUnlock40");
		ReportAchievements( unlocked >= 60, "animalUnlock60");
	}

	public 	 void UpdateGameResultAchievements( int score, int combo, int mission )
	{
		ReportAchievements( score >= 100,  "score100");
		ReportAchievements( score >= 200,  "score200");
		ReportAchievements( score >= 400,  "score400");
		ReportAchievements( score >= 700,  "score700");
		ReportAchievements( score >= 1000, "score1000");

		ReportAchievements( combo >= 10,   "combo10");
		ReportAchievements( combo >= 20,   "combo20");
		ReportAchievements( combo >= 40,   "combo40");

		ReportAchievements( mission >= 20, 		"mission20");
		ReportAchievements( mission >= 50, 		"mission50");
		ReportAchievements( mission >= 100,		"mission100");
		ReportAchievements( mission >= 150,		"mission150");
	}

	public   void UpdateGamePlayAchievements(int playCount)
	{
		ReportAchievements( playCount >= 100,  "play100");
		ReportAchievements( playCount >= 500,  "play500");
		ReportAchievements( playCount >= 1000, "play1000");
	}

	public static void ReportAchievements( bool condition, string missionid )
	{
		if ( condition )
		{
			// MyLogger.Blue("ReportAchievements", string.Format("{0} Available", missionid));
			// UM_GameServiceManager.instance.IncrementAchievement(missionid, 100f);
		}
	}
}
