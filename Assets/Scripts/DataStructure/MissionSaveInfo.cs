using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using CodeStage.AntiCheat.ObscuredTypes;


public class MissionLocalization
{
    public  string      descriptionEng;
    public  string 		descriptionKor;
    public  string 		descriptionFre;
    public  string 		descriptionPor;
    public  string 		descriptionBra;
    public  string 		descriptionSpa;

    public  string 		summaryEng;
    public  string 		summaryKor;
    public  string 		summaryFre;
    public  string 		summaryPor;
    public  string 		summaryBra;
    public  string 		summarySpa;
}

public class MissionSerializable : ISerializable
{		
	public  int 			index 				{get;set;}
    public  MissionType 	missionType 		{get;set;}

    public  ObscuredInt 	defaultGoal			{get;set;}
    public  ObscuredInt 	increaseGoal		{get;set;}
    public  ObscuredInt 	additionalGoal		{get;set;}
    public  ObscuredInt 	maxGoal 			{get;set;}
    public  ObscuredInt 	defaultReward		{get;set;}
    public  ObscuredInt 	increaseReward		{get;set;}
    public  ObscuredInt 	maxReward 			{get;set;}

    public MissionSerializable()
    {

    }
    private MissionSerializable(SerializationInfo info, StreamingContext context)
    {
        this.index 			= (int)info.GetValue("index", typeof(int) );
        this.missionType 	= (MissionType)info.GetValue("missionType", typeof(MissionType) );

        this.defaultGoal 	= (int)info.GetValue("defaultGoal", typeof(int) );
        this.increaseGoal 	= (int)info.GetValue("increaseGoal", typeof(int) );
        this.additionalGoal = (int)info.GetValue("additionalGoal", typeof(int) );
        this.maxGoal 		= (int)info.GetValue("maxGoal", typeof(int) );
        this.defaultReward 	= (int)info.GetValue("defaultReward", typeof(int) );
        this.increaseReward = (int)info.GetValue("increaseReward", typeof(int) );
        this.maxReward 		= (int)info.GetValue("maxReward", typeof(int) );
        //= (string)info.GetValue("obscuredString", typeof(string));
    }
 
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("index", 					this.index);
        info.AddValue("missionType", 			this.missionType);

        info.AddValue("defaultGoal", 			this.defaultGoal);
        info.AddValue("increaseGoal", 			this.increaseGoal);
        info.AddValue("additionalGoal", 		this.additionalGoal);
        info.AddValue("maxGoal", 				this.maxGoal);
        info.AddValue("defaultReward", 			this.defaultReward);
        info.AddValue("increaseReward", 		this.increaseReward);
        info.AddValue("maxReward", 				this.maxReward);
    }
}

public enum MissionType : int
{
	SCORE_TOTAL = 0,				 
	SCORE_GAME,     		     
	COMBO_GAME,                      
	COUNT_GAMEPLAY,                
	DIAMOND_TOTAL,    
	DIAMOND_GAME,    
	SCORE_WITHOUT_DIAMOND,
	SIZET
};

public class MissionSaveInfo 
{
	public  int 					level			{ get; set; }
	private MissionSerializable 	serialized;

	private int  			missionIndex; 	
    private bool        	stack       ;         	

    public  float GetGoalValue()
    {
		if ( this.serialized == null )
			this.serialized = GameInfo.instance.GetMissionInfo(this.missionIndex);
		
		float add = this.serialized.additionalGoal;
		if ( this.level < 2 )
			add = 0;

		float increaseGoal = this.serialized.increaseGoal + (this.level * add);
		float goal = this.serialized.defaultGoal + (this.level * increaseGoal);
    	if ( this.serialized.maxGoal != 0 && goal > this.serialized.maxGoal )
    	{
    		MyLogger.Red("MissionSerializable-GetGoalValue", "Exceeded Max Goal");
    		goal = this.serialized.maxGoal;
    	}

    	return goal;
    }

    public  int GetRewardValue()
    {
		if ( this.serialized == null )
			this.serialized = GameInfo.instance.GetMissionInfo(this.missionIndex);

		int reward = this.serialized.defaultReward + (this.level * this.serialized.increaseReward);
    	if ( reward > this.serialized.maxReward )
    	{
    		MyLogger.Red("MissionSerializable-GetGoalValue", "Exceeded Max Level");
    		reward = this.serialized.maxReward;
    	}

    	return reward;
    }

    public  string  GetSummary()
    {
		return GameInfo.instance.GetMissionSummary( UserInfo.instance.GetLocale(), this.missionIndex );
    }

	public  string 	GetName()
	{        
		string desc = GameInfo.instance.GetMissionDescription( UserInfo.instance.GetLocale(), this.missionIndex );
		return desc.Replace("\\n", "\n");
	}

	public MissionSaveInfo(){}

