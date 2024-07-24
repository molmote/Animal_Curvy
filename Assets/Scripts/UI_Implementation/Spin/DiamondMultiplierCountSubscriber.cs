using UnityEngine;
using System.Collections;

namespace Waggle
{
	public class DiamondMultiplierCountSubscriber : MonoBehaviour 
	{
		public  GameObject 	targetObject;
		public  UILabel 	freeSpinCount;
		public  UILabel 	multiplier;

		private void Awake()
		{
			EventManager.Subscribe<MainUIEnableEvent>(OnMainUIEnable);
		}

		private void Start()
		{
			UserInfo.SpinBonus spinBonus 		= UserInfo.instance.spinBonus;
			UserInfo.SpinBonusBuffStat buffStat = UserInfo.instance.currentBuffStat;
		}

		private void OnDestroy()
		{
			EventManager.UnSubscribe<MainUIEnableEvent>(OnMainUIEnable);
		}

		private void OnEnable()
		{
			UserInfo.SpinBonusBuffStat buffStat = UserInfo.instance.currentBuffStat;
			UpdateFreeSpinCount(buffStat.buffSpinCount);
			multiplier.text = System.Convert.ToString(buffStat.buffMultiplier);
		}

		private void OnMainUIEnable(MainUIEnableEvent evt)
		{
			if (evt.isEnabled)
			{
				UserInfo.SpinBonusBuffStat buffStat = UserInfo.instance.currentBuffStat;
				UpdateFreeSpinCount(buffStat.buffSpinCount);
				multiplier.text = System.Convert.ToString(buffStat.buffMultiplier);
			}
		}

		private void UpdateFreeSpinCount(int count)
		{
			if (count <= 0)
			{
				targetObject.SetActive(false);
			}
			else
			{
				if (!targetObject.activeSelf)
				{
					targetObject.SetActive(true);
				}

				freeSpinCount.text = string.Format("REMAIN: {0}", count);
			}
		}
	}

}