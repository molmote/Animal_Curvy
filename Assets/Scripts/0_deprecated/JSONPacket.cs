using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class JSONPacket 
{
       
}


//!
//! TBR
public class ItemInfoPacket : JSONPacket
{
}

//!
//! TBR
public class LoginInfoPacket : JSONPacket
{
    //public UserInfo                     _userLoginInfo             { get; set; }
    public LoginInfoPacket(string jsonString) : base()
    {
        this.Deserialize(jsonString);
    }

    private void Deserialize(string jsonString)
    {
        /*Dictionary<string,object> dict        = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString) as Dictionary<string, object>;

        Dictionary<string,object> userInfoDict = dict["player_info"] as Dictionary<string,object>;
        CreateUserInfo(userInfoDict);*/
    }

    private void CreateUserInfo(Dictionary<string,object> dict)
    {
    }

}

//!
//! TBR
public class GameInitPacket : JSONPacket
{
	// public List<GameEntityInfo> 		_entityInfoList 			{ get; set; }
	public Vector2 						_mapSize 					{ get; set; }

	public GameInitPacket(string jsonString) : base()
	{
		this.Deserialize(jsonString);
	}

	private void Deserialize(string jsonString)
	{
       
	}

    private void CreateEntityFromDictionary(Dictionary<string,object> dict)
    {
    }
}
