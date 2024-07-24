using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour 
{
	public delegate void OnProcessFinishedDelegate();
	public OnProcessFinishedDelegate 				OnProcessFinished;

	private Dictionary<string, BaseSoundEntity> 	dict;
	private List<BaseSoundEntity> 					list;

	private Dictionary<string, BaseSoundEntity> 	staticDict;
	private List<BaseSoundEntity> 					staticList;

	public  string 									soundPath;

	private static SoundController _instance = null;
	public  static SoundController instance
	{
		get
		{
			if(_instance == null)
			{
				GameObject obj = GameObject.Find("_Plugins");
				_instance = obj.GetComponent<SoundController>();
				// SoundManager.UnPause();
				// _instance.gameObject.SetActive(true);
				// GameObject gameObj = new GameObject("SoundController");
				// _instance = gameObj.AddComponent<SoundController>();
			}

			return _instance;
		}
	}

	public void Awake()
	{
		DontDestroyOnLoad(gameObject);

		 //gameObject.AddComponent<AudioListener>();
	}

	private void OnDestroy()
	{
		this.Clear();

		this.staticList.Clear();
		this.staticDict.Clear();
	}

	public void Initialize(string []names = null, bool isStatic = false)
	{
		this.dict 		= new Dictionary<string, BaseSoundEntity>();
		this.list 		= new List<BaseSoundEntity>();

		this.staticDict = new Dictionary<string, BaseSoundEntity>();
		this.staticList = new List<BaseSoundEntity>();
		
		if(isStatic)
		{
			foreach(string name in names)
			{
				GameObject 					gameObj   	 = new GameObject(name);
				AudioSource 				audioSource  = gameObj.AddComponent<AudioSource>();
				ProgrammableSoundEntity 	audioEntity  = gameObj.AddComponent<ProgrammableSoundEntity>();
				AudioClip 					audioClip 	 = Loader.Load(this.soundPath+"/"+name) as AudioClip;

				audioEntity.SetClip(audioClip);

				this.staticList.Add(audioEntity);
				this.staticDict[name] = audioEntity;				
			}
		}
		else
		{
			AudioClip [] audioClipArray = Resources.LoadAll<AudioClip>(this.soundPath);

			foreach(AudioClip audioClip in audioClipArray)
			{
				string name 							 = audioClip.name;
				GameObject 					gameObj   	 = new GameObject(name);
				gameObj.transform.parent 				 = this.gameObject.transform;
				ProgrammableSoundEntity 	audioEntity  = gameObj.AddComponent<ProgrammableSoundEntity>();
				
				audioEntity.SetClip(audioClip);

				this.list.Add(audioEntity);
				this.dict[name] = audioEntity;
				//Logger.Red("audio dictionary key : ", name);
			}
		}
	}

	public void Clear()
	{
		for(int i = 0; i < this.list.Count; i++)
		{
			GameObject.Destroy(this.list[i].gameObject);
		}

		this.list.Clear();
		// this.dict.Clear();
	}

	public bool IsPlaying(string key, bool isStatic = false)
	{
		if(isStatic)
		{
			return false;
		}
		else
		{
			return this.dict[key].IsPlaying();
		}
	}

	public void Play(string key, bool playOver = true, OnProcessFinishedDelegate OnProcessFinished = null, bool isStatic = false)
	{
		if(isStatic)
		{

		}
		else
		{
			this.dict[key].Play(!playOver);
		}
	}

	/*public void PlayMusic( string key, bool loop = true )
	{
		BaseSoundEntity sound 	= this.dict[key];
		AudioSource 	bgm 	= sound.GetComponent<AudioSource>();
		//sound.
		bgm.loop = loop;
		bgm.Play();
		//sound.Play(true);
	}*/

	public void Pause(string key, OnProcessFinishedDelegate OnProcessFinished = null, bool isStatic = false)
	{
		if(isStatic)
		{
			
		}
		else
		{
			this.dict[key].Pause();
		}
	}

	public void Stop(string key, OnProcessFinishedDelegate OnProcessFinished = null, bool isStatic = false)
	{
		if(isStatic)
		{
			
		}
		else
		{
			this.dict[key].Stop();
		}
	}

	public void SetVolume(float volume)
	{
		AudioListener.volume = volume;
	}
}
