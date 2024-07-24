using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MissionMenuEntity : BaseTransitionPopupEntity 
{
    private UILabel     labelCurrentDiamond;

    private     UILabel                 labelMissionInfo;
    private     UILabel                 labelMissionGoal;
    private     UILabel                 labelRewardText;
    private     UILabel                 labelMissionNumber;
    
	protected override void Awake()
	{
        base.Awake();
        this.LinkUI();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
	}	

    private void LinkUI()
    {
        this.labelMissionInfo       = this.GetComponent<UILabel>("Label Mission Info");
        this.labelMissionGoal       = this.GetComponent<UILabel>("Label Mission Goal");
        this.labelRewardText        = this.GetComponent<UILabel>("Label Reward");
        this.labelMissionNumber     = this.GetComponent<UILabel>("Label Title");

        this.labelCurrentDiamond    = this.GetComponent<UILabel>("Label Diamond");

        // this.AddOnClickListener("Button Skip",              OnSkipToNextMission);
        this.AddOnClickListener("Button Back",              OnMenuClick);
        this.AddOnClickListener("Button Diamond",           OnShopClick);
    }

    public  void        OnShopClick( GameObject go)
    {
        this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Shop UI") );
    }

    void OnEnable()
    {
        this.UpdateUI();
        //GoogleAnalytics.Client.SendScreenHit("MissionMenu");
    }

    private void UpdateUI()
    {
        float lastScore = GamePlayerEntity.instance.targetScore;
        
        this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();

        MissionController.instance.GetMissionDescriptionFormat(false, this.labelMissionInfo);
        this.labelMissionGoal.text = MissionController.instance.GetMissionGoalFormat();
        MissionController.instance.GetMissionCompletedNumberFormat(this.labelMissionNumber);
        MissionController.instance.GetRewardFormat(this.labelRewardText);
    }
}

