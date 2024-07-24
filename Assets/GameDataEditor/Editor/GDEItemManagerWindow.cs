using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDataEditor;

public class GDEItemManagerWindow : GDEManagerWindowBase
{
    private string newItemName = "";
    private int schemaIndex = 0;

    private int filterSchemaIndex = 0;

    private List<string> deletedItems = new List<string>();
    private Dictionary<string, string> renamedItems = new Dictionary<string, string>();

    #region OnGUI Method
    protected override void OnGUI()
    {
        mainHeaderText = GDEConstants.GameDataHeader;

		if (EditorGUIUtility.isProSkin)
			headerColorString = EditorPrefs.GetString(GDEConstants.CreateDataColorKey, GDEConstants.CreateDataColorPro);
		else
			headerColorString = EditorPrefs.GetString(GDEConstants.CreateDataColorKey, GDEConstants.CreateDataColor);

		base.OnGUI();

        DrawExpandCollapseAllFoldout(GDEItemManager.AllItems.Keys.ToArray(), GDEConstants.ItemListHeader);


        float currentGroupHeightTotal = CalculateGroupHeightsTotal();
        scrollViewHeight = HeightToBottomOfWindow();
        scrollViewY = TopOfLine();
        verticalScrollbarPosition = GUI.BeginScrollView(new Rect(currentLinePosition, scrollViewY, FullWindowWidth(), scrollViewHeight), 
                                                        verticalScrollbarPosition,
                                                        new Rect(currentLinePosition, scrollViewY, ScrollViewWidth(), currentGroupHeightTotal));

        int count = 0;
        foreach (KeyValuePair<string, Dictionary<string, object>> item in GDEItemManager.AllItems)
        {
            // If we are filtered out, continue
            if (ShouldFilter(item.Key, item.Value))
                continue;

            float currentGroupHeight;
            groupHeights.TryGetValue(item.Key, out currentGroupHeight);

            if (currentGroupHeight == 0f || 
                (currentGroupHeight.NearlyEqual(GDEConstants.LineHeight) && entryFoldoutState.Contains(item.Key)))
            {
                string itemSchema = GDEItemManager.GetSchemaForItem(item.Key);
                if (!groupHeightBySchema.TryGetValue(itemSchema, out currentGroupHeight))
                    currentGroupHeight = GDEConstants.LineHeight;
            }

            if (IsVisible(currentGroupHeight) || 
                (count == GDEItemManager.AllItems.Count-1 && verticalScrollbarPosition.y.NearlyEqual(currentGroupHeightTotal - GDEConstants.LineHeight)))
            {
                DrawEntry(item.Key, item.Value);
            }
            else
            {
                NewLine(currentGroupHeight/GDEConstants.LineHeight);
            }

            count++;
        }
        GUI.EndScrollView();
        
        //Remove any items that were deleted
        foreach(string deletedkey in deletedItems)        
            Remove(deletedkey);
        deletedItems.Clear();

        //Rename any items that were renamed
        string error;
        foreach(KeyValuePair<string, string> pair in renamedItems)
        {
            if (!GDEItemManager.RenameItem(pair.Key, pair.Value, null, out error))
                EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, string.Format("Couldn't rename {0} to {1}: {2}", pair.Key, pair.Value, error), GDEConstants.OkLbl);
        }

