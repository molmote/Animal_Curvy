using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtreamComboManager
{
}

public partial class GameUIManager : UIEntityManager 
{
	private static GameUIManager _instance = null;//new GameUIManager();
	public  static GameUIManager instance
	{
		get
		{
			return _instance;
		}
	}

	private GameUIManager() 			
	{
		_instance = this;
	}

	void OnEnable()
	{
	}

	public void HideFinger()
	{
		if ( this.animationFinger != null && this.animationFinger.IsPlaying() == false )
        this.animationFinger.gameObject.SetActive(false);
	}

    public void AnimateFinger()
    {
    	this.animationFinger.gameObject.SetActive(true);
        AnimationEntity.OnAnimationFinishDelegate OnDisappear = delegate(AnimationEntity ani)
        { 
            this.animationFinger.gameObject.SetActive(false);
        };
        this.animationFinger.Play("Finger Ingame", OnDisappear);
    }

	public void ChangePreviewInstance(GameCharacterInformation info)
	{
	}

	public void ResetDeadTrigger()
	{
        if ( this.resultBeforePosition != null )
        	this.resultBeforePosition.GetComponent<Animator>().SetTrigger("Die");
	}

	public void ChangeColorToSnow(Color color)
	{
		this.labelScore.color 	 = color;
		this.labelDiamond.color  = color;
		this.spriteDiamond.color = color;
	}

	void Awake()
	{
		GameObject game 		= this.FindChild("In Game UI");
        game.GetComponent<UIPanel>().alpha = 1f;
		game.SetActive(false);

		this.labelScore 		= this.GetComponent<UILabel>("In Game UI", "Label Score");
		//this.labelFollower 		= this.GetComponent<UILabel>("In Game UI", "Label Follower");	
		this.labelDiamond 		= this.GetComponent<UILabel>("In Game UI", "Label Diamond");	
		this.spriteDiamond 		= this.GetComponent<UISprite>("In Game UI", "Sprite Diamond Icon");	
		this.aniNewRecord 		= this.GetComponent<Transform>("In Game UI", "New Record");	
		this.aniReached500 		= this.GetComponent<Animation>("In Game UI", "Label Diamond");	

		this.animationCombo 	= this.GetComponent<AnimationEntity>("Combo");
		this.labelCombo	 		= this.GetComponent<UILabel>("Combo", "Label Combo");

		this.animationCombo.gameObject.SetActive(false);
		// this.gameObject.SetActive(false);

		this.colorComboDefault 	= this.labelCombo.color;
		this.colorComboFailed 	= new Color(255f/255f, 68f/255f, 102f/255f);

		//this.transformBuff		= this.GetComponent<BuffPropertyUI>("In Game UI", "Bonus Effect Preview");
		this.animationFinger 	= this.GetComponent<AnimationEntity>("In Game UI", "Finger");
		this.dangerLeft 		= new DangerUI( this.GetComponent<UIWidget>("Danger Left") );
		this.dangerRight 		= new DangerUI( this.GetComponent<UIWidget>("Danger Right") );

		this.ChangePreviewInstance( UserInfo.instance.GetCharacterInfo() );
	}

	//<<-----------------------------------------------------------------------
	public  void UpdateUI()
	{
		this.labelScore.text 	= ""+Mathf.FloorToInt( GamePlayerEntity.instance.currentScore );
		// this.labelFollower.text = ""+Mathf.FloorToInt( GamePlayerEntity.instance.currentFollower );
		int dia = UserInfo.instance.GetCurrentDiamond() + GamePlayerEntity.instance.currentDiamond;
		this.labelDiamond.text  = ""+dia;
		if ( this.notEnough && dia >= 500 )
		{
			MyLogger.Yellow("UpdateUI", "OnNormalCharacterAvailable");
			this.notEnough = false;
			this.OnNormalCharacterAvailable();
		}
	}

	bool notEnough = true;

	public  void ResetUI()
	{
		this.labelScore.text 	= "0";
		this.labelDiamond.text  = ""+UserInfo.instance.GetCurrentDiamond();
		this.aniNewRecord.gameObject.SetActive(false);
		// this.aniReached500.gameObject.SetActive(false);
		this.ChangeColorToSnow( Color.white );
    	this.animationFinger.gameObject.SetActive(false);
		this.dangerLeft.Hide();
		this.dangerRight.Hide();

		if ( UserInfo.instance.GetCurrentDiamond() < 500 )
		this.notEnough = true;
	}

