using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIEntityManager : BaseEntityManager
{
	protected Dictionary<string, UIPanel>			panelDict;
	protected Dictionary<string, UIEventListener> 	buttonEventListenerDict;
	private	  Dictionary<string, BoxCollider> 		buttonUIColliderDict;
	private   Dictionary<string, UIButton> 			buttonUIDict;

	//<<-----------------------------------------------------------------------
	protected UIEntityManager()
	{
		//this.controller 	  					= null;
		this.buttonEventListenerDict 	  		= new Dictionary<string, UIEventListener>();
		this.panelDict 		  					= new Dictionary<string, UIPanel>();
		this.buttonUIColliderDict 				= new Dictionary<string, BoxCollider>();
		this.buttonUIDict 						= new Dictionary<string, UIButton>();
	}

	protected virtual void Awake()
	{
		if(GameObject.Find(Defines.LOADING_UI) == null)
		{
    		/*
    		MyLogger.Yellow("BaseController", "CLC"+this.gameObject.name);
			GameObject gameObj = Creater.Create(Defines.LOADING_UI);
			gameObj.transform.Find("loadingUI").
							  GetComponent<CommonLoadingController>().Initialize();
			gameObj.name 		= Defines.LOADING_UI;	
			*/		
		} 	

		MyLogger.Log("UIEntityManager", "SetTargetFrame "+Defines.TARGET_FPS);
		this.SetTargetFrame( Defines.TARGET_FPS );	
		QualitySettings.vSyncCount = 0;	
	}

	private void SetTargetFrame(int fps)
	{
		Application.targetFrameRate = fps;

#if UNITY_WEBGL
        Application.targetFrameRate = 0;
#endif
    }

    //<<-----------------------------------------------------------------------
    protected UIEventListener AddOnClickListener(string name,
												 UIEventListener.VoidDelegate OnClick)
	{
		GameObject btn 				= this.FindChild(this.gameObject, name);
		UIEventListener events		= null;
		BoxCollider 	collider 	= null;
		UIButton 		button 		= null;

		if(btn == null)
		{
			MyLogger.Red("DEBUG", "Button IS NOT FOUND in the panel");
		}
		else
		{
			collider 		 = btn.GetComponent<BoxCollider>();
			button 			 = btn.GetComponent<UIButton>();
			events			 = UIEventListener.Get(btn);

			events.onClick 	 = OnClick;
			this.buttonEventListenerDict.Add(name, events);
			this.buttonUIColliderDict.Add(name, collider);
			this.buttonUIDict.Add(name, button);
		}
		
		return events;
	}
	//<<-----------------------------------------------------------------------
	protected UIEventListener AddOnPressListener(string name,
												 UIEventListener.BoolDelegate OnPress)
	{
		GameObject btn 				= this.FindChild(this.gameObject, name);
		UIEventListener events		= null;
		BoxCollider 	collider 	= null;
		UIButton 		button 		= null;

		if(btn == null)
		{
			MyLogger.Red("DEBUG", "Button IS NOT FOUND in the panel");
		}
		else
		{
			collider 		 = btn.GetComponent<BoxCollider>();
			button 			 = btn.GetComponent<UIButton>();
			events			 = UIEventListener.Get(btn);

			events.onPress 	 = OnPress;
			this.buttonEventListenerDict.Add(name, events);
			this.buttonUIColliderDict.Add(name, collider);
			this.buttonUIDict.Add(name, button);
		}
		
		return events;
	}

	//<<-----------------------------------------------------------------------
	protected UIEventListener AddOnClickListener(string parentName,
												 string name,
												 UIEventListener.VoidDelegate OnClick)
	{
		GameObject 		btn 		= this.FindChild(this.gameObject, parentName, name);
		UIEventListener events		= null;
		BoxCollider 	collider 	= null;
		UIButton 		button 		= null;

		if(btn == null)
		{
			MyLogger.Red("DEBUG", "Button IS NOT FOUND in the panel");
		}
		else
		{
			collider 		 = btn.GetComponent<BoxCollider>();
			button 			 = btn.GetComponent<UIButton>();
			events			 = UIEventListener.Get(btn);
			events.onClick 	 = OnClick;
			this.buttonEventListenerDict.Add(name, events);
			this.buttonUIColliderDict.Add(name, collider);
			this.buttonUIDict.Add(name, button);
		}

		return events;
	}

	//<<-----------------------------------------------------------------------
	public UIPanel AddPanelObject(string name)
	{
		GameObject obj 			= this.FindChild(this.gameObject, name);
		UIPanel    panel        = obj.GetComponent<UIPanel>();

		if(panel == null)
		{
			MyLogger.Red("DEBUG", "Panel IS NOT FOUND in the panel");
		}
		else
		{
			this.panelDict.Add(name, panel);
		}

		return panel;
	}
	//<<-----------------------------------------------------------------------
	public void SetEnableAllButtons(bool isEnabled)
	{
		foreach(KeyValuePair<string, BoxCollider> valuePair in buttonUIColliderDict)
		{
			valuePair.Value.enabled = isEnabled;
		}

		foreach(KeyValuePair<string, UIEventListener> valuePair in buttonEventListenerDict)
		{
			valuePair.Value.enabled = isEnabled;
		}


		foreach(KeyValuePair<string, UIButton> valuePair in buttonUIDict)
		{
			valuePair.Value.enabled = isEnabled;
		}
	}
	//<<-----------------------------------------------------------------------
	protected void SetEnableButton(string buttonName, bool isEnabled)
	{
		this.buttonEventListenerDict[buttonName].enabled 	= isEnabled;
		this.buttonUIColliderDict[buttonName].enabled 		= isEnabled;
		this.buttonUIDict[buttonName].enabled 				= isEnabled;
	}

	//<<-----------------------------------------------------------------------
	public void SetActivePanel(string name, bool isActive)
	{
		this.panelDict[name].gameObject.SetActive(isActive);
	}

	//<<-----------------------------------------------------------------------
	public void SetActiveButton(string buttonName, bool isActive)
	{
		this.buttonEventListenerDict[buttonName].gameObject.SetActive(isActive);
	}
	
	//<<-----------------------------------------------------------------------
	public virtual void Clear()
	{
		this.buttonEventListenerDict.Clear();
		this.panelDict.Clear();
		this.buttonUIColliderDict.Clear();
		this.buttonUIDict.Clear();
		PopupController.instance.Clear();
	}

	protected virtual void OnControllerEnable()
	{

	}
	
	protected virtual void OnControllerDisable()
	{

	}


	public void SetControllerEnable(bool isEnabled)
	{
		if(isEnabled)
		{
            this.OnControllerEnable();
		}
		else
		{
            this.OnControllerDisable();
		}

		this.enabled = isEnabled;
	}	

	private void UpdateSliderValue(UISlider _slider, float _current, float _base)
	{
		if ( _current < 0 )
		{
			_current = 0;
		}
		_slider.value = _current / _base;	
	}

	private void UpdateStatusLabel(UILabel _label, float _current, float _base)
	{		
		if ( _current < 0 )
		{
			_current = 0;
		}
		_label.text   = Defines.CurrentStatusPerBaseFormat(Mathf.CeilToInt(_current), (int)_base );
	}

	private void UpdateStatusLabel(UILabel _label, string text)
	{
		_label.text   = text;
	}

}
