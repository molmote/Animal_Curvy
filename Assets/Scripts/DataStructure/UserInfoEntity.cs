using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using System.Runtime.Serialization;
using CodeStage.AntiCheat.ObscuredTypes;

public class UserInfoExtended : ISerializable
{
	public 	ObscuredInt 			currentDiamond					{ get; set; }
	public  ObscuredInt 			playCountTotal 					{ get; set; }			
	public  ObscuredInt 			probFreeGift					{ get; set; }	

	public  UserInfoExtended()
	{
		this.currentDiamond = 100;
		this.playCountTotal = 0;
		this.probFreeGift 	= _SETTING._.freeGiftProbs[0];
	}
 
    private UserInfoExtended(SerializationInfo info, StreamingContext context)
    {
        this.currentDiamond = (int)info.GetValue("currentDiamond", typeof(int) );
        this.playCountTotal = (int)info.GetValue("playCountTotal", typeof(int) );
        this.probFreeGift 	= (int)info.GetValue("probFreeGift", typeof(int) );
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("currentDiamond", 			this.currentDiamond);
        info.AddValue("playCountTotal", 			this.playCountTotal);
        info.AddValue("probFreeGift", 				this.probFreeGift);
    }
}

public partial class UserInfo // : ISerializable
{
	private static UserInfo _instance = null;//UserInfo.Load();
	public static UserInfo instance 
	{
		get
		{
			if (_instance == null)
			{
				_instance = UserInfo.Load();
    			_instance.OnLoadSuccess();
    		}
			return _instance;
		}
	}

	// public int GetCurrentDiamond()

	public int GetPlayCountTotal()
	{
		return this.extended.playCountTotal;
	}

	public int GetFreeGiftProbablity()
	{
		return this.extended.probFreeGift;
	}

	public  UserInfoExtended 		extended 						{ get; set; }
	// public 	int 					diamondRetrieved				{ get; set; }
	public 	float 					bestScore						{ get; set; }
	public 	int 					bestDiamond						{ get; set; }
	public  int 					obstacleCount 					{ get; set; }

	public  MissionController		currentMissionInfo	 			{ get; set; }
	public  GameSystemInfo 			currentSystemInfo				{ get; set; }

	public  List<LockStatus> 		characterStatusList 			{ get; set; }
	public  int 					currentCharacterIndex 			{ get; set; }
	public  bool 					isCharacterUnlockedRecently		{ get; set; }

	public  bool 					isReviewed 						{ get; set; }
	private int 					playCountThisSession; 			
	private bool 					isReviewPopupOpened;

	public  bool 					gameplayTutorialInitiated 		{ get; set; }
	public  bool 					initialRankingShown				{ get; set; }

	public  string  				locale 							{ get; set; }
	public  DateTime 				timeLastReset 					{ get; set; }
	private UIFont 					defaultFont;	 				
	private UIFont 					localizeFont;	

	[System.Serializable]
	public class SpinBonus
	{
		public bool  spinBonusActivate;

		public int   bonusSpinCount;
		public int   spinBonusTimerCount;
		public int[] spinBonusTimers 	= new int[]{3,10,60,180,360}; // minutes

		public long lastEarnedBonusSeconds;


		public long GetRemainSeconds()
		{
			long bonusEarnSeconds 		 = spinBonusTimers[spinBonusTimerCount] * 60;
			long spendTimeSinceLastBonus = Utility.GetCurrentTimeSeconds() - lastEarnedBonusSeconds;
			long remainSeconds 			 = bonusEarnSeconds - spendTimeSinceLastBonus;

			Debug.Log(spinBonusTimerCount + " " + bonusEarnSeconds + " " + spendTimeSinceLastBonus + " " + remainSeconds);

			return remainSeconds < 0 ? 0 : remainSeconds;
		}

		public bool CheckSpinTime()
		{
			long bonusEarnSeconds 		 = spinBonusTimers[spinBonusTimerCount] * 60;
			long spendTimeSinceLastBonus = Utility.GetCurrentTimeSeconds() - lastEarnedBonusSeconds;
			long remainSeconds 			 = bonusEarnSeconds - spendTimeSinceLastBonus;

			if (remainSeconds <= 0)
			{
				spinBonusActivate = true;
				return true;
			}
			return false;
		}

		public void UpdateBonusTime()
		{			
			lastEarnedBonusSeconds 	= Utility.GetCurrentTimeSeconds();
			spinBonusTimerCount  	= (int)Mathf.Min(++spinBonusTimerCount, 5);
			
			++bonusSpinCount;
		}

