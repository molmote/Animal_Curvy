using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OptionTransitionEntity : BaseTransitionPopupEntity 
{
    /*
	protected override void Awake()
	{
        // MyLogger.Blue("DEBUG", gameObject.name + " Awake"); 
        base.Awake();
        this.LinkUI();

        this.isFocused = false;
	}

    private UILabel     labelVersion;
    private Animation   aniSound;
    private Animation   aniMusic;

	private void LinkUI()
	{
        this.labelVersion = this.GetComponent<UILabel>("labelVersion");

        this.AddOnClickListener("btnExit",	 		OnMenuClick);
        this.AddOnClickListener("spriteBackground", OnMenuClick);

        this.aniSound = this.AddOnClickListener("btnSound",          OnSoundClick).GetComponent<Animation>();
        this.aniMusic = this.AddOnClickListener("btnBgm",            OnMusicClick).GetComponent<Animation>();  

        this.AddOnClickListener("btnLanguage",       OnLanguageClick);      
        this.AddOnClickListener("btnFacebook",  	 OnFacebookShareClick);  
        this.AddOnClickListener("btnAsk",  			 OnAskMolamolaClick);  
        this.AddOnClickListener("btnReset",      	 OnDataResetBtnClick);  
        this.AddOnClickListener("btnRestore",        OnDataLoadClick);  
        this.AddOnClickListener("btnSpecialThanks",  OnThanks2BtnClick);  
	}

    private void OnEnable()
    {        
        this.isFocused = true;
        this.isClosing = false;
        // MyLogger.Blue("OnEnable", gameObject.name + " OnEnable"); 
        if ( GameSystemInfo.instance.IsSoundEnabled() )
        {
            this.aniSound.Play("btnOnIdle");
        }
        else
        {
            this.aniSound.Play("btnOffIdle");
        }
        
        if ( GameSystemInfo.instance.IsMusicEnabled() )
        {            
            this.aniMusic.Play("btnOnIdle");
        }
        else
        {            
            this.aniMusic.Play("btnOffIdle");
        }
    }

    private void OnDataLoadClick(GameObject go)
    {
    }

    private void OnLanguageClick(GameObject btn)
    {
        this.isFocused = false;
        MainUIEntityManager.instance.PlayHeadtoAnimation("transitionOptionToLanguage");
    }

    private void OnThanks2BtnClick(GameObject go)
    {
        this.isFocused = false;
        PopupController.instance.Open("thanks");
    }

    private void OnSoundClick(GameObject btn)
    {
        bool enable = GameSystemInfo.instance.IsSoundEnabled();

        if ( enable )
        {
            this.aniSound.PlayQueued("btnOff", QueueMode.PlayNow);
        }
        else
        {
            this.aniSound.PlayQueued("btnOn", QueueMode.PlayNow);
        }

        GameSystemInfo.instance.SetSoundEnable( !enable );
        GameSystemInfo.instance.Save();
        // MyLogger.Blue("DEBUG", gameObject.name + " OnSoundClick click"); 
    }

    private void OnMusicClick(GameObject btn)
    {
        bool enable = GameSystemInfo.instance.IsMusicEnabled();

        if ( enable )
        {
            this.aniMusic.PlayQueued("btnOff", QueueMode.PlayNow);
        }
        else
        {
            this.aniMusic.PlayQueued("btnOn", QueueMode.PlayNow);
        }

        GameSystemInfo.instance.SetMusicEnable( !enable );
        GameSystemInfo.instance.Save();
        // MyLogger.Blue("DEBUG", gameObject.name + " OnMusicClick click"); 
    }

    private void OnFacebookShareClick(GameObject btn)
    {
        // MyLogger.Blue("DEBUG", gameObject.name + " OnFacebookShareClick click"); 
        Application.OpenURL(Defines.FACEBOOK_HOME_URL);  
    }

    private void OnAskMolamolaClick(GameObject btn)
    {
        string email    = Defines.CONTACT_EMAIL_ADDRESS;
        string subject  = "Subject";
        string body     = "body";

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);    
        // MyLogger.Blue("DEBUG", gameObject.name + " OnAskMolamolaClick click"); 
    }

    private void OnDataResetBtnClick(GameObject btn)
    {
        PopupController.PopupCallback Reset = delegate( string response, PopupParam param )
        { 
            if ( param.condition != POPUP_EVENT.POPUP_ACCEPT )
                return;

            // will it reset cloud too?
            UserInfo.instance.ResetUserData ();
            string savedLocale = UserInfo.instance.GetLocale();
            UserInfo.instance.SetLocale(savedLocale);
            // if ( BuddyChangePopup.instance != null )
            //     BuddyChangePopup.instance.ReInitialize();
            
            this.OnMenuClick(null);
        }; 

        PopupController.instance.Open("UI Global YSE_NO", 
            new PopupParam(UserInfo.instance.GetLocal("40") ), Reset);
    }

    private void UpdateUI(PopupParam popupParam)
    {
        // GameInfo.instance.Pause();
    }

    protected override void OnMenuClick(GameObject btn)
    {
        UserInfo.instance.Save();
        MainUIEntityManager.instance.PlayHeadtoAnimation("transitionOptionDisappear");
    }
    */
}
