using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using GameDataEditor;

public abstract class GDEManagerWindowBase : EditorWindow {

    protected HashSet<string> entryFoldoutState = new HashSet<string>();
    protected HashSet<string> listFieldFoldoutState = new HashSet<string>();
    protected bool currentFoldoutAllState = false;

    protected Dictionary<string, int> newListCountDict = new Dictionary<string, int>();

    protected string filterText = "";

    protected Vector2 verticalScrollbarPosition;

    protected float currentLine = 0;
    protected float currentLinePosition = GDEConstants.LeftBuffer;

    protected Dictionary<string, float> groupHeights = new Dictionary<string, float>();
    protected float scrollViewHeight = 0;
    protected float scrollViewY = 0;

    protected Dictionary<string, float> groupHeightBySchema = new Dictionary<string, float>();

    protected GUIStyle foldoutStyle = null;
    protected GUIStyle labelStyle = null;
    protected GUIStyle saveButtonStyle = null;
    protected GUIStyle loadButtonStyle = null;
    protected GUIStyle mainHeaderStyle = null;
    protected GUIStyle subHeaderStyle = null;
    protected GUIStyle linkStyle = null;
    protected GUIStyle rateBoxStyle = null;

    protected string saveButtonText = GDEConstants.SaveBtn;

    protected Color headerColor = Color.red;
	protected string headerColorString = "red";
    protected string mainHeaderText = "Oops"; 

    protected string highlightColor;

    protected double lastClickTime = 0;
    protected string lastClickedKey = "";
    protected HashSet<string> editingFields = new HashSet<string>();
    protected Dictionary<string, string> editFieldTextDict = new Dictionary<string, string>();
    protected delegate bool DoRenameDelgate(string oldValue, string newValue, Dictionary<string, object> data, out string errorMsg);
   
    void OnFocus()
    {
        if (GDEItemManager.AllItems.Count.Equals(0) && GDEItemManager.AllSchemas.Count.Equals(0))
            GDEItemManager.Load();
    }

