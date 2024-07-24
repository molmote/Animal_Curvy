using UnityEngine;
using System.Collections;

public class MenuTransitionPopup : BaseTransitionPopupEntity 
{
	private static MenuTransitionPopup _instance = null;
	public static MenuTransitionPopup instance 
	{
		get
		{	
			return _instance;
		}
	}

    protected override void Awake() 
    {        
    	_instance = this;
    	this.animationEntity = this.GetComponent<AnimationEntity>();
		this.OnAnimationHalfway = OnNullTrigger;
		this.OnAnimationFinish = OnNullTrigger;
    }

    AnimationEntity.OnAnimationFinishDelegate OnAnimationHalfway;
    AnimationEntity.OnAnimationFinishDelegate OnAnimationFinish;
    public AnimationEntity.OnAnimationFinishDelegate OnNullTrigger = delegate(AnimationEntity animationEntity){};

	public void StartTransition( 
		AnimationEntity.OnAnimationFinishDelegate onHalf, 
		AnimationEntity.OnAnimationFinishDelegate onFinish = null )
	{
		this.gameObject.SetActive(true);

		this.OnAnimationHalfway = onHalf;
		if ( onFinish != null )
		{
			this.OnAnimationFinish = onFinish;
		}
		else
		{
			this.OnAnimationFinish = this.OnNullTrigger;		
		}
		
        AnimationEntity.OnAnimationFinishDelegate OnDisappear = delegate(AnimationEntity animationEntity)
        {
        	//MyLogger.Red("StartTransition","OnDisappear");
            this.gameObject.SetActive(false);
        };

		this.animationEntity.Play("Transition", OnDisappear);
	}

	public void StartTransitionDead(
		AnimationEntity.OnAnimationFinishDelegate onHalf, 
		AnimationEntity.OnAnimationFinishDelegate onFinish = null )
	{
		this.gameObject.SetActive(true);

		this.OnAnimationHalfway = onHalf;
		if ( onFinish != null )
		{
			this.OnAnimationFinish = onFinish;
		}
		else
		{
			this.OnAnimationFinish = this.OnNullTrigger;		
		}
		
        AnimationEntity.OnAnimationFinishDelegate OnDisappear = delegate(AnimationEntity animationEntity)
        {
        	MyLogger.Red("StartTransitionDead","OnDisappear");
            this.gameObject.SetActive(false);
        };

		this.animationEntity.Play("Transition Die", OnDisappear);
	}

	public void TriggerHalfway()
	{
		this.OnAnimationHalfway(this.animationEntity);
		this.OnAnimationHalfway = OnNullTrigger;
	}

	public void TriggerFinish()
	{
		this.OnAnimationFinish(this.animationEntity);
		this.OnAnimationFinish = OnNullTrigger;
	}

    public override void CloseByAndroidBack()
    {        
        /*if ( MyPopupEntity.IsAnyPopupActive() )// || this.isClosing )
        {
            MyPopupEntity.OnCloseByAndroidCallback();
        }
        else
        {
            this.OnMenuClick(null);
            //this.isClosing = true;
        }*/
    }
}
