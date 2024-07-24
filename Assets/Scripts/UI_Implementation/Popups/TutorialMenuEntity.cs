using UnityEngine;
using System.Collections;

public class TutorialMenuEntity : MyPopupEntity 
{
    private static TutorialMenuEntity _instance = null;
    public static TutorialMenuEntity instance 
    {
        get
        {
            return _instance;
        }
    }

	// Use this for initialization
	protected override void Awake()
	{
        _instance = this;
        base.Awake();
        
        this.animationFinger = this.GetComponent<Animation>("Finger");
        this.AddOnClickListener("Button Play",              OnStartClick);
        this.AddOnClickListener("Button Back",              OnStartClick);
	}

    public void Show()
    {
        // this.OnDisappear = OnDisappear;

        this.gameObject.SetActive(true);
        MyAdManager.instance.HideBanner();
    }

    public void AnimateFinger()
    {
        this.animationFinger.enabled = true;
        this.animationFinger.Stop(); 
        this.animationFinger.PlayQueued("Tutorial Finger", QueueMode.PlayNow);
    }

    protected virtual void Update()
    {        
        if( Input.GetKeyDown(KeyCode.Escape) )
        {     
            this.OnExitClick(null);
            //MyPopupEntity.OnCloseByAndroidCallback();
        }
    }

    protected override void OnExitClick(GameObject go)
    {
        this.gameObject.SetActive(false);
        GamePlayerEntity.instance.OnTutorialCanaceled();
        UserInfo.instance.gameplayTutorialInitiated = false;
        MyAdManager.instance.ShowBanner();
    }

    private void TriggerRealGame()
    {
        //MyAdManager.instance.ShowBanner();
        UserInfo.instance.gameplayTutorialInitiated = false;
        this.gameObject.SetActive(false);

        GamePlayerEntity.instance.SendEvent("GAME_RETRY");
        GamePlayerEntity.instance.myAnimator.SetTrigger("Start");
        GamePlayerEntity.instance.OnStartTrigger();
            GamePlayerEntity.instance.tutorialCount = 1;

        restart = false;
        MyAdManager.instance.ShowBanner();
    }

    protected void OnStartClick(GameObject go)
    {
        if ( restart )
            return;

        if ( false == GamePlayerEntity.instance.IsFsmState("Playing") )
        {
            restart = true;
            this.Invoke("TriggerRealGame", 1f);
        }
        else
        {
            this.TriggerRealGame();
        }
    }

    bool restart;

    Animation   animationFinger;
    AnimationEntity.OnAnimationFinishDelegate OnDisappear;
}
