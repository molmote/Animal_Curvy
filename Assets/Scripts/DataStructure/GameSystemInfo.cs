using UnityEngine;
using System.Collections;

using System;
using System.Runtime.Serialization;
using CodeStage.AntiCheat.ObscuredTypes;

public class GameSystemInfo : ISerializable
{
	private static GameSystemInfo 	_instance = null;
	public static GameSystemInfo 	instance
	{
		get
		{	
			if( _instance == null )
				_instance = UserInfo.instance.currentSystemInfo;
			return _instance;
		}
	}

	//[Newtonsoft.Json.JsonConstructor]
	public GameSystemInfo(int i)
	{
	}	

	public GameSystemInfo()
	{		
		this.isSoundEnabled 	= true;
		this.isMusicEnabled 	= true;

		// this.totalPlayTime  	= 0.0;		

		this.SetSoundEnable(true);
		this.SetMusicEnable(true, true);

		this.boostedAdFree 			= false;
		this.boostedDoubleDiamond 	= false;
		this.boostedMissionReward 	= false;
		this.boostedMagneticForce 	= false;
	}
 
    private GameSystemInfo(SerializationInfo info, StreamingContext context)
    {
        this.isSoundEnabled = (bool)info.GetValue("isSoundEnabled", typeof(bool) );
        this.isMusicEnabled = (bool)info.GetValue("isMusicEnabled", typeof(bool) );

        this.boostedAdFree = (bool)info.GetValue("boostedAdFree", typeof(bool) );
        this.boostedDoubleDiamond = (bool)info.GetValue("boostedDoubleDiamond", typeof(bool) );
        this.boostedMissionReward = (bool)info.GetValue("boostedMissionReward", typeof(bool) );
        this.boostedMagneticForce = (bool)info.GetValue("boostedMagneticForce", typeof(bool) );
        //= (string)info.GetValue("obscuredString", typeof(string));
        
        MainUIEntityManager.instance.SetSoundButton(this.isSoundEnabled);
    }
 
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("isSoundEnabled", 		this.isSoundEnabled);
        info.AddValue("isMusicEnabled", 		this.isMusicEnabled);

        info.AddValue("boostedAdFree", 			this.boostedAdFree);
        info.AddValue("boostedDoubleDiamond", 	this.boostedDoubleDiamond);
        info.AddValue("boostedMissionReward", 	this.boostedMissionReward);
        info.AddValue("boostedMagneticForce", 	this.boostedMagneticForce);
    }

	public  bool 	isSoundEnabled  				{ get; set; }
	public  bool 	isMusicEnabled  				{ get; set; }
	// public  double  totalPlayTime 					{ get; set; }

	public  ObscuredBool 	boostedAdFree 					{ get; set; }
	public  ObscuredBool 	boostedDoubleDiamond 			{ get; set; }
	public  ObscuredBool 	boostedMissionReward 			{ get; set; }
	public  ObscuredBool 	boostedMagneticForce 			{ get; set; }

	public  double 	GetTotalPlayedTime()
	{
		return 0;//this.totalPlayTime;
	}

	public  void UpdateTotalApplicationRuntime()
	{
		// this.totalPlayTime += Time.deltaTime;
	}

	public  void SetSoundEnable(bool isEnable)
	{       
		this.isSoundEnabled = isEnable;

		if(this.isSoundEnabled)
			SoundController.instance.SetVolume(1.0f);
		else 
			SoundController.instance.SetVolume(0.0f);
		// this.Save();
	}

	public  bool ToggleSound()
	{
		this.SetSoundEnable ( !this.isSoundEnabled ); 
		return this.isSoundEnabled;
	}

	public 	void SetMusicEnable(bool isEnable, bool start = false)
	{		
		this.isMusicEnabled 	= isEnable;
		// this.Save();
	}

	public 	bool IsMusicEnabled()
	{
		return this.isMusicEnabled;
	}

	public  bool  	IsSoundEnabled()
	{
		return this.isSoundEnabled;
	}

	public void Save()
	{
	}

	public bool IsAdfreeVersion()
	{
		return true;
	}

	public bool IsDiamondBoosted()
	{
		return this.boostedDoubleDiamond;
	}

	public bool IsMissionBoosted()
	{
		return true;
	}

	public bool IsMagneticForce()
	{
		return this.boostedMagneticForce;
	}
    
    public void SetMissionBoost()
    {
    	this.boostedMissionReward 	= true;
    }
    
    public void SetAdFree()
    {
    	this.boostedAdFree 			= true;
    }

    public void SetDiamondBoost()
    {
    	this.boostedDoubleDiamond  	= true;
    }

    public void SetMagneticForce()
    {
    	this.boostedMagneticForce 	= true;
    }
}
