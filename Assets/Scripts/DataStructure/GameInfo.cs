using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Runtime.Serialization;
using CodeStage.AntiCheat.ObscuredTypes;

using GameDataEditor;

public partial class GameInfo
{
	private static GameInfo _instance = null;// new GameInfo();
	public static GameInfo instance 
	{
		get
		{
			if(_instance == null)
			{
			 	_instance = new GameInfo();
			 	_instance.LoadStageParameters();
			}
			return _instance;
		}
	}

	private bool 								isGamePaused  	 { get; set; }

	private List<MissionSerializable>			missionInfoList;
	private List<GameCharacterInformation> 		characterInformationList;
	private List<BuffSerializable> 				buffInfoList;
	
	private List<MissionLocalization>			missionLocalization;
	private List<CharacterLocalization> 		characterLocalization;

	private RandomListInteger 					listRandomBuff;
	private RandomListInteger 					listRandomMegaBuff;

	public int GetMissionInfoSize()
	{
		return this.missionInfoList.Count;
	}

	public int GetCharacterInfoSize()
	{
		return this.characterInformationList.Count;
	}

	public int GetBuffInfoSize()
	{
		if ( this.buffInfoList == null )
			this.LoadBuffInformation();
		return this.buffInfoList.Count;
	}

	public void Reset()
	{	
		this.InitEnum();
	}

	private void LoadCharacterInformation()
	{		
		if ( this.characterInformationList != null )
			return;

		string jsonChar 			= FileIOController.instance.LoadText(Defines.DATABASE_PATH+"characterInfo");
 		// MyLogger.LogObject("char string : ", jsonChar);
		
		this.characterInformationList 	= Newtonsoft.Json.JsonConvert.
				DeserializeObject<List<GameCharacterInformation>>(jsonChar);
		//this.characterInformationList[i].phaseColor 	= colorDict[this.characterInformationList[i].color];
		//this.characterInformationList[i].defaultColor 	= colorDict["default"];
		
		
 		// MyLogger.LogObject("char info : ", this.characterInformationList);
 		// MyLogger.LogObject("color info : ", colorDict);
	}

	public class ColorSerialized
	{
		public 		string name;
		public 		byte   r;
		public 		byte   g;
		public 		byte   b;
		public 		byte   a;

		public Color GetColor()
		{
			return new Color32( r,g,b, a );
		}
	}

	private void LoadBuffInformation()
	{
		if ( this.buffInfoList != null )
			return;

		string jsonBuff 			= FileIOController.instance.LoadText(Defines.DATABASE_PATH+"buffInfo");
 		// MyLogger.LogObject("buff string : ", jsonBuff);
		
		this.buffInfoList 	= Newtonsoft.Json.JsonConvert.
				DeserializeObject<List<BuffSerializable>>(jsonBuff);

		this.listRandomBuff 		= new RandomListInteger(new Range2(0,0));
		this.listRandomMegaBuff 	= new RandomListInteger(new Range2(0,0));

		for ( int i = 0 ; i < this.buffInfoList.Count ; i++ )
		{
			for ( int j = 0 ; j < this.buffInfoList[i].normalChance ; j++ )
			{	
				listRandomBuff.Add(i);
			}
			for ( int k = 0 ; k < this.buffInfoList[i].megaChance ; k++ )
			{
				listRandomMegaBuff.Add(i);
			}
		}
		
 		MyLogger.LogObject("buff info : ", this.listRandomBuff);
 		MyLogger.LogObject("mega buff info : ", this.listRandomMegaBuff);
	}

	public  int  GetRandomBuff(bool isMegaBuff)
	{
		int index = -1;
		if ( isMegaBuff )
		{
			index = this.listRandomMegaBuff.GetRandomSafe();
		}
		else
		{
			index = this.listRandomBuff.GetRandomSafe();
		}
		
		return index;
	}

