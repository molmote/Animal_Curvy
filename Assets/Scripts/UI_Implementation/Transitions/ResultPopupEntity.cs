using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResultPopupEntity : BaseTransitionPopupEntity 
{
    private static ResultPopupEntity _instance = null;
    public static ResultPopupEntity instance 
    {
        get
        {
            return _instance;
        }
    }  

    private UILabel     labelBestScore;
    private UILabel     labelLastScore;
    private UILabel     labelCurrentDiamond;
    private UILabel     labelRibbonDiamond;
    private Animation   aniDiamondUpdate;

    private BoxCollider           btnUnlock;
    private AnimationEntity     aniUnlock;

    private BoxCollider           btnFreeGift;
    private AnimationEntity     aniFreeGift;
    private Transform           resultFreeGift;
    private UILabel             labelFreeGift;

    private BoxCollider           btnVideo;
    private Transform           resultVideoAd;

    private AnimationEntity     aniBackground;
    private AnimationEntity     aniBonusScene;
    private AnimationEntity     aniMainMenu;

    private Animation           aniBestScore;
    
	protected override void Awake()
	{
        _instance = this;
        base.Awake();
        this.LinkUI();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
	}	

    private void LinkUI()
    {
        this.labelBestScore         = this.GetComponent<UILabel>("Label Best Score");
        this.labelLastScore         = this.GetComponent<UILabel>("Label Score");
        this.aniBestScore           = this.GetComponent<Animation>("Label Score");
        this.labelCurrentDiamond    = this.GetComponent<UILabel>("Label Diamond");
        this.labelRibbonDiamond    = this.GetComponent<UILabel>("Diamonds", "Label Diamond");
        //this.aniDiamondUpdate       = this.GetComponent<Animation>("Diamonds");
        this.aniDiamondUpdate       = this.GetComponent<Animation>("Diamonds");

        this.AddOnClickListener("Button Share",             OnShareButtonClick);
        this.AddOnClickListener("Button Retry",             OnMenuClick);
        
        this.aniUnlock   = this.GetComponent<AnimationEntity>("Button Character Unlock");
        this.btnUnlock   = this.GetComponent<BoxCollider>    ("Button Character Unlock");
        this.AddOnClickListener("Button Character Unlock",  OnCharacterUnlock);


        this.aniFreeGift = this.GetComponent<AnimationEntity>("Button Free Gift");
        this.btnFreeGift = this.GetComponent<BoxCollider>    ("Button Free Gift");
        this.AddOnClickListener("Button Free Gift",         OnFreeGiftClick);

        this.labelFreeGift = this.GetComponent<UILabel>("Label Free Gift Amount");
        this.resultFreeGift = this.GetComponent<Transform>("Free Gift Reward");

        this.AddOnClickListener("Button Video",             OnWatchVideoAd);
        this.btnVideo       = this.GetComponent<BoxCollider>("Button Video");
        this.resultVideoAd  = this.GetComponent<Transform>("Video Reward");

        this.aniMainMenu            = this.GetComponent<AnimationEntity>(); // GameOverAppear
        this.aniBonusScene          = this.GetComponent<AnimationEntity>("Bonus"); 
        this.aniBackground          = this.GetComponent<AnimationEntity>("Background"); 
    }

    private void        OnHelpClick(GameObject go)
    {
        UserInfo.instance.gameplayTutorialInitiated = true;
        GamePlayerEntity.instance.RestartGame();
        GamePlayerEntity.instance.myAnimator.SetTrigger("Idle");
    }

    private void IncreaseAndUpdateRibbon(float start, float end)
    {
        float diff  = end - start;

        if ( diff == 0 )
        {
            this.labelRibbonDiamond.text   = this.GetDiaUpdateFormat ( Mathf.CeilToInt(start) );
            this.backup = start;
        }
        else
        {
            //MyLogger.Red("IncreaseAndUpdateRibbon", ""+diff);
            this.diaTimer       = 0;
            float timeSpan      = Mathf.Lerp ( 0.3f, 1f, diff / 100 );
            this.diaTimeSpan    = timeSpan / (diff+1);
            if ( diaTimeSpan < 0.07f )      // 4 ~ 15
                this.diaTimeSpan = 0.07f;
            LeanTween.value( this.gameObject, start, end, timeSpan )
            .setOnUpdate(this.OnUIValueChange);

            this.aniDiamondUpdate.Play("Game Over Diamond");
        }
    }

    public  void UpdateDiamondLabel()
    {
        int dia = Mathf.CeilToInt(UserInfo.instance.GetCurrentDiamond());
        this.labelCurrentDiamond.text = ""+dia;
        this.IncreaseAndUpdateRibbon(this.backup,dia);
    }

    float backup;

    private void OnUIValueChange(float dia)
    {
        // MyLogger.Red("OnUIValueChange","dia :"+dia);
        this.diaTimer                += Time.deltaTime;
        if ( this.diaTimer > this.diaTimeSpan )
        {
            this.diaTimer = 0;
            SoundController.instance.Play("Diamond Gain");
        }
        this.labelRibbonDiamond.text   = this.GetDiaUpdateFormat( Mathf.CeilToInt(dia) );
        this.backup = dia;
    }

    float diaTimeSpan;
    float diaTimer;
    int  diaBefore;
    int  diaBonus;

    private string GetDiaUpdateFormat(int dia)
    {
        return string.Format("{0}/500", dia);
    }

    private bool newRecord;

    public void OnMainAppear()
    {
        AnimationEntity.OnAnimationFinishDelegate OnGameOverAppeared = delegate(AnimationEntity ani)
        { 
            if ( this.newRecord )
            {
                // this.aniBestScore.PlayQueued("Score", QueueMode.PlayNow);
                this.newRecord = false;
            }
        };

        this.aniMainMenu.Play("Game Over Appear", OnGameOverAppeared);
        int     lastScore = (int)GamePlayerEntity.instance.targetScore;
        int     combo     = GamePlayerEntity.instance.maxCombo;

        int bestScore = UserInfo.instance.ChangeBestScore(lastScore);
        if ( bestScore == lastScore )
            this.newRecord = true;

        UserInfo.instance.SetBestText( this.labelBestScore, bestScore );
        this.labelLastScore.text        = "" + lastScore;

        SocialManager.instance.UpdateLeaderboardOnGameOver(
            lastScore, combo, UserInfo.instance.GetPlayCountTotal() );

        SocialManager.instance.UpdateGameResultAchievements(
            lastScore, combo, MissionController.instance.completedCount );

        UserInfo.instance.MakeSpecialCharacterAvailable();
        
        UserInfo.instance.BuyDiamond( GamePlayerEntity.instance.currentDiamond);
        int dia = Mathf.CeilToInt(UserInfo.instance.GetCurrentDiamond());
        this.labelCurrentDiamond.text   = "" + dia;
        this.labelRibbonDiamond.text    = this.GetDiaUpdateFormat(dia);
        this.backup = dia;

        GameUIManager.instance.ResetDeadTrigger();
    }

    private bool CheckVideoAdAvailable()
    {
        return MyAdManager.instance.IsVideoAvailable();
    }

    private bool IsFreeGiftAvailable()
    {
        this.diaBonus = UserInfo.instance.GenerateFreeGift();
        return ( 0 != this.diaBonus );
    }

    private bool IsCharacterUnlockAvailable()
    {
        return ( UserInfo.instance.GetCurrentDiamond() + GamePlayerEntity.instance.currentDiamond 
            >= Defines.CHARACTER_UNLOCK_COST );
    }

    private void Hide()
    {
        ResultPopupEntity.instance.GetComponent<UIPanel>().alpha = 0;
    }

    // covers two cases ; other conditions for special unlock || dia reaches 100
    private void        OnCharacterUnlock(GameObject go)
    {
        //int index = UserInfo.instance.SpecialCharacterAvailable();
        
        UserInfo.instance.Consume(500);
        this.UpdateDiamondLabel();
        int index = UserInfo.instance.GetAvailableRandomCharacterIndex();

        AnimationEntity.OnAnimationFinishDelegate OnUnlockBtnAnimation = delegate(AnimationEntity ani)
        { 
            UnlockMenuEntity.instance.Show( index );
        };
        
        this.btnUnlock.enabled = false;
        this.aniUnlock.Play("Button Character Unlock", null);
        this.aniUnlock.RegisterEvent("Button Character Unlock",OnUnlockBtnAnimation);
        this.Invoke("Hide", 2f);

        this.isUnlockButtonClicked = true;
    }

    private void        OnFreeGiftClick(GameObject go)
    {
        AnimationEntity.OnAnimationFinishDelegate OnFreeGift = delegate(AnimationEntity ani)
        { 
            UserInfo.instance.BuyDiamond(this.diaBonus);
            this.UpdateDiamondLabel();
            this.resultFreeGift.gameObject.SetActive(true);
            UserInfo.instance.OnReceivedFreeGift();
            this.diaBonus = 0;
        };
            this.btnFreeGift.enabled = false;
        this.aniFreeGift.PlayAndDeactive("Button Character Unlock");
        this.aniFreeGift.RegisterEvent("Button Character Unlock",OnFreeGift);
    }

    private void        OnWatchVideoAd( GameObject go )
    {
            this.btnVideo.enabled = false;
        MyAdManager.instance.RequestVideoAd();
    }

    public  void        OnCharacterConfirmed(bool isNew)
    {
        if (!isNew)
        {
            GamePlayerEntity.instance.SetIdle();
        }
        this.BackToMainMenu();
        
    }

    private void        BackToMainMenu()
    {
        AnimationEntity.OnAnimationFinishDelegate OnDisappearMain = delegate(AnimationEntity ani)
        { 
            this.GetComponent<UIPanel>().alpha = 1;
            this.gameObject.SetActive(false);
        };

        AnimationEntity.OnAnimationFinishDelegate OnAnimationEvent = delegate(AnimationEntity ani)
        { 
            MainUIEntityManager.instance.OnMainMenuOpened();
        };
        this.aniMainMenu.Play("Game Over Disappear", OnDisappearMain);
        this.aniMainMenu.RegisterEvent("Game Over Disappear", OnAnimationEvent);

        this.isClosing = true;
    }

    protected    override void        OnMenuClick( GameObject go)
    {
        if ( this.isUnlockButtonClicked )
            return;
        GamePlayerEntity.instance.SetIdle();
        this.BackToMainMenu();
    }

    bool isVideoAvailable;

    void OnEnable()
    {
        // this.aniDiamondUpdate.gameObject.SetActive(false);
        this.btnUnlock.gameObject.SetActive(false);
        this.btnVideo.gameObject.SetActive(false);
        this.btnFreeGift.gameObject.SetActive(false);
        this.resultFreeGift.gameObject.SetActive(false);
        this.resultVideoAd.gameObject.SetActive(false);
        bool isFreeGift = this.IsFreeGiftAvailable();
        this.isVideoAvailable = this.CheckVideoAdAvailable();
        this.isUnlockButtonClicked = false;

        if ( this.IsCharacterUnlockAvailable() || isFreeGift || this.isVideoAvailable )
        {
            if ( MyPopupEntity.IsAnyPopupActive() )
            {
                MyLogger.Red("ReviewPopupENtity", "Review POpup detected");
            }
            else
            {
                this.OnReviewFinished();
            }
        }
        else
        {
            if ( MyPopupEntity.IsAnyPopupActive() )
            {
                MyLogger.Red("ReviewPopupENtity", "Review POpup detected");
            }
            else
            {
                this.aniBackground.Play("Game Over Background Appear", null);
                this.OnMainAppear();
            }
        }

        // GoogleAnalytics.Client.SendScreenHit("GameOverMenu");
    }

    public  void        OnReviewFinished()
    {
        bool isFreeGift = this.diaBonus != 0;
        if ( this.IsCharacterUnlockAvailable() || isFreeGift || this.isVideoAvailable )
        {
            AnimationEntity.OnAnimationFinishDelegate OnFinishedAppear = delegate(AnimationEntity ani)
            { 
                this.OnMainAppear();
            };
            this.aniBackground.Play("Game Over Background Bonus Appear", OnFinishedAppear);
            this.aniBonusScene.gameObject.SetActive(true);
            this.aniBonusScene.Play("Game Over Bonus Appear", null);

            if ( this.IsCharacterUnlockAvailable() ) 
            {
                this.btnUnlock.gameObject.SetActive(true);
                this.btnUnlock.enabled = true;
            }
            else if ( isFreeGift )
            {
                this.btnFreeGift.gameObject.SetActive(true);
                this.btnFreeGift.enabled = true;
                //  this.diaBonus = 490;
                this.labelFreeGift.text = "" + diaBonus;
            }
            else if ( this.isVideoAvailable )
            {
                this.btnVideo.gameObject.SetActive(true);
                this.btnVideo.enabled = true;
            }
        }
        else
        {
            // this.aniDiamondUpdate.gameObject.SetActive(true);
            AnimationEntity.OnAnimationFinishDelegate OnBackgroundAppear = delegate(AnimationEntity ani){};
            this.aniBackground.Play("Game Over Background Appear", OnBackgroundAppear);
            this.OnMainAppear();
        }
    }

    private void        OnShareButtonClick( GameObject go )
    {
        SocialManager.instance.GameOverShare( (int)GamePlayerEntity.instance.targetScore );
    }

    public  void        OnAchievementClick( GameObject go)
    {
        SocialManager.instance.ShowAchievement();
    }

    public  void        OnRankingClick( GameObject go)
    {
        SocialManager.instance.ShowLeaderboard();
    }

    public  void        OnOptionClick( GameObject go)
    {
        GameSystemInfo.instance.ToggleSound();
    }

    float expGained;

    bool  isRankingChecked = true;
    bool  isUnlockButtonClicked;

    public  void OnRewardVideoClosed(bool shown)
    {
        if ( shown )
        {
            // this.resultVideoAd.text = ""+100;
            this.btnVideo.gameObject.SetActive(false);
            this.resultVideoAd.gameObject.SetActive(true);
            UserInfo.instance.BuyDiamond(100);
            this.UpdateDiamondLabel();
            UserInfo.instance.Save();
        }
        else
        {

        }
    }

    /*
    public  void OnInterstitialClosed()
    {   
        this.SetButtonActive(true);
        // MyAdManager.instance.OnPopupShown();
        this.StartUpdateUI();
    }*/
}