	private Color colorComboFailed;
	private Color colorComboDefault;

	public  void ShowNewRecord()
	{
		this.aniNewRecord.gameObject.SetActive(true);
		// this.aniNewRecord.PlayAndDeactive("New Record");
	}

	private void OnNormalCharacterAvailable()
	{
		this.aniReached500.PlayQueued("Score", QueueMode.PlayNow);
	}

	void Update()
	{
        if ( Input.GetKeyUp(KeyCode.Escape) )
        {
			GameInfo.instance.Resume();     
        }

		if ( GamePlayerEntity.instance.IsDead() || GameInfo.instance.IsGamePaused() || 
		     GamePlayerEntity.instance.IsFsmState("MainAuto") )
		{
			return;
		}

		this.UpdateUI();
	}

	public  void DisableCombo()
	{
        this.animationCombo.gameObject.SetActive(false);
        this.labelCombo.color = this.colorComboDefault;
	}

	public  void ResetCombo()
	{
		if ( UserInfo.instance.gameplayTutorialInitiated )
			return;
			
        AnimationEntity.OnAnimationFinishDelegate OnDisappear = delegate(AnimationEntity animationEntity)
        {
        	this.DisableCombo();
        };

        this.labelCombo.color = this.colorComboFailed;
		this.animationCombo.Play("Combo Failed", OnDisappear, true);
	}

	public  void UpdateCombo(int combo)
	{
		if ( UserInfo.instance.gameplayTutorialInitiated )
			return;
		this.animationCombo.gameObject.SetActive(true);
        this.labelCombo.color = this.colorComboDefault;

		this.animationCombo.Play("Combo", null, true);
		this.labelCombo.text = ""+combo;
	}

	bool right;

	public  void InvokeDanger(bool _right)
	{
		this.right = _right;
		float time = Mathf.Lerp( _SETTING._.dangerDelayRange.max, _SETTING._.dangerDelayRange.min, 
			GamePlayerEntity.instance.verticalSpeed / (_SETTING._.verticalSpeedDefault * 1.5f) );
		this.Invoke("ShowDanger", time);
	}

	public  void ShowDanger()
	{
		if ( GamePlayerEntity.instance.IsDead() || GameInfo.instance.IsGamePaused() || 
		     GamePlayerEntity.instance.IsFsmState("MainAuto") || UserInfo.instance.gameplayTutorialInitiated )
		{
			MyLogger.Blue("DangerUI", "ShowDanger Cancelled");
			return;
		}

		if ( this.right )
		{
			this.dangerRight.Show();
		}
		else
		{
			this.dangerLeft.Show();
		}
	}

	private  			Transform 				aniNewRecord;
	private 			Animation 				aniReached500;

	public 				float 					scrollDampingMomentum;

	public 				Transform 				unlockPreviewPosition;
	private 			Transform 				mainBeforePosition;
	private 			Transform 				resultBeforePosition;
	private 			AnimationEntity 		animationFinger;
	[SerializeField] 	AnimationEntity 		animationCombo;
	private				UILabel 				labelCombo;

	private 			UILabel 				labelScore;
	private 			UILabel 				labelDiamond;
	private 			UISprite 				spriteDiamond;
	private 			DangerUI 				dangerLeft;
	private 			DangerUI 				dangerRight;

}	

public class DangerUI
{
	UIWidget 			danger;
	AnimationEntity 	aniController;

	public DangerUI( UIWidget ui )
	{
		this.danger = ui;

		this.aniController = ui.GetComponent<AnimationEntity>();
		//this.danger.alpha  = 0;
		this.danger.gameObject.SetActive(false);
	}

	public bool IsActive()
	{
		return this.danger.gameObject.activeInHierarchy;
	}

	public void Alpha(float alpha)
	{
		this.danger.alpha = alpha;
	}

	public void Show()
	{
		//GameInfo.instance.Pause();
		AnimationEntity.OnAnimationFinishDelegate OnFollowingAnimation = delegate(AnimationEntity animationEntity)
        {
			this.danger.gameObject.SetActive(false);
        };

		MyLogger.Red("DangerUI", "Show"+GamePlayerEntity.instance.transform.position);
		this.danger.gameObject.SetActive(true);
		this.aniController.Play("Danger", OnFollowingAnimation, true, true);
	}

	public void StartHide()
	{
	}

	public void Hide()
	{
		MyLogger.Red("Danger", "Hide");
		this.aniController.gameObject.SetActive(false);
	}
}
