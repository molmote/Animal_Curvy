using UnityEngine;
using System.Collections;

public class MyPopupEntity : BaseWidgetEntity 
{
    public delegate void VoidPopupDelegate();
    protected 	VoidPopupDelegate 		OnExitCallback;
    protected 	BaseWidgetEntity 		popupOpened;
	protected   string                  popupName       = "";
	static 		MyPopupEntity 			currentPopup;
    
	// Use this for initialization
	protected override void Awake()
	{
        base.Awake();
        this.popupName              = gameObject.name;     

        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
	}

	public static bool IsAnyPopupActive()
	{
		return ( currentPopup != null && currentPopup.gameObject.activeInHierarchy );
	}

	public static void OnCloseByAndroidCallback()
	{
		currentPopup.OnExitClick(null);
	}

	void OnEnable()
	{
		currentPopup = this;
	}

	protected virtual void OnExitClick(GameObject go)
	{
        this.gameObject.SetActive(false);//this.OnExitCallback();
	}

	public virtual void Show(BaseWidgetEntity popup)
	{
        this.popupOpened = popup;
        // this.popupOpened.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
	}

	/* from basepopupentity */
    public string GetName()
    {
        return this.popupName;
    }

    /*protected virtual void Update()
    {        
        if( Input.GetKeyDown(KeyCode.Escape) )
        {     
            if ( PopupController.instance.IsPopupAvailableAtPeek(this.popupName) )
            {
                // PopupController.instance.CloseByAndroidBack();
                this.CloseByAndroidBack();
            }
        }
    }*/

    public bool CompareByName(string popupName)
    {
        return this.popupName == popupName;
    }

    public bool isClosing = false;

    /*

    public virtual void CloseByAndroidBack()
    {
        this.isClosing = true;
        MyLogger.Blue("CloseByAndroidBack", this.popupName );
        //PopupController.instance.Close(this.popupName);        
    }*/

    // protected void SetEnableAll(bool isEnabled)
}