        renamedItems.Clear();
    }
    #endregion

    #region Draw Methods
    protected override void DrawCreateSection()
    {
        float topOfSection = TopOfLine() + 4f;
        float bottomOfSection = 0;
        float leftBoundary = currentLinePosition;
        float width = 0;

        DrawSubHeader(GDEConstants.CreateNewItemHeader);

        width = 60;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.SchemaLbl);
        currentLinePosition += (width + 2);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;
        
        width = 100;
        schemaIndex = EditorGUI.Popup(new Rect(currentLinePosition, PopupTop(), width, StandardHeight()), schemaIndex, GDEItemManager.SchemaKeyArray);
        currentLinePosition += (width + 6);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;
        
        width = 65;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), GDEConstants.ItemNameLbl);
        currentLinePosition += (width + 2);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;
        
        width = 180;
        newItemName = EditorGUI.TextField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), newItemName);
        currentLinePosition += (width + 2);
        if (currentLinePosition > leftBoundary)
            leftBoundary = currentLinePosition;

        GUIContent content = new GUIContent(GDEConstants.CreateNewItemBtn);
        width = GUI.skin.button.CalcSize(content).x;
        if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
        {
            if (GDEItemManager.SchemaKeyArray.IsValidIndex(schemaIndex))
            {
                List<object> args = new List<object>();
                args.Add(GDEItemManager.SchemaKeyArray[schemaIndex]);
                args.Add(newItemName);
                
                if (Create(args))
                {            
                    newItemName = "";
                    GUI.FocusControl("");
                }
            }
            else
                EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingItem, GDEConstants.NoOrInvalidSchema, GDEConstants.OkLbl);
        }
        currentLinePosition += (width + 6);
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

    protected override bool DrawFilterSection()
    {
        bool clearSearch = base.DrawFilterSection();

        float width = 200;

        int totalItems = GDEItemManager.AllItems.Count;
        string itemText = totalItems != 1 ? "items" : "item";
        if (!string.IsNullOrEmpty(filterText) || 
            (GDEItemManager.FilterSchemaKeyArray.IsValidIndex(filterSchemaIndex) && !GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex].Equals("_All")))
        {
            string resultText = string.Format("{0} of {1} {2} displayed", NumberOfItemsBeingShown(GDEItemManager.AllItems), totalItems, itemText);
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), resultText);
            currentLinePosition += (width + 2);
        }

        NewLine(1.25f);
        
        // Filter dropdown
        GUIContent content = new GUIContent(GDEConstants.FilterBySchemaLbl);
        width = labelStyle.CalcSize(content).x;
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content.text);
        currentLinePosition += (width + 8);

        width = 100;
        filterSchemaIndex = EditorGUI.Popup(new Rect(currentLinePosition, PopupTop(), width, StandardHeight()), filterSchemaIndex, GDEItemManager.FilterSchemaKeyArray);

        NewLine();

        return clearSearch;
    }

    protected override void DrawEntry(string key, Dictionary<string, object> data)
    {
        float beginningHeight = CurrentHeight();
        string schemaType = "<unknown>";
        object temp;
        
        if (data.TryGetValue(GDMConstants.SchemaKey, out temp))
            schemaType = temp as string;

        // Start drawing below
		bool isOpen = DrawFoldout(schemaType+":", key, key, key, RenameItem);
		NewLine();

        if (isOpen)
        {
            bool shouldDrawSpace = false;
            bool didDrawSpaceForSection = false;
			bool isFirstSection = true;

            // Draw the basic types
            foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
            {
                List<string> fieldKeys = GDEItemManager.ItemFieldKeysOfType(key, fieldType.ToString(), 0);                
                foreach(string fieldKey in fieldKeys)
                {
                    currentLinePosition += GDEConstants.Indent;
                    DrawSingleField(schemaType, fieldKey, data);
                    shouldDrawSpace = true;
					isFirstSection = false;
                }
            }
            
            // Draw the custom types
            foreach(string fieldKey in GDEItemManager.ItemCustomFieldKeys(key, 0))
            {
                if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
                {
                    NewLine(0.5f);
                    didDrawSpaceForSection = true;
                }
                
                currentLinePosition += GDEConstants.Indent;
                DrawSingleField(schemaType, fieldKey, data);
                shouldDrawSpace = true;
				isFirstSection = false;
            }
            didDrawSpaceForSection = false;
            
			// Draw the basic lists
			for(int dimension=1;  dimension <= 2;  dimension++)
			{
				foreach(BasicFieldType fieldType in GDEItemManager.BasicFieldTypes)
				{
					List<string> fieldKeys = GDEItemManager.ItemFieldKeysOfType(key, fieldType.ToString(), dimension);                
					foreach(string fieldKey in fieldKeys)
					{
						if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
						{
							NewLine(0.5f);
							didDrawSpaceForSection = true;
						}
						
						currentLinePosition += GDEConstants.Indent;

						if (dimension == 1)
							DrawListField(schemaType, key, fieldKey, data);
						else
							Draw2DListField(schemaType, key, fieldKey, data);

						shouldDrawSpace = true;
						isFirstSection = false;
						didDrawSpaceForSection = true;
					}
				}
				didDrawSpaceForSection = false;
			}
            
            // Draw the custom lists
			for(int dimension=1;  dimension <= 2;  dimension++)
			{
	            foreach(string fieldKey in GDEItemManager.ItemCustomFieldKeys(key, dimension))
	            {
					if (shouldDrawSpace && !didDrawSpaceForSection && !isFirstSection)
	                {
	                    NewLine(0.5f);
	                    didDrawSpaceForSection = true;
	                }

	                currentLinePosition += GDEConstants.Indent;
					if (dimension == 1)
						DrawListField(schemaType, key, fieldKey, data);
					else
						Draw2DListField(schemaType, key, fieldKey, data);

	                shouldDrawSpace = true;
					isFirstSection = false;
					didDrawSpaceForSection = true;
	            }
				didDrawSpaceForSection = false;
			}

            NewLine(0.5f);

            GUIContent content = new GUIContent(GDEConstants.DeleteBtn);
            float width = GUI.skin.button.CalcSize(content).x;
            if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                deletedItems.Add(key);

            NewLine();

            DrawSectionSeparator();

            NewLine(0.25f);
        }
        else
        {
            // Collapse any list foldouts as well
            List<string> listKeys = GDEItemManager.ItemListFieldKeys(key);
            string foldoutKey;
            foreach(string listKey in listKeys)
            {
                foldoutKey = string.Format(GDMConstants.MetaDataFormat, key, listKey);
                listFieldFoldoutState.Remove(foldoutKey);
            }
        }

        float newGroupHeight = CurrentHeight() - beginningHeight;
        float currentGroupHeight;
        groupHeights.TryGetValue(key, out currentGroupHeight);

        // Set the minimum height for the schema type
        if (currentGroupHeight.NearlyEqual(GDEConstants.LineHeight) && !newGroupHeight.NearlyEqual(GDEConstants.LineHeight))
            SetSchemaHeight(schemaType, newGroupHeight);

        SetGroupHeight(key, newGroupHeight);
    }

    void DrawSingleField(string schemaKey, string fieldKey, Dictionary<string, object> itemData)
    {
        string fieldType;
        itemData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out fieldType);

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

		content.text = fieldKey.HighlightSubstring(filterText, highlightColor);
		width = Math.Max(labelStyle.CalcSize(content).x, GDEConstants.MinLabelWidth);

        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content, labelStyle);
        currentLinePosition += (width + 2);

        switch(fieldTypeEnum)
        {
            case BasicFieldType.Bool:
            {
                DrawBool(fieldKey, itemData, GDEConstants.ValueLbl);
                NewLine();
                break;
            }
            case BasicFieldType.Int:
            {
                DrawInt(fieldKey, itemData, GDEConstants.ValueLbl);
                NewLine();
                break;
            }
            case BasicFieldType.Float:
            {
                DrawFloat(fieldKey, itemData, GDEConstants.ValueLbl);
                NewLine();
                break;
            }
            case BasicFieldType.String:
            {
                DrawString(fieldKey, itemData, GDEConstants.ValueLbl);
                NewLine();
                break;
            }
            case BasicFieldType.Vector2:
            {
                DrawVector2(fieldKey, itemData, GDEConstants.ValuesLbl);
                NewLine(GDEConstants.VectorFieldBuffer+1);
                break;
            }
            case BasicFieldType.Vector3:
            {
                DrawVector3(fieldKey, itemData, GDEConstants.ValuesLbl);
                NewLine(GDEConstants.VectorFieldBuffer+1);
                break;
            }
            case BasicFieldType.Vector4:
            {
                DrawVector4(fieldKey, itemData, GDEConstants.ValuesLbl);
                NewLine(GDEConstants.VectorFieldBuffer+1);
                break;
            }
            case BasicFieldType.Color:
            {
                DrawColor(fieldKey, itemData, GDEConstants.ValuesLbl);
                NewLine();
                break;
            }
                
            default:
            {
                List<string> itemKeys = GetPossibleCustomValues(schemaKey, fieldType);
                DrawCustom(fieldKey, itemData, true, itemKeys);
                NewLine();
                break;
            }
        }
    }

    void DrawListField(string schemaKey, string itemKey, string fieldKey, Dictionary<string, object> itemData)
    {
        try
        {
            string foldoutKey = string.Format(GDMConstants.MetaDataFormat, itemKey, fieldKey);
            bool newFoldoutState;
            bool currentFoldoutState = listFieldFoldoutState.Contains(foldoutKey);
            object defaultResizeValue = null;
            
            string fieldType;
            itemData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out fieldType);

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

			content.text = fieldKey.HighlightSubstring(filterText, highlightColor);
			width = Math.Max(labelStyle.CalcSize(content).x, GDEConstants.MinLabelWidth);
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), fieldKey.HighlightSubstring(filterText, highlightColor), labelStyle);
            currentLinePosition += (width + 2);

            if (newFoldoutState != currentFoldoutState)
            {
                if (newFoldoutState)
                    listFieldFoldoutState.Add(foldoutKey);
                else
                    listFieldFoldoutState.Remove(foldoutKey);
            }
            
            object temp = null;
            List<object> list = null;
            
            if (itemData.TryGetValue(fieldKey, out temp))
                list = temp as List<object>;

            content.text = GDEConstants.SizeLbl;
            width = GUI.skin.label.CalcSize(content).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
            currentLinePosition += (width + 2);

            int newListCount;
            string listCountKey = string.Format(GDMConstants.MetaDataFormat, itemKey, fieldKey);
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
            newListCount = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), newListCount);
            currentLinePosition += (width + 4);

            content.text = GDEConstants.ResizeBtn;
            width = GUI.skin.button.CalcSize(content).x;
            newListCountDict[listCountKey] = newListCount;
            if (newListCount != list.Count && GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))            
            {
                ResizeList(list, newListCount, defaultResizeValue);
                newListCountDict[listCountKey] = newListCount;
                currentLinePosition += (width + 2);
            }

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
                            List<string> itemKeys = GetPossibleCustomValues(schemaKey, fieldType);
							DrawListCustom(content, i, list[i] as string, list, true, itemKeys);
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

	void Draw2DListField(string schemaKey, string itemKey, string fieldKey, Dictionary<string, object> itemData)
	{
		try
		{
			string foldoutKey = string.Format(GDMConstants.MetaDataFormat, itemKey, fieldKey);
			object defaultResizeValue = new List<object>();
			
			string fieldType;
			itemData.TryGetString(string.Format(GDMConstants.MetaDataFormat, GDMConstants.TypePrefix, fieldKey), out fieldType);
			
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
			bool isOpen = DrawFoldout(content.text, foldoutKey, string.Empty, string.Empty, null);

			currentLinePosition = Math.Max(currentLinePosition, GDEConstants.MinLabelWidth+GDEConstants.Indent+4);
			content.text = fieldKey.HighlightSubstring(filterText, highlightColor);
			float width = Math.Max(labelStyle.CalcSize(content).x, GDEConstants.MinLabelWidth);
			GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), fieldKey.HighlightSubstring(filterText, highlightColor), labelStyle);
			currentLinePosition += (width + 2);
			
			object temp = null;
			List<object> list = null;
			
			if (itemData.TryGetValue(fieldKey, out temp))
				list = temp as List<object>;
			
			content.text = GDEConstants.SizeLbl;
			width = GUI.skin.label.CalcSize(content).x;
			GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
			currentLinePosition += (width + 2);
			
			int newListCount;
			string listCountKey = string.Format(GDMConstants.MetaDataFormat, itemKey, fieldKey);
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
			newListCount = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), newListCount);
			currentLinePosition += (width + 4);
			
			content.text = GDEConstants.ResizeBtn;
			width = GUI.skin.button.CalcSize(content).x;
			newListCountDict[listCountKey] = newListCount;
			if (newListCount != list.Count && GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))            
			{
				ResizeList(list, newListCount, defaultResizeValue);
				newListCountDict[listCountKey] = newListCount;
				currentLinePosition += (width + 2);
			}
			
			NewLine();
			
			if (isOpen)
			{
				defaultResizeValue = GDEItemManager.GetDefaultValueForType(fieldTypeEnum);
				for (int index = 0; index < list.Count; index++) 
				{
					List<object> subList = list[index] as List<object>;
					
					currentLinePosition += GDEConstants.Indent*2;
					content.text = string.Format("[{0}]:    List<{1}>", index, fieldType);
					
					isOpen = DrawFoldout(content.text, foldoutKey+"_"+index, "", "", null);
					currentLinePosition += 4;

					// Draw resize
					content.text = GDEConstants.SizeLbl;
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
									List<string> itemKeys = GetPossibleCustomValues(schemaKey, fieldType);
									DrawListCustom(content, x, subList[x] as string, subList, true, itemKeys);
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

    List<string> GetPossibleCustomValues(string fieldKey, string fieldType)
    {
        object temp;
        List<string> itemKeys = new List<string>();
        itemKeys.Add("null");
        
        // Build a list of possible custom field values
        // All items that match the schema type of the custom field type
        // will be added to the selection list
        foreach(KeyValuePair<string, Dictionary<string, object>> item in GDEItemManager.AllItems)
        {
            string itemType = "<unknown>";
            Dictionary<string, object> itemData = item.Value as Dictionary<string, object>;
            
            if (itemData.TryGetValue(GDMConstants.SchemaKey, out temp))
                itemType = temp as string;
            
            if (item.Key.Equals(fieldKey) || !itemType.Equals(fieldType))
                continue;
            
            itemKeys.Add(item.Key);
        }

        return itemKeys;
    }
    #endregion

    #region Filter Methods
    protected override bool ShouldFilter(string itemKey, Dictionary<string, object> itemData)
    {
        if (itemData == null)
            return true;

        string schemaType = "<unknown>";
        object temp;
        
        if (itemData.TryGetValue(GDMConstants.SchemaKey, out temp))
            schemaType = temp as string;
        
        // Return if we don't match any of the filter types
        if (GDEItemManager.FilterSchemaKeyArray.IsValidIndex(filterSchemaIndex) &&
            !GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex].Equals("_All") &&
            !schemaType.Equals(GDEItemManager.FilterSchemaKeyArray[filterSchemaIndex]))
            return true;
        
        bool schemaKeyMatch = schemaType.ToLower().Contains(filterText.ToLower());
        bool fieldKeyMatch = !GDEItemManager.ShouldFilterByField(schemaType, filterText);
        bool itemKeyMatch = itemKey.ToLower().Contains(filterText.ToLower());
        
        // Return if the schema keys don't contain the filter text or
        // if the schema fields don't contain the filter text
        if (!schemaKeyMatch && !fieldKeyMatch && !itemKeyMatch)
            return true;

        return false;
    }

    protected override void ClearSearch()
    {
        base.ClearSearch();
        filterSchemaIndex = GDEItemManager.FilterSchemaKeyArray.ToList().IndexOf("_All");
    }
    #endregion

    #region Load/Save/Create/Remove Item Methods
    protected override void Load()
    {
        base.Load();

        newItemName = "";
        schemaIndex = 0;
        filterSchemaIndex = 0;
        deletedItems.Clear();
        renamedItems.Clear();
    }

    protected override bool Create(object data)
    {
        bool result = true;
        List<object> args = data as List<object>;
        string schemaKey = args[0] as string;
        string itemName = args[1] as string;

        Dictionary<string, object> schemaData = null;       
        if (GDEItemManager.AllSchemas.TryGetValue(schemaKey, out schemaData))
        {
            Dictionary<string, object> itemData = schemaData.DeepCopy();
            itemData.Add(GDMConstants.SchemaKey, schemaKey);

            string error;
            if (GDEItemManager.AddItem(itemName, itemData, out error))
            {
                SetFoldout(true, itemName);
                SetNeedToSave(true);
            }
            else
            {
                result = false;
                EditorUtility.DisplayDialog(GDEConstants.ErrorCreatingItem, error, GDEConstants.OkLbl);
            }
        }
        else
        {
            result = false;
            EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, GDEConstants.SchemaNotFound + ": " + schemaKey, GDEConstants.OkLbl);
        }

        return result;
    }

    protected override void Remove(string key)
    {
        GDEItemManager.RemoveItem(key);
        SetNeedToSave(true);
    }

    protected override bool NeedToSave()
    {
        return GDEItemManager.ItemsNeedSave;
    }

    protected override void SetNeedToSave(bool shouldSave)
    {
        GDEItemManager.ItemsNeedSave = shouldSave;
    }
    #endregion

    #region Helper Methods
    void SetSchemaHeight(string schemaKey, float groupHeight)
    {
        if (groupHeightBySchema.ContainsKey(schemaKey))
            groupHeightBySchema[schemaKey] = groupHeight;
        else
            groupHeightBySchema.Add(schemaKey, groupHeight);
    }

    protected override float CalculateGroupHeightsTotal()
    {
        float totalHeight = 0;
        float schemaHeight = 0;
        string schema = "";
        
        foreach(KeyValuePair<string, float> pair in groupHeights)
        {
            Dictionary<string, object> itemData;
            GDEItemManager.AllItems.TryGetValue(pair.Key, out itemData);
            if (ShouldFilter(pair.Key, itemData))
                continue;

            //Check to see if this item's height has been updated
            //otherwise use the min height for the schema
            if (entryFoldoutState.Contains(pair.Key) && pair.Value.NearlyEqual(GDEConstants.LineHeight))
            {
                schema = GDEItemManager.GetSchemaForItem(pair.Key);
                groupHeightBySchema.TryGetValue(schema, out schemaHeight);
                totalHeight += schemaHeight;
            }
            else
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
    protected bool RenameItem(string oldItemKey, string newItemKey, Dictionary<string, object> data, out string error)
    {
        error = "";
        renamedItems.Add(oldItemKey, newItemKey);
        return true;
    }
    #endregion
}
