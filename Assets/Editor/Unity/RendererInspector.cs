using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;  
using UnityEditorInternal;  

[CanEditMultipleObjects()]
[CustomEditor(typeof(Renderer), true)]
public class RendererInspector : Editor 
{
    protected Renderer renderer;
    protected string[] sortingLayerNames;
    protected int      selectedOption;

    protected void OnEnable()
    {
        renderer = target as Renderer;

        sortingLayerNames = GetSortingLayerNames();
        for (int i = 0; i < sortingLayerNames.Length; ++i)
        {
            if (sortingLayerNames[i] == renderer.sortingLayerName)
            {
                selectedOption = i;
                break;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUI.BeginChangeCheck();
        selectedOption = EditorGUILayout.Popup("Sorting Layer", selectedOption, sortingLayerNames);
        if (EditorGUI.EndChangeCheck())
        {
            renderer.sortingLayerName = sortingLayerNames[selectedOption];
            EditorUtility.SetDirty(renderer);
        }
 
        int newSortingLayerOrder = EditorGUILayout.IntField("Sorting Layer Order", renderer.sortingOrder);
        if (newSortingLayerOrder != renderer.sortingOrder) 
        {
            renderer.sortingOrder = newSortingLayerOrder;
            EditorUtility.SetDirty(renderer);
        }

        if (GUILayout.Button("Apply to children"))
        {
            Renderer[] childrenRenderer = renderer.transform.GetComponentsInChildren<Renderer>();
            if (childrenRenderer != null)
            {
                for (int i = 0; i < childrenRenderer.Length; ++i)
                {
                    childrenRenderer[i].sortingLayerName = sortingLayerNames[selectedOption];
                    childrenRenderer[i].sortingOrder = newSortingLayerOrder;
                }
            }
        }
    }

    public string[] GetSortingLayerNames()
    {
        Type t = typeof(InternalEditorUtility);
        PropertyInfo prop = t.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[])prop.GetValue(null, null);
    }
}
