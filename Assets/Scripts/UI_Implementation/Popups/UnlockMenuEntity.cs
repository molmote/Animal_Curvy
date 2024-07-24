using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnlockMenuEntity : MyPopupEntity 
{
    private static UnlockMenuEntity _instance = null;
    public static UnlockMenuEntity instance 
    {
        get
        {
            return _instance;
        }
    }

	// Use this for initialization
	protected override void Awake()
	{
        _instance = this;
        base.Awake();
        // this.GetComponent<UIPanel>().alpha = 0f;

        this.labelCharacterName 	= this.GetComponent<UILabel>("Label Name");
        this.labelCondition         = this.GetComponent<UILabel>("Label Condition Unlock");

        this.AddOnClickListener("Button Select",         OnExitClick);
        this.AddOnClickListener("Button Share",          OnShareClick);
	}

    private void OnShareClick(GameObject go)
    {
        MyLogger.Red("Unl", "OnShareClick");

        GameCharacterInformation info = GameInfo.instance.GetCharacterInfo(this.characterIndex);
        SocialManager.instance.UnlockShare( info.prefabName );
    }

    private void PlayAnimalSound()
    {
        GameCharacterInformation info = GameInfo.instance.GetCharacterInfo(this.characterIndex);
        SoundController.instance.Play(info.unlockSound);
    }

    bool shouldReturnToMenu;
    List<int> charIndexList;

    private void InvokeMultipleUnlock()
    {
        this.ShowMultipleUnlock(this.charIndexList);
    }

    public void ShowMultipleUnlock( List<int> charIndex)
    {
        this.charIndexList = charIndex;

        this.Show( this.charIndexList[0], true );
        this.charIndexList.RemoveAt(0);
    }

	public void Show( int charIndex, bool isCharacterMenu = false )// float delay)
	{
        //GoogleAnalytics.Client.SendScreenHit("Popup-UnlockCharacter");

        this.shouldReturnToMenu = !isCharacterMenu;
        this.gameObject.SetActive(true);
        // this.GetComponent<UIPanel>().alpha = 0f;

        this.characterIndex = charIndex;
        GameCharacterInformation info = GameInfo.instance.GetCharacterInfo(this.characterIndex);
        this.Invoke("PlayAnimalSound", 1f);

        GameObject character = AbstractObjectPooler.instance.GetSngletonObject( "Unlock"+info.prefabName );  
        character.SetActive(true);
        if ( info.size == PlayerCharacteristic.Small )
        {
            character.transform.localScale = new Vector3(0.7f,0.7f,1);
        }
        else if ( info.size == PlayerCharacteristic.Big )
        {
            character.transform.localScale = new Vector3(1.4f,1.4f,1);
        }
        this.preview = character.transform;
        this.GetComponent<Transform>("Preview").localScale = Vector3.zero;

        this.labelCharacterName.text = GameInfo.instance.
            GetLocalizedCharacterName(this.characterIndex);// info.prefabName.ToUpper();
        this.labelCharacterName.bitmapFont = UserInfo.instance.GetLocalizationFont();

        this.labelCondition.gameObject.SetActive(false);
        CharacterMenuEntity.GetConditionText(info,this.labelCondition);
	}


    public bool isBonusSpinUnlock { get; set; }
    public System.Action onBonusSpinUnlock;
    protected override void OnExitClick(GameObject go)
    {
        bool isCharacterNew = UserInfo.instance.UnlockCharacter(this.characterIndex);
        if ( isCharacterNew )
        {
            UserInfo.instance.SelectCharacter(this.characterIndex);
        }

        AnimationEntity.OnAnimationFinishDelegate half = delegate(AnimationEntity animationEntity)
        {
            if ( this.preview != null )
            this.preview.gameObject.SetActive(false); 
            this.gameObject.SetActive(false);
        };

        AnimationEntity.OnAnimationFinishDelegate end = delegate(AnimationEntity animationEntity)
        {
            // GameUIManager.instance.ResetUI();
        };

        if ( this.shouldReturnToMenu )
        {
            if (isBonusSpinUnlock)
            {
                onBonusSpinUnlock();
                MenuTransitionPopup.instance.StartTransition(half, end);
            }
            else
            {
                ResultPopupEntity.instance.OnCharacterConfirmed(isCharacterNew);
                MenuTransitionPopup.instance.StartTransition(half, end);
            }
            onBonusSpinUnlock = null;
            isBonusSpinUnlock = false;
        }
        else
        {
            if ( this.preview != null )
            this.preview.gameObject.SetActive(false); 
            this.gameObject.SetActive(false);

            if ( this.charIndexList != null && this.charIndexList.Count > 0 )
            {
                CharacterMenuEntity.instance.UpdateMainPosition( this.charIndexList[0] );
                CharacterMenuEntity.instance.UpdateBarStatus( this.charIndexList[0] );
                this.Invoke("InvokeMultipleUnlock", 0.2f);
            }
            else
            {
                CharacterMenuEntity.instance.UpdateMainPosition();
                CharacterMenuEntity.instance.UpdateBarStatus();
            }

        }
    }

    private     int         characterIndex;
    private     UILabel     labelMissionCompleted;
    private     UILabel     labelCharacterName;
    private     Transform   preview;
    private     UILabel     labelCondition;
    AnimationEntity.OnAnimationFinishDelegate OnDisappear;
}