	public  void LoadStageParameters()
	{	
		//GDESchemaData
		//GDEDataManager.Init("gde_data_enc", true);
 		this.LoadMissionInfo();
 		this.LoadMissionLocalization();

 		this.LoadCharacterInformation();
 		this.LoadCharacterLocalization();
 		this.LoadBuffInformation();
	}

	public void UpdatePlayerStatus(GamePlayerEntity player)
	{
		// this.currentStage.UpdatePlayerStatus(player);
	}
	
	public void UpdatePlayerStatus()
	{
		this.UpdatePlayerStatus(GamePlayerEntity.instance);
	}

	public bool IsGamePaused()
	{
		return this.isGamePaused;
	}

	public void Pause()
	{
        this.isGamePaused = true;
		AbstractObjectPooler.instance.Pause();
	}

	public void Resume()
	{
		this.isGamePaused = false;
		AbstractObjectPooler.instance.Resume();
	}

	public BuffSerializable 	GetBuffInfo(int index)
	{
		return this.buffInfoList[index];
	}

	public MissionSerializable GetMissionInfo(int index)
	{
		return this.missionInfoList[index];
	}

	public void LoadMissionInfo()
	{		
		if ( this.missionInfoList != null )
			return;

		string jsonMissionInfo 		= FileIOController.instance.LoadText(Defines.DATABASE_PATH+"missionInfo");
		
		this.missionInfoList = Newtonsoft.Json.JsonConvert.
				DeserializeObject<List<MissionSerializable>>(jsonMissionInfo);
					
		MyLogger.LogObject("mission info : ", this.missionInfoList);
	}

    public  string  GetMissionSummary(string lang, int index)
    {
    	if ( lang == "English" )
    	{
    		return this.missionLocalization[index].summaryEng;
    	}
    	else if ( lang == "Korean" )
    	{
    		return this.missionLocalization[index].summaryKor;
    	}
    	else if ( lang == "French" )
    	{
    		return this.missionLocalization[index].summaryFre;
    	}
    	else if ( lang == "Portuguese" )
    	{
    		return this.missionLocalization[index].summaryPor;
    	}
    	else if ( lang == "Spanish" )
    	{
    		return this.missionLocalization[index].summarySpa;
    	}
    	else if ( lang == "Brazillian" ) 
    	{
    		return this.missionLocalization[index].summaryBra;
    	}
		return "null";
    }

    public  string  GetMissionDescription(string lang, int index)
    {
    	if ( lang == "English" )
    	{
    		return this.missionLocalization[index].descriptionEng;
    	}
    	else if ( lang == "Korean" )
    	{
    		return this.missionLocalization[index].descriptionKor;
    	}
    	else if ( lang == "French" )
    	{
    		return this.missionLocalization[index].descriptionFre;
    	}
    	else if ( lang == "Portuguese" )
    	{
    		return this.missionLocalization[index].descriptionPor;
    	}
    	else if ( lang == "Spanish" )
    	{
    		return this.missionLocalization[index].descriptionSpa;
    	}
    	else if ( lang == "Brazillian" ) 
    	{
    		return this.missionLocalization[index].descriptionBra;
    	}
		return "null";
    }

	/*public  string 	GetName(string lang)
	{        
		return this.serialized.description.Replace("\\n", "\n");
	}*/

public enum LanguageType : int 
{
	Brazillian,
	English,
	French,
	Korean,
	Portuguese,
	Spanish,
	SIZE
};

	public 	string  GetMissionSummary(int index, string lang)
	{
		return "";
	}

	public void LoadCharacterLocalization()
	{		
		if ( this.characterLocalization != null )
			return;

		string jsonCharName 		= FileIOController.instance.LoadText(Defines.DATABASE_PATH + "characterNameLocalize");

		this.characterLocalization = 
			 Newtonsoft.Json.JsonConvert.DeserializeObject<List<CharacterLocalization>>(jsonCharName);

		MyLogger.LogObject("local info : ", this.characterLocalization);
	}