		public SpinBonus()
		{
			spinBonusActivate 		= false;
			bonusSpinCount 	  		= 0;
			spinBonusTimerCount 	= 0;
			lastEarnedBonusSeconds 	= Utility.GetCurrentTimeSeconds();

			spinBonusTimers 		= new int[]{0,3,10,60,180,360}; // minutes
		}
	}
	public SpinBonus 				spinBonus;

	[System.Serializable]
	public class SpinBonusBuffStat
	{
		public int 	buffSpinCount;
		public int 	buffMultiplier;

		public SpinBonusBuffStat()
		{
			buffSpinCount  		= 0;
			buffMultiplier 		= 1;
		}

		public int GetMultiplier()
		{
			if 		(buffMultiplier == 1)
			{
				return 2;
			}
			else if (buffMultiplier == 2)
			{
				return 4;
			}
			else if (buffMultiplier == 3)
			{
				return 10;
			}
			else
			{
				return 1;
			}
		}
	};
	public SpinBonusBuffStat currentBuffStat;
	public SpinBonusBuffStat lastEarnedBuffStat;
	public void ApplyBuffStat()
	{
		this.currentBuffStat.buffSpinCount  = this.lastEarnedBuffStat.buffSpinCount;
		this.currentBuffStat.buffMultiplier = this.lastEarnedBuffStat.buffMultiplier;
	}

	public void MakeLastBuffStatEmpty()
	{
		this.lastEarnedBuffStat.buffMultiplier = 0;
		this.lastEarnedBuffStat.buffSpinCount  = 0;
	}

	public void CheckAndUpdateBuffBonus()
	{
		int buffSpinCount = this.currentBuffStat.buffSpinCount;
		this.currentBuffStat.buffSpinCount = Mathf.Max(--buffSpinCount, -1);
		
		if (this.currentBuffStat.buffSpinCount < 0)
		{
			this.currentBuffStat.buffMultiplier = 1;
		}
	}

	public void CheckTutorialTreshold()
	{
		if ( this.bestScore < 30 )
		this.gameplayTutorialInitiated = true;
	} 			

	public int GenerateFreeGift()
	{
		int dia  = 0;
		int prob = this.extended.probFreeGift;
		if ( this.playCountThisSession >= prob )
        {
			dia = _SETTING._.GetFreeGiftDiamond();//UnityEngine.Random.Range(5,20) * 10;
        }

		return dia;
	}

	public void OnReceivedFreeGift()
	{
		int prob = this.extended.probFreeGift;

		if ( prob == _SETTING._.freeGiftProbs[0] )
		{
			prob += _SETTING._.freeGiftProbs[1];
		}
		else //if ( prob == prob2 )
		{
			prob += _SETTING._.freeGiftProbs[2];
		}
		this.extended.probFreeGift = prob;
	}

	public void ResetFreeGiftProbability()
	{
		TimeSpan remain = this.timeLastReset - UnbiasedTime.Instance.Now();
		if ( remain.TotalHours < 0 )
		{
			this.extended.probFreeGift = _SETTING._.freeGiftProbs[0];
			this.Save();
			this.timeLastReset = UnbiasedTime.Instance.Now().AddHours(12);
		}
	}

	public void SetReviewed()
	{
        
		this.isReviewed = true;
		this.Save();
	}

	public  bool 	IsReviewPopupNeeded()
	{
		if ( false == this.isReviewed && false == this.isReviewPopupOpened &&
			 this.playCountThisSession > 10 && MyAdManager.instance.IsPopupAvailable() == false )
		{
			this.isReviewPopupOpened = true;
			return true;
		}
		return false;
	}

    public 	UIFont 	GetLocalizationFont()
    {
    	if ( this.locale == "Korean" )
    		return this.localizeFont;
    	else
    		return this.defaultFont;
    }

    public  UIFont  GetDefaultFont()
    {
    	return this.defaultFont;
    }

    public void SetRemainText ( UILabel label, int remain )
    {
        label.text          = string.Format("{0}: {1}", GetLocal("9"), remain);
        label.bitmapFont    = UserInfo.instance.GetLocalizationFont();
    }

    public void SetBestText( UILabel label, int best )
    {
        label.text        = string.Format("{0} {1}",  GetLocal("4"), best );
        label.bitmapFont  = UserInfo.instance.GetLocalizationFont();
    }

	public  string 	GetLocale()
	{
		return this.locale;
	}

