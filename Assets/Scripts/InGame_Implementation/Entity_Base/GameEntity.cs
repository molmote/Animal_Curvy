using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameEntity  : BaseWidgetEntity 
{
	[SerializeField]
	protected   bool 				isAlive;		

	protected   string  			objectType;		// monster? magic? player? - same as tag??
	
	protected  	string     			id;		// which monster? which spell? -> 
	//protected  	string  			name;		// which monster? which spell? -> 
	
	[SerializeField]
	protected   float 				baseHP	;			
	protected   float 				currentHP		{ get; set; }	

	protected   Animator 			animator;

	[SerializeField] protected int 			instanceId;
	public GameEntity(){}

	void Awake()
	{
		Transform [] childArray = this.gameObject.GetComponentsInChildren<Transform>();
		this.childs 		= new List<Transform>(childArray);
		this.animator 		= GetComponent<Animator>();
		this.currentHP 		= this.baseHP; 
		this.isAlive 		= true;	
		this.instanceId 	= this.gameObject.GetInstanceID();
	}

	public void SetAlive(bool alive)
	{
		this.isAlive = alive;;
	}

	public void SetId(string id)
	{
		this.id = id;
	}

	public void SetType(string type)
	{
		this.objectType = objectType;
	}

	public string GetId()
	{
		return id;
	}

	public string GetType()
	{
		return this.objectType;
	}

	public void SetPosition(Vector3 position)
	{
		MyLogger.Log(position);
		
		transform.position = position;
	}

	protected void SetHP(float hp)
	{
		this.currentHP = hp;
	}

	public float GetHP()
	{
		return this.currentHP;
	}

	public void SetBaseHP(float hp)
	{
		this.baseHP = hp;
	}

	public void OnDieAnimationFinish(AnimationEntity animationEntity)
	{
		MyLogger.Log("OnDieAnimationFinish");
		this.DestroySelf();
	}

	public void Dump()
	{
		//MyLogger.Log("----------------------");
		//MyLogger.Log(" hp = : " + currentHP + " / " + baseHP);
		//MyLogger.Log(" money = : " +gold);
	}

	public void SetTrigger( string trigger )
	{
		if ( this.animator && this.gameObject.activeInHierarchy )
		this.animator.SetTrigger(trigger);
	}

	public void SetBoolean( string name, bool trigger )
	{
		if ( this.animator && this.gameObject.activeInHierarchy )
		this.animator.SetBool(name, trigger);
	}

	void Update()
	{

	}

	public bool IsDestroyed()
	{
		return !this.isAlive;
	}

	public bool IsAlive()
	{
		return this.isAlive;
	}


	protected bool IsState(string stateName, string layerName = "Base Layer")
	{
		if ( animator )
		return this.animator.GetCurrentAnimatorStateInfo(0).nameHash == 
					Animator.StringToHash(layerName + "." + stateName);

		return false;
	}

	protected bool IsState(int nameHash, string stateName, string layerName = "Base Layer")
	{
		return nameHash == Animator.StringToHash(layerName + "." + stateName);
	}

	public virtual bool IsAlreadyCollided()
	{
		return false;
	}

	public virtual void MarkAsDestroyed()
	{
		this.SetAlive(false);
	}

	public virtual float GetBaseHP()
	{
		return this.baseHP;
	}

	public virtual void OnTriggerEnter2D(Collider2D collision) 	{}

	public virtual void OnCollideWithOther(GameObject gameObj)  {}

	public virtual bool MarkAsDamaged(float damage)
	{
		//MyLogger.Log("DEBUG", gameObject.tag + "took " + damage +"damage");
		//MyLogger.Log("DEBUG", String.Format("hp = {0} - {1} = {2}/{3}", currentHP, damage, currentHP - damage, baseHP) );

		this.currentHP -= damage;

		if ( this.currentHP <= 0.0f )
		{
			//Collider col = gameObject.GetComponent<Collider>();
			//GameObject.Destroy(col);
			return true;
		}

		return false;
	}

	public virtual void DestroySelf()
	{
		this.gameObject.SetActive(false);	
		this.isAlive = false;
		AbstractObjectPooler.instance.ResetUsedObject(this);
	}

	public virtual void Reset()
	{
		this.SetHP( GetBaseHP() );
		this.isAlive = true;	
		this.gameObject.SetActive(true);		
	}

	public virtual void Clear() {}

	public virtual void Pause()	{}

	public virtual void Resume(){}

	public virtual void SetDamageEffect(string effectPrefabName)
	{
	}

	public void SetChildVisibility(bool visible)
	{
		for ( int i = 0 ; i < childs.Count ; i++ )
		{
			if ( childs[i] != this.transform )
			childs[i].gameObject.SetActiveRecursively(visible);
		}
	}

	private List<Transform> childs ;
}
