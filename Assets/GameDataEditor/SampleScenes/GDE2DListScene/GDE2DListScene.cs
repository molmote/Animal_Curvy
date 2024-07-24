using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GameDataEditor;

public class GDE2DListScene : MonoBehaviour {

	GDETwoDListData data;
	GUIStyle labelStyle;
	float x = 0f;
	float y = 0f;
	const float lineHeight = 20f;

	// Use this for initialization
	void Start () {
		if (!GDEDataManager.Init("2dlist_scene_data"))
			Debug.LogError("Error initializing!");
		else
		{
			if (!GDEDataManager.DataDictionary.TryGetCustom(GDE2DListItemKeys.TwoDList_DevItem, out data))
				Debug.LogError("Something went wrong :(");

			if(data == null)
				Debug.Log("data null!");
		}
	}
	
	void OnGUI() {
		Vector2 size;
		x = 0f;
		y = 0f;

		if (labelStyle == null)
			labelStyle = GUI.skin.label;

		GUIContent content = new GUIContent("2D List Test\nThis scene walks all 2 dimensional lists in the data set, and prints out the values.");
		size = labelStyle.CalcSize(content);
		GUI.Label(new Rect(x, y, size.x, lineHeight), content);
		y += lineHeight;

		// Bool
		DrawBasic("Bool:", data.b);

		// Int
		DrawBasic("Int:", data.i);

		// Float
		DrawBasic("Float:", data.f);

		// String
		DrawBasic("String:", data.s);

		// Vec2
		DrawBasic("Vector2:", data.v2);

		// Vec3
		DrawBasic("Vector3:", data.v3);

		// Vec4
		DrawBasic("Vector4:", data.v4);

		// Color
		DrawBasic("Color:", data.c);

		// Custom
		content.text = "Custom: ";
		GUI.Label(new Rect(x, y, size.x, lineHeight), content);
		y += lineHeight;
		int curListNum = 1;
		content.text = string.Empty;
		foreach(var sublist in data.cus) 
		{
			content.text += "List#"+curListNum+": ";
			foreach(var entry in sublist) 
			{
				content.text += "[";
				if (entry.asd != null)
					entry.asd.ForEach(itr => content.text += itr + ", ");
				content.text += "] |";
			}
			content.text += "\n";
			curListNum++;
		}
		
		size = labelStyle.CalcSize(content);
		GUI.Label(new Rect(x, y, size.x, size.y), content);
		y += size.y;
	}

	void DrawBasic<T>(string typeLabel, T twodlist) where T : IList
	{
		int curListNum = 1;
		Vector2 size;

		GUIContent content = new GUIContent(typeLabel);
		size = labelStyle.CalcSize(content);
		GUI.Label(new Rect(x, y, size.x, lineHeight), content);
		y += lineHeight;
		
		content.text = string.Empty;
		foreach(IList list in twodlist) {
			content.text += "List #"+curListNum+": ";

			foreach(var entry in list) {
				content.text += entry+" | ";
			}

			content.text += "\n";
			curListNum++;
		}
		size = labelStyle.CalcSize(content);
		GUI.Label(new Rect(x, y, size.x, size.y), content);
		y += size.y;
	}
}
