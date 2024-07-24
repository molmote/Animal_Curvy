using UnityEngine;
using System.Collections;

public class Loader
{
	public static System.Object Load(string path)
	{
		return Resources.Load(path);
	}

	public static System.Object[] LoadAll(string path)
	{
		return Resources.LoadAll(path);
	}
	
	public static GameObject LoadPrefab(string prefabpath)
	{
		return Resources.Load(prefabpath) as GameObject;
	}

	public static string LoadTextAsset(string filepath)
	{
		TextAsset rawData = Resources.Load(filepath) as TextAsset;
		return rawData.text;		
	}
}