using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

	/*------------------------------------ USER INFO ENTITY -------------------------------------*/

public partial class UserInfo
{
	/*public 		List<FollowerCount> 			GetRandomFollowerTable()
	{
		List<FollowerCount> followerCountList = new List<FollowerCount>();
		RandomListInteger 	defaultList = new RandomListInteger(new Range2(0,GameUIManager.instance.defaultFollowerList.Count));

		followerCountList.Add ( new FollowerCount(this.GetCurrentPlayersFollower() ) );
		int index = defaultList.GetRandom();
		followerCountList.Add ( new FollowerCount(GameUIManager.instance.defaultFollowerList[index]) );
		    index = defaultList.GetRandom();
		followerCountList.Add ( new FollowerCount(GameUIManager.instance.defaultFollowerList[index]) );

		return followerCountList;
	}		*/

	private int GetBuddySetValue(PlayerCharacteristic setAbility)
	{	
		return 0;
	}

	public int GetBuddyValueTotal(PlayerCharacteristic ability)
	{
		return 0;//this.GetBuddyValue(ability) + this.GetBuddySetValue(ability);
	}

	public string GetCurrentBuddyName()
	{		
		return "";
	}	

	public bool GetBuddyAbilityTotal(PlayerCharacteristic ability)
	{		
		return true;
	}
	
	public int GetBuddyGotchaIndex()
	{
		return -1;
	}

	public void SelectBuddy(int i)
	{		
	}

	public bool IsBuddyUnlocked(int index)
	{
		return false;
	}

	public bool UnlockBuddy(int index)
	{		
		return true;
	}

	public int  GetJellyRetrieved()
	{
		return 0;//this.jellyRetrieved;
	}

	public void NextTutorial()
	{

		this.Save();
        //Me.instance.currentTutorial ++;
        //DataSaver.instance.SaveUserData();
	}

	public  void 	SetRankingFlag()
	{
		this.initialRankingShown = true;
		this.Save();
	}	

	public void SetInfiniteMode(bool infinite)
	{
	}

	public bool IsInfiniteMode()
	{
		return false;
	}

	public static int  GetInfiniteBonus(int level)
	{
		return 0;//(_SETTING._.infiniteGrowthBonusPerLevel * (level-1));
	}

	public void TriggerBuy(string id)
	{
	}

	public  bool    IsCharacterEndingAvailable()
	{
		return true;
	}

	public  bool 	IsAllCharacterEndingAvailable()
	{
		return true;
	}

	public  bool 	SawAnyCharacterEnding()
	{
		return false;
	}

	public  void    SetCharacterEndingFlag()
	{
	}

	public int GetUpgradeLevel(int i)
	{
		return 0;
	}

	public bool ConsumeJelly(int jelly)
	{
		return true;
	}


	/*static private UserInfo LoadFromJson(string json)
	{		
        MyLogger.LogObject("UserInfoEntity-json",json);
 	
 		UserInfo user;
 		try 
 		{
        	user = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(json); 
    	}
    	catch(Exception ex)
    	{
    		GameSystemInfo.instance.isSyncWithCloud = false;

			user = new UserInfo(1);
			user.SetLocale(Application.systemLanguage.ToString());

			return user;
    	}    	

        //user.currentMissionInfo = new MissionController();
        user.currentMissionInfo.LoadMissionSaved();
        user.ResetBuddyWeight();

        MyLogger.LogObject("UserInfoEntity-info",user);

    	//user.SetLocaleCode( (LanguageType)UserInfo.instance.GetLocaleCode() );
    	user.SetLocale( user.GetLocale() );
    	//DataSaver.instance.LoadSavedGames();
        return user;
	}*/
	/*public void LoadFromCloud(string data)
	{
        string savedLocale = this.GetLocale();
		_instance = LoadFromJson( data );
        // this.ResetUserData ();
        _instance.SetLocale(savedLocale);

        if ( BuddyChangePopup.instance != null )
        BuddyChangePopup.instance.ReInitialize();

        MainUIEntityManager.instance.OnDataReset();
	}*/

	/*public bool CheckIfLocalIsLatest(string localData, string serverData)
	{
		UserInfo local = null;
		UserInfo server = null;
 		try 
 		{
        	local  = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(localData);  
    	}
    	catch(Exception ex)
    	{
    		return false;
    	}    	 
    	//MyLogger.LogObject(local);
 		try 
 		{
        	server = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(serverData);
        }
        catch(Exception ex)
    	{
    		return true;
    	}
    	//MyLogger.LogObject(server);

        if ( local.playCountTotal > server.playCountTotal )
		return true;
		
		return false;
	}*/
}