using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgrammableSoundEntity : BaseSoundEntity
{
	private AudioClip audioSource;

	public void SetClip(AudioClip clip)
	{
		this.soundSourceList[0].clip 			= clip;
	}

	protected void Awake()
	{
		this.soundSourceList 			= new List<AudioSource>();
		/*GameObject 		audioClipObject = new GameObject();*/
		AudioSource 	audioSource 	= gameObject.AddComponent<AudioSource>();
		this.soundSourceList.Add(audioSource);
	
		soundIndex = 0;
	}

	public override void Play(bool notPlayingOver = true) 
	{
		if(base.IsPlaying() && notPlayingOver)
		{
			//Logger.Blue("DEBUG", "audio clip " + gameObject.name + "is already being played");
		}
		else
		{
			/*foreach(float playIndex in this.soundIndexList)
			{
				this.soundSourceList[(int)playIndex].Play();
			}*/
			this.soundSourceList[0].Play();
			//Logger.Red("DEBUG", "audio clip " + gameObject.name + "is now start");
		}
	}

	public override void Pause() 
	{
		if(base.IsPlaying())
		{
			foreach(float playIndex in this.soundIndexList)
			{
				this.soundSourceList[(int)playIndex].Pause();
			}
		}
		else
		{

		}
	}

	public override void Stop() 
	{
		if(base.IsPlaying())
		{
			foreach(float playIndex in this.soundIndexList)
			{
				this.soundSourceList[(int)playIndex].Stop();
			}
		}
		else
		{

		}
	}
}
