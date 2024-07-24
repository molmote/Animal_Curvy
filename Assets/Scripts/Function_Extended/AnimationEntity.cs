using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationEntity : MonoBehaviour
{
	public  delegate void OnAnimationFinishDelegate(AnimationEntity animationEntity);
	public  Dictionary<string, OnAnimationFinishDelegate> OnAniFinishCallbacks;
	public  Dictionary<string, OnAnimationFinishDelegate> EventCallback;

	private Animation _animation = null;

	protected AnimationEntity()						 
	{
		
	}

	void Awake()
	{
		this.OnAniFinishCallbacks = new Dictionary<string, OnAnimationFinishDelegate>();
		this.EventCallback 		  = new Dictionary<string, OnAnimationFinishDelegate>();
		this._animation = gameObject.GetComponent<Animation>();
	}

	///
	public bool IsPlaying(string name)
	{
		return _animation.IsPlaying(name);
	}

	public bool IsPlaying()
	{
		if ( _animation == null )
			_animation = this.GetComponent<Animation>();
		return _animation.isPlaying;
	}

	public void RegisterEvent(string animationName,
					 OnAnimationFinishDelegate OnEventCallback)
	{
		this.EventCallback[animationName] = OnEventCallback;
	}

	public void RegisterEvent( OnAnimationFinishDelegate OnEventCallback )
	{
		this.EventCallback[_animation.clip.name] = OnEventCallback;
	}

	public void Play(string animationName,
					 OnAnimationFinishDelegate OnFinishEventCallback = null,
					 bool isPlayNow = true, bool playSame = false )
	{
		if(_animation == null)
			this._animation = gameObject.GetComponent<Animation>();

		if(animationName == null)
		{
			animationName = this._animation.clip.name;
		}

		// to prevent achievement reward panel error at the opening
		if(this.gameObject.activeInHierarchy == false)
			return;

		QueueMode mode = QueueMode.CompleteOthers;
		if( isPlayNow )
		{			
			mode = QueueMode.PlayNow;
			//if ( this.IsPlaying() )
			this.Stop();
		}
		else
		{
			//MyLogger.Red("DEBUG","animation is not played now");
		}

		this.OnAniFinishCallbacks[animationName]	 = OnFinishEventCallback;
		if ( this.IsPlaying(animationName) || playSame )
		{
			this._animation.CrossFadeQueued(animationName, 0.1f, mode);
		}
		else
		{
			this._animation.PlayQueued(animationName, mode);
		}
		this._animation.gameObject.GetComponent<MonoBehaviour>().StartCoroutine(StartCheckingEnd(animationName));

		//MyLogger.Red("DEBUG","animation("+ animationName +") start now");
	}

	public void Stop()
	{
		this._animation.Stop();	
	}

	public void SetAnimationSpeed(string animationName, float speed)
	{		
		this._animation[animationName].speed = speed;
	}

	public float GetAnimationLength(string animationName)
	{
		return this._animation[animationName].length;
	}

	public IEnumerator StartCheckingEnd(string animationName)
	{
		while( this.IsPlaying(animationName) )
		{
			//if( animationName == "transitionEnd" || animationName == "transitionStart" )
			//MyLogger.Log("DEBUG","animation(" + animationName + ") is still playing");

			yield return null;
		}

		//MyLogger.Blue("DEBUG","an animation("+ animationName+ ") is finished");
		 
		if(OnAniFinishCallbacks[animationName] != null)
		{	
			OnAniFinishCallbacks[animationName](this);
		}
	}

	public void PlayAndDeactive(string animationAppear, bool isPlayNow = true)
	{
		AnimationEntity.OnAnimationFinishDelegate OnFollowingAnimation = delegate(AnimationEntity animationEntity)
        {
			MyLogger.Log("DEBUG","on animation finish: " + animationAppear);
        	this.gameObject.SetActive(false);
        };

		this.Play(animationAppear, OnFollowingAnimation, isPlayNow);
	}

	public void PlayAnimationEvent()
	{
		this.EventCallback[this._animation.clip.name](this);
	}

	public void PlayAnimationEventName(string name)
	{
		this.EventCallback[name](this);
	}
}