	public void LoadMissionLocalization()
	{		
		if ( this.missionLocalization != null )
			return;

		string jsonMission 		= FileIOController.instance.LoadText(Defines.DATABASE_PATH + "missionLocalize");

		this.missionLocalization = 
			 Newtonsoft.Json.JsonConvert.DeserializeObject<List<MissionLocalization>>(jsonMission);

		MyLogger.LogObject("local info : ", this.missionLocalization);
	}

	public int GetCharacterTotal()
	{
		return this.characterInformationList.Count;
	}

	public GameCharacterInformation GetCharacterInfo(int i)
	{
		return this.characterInformationList[i];
	}

	public string GetLocalizedCharacterName(int index)
	{
    	string lang = UserInfo.instance.GetLocale();

    	if ( lang == "English" )
    	{
    		return this.characterLocalization[index].nameEng;// .ToUpper();
    	}
    	else if ( lang == "Korean" )
    	{
    		return this.characterLocalization[index].nameKor;
    	}
    	else if ( lang == "French" )
    	{
    		return this.characterLocalization[index].nameFre;
    	}
    	else if ( lang == "Portuguese" )
    	{
    		return this.characterLocalization[index].namePor;
    	}
    	else if ( lang == "Spanish" )
    	{
    		return this.characterLocalization[index].nameSpa;
    	}
    	else if ( lang == "Brazillian" ) 
    	{
    		return this.characterLocalization[index].nameBra;
    	}
		return "null";
	}
}


public enum PURCHASE_TYPE : int
{
	TWITTER,
	FACEBOOK,
	NORMAL,
	COMBO,
	SCORE,
	OBSTACLE
}

public enum PlayerCharacteristic : int
{
	Small,
	Normal,
	Big
}

public enum BUFF_TYPE : int 
{
	DIAMOND,
	COMBO,
	SCORE
}

public class GameCharacterInformation 
{
	public 		int 					index;
	public 		string 					prefabName;
	public  	PURCHASE_TYPE  			purchaseType;	
	public 		int 					unlockCondition;
	public 		PlayerCharacteristic 	size;
	public 		string 					unlockSound;
	public 		string 					dieSound;
	private 	ObscuredInt 			purchaseCostInt;
}

public class CharacterLocalization
{
    public  string 		nameEng;
    public  string 		nameKor;
    public  string 		nameFre;
    public  string 		namePor;
    public  string 		nameBra;
    public  string 		nameSpa;
}

public class BuffSerializable : ISerializable
{
	public 		string 			name;
	public 		ObscuredInt 	normalAmount; 	
	public 		ObscuredInt 	normalChance;		
	public 		ObscuredInt 	megaAmount;	
	public 		ObscuredInt 	megaChance;
	public 		BUFF_TYPE 		type;
	public 		ObscuredFloat 	effect;

	private BuffSerializable(SerializationInfo info, StreamingContext context)
    {
        this.name 			= (string)info.GetValue("name", typeof(string) );
        this.normalAmount 	= (int)info.GetValue("normalAmount", typeof(int) );
        this.normalChance 	= (int)info.GetValue("normalChance", typeof(int) );
        this.megaAmount 	= (int)info.GetValue("megaAmount", typeof(int) );
        this.megaChance 	= (int)info.GetValue("megaChance", typeof(int) );
        this.type 			= (BUFF_TYPE)info.GetValue("type", typeof(BUFF_TYPE) );
        this.effect 		= (float)info.GetValue("effect", typeof(float) );
    }
 
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("name", 				this.name);
        info.AddValue("normalAmount", 		this.normalAmount);
        info.AddValue("normalChance", 		this.normalChance);
        info.AddValue("megaAmount", 		this.megaAmount);
        info.AddValue("megaChance", 		this.megaChance);
        info.AddValue("type", 				this.type);
        info.AddValue("effect", 			this.effect);
    }
}