using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileEntity : GameEntity
{
	//[HideInInspector]
	public 		GameObject 	anchor;
	public 		GameObject 	right;
	public 		GameObject 	left;
	public 		int 		angle;
	[HideInInspector]
	public  	PatternTemplate pattern;
	[HideInInspector]
	public  	Transform 							patternPosition;
	public 		bool 		isObstacleRight;
	private 	List<GameGemEntity> 				gems;
	private 	Dictionary<string,Transform> 		environment;

	private 	Transform 	obstaclePosition;
	private 	GameEntity  obstacle;

	void Awake()
	{
		this.pattern 		 = this.GetComponent<PatternTemplate>();
		this.patternPosition = this.transform.Find("Pattern");
		this.obstaclePosition= this.transform.Find("Obstacle");
		this.gems 			 = new List<GameGemEntity>();
		this.environment	 = new Dictionary<string,Transform>();
		Transform pool = this.GetComponent<Transform>("Environment");
		foreach ( Transform t in pool )
		{
			if ( "Tile Start" != this.gameObject.name )
			{
				t.gameObject.SetActive(false);
			}
			
			this.environment[t.gameObject.name] = t;
		}
	}

	public bool RegisterNewObstacle()
	{
        GameEntity obs    = AbstractObjectPooler.instance.GetIdleObject("Obstacle_01").GetComponent<GameEntity>();  
		this.obstacle 	  = obs;
		//MyLogger.Red("TileEntity", "RegisterNewObstacle"+this.obstacle.GetInstanceID());

        this.obstacle.gameObject.SetActive(true);
        this.obstacle.transform.parent            = this.obstaclePosition;
        this.obstacle.transform.localPosition     = Vector3.zero;
        this.obstacle.transform.localScale        = Vector3.one;

		return this.isObstacleRight;
	}

	public void RegisterNewGem(Transform pos)
	{
		GameGemEntity gem    = AbstractObjectPooler.instance.GetIdleObject("Object Diamond").GetComponent<GameGemEntity>();    
            
		gem.tile 		= this;
		gem.myTransform = gem.transform;
		gem.radius		= gem.GetComponent<SphereCollider>().radius;
        gem.isPassed    = false;
		gem.lockedOn 	= false;

        gem.gameObject.SetActive(true);
        gem.transform.parent            = this.patternPosition;
        gem.transform.localPosition     = pos.localPosition;
        gem.transform.localScale        = Vector3.one;

		this.gems.Add(gem);
	}		

	void Update()
	{
		if ( this.isAlive )
		{
			float playerRadius = GamePlayerEntity.instance.GetRadius();
			Vector3 playerPos = GamePlayerEntity.instance.GetTransform().position;

			if ( playerPos.y < this.anchor.transform.position.y )
			{
				return;
			}

			foreach ( GameGemEntity gem in gems )
			{
				if ( gem.isPassed || gem.lockedOn )
					break;

				Vector3 gemPos = gem.myTransform.position;
				float distance = Vector3.Distance( gemPos, playerPos );
				if ( playerPos.y < gemPos.y && 
					 distance > (gem.radius + playerRadius) * 2f)
				{
					// MyLogger.Red("CheckCombo-ResetCombo", string.Format("{0}>{1}",gemPos, playerPos) );
					GamePlayerEntity.instance.ResetCombo();
					gem.isPassed = true;
				}
				//gem.DestroySelf();
			}
		}
	}

	public void DestroyGem(GameGemEntity gem)
	{
		gems.Remove(gem);
		gem.DestroySelf();
	}

	public override void DestroySelf()
	{
		base.DestroySelf();
		if ( "Tile Start" == this.gameObject.name )
		{
			return;
		}

		foreach ( GameGemEntity gem in gems )
		{
			gem.DestroySelf();
		}

		gems.Clear();
		if ( this.obstacle != null )
		{
			//MyLogger.Red("TileEntity", "Destroy obstacle"+this.obstacle.GetInstanceID());
			this.obstacle.DestroySelf();
			// this.obstacle.gameObject.SetActive(false);
			this.obstacle = null;
		}

		foreach(KeyValuePair<string, Transform> entry in this.environment)
		{
			entry.Value.gameObject.SetActive(false);
		}
	}

	public void Initialize (string name)//, Color color)
	{
		this.gameObject.SetActive(true);
		this.environment[name].gameObject.SetActive(true);
	}

	public void ClearObjects()
	{
		GameEntity obs = this.GetComponent<GameEntity>("Obstacle", "Obstacle_01");
		if ( obs != null )
		{
			MyLogger.Red("TileEntity", "ClearObjects"+obs.GetInstanceID());
			obs.DestroySelf();
		}
	}
}