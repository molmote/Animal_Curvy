// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using HutongGames.PlayMakerEditor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayMakerGUI))]
class PlayMakerGUIInspector : Editor
{
	private PlayMakerGUI guiComponent;

	void OnEnable()
	{
		guiComponent = (PlayMakerGUI) target;

		CheckForDuplicateComponents();
	}

	public override void OnInspectorGUI()
    {
	}

	void CheckForDuplicateComponents()
	{
	}

}
