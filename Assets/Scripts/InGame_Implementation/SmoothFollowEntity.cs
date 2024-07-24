using UnityEngine;
using System.Collections;

public class SmoothFollowEntity : MonoBehaviour {

	//[SerializeField]
	//private Transform target = null;

	public 		Vector3 	playerPreviousPosition;

	public  	float 		xLimitOffset;
	public  	Vector3 	playerPreviousSpeed;
	public   	float		recoveryTimer;
	public 		float 		recoveryTimeSpan = 3f;

	public 		float 		walkingTimer;
	public 		float 		walkingDistanceTotal;
	public 		float 		walkingDistance;
	public  	Vector3 	walkingPrevPosition;

	void Awake()
	{	
		this.playerPreviousPosition = GamePlayerEntity.instance.transform.position;
	}

	public void OnStartGame()
	{
		//MyLogger.Red("SmoothFollowEntity", "OnStartGame");
		Vector3 playerPos 			= GamePlayerEntity.instance.GetTransform().position ;
		this.transform.position 	= new Vector3 ( playerPos.x, playerPos.y - 265f, this.transform.position.z );
		this.playerPreviousPosition = playerPos;
	}

	public void OnMainMenu()
	{
		//MyLogger.Red("SmoothFollowEntity", "OnMainMenu");
		//this.transform.localPosition 	= Vector3.zero;

		Vector3 playerPos 			= GamePlayerEntity.instance.GetTransform().position ;
		this.transform.position 	= new Vector3 ( playerPos.x, playerPos.y, this.transform.position.z );
		this.playerPreviousPosition = playerPos;
	}

	void LateUpdate()
	{
		GamePlayerEntity player = GamePlayerEntity.instance;// this.target.GetComponent<GamePlayerEntity>();
		if ( player != null )//&& player.IsDead() == false )
		{
		//MyLogger.Blue("SmoothFollowEntity", "LateUpdate");
	    	float ySave = this.transform.position.y + (player.transform.position.y - this.playerPreviousPosition.y);
	    	float xSave = this.transform.position.x + (player.transform.position.x - this.playerPreviousPosition.x);
	    	float xDiff = this.transform.position.x - player.transform.position.x;
	    	
	    	this.transform.position 	= new Vector3 (xSave, ySave, this.transform.position.z);
	    	
	    	this.playerPreviousPosition = player.transform.position;
	    	this.playerPreviousSpeed 	= new Vector3( player.horizontalSpeed, -player.verticalSpeed, 0);
	    	//this.playerPreviousSpeed.Normalize();
	    }
	    else
	    {

	    }
	}
}
