using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationSoundEntity : BaseSoundEntity
{
	public  List<string> 		soundPathList;

	public void SoundPlay()
	{
		if ( soundPathList.Count > 0)
		SoundController.instance.Play( soundPathList[(int)soundIndex], true);
	}

	public void SoundPlayIndex(int index)
	{
		if ( soundPathList.Count > index)
		SoundController.instance.Play( soundPathList[index], true);
	}
}