	public  void SetLocale(string _locale)
	{
		if ( this.defaultFont == null )
		{
			this.defaultFont 	= (Resources.Load("8_Font/Circula 70(UIFont)") as GameObject).GetComponent<UIFont>();
			this.localizeFont 	= (Resources.Load("8_Font/Korean Font(UIFont)") as GameObject).GetComponent<UIFont>();
		}
		this.locale = "English";
		for ( int i = 0 ; i < (int)GameInfo.LanguageType.SIZE ; i++ )
		{
			if ( _locale == ((GameInfo.LanguageType)i).ToString() )
				this.locale = _locale;
		}
		Localization.language = this.locale;
		MyLogger.Red("SetLocale", this.locale );
	}

	public static string GetLocal(string key)
	{
		string local = Localization.Get(key);
		return local.Replace("\\n", "\n");
	}

	private bool isAnyMissionCompleted;

	public class BuffStatus
	{
		public BuffSerializable serialized;
		public ObscuredInt 		currentCount;
		public int 				totalCount;

		public  BuffStatus(){}

	    private BuffStatus(SerializationInfo info, StreamingContext context)
	    {
	        this.currentCount = (int)info.GetValue("currentCount", typeof(int) );
	    }
	 
	    public void GetObjectData(SerializationInfo info, StreamingContext context)
	    {
	        info.AddValue("currentCount", 			this.currentCount);
	    }
	}

	public UserInfo(int i)
	{
		MyLogger.Blue("UserInfo file not found", "InitializeSaveFile");
		this.InitializeSaveFile();
		this.Save();
	}

	public UserInfo(){}

	public void ResetUserData()
	{
		this.InitializeSaveFile();
		this.Save();
	}

	private void InitializeSaveFile()
	{
		this.extended 			= new UserInfoExtended();

		this.bestScore 			= 0f;
		this.bestDiamond 		= 0;
		this.isReviewed 		= false;
		this.playCountThisSession = 0;
		this.timeLastReset = UnbiasedTime.Instance.Now().AddHours(12);
		
		this.gameplayTutorialInitiated 		= true;
		this.initialRankingShown  			= false;

		this.characterStatusList = new List<LockStatus>();
        //int real = GameInfo.instance.GetCharacterInfoSize();
        int size = 65;//

        for ( int i = 0 ; i < size ; i++ )
		{
			this.characterStatusList.Add(LockStatus.LOCKED);
		}
		this.characterStatusList[2] 	= LockStatus.UNLOCKED;
		this.currentCharacterIndex 		= 2; // aka black cat
		this.isCharacterUnlockedRecently = false;
	}

	public string GetCurrentPlayerName()
	{
		return this.GetCharacterInfo().prefabName;
	}

