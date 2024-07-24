using UnityEngine;
using System.Collections;

public class GlobalOkayPopup : BasePopupEntity
{
    private UILabel     labelContent;

	protected override void Awake()
	{
        base.Awake();
        this.LinkUI();
	}

	private void LinkUI()
	{
		this.AddOnClickListener("btnOkay", 		OnOkayClick);
        this.AddOnClickListener("btnExit",          OnExitClick);
        this.AddOnClickListener(this.gameObject,    OnExitClick);
        this.labelContent       = this.GetComponent<UILabel>("labelDescription");
	}

    public override void DeactivateWithCallback(string animationName = "default", float _invoke = -1f)
    {
    	base.DeactivateWithCallback("popupDisappear", _invoke);
    }

    private void UpdateUI(PopupParam popupParam)
    {
        if (popupParam != null )
        {
            this.labelContent.text = popupParam.condition;
        }
        
        GameInfo.instance.Pause();
    }

	public override void Activate(int layerIndex,
                                 PopupParam popupParameter              = null, 
                                 PopupController.PopupCallback Callback = null,
                                 string animationName                   = "default")
    {
        base.Activate(layerIndex, popupParameter, Callback, "popupAppear");
        this.UpdateUI(popupParameter);
    }

    private void OnOkayClick(GameObject btn)
    {
    	MyLogger.Blue("DEBUG", gameObject.name + " OnOkayClick click");
        this.popupParameter = new PopupParam(POPUP_EVENT.POPUP_ACCEPT);

        PopupController.instance.Close(this.popupName);
    }

    private void OnExitClick(GameObject btn)
    {
        MyLogger.Blue("DEBUG", gameObject.name + " OnExitClick click");
        this.popupParameter = new PopupParam(POPUP_EVENT.POPUP_CLOSE); //POPUP_DECLINE

        PopupController.instance.Close(this.popupName);
    }
    
}
