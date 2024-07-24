using UnityEngine;
using System.Collections;

public class GlobalYesNoPopup : BasePopupEntity {

    private UILabel     labelContent;
    private GameObject  btnNever;

	protected override void Awake()
	{
        base.Awake();
        this.LinkUI();
	}

	private void LinkUI()
	{
		this.AddOnClickListener("btnYes", 		    OnYesClick);
        this.AddOnClickListener("btnNo",            OnNoClick);
        this.AddOnClickListener(this.gameObject,    OnNoClick);
        this.btnNever = this.AddOnClickListener("btnNever", OnNeverClick);
        this.labelContent       = this.GetComponent<UILabel>("labelDescription");
	}

    public override void DeactivateWithCallback(string animationName = "default", float _invoke = -1f)
    {
    	base.DeactivateWithCallback("popupDisappear", _invoke);
    }

    private void UpdateUI(PopupParam popupParam)
    {
        /*this.btnNever.SetActive(false);
        if (popupParam != null )
        {
            if ( popupParam.condition == "review")
            {
                this.btnNever.SetActive(true);
                this.labelContent.text = UserInfo.instance.GetLocal("32");
            }
            else
            {            
                this.labelContent.text = popupParam.condition;
            }
        }
        
        GameInfo.instance.Pause();*/
    }

	public override void Activate(int layerIndex,
                                 PopupParam popupParameter              = null, 
                                 PopupController.PopupCallback Callback = null,
                                 string animationName                   = "default")
    {
        base.Activate(layerIndex, popupParameter, Callback, "popupAppear");
        this.UpdateUI(popupParameter);
    }

    private void OnNeverClick(GameObject btn)
    {
        MyLogger.Blue("DEBUG", gameObject.name + " OnNeverClick click");
        UserInfo.instance.SetReviewed();
        this.OnNoClick(null);        
    }

    private void OnYesClick(GameObject btn)
    {
    	MyLogger.Blue("DEBUG", gameObject.name + " OnYesClick click");
        this.popupParameter = new PopupParam(POPUP_EVENT.POPUP_ACCEPT);

        PopupController.instance.Close(this.popupName);
    }

    private void OnNoClick(GameObject btn)
    {
        MyLogger.Blue("DEBUG", gameObject.name + " OnNoClick click");
        this.popupParameter = new PopupParam(POPUP_EVENT.POPUP_DECLINE); //POPUP_DECLINE

        PopupController.instance.Close(this.popupName);
    }
    
    public override void CloseByAndroidBack()
    {
        MyLogger.Blue("CloseByAndroidBack", this.popupName );
    }
}
