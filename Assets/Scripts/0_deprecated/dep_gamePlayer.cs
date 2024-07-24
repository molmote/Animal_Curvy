using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	/*------------------------------------ Private method -------------------------------------*/
public partial class GamePlayerEntity : GameEntity
{
}

	/*------------------------------------ Deprecated method -------------------------------------*/
public partial class GamePlayerEntity : GameEntity
{

	public void ShowObtainMessage(Transform monsterPos, float weight, int jelly )
	{
	}

	private void ProcessAnimator()
	{		
	}

	public float GetCurrentDashSpeed(float timeSpanMultiply)
	{
		return 0;
	}

	private void ShowItemObtainEffect()
	{		
	}

	private 	bool 	isReviveAvailable;
	private 	bool 	endingFlag;

	public void OnDamageColliderEnter(GameEntity other) 
	{
	}

	public float GetDistanceFrom(Vector3 pos)
	{
		return 0;
	}

}

/*public class CharacterLocalization
{
	public 		int 		no;
	public 		string 		name;
	public 		string 		abilityDescription;
	public 		string 		description;

	public		string 		GetName()
	{
		return this.name.Replace("\\n", "\n");
	}

	public 		string 		GetAbilityDescription()
	{
		return this.abilityDescription.Replace("\\n", "\n");
	}

	public 		string 		GetDescription()
	{
		return this.description.Replace("\\n", "\n");
	}
}*/

public class BuddyLocalization
{
	public 		int 		index;
	public 		string 		name;
	public 		string 		ability;
	public 		string 		setAbility;
	public 		string 		description;

	public		string 		GetName()
	{
		return this.name.Replace("\\n", "\n");
	}

	public 		string 		GetAbilityDescription()
	{
		return this.ability.Replace("\\n", "\n");
	}

	public 		string 		GetSetDescription()
	{
		return this.setAbility.Replace("\\n", "\n");
	}

	public 		string 		GetDescription()
	{
		return this.description.Replace("\\n", "\n");
	}
}

public class GameBuddyInformation
{
	public 		int 					index;
	public 		string 					prefabName;
	public 		int 					setIndex;

	public 		PlayerCharacteristic 	abilityEnum;
	public 		int 					abilityPower;

	public 		PlayerCharacteristic 	setAbilityEnum;
	public 		int 					setAbilityPower;
	public 		int 					probability;
}