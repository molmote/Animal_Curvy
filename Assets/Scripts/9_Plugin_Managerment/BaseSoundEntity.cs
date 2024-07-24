using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BaseSoundEntity : MonoBehaviour 
{
	//! 
	//! Don't change it by code.
	//! It will managed by animatoins.
	//! 

	protected  List<AudioClip> 		soundList;

	public 	   float 				soundIndex;

	protected  List<int>			soundIndexList;

	//! 
	//! 
	//! 
	protected  List<AudioSource> 	soundSourceList;


	public BaseSoundEntity()
	{

	}

	//! 
	//!
	//!
	public  virtual void 		Play(bool notPlayingOver){}
	public  virtual void		Pause()					 {}
	public  virtual void 		Stop() 					 {}
	public  virtual void 		SetVolume(float volume)  {}


	protected  IEnumerator 			OnEndOfSound(System.Action onFinished)
	{
		while(enabled)
		{
			yield return null;
		}
	}

	protected  virtual void 		OnWait(System.Action onFinished)
	{
		StartCoroutine(OnEndOfSound(onFinished));
	}


	public  bool 				IsPlaying() 			
	{
		return this.soundSourceList[(int)soundIndex].isPlaying;
	}


	protected void Initialize(bool isPlayingByAnimation)
	{
		if(isPlayingByAnimation)
		{
			this.soundSourceList = new List<AudioSource>();
			foreach(AudioClip clip in this.soundList)
			{
				GameObject 		audioClipObject = new GameObject();
				AudioSource 	audioSource 	= audioClipObject.AddComponent<AudioSource>();
				this.soundSourceList.Add(audioSource);
			}
		}
		else
		{
			this.soundSourceList 			= new List<AudioSource>();
			GameObject 		audioClipObject = new GameObject();
			AudioSource 	audioSource 	= audioClipObject.AddComponent<AudioSource>();
			this.soundSourceList.Add(audioSource);
		}
	}


	private void OnDestroy()
	{

	}
}
