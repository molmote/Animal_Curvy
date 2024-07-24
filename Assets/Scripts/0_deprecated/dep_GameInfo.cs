using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameInfo
{
	private void InitEnum()
	{
	}


	public static bool IsLanguageLocalized(string localeName)
	{
		for ( int i = 0 ; i < (int)LanguageType.SIZE ; i++ )
		{
			if ( localeName == ((LanguageType)i).ToString() )
			return true;
		}
		return false;
	}

	public static int GetLocaleCode(string localeName)
	{		
		for ( int i = 0 ; i < (int)LanguageType.SIZE ; i++ )
		{
			if ( localeName == ((LanguageType)i).ToString() )
			return i;
		}
		return -1;
	}
}


public enum INGAME_STATE : int
{
	NOTREADY,
	PLAYING,
	TRANSITION,
	TRANSITIONFINISHED,
	PAUSED
};

public enum ElementalStatus : int 
{
	SIZE = 0
};


public enum ChapterList : int
{
	FOREST = 0,
	DESERT,
	SIZE
};