	public MissionSaveInfo(int index)
	{		
		this.missionIndex 	= index;
		this.level 			= 0;

		this.Initialize(index);
	}

	public void Initialize(int index)
	{
		this.missionIndex = index;
    	//MyLogger.Red("Initialize-GetMissionInfo", "missionIndex "+this.missionIndex);
        this.serialized = GameInfo.instance.GetMissionInfo(this.missionIndex);

		this.stack 			= false;
		switch ( this.serialized.missionType )
		{
			case MissionType.SCORE_TOTAL: //case MissionType.COMBO_TOTAL:
			case MissionType.DIAMOND_TOTAL:
			case MissionType.COUNT_GAMEPLAY:
				this.stack = true;
			break;
		}
	}

	public void OnCompleted()
	{
		if ( this.serialized == null )
        this.serialized = GameInfo.instance.GetMissionInfo(missionIndex);

		this.level ++;
	}

	public bool IsStack()
	{
		return this.stack;
	}

	public bool IsType(MissionType type)
	{
		if ( this.serialized == null )
        this.serialized = GameInfo.instance.GetMissionInfo(missionIndex);
		return this.serialized.missionType == type;
	}
}

public class MissionController
{
	public  int  					currentMissionIndex;
	public  float  					currentMissionCounting;
	public  float 					currentMissionGoal	;
	public  int 					completedCount 	{ get; set; }

	// public  bool 					checkUnlockedCharacter;

	public  MissionSaveInfo[] 		missionList;
	public  bool 					isMissionCompleted = false;
	public static MissionController _instance = null;
	public static MissionController instance 
	{
		get
		{	
			return _instance;
		}
	}

	//[Newtonsoft.Json.JsonConstructor]
	public MissionController(int i)
	{
	}	

	public MissionController()
	{		
		//_instance = this;
	}
	
	public 	void 	ResetMission()
	{
		List<MissionSaveInfo> tmpList = new List<MissionSaveInfo>();
		if ( this.missionList == null )
		{
			// this.missionList = new List<MissionSaveInfo>();
			// this.missionList.Clear();
		}
		
		for ( int i = 0 ; i < GameInfo.instance.GetMissionInfoSize() ; i++ )
		{
			tmpList.Add( new MissionSaveInfo(i) );
		}
		this.missionList = tmpList.ToArray();

		this.currentMissionIndex = -1;
		this.GetNextMission();
		this.completedCount 	= 0;
		this.isMissionCompleted = false; 
	}

	public  void 	OnMissionLoaded()
	{
		// this.missionList = new List<LockStatus>();
		for ( int i = 0 ; i < this.missionList.Length ; i++ )//GameInfo.instance.GetCharacterInfoSize() ; i++ )
		{
			this.missionList[i].Initialize(i);
		}

		if ( this.IsRewardAvailable() )
		{
			this.OnMissionCompleted(false);
		}

		// MyLogger.Red("LoadMissionSaved", "Method not implemented, slap your programmer to resolve the issue");
	}
	
	public  void 	ResetNonStackMissionCount()
	{
		if ( false == this.IsRewardAvailable() 
		  && this.currentMission().IsStack() == false )
		{
            MyLogger.Red("ResetNonStackMissionCount", "reset");
			this.currentMissionCounting = 0;
            MissionFeedbackController.instance.UpdateNotification();
		}
	}

	private MissionSaveInfo currentMission()
	{
		return this.missionList[this.currentMissionIndex];
	}

	public  bool IsRewardAvailable()
	{
		return this.isMissionCompleted ;
	}

	public  void 	UpdateCount(MissionType type, float value)
	{
		if ( UserInfo.instance.gameplayTutorialInitiated )
			return;

		MissionSaveInfo missionInfo = this.currentMission();
		if ( missionInfo.IsType(type) )
		{
            //MyLogger.Red("UpdateCount", "type :"+type);
            this.currentMissionCounting = value;

            if ( this.currentMissionCounting >= this.currentMissionGoal )
            {
            	if ( this.IsRewardAvailable() == false)
	            {
					this.isMissionCompleted = true;
					UserInfo.instance.Save();
            		MissionFeedbackController.instance.UpdateNotification();
	            }
            }
            else if ( this.IsRewardAvailable() == false)
            {
            	MissionFeedbackController.instance.UpdateNotification();
            }
		}
	}

	public  void 	IncrementCount(MissionType type, float value)
	{
		float inc = this.currentMissionCounting + value;
		this.UpdateCount(type, inc);
	}

	public  void 	OnMissionCompleted(bool update = true)
	{
		if ( this.IsRewardAvailable() )
		{
			this.isMissionCompleted = false;

			GamePlayerEntity.instance.currentDiamond += this.GetRewardValue() ;

			if ( update )
			GameUIManager.instance.UpdateUI();
			this.GetNextMission();
			this.completedCount++;
			
			//this.checkUnlockedCharacter = true;
			//UserInfo.instance.UnlockNextMissionCharacter();
		}
		UserInfo.instance.Save();
        // SocialManager.UpdateMissionCount( this.completedCount);
	}

