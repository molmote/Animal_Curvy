using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Waggle;

[CustomEditor(typeof(SlotMachine))]
public class SlotMachineInspector : Editor 
{

	SlotMachine slotMachine;

	public override void OnInspectorGUI ()
	{
		slotMachine = target as SlotMachine;

		if (GUILayout.Button("Spin"))
		{
			slotMachine.Spin();
		}

		DrawDefaultInspector();
	}
}
