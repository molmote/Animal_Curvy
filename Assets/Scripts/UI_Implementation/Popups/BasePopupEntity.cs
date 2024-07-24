using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BasePopupEntity : BaseWidgetEntity 
{
    protected   string                                  popupName       = "";
    protected   string                                  response        = POPUP_EVENT.POPUP_CLOSE;

    protected   PopupController.PopupCallback           Callback        = null;
    protected   PopupParam                              popupParameter  = null;

    private     UIPanel                                 uiPanel;
    public      bool                                    isUsingCurrentPosition = false;

    // public      UIEntityManager                         activeUI;
    // public      Transform                               parentUI;
    // public      string                                  parentName;

    protected override void Awake() 
    {
        base.Awake();
        
        this.enabled                = false;
        this.gameObject.name        = this.gameObject.name.Replace("(Clone)", "");
        this.popupName              = gameObject.name;     

        PopupController.instance.Register(this.popupName, this);

        gameObject.SetActive(false);

        if ( this.gameObject.GetComponent<AnimationEntity>() == null )
            this.gameObject.AddComponent<AnimationEntity>();

        // this.SetLayer(0);
    }

    protected virtual void Update()
    {        
        if( Input.GetKeyDown(KeyCode.Escape) )
        {     
            if ( PopupController.instance.IsPopupAvailableAtPeek(this.popupName) )
            {
                // PopupController.instance.CloseByAndroidBack();
                this.CloseByAndroidBack();
            }
        }
    }

	public virtual void Activate(int layerIndex,
                                 PopupParam popupParameter              = null, 
                                 PopupController.PopupCallback Callback = null,
                                 string animationName                   = "none")
    {
        if ( MainUIEntityManager.instance.gameObject.activeInHierarchy )
        {
            if ( this.isUsingCurrentPosition == false )
            this.transform.parent           = FindChild(MainUIEntityManager.instance.gameObject, "base").transform;
            this.SetLayerInChildren(LayerMask.NameToLayer("uiTitle"));
        }
        else 
        {
            this.transform.parent           = FindChild(GameUIManager.instance.gameObject, "center").transform;
            this.SetLayerInChildren(LayerMask.NameToLayer("ui"));
        }
        this.transform.localPosition    = Vector3.zero;
        this.transform.localScale       = Vector3.one;

        this.BeforeShowingAnimation();

        this.Callback       = Callback;
        this.popupParameter = popupParameter;

        // this.SetLayer(layerIndex);

		this.ShowPopupAnimation(animationName);
        this.SetEnableAll(true);    
        
        if(this.GetComponent<UIPanel>() != null)
        {
            this.uiPanel = this.GetComponent<UIPanel>();
            if(this.gameObject.activeInHierarchy == false)
                return;
            StartCoroutine(OnUpdateWidget());
        }
        
        //BaseController.instance.SetEffectCameraStatus(false);
	}

    public IEnumerator OnUpdateWidget()
    {
        yield return null;

        foreach(UIWidget w in this.uiPanel.widgets)
        {
            w.UpdateVisibility(true, true);
        }
    }

    public bool isClosing = false;

    public virtual void CloseByAndroidBack()
    {
        this.isClosing = true;
        MyLogger.Blue("CloseByAndroidBack", this.popupName );
        //PopupController.instance.Close(this.popupName);        
    }

	public virtual void DeactivateWithCallback(string animationName = "default", float _invoke = -1f)
    {
		this.HidePopupAnimation(animationName, _invoke);
        //BaseController.instance.SetEffectCameraStatus(true);
    }

    protected virtual void OnReady()
    {

    }

    protected UIButton GetButton(string name)
    {
        if(this.buttonDict.ContainsKey(name))
        {
            return this.buttonDict[name];
        }
        else
        {
            return null;
        }
    } 

    protected void SetEnableAll(bool isEnabled)
    {
        foreach(UIButton button in this.buttonList)
        {
            button.isEnabled = isEnabled;
        }
    }

    protected GameObject AddOnClickListener(string tag, 
                                            string name, 
                                            UIEventListener.VoidDelegate OnClick)
    {
        GameObject btn = this.FindChild(tag, name);
        this.AddOnClickListener(btn, OnClick);
        return btn;
    }

    protected GameObject AddOnClickListener(string name, UIEventListener.VoidDelegate OnClick)
    {
        GameObject btn = this.FindChild(name);
        this.AddOnClickListener(btn, OnClick);
        return btn;
    }

    //
    // Public Methods
    //
    public string GetName()
    {
        return this.popupName;
    }

    public bool CompareByName(string popupName)
    {
        return this.popupName == popupName;
    }

    public void SetLayer(int layerIndex)
    {
        float positionX                 = gameObject.transform.position.x;
        float positionY                 = gameObject.transform.position.y;
    }


	protected virtual void ShowPopupAnimation(string animationName)
	{
        AnimationEntity ani = gameObject.GetComponent<AnimationEntity>();

        if(ani == null || animationName == "none")
        {
            this.OnReady();
        }
        else
        {
            AnimationEntity.OnAnimationFinishDelegate OnShowAnimationFinished = delegate(AnimationEntity animationEntity)
            {
                this.OnReady();
            };

            ani.Play(animationName, OnShowAnimationFinished, true );
        }
	}

    private void PreCallBack()
    {        
        if(this.Callback != null)
        {
            this.Callback(this.response, this.popupParameter);
            this.Callback = null;
        }
    }
 
	protected virtual void HidePopupAnimation(string animationName, float invoke = -1f)
	{
        AnimationEntity ani = gameObject.GetComponent<AnimationEntity>();

        //MyLogger.Yellow("HidePopupAnimation","HIDE " + name);

        if(ani == null)
        {
            this.AfterHidingAnimation();
        }
        else
        {
            if ( invoke > -0.1f )
            {
                this.Invoke("PreCallBack", invoke);
            }
            
            AnimationEntity.OnAnimationFinishDelegate OnHideAnimationFinished = delegate(AnimationEntity animationEntity)
            {
                this.AfterHidingAnimation();
            };
            ani.Play(animationName, OnHideAnimationFinished, true );             
        }
	}

    //
    // Animation Callbacks
    //
    public void BeforeShowingAnimation()
    {
        gameObject.SetActive(true);
    }

    public virtual void AfterHidingAnimation()
    {
        this.isClosing = false;
        if(this.Callback != null)
        {
            this.Callback(this.response, this.popupParameter);
        }

        PopupController.instance.CheckAndEnableController();

        this.Callback       = null;
        this.popupParameter = null;

        gameObject.SetActive(false);
    }

    protected IEnumerator StartTween(long from, long to, float period, Action<long, float> updateAction)
    {      
        //long to float, be aware. Actually ticket is not needed to be long type
        float   startTime       = Time.time;
        float   endTime         = Time.time + period;
        long    difference      = to - from;
        float   deltaPerSecond  = (float)difference / period;
        
        while (Time.time < endTime)
        {   
            float   runnedTime          = Time.time - startTime;
            float   increasedValueFloat = (runnedTime * deltaPerSecond);
            long    increasedValue      = (long)(runnedTime * deltaPerSecond);
            updateAction(from + increasedValue, (float)from + increasedValueFloat);
            
            yield return null;
        }
        
        updateAction(to, (float)to);
    }
}