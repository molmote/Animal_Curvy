using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

public class GDESchemaManagerWindow : GDEManagerWindowBase {

    private string newSchemaName = "";
    private Dictionary<string, int> basicFieldTypeSelectedDict = new Dictionary<string, int>();
    private Dictionary<string, int> customSchemaTypeSelectedDict = new Dictionary<string, int>();

    private Dictionary<string, string> newBasicFieldName = new Dictionary<string, string>();
    private HashSet<string> isBasicList = new HashSet<string>();
	private HashSet<string> isBasic2DList = new HashSet<string>();

	private Dictionary<string, string> newCustomFieldName = new Dictionary<string, string>();
    private HashSet<string> isCustomList = new HashSet<string>();
	private HashSet<string> isCustom2DList = new HashSet<string>();
    
    private List<string> deletedFields = new List<string>();
    private Dictionary<List<string>, Dictionary<string, object>> renamedFields = new Dictionary<List<string>, Dictionary<string, object>>();

    private List<string> deletedSchemas = new List<string>();
    private Dictionary<string, string> renamedSchemas = new Dictionary<string, string>();
    
    #region OnGUI/Header Methods
    protected override void OnGUI()
    {
        mainHeaderText = GDEConstants.DefineDataHeader;


		if (EditorGUIUtility.isProSkin)
			headerColorString = EditorPrefs.GetString(GDEConstants.DefineDataColorKey, GDEConstants.DefineDataColorPro);
        else
			headerColorString = EditorPrefs.GetString(GDEConstants.DefineDataColorKey, GDEConstants.DefineDataColor);

		base.OnGUI();

        DrawExpandCollapseAllFoldout(GDEItemManager.AllSchemas.Keys.ToArray(), GDEConstants.SchemaListHeader);

        float currentGroupHeightTotal = CalculateGroupHeightsTotal();
        scrollViewHeight = HeightToBottomOfWindow();
        scrollViewY = TopOfLine();
        verticalScrollbarPosition = GUI.BeginScrollView(new Rect(currentLinePosition, scrollViewY, FullWindowWidth(), scrollViewHeight), 
                                                        verticalScrollbarPosition,
                                                        new Rect(currentLinePosition, scrollViewY, ScrollViewWidth(), currentGroupHeightTotal));

        foreach(KeyValuePair<string, Dictionary<string, object>> schema in GDEItemManager.AllSchemas)
        {   
            // If we are filtered out, return
            if (ShouldFilter(schema.Key, schema.Value))
                continue;

            float currentGroupHeight;
            if (!groupHeights.TryGetValue(schema.Key, out currentGroupHeight))
                currentGroupHeight = GDEConstants.LineHeight;
            
            if (IsVisible(currentGroupHeight))
                DrawEntry(schema.Key, schema.Value);
            else
            {
                NewLine(currentGroupHeight/GDEConstants.LineHeight);
            }
        }
        GUI.EndScrollView();

        //Remove any schemas that were deleted
        foreach(string deletedSchemaKey in deletedSchemas)        
            Remove(deletedSchemaKey);
        deletedSchemas.Clear();

        // Rename any schemas that were renamed
        string error;
        foreach(KeyValuePair<string, string> pair in renamedSchemas)
        {
            if (!GDEItemManager.RenameSchema(pair.Key, pair.Value, out error))
                EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, string.Format("Couldn't rename {0} to {1}: {2}", pair.Key, pair.Value, error), GDEConstants.OkLbl);
        }
        renamedSchemas.Clear();
    }
    #endregion

    #region Draw Methods
    protected override void DrawCreateSection()
    {
        float topOfSection = TopOfLine() + 4f;
        float bottomOfSection = 0;
        float leftBoundary = 0;
        float width = 0;

        DrawSubHeader(GDEConstants.CreateNewSchemaHeader);

        width = 100;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.SchemaNameLbl);
        currentLinePosition += (width + 2);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;

        width = 120;
        newSchemaName = EditorGUI.TextField(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), newSchemaName);
        currentLinePosition += (width + 2);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;

        GUIContent content = new GUIContent(GDEConstants.CreateNewSchemaBtn);
        width = GUI.skin.button.CalcSize(content).x;
        if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
        {
            if (Create(newSchemaName))
            {
                newSchemaName = "";
                GUI.FocusControl("");
            }
        }
        currentLinePosition += (width + 6f);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;

        NewLine();

        bottomOfSection = TopOfLine();

        DrawSectionSeparator();

        leftBoundary += 5f;
        
        // Draw rate box        
        content.text = GDEConstants.ForumLinkText;
        Vector2 size = linkStyle.CalcSize(content);
        width = size.x;
        if (GUI.Button(new Rect(leftBoundary+5f, bottomOfSection-size.y-2f, width+5f, size.y), content, linkStyle))
        {
            Application.OpenURL(GDEConstants.ForumURL);
        }
        
        content.text = GDEConstants.RateMeText;
        size = linkStyle.CalcSize(content);
        if(GUI.Button(new Rect(leftBoundary+5f, topOfSection+3f, width+5f, size.y), content, linkStyle))
        {
            Application.OpenURL(GDEConstants.RateMeURL);
        }
        
        DrawRateBox(leftBoundary, topOfSection, width+10f, bottomOfSection-topOfSection);
    }

    private void DrawAddFieldSection(string schemaKey, Dictionary<string, object> schemaData)
    {
        currentLinePosition += GDEConstants.Indent;

        GUIContent content = new GUIContent(GDEConstants.NewFieldHeader);
        float width = labelStyle.CalcSize(content).x;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content, labelStyle);

        NewLine();

        currentLinePosition += GDEConstants.Indent;

        // ***** Basic Field Type Group ***** //
        width = 120;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.BasicFieldTypeLbl);
        currentLinePosition += (width + 2);

        // Basic field type selected
        int basicFieldTypeIndex;
        if (!basicFieldTypeSelectedDict.TryGetValue(schemaKey, out basicFieldTypeIndex))
        {
            basicFieldTypeIndex = 0;
            basicFieldTypeSelectedDict.TryAddValue(schemaKey, basicFieldTypeIndex);
        }

        width = 80;
        int newBasicFieldTypeIndex = EditorGUI.Popup(new Rect(currentLinePosition, PopupTop(), width, StandardHeight()), basicFieldTypeIndex, GDEItemManager.BasicFieldTypeStringArray);
        currentLinePosition += (width + 6);

        if (newBasicFieldTypeIndex != basicFieldTypeIndex && GDEItemManager.BasicFieldTypeStringArray.IsValidIndex(newBasicFieldTypeIndex))
        {
            basicFieldTypeIndex = newBasicFieldTypeIndex;
            basicFieldTypeSelectedDict.TryAddOrUpdateValue(schemaKey, basicFieldTypeIndex);
        }


        // Basic field type name field
        string newBasicFieldNameText = "";
        if (!newBasicFieldName.TryGetValue(schemaKey, out newBasicFieldNameText))
        {
            newBasicFieldName.Add(schemaKey, "");
            newBasicFieldNameText = "";
        }

        width = 70;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.FieldNameLbl);
        currentLinePosition += (width + 2);

        width = 120;
        newBasicFieldNameText = EditorGUI.TextField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), newBasicFieldNameText);
        currentLinePosition += (width + 6);

        if (!newBasicFieldNameText.Equals(newBasicFieldName[schemaKey]))
            newBasicFieldName[schemaKey] = newBasicFieldNameText;


        // Basic field type isList checkbox
        bool isBasicListTemp = isBasicList.Contains(schemaKey);
		content.text = GDEConstants.IsListLbl;
		width = GUI.skin.label.CalcSize(content).x;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
        currentLinePosition += (width + 2);

        width = 15;
        isBasicListTemp = EditorGUI.Toggle(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), isBasicListTemp);
        currentLinePosition += (width + 6);

        if (isBasicListTemp && !isBasicList.Contains(schemaKey))
		{
			isBasicList.Add(schemaKey);

			// Turn 2D List off
			isBasic2DList.Remove(schemaKey);
		}
        else if (!isBasicListTemp && isBasicList.Contains(schemaKey))
            isBasicList.Remove(schemaKey);


		// Basic field type is2DList checkbox
		bool isBasic2DListTemp = isBasic2DList.Contains(schemaKey);
		content.text = GDEConstants.Is2DListLbl;
		width = GUI.skin.label.CalcSize(content).x;
		GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
		currentLinePosition += (width + 2);
		
		width = 15;
		isBasic2DListTemp = EditorGUI.Toggle(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), isBasic2DListTemp);
		currentLinePosition += (width + 6);
		
		if (isBasic2DListTemp && !isBasic2DList.Contains(schemaKey))
		{
			isBasic2DList.Add(schemaKey);

			// Turn off 1D List
			isBasicList.Remove(schemaKey);
		}
		else if (!isBasic2DListTemp && isBasic2DList.Contains(schemaKey))
			isBasic2DList.Remove(schemaKey);


        content.text = GDEConstants.AddFieldBtn;
        width = GUI.skin.button.CalcSize(content).x;
        if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
        {
            if (AddBasicField(GDEItemManager.BasicFieldTypes[basicFieldTypeIndex], schemaKey, schemaData, newBasicFieldNameText, isBasicListTemp, isBasic2DListTemp))
            {
                isBasicList.Remove(schemaKey);
                newBasicFieldName.TryAddOrUpdateValue(schemaKey, "");

                newBasicFieldNameText = "";
                GUI.FocusControl("");
            }
        }

        NewLine();


        // ****** Custom Field Type Group ****** //
        currentLinePosition += GDEConstants.Indent;

        width = 120;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.CustomFieldTypeLbl);
        currentLinePosition += (width + 2);

        List<string> customTypeList = GDEItemManager.AllSchemas.Keys.ToList();
        customTypeList.Remove(schemaKey);

        string[] customTypes = customTypeList.ToArray();

        int customSchemaTypeIndex;
        if (!customSchemaTypeSelectedDict.TryGetValue(schemaKey, out customSchemaTypeIndex))
        {
            customSchemaTypeIndex = 0;
            customSchemaTypeSelectedDict.TryAddValue(schemaKey, customSchemaTypeIndex);
        }

        // Custom schema type selected
        width = 80;
        int newCustomSchemaTypeSelected = EditorGUI.Popup(new Rect(currentLinePosition, PopupTop(), width, StandardHeight()), customSchemaTypeIndex, customTypes);
        currentLinePosition += (width + 6);

        if (newCustomSchemaTypeSelected != customSchemaTypeIndex && customTypes.IsValidIndex(newCustomSchemaTypeSelected))
        {
            customSchemaTypeIndex = newCustomSchemaTypeSelected;
            customSchemaTypeSelectedDict.TryAddOrUpdateValue(schemaKey, customSchemaTypeIndex);
        }


        // Custom field type name field
        string newCustomFieldNameText = "";
        if (!newCustomFieldName.TryGetValue(schemaKey, out newCustomFieldNameText))
        {
            newCustomFieldName.Add(schemaKey, "");
            newCustomFieldNameText = "";
        }

        width = 70;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.FieldNameLbl);
        currentLinePosition += (width + 2);

        width = 120;
        newCustomFieldNameText = EditorGUI.TextField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), newCustomFieldNameText);
        currentLinePosition += (width + 6);

        if (!newCustomFieldNameText.Equals(newCustomFieldName[schemaKey]))
            newCustomFieldName[schemaKey] = newCustomFieldNameText;


        // Custom field type isList checkbox
        bool isCustomListTemp = isCustomList.Contains(schemaKey);

        width = 38;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.IsListLbl);
        currentLinePosition += (width + 2);

        width = 15;
        isCustomListTemp = EditorGUI.Toggle(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), isCustomListTemp);
        currentLinePosition += (width + 6);

        if (isCustomListTemp && !isCustomList.Contains(schemaKey))
		{
			isCustomList.Add(schemaKey);

			// Turn off 2D List
			isCustom2DList.Remove(schemaKey);
		}
        else if(!isCustomListTemp && isCustomList.Contains(schemaKey))
            isCustomList.Remove(schemaKey);


		// Custom field type is2DList checkbox
		bool isCustom2DListTemp = isCustom2DList.Contains(schemaKey);
		content.text = GDEConstants.Is2DListLbl;
		width = GUI.skin.label.CalcSize(content).x;
		GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
		currentLinePosition += (width + 2);
		
		width = 15;
		isCustom2DListTemp = EditorGUI.Toggle(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), isCustom2DListTemp);
		currentLinePosition += (width + 6);
		
		if (isCustom2DListTemp && !isCustom2DList.Contains(schemaKey))
		{
			isCustom2DList.Add(schemaKey);
			
			// Turn off 1D List
			isCustomList.Remove(schemaKey);
		}
		else if (!isCustom2DListTemp && isCustom2DList.Contains(schemaKey))
			isCustom2DList.Remove(schemaKey);


        content.text = GDEConstants.AddCustomFieldBtn;
        width = GUI.skin.button.CalcSize(content).x;
        if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
        {
            if (!customTypes.IsValidIndex(customSchemaTypeIndex) || customTypes.Length.Equals(0))
            {
                EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, GDEConstants.InvalidCustomFieldType, GDEConstants.OkLbl);
            }
            else if (AddCustomField(customTypes[customSchemaTypeIndex], schemaKey, schemaData, newCustomFieldNameText, isCustomListTemp, isCustom2DListTemp))
            {
                isCustomList.Remove(schemaKey);
                newCustomFieldName.TryAddOrUpdateValue(schemaKey, "");
                newCustomFieldNameText = "";
                GUI.FocusControl("");
            }
        }
    }

    protected override void DrawEntry(string schemaKey, Dictionary<string, object> schemaData)
    {
        float beginningHeight = CurrentHeight();

		// Start drawing below
		bool isOpen = DrawFoldout(GDEConstants.SchemaLbl + " ", schemaKey, schemaKey, schemaKey, RenameSchema);
		NewLine();

        if (isOpen)
        {
            bool shouldDrawSpace = false;
            bool didDrawSpaceForSection = false;
			bool isFirstSection = true;
			int listDimension = 0;

            // Draw the basic types
            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
            {
                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), 0);
                foreach(string fieldKey in fieldKeys)
                {
                    currentLinePosition += GDEConstants.Indent;
                    DrawSingleField(schemaKey, fieldKey, schemaData);
                    shouldDrawSpace = true;
					isFirstSection = false;
                }
            }

            // Draw the custom types
            foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, 0))
            {
                if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
                {
                    NewLine(0.5f);
                }

                currentLinePosition += GDEConstants.Indent;
                DrawSingleField(schemaKey, fieldKey, schemaData);

                shouldDrawSpace = true;
				isFirstSection = false;
				didDrawSpaceForSection = true;
            }
            didDrawSpaceForSection = false;

            // Draw the lists
			for(int dimension=1;  dimension <=2;  dimension++)
			{
	            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
	            {
	                List<string> fieldKeys = GDEItemManager.SchemaFieldKeysOfType(schemaKey, fieldType.ToString(), dimension);
	                foreach(string fieldKey in fieldKeys)
	                {
						string isListKey = string.Format(GDMConstants.MetaDataFormat, GDMConstants.IsListPrefix, fieldKey);
						schemaData.TryGetInt(isListKey, out listDimension);

	                    if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
	                    {
	                        NewLine(0.5f);
	                    }

	                    currentLinePosition += GDEConstants.Indent;
						if (listDimension == 1)
	                    	DrawListField(schemaKey, schemaData, fieldKey);
						else
							Draw2DListField(schemaKey, schemaData, fieldKey);

	                    shouldDrawSpace = true;
						didDrawSpaceForSection = true;
						isFirstSection = false;
	                }
	            }
	            didDrawSpaceForSection = false;
			}

            // Draw the custom lists
			for(int dimension=1;  dimension <=2;  dimension++)
			{
	            foreach(string fieldKey in GDEItemManager.SchemaCustomFieldKeys(schemaKey, dimension))
	            {
	                if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
	                {
	                    NewLine(0.5f);
	                }

	                currentLinePosition += GDEConstants.Indent;
	                if (dimension == 1)
						DrawListField(schemaKey, schemaData, fieldKey);
					else
						Draw2DListField(schemaKey, schemaData, fieldKey);

	                shouldDrawSpace = true;
					didDrawSpaceForSection = true;
					isFirstSection = false;
	            }
			}
			didDrawSpaceForSection = false;

            // Remove any fields that were deleted above
            foreach(string deletedKey in deletedFields)
            {
                RemoveField(schemaKey, schemaData, deletedKey);
            }
            deletedFields.Clear();

            // Rename any fields that were renamed
            string error;
            string oldFieldKey;
            string newFieldKey;
            foreach(KeyValuePair<List<string>, Dictionary<string, object>> pair in renamedFields)
            {
                oldFieldKey = pair.Key[0];
                newFieldKey = pair.Key[1];
                if (!GDEItemManager.RenameSchemaField(oldFieldKey, newFieldKey, schemaKey, pair.Value, out error))
                    EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, string.Format("Couldn't rename {0} to {1}: {2}", oldFieldKey, newFieldKey, error), GDEConstants.OkLbl);
            }
            renamedFields.Clear();

            NewLine();

            DrawAddFieldSection(schemaKey, schemaData);
            
            NewLine(2f);

            GUIContent content = new GUIContent(GDEConstants.DeleteBtn);
            float width = GUI.skin.button.CalcSize(content).x;
            if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                deletedSchemas.Add(schemaKey);

            NewLine();
            
            DrawSectionSeparator();

            NewLine(0.25f);
        }
        
		float groupHeight = CurrentHeight() - beginningHeight;
        SetGroupHeight(schemaKey, groupHeight);
    }

    void DrawSingleField(string schemaKey, string fieldKey, Dictionary<string, object> schemaData)
    {
        string fieldType;
        schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out fieldType);

        BasicFieldType fieldTypeEnum = BasicFieldType.Undefined;
        if (Enum.IsDefined(typeof(BasicFieldType), fieldType))
        {
            fieldTypeEnum = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), fieldType);
            if (!fieldTypeEnum.Equals(BasicFieldType.Vector2) && 
                !fieldTypeEnum.Equals(BasicFieldType.Vector3) && 
                !fieldTypeEnum.Equals(BasicFieldType.Vector4) &&
                !fieldTypeEnum.Equals(BasicFieldType.Color))
                fieldType = fieldType.ToLower();
        }

		GUIContent content = new GUIContent(fieldType);
		float width = Math.Max(labelStyle.CalcSize(content).x, GDEConstants.MinLabelWidth);
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content, labelStyle);
        currentLinePosition += (width + 2);

		string editFieldKey = string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey);
        DrawEditableLabel(fieldKey, editFieldKey, RenameSchemaField, schemaData);

        switch(fieldTypeEnum)
        {
            case BasicFieldType.Bool:
                DrawBool(fieldKey, schemaData, GDEConstants.DefaultValueLbl);
                break;
            case BasicFieldType.Int:
                DrawInt(fieldKey, schemaData, GDEConstants.DefaultValueLbl);
                break;
            case BasicFieldType.Float:
                DrawFloat(fieldKey, schemaData, GDEConstants.DefaultValueLbl);
                break;
            case BasicFieldType.String:
                DrawString(fieldKey, schemaData, GDEConstants.DefaultValueLbl);
                break;

            case BasicFieldType.Vector2:
                DrawVector2(fieldKey, schemaData, GDEConstants.DefaultValuesLbl);
                break;
            case BasicFieldType.Vector3:
                DrawVector3(fieldKey, schemaData, GDEConstants.DefaultValuesLbl);
                break;
            case BasicFieldType.Vector4:
                DrawVector4(fieldKey, schemaData, GDEConstants.DefaultValuesLbl);
                break;
            
            case BasicFieldType.Color:
                DrawColor(fieldKey, schemaData, GDEConstants.DefaultValuesLbl);
                break;

            default:
                DrawCustom(fieldKey, schemaData, false);
                break;
        }

        content.text = GDEConstants.DeleteBtn;
        width = GUI.skin.button.CalcSize(content).x;
        if (fieldTypeEnum.Equals(BasicFieldType.Vector2) ||
            fieldTypeEnum.Equals(BasicFieldType.Vector3) ||
            fieldTypeEnum.Equals(BasicFieldType.Vector4))
        {
            if (GUI.Button(new Rect(currentLinePosition, VerticalMiddleOfLine(), width, StandardHeight()), content))
                deletedFields.Add(fieldKey);

            NewLine(GDEConstants.VectorFieldBuffer+1);
        }
        else
        {
            if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                deletedFields.Add(fieldKey);

            NewLine();
        }
    }

    void DrawListField(string schemaKey, Dictionary<string, object> schemaData, string fieldKey)
    {
        try
        {
            string foldoutKey = string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey);
            bool newFoldoutState;
            bool currentFoldoutState = listFieldFoldoutState.Contains(foldoutKey);
            object defaultResizeValue = null;

            string fieldType;
            schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out fieldType);

            BasicFieldType fieldTypeEnum = BasicFieldType.Undefined;
            if (Enum.IsDefined(typeof(BasicFieldType), fieldType))
            {
                fieldTypeEnum = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), fieldType);
                if (!fieldTypeEnum.Equals(BasicFieldType.Vector2) && 
                    !fieldTypeEnum.Equals(BasicFieldType.Vector3) && 
                    !fieldTypeEnum.Equals(BasicFieldType.Vector4) && 
                    !fieldTypeEnum.Equals(BasicFieldType.Color))                
                    fieldType = fieldType.ToLower();

                defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);
            }

			GUIContent content = new GUIContent(string.Format("List<{0}>", fieldType));
			float width = Math.Max(EditorStyles.foldout.CalcSize(content).x, GDEConstants.MinLabelWidth);
            newFoldoutState = EditorGUI.Foldout(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), currentFoldoutState, content);
            currentLinePosition += (width + 2);
            
            DrawEditableLabel(fieldKey, string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey), RenameSchemaField, schemaData);

            if (newFoldoutState != currentFoldoutState)
            {
                if (newFoldoutState)
                    listFieldFoldoutState.Add(foldoutKey);
                else
                    listFieldFoldoutState.Remove(foldoutKey);
            }

            object temp = null;
            List<object> list = null;

            if (schemaData.TryGetValue(fieldKey, out temp))
                list = temp as List<object>;

            content.text = GDEConstants.DefaultSizeLbl;
            width = GUI.skin.label.CalcSize(content).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
            currentLinePosition += (width + 2);

            int newListCount;
            string listCountKey = string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey);
            if (newListCountDict.ContainsKey(listCountKey))
            {
                newListCount = newListCountDict[listCountKey];
            }
            else
            {
                newListCount = list.Count;
                newListCountDict.Add(listCountKey, newListCount);
            }

            width = 40;
            newListCount = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), newListCount);
            currentLinePosition += (width + 2);

            newListCountDict[listCountKey] = newListCount;

            content.text = GDEConstants.ResizeBtn;
            width = GUI.skin.button.CalcSize(content).x;
            if (newListCount != list.Count)
            {
                if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                    ResizeList(list, newListCount, defaultResizeValue);
                currentLinePosition += (width + 2);
            }
                 
            content.text = GDEConstants.DeleteBtn;
            width = GUI.skin.button.CalcSize(content).x;
            if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                deletedFields.Add(fieldKey);

            NewLine();

            if (newFoldoutState)
            {
                for (int i = 0; i < list.Count; i++) 
                {
                    currentLinePosition += GDEConstants.Indent*2;
					content.text = string.Format("[{0}]:", i);

                    switch (fieldTypeEnum) {
                        case BasicFieldType.Bool:
                        {
                            DrawListBool(content, i, Convert.ToBoolean(list[i]), list);
                            NewLine();
                            break;
                        }
                        case BasicFieldType.Int:
                        {
							DrawListInt(content, i, Convert.ToInt32(list[i]), list);
                            NewLine();
                            break;
                        }
                        case BasicFieldType.Float:
                        {
							DrawListFloat(content, i, Convert.ToSingle(list[i]), list);
                            NewLine();
                            break;
                        }
                        case BasicFieldType.String:
                        {
							DrawListString(content, i, list[i] as string, list);
                            NewLine();
                            break;
                        }
                        case BasicFieldType.Vector2:
                        {
							DrawListVector2(content, i, list[i] as Dictionary<string, object>, list);
                            NewLine(GDEConstants.VectorFieldBuffer+1);
                            break;
                        }
                        case BasicFieldType.Vector3:
                        {
							DrawListVector3(content, i, list[i] as Dictionary<string, object>, list);
                            NewLine(GDEConstants.VectorFieldBuffer+1);
                            break;
                        }
                        case BasicFieldType.Vector4:
                        {
							DrawListVector4(content, i, list[i] as Dictionary<string, object>, list);
                            NewLine(GDEConstants.VectorFieldBuffer+1);
                            break;
                        }
                        case BasicFieldType.Color:
                        {
							DrawListColor(content, i, list[i] as Dictionary<string, object>, list);
                            NewLine();
                            break;
                        }
                        default:
                        {
							DrawListCustom(content, i, list[i] as string, list, false);
                            NewLine();
                            break;
                        }
                    }
                }
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

	void Draw2DListField(string schemaKey, Dictionary<string, object> schemaData, string fieldKey)
	{
		try
		{
			string foldoutKey = string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey);
			bool newFoldoutState;
			bool currentFoldoutState = listFieldFoldoutState.Contains(foldoutKey);
			object defaultResizeValue = new List<object>();
			
			string fieldType;
			schemaData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out fieldType);
			
			BasicFieldType fieldTypeEnum = BasicFieldType.Undefined;
			if (Enum.IsDefined(typeof(BasicFieldType), fieldType))
			{
				fieldTypeEnum = (BasicFieldType)Enum.Parse(typeof(BasicFieldType), fieldType);
				if (!fieldTypeEnum.Equals(BasicFieldType.Vector2) && 
				    !fieldTypeEnum.Equals(BasicFieldType.Vector3) && 
				    !fieldTypeEnum.Equals(BasicFieldType.Vector4) && 
				    !fieldTypeEnum.Equals(BasicFieldType.Color))                
					fieldType = fieldType.ToLower();
			}
			
			GUIContent content = new GUIContent(string.Format("List<List<{0}>>", fieldType));
			float width = Math.Max(EditorStyles.foldout.CalcSize(content).x, GDEConstants.MinLabelWidth);
			newFoldoutState = EditorGUI.Foldout(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), currentFoldoutState, content);
			currentLinePosition += (width + 2);
			
			DrawEditableLabel(fieldKey, string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey), RenameSchemaField, schemaData);
			
			if (newFoldoutState != currentFoldoutState)
			{
				if (newFoldoutState)
					listFieldFoldoutState.Add(foldoutKey);
				else
					listFieldFoldoutState.Remove(foldoutKey);
			}
			
			object temp = null;
			List<object> list = null;
			
			if (schemaData.TryGetValue(fieldKey, out temp))
				list = temp as List<object>;
			
			content.text = GDEConstants.DefaultSizeLbl;
			width = GUI.skin.label.CalcSize(content).x;
			GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
			currentLinePosition += (width + 2);
			
			int newListCount;
			string listCountKey = string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey);
			if (newListCountDict.ContainsKey(listCountKey))
			{
				newListCount = newListCountDict[listCountKey];
			}
			else
			{
				newListCount = list.Count;
				newListCountDict.Add(listCountKey, newListCount);
			}
			
			width = 40;
			newListCount = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), newListCount);
			currentLinePosition += (width + 2);
			
			newListCountDict[listCountKey] = newListCount;
			
			content.text = GDEConstants.ResizeBtn;
			width = GUI.skin.button.CalcSize(content).x;
			if (newListCount != list.Count)
			{
				if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
					ResizeList(list, newListCount, defaultResizeValue);
				currentLinePosition += (width + 2);
			}
			
			content.text = GDEConstants.DeleteBtn;
			width = GUI.skin.button.CalcSize(content).x;
			if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
				deletedFields.Add(fieldKey);
			
			NewLine();
			
			if (newFoldoutState)
			{
				defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);
				for (int index = 0; index < list.Count; index++) 
				{
					List<object> subList = list[index] as List<object>;

					currentLinePosition += GDEConstants.Indent*2;
					content.text = string.Format("[{0}]: List<{1}>", index, fieldType);

					bool isOpen = DrawFoldout(content.text, foldoutKey+"_"+index, "", "", null);

					// Draw resize
					content.text = GDEConstants.DefaultSizeLbl;
					width = GUI.skin.label.CalcSize(content).x;
					GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
					currentLinePosition += (width + 2);
					
					listCountKey = string.Format(GDMConstants.MetaDataFormat, schemaKey, fieldKey)+"_"+index;
					if (newListCountDict.ContainsKey(listCountKey))
					{
						newListCount = newListCountDict[listCountKey];
					}
					else
					{
						newListCount = subList.Count;
						newListCountDict.Add(listCountKey, newListCount);
					}
					
					width = 40;
					newListCount = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), newListCount);
					currentLinePosition += (width + 2);
					
					newListCountDict[listCountKey] = newListCount;
					
					content.text = GDEConstants.ResizeBtn;
					width = GUI.skin.button.CalcSize(content).x;
					if (newListCount != subList.Count)
					{
						if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
							ResizeList(subList, newListCount, defaultResizeValue);
						currentLinePosition += (width + 2);
					}
					NewLine();

					if (isOpen)
					{
						for (int x = 0; x < subList.Count; x++) 
						{
							currentLinePosition += GDEConstants.Indent*3;
							content.text = string.Format("[{0}][{1}]:", index, x);
							
							switch (fieldTypeEnum) 
							{
								case BasicFieldType.Bool:
								{
									DrawListBool(content, x, Convert.ToBoolean(subList[x]), subList);
									NewLine();
									break;
								}
								case BasicFieldType.Int:
								{
									DrawListInt(content, x, Convert.ToInt32(subList[x]), subList);
									NewLine();
									break;
								}
								case BasicFieldType.Float:
								{
									DrawListFloat(content, x, Convert.ToSingle(subList[x]), subList);
									NewLine();
									break;
								}
								case BasicFieldType.String:
								{
									DrawListString(content, x, subList[x] as string, subList);
									NewLine();
									break;
								}
								case BasicFieldType.Vector2:
								{
									DrawListVector2(content, x, subList[x] as Dictionary<string, object>, subList);
									NewLine(GDEConstants.VectorFieldBuffer+1);
									break;
								}
								case BasicFieldType.Vector3:
								{
									DrawListVector3(content, x, subList[x] as Dictionary<string, object>, subList);
									NewLine(GDEConstants.VectorFieldBuffer+1);
									break;
								}
								case BasicFieldType.Vector4:
								{
									DrawListVector4(content, x, subList[x] as Dictionary<string, object>, subList);
									NewLine(GDEConstants.VectorFieldBuffer+1);
									break;
								}
								case BasicFieldType.Color:
								{
									DrawListColor(content, x, subList[x] as Dictionary<string, object>, subList);
									NewLine();
									break;
								}
								default:
								{
									DrawListCustom(content, x, subList[x] as string, subList, false);
									NewLine();
									break;
								}
							}
						}
					}
				}
			}
		}
		catch(Exception ex)
		{
			Debug.LogError(ex);
		}
	}
    #endregion

    #region Filter Methods
    protected override bool ShouldFilter(string schemaKey, Dictionary<string, object> schemaData)
    {
        bool schemaKeyMatch = schemaKey.ToLower().Contains(filterText.ToLower());
        bool fieldKeyMatch = !GDEItemManager.ShouldFilterByField(schemaKey, filterText);
        
        // Return if the schema keys don't contain the filter text or
        // if the schema fields don't contain the filter text
        if (!schemaKeyMatch && !fieldKeyMatch)
            return true;

        return false;
    }

    protected override bool DrawFilterSection()
    {
        bool clearSearch = base.DrawFilterSection();
        
        float width = 200;
        
        int totalItems = GDEItemManager.AllSchemas.Count;
        string itemText = totalItems != 1 ? "items" : "item";
        if (!string.IsNullOrEmpty(filterText))
        {
            string resultText = string.Format("{0} of {1} {2} displayed", NumberOfItemsBeingShown(GDEItemManager.AllSchemas), totalItems, itemText);
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), resultText);
            currentLinePosition += (width + 2);
        }
        
        NewLine();

        return clearSearch;
    }
    #endregion

    #region Add/Remove Field Methods
    private bool AddBasicField(BasicFieldType type, string schemaKey, Dictionary<string, object> schemaData, string newFieldName, bool isList, bool is2DList)
    {
        bool result = true;
        object defaultValue = GDEItemManager.GetDefaultValueForType(type);
        string error;

        if (GDEItemManager.AddBasicFieldToSchema(type, schemaKey, schemaData, newFieldName, out error, isList, is2DList, defaultValue))
            SetNeedToSave(true);
        else
        {
            EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingField, error, GDEConstants.OkLbl);
            result = false;
        }

        return result;
    }

    private bool AddCustomField(string customType, string schemaKey, Dictionary<string, object> schemaData, string newFieldName, bool isList, bool is2DList)
    {
        bool result = true;
        string error;

        if (GDEItemManager.AddCustomFieldToSchema(customType, schemaKey, schemaData, newFieldName, isList, is2DList, out error))        
            SetNeedToSave(true);
        else
        {
            EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingField, error, GDEConstants.OkLbl);
            result = false;
        }

        return result;
    }

    private void RemoveField(string schemaKey, Dictionary<string, object> schemaData, string deletedFieldKey)
    {
        newListCountDict.Remove(string.Format(GDMConstants.MetaDataFormat, schemaKey, deletedFieldKey));
        GDEItemManager.RemoveFieldFromSchema(schemaKey, schemaData, deletedFieldKey);

        SetNeedToSave(true);
    }
    #endregion

    #region Load/Save Schema Methods
    protected override void Load()
    {
        base.Load();

        newSchemaName = "";
        basicFieldTypeSelectedDict.Clear();
        customSchemaTypeSelectedDict.Clear();
        newBasicFieldName.Clear();
        isBasicList.Clear();
        newCustomFieldName.Clear();
        isCustomList.Clear();
        deletedFields.Clear();
        renamedFields.Clear();
        deletedSchemas.Clear();
        renamedSchemas.Clear();
    }

    protected override bool NeedToSave()
    {
        return GDEItemManager.SchemasNeedSave;
    }

    protected override void SetNeedToSave(bool shouldSave)
    {
        GDEItemManager.SchemasNeedSave = true;
    }
    #endregion

    #region Create/Remove Schema Methods
    protected override bool Create(object data)
    {
        bool result = true;
        string key = data as string;
        string error;

        result = GDEItemManager.AddSchema(key, new Dictionary<string, object>(), out error);
        if (result)
        {
            SetNeedToSave(true);
            SetFoldout(true, key);
        }
        else
        {
            EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingSchema, error, GDEConstants.OkLbl);
            result = false;
        }

        return result;
    }

    protected override void Remove(string key)
    {
        // Show a warning if we have items using this schema
        List<string> items = GDEItemManager.GetItemsOfSchemaType(key);
        bool shouldDelete = true;

        if (items!= null && items.Count > 0)
        {
            string itemWord = items.Count == 1 ? "item" : "items";
            shouldDelete = EditorUtility.DisplayDialog(string.Format("{0} {1} will be deleted!", items.Count, itemWord), GDEConstants.SureDeleteSchema, GDEConstants.DeleteSchemaBtn, GDEConstants.CancelBtn);
        }

        if (shouldDelete)
        {
            GDEItemManager.RemoveSchema(key, true);
            SetNeedToSave(true);
        }
    }
    #endregion

    #region Helper Methods
    protected override float CalculateGroupHeightsTotal()
    {
        float totalHeight = 0;
        foreach(KeyValuePair<string, float> pair in groupHeights)
        {
            if (!ShouldFilter(pair.Key, null))
                totalHeight += pair.Value;
        }
        
        return totalHeight;
    }

    protected override string FilePath()
    {
        return GDEItemManager.DataFilePath;
    }
    #endregion

    #region Rename Methods
    protected bool RenameSchema(string oldSchemaKey, string newSchemaKey, Dictionary<string, object> data, out string error)
    {
        error = "";
        renamedSchemas.Add(oldSchemaKey, newSchemaKey);
        return true;
    }

    protected bool RenameSchemaField(string oldFieldKey, string newFieldKey, Dictionary<string, object> schemaData, out string error)
    {
        error = "";
        List<string> fieldKeys = new List<string>(){oldFieldKey, newFieldKey};
        renamedFields.Add(fieldKeys, schemaData);
        return true;
    }
    #endregion
}
