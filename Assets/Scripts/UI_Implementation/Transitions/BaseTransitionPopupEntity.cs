using UnityEngine;
using System.Collections;

public class BaseTransitionPopupEntity : BaseWidgetEntity 
{
    protected   PopupController.PopupCallback           Callback        = null;
    protected   AnimationEntity                         animationEntity;

    public      bool                            isClosing;
    // static      BaseTransitionPopupEntity       focused;
    public      bool                            isFocused;

    [HideInInspector]
    public  BaseTransitionPopupEntity backTo;

    protected override void Awake() 
    {        
    	base.Awake(); 
        this.isClosing = false;

        gameObject.SetActive(false);

        if ( this.gameObject.GetComponent<AnimationEntity>() == null )
            this.gameObject.AddComponent<AnimationEntity>();

        this.animationEntity = this.gameObject.GetComponent<AnimationEntity>();
    }

	public virtual void Activate( string animationName = "", PopupParam popupParameter = null )
    {
        this.gameObject.SetActive(true);

        this.Callback       = Callback;

        this.SetEnableAll(true);          
        this.UpdateUI();

        GameInfo.instance.Pause();
	}
    
    protected virtual void UpdateUI()
    {

    }

	public virtual void Close(string animationName = "default",
                              AnimationEntity.OnAnimationFinishDelegate OnHideAnimationFinished = null)
    {
        if ( OnHideAnimationFinished != null)
        {
            this.animationEntity.Play(animationName, OnHideAnimationFinished, true ); 
            // focused = null;
        }
        else
        {
            AnimationEntity.OnAnimationFinishDelegate deactiveAndResume = delegate(AnimationEntity ani)
            {
                this.gameObject.SetActive(false);
                GameInfo.instance.Resume();

                // focused = null;
            };
            this.animationEntity.Play(animationName, deactiveAndResume, true);    
        }
    }

    protected virtual void        OnDisappear()
    {
        this.gameObject.SetActive(false);
    }

    protected virtual void        EnterSubMenu(GameObject go)
    {
        GamePlayerEntity.instance.gameObject.SetActive(false);
        BaseTransitionPopupEntity target = go.GetComponent<BaseTransitionPopupEntity>();
        target.backTo = this;

        go.SetActive(true);
        LeanTween.alpha( this.gameObject, 1f, 0 ).setOnComplete(this.OnDisappear);
    }

    //deprecated since every 
    protected    virtual void        OnMenuClick( GameObject go)
    {
        this.isClosing = true;
        GameObject main = FindChild(GameUIManager.instance.gameObject, "Main");
        if ( this.backTo != null && this.backTo.gameObject != main )
        {
            backTo.gameObject.SetActive(true);
        }
        else
        {
            GamePlayerEntity.instance.gameObject.SetActive(true);
            main.SetActive(true);
            MyLogger.Red(this.gameObject.name, "TileController.instance.OnRestart");
            TileController.instance.OnRestart();
        }
        this.OnDisappear();
    }

    protected virtual void Update()
    {        
        if( Input.GetKeyDown(KeyCode.Escape) )
        {     
            this.CloseByAndroidBack();
        }
    }

    public virtual void CloseByAndroidBack()
    {        
        if ( MyPopupEntity.IsAnyPopupActive() )// || this.isClosing )
        {
            MyPopupEntity.OnCloseByAndroidCallback();
        }
        else
        {
            this.OnMenuClick(null);
            //this.isClosing = true;
        }
    }
}
