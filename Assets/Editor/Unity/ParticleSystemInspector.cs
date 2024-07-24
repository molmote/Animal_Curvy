using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects()]
[CustomEditor(typeof(ParticleSystemSortingLayerExtension), true)]
public class ParticleRendererInspector : RendererInspector 
{
    protected void OnEnable()
    {
        renderer = (target as ParticleSystemSortingLayerExtension).GetComponent<ParticleSystem>().GetComponent<Renderer>();

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
}
