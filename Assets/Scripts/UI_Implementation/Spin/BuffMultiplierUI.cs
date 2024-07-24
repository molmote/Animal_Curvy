using UnityEngine;
using System.Collections;


namespace Waggle
{

public class BuffMultiplierUI : MonoBehaviour 
{
	public  int 		compareCount = 0;

	public  GameObject 	targetObject;
	public  UILabel 	freeSpinCount;
	public  UILabel 	multiplier;

	public  string 		multiplierMessage; // FOR (X) GAMES

	public void OnEnable()
	{
		UserInfo.SpinBonusBuffStat buffStat = UserInfo.instance.currentBuffStat;
		UpdateFreeSpinCount(buffStat.buffSpinCount, buffStat.buffMultiplier);
	}

	public void OnDisable()
	{

	}

	private void UpdateFreeSpinCount(int count, int multiplierCount)
	{
		if (count < compareCount)
		{
			targetObject.SetActive(false);
		}
		else
		{
			if (!targetObject.activeSelf)
			{
				targetObject.SetActive(true);
			}

			multiplier.text = string.Format(multiplierMessage, multiplierCount);
		}
	}
}

}