using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

using GameDataEditor;
using GameDataEditor.Google;
using Excel;

namespace GameDataEditor
{
    public enum GDEImportView
    {
        Default,
        ImportLocalFile,
        LaunchAuthURL,
        Authenticate,
        GoogleSheetDownload,
        ImportComplete
    }
}

public class GDEImportExcel : EditorWindow {
    private GUIStyle headerStyle = null;
    private GUIStyle linkStyle = null;
    private GUIStyle buttonStyle = null;
    private GUIStyle labelStyle = null;
    private GUIStyle rateBoxStyle = null;
    private GUIStyle comboBoxStyle = null;
    private GUIStyle textFieldStyle = null;

    private string spreadsheetPath = "";
    private string progressMessage = "";
    private int processedItems = 1;
    private int totalItems = 1;

    private GDEDriveHelper googleDriveHelper;
    string accessCode = "";
    int downloadSelectionIndex = 0;

    private static GDEImportView currentView;
    private static GDEImportView nextView;

    private static float windowPadding = 20f;

	private static RemoteCertificateValidationCallback originalValidationCallback;
	
	public static bool GDEOAuthValidator (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
		return true;
	}

    private struct FieldInfo
    {
        public string name;
        public BasicFieldType type;
        public string customType;

        public bool isList;
		public bool is2DList;
        public bool isCustom;
		public bool skip;

        public string cellCol;
        public string cellRow;
        public string cellID
        {
            get{ return cellCol + cellRow; }
            private set{}
        }
    }

	static void SetCertValidation()
	{
		// Set up cert validator
		originalValidationCallback = ServicePointManager.ServerCertificateValidationCallback;
		ServicePointManager.ServerCertificateValidationCallback = GDEOAuthValidator;
	}

	static void ResetCertValidation()
	{
		ServicePointManager.ServerCertificateValidationCallback = originalValidationCallback;
	}

	void OnEnable()
	{
		SetCertValidation();
	}

	void OnDisable()
	{
		ResetCertValidation();
	}

    public void LoadSettings(GDEImportView view = GDEImportView.Default)
    {
        spreadsheetPath = EditorPrefs.GetString(GDEConstants.WorkbookFilePathKey, "");

        currentView = view;
        nextView = view;

		Vector2 newSize = new Vector2(420, 250);
		position = new Rect((Screen.width+newSize.x/4), (Screen.height+newSize.y)/2, newSize.x, newSize.y);
		minSize = newSize;
		maxSize = minSize;
    }

	void SetStyles()
	{
		if (headerStyle == null || headerStyle.name.Equals(string.Empty))
		{
			headerStyle = new GUIStyle(GUI.skin.label);
			headerStyle.fontStyle = FontStyle.Bold;
			headerStyle.fontSize = 16;
		}
		
		if (buttonStyle == null|| buttonStyle.name.Equals(string.Empty))
		{
			buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fontSize = 14;
			buttonStyle.padding = new RectOffset(15, 15, 5, 5);
		}
		
		if (labelStyle == null || labelStyle.name.Equals(string.Empty))
		{
			labelStyle = new GUIStyle(GUI.skin.label);
			labelStyle.fontSize = 14;
			labelStyle.padding = new RectOffset(0, 0, 3, 3);
		}
		
		if (linkStyle == null || linkStyle.name.Equals(string.Empty))
		{
			linkStyle = new GUIStyle(GUI.skin.label);
			linkStyle.fontSize = 16;
			linkStyle.alignment = TextAnchor.MiddleCenter;
			linkStyle.normal.textColor = Color.blue;
		}
		
		if (rateBoxStyle == null || rateBoxStyle.name.Equals(string.Empty))
		{
			rateBoxStyle = new GUIStyle(GUI.skin.box);
			rateBoxStyle.normal.background = (Texture2D)AssetDatabase.LoadAssetAtPath(GDEConstants.BorderTexturePath, typeof(Texture2D));
			rateBoxStyle.border = new RectOffset(2, 2, 2, 2);
		}
		
		if (comboBoxStyle == null || comboBoxStyle.name.Equals(string.Empty))
		{
			comboBoxStyle = new GUIStyle(EditorStyles.popup);
			comboBoxStyle.fontSize = 14;
			comboBoxStyle.fixedHeight = 24f;
		}
		
		if (textFieldStyle == null || textFieldStyle.name.Equals(string.Empty))
		{
			textFieldStyle = new GUIStyle(GUI.skin.textField);
			textFieldStyle.fontSize = 14;
			textFieldStyle.fixedHeight = 22f;
			textFieldStyle.alignment = TextAnchor.UpperLeft;
			textFieldStyle.margin = new RectOffset(0, 0, 0, 8);
		}
	}
    
