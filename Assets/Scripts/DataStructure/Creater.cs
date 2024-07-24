using UnityEngine;
using System.Collections;

public class Creater 
{
	public static GameObject Clone(GameObject gameObj, bool nameClear = true)
	{
		GameObject game = GameObject.Instantiate(gameObj) as GameObject;
		if (nameClear)
			game.name = game.name.Replace("(Clone)","");
		return game;
	}

	public static GameObject Create(string prefabPath, bool nameClear = true )
	{
		GameObject game = Creater.Clone( Loader.LoadPrefab(prefabPath), nameClear );
			
		return game;
	}
}
