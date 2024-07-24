using UnityEngine;
using UnityEditor;
using GameDataEditor;

using System;
using System.IO;

public class GDEMenu : EditorWindow {
	
	const string menuItemLocation = "Window/Game Data Editor";
	const int menuItemStartPriority = 300;

	[MenuItem(menuItemLocation + "/Preferences", false, menuItemStartPriority)]
	private static void GDEPreferences()
	{
		var window = EditorWindow.GetWindow<GDEPreferences>(true, "Game Data Editor Preferences");
		window.LoadPreferences();

		Vector2 newSize = new Vector2(800, 245);
		window.position = new Rect((Screen.width+newSize.x/2)/2, (Screen.height+newSize.y)/2, newSize.x, newSize.y);
		window.Show();
	}


	// **** Divider Here **** //
	

	[MenuItem(menuItemLocation + "/Define Data", false, menuItemStartPriority+50)]
	private static void GDESchemaEditor()
	{
		EditorWindow.GetWindow<GDESchemaManagerWindow>(false, "Define Data");
	}
	
	[MenuItem(menuItemLocation + "/Create Data", false, menuItemStartPriority+51)]
	private static void GDEItemEditor()
	{
		EditorWindow.GetWindow<GDEItemManagerWindow>(false, "Create Data");
	}
	
	[MenuItem(menuItemLocation + "/Encrypt Data", false, menuItemStartPriority+52)]
	private static void GDEEncrypt()
	{
		Debug.Log("Encrypting...");
		string dataFilePath = GDEItemManager.DataFilePath;
		GDEEncryption.Encrypt(File.ReadAllText(dataFilePath), GDEItemManager.EncryptedFilePath);
		Debug.Log("Done");

		var window = EditorWindow.GetWindow<GDEEncryptionWindow>(true, "GDE Encryption Complete!");

		Vector2 newSize = new Vector2(650, 200);
		window.position = new Rect((Screen.width+newSize.x)/2, (Screen.height+newSize.y)/2, newSize.x, newSize.y);
		window.Show();
	}


	// **** Divider Here **** //


	[MenuItem(menuItemLocation + "/Import Spreadsheet", false, menuItemStartPriority+100)]
	private static void showEditor()
	{
		var window = EditorWindow.GetWindow<GDEImportExcel>(true, "Import Spreadsheet");
		window.LoadSettings();
		window.Show();
	}

	[MenuItem(menuItemLocation + "/Generate Custom Extensions", false, menuItemStartPriority+101)]
	public static void DoGenerateCustomExtensions()
	{
		GDEItemManager.Load();
		
		GDECodeGen.GenStaticKeysClass(GDEItemManager.AllSchemas);
		GDECodeGen.GenClasses(GDEItemManager.AllSchemas);

		AssetDatabase.Refresh();
	}


	// **** Divider Here **** //


	[MenuItem(menuItemLocation + "/GDE Forum", false, menuItemStartPriority+200)]
	private static void GDEForumPost()
	{
		Application.OpenURL(GDEConstants.ForumURL);
	}

	[MenuItem(menuItemLocation + "/GDE Documentation", false, menuItemStartPriority+201)]
	private static void GDEFreeDocs()
	{
		Application.OpenURL(GDEConstants.DocURL);
	}

	[MenuItem(menuItemLocation + "/Rate GDE", false, menuItemStartPriority+202)]
	private static void GDERateMe()
	{
		Application.OpenURL(GDEConstants.RateMeURL);
	}

	[MenuItem(menuItemLocation + "/Contact/Email", false, menuItemStartPriority+203)]
	private static void GDEEmail()
	{
		Application.OpenURL(GDEConstants.Contact);
	}

	[MenuItem(menuItemLocation + "/Contact/Twitter", false, menuItemStartPriority+203)]
	private static void GDETwitter()
	{
		Application.OpenURL(GDEConstants.Twitter);
	}


	// **** Context Menu Below **** //

	[MenuItem("Assets/Game Data Editor/Load Data", true)]
	private static bool GDELoadDataValidation()
	{
		return Selection.activeObject.GetType() == typeof(TextAsset);
	}

	[MenuItem("Assets/Game Data Editor/Load Data")]
	private static void GDELoadData () 
	{
		string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		string fullPath = Path.GetFullPath(assetPath);

		EditorPrefs.SetString(GDEConstants.DataFileKey, fullPath);
		GDEItemManager.Load(true);
	}

	[MenuItem("Assets/Game Data Editor/Load Data and Generate Data Classes")]
	private static void GDELoadAndGenData () 
	{
		GDELoadData();
		DoGenerateCustomExtensions();
	}
  
  	[MenuItem("Assets/Game Data Editor/Load Data and Generate Data Classes", true)]
  	private static bool GDELoadAndGenDataValidation()
  	{
    	return GDELoadDataValidation();
  	}
}

