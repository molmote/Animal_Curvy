using UnityEngine;
using System.Collections;

public class EatingColliderEntity : MonoBehaviour 
{
	void OnEnable()
	{		
		
	}

	public void OnTriggerEnter(Collider other) 
	{
		GamePlayerEntity.instance.OnEatingColliderEnter(other.gameObject);
	}

}