    void OnGUI()
    {
		SetStyles();

		if (currentView.Equals(GDEImportView.Default))
            DrawDefaultView();
        else if (currentView.Equals(GDEImportView.LaunchAuthURL))
            DrawLaunchAuthURL();
        else if (currentView.Equals(GDEImportView.Authenticate))
            DrawAuthenticateView();
        else if (currentView.Equals(GDEImportView.GoogleSheetDownload))
            DrawGoogleSheetDownload();
        else if (currentView.Equals(GDEImportView.ImportLocalFile))
            DrawImportLocalFile();
        else if (currentView.Equals(GDEImportView.ImportComplete))
            DrawImportComplete();

        currentView = nextView;
    }

    void DrawDefaultView()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIContent content = new GUIContent("Choose Import Method");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label(content, headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20f);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Import Local File", buttonStyle))
            nextView = GDEImportView.ImportLocalFile;
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Import Google SpreadSheet", buttonStyle))
        {
            if (HasAuthenticated())
            {
                nextView = GDEImportView.GoogleSheetDownload;
                googleDriveHelper.GetSpreadsheetList();
            }
            else
                nextView = GDEImportView.LaunchAuthURL;
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();        
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reauthenticate With Google", buttonStyle))
        {
            nextView = GDEImportView.LaunchAuthURL;
        }
        GUILayout.FlexibleSpace();       
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
    }

    void DrawImportLocalFile()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        GUIContent content = new GUIContent("Import Excel Workbook");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label(content, headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();        
        GUILayout.Space(windowPadding);
        content.text = "Excel File (.xlsx or .xls)";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();        
        GUILayout.Space(windowPadding);
        spreadsheetPath = EditorGUILayout.TextField(spreadsheetPath, textFieldStyle);
        GUILayout.Space(windowPadding);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        if (GUILayout.Button("Browse ...", buttonStyle))
        {
            string newSpreadSheetPath = EditorUtility.OpenFilePanel("Open Workbook", spreadsheetPath, string.Empty);
            if (!string.IsNullOrEmpty(newSpreadSheetPath) && !newSpreadSheetPath.Equals(spreadsheetPath))
                spreadsheetPath = newSpreadSheetPath;
            GUI.FocusControl(string.Empty);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding*2f);
        if (GUILayout.Button("Back", buttonStyle))        
            nextView = GDEImportView.Default;

        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Import", buttonStyle))
        {
            EditorPrefs.SetString(GDEConstants.WorkbookFilePathKey, spreadsheetPath);
            ProcessSheet();
            nextView = GDEImportView.ImportComplete;
        }
        GUILayout.Space(windowPadding*2f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(windowPadding);
    }

    void DrawLaunchAuthURL()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        GUIContent content = new GUIContent("Authenticate With Google");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label(content, headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        content.text = "1) Make sure you are logged in to the";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding+20f);
        content.text = "correct google account in your browser.";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        content.text = "2) Authorize access to your Google Sheet.";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding+20f);
        content.text = "Enter the code specified after you accept.";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        if (GUILayout.Button("Back", buttonStyle))        
            nextView = GDEImportView.Default;

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Go to Google Authenticate URL", buttonStyle))
        {
            if (googleDriveHelper == null)
                googleDriveHelper = new GDEDriveHelper();
            
            googleDriveHelper.RequestAuthFromUser();
            nextView = GDEImportView.Authenticate;
        }
        GUILayout.Space(windowPadding*2f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(windowPadding);
    }

