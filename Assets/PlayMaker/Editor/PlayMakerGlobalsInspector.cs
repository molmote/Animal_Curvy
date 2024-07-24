// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HutongGames.PlayMakerEditor;

[CustomEditor(typeof(PlayMakerGlobals))]
class PlayMakerGlobalsInspector : Editor
{
	private PlayMakerGlobals globals;
	private bool refresh;

	private List<FsmVariable> variableList;

#if UNITY_3_4
	private GUIStyle warningBox;
#endif

	void OnEnable()
	{
		//Debug.Log("PlayMakerGlobalsInspector: OnEnable");

		globals = target as PlayMakerGlobals;

		BuildVariableList();
	}

	public override void OnInspectorGUI()
	{
	
		if (refresh)
		{
			Refresh();
			return;
		}

		if (variableList.Count > 0)
		{
			foreach (var fsmVariable in variableList)
			{
				var tooltip = fsmVariable.Name;

				if (!string.IsNullOrEmpty(fsmVariable.Tooltip))
				{
					tooltip += "\n" + fsmVariable.Tooltip;
				}

				fsmVariable.DoValueGUI(new GUIContent(fsmVariable.Name, tooltip), true);
			}
		}

		if (globals.Events.Count > 0)
		{
			foreach (var eventName in globals.Events)
			{
				GUILayout.Label(eventName);
			}
		}

        GUILayout.Space(5);
	}

	void Refresh()
	{
		refresh = false;
		BuildVariableList();
		Repaint();
	}

	void BuildVariableList()
	{
		variableList = FsmVariable.GetFsmVariableList(globals.Variables, globals);
		variableList.Sort();
	}
}