	public  void  	GetNextMission()
	{
		int level = 0;
		if ( this.currentMissionIndex != -1 )
		{
			level = this.currentMission().level;
			this.currentMission().OnCompleted();
		}

		RandomListInteger candidates = new RandomListInteger(new Range2(0,0));
		RandomListInteger rests 	 = new RandomListInteger(new Range2(0,0));
		for ( int idx = 0 ; idx < this.missionList.Length ; idx++ )
		{
			if ( this.missionList[idx].level <= level )
			{
				candidates.Add( idx );
			}
			else if ( this.currentMissionIndex != idx )
			{
				rests.Add( idx );
			}
		}
		string strRest = "";
		for ( int i = 0 ; i < rests.range ; i++ )
		{
			strRest += string.Format("{0},",rests.randomList[i]);
		}

		string strcand = "";
		for ( int i = 0 ; i < candidates.range ; i++ )
		{
			strcand += string.Format("{0},",candidates.randomList[i]);
		}

		MyLogger.Red( "cand :",strcand);
		MyLogger.Red( "rest :",strRest);

		int newIndex = -1;
		if ( candidates.range > 0 )
		{
			newIndex = candidates.GetRandom();
		}
		else
		{
			newIndex = rests.GetRandom();
		}
		MyLogger.Red( "GetNextMission", "Final Choice "+newIndex);

		this.currentMissionIndex = newIndex;
		this.currentMissionCounting = 0;
		this.currentMissionGoal	 	= this.currentMission().GetGoalValue();
	}

	public  void 	SkipToNextRandomMission()
	{
		int newIndex = RandomListInteger.GetRandom( new Range2(0,this.missionList.Length), this.currentMissionIndex);

		this.currentMissionIndex = newIndex;
		this.currentMissionCounting = 0;
		this.currentMissionGoal	 	= this.currentMission().GetGoalValue();
	}

    public  void GetMissionDescriptionFormat(bool ingame, UILabel label)
    {       
    	string	whole 		= "[818e91]";
    	string	highlight	= "[e8687e]";
    	if ( ingame )
    	{
		 	whole 		= "[ffffff]";
		 	highlight  	= "[ffffbb]";
    	}

    	MissionSaveInfo missionInfo = this.currentMission();
    	string description = missionInfo.GetName().Replace("[A]", whole);
    		   description = description.Replace( "[B]", highlight );

        string processed = description.Replace("(x)", ""+this.currentMissionGoal);

        label.text 			= processed;
        //label.trueTypeFont	= UserInfo.instance.GetLocalizationFont();
        label.bitmapFont	= UserInfo.instance.GetLocalizationFont();
    }

    public  string GetMissionGoalFormat()
    {        
        string description = string.Format( "({0:f0}/{1:f0})",
            this.currentMissionCounting, 
            this.currentMissionGoal);

        return description;
    }

    public  void GetCompletionCongratFormat(UILabel label)
    {
    	string desc = UserInfo.GetLocal("10");

        label.text = desc.Replace("(x)", ""+(this.completedCount));
        label.bitmapFont = UserInfo.instance.GetLocalizationFont();
    }

    public  void GetMissionCompletedNumberFormat(UILabel label)
    {
    	int count = this.completedCount +1;

    	string name = UserInfo.GetLocal("3");
    	string description = string.Format( "{0} {1}", name, count );

        label.text 			= description;
        label.bitmapFont	= UserInfo.instance.GetLocalizationFont();
    }

    public  void GetRewardFormat(UILabel label)
    {
    	string description = ""+this.GetRewardValue();
    	label.text 			= description;
    	// label.bitmapFont 	= UserInfo.instance.GetLocalizationFont();
    }

    public  string GetMissionProgressFormat()
    {
    	MissionSaveInfo missionInfo = this.currentMission();
	    
	    string postScript = "[FFFFFF]";
    	if ( this.isMissionCompleted )
    	{
	        postScript = "[68DEB3]";
	    }

	    string 	format = missionInfo.GetSummary().Replace("(x)", this.GetMissionGoalFormat());
        string  description = string.Format( postScript+"{0}"+"[-]", format );

        return description;

    	//string 	format = this.currentMission.summary; // string.Format( "{0}  ({1}/{2})",
    	//		format = format.Replace("(x)", this.GetMissionGoalFormat());
    	//return  format;
    }

    public  int    GetRewardValue()
    {
		if ( GameSystemInfo.instance.IsMissionBoosted() )
		{
    		return 2 * this.currentMission().GetRewardValue();
		}
    	return this.currentMission().GetRewardValue();
    }

};
