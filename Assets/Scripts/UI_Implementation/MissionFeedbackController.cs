using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MissionFeedbackController : BaseWidgetEntity
{
    public      Queue<string> 		noticeQueue     		= new Queue<string>();
    private     string 			    currentNotification		= null;
    
    private     AnimationEntity         missionStart;
    private     UILabel                 missionStartNumber;
    private     UILabel                 missionStartDesc;
    private     UILabel                 missionStartGoal;

    private     AnimationEntity         missionCount;
    private     UILabel                 missionCountLabel;

    private     AnimationEntity         missionEnd;
    private     UILabel                 missionEndLabel;
    private     UILabel                 missionEndNumber;
    private     UILabel                 missionEndReward;
    private     UILabel                 newMissionReward;

    public      void ShowMissionStart()
    {
        if ( UserInfo.instance.gameplayTutorialInitiated == false )
        {
            this.missionStart.gameObject.SetActive(false);
            this.missionStart.gameObject.SetActive(true);
            MissionController.instance.GetMissionDescriptionFormat(true, this.missionStartDesc);
            MissionController.instance.GetMissionCompletedNumberFormat(this.missionStartNumber);
            this.missionStartGoal.text = MissionController.instance.GetMissionGoalFormat();
        }
    }

    public      void ShowMissionEnd(AnimationEntity.OnAnimationFinishDelegate OnDisappear)
    {
        AnimationEntity.OnAnimationFinishDelegate half = delegate(AnimationEntity animationEntity)
        {
            MissionController.instance.OnMissionCompleted();
            MissionController.instance.GetMissionDescriptionFormat(true, this.missionEndLabel);
            MissionController.instance.GetMissionCompletedNumberFormat(this.missionEndNumber);
            MissionController.instance.GetRewardFormat(this.newMissionReward);
        };

        this.missionEnd.gameObject.SetActive(true);
        // if ( this.missionEnd.gameObject )

        AnimationEntity.OnAnimationFinishDelegate showUnlocked = delegate(AnimationEntity animationEntity)
        {
            OnDisappear(animationEntity);
            /*
            GameCharacterInformation info = UserInfo.instance.GetNextMissionCharacter();
            if ( info != null )
            {
                UnlockMenuEntity.instance.ShowMissionUnlock(info);
            }
            else
            {
                OnDisappear(animationEntity);
            }*/
        };

        this.missionEnd.Play("Mission Complete",showUnlocked);
        this.missionEnd.RegisterEvent("Mission Complete", half);

        MissionController.instance.GetMissionDescriptionFormat(true, this.missionEndLabel);
        MissionController.instance.GetMissionCompletedNumberFormat(this.missionEndNumber);
        MissionController.instance.GetRewardFormat(this.missionEndReward);
    }

    void Awake() 
    {
        base.Awake();
        _instance = this;

        this.missionStart       = this.GetComponent<AnimationEntity>("Mission Start");
        this.missionStartDesc   = this.GetComponent<UILabel>("Mission Start","Label Mission");
        this.missionStartNumber = this.GetComponent<UILabel>("Mission Start","Label Mission Number");
        this.missionStartGoal   = this.GetComponent<UILabel>("Mission Start","Label Mission Goal");
        
        this.missionCount       = this.GetComponent<AnimationEntity>("Mission Progress");

        this.missionEnd         = this.GetComponent<AnimationEntity>("Mission End");
        this.missionEndLabel    = this.GetComponent<UILabel>("Mission End", "Label Mission");
        this.missionEndNumber   = this.GetComponent<UILabel>("Mission End", "Label Mission Number");
        this.missionEndReward   = this.GetComponent<UILabel>("Mission End", "Label Reward");
        this.newMissionReward   = this.GetComponent<UILabel>("Mission End", "Label Reward New");
        this.missionCountLabel      = this.GetComponent<UILabel>("Label Mission Progress");

        this.missionStart.gameObject.SetActive(false);
        this.missionCount.gameObject.SetActive(false);    
    } 

    public void UpdateNotification()
    {
        if ( UserInfo.instance.gameplayTutorialInitiated == false )
        {
            this.noticeQueue.Enqueue(MissionController.instance.GetMissionProgressFormat());
            //if ( this.missionCount.IsPlaying() == false )
            this.PlayNextNotification();
        }
    }

    public void Reset()
    {        
        this.missionCountLabel.text          = currentNotification;
        this.currentNotification = null;
        this.missionCount.gameObject.SetActive(false);    
    }

    public void PlayNextNotification()
    {
    	//this.currentNotification.gameObject.SetActive(false);

    	if(this.noticeQueue.Count > 0)
    	{
            //MyLogger.Red("PlayNextNotification", "PlayNextNotification");
    		// this.currentNotification      = this.noticeQueue.Dequeue();
            this.missionCountLabel.text   = this.noticeQueue.Dequeue();
            this.missionCountLabel.bitmapFont  = UserInfo.instance.GetLocalizationFont();
            this.missionCount.gameObject.SetActive(true);

            AnimationEntity.OnAnimationFinishDelegate OnShowAnimationFinished = delegate(AnimationEntity ani)
            {
                if ( this.noticeQueue.Count > 0)
                {
                    this.PlayNextNotification();   
                }
                else
                {
                    this.missionCount.gameObject.SetActive(false);
                }
            };

            this.missionCount.Play("Mission Progress Appear", OnShowAnimationFinished, true );
            
    	}
    	else
    	{
             // this.Reset();   
    	}
    }

    public  void Clear()
    {
        this.noticeQueue.Clear();     
    }

    private static MissionFeedbackController _instance    = null;
    public static MissionFeedbackController instance 
    {         
        get
        {
            return _instance;
        }
    }

}