	public static string Serialize( object obj )
	{
        if ( obj == null )
            return "";

        byte []byteArray = null;
        string jsonString 	= "";
        try
        {
            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        catch(System.Exception e)
        {
            //MyLogger.GA_CreateHitTracking("DataSaver-Save", e.ToString(), false);
            MyLogger.LogError("DEBUG", "DataSaver Save ERROR: ");
        }

        return jsonString;
	}

	public void Save(bool isQuit = false)
	{		
        //MyLogger.LogObject("Save-info",this);
	    string jsonString = UserInfo.Serialize(this);
		ES2.Save 	   (jsonString, "save_encrypt.txt");
        #if UNITY_EDITOR 
	    	DataSaver.Save (jsonString, "save.txt" );
	    #endif
	}	

    static public  UserInfo Load()
    {        
        UserInfo user     		= null;
        // #if UNITY_EDITOR 
        // 	user = DataSaver.Load<UserInfo>("save.txt");
       // #endif
        
        if ( user == null )
        {
        	if ( ES2.Exists("save_encrypt.txt") )
        	{
                string ezsave ="";
                try
                {
                    ezsave = ES2.Load<string>("save_encrypt.txt");
                }
                catch
                {
                }
	        	
                if (ezsave != "")
                {
                    MyLogger.LogObject("UserInfoEntity-json", ezsave);
                    user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(ezsave);
                }
                else
                {
                    user = new UserInfo(1);
                    user.SetLocale(Application.systemLanguage.ToString());
                }
            }
        	else
        	{
				user = new UserInfo(1);
				user.SetLocale(Application.systemLanguage.ToString());
        	}
        }

        if (user.spinBonus == null)
        {
        	user.spinBonus 			= new SpinBonus();
        }

        if (user.currentBuffStat == null)
        {
        	user.currentBuffStat  	= new SpinBonusBuffStat();
        }

        if (user.lastEarnedBuffStat == null)
        {
        	user.lastEarnedBuffStat = new SpinBonusBuffStat();
        }

    	//user.OnLoadSuccess();
        return user;
    }

    private void OnLoadSuccess()
    {
    	this.ResetGiftCharacter();
		this.SetLocale(Application.systemLanguage.ToString());

		if ( this.currentMissionInfo == null )
		{
	        this.currentMissionInfo = new MissionController();
	        this.currentMissionInfo.ResetMission();	
		}
        if ( this.currentSystemInfo == null )
        {
        	this.currentSystemInfo  = new GameSystemInfo();
        }
    	MissionController._instance = this.currentMissionInfo;
     	//_instance = this;
     	this.currentMissionInfo.OnMissionLoaded();
     	// this.currentSystemInfo.ApplyBoost();
     	this.currentSystemInfo.SetSoundEnable( this.currentSystemInfo.isSoundEnabled );

    	string name = GameInfo.instance.GetCharacterInfo(this.currentCharacterIndex).prefabName;
    	GamePlayerEntity.ChangeInstance(name);
    	this.ResetFreeGiftProbability();
    }

    public void ResetGiftCharacter()
    {
    }

	public int GetCurrentDiamond()
	{
		return this.extended.currentDiamond;
	}

	public bool Consume(int cost)
	{
		if ( cost < 0 )
			return false;

		if ( cost > this.extended.currentDiamond )
			return false;

		this.extended.currentDiamond -= cost;
		this.Save();
		return true;
	}

	public int BuyDiamond(int diamond)
	{		
		if ( diamond > 0 )
		{
	    	MyLogger.Blue("DEBUG", "BuyJelly click");

			this.extended.currentDiamond += diamond;
			this.Save();
		}
		int currentDiamond = this.extended.currentDiamond;

		//SocialManager.ReportAchievements( currentDiamond  >= 5000,	"diamond5000");
		//SocialManager.ReportAchievements( currentDiamond  >= 10000,	"diamond10000");

		return currentDiamond;
	}

	public int ChangeBestScore(float score)
	{
		if ( score > this.bestScore )
		{
			this.bestScore = score;
			this.Save();
		}
		return (int)this.bestScore;
	}

	public void ChangeBestDiamond(int dia)
	{
		if ( dia > this.bestDiamond )
		{
			this.bestDiamond = dia;
			this.Save();
		}
	}

	public  float 	GetBestScore()
	{
		return this.bestScore;
	}

	public  int 	GetBestDiamond()
	{
		return this.bestDiamond;
	}

	public  int  GetUnlockedCharacterCount()
	{
		int count = 0;
		for ( int i = 0 ; i < this.characterStatusList.Count ; i++ )
		{
			if ( this.IsCharacterLocked(i) == false )
			count++;
		}
		return count;
	}

	public  bool IsCharacterLocked(int index)
	{
		if ( index >= 0 && index < this.characterStatusList.Count )
		{
			return ( this.characterStatusList[index] == LockStatus.LOCKED );
		}

		// MyLogger.Assert("Character Index Out of Range");
		return false;
	}

	public  bool IsCharacterUnlockedRecently( int index )
	{
		return this.characterStatusList[index] == LockStatus.NEW ;
	}

	public  bool IsAnyCharacterUnlockedRecently()
	{
		return this.isCharacterUnlockedRecently;
	}

	public  void    SetNewlyUnlockedCharacter( int index,  bool newlyAvailable )
	{
		if ( newlyAvailable )
		{
			if ( characterStatusList[index] == LockStatus.LOCKED )
			{
				this.characterStatusList[index] = LockStatus.NEW;
				this.isCharacterUnlockedRecently = true;
				//int unlockedCount = this.GetUnlockedCharacterCount();
			}
		}
		else
		{
			if ( characterStatusList[index] == LockStatus.NEW )
			{
				this.characterStatusList[index] = LockStatus.UNLOCKED;
				this.isCharacterUnlockedRecently = false;
			}
		}	
		this.Save();
	}

	public  List<int>  GetNewlyUnlockedSpecialCharacter()
	{
		List<int> listUnlocked = new List<int>();
		for ( int index = 60 ; index < this.characterStatusList.Count ; index++ )
		{
			if ( this.IsCharacterUnlockedRecently( index ))
			{
				this.SetNewlyUnlockedCharacter(index, false);
				listUnlocked.Add(index);
			}
		}
		return listUnlocked;
	}

	public  bool MakeSpecialCharacterAvailable()
	{
		for ( int index = 60 ; index < this.characterStatusList.Count ; index++ )
		{
			if ( this.IsCharacterLocked( index ))
			{
				GameCharacterInformation info = GameInfo.instance.GetCharacterInfo(index);
				if ( PURCHASE_TYPE.COMBO == info.purchaseType &&
					 GamePlayerEntity.instance.maxCombo >= info.unlockCondition  )
				{
					this.UnlockCharacter(index);
				}
				else if ( PURCHASE_TYPE.SCORE == info.purchaseType &&
					 GamePlayerEntity.instance.currentScore >= info.unlockCondition  )
				{
					this.UnlockCharacter(index);
				}
				else if ( PURCHASE_TYPE.OBSTACLE == info.purchaseType && 
					this.obstacleCount >= info.unlockCondition )
				{
					this.UnlockCharacter(index);
				}
			}

		}
		//this will be ignored after first off

		return this.IsAnyCharacterUnlockedRecently();
	}

	public  int 	GetRandomCharacterIndex()
	{
		int index = UnityEngine.Random.Range( 2, 59 );
		return index;
	}

	public  int 	GetAvailableRandomCharacterIndex()
	{
		List<int> availableTmp = new List<int>();
		for ( int i = 2; i < 60 ; i++ )
		{
			if ( this.IsCharacterLocked(i) )
			availableTmp.Add(i);
		}

		if ( availableTmp.Count == 0 )
		{
			return this.GetRandomCharacterIndex();
		}

		RandomListInteger available = new RandomListInteger(availableTmp);

		MyLogger.RedObject("GetAvailableRandomCharacterIndex", availableTmp);

		return available.GetRandom();
	}

	public  GameCharacterInformation    GetNextMissionCharacter()
	{		
		return null;
	}

	private bool 	IsUnlockedRecently( int index )
	{
		return this.characterStatusList[index] == LockStatus.NEW;
	}

	public  int 	GetCurrentCharacterIndex()
	{
		return this.currentCharacterIndex;
	}	

	public bool SelectCharacter(int i)
	{
		if ( i == this.currentCharacterIndex )
		{
			return false;
		}

		if ( i >= 0 && i < this.characterStatusList.Count )
		{
			if ( this.IsCharacterLocked(i) == false )
			{
				this.currentCharacterIndex = i;
				this.Save();

				GameCharacterInformation info = this.GetCharacterInfo();

		    	GamePlayerEntity.ChangeInstance(info.prefabName);
		        MyLogger.Red("Item Chosen","Character-OnSelectClick");

		        GameUIManager.instance.ChangePreviewInstance(info);

				return true;
			}
			else 
			{
				MyLogger.Assert("trying to select locked Character");
				return false;
			}
		}
		else	
		{
			MyLogger.Assert("Character Index Out of Range");
		}
		return false;
	}

	public void LockCharacter(int index)
	{
		if ( index >= 0 && index < this.characterStatusList.Count )
		{
			if (this.IsCharacterLocked(index) )
				MyLogger.Red("UnlockCharacter", "Trying to unlock character already LOCKED");

			this.characterStatusList[index] = LockStatus.LOCKED;
			if ( this.currentCharacterIndex == index )
			{
				this.SelectCharacter(0);
			}
			this.Save();
		}
		else
			MyLogger.Red("UnlockCharacter", "Character Index Out of Range");
	}

	public bool UnlockCharacter(int index)
	{		
		if ( index >= 0 && index < this.characterStatusList.Count )
		{
			if (this.IsCharacterLocked(index) == false)
			{

				MyLogger.Red("UnlockCharacter", string.Format("{0} Trying to unlock character already UNLOCKED", index) );
				return false;
			}
				
			SocialManager.instance.UpdateUnlockAchievements();
			this.SetNewlyUnlockedCharacter(index, true);
			this.Save();
		}
		else
		{
			MyLogger.Red("UnlockCharacter", "Character Index Out of Range");
			return false;
		}
		
		return true;
	}

	public GameCharacterInformation GetCharacterInfo()
	{
		return GameInfo.instance.GetCharacterInfo( this.currentCharacterIndex );
	}

	public void IncrementPlayCount()
	{
		this.extended.playCountTotal ++;
		this.playCountThisSession ++;
		SocialManager.instance.UpdateGamePlayAchievements(this.extended.playCountTotal);
	}

}