	void SetStyles()
	{
		if (labelStyle == null || labelStyle.name.Equals(string.Empty))
		{
			labelStyle = new GUIStyle(GUI.skin.label);
			SetRichText(labelStyle, true);
		}
		
		if (saveButtonStyle == null || saveButtonStyle.name.Equals(string.Empty))
		{
			saveButtonStyle = new GUIStyle(GUI.skin.button);
			saveButtonStyle.fontSize = 14;
		}
		
		if (loadButtonStyle == null || loadButtonStyle.name.Equals(string.Empty))
		{
			loadButtonStyle = new GUIStyle(GUI.skin.button);
			loadButtonStyle.fontSize = 14;
		}
		
		if (foldoutStyle == null || foldoutStyle.name.Equals(string.Empty))
		{
			foldoutStyle = new GUIStyle(EditorStyles.foldout);
			SetRichText(foldoutStyle, true);
		}
		
		if (mainHeaderStyle == null || mainHeaderStyle.name.Equals(string.Empty))
		{
			mainHeaderStyle = new GUIStyle(GUI.skin.label);
			mainHeaderStyle.fontSize = 20;
			mainHeaderStyle.fontStyle = FontStyle.Bold;
		}
		
		if (subHeaderStyle == null || subHeaderStyle.name.Equals(string.Empty))
		{
			subHeaderStyle = new GUIStyle(GUI.skin.label);
			subHeaderStyle.fontSize = mainHeaderStyle.fontSize - 4;
			subHeaderStyle.fontStyle = FontStyle.Bold;
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
	}
     
    #region OnGUI and DrawHeader Methods
    protected virtual void OnGUI()
    {
		SetStyles();
		
        highlightColor = EditorPrefs.GetString(GDEConstants.HighlightColorKey, GDEConstants.HighlightColor);
		
        ResetToTop();

        // Process page up/down press & home/end press
        if (Event.current.isKey)
        {
            if (Event.current.keyCode == KeyCode.PageDown)
            {
                verticalScrollbarPosition.y += scrollViewHeight/2f;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.PageUp)
            {
                verticalScrollbarPosition.y -= scrollViewHeight/2f;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.Home)
            {
                verticalScrollbarPosition.y = 0;
                Event.current.Use();
            }
            else if (Event.current.keyCode == KeyCode.End)
            {
                verticalScrollbarPosition.y = CalculateGroupHeightsTotal()-GDEConstants.LineHeight-scrollViewY;
                Event.current.Use();
            }
        }

        DrawHeaderLabel();
        DrawHeader();

        DrawCreateSection();
    }

	protected virtual void SetRichText(GUIStyle style, bool richTextValue)
	{
		// If richText is supported, set it
		Type guiStyleType = typeof(GUIStyle);
		if (guiStyleType.GetProperty("richText") != null)
		{
			guiStyleType.InvokeMember("richText", BindingFlags.SetProperty | BindingFlags.Instance, null, style, new object[] {richTextValue});
		}
	}

    protected virtual void DrawHeaderLabel()
    {
        GUIContent labelContent = new GUIContent(mainHeaderText);
		headerColor = headerColorString.ToColor();
		headerColor.a = 1f;
		mainHeaderStyle.normal.textColor = headerColor;
        
        Vector2 contentSize = mainHeaderStyle.CalcSize(labelContent);        
        float headerLabelWidth = contentSize.x;
        float headerLabelHeight = contentSize.y;
        
        currentLinePosition = Math.Max(HorizontalMiddleOfLine()-headerLabelWidth/2f, 0);
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), headerLabelWidth, headerLabelHeight), labelContent, mainHeaderStyle);
        currentLinePosition += (headerLabelWidth + 2);
        
        NewLine(headerLabelHeight/GDEConstants.LineHeight);
    }

    protected virtual void DrawHeader()
    {
        float width = 60;
        float buttonHeightMultiplier = 1.5f;

        if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()*buttonHeightMultiplier), GDEConstants.LoadBtn, loadButtonStyle))
        {
            Load();
            GUI.FocusControl("");
        }
        currentLinePosition += (width + 2);

        GUIContent filePath = new GUIContent(FilePath());
        Vector2 size = labelStyle.CalcSize(filePath);
        EditorGUI.SelectableLabel(new Rect(currentLinePosition, TopOfLine(), WidthLeftOnCurrentLine(), size.y), filePath.text);
        currentLinePosition += (size.x + 2);

        NewLine(buttonHeightMultiplier+.1f);

        if (NeedToSave())
        {
            width = 110;
            saveButtonStyle.normal.textColor = Color.red;
            saveButtonStyle.fontStyle = FontStyle.Bold;
            saveButtonText = GDEConstants.SaveNeededBtn;
        }
        else
        {
            width = 60;
            saveButtonStyle.normal.textColor = GUI.skin.button.normal.textColor;
            saveButtonStyle.fontStyle = FontStyle.Normal;
            saveButtonText = GDEConstants.SaveBtn;
        }

        if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()*buttonHeightMultiplier), saveButtonText, saveButtonStyle))
            Save();

        NewLine(buttonHeightMultiplier);

        DrawSectionSeparator();
        
        if (DrawFilterSection())
            ClearSearch();

        DrawSectionSeparator();
    }

    protected virtual void DrawSubHeader(string text)
    {
        GUIContent subHeaderContent = new GUIContent(text);
		subHeaderStyle.normal.textColor = headerColor;
		
        Vector2 size = subHeaderStyle.CalcSize(subHeaderContent);
        float width = size.x;
        float height = size.y;

        NewLine(0.25f);
        
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, height), subHeaderContent, subHeaderStyle);
        currentLinePosition += (width + 2);
        
        NewLine(height/GDEConstants.LineHeight+0.5f);
    }

    protected virtual void DrawSectionSeparator()
    {
        NewLine(0.25f);
        GUI.Box(new Rect(currentLinePosition, TopOfLine(), FullSeparatorWidth(), 1), string.Empty);
    }

    protected virtual void DrawRateBox(float left, float top, float width, float height)
    {
        GUI.Box(new Rect(left, top, 2f, height), string.Empty, rateBoxStyle);
        GUI.Box(new Rect(left, top, width, 2f), string.Empty, rateBoxStyle);
        GUI.Box(new Rect(left, top+height, width+2, 2f), string.Empty, rateBoxStyle);
        GUI.Box(new Rect(left+width, top, 2f, height), string.Empty, rateBoxStyle);
    }
    #endregion

    #region GUI Position Methods
    protected virtual void ResetToTop()
    {
        currentLine = GDEConstants.TopBuffer/GDEConstants.LineHeight;
        currentLinePosition = GDEConstants.LeftBuffer;
    }

    protected virtual void NewLine(float numNewLines = 1)
    {
        currentLine += numNewLines;
        currentLinePosition = GDEConstants.LeftBuffer;
    }

    protected virtual float TopOfLine()
    {
        return GDEConstants.LineHeight*currentLine;
    }

    protected virtual float VerticalMiddleOfLine()
    {
        return GDEConstants.LineHeight*currentLine + GDEConstants.LineHeight/2;
    }

    protected virtual float HorizontalMiddleOfLine()
    {
        return FullSeparatorWidth()/2f + GDEConstants.LeftBuffer;
    }

    protected virtual float PopupTop()
    {
        return TopOfLine()+1;
    }

    protected virtual float StandardHeight()
    {
        return GDEConstants.LineHeight-2;
    }

    protected virtual float TextBoxHeight()
    {
        return GDEConstants.LineHeight-4;
    }

    protected virtual float VectorFieldHeight()
    {
        return GDEConstants.LineHeight*1.2f;
    }

    protected virtual float FullSeparatorWidth()
    {
        return this.position.width-GDEConstants.LeftBuffer-GDEConstants.RightBuffer;
    }

    protected virtual float WidthLeftOnCurrentLine()
    {
        return FullWindowWidth() - GDEConstants.LeftBuffer - GDEConstants.RightBuffer - currentLinePosition;
    }

    protected virtual float ScrollViewWidth()
    {
        return FullWindowWidth() - 20;
    }

    protected virtual float FullWindowWidth()
    {
        return this.position.width;
    }

    protected virtual float HeightToBottomOfWindow()
    {
        return this.position.height - (currentLine*GDEConstants.LineHeight);
    }

    protected virtual float CurrentHeight()
    {
        return currentLine*GDEConstants.LineHeight;
    }

    protected virtual bool IsVisible(float groupHeight)
    {
        float topSkip = this.verticalScrollbarPosition.y;            
        float bottomThreshold = CurrentHeight() + groupHeight;            
        if (topSkip >= bottomThreshold) {                
            // the group is above our current window                
            return false;                
        }
        
        float bottomSkip = topSkip + scrollViewHeight + scrollViewY;            
        float topThreshold = CurrentHeight() - GDEConstants.LineHeight;
        if (topThreshold >= bottomSkip) {                
            // the group is below our current window
            return false;
        }
        
        return true;
    }

    protected virtual void SetGroupHeight(string forKey, float height)
    {
        if (groupHeights.ContainsKey(forKey))
            groupHeights[forKey] = height;
        else
            groupHeights.Add(forKey, height);
    }
    #endregion

    #region Foldout Methods
    protected virtual void DrawEditableLabel(string editableLabel, string editFieldKey, DoRenameDelgate doRename)
    {
        DrawEditableLabel(editableLabel, editFieldKey, doRename, null);
    }

    protected virtual void DrawEditableLabel(string editableLabel, string editFieldKey, DoRenameDelgate doRename, Dictionary<string, object> data)
    {
        float width;
        
        if (!editingFields.Contains(editFieldKey))
        {
			GUIContent content = new GUIContent(editableLabel.HighlightSubstring(filterText, highlightColor));
			width = Math.Max(labelStyle.CalcSize(content).x, GDEConstants.MinLabelWidth);

            if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), editableLabel.HighlightSubstring(filterText, highlightColor), labelStyle))
            {
                if (EditorApplication.timeSinceStartup - lastClickTime < GDEConstants.DoubleClickTime && lastClickedKey.Equals(editFieldKey))
                {
                    lastClickedKey = "";
                    editingFields.Add(editFieldKey);
                }
                else
                {
                    lastClickedKey = editFieldKey;
                    editingFields.Remove(editFieldKey);
                }
                
                lastClickTime = EditorApplication.timeSinceStartup;
            }
            currentLinePosition += (width + 2);
        }
        else
        {
            string editFieldText;
            if (!editFieldTextDict.TryGetValue(editFieldKey, out editFieldText))
                editFieldText = editableLabel;
            
            string newValue = DrawResizableTextBox(editFieldText);
            editFieldTextDict.TryAddOrUpdateValue(editFieldKey, newValue);
            
            if (!newValue.Equals(editableLabel))
            {
                GUIContent content = new GUIContent(GDEConstants.RenameBtn);
                width = GUI.skin.button.CalcSize(content).x;
                if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content) && doRename != null)
                {
                    string error;
                    if (doRename(editableLabel, newValue, data, out error))
                    {
                        editingFields.Remove(editFieldKey);
                        editFieldTextDict.Remove(editFieldKey);                    
                        GUI.FocusControl("");
                    }
                    else
                        EditorUtility.DisplayDialog(GDEConstants.ErrorLbl, string.Format("Couldn't rename {0} to {1}: {2}", editableLabel, newValue, error), GDEConstants.OkLbl);
                }
                currentLinePosition += (width + 2);

                content.text = GDEConstants.CancelBtn;
                width = GUI.skin.button.CalcSize(content).x;
                if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                {
                    editingFields.Remove(editFieldKey);
                    editFieldTextDict.Remove(editFieldKey);                    
                    GUI.FocusControl("");
                }
                currentLinePosition += (width + 2);
            }
            else
            {
                GUIContent content = new GUIContent(GDEConstants.CancelBtn);
                width = GUI.skin.button.CalcSize(content).x;
                if (GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content))
                {
                    editingFields.Remove(editFieldKey);
                    editFieldTextDict.Remove(editFieldKey);                    
                    GUI.FocusControl("");
                }
                currentLinePosition += (width + 2);
            }
        }
    }

    protected virtual bool DrawFoldout(string foldoutLabel, string key, string editableLabel, string editFieldKey, DoRenameDelgate doRename)
    {
        bool currentFoldoutState = entryFoldoutState.Contains(key);

        GUIContent content = new GUIContent(foldoutLabel);
        float width = foldoutStyle.CalcSize(content).x;
        bool newFoldoutState = EditorGUI.Foldout(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), currentFoldoutState, 
                                                 foldoutLabel.HighlightSubstring(filterText, highlightColor), foldoutStyle);
        currentLinePosition += (width + 2);

		if (doRename != null)
        	DrawEditableLabel(editableLabel, editFieldKey, doRename);
       
        SetFoldout(newFoldoutState, key);
            
        return newFoldoutState;
    }

    protected virtual void DrawExpandCollapseAllFoldout(string[] forKeys, string headerText)
    {
        DrawSubHeader(headerText);

        string label;
        if (currentFoldoutAllState)
            label = GDEConstants.CollapseAllLbl;
        else
            label = GDEConstants.ExpandAllLbl;

        float width = 80;
        bool newFoldAllState = EditorGUI.Foldout(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), currentFoldoutAllState, label);
        if (newFoldAllState != currentFoldoutAllState) 
        {
            SetAllFoldouts(newFoldAllState, forKeys);
            currentFoldoutAllState = newFoldAllState;
            
            // Reset scrollbar if we just collapsed everything
            if (!newFoldAllState)
                verticalScrollbarPosition.y = 0;
        }

        NewLine();
    }

    protected virtual void SetAllFoldouts(bool state, string[] forKeys)
    {
        foreach(string key in forKeys)        
            SetFoldout(state, key);
    }

    protected virtual void SetFoldout(bool state, string forKey)
    {
        if (state)        
            entryFoldoutState.Add(forKey);
        else
        {
            entryFoldoutState.Remove(forKey);

            // Reset the group height to be a single line for the root foldout
            SetGroupHeight(forKey, GDEConstants.LineHeight);
        }
    }
    #endregion

    #region Draw Field Methods
    protected virtual void DrawBool(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            object currentValue;
            bool newValue;
            string key = fieldName;
            
            data.TryGetValue(key, out currentValue);

            GUIContent content = new GUIContent(label);
            float width = labelStyle.CalcSize(content).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
            currentLinePosition += (width + 2);

            width = 50;
            newValue = EditorGUI.Toggle(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), Convert.ToBoolean(currentValue));
            currentLinePosition += (width + 2);

            if (newValue != Convert.ToBoolean(currentValue))
            {
                data[key] = newValue;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawListBool(GUIContent label, int index, bool value, List<object> boolList)
    {
		try
        {
            bool newValue;

            float width = GUI.skin.label.CalcSize(label).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), label);
            currentLinePosition += (width + 2);

            width = 30;
            newValue = EditorGUI.Toggle(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), value);
            currentLinePosition += (width + 2);

            if (value != newValue)
            {
                boolList[index] = newValue;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawInt(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            object currentValue;
            int newValue;
            string key = fieldName;
            
            data.TryGetValue(key, out currentValue);

            GUIContent content = new GUIContent(label);
            float width = labelStyle.CalcSize(content).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
            currentLinePosition += (width + 2);

            width = 50;
            newValue = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), Convert.ToInt32(currentValue));
            currentLinePosition += (width + 2);

            if (newValue != Convert.ToInt32(currentValue))
            {
                data[key] = newValue;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawListInt(GUIContent label, int index, int value, List<object> intList)
    {
        try
        {
            int newValue;

			float width = GUI.skin.label.CalcSize(label).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), label);
            currentLinePosition += (width + 2);

            width = 50;
            newValue = EditorGUI.IntField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), value);
            currentLinePosition += (width + 2);

            if (value != newValue)
            {
                intList[index] = newValue;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    
    protected virtual void DrawFloat(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            object currentValue;
            float newValue;
            string key = fieldName;
            
            data.TryGetValue(key, out currentValue);

            GUIContent content = new GUIContent(label);
            float width = labelStyle.CalcSize(content).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
            currentLinePosition += (width + 2);

            width = 50;
            newValue = EditorGUI.FloatField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), Convert.ToSingle(currentValue));
            currentLinePosition += (width + 2);

            if (newValue != Convert.ToSingle(currentValue))
            {
                data[key] = newValue;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawListFloat(GUIContent label, int index, float value, List<object> floatList)
    {
        try
        {
            float newValue;

            float width = GUI.skin.label.CalcSize(label).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), label);
            currentLinePosition += (width + 2);

            width = 50;
            newValue = EditorGUI.FloatField(new Rect(currentLinePosition, TopOfLine(), width, TextBoxHeight()), value);
            currentLinePosition += (width + 2);

            if (value != newValue)
            {
                floatList[index] = newValue;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    
    protected virtual void DrawString(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            string key = fieldName;
            object currentValue;
            
            data.TryGetValue(key, out currentValue);

            GUIContent content = new GUIContent(label);
            Vector2 size = labelStyle.CalcSize(content);
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), size.x, StandardHeight()), content);
            currentLinePosition += (size.x + 2);

            string newValue = DrawResizableTextBox(currentValue as string);

            if (newValue != (string)currentValue)
            {
                data[key] = newValue;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual string DrawResizableTextBox(string text)
    {
        GUIContent content = new GUIContent(text);
        Vector2 size = GUI.skin.textField.CalcSize(content);
        size.x = Math.Min(Math.Max(size.x, GDEConstants.MinTextAreaWidth), WidthLeftOnCurrentLine() - 62); 
        size.y = Math.Max(size.y, GDEConstants.MinTextAreaHeight);

        string newValue = EditorGUI.TextArea(new Rect(currentLinePosition, TopOfLine(), size.x, size.y), content.text);
        currentLinePosition += (size.x + 2);
        
        float tempLinePosition = currentLinePosition;
        NewLine(size.y/GDEConstants.LineHeight - 1);
        currentLinePosition = tempLinePosition;

        return newValue;
    }

    protected virtual void DrawListString(GUIContent label, int index, string value, List<object> stringList)
    {
        try
        {
            float width = GUI.skin.label.CalcSize(label).x;
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), label);
            currentLinePosition += (width + 2);

            string newValue = DrawResizableTextBox(value);

            if (!value.Equals(newValue))
            {
                stringList[index] = newValue;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawVector2(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            object temp = null;
            Dictionary<string, object> vectDict = null;
            Vector2 currentValue = Vector2.zero;
            Vector2 newValue;
            string key = fieldName;
            
            if (data.TryGetValue(key, out temp))
            {
                vectDict = temp as Dictionary<string, object>;
                currentValue.x = Convert.ToSingle(vectDict["x"]);
                currentValue.y = Convert.ToSingle(vectDict["y"]);
            }

            float width = 136;
            newValue = EditorGUI.Vector2Field(new Rect(currentLinePosition, TopOfLine(), width, VectorFieldHeight()), label, currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                vectDict["x"] = newValue.x;
                vectDict["y"] = newValue.y;
                data[key] = vectDict;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    
    protected virtual void DrawListVector2(GUIContent label, int index, Dictionary<string, object> value, List<object> vectorList)
    {
        try
        {
            Vector2 newValue;
            Vector2 currentValue = Vector2.zero;

            currentValue.x = Convert.ToSingle(value["x"]);
            currentValue.y = Convert.ToSingle(value["y"]);

            float width = 136;
            newValue = EditorGUI.Vector2Field(new Rect(currentLinePosition, TopOfLine(), width, VectorFieldHeight()), label.text, currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                value["x"] = newValue.x;
                value["y"] = newValue.y;
                vectorList[index] = value;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawVector3(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            object temp = null;
            Dictionary<string, object> vectDict = null;
            Vector3 currentValue = Vector3.zero;
            Vector3 newValue;
            string key = fieldName;
            
            if (data.TryGetValue(key, out temp))
            {
                vectDict = temp as Dictionary<string, object>;
                currentValue.x = Convert.ToSingle(vectDict["x"]);
                currentValue.y = Convert.ToSingle(vectDict["y"]);
                currentValue.z = Convert.ToSingle(vectDict["z"]);
            }

            float width = 200;
            newValue = EditorGUI.Vector3Field(new Rect(currentLinePosition, TopOfLine(), width, VectorFieldHeight()), label, currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                vectDict["x"] = newValue.x;
                vectDict["y"] = newValue.y;
                vectDict["z"] = newValue.z;
                data[key] = vectDict;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    
    protected virtual void DrawListVector3(GUIContent label, int index, Dictionary<string, object> value, List<object> vectorList)
    {
        try
        {
            Vector3 newValue;
            Vector3 currentValue = Vector3.zero;
            
            currentValue.x = Convert.ToSingle(value["x"]);
            currentValue.y = Convert.ToSingle(value["y"]);
            currentValue.z = Convert.ToSingle(value["z"]);

            float width = 200;
            newValue = EditorGUI.Vector3Field(new Rect(currentLinePosition, TopOfLine(), width, VectorFieldHeight()), label.text, currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                value["x"] = newValue.x;
                value["y"] = newValue.y;
                value["z"] = newValue.z;
                vectorList[index] = value;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawVector4(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            object temp = null;
            Dictionary<string, object> vectDict = null;
            Vector4 currentValue = Vector4.zero;
            Vector4 newValue;
            string key = fieldName;
            
            if (data.TryGetValue(key, out temp))
            {
                vectDict = temp as Dictionary<string, object>;
                currentValue.x = Convert.ToSingle(vectDict["x"]);
                currentValue.y = Convert.ToSingle(vectDict["y"]);
                currentValue.z = Convert.ToSingle(vectDict["z"]);
                currentValue.w = Convert.ToSingle(vectDict["w"]);
            }

            float width = 228;
            newValue = EditorGUI.Vector4Field(new Rect(currentLinePosition, TopOfLine(), width, VectorFieldHeight()), label, currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                vectDict["x"] = newValue.x;
                vectDict["y"] = newValue.y;
                vectDict["z"] = newValue.z;
                vectDict["w"] = newValue.w;
                data[key] = vectDict;
                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    
    protected virtual void DrawListVector4(GUIContent label, int index, Dictionary<string, object> value, List<object> vectorList)
    {
        try
        {
            Vector4 newValue;
            Vector4 currentValue = Vector4.zero;
            
            currentValue.x = Convert.ToSingle(value["x"]);
            currentValue.y = Convert.ToSingle(value["y"]);
            currentValue.z = Convert.ToSingle(value["z"]);
            currentValue.w = Convert.ToSingle(value["w"]);

            float width = 228;
            newValue = EditorGUI.Vector4Field(new Rect(currentLinePosition, TopOfLine(), width, VectorFieldHeight()), label.text, currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                value["x"] = newValue.x;
                value["y"] = newValue.y;
                value["z"] = newValue.z;
                value["w"] = newValue.w;
                vectorList[index] = value;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawColor(string fieldName, Dictionary<string, object> data, string label)
    {
        try
        {
            Color newValue;
            Color currentValue = Color.white;
            object temp;
			Dictionary<string, object> colorDict = new Dictionary<string, object>();

            if (data.TryGetValue(fieldName, out temp))
            {
                colorDict = temp as Dictionary<string, object>;  
                colorDict.TryGetFloat("r", out currentValue.r);
                colorDict.TryGetFloat("g", out currentValue.g);
                colorDict.TryGetFloat("b", out currentValue.b);
                colorDict.TryGetFloat("a", out currentValue.a);
            }      

            GUIContent content = new GUIContent(label);
            Vector2 size = labelStyle.CalcSize(content);
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), size.x, StandardHeight()), content);
            currentLinePosition += (size.x + 2);

            float width = 230 - size.x;
            newValue = EditorGUI.ColorField(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), currentValue);
            currentLinePosition += (width + 2);

            if (newValue != currentValue)
            {
                colorDict.TryAddOrUpdateValue("r", newValue.r);
                colorDict.TryAddOrUpdateValue("g", newValue.g);
                colorDict.TryAddOrUpdateValue("b", newValue.b);
                colorDict.TryAddOrUpdateValue("a", newValue.a);

                SetNeedToSave(true);
            }
        }
        catch(Exception ex)
        {
            // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
            if (!(ex is ExitGUIException))
                Debug.LogError(ex);
        }

    }

    protected virtual void DrawListColor(GUIContent label, int index, Dictionary<string, object> value, List<object> colorList)
    {
        try
        {
            Color newValue;
            Color currentValue = Color.white;
            
            value.TryGetFloat("r", out currentValue.r);
            value.TryGetFloat("g", out currentValue.g);
            value.TryGetFloat("b", out currentValue.b);
            value.TryGetFloat("a", out currentValue.a);
            
            Vector2 size = labelStyle.CalcSize(label);
            GUI.Label(new Rect(currentLinePosition, TopOfLine(), size.x, StandardHeight()), label);
            currentLinePosition += (size.x + 2);
            
            float width = 230 - size.x;
            newValue = EditorGUI.ColorField(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), currentValue);
            currentLinePosition += (width + 2);
            
            if (newValue != currentValue)
            {
                value.TryAddOrUpdateValue("r", newValue.r);
                value.TryAddOrUpdateValue("g", newValue.g);
                value.TryAddOrUpdateValue("b", newValue.b);
                value.TryAddOrUpdateValue("a", newValue.a);

                colorList[index] = value;
                SetNeedToSave(true);
            }
        } 
        catch (Exception ex)
        {
            // Don't log ExitGUIException here. This is a unity bug with ObjectField and ColorField.
            if (!(ex is ExitGUIException))
                Debug.LogError(ex);
        }
    }

    protected virtual void DrawCustom(string fieldName, Dictionary<string, object> data, bool canEdit, List<string> possibleValues = null)
    {
        try
        {
            object currentValue;
            int newIndex;
            int currentIndex;
            string key = fieldName;

            data.TryGetValue(key, out currentValue);

            float width;
            if (canEdit && possibleValues != null)
            {
                currentIndex = possibleValues.IndexOf(currentValue as string);

                GUIContent content = new GUIContent(GDEConstants.ValueLbl);
                width = labelStyle.CalcSize(content).x;
                GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
                currentLinePosition += (width + 2);

                width = 80;
                newIndex = EditorGUI.Popup(new Rect(currentLinePosition, PopupTop(), width, StandardHeight()), currentIndex, possibleValues.ToArray());
                currentLinePosition += (width + 2);

                if (newIndex != currentIndex)   
                {
                    data[key] = possibleValues[newIndex];                    
                    SetNeedToSave(true);
                }
            }
            else
            {
                GUIContent content = new GUIContent(GDEConstants.DefaultValueLbl + " null");
                width = labelStyle.CalcSize(content).x;
                GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
                currentLinePosition += (width + 4);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    protected virtual void DrawListCustom(GUIContent label, int index, string value, List<object> customList,  bool canEdit, List<string> possibleValues = null)
    {
        try
        {
            int newIndex;
            int currentIndex;
            float width;

            if (canEdit && possibleValues != null)
            {
                currentIndex = possibleValues.IndexOf(value);

                width = GUI.skin.label.CalcSize(label).x;
                GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), label);
                currentLinePosition += (width + 2);

                width = 80;
                newIndex = EditorGUI.Popup(new Rect(currentLinePosition, PopupTop(), width, StandardHeight()), currentIndex, possibleValues.ToArray());
                currentLinePosition += (width + 2);

                if (newIndex != currentIndex)     
                {
                    customList[index] = possibleValues[newIndex];
                    SetNeedToSave(true);
                }
            }
            else
            {
                label.text += " null";
                width = labelStyle.CalcSize(label).x;
                GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), label);
                currentLinePosition += (width + 2);
            }
        }
        catch(Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    #endregion

    #region Filter/Sorting Methods
    protected virtual bool DrawFilterSection()
    {
        GUIContent content = new GUIContent(GDEConstants.SearchHeader);
        Vector2 size = subHeaderStyle.CalcSize(content);
        float width = size.x;
        float height = size.y;
        
        NewLine(0.25f);
        
        GUI.Label(new Rect(currentLinePosition, TopOfLine(), width, height), content, subHeaderStyle);
        currentLinePosition += (width + 8);

        // Text search
        width = 180;
        filterText = EditorGUI.TextField(new Rect(currentLinePosition, TopOfLine()+(height-GDEConstants.LineHeight+2), width, TextBoxHeight()), filterText);
        currentLinePosition += (width + 2);

        // Clear search Button
        content.text = GDEConstants.ClearSearchBtn;
        size = GUI.skin.button.CalcSize(content);
        width = size.x;
        height = size.y;

        bool clearSearch = GUI.Button(new Rect(currentLinePosition, TopOfLine(), width, StandardHeight()), content);
        currentLinePosition += (width + 2);

        return clearSearch;
    }

    protected virtual int NumberOfItemsBeingShown(Dictionary<string, Dictionary<string, object>> data)
    {
        int resultCount = 0;
        
        foreach(KeyValuePair<string, Dictionary<string, object>> pair in data)
        {
            if (!ShouldFilter(pair.Key, pair.Value))
                resultCount++;
        }
        
        return resultCount;
    }

    protected virtual void ClearSearch()
    {
        filterText = "";
        GUI.FocusControl("");
    }
    #endregion

    #region List Helper Methods
    protected virtual void ResizeList(IList list, int size, object defaultValue)
    {
        // Remove from the end until the size matches what we want
        if (list.Count > size)
        {
			while(list.Count > size)
				list.RemoveAt(list.Count-1);
            
			SetNeedToSave(true);
        }
        else if (list.Count < size)
        {
            // Add entries with the default value until the size is what we want
            for (int i = list.Count; i < size; i++) 
            {
                if (defaultValue != null && defaultValue.GetType().Equals(typeof(Dictionary<string, object>)))
                    list.Add(new Dictionary<string, object>((defaultValue as Dictionary<string, object>).DeepCopy()));
				else if (defaultValue != null && defaultValue.GetType().Equals(typeof(List<object>)))
					list.Add(new List<object>((defaultValue as List<object>).DeepCopy()));
                else
                    list.Add(defaultValue);
            }
            SetNeedToSave(true);
        }
    }
    #endregion

    #region Save/Load methods
    protected virtual void Load()
    {
        GDEItemManager.Load();

        entryFoldoutState.Clear();
        listFieldFoldoutState.Clear();
        currentFoldoutAllState = false;
        newListCountDict.Clear();
        filterText = "";
        groupHeights.Clear();
        groupHeightBySchema.Clear();
        editingFields.Clear();
        editFieldTextDict.Clear();
    }

    protected virtual void Save()
    {
        GDEItemManager.Save();
    }
    #endregion
    
    #region Abstract methods
    protected abstract bool Create(object data);
    protected abstract void Remove(string key);

    protected abstract void DrawEntry(string key, Dictionary<string, object> data);
    protected abstract void DrawCreateSection();

    protected abstract bool ShouldFilter(string key, Dictionary<string, object> data);

    protected abstract bool NeedToSave();
    protected abstract void SetNeedToSave(bool shouldSave);

    protected abstract string FilePath();

    protected abstract float CalculateGroupHeightsTotal();
    #endregion
}
