using UnityEngine;
using System.Collections;

public class DamageColliderEntity : MonoBehaviour 
{
	//private CircleCollider2D 	circle;
	//private float 				defaultRadius;

	void Awake()
	{
		//this.circle 		= this.GetComponent<CircleCollider2D>();
		//this.defaultRadius 	= circle.radius;
	}

	public float GetDefaultRadius()
	{
		return 0;// this.defaultRadius;
	}

	void OnEnable()
	{		
		//if ( GameInfo.instance.IsState(INGAME_STATE.NOTREADY) == false )
		//{
		//}
	}

	public void OnTriggerEnter(Collider other) 
	{
		// MyLogger.Red("DamageColliderEntity", "Collided with Damage collider");
		//GameMonsterEntity monster =  other.transform.parent.parent.GetComponent<GameMonsterEntity>();
		GamePlayerEntity.instance.OnDamageColliderEnter(other);
	}
}
