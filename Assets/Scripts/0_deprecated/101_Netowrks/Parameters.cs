using UnityEngine;
using System.Collections;

public class PopupParam
{
	public PopupParam()
	{
		this.condition 			= "";
		this.message 			= "";
		this.language 			= "";
		this.currentTime 		= 0.0f;
	    //this.userInfo 			= null;
	    // this.stageInfo 			= null;
	    this.gameInfo 			= null;
	}

	public PopupParam(string condition)
	{
		this.condition 			= condition;
		this.message 			= "";
		this.language 			= "";
		this.currentTime 		= 0.0f;
	   	// this.stageInfo 			= null;
	    //this.userInfo 			= null;
	    this.gameInfo 			= null;
	}

	public PopupParam(string condition, string message)
	{
		this.language 			= "";
		this.condition 			= condition;
		this.message 			= message;	
		this.currentTime 		= 0.0f;
	   	// this.stageInfo 			= null;
	    //this.userInfo 			= null;
	}

	public PopupParam(GameInfo gameInfo, string condition) : this(condition)
	{
		//this.gameInfo 			= new GameInfo(gameInfo);
	}

	
	public  string 			name 			{ get; set; }	
	public  string 		    language 		{ get; set; }
	public  string 			message 		{ get; set; }
	public  string 			condition 		{ get; set; }
	public  float 			currentTime 	{ get; set; }

	public  GameInfo 		gameInfo 		{ get; set; }
	//public 	StageInfo 		stageInfo 		{ get; set; }
	//public  SimpleUserInfo  userInfo 		{ get; set; }

	//! 팝업으로 파라메터로 추가해야 되는게 있으면 여기에 추가 필요
}
