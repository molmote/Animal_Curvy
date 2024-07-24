using UnityEngine;
using System.Collections;

public class UniversalPopupEntity : MyPopupEntity 
{
    private static UniversalPopupEntity _instance = null;
    public static UniversalPopupEntity instance 
    {
        get
        {
            return _instance;
        }
    }

	public void Show(BaseWidgetEntity popup)
	{
        this.popupOpened = popup;
        this.popupOpened.gameObject.SetActive(false);
		this.gameObject.SetActive(true);
	}

    public void ShowViedoReward(int diamond)
    {
        VoidPopupDelegate OnDisappear = delegate()
        {
            this.gameObject.SetActive(false);
            ResultPopupEntity.instance.OnRewardVideoClosed(true);
        };

        this.spriteRewardDiamond.gameObject.SetActive(false);
        this.labelRewardDiamond.text = ""+diamond;
        this.labelRewardDiamond.gameObject.SetActive(false);
        UserInfo.instance.BuyDiamond(diamond);
        UserInfo.instance.Save();

        this.OnExitCallback = OnDisappear;
        //this.popupOpened = popup;
        this.gameObject.SetActive(true);
    }

    // Use this for initialization
    protected override void Awake()
    {
        _instance = this;
        base.Awake();

        this.AddOnClickListener("Button Okay",  OnExitClick);
        this.labelRewardDiamond = this.GetComponent<UILabel>("Reward", "Label Reward Diamond");

        this.spriteRewardDiamond    = this.GetComponent<UISprite>("Reward", "Sprite Icon");
    }

    private         UISprite            spriteRewardDiamond;
    private         UILabel             labelRewardDiamond;
}
