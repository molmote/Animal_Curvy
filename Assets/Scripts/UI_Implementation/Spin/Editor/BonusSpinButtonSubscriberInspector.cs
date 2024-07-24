using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Waggle;

[CustomEditor(typeof(BonusSpinButtonSubscriber))]
public class BonusSpinButtonSubscriberInspector : Editor 
{

	BonusSpinButtonSubscriber bonusSpinButton;

	public override void OnInspectorGUI ()
	{
		bonusSpinButton = target as BonusSpinButtonSubscriber;

		if (GUILayout.Button("Initialize"))
		{
			UserInfo.instance.spinBonus 		 = new UserInfo.SpinBonus();
			UserInfo.instance.currentBuffStat 	 = new UserInfo.SpinBonusBuffStat();
			UserInfo.instance.lastEarnedBuffStat = new UserInfo.SpinBonusBuffStat();
			UserInfo.instance.Save();
		}

		DrawDefaultInspector();
	}
}
