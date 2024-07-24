using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameDataEditor;


public class ReadDataScene : MonoBehaviour 
{
    public GUIText BoolField;
    public GUIText BoolListField;
    public GUIText IntField;
    public GUIText IntListField;
    public GUIText FloatField;
    public GUIText FloatListField;
    public GUIText StringField;
    public GUIText StringListField;
    public GUIText Vector2Field;
    public GUIText Vector2LisField;
    public GUIText Vector3Field;
    public GUIText Vector3ListField;
    public GUIText Vector4Field;
    public GUIText Vector4ListField;
    public GUIText ColorField;
    public GUIText ColorListField;
    public GUIText CustomField;
    public GUIText CustomListField;

    public GUIText GetAllDataBySchema;
    public GUIText GetAllKeysBySchema;

    public GUIText Status;

	GDEReadSceneData data;

	void Start () 
    {
		// Initialize GDE
        if (!GDEDataManager.Init("read_scene_data")) {
			Status.text = "Status: Something went wrong. GDEDataManager was not initialized!";
			return;
		}

		// Load the data for the scene into the GDEReadSceneData object
		if (!GDEDataManager.DataDictionary.TryGetCustom(GDEReadSceneItemKeys.ReadScene_test_data, out data)) {
			Status.text = "Status: Something went wrong. Couldn't load Read Scene data!";
			return;
		}

		try
	    {
	        // Bool
	        BoolField.text = "Bool Field: ";
	        BoolField.text += data.bool_field;

	        // Bool List
	        BoolListField.text = "Bool List Field: ";
	        foreach(bool boolVal in data.bool_list_field)
	            BoolListField.text += string.Format("{0} ", boolVal);

	        // Int
	        IntField.text = "Int Field: ";
	        IntField.text += data.int_field;

	        // Int List
	        IntListField.text = "Int List Field: ";
	        foreach(int intVal in data.int_list_field)
	            IntListField.text += string.Format("{0} ", intVal);

	        // Float
	        FloatField.text = "Float Field: ";
	        FloatField.text += data.float_field;

	        // Float List
	        FloatListField.text = "Float List Field: ";
	        foreach(float floatVal in data.float_list_field)
	            FloatListField.text += string.Format("{0} ", floatVal);

	        // String
	        StringField.text = "String Field: ";
	        StringField.text += data.string_field;

	        // String List
	        StringListField.text = "String List Field: ";
	        foreach(string stringVal in data.string_list_field)
	            StringListField.text += string.Format("{0} ", stringVal);

	        // Vector2
	        Vector2Field.text = "Vector2 Field: ";
	        Vector2Field.text += string.Format("({0}, {1})", data.vector2_field.x, data.vector2_field.y);

	        // Vector2 List
	        Vector2LisField.text = "Vector2 List Field: ";
	        foreach(Vector2 vec2Val in data.vector2_list_field)
	            Vector2LisField.text += string.Format("({0}, {1}) ", vec2Val.x, vec2Val.y);

	        // Vector3
	        Vector3Field.text = "Vector3 Field: ";
			Vector3Field.text += string.Format("({0}, {1}, {2})", data.vector3_field.x, data.vector3_field.y, data.vector3_field.z);

	        // Vector3 List
	        Vector3ListField.text = "Vector3 List Field: ";
	        foreach(Vector3 vec3Val in data.vector3_list_field)
	            Vector3ListField.text += string.Format("({0}, {1}, {2}) ", vec3Val.x, vec3Val.y, vec3Val.z);

	        // Vector4
	        Vector4Field.text = "Vector4 Field: ";
			Vector4Field.text += string.Format("({0}, {1}, {2}, {3})", data.vector4_field.x, data.vector4_field.y, data.vector4_field.z, data.vector4_field.w);

	        // Vector4 List
	        Vector4ListField.text = "Vector4 List Field: ";
	        foreach(Vector4 vec4Val in data.vector4_list_field)
	            Vector4ListField.text += string.Format("({0}, {1}, {2}, {3}) ", vec4Val.x, vec4Val.y, vec4Val.z, vec4Val.w); 

	        // Color
	        ColorField.text = "Color Field: ";
			ColorField.text += data.color_field.ToString();

	        // Color List
	        ColorListField.text = "Color List Field: ";
	        foreach(Color colVal in data.color_list_field)
	            ColorListField.text += string.Format("{0}   ", colVal);

	        // Custom
	        CustomField.text = "Custom Field: ";
	        CustomField.text += data.custom_field.description;

	        // Custom List
	        CustomListField.text = "Custom List Field:" + Environment.NewLine;
	        foreach(var customField in data.custom_list_field)
	            CustomListField.text += string.Format("     {0}{1}", customField.description, Environment.NewLine);


			/**
			 * 
			 * The following two methods (GetAllDataBySchema and GetAllKeysBySchema) are part of the old version of the GDE API.
			 * They still work, but require a little more code to use.
			 * 
			 **/

	        // Get All Data By Schema
	        GetAllDataBySchema.text = "Get All Data By Schema:" + Environment.NewLine;
	        Dictionary<string, object> allDataByCustomSchema;
	        GDEDataManager.GetAllDataBySchema("ReadSceneCustom", out allDataByCustomSchema);
	        foreach(KeyValuePair<string, object> pair in allDataByCustomSchema)
	        {
	            string description;
	            Dictionary<string, object> customData = pair.Value as Dictionary<string, object>;
	            customData.TryGetString("description", out description);
	            GetAllDataBySchema.text += string.Format("     {0}{1}", description, Environment.NewLine);
	        }

	        // Get All Keys By Schema
	        GetAllKeysBySchema.text = "Get All Keys By Schema: ";
	        List<string> customKeys;
	        GDEDataManager.GetAllDataKeysBySchema("ReadSceneCustom", out customKeys);
	        foreach(string key in customKeys)
	            GetAllKeysBySchema.text += string.Format("{0} ", key);

	        Status.text = "Status: Everything looks great!";
	    }
	    catch(Exception ex)
	    {
	        Status.text = "Status: Something went wrong. See console for exception text.";
	        Debug.LogError(ex);
	    }
	}
}