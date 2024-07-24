using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using GameDataEditor;

public class GDEPreferences : EditorWindow {

    GUIStyle headerStyle = null;

    Color32 createDataColor;
    Color32 defineDataColor;
    Color32 highlightColor;

	string defaultCreateDataColor;
	string defaultDefineDataColor;

    string dataFilePath;

	void SetStyles()
	{
		if (headerStyle == null)
		{
			headerStyle = new GUIStyle(GUI.skin.label);
			headerStyle.fontStyle = FontStyle.Bold;
			headerStyle.fontSize = 16;
		}
	}

	void LoadDefaultColors()
	{
		if (EditorGUIUtility.isProSkin)
		{
			defaultCreateDataColor = GDEConstants.CreateDataColorPro;
			defaultDefineDataColor = GDEConstants.DefineDataColorPro;
		}
		else 
		{
			defaultCreateDataColor = GDEConstants.CreateDataColor;
			defaultDefineDataColor = GDEConstants.DefineDataColor;
		}
	}
        
    void OnGUI()
    {
		SetStyles();

		LoadDefaultColors();

        GUIContent content = new GUIContent("File Locations");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label("File Locations", headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Create Data File", "", GUILayout.Width(100));
        dataFilePath = EditorGUILayout.TextField(dataFilePath);
        if (GUILayout.Button("Browse ...", GUILayout.Width(70)))
        {
            string newDataFilePath = EditorUtility.OpenFilePanel("Open Data File", dataFilePath, string.Empty);
            if (!string.IsNullOrEmpty(newDataFilePath) && !newDataFilePath.Equals(dataFilePath))
                dataFilePath = newDataFilePath;
            GUI.FocusControl("");
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20);

        content.text = "Colors";
        size = headerStyle.CalcSize(content);
        GUILayout.Label("Colors", headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));

        createDataColor = EditorGUILayout.ColorField("Create Data Header", createDataColor);
        defineDataColor = EditorGUILayout.ColorField("Define Data Header", defineDataColor);
        highlightColor = EditorGUILayout.ColorField("Highlight", highlightColor);

        GUILayout.Space(20);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Use Defaults"))
            LoadDefaults();

        if (GUILayout.Button("Apply"))
            SavePreferences();

        EditorGUILayout.EndHorizontal();
    }

    public void LoadPreferences()
    {
		LoadDefaultColors();

        dataFilePath = EditorPrefs.GetString(GDEConstants.DataFileKey, Application.dataPath + GDEConstants.DefaultDataFilePath + GDEConstants.DataFile);
        if (dataFilePath.Equals(Application.dataPath + GDEConstants.DefaultDataFilePath + GDEConstants.DataFile))
            CreateDefaultDataFileDirectory();

		string color;

		color = EditorPrefs.GetString(GDEConstants.CreateDataColorKey, defaultCreateDataColor);
        createDataColor = color.ToColor();

        color = EditorPrefs.GetString(GDEConstants.DefineDataColorKey, defaultDefineDataColor);
        defineDataColor = color.ToColor();
        
        color = EditorPrefs.GetString(GDEConstants.HighlightColorKey, GDEConstants.HighlightColor);
        highlightColor = color.ToColor();
    }

    void LoadDefaults()
    {
        dataFilePath = Application.dataPath + GDEConstants.DefaultDataFilePath + GDEConstants.DataFile;
        CreateDefaultDataFileDirectory();

        createDataColor = defaultCreateDataColor.ToColor();
        defineDataColor = defaultDefineDataColor.ToColor();
        highlightColor = GDEConstants.HighlightColor.ToColor();

        GUI.FocusControl("");

        SavePreferences();
    }

    void SavePreferences()
    {
        EditorPrefs.SetString(GDEConstants.CreateDataColorKey, "#" + createDataColor.ToHexString());
        EditorPrefs.SetString(GDEConstants.DefineDataColorKey, "#" + defineDataColor.ToHexString());
        EditorPrefs.SetString(GDEConstants.HighlightColorKey, "#" + highlightColor.ToHexString());

        string dataFileDirectory = dataFilePath.Replace(Path.GetFileName(dataFilePath), "");
        if (Directory.Exists(dataFileDirectory))
        {
            EditorPrefs.SetString(GDEConstants.DataFileKey, dataFilePath);
            GDEItemManager.Load(true);
            GUI.FocusControl("");
        }
        else
        {
            EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, string.Format(GDEConstants.DirectoryNotFound, dataFileDirectory), GDEConstants.OkLbl);
        }
    }

    void CreateDefaultDataFileDirectory()
    {
        string dataDirectory = Application.dataPath + GDEConstants.DefaultDataFilePath;
        if (!Directory.Exists(dataDirectory))
            Directory.CreateDirectory(dataDirectory);
    }
}