    void DrawAuthenticateView()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        GUIContent content = new GUIContent("Authenticate With Google");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label(content, headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        content.text = "Enter Access Code from Google:";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        accessCode = EditorGUILayout.TextField(accessCode, textFieldStyle);
        GUILayout.Space(windowPadding);
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding*2f);
        if (GUILayout.Button("Back", buttonStyle))        
            nextView = GDEImportView.LaunchAuthURL;

        GUILayout.FlexibleSpace();

        if (accessCode != string.Empty && GUILayout.Button("Set Code", buttonStyle))
        {
            googleDriveHelper.SetAccessCode(accessCode);
            googleDriveHelper.GetSpreadsheetList();
            nextView = GDEImportView.GoogleSheetDownload;
        }
        GUILayout.Space(windowPadding*2f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(windowPadding);
    }

    void DrawGoogleSheetDownload()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        GUIContent content = new GUIContent("Download Google Sheet");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label(content, headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();
        
        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        content.text = "Select Spreadsheet to Import:";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        downloadSelectionIndex = EditorGUILayout.Popup(downloadSelectionIndex, googleDriveHelper.SpreadSheetNames, comboBoxStyle);
        GUILayout.Space(windowPadding);
        EditorGUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding*2f);
        if (GUILayout.Button("Back", buttonStyle))        
            nextView = GDEImportView.Default;
        
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Download", buttonStyle))
        {
            googleDriveHelper.DownloadSpreadSheet(googleDriveHelper.SpreadSheetNames[downloadSelectionIndex]);
            spreadsheetPath = googleDriveHelper.DownloadPath;

            ProcessSheet();
            nextView = GDEImportView.ImportComplete;
        }
        GUILayout.Space(windowPadding*2f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(windowPadding);
    }

    void DrawImportComplete()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUIContent content = new GUIContent("Import Complete!");
        Vector2 size = headerStyle.CalcSize(content);
        GUILayout.Label(content, headerStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20f);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        content.text = "Your import is complete. Close this window or";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding);
        content.text = "select \"Import Again\" to import a different spreadsheet.";
        size = labelStyle.CalcSize(content);
        GUILayout.Label(content, labelStyle, GUILayout.Width(size.x), GUILayout.Height(size.y));
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(20f);

        // Draw rate box    
        float heightOfBox = 50f;
        float topOfBox = this.position.height * .5f + 5f;
        float bottomOfBox = topOfBox + heightOfBox;

        content.text = GDEConstants.ForumLinkText;
        size = linkStyle.CalcSize(content);

        float widthOfBox = size.x+10f;
        float leftOfBox = (this.position.width - widthOfBox)/2f;

        if (GUI.Button(new Rect(leftOfBox+6f, bottomOfBox-size.y-2f, size.x, size.y), content, linkStyle))
        {
            Application.OpenURL(GDEConstants.ForumURL);
        }
        
        content.text = GDEConstants.RateMeText;
        if(GUI.Button(new Rect(leftOfBox+6f, topOfBox+3f, size.x, size.y), content, linkStyle))
        {
            Application.OpenURL(GDEConstants.RateMeURL);
        }
        
        GUI.Box(new Rect(leftOfBox, topOfBox, 2f, heightOfBox), string.Empty, rateBoxStyle);
        GUI.Box(new Rect(leftOfBox, topOfBox, widthOfBox, 2f), string.Empty, rateBoxStyle);
        GUI.Box(new Rect(leftOfBox, topOfBox+heightOfBox, widthOfBox+2f, 2f), string.Empty, rateBoxStyle);
        GUI.Box(new Rect(leftOfBox+widthOfBox, topOfBox, 2f, heightOfBox), string.Empty, rateBoxStyle);

        GUILayout.FlexibleSpace();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(windowPadding*2f);
        if (GUILayout.Button("Import Again", buttonStyle))        
            nextView = GDEImportView.Default;
        
        GUILayout.FlexibleSpace();
        
        if (GUILayout.Button("Close", buttonStyle))
        {
			Close();
        }
        GUILayout.Space(windowPadding*2f);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(windowPadding);
    }

    void OnInspectorUpdate() 
    {
        Repaint();
    }

    bool HasAuthenticated()
    {
        if (googleDriveHelper == null)
            googleDriveHelper = new GDEDriveHelper();
        
        return googleDriveHelper.HasAuthenticated();
    }

    void ProcessSheet()
    {
        try
        {
            // Clear any data already loaded into the GDEItemManager
            GDEItemManager.ClearAll();
            GDEItemManager.Save();

            ExcelDataHelper workbookHelper = new ExcelDataHelper(spreadsheetPath);
            List<string> sheetNames = workbookHelper.GetSheetNames();
            Dictionary<string, List<FieldInfo>> allFields = new Dictionary<string, List<FieldInfo>>();
            Dictionary<string, List<DataRow>> allSheetData = new Dictionary<string, List<DataRow>>();

            // Calculate Progressbar settings
            progressMessage = "Importing Schemas...";
            processedItems = 0;
            totalItems = sheetNames.Count;

            // Create all the schemas first
            foreach(string name in sheetNames)
            {
                EditorUtility.DisplayProgressBar("Importing Game Data", progressMessage, processedItems/totalItems);

                List<DataRow> sheetRows = workbookHelper.GetSheetData(name);
                allSheetData.Add(name, sheetRows);

                List<FieldInfo> fields = GetFields(name, sheetRows);
                allFields.Add(name, fields);

                CreateSchema(name, fields);

                processedItems++;
            }

            // Calculate Progressbar settings
            progressMessage = "Importing Items...";
            processedItems = 0;
            totalItems = 0;
            foreach(KeyValuePair<string, List<DataRow>> pair in allSheetData)        
                totalItems += (pair.Value.Count-2) * allFields[pair.Key].Count;

            // Then create all the items for each schema       
            foreach(string name in sheetNames)
            {
                List<DataRow> sheetRows = allSheetData[name];
                List<FieldInfo> fields = allFields[name];

                CreateItems(name, fields, sheetRows);
            }

            workbookHelper.CloseWorkbook();
            GDEItemManager.Save();
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    void CreateSchema(string name, List<FieldInfo> fields)
    {
        string error;
        Dictionary<string, object> schemaData = new Dictionary<string, object>();
        GDEItemManager.AddSchema(name, schemaData, out error, true);

        foreach(FieldInfo field in fields)
        {
			if (field.skip)
				continue;
            else if (field.type != BasicFieldType.Undefined)
            {
                GDEItemManager.AddBasicFieldToSchema(field.type, name, schemaData, field.name, out error, field.isList, 
                                                 field.is2DList, GDEItemManager.GetDefaultValueForType(field.type));
            }
            else
            {
                GDEItemManager.AddCustomFieldToSchema(field.customType, name, schemaData, field.name, field.isList, field.is2DList, out error);
            }

            if (error != string.Empty)
                Debug.LogError("Error in Sheet "+name+": "+error + " (Cell: " + field.cellID + ")");
        }
    }

    void CreateItems(string schemaName, List<FieldInfo> fields, List<DataRow> sheetRows)
    {
        Dictionary<string, object> schemaData = null;       
        GDEItemManager.AllSchemas.TryGetValue(schemaName, out schemaData);

        // If schema failed to parse (schema data is null), we can't parse any items
        if (schemaData == null)
            return;

        string itemName;
        string error;
        Dictionary<string, object> itemData;
        DataRow currentRow;
		for(int index=0;  index<sheetRows.Count;  index++)
        {
            currentRow = sheetRows[index];
            itemName = currentRow.ItemArray[0].ToString();

            // Skip rows with a blank name
            if (itemName.Equals(string.Empty) || 
			    itemName.Equals(ExcelDataHelper.IgnoreToken) ||
			    itemName.Equals(ExcelDataHelper.FieldNameToken) ||
			    itemName.Equals(ExcelDataHelper.FieldTypeToken))
                continue;

            itemData = schemaData.DeepCopy();
            itemData.Add(GDMConstants.SchemaKey, schemaName);

            for(int x=0;  x<fields.Count;  x++)
            {
                try
                {
                    List<object> matches;
					if (fields[x].skip)
						continue;
                    else if (fields[x].isList)
                    {
						if (fields[x].type.Equals(BasicFieldType.String) || 
                            fields[x].type.Equals(BasicFieldType.Vector2) ||
                            fields[x].type.Equals(BasicFieldType.Vector3) ||
                            fields[x].type.Equals(BasicFieldType.Vector4) ||
                            fields[x].type.Equals(BasicFieldType.Color) ||
                            fields[x].isCustom)
                        {
                            matches = GDEParser.Parse(currentRow.ItemArray[x+1].ToString());
                        }
                        else
                        {
                            matches = new List<object>(currentRow.ItemArray[x+1].ToString().Split(','));
                        }

                        itemData[fields[x].name] = ConvertListValueToType(fields[x].type, matches);
                    }
                    else
                    {
                        if (fields[x].type.Equals(BasicFieldType.Vector2) ||
                            fields[x].type.Equals(BasicFieldType.Vector3) ||
                            fields[x].type.Equals(BasicFieldType.Vector4) ||
                            fields[x].type.Equals(BasicFieldType.Color))
                        {
                            matches = new List<object>(currentRow.ItemArray[x+1].ToString().Split(','));
                            matches.ForEach(obj => obj = obj.ToString().Trim());
                            itemData[fields[x].name] = ConvertValueToType(fields[x].type, matches);
                        }
                        else
                            itemData[fields[x].name] = ConvertValueToType(fields[x].type, currentRow.ItemArray[x+1]);
                    }
                }
                catch
                {
                    Debug.LogError("Error in Sheet: "+schemaName+". Error parsing cell: "+fields[x].cellCol+(index+1).ToString()+". Using the default value for that type.");
                    object defaultValue = GDEItemManager.GetDefaultValueForType(fields[x].type);
                    if (fields[x].isList)
                    {
                        itemData[fields[x].name] = new List<object>();
                    }
                    else
                        itemData[fields[x].name] = defaultValue;
                }
                processedItems++;
                EditorUtility.DisplayProgressBar("Importing Game Data", progressMessage, processedItems/totalItems);
            }

            GDEItemManager.AddItem(itemName, itemData, out error);
        }
    }

    object ConvertValueToType(BasicFieldType type, object value)
    {
        object convertedValue = 0;

        switch(type)
        {
            case BasicFieldType.Bool:
            {
                string b = value.ToString().Trim();
                if (b.ToString().Equals("0"))
                    convertedValue = false;
                else if (b.ToString().Equals("1"))
                    convertedValue = true;
                else
                    convertedValue = Convert.ToBoolean(b);

                break;
            }
            case BasicFieldType.Int:
            {
                convertedValue = Convert.ToInt32(value);
                break;
            }
            case BasicFieldType.Float:
            {
                convertedValue = Convert.ToSingle(value);
                break;
            }
            case BasicFieldType.String:
            {
                convertedValue = value.ToString();
                break;
            }    
            case BasicFieldType.Color:
            {
                List<object> colorValues = value as List<object>;
                Dictionary<string, object> colorDict = new Dictionary<string, object>();
                colorDict.TryAddOrUpdateValue("r", Convert.ToSingle(colorValues[0])/255f);
                colorDict.TryAddOrUpdateValue("g", Convert.ToSingle(colorValues[1])/255f);
                colorDict.TryAddOrUpdateValue("b", Convert.ToSingle(colorValues[2])/255f);
                colorDict.TryAddOrUpdateValue("a", Convert.ToSingle(colorValues[3]));
                
                convertedValue = colorDict;
                
                break;
            }
            case BasicFieldType.Vector2:
            {
                List<object> vectValues = value as List<object>;
                Dictionary<string, object> vectDict = new Dictionary<string, object>();
                vectDict.TryAddOrUpdateValue("x", Convert.ToSingle(vectValues[0]));
                vectDict.TryAddOrUpdateValue("y", Convert.ToSingle(vectValues[1]));
                
                convertedValue = vectDict;
                
                break;
            }
            case BasicFieldType.Vector3:
            {
                List<object> vectValues = value as List<object>;
                Dictionary<string, object> vectDict = new Dictionary<string, object>();
                vectDict.TryAddOrUpdateValue("x", Convert.ToSingle(vectValues[0]));
                vectDict.TryAddOrUpdateValue("y", Convert.ToSingle(vectValues[1]));
                vectDict.TryAddOrUpdateValue("z", Convert.ToSingle(vectValues[2]));
                
                convertedValue = vectDict;
                
                break;
            }
            case BasicFieldType.Vector4:
            {
                List<object> vectValues = value as List<object>;
                Dictionary<string, object> vectDict = new Dictionary<string, object>();
                vectDict.TryAddOrUpdateValue("x", Convert.ToSingle(vectValues[0]));
                vectDict.TryAddOrUpdateValue("y", Convert.ToSingle(vectValues[1]));
                vectDict.TryAddOrUpdateValue("z", Convert.ToSingle(vectValues[2]));
                vectDict.TryAddOrUpdateValue("w", Convert.ToSingle(vectValues[3]));
                
                convertedValue = vectDict;
                
                break;
            }
            case BasicFieldType.Undefined:
            {
                convertedValue = value.ToString();
                break;
            }
        }

        return convertedValue;
    }

    List<object> ConvertListValueToType(BasicFieldType type, List<object> values)
    {
        List<object> convertedValues = new List<object>();

        if (type.Equals(BasicFieldType.String) || type.Equals(BasicFieldType.Undefined))
            values.ForEach(val => convertedValues.Add(val));
        else        
            values.ForEach(val => convertedValues.Add(ConvertValueToType(type, val)));

        return convertedValues;
    }
    
    List<FieldInfo> GetFields(string sheetName, List<DataRow> sheetRows)
    {
        List<FieldInfo> fields = new List<FieldInfo>();

        DataRow headerRow = sheetRows[0];
        
        DataRow fieldNames = sheetRows.Find(r => r[0].ToString().Equals(ExcelDataHelper.FieldNameToken));
		if (fieldNames == null)
		{
			Debug.LogError("Could not find Field Names row for sheet: "+sheetName);
			return new List<FieldInfo>();
		}

		DataRow fieldTypes = sheetRows.Find(r => r[0].ToString().Equals(ExcelDataHelper.FieldTypeToken));
		if (fieldTypes == null)
		{
			Debug.LogError("Could not find Field Types row for sheet: "+sheetName);
			return new List<FieldInfo>();
		}
        
        for(int index=1;  index<fieldNames.ItemArray.Length;  index++)
        {
			FieldInfo fieldInfo;

			if (headerRow[index].ToString().Equals(ExcelDataHelper.IgnoreToken))
			{
				fieldInfo = new FieldInfo();
				fieldInfo.skip = true;
			}
			else 
			{
            	ParseFieldType(fieldTypes[index].ToString(), out fieldInfo);
            	fieldInfo.name = fieldNames[index].ToString().Trim();
			}

            fieldInfo.cellCol = GetExcelColumnName(index);
            fieldInfo.cellRow = "1";

            fields.Add(fieldInfo);
        }

        return fields;
    }

    bool ParseFieldType(string type, out FieldInfo fieldInfo)
    {
        bool result = true;

        fieldInfo = new FieldInfo();
        fieldInfo.isList = false;
        fieldInfo.type = BasicFieldType.Undefined;
        fieldInfo.customType = string.Empty;
		fieldInfo.skip = false;

        try
        {
            if(type.StartsWith("list_"))
            {
                type = type.Replace("list_", "");
                fieldInfo.isList = true;
            }

            fieldInfo.type = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), type, true);
        }
        catch
        {
            result = false;
            fieldInfo.type = BasicFieldType.Undefined;
            fieldInfo.customType = type;
            fieldInfo.isCustom = true;
        }

        return result;
    }

    string GetExcelColumnName(int columnNumber)
    {
        int dividend = columnNumber + 1;
        string columnName = String.Empty;
        int modulo;
        
        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (int)((dividend - modulo) / 26);
        } 
        
        return columnName;
    }
}
