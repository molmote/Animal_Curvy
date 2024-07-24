using UnityEngine;
using System.Collections;

public class ExtreamColliderEntity : MonoBehaviour 
{
	void OnEnable()
	{		
		
	}

	public void OnTriggerExit(Collider other) 
	{
		// GamePlayerEntity.instance.OnExtreamColliderExit(other);
	}

	public void OnTriggerStay(Collider other) 
	{
		GamePlayerEntity.instance.OnExtreamColliderStay(other);
	}

}
