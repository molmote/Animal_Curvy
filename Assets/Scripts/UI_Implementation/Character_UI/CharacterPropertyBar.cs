using UnityEngine;
using System.Collections;


public class CharacterPropertyBar : BaseWidgetEntity
{			
	private 		UILabel 			labelUnlockNumber;

 	private 		Transform 			unlocked;
	private 		Transform 			locked;

	private 		UILabel 			labelName;

	private 		GameCharacterInformation  				serialized;
	private 		Vector3 			originalPosition;
	private 		int 				idMoveTween = -1;

	public 		void    Move(int offset)
	{
		//this.transform.localPosition = this.originalPosition + (Vector3.right * offset);
	}

	public 		void 	ResetPosition()
	{
		//this.transform.localPosition = this.originalPosition;
	}

	public 		void    MoveTween(int offset)
	{
		/*if ( this.transform.localPosition.x != (int)this.originalPosition.x + offset )
		{
			if ( this.idMoveTween != -1 )
			LeanTween.cancel(this.gameObject, this.idMoveTween);

			this.idMoveTween = LeanTween.moveLocal( this.gameObject, 
				this.originalPosition + (Vector3.right * offset),
				CharacterMenuEntity.instance.spacingTimeSpan ).id;
		}*/
	}

	public 		CharacterPropertyBar( )
	{
	}

	public 		void 	Unlock()
	{		
		this.unlocked.gameObject.SetActive(true);
		this.locked.gameObject.SetActive(false);

		if ( UserInfo.instance.IsCharacterLocked(this.serialized.index) )
		{
			//UserInfo.instance.UnlockCharacter(this.serialized.index);				
		}
	}

	public 		void    Lock()
	{
		this.unlocked.gameObject.SetActive(false);
		this.locked.gameObject.SetActive(true);
	}

	public 		void  	Initialize( GameCharacterInformation info, bool _locked )
	{
		if ( info != null )
		this.serialized 		= info;

		this.originalPosition 	= new Vector3( this.serialized.index * 160f, 0,0 );

        this.unlocked           = this.GetComponent<Transform>("Unlock");
        UISprite unlockSprite 	= this.GetComponent<UISprite>("Sprite Preview Unlock");
        unlockSprite.spriteName = info.prefabName;
        unlockSprite.MakePixelPerfect();

	    this.locked           	= this.GetComponent<Transform>("Preview Lock");
        UISprite lockSprtie 	= this.GetComponent<UISprite>("Sprite Preview Lock");

        if ( this.serialized.purchaseType == PURCHASE_TYPE.TWITTER || 
        	 this.serialized.purchaseType == PURCHASE_TYPE.FACEBOOK )
        {
	        lockSprtie.spriteName = info.prefabName+" Lock";
        }
        else
        {
        	lockSprtie.spriteName = info.prefabName;	
        }
        lockSprtie.MakePixelPerfect();

        if ( PlayerCharacteristic.Big == this.serialized.size )
        {
        	this.GetComponent<Transform>("Preview Lock").localScale = 
        		CharacterMenuEntity.instance.bigCharacterScale;
        	this.GetComponent<Transform>("Preview Unlock").localScale = 
        		CharacterMenuEntity.instance.bigCharacterScale;
        }
	    this.AddOnDragListener( this.gameObject, this.gameObject.name,         OnMyDrag);

		if ( _locked == false )
		{
			this.Unlock();
		}
	}

	public 		int 	GetUnlockCondition()
	{
		return this.serialized.unlockCondition;//.price;
	}

	public 		bool 	OnPurchaseTry ()
	{
		return false;
	}

    void OnMyDrag(GameObject go, Vector2 delta)
    {
        float momentum = Mathf.Lerp( 
        	CharacterMenuEntity.instance.defaultMomentum, 
        	CharacterMenuEntity.instance.maxMomentum, 
        	Mathf.Abs(delta.x) / CharacterMenuEntity.instance.dragSensibility );

        float speedRatio = momentum / CharacterMenuEntity.instance.maxMomentum;
        float damping = Mathf.Lerp( 
        	CharacterMenuEntity.instance.dampingRange.min, 
        	CharacterMenuEntity.instance.dampingRange.max, 
        	speedRatio );
        GameUIManager.instance.scrollDampingMomentum = damping;
        CharacterMenuEntity.instance.ChangeMomentum( momentum );
    }

    public void Update()
    {
    	//if ( this.isFocused == false )
    	//	this.transform.localScale = Vector3.one;
    }

    private bool isFocused;

    public void OnFocusChanged(bool focus)
    {	
    	if ( this.isFocused == focus )
    	{

    	}
    	else
    	{
	    	if ( focus )
	    	{
	    		// LeanTween.cancel(this.gameObject);
	    		LeanTween.scale( this.gameObject, 
	    			CharacterMenuEntity.instance.focusedSize, 
	    			CharacterMenuEntity.instance.upScaleTimeSpan );
	    	}
	    	else
	    	{
	    		LeanTween.cancel(this.gameObject);
	    		LeanTween.scale( this.gameObject, Vector3.one, CharacterMenuEntity.instance.downScaleTimeSpan );
	    	}
    	}
    	this.isFocused = focus;
        this.unlocked           = this.GetComponent<Transform>("Unlock");
        UISprite unlockSprite 	= this.GetComponent<UISprite>("Sprite Preview Unlock");
        unlockSprite.MakePixelPerfect();

	    this.locked           	= this.GetComponent<Transform>("Preview Lock");
        UISprite lockSprtie 	= this.GetComponent<UISprite>("Sprite Preview Lock");
        lockSprtie.MakePixelPerfect();
    }
}