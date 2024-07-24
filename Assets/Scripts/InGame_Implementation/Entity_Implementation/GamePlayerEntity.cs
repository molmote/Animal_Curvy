using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public partial class GamePlayerEntity : GameEntity 
{

	private static GamePlayerEntity _instance = null;
	public static GamePlayerEntity instance 
	{
		get
		{	
			if ( _instance == null )
			{
		    	string name = GameInfo.instance.GetCharacterInfo(UserInfo.instance.currentCharacterIndex).prefabName;
		    	GamePlayerEntity.ChangeInstance(name);
			}
				//_instance = GameObject.Find("Fox").GetComponent<GamePlayerEntity>();
			return _instance;
		}
	}

	public 				float 						horizontalSpeedTarget;		// 180
	public				float  						horizontalSpeed;	
	public				float  						verticalSpeed;

	private 			GameObject 					particleDash;

	public 				float 						gameTimeSpan;
	[SerializeField]	float 						extreamTimer		= 0;

	[HideInInspector] 	
	public 				int   						tutorialCount;
	private 			float 						initialTurnTimer;

	private 			bool 						initialStraightCourse;

	private 			int  						countTurned;

	public Transform 	GetTransform()
	{
		if (this.myTransform == null )
			this.myTransform = this.transform;
		return this.myTransform;
	}

	void Awake()
	{
		this.isAlive = false;
		this.mainCamera = Camera.main;
	}

	void Start()
	{		
        this.transform.parent 		= TileController.instance.transform;

        this.skateBoard 					= this.GetComponent<Transform>("Board");
        this.particleDead 			= this.FindChild("Particle Dead");
        this.particleDead.SetActive(false);

        this.particleDash 			= this.FindChild("Dash Effect");
        if ( this.particleDash != null )
        {
        	this.particleDash.SetActive(false);
        }

        this.trail 					= this.GetComponent<TrailRenderer>("Trail Left");
        this.trail2 				= this.GetComponent<TrailRenderer>("Trail Right");

	    this.trail.sortingLayerName 	= "Object";
	    this.trail.sortingOrder 		= 0;
	    this.trail2.sortingLayerName 	= "Object";
	    this.trail2.sortingOrder 		= 0;

	    this.rootAnimation 		= this.transform.Find("Animation");
	    this.sphere 			= this.GetComponent<SphereCollider>();

        this.fsmInterface       = this.GetComponent<PlayMakerFSM>();
        this.myAnimator			= this.rootAnimation.GetComponent<Animator>();

        this.extreamTimer 		= _SETTING._.extreamTimeSpan;
	    this.mainCamera.GetComponent<SmoothFollowEntity>().OnMainMenu();
	    this.ResetValueForStart();
	}

	public  void ResetValueForAuto()
	{
	    this.mainCamera.GetComponent<SmoothFollowEntity>().OnMainMenu();
	    this.ResetValueForStart();
		this.gameObject.SetActive(true);
		if ( this.particleDead != null )
        this.particleDead.SetActive(false);
	}

	private void ResetValueForStart()
	{
		if ( this.trail == null )
			return;

        this.transform.localPosition    = Vector3.zero;

		this.accelerateTimer 	= 0f;
		this.startSlowTimer 	= 0f;
		this.startSlowFlag 		= true;
		this.horizontalSpeed 	= 0;
		this.verticalSpeed 		= 0;

		this.horizontalSpeedTarget = -Mathf.Abs(_SETTING._.GetHorizontalSpeedTarget(this));
		this.ApplyCharacterRotataion();

		this.ResetRotation();

		this.directionChangeApplied 	= true;
		this.initialStraightCourse 		= true;

        this.trail.gameObject.SetActive(true);
        this.trail2.gameObject.SetActive(true);

		this.transform.Find("Particle Right").GetComponent<ParticleSystem>().emissionRate = 0f;
		this.transform.Find("Particle Left").GetComponent<ParticleSystem>().emissionRate = 0f;
		this.ResetUIParameters();

		this.isAlive = true;
        this.particleDead.SetActive(false);

        this.ResetCombo();
        this.currentScore = this.targetScore = 0;
        this.scoreWithoutDiamond = 0;

		this.gameTimeSpan 	= 0f;
		this.tutorialCount 	= 0;
		this.initialTurnTimer = 0;
		this.countTurned	= 0;

        this.Invoke("InvokePlaySkateSound", 0.3f);
	}

	private void ResetUIParameters()
	{
		this.currentScore 		= 0;
		this.scoreWithoutDiamond = 0;
		this.targetScore 		= 0;

		this.currentComboCount	= 0;
		this.maxCombo 			= 0;
		this.currentDiamond 	= 0;
	}

	private void OnRestartTrigger()
	{

	}

	public  void RestartGame()
	{
        this.SendEvent("GAME_RETRY");
        this.myAnimator.SetTrigger("Start");
	}

	public  void SetIdle()
	{
        this.myAnimator.SetTrigger("Idle");
		this.horizontalSpeedTarget = -Mathf.Abs(this.horizontalSpeedTarget);
		this.ApplyCharacterRotataion();

		this.ResetRotation();
		if ( this.particleDead != null )
        this.particleDead.SetActive(false);
	}

	public bool isLoading;

	public void OnStartTrigger()
	{
		this.isLoading = true;
        this.trail.time = 0;
        this.trail2.time = 0;
		MissionController.instance.IncrementCount(MissionType.COUNT_GAMEPLAY, 1);
		UserInfo.instance.IncrementPlayCount();
		MissionFeedbackController.instance.ShowMissionStart();

        AnimationEntity.OnAnimationFinishDelegate half = delegate(AnimationEntity animationEntity)
        {
    		//MyLogger.Red("StartTransition","OnHalf");
        	this.OnHalfStartGame();
        };

        AnimationEntity.OnAnimationFinishDelegate end = delegate(AnimationEntity animationEntity)
        {
    		//MyLogger.Red("StartTransition","OnEnd");
            this.OnStartGame();
			this.isLoading = false;
        };

		MenuTransitionPopup.instance.StartTransition(half, end);
	}

	private void OnHalfStartGame()
	{
        this.transform.localPosition    = Vector3.zero;

	    this.mainCamera.GetComponent<SmoothFollowEntity>().OnStartGame();
   	 	TileController.instance.OnRestart();

    	//FindChild(GameUIManager.instance.gameObject, "Game Over UI").gameObject.SetActive(false);
    	FindChild(GameUIManager.instance.gameObject, "Main").gameObject.SetActive(false);

    	if ( UserInfo.instance.gameplayTutorialInitiated == false )
    	{
        	FindChild(GameUIManager.instance.gameObject, "In Game UI").SetActive(true);
			GameUIManager.instance.DisableCombo();

			GameUIManager.instance.HideFinger();
    	}
    	else
    	{
    		TutorialMenuEntity.instance.Show();
    	}

        MissionFeedbackController.instance.UpdateNotification();
		this.gameObject.SetActive(true);

		this.accelerateTimer 	= 0f;
		this.startSlowTimer 	= 0f;
		this.startSlowFlag 		= true;
		this.horizontalSpeed 	= 0;
		this.verticalSpeed 		= 0;
		this.tutorialCount 		= 0;
		this.initialTurnTimer 	= 0;

		this.horizontalSpeedTarget = -Mathf.Abs(this.horizontalSpeedTarget);
		this.ApplyCharacterRotataion();

		this.ResetRotation();

		this.directionChangeApplied 	= true;
		this.initialStraightCourse 		= true;

		this.ResetUIParameters();
        
		this.horizontalSpeedTarget = -Mathf.Abs(_SETTING._.GetHorizontalSpeedTarget(this));

		this.transform.Find("Particle Right").GetComponent<ParticleSystem>().emissionRate = 0f;
		this.transform.Find("Particle Left").GetComponent<ParticleSystem>().emissionRate = 0f;
	}

	private void OnStartGame()
	{

		this.isAlive = true;
        this.particleDead.SetActive(false);

        this.ResetCombo();
        this.currentScore = this.targetScore = 0;
        this.scoreWithoutDiamond = 0;

		this.gameTimeSpan 	= 0f;
		this.countTurned	= 0;

        this.Invoke("InvokePlaySkateSound", 0.3f);
        this.trail.time  = 1;//.SetActive(false);
        this.trail2.time = 1;//gameObject.SetActive(false);
	}
	
	public  void InvokePlaySkateSound()
	{
		this.PlaySkateSound(false);
	}
	

	private void OnDamaged()
	{
		if ( this.IsDead() )
			return;

		if ( false == UserInfo.instance.gameplayTutorialInitiated )
		UserInfo.instance.ChangeBestDiamond(this.maxCombo);
		
		this.directionalSnowEffect = this.transform.Find("Particle Right").GetComponent<ParticleSystem>();
        this.directionalSnowEffect.emissionRate = 0;
        
		this.directionalSnowEffect = this.transform.Find("Particle Left").GetComponent<ParticleSystem>();
        this.directionalSnowEffect.emissionRate = 0;

		this.isAlive = false;

        AnimationEntity.OnAnimationFinishDelegate half = delegate(AnimationEntity a1)
        {
    		MyLogger.Red("StartTransition","OnHalf");
        };

        AnimationEntity.OnAnimationFinishDelegate end = delegate(AnimationEntity a2)
        {
    		MyLogger.Red("StartTransition","OnEnd");
            //this.OnStartGame();
        };
        	//GameUIManager.instance.HideFinger();

		MenuTransitionPopup.instance.StartTransitionDead(half, end);

		this.particleDead.SetActive(true);
		if (this.particleDash != null) 
			this.particleDash.SetActive(false);

		this.myAnimator.SetTrigger("Die");
		SoundController.instance.Play(UserInfo.instance.GetCharacterInfo().dieSound);
		this.fsmInterface.SendEvent("ON_PLAYER_DEAD");
	}

	void OnCameraWalk()
	{
		this.accelerateTimer 			= 0;
		this.directionChangeApplied		= false;
		
		if ( this.directionalSnowEffect != null )
        	this.directionalSnowEffect.emissionRate = 0;

		this.emissionTimer 		= 	 0;

		this.horizontalSpeedTarget 		= -this.horizontalSpeedTarget;

		if ( this.horizontalSpeedTarget > 0 )
		{
			this.rootAnimation.rotation = Quaternion.Euler(0, 180, 0);
		}
		else
		{
			this.rootAnimation.rotation = Quaternion.identity;
		}

		Vector3 camPos = this.mainCamera.transform.position;
		camPos += Vector3.right * this.horizontalSpeed * _SETTING._.walkingTimeSpan * _SETTING._.playerDeadSpeed;
		camPos += Vector3.down  * this.verticalSpeed   * _SETTING._.walkingTimeSpan * _SETTING._.playerDeadSpeed;

		LeanTween.move(this.mainCamera.gameObject, 
			new Vector3( camPos.x, camPos.y, this.mainCamera.transform.position.z), 
			_SETTING._.walkingTimeSpan) 
		.setEase( _SETTING._.inDeadTweenType )
		.setOnComplete( this.OnCameraWalkingTween );
	}

	void OnCameraRecovery()
	{
		LeanTween.move(this.mainCamera.gameObject, 
			new Vector3( this.myTransform.position.x,this.myTransform.position.y + 100, this.mainCamera.transform.position.z), _SETTING._.recoveryTimeSpan) 
		.setEase( _SETTING._.outDeadTweenType )
		.setOnComplete( this.OnCameraRecoveryTween );
	}

	private void OnCameraRecoveryTween()
	{
		this.SendEvent("ON_CAMERA_RECOVERY");

		if ( UserInfo.instance.gameplayTutorialInitiated )
		{
			// this.SendEvent("ON_CAMERA_RECOVERY");
	    	this.SendEvent("GAME_RETRY");
	        this.myAnimator.SetTrigger("Start");
            this.myAnimator.SetTrigger("Idle");
		}
		else
		{
	        this.trail.gameObject.SetActive(false);
	        this.trail2.gameObject.SetActive(false);
		}
	}

	private void OnCameraWalkUpdate()
	{
		this.Move();
		this.Accelerate();
		this.ProcessAnimator();
		this.RotateBoard();
		this.emissionTimer += Time.deltaTime;
		if ( this.emissionTimer > _SETTING._.walkingTimeSpan )
		{
	        this.SendEvent("ON_CAMERA_WALK");
		}
	}

	public  void OnTutorialCanaceled()
	{
		if ( false == UserInfo.instance.gameplayTutorialInitiated )
			return;

        this.SendEvent("ON_TUTORIAL_CANCELED");
		
		if ( this.directionalSnowEffect != null )
        	this.directionalSnowEffect.emissionRate = 0;

		this.isAlive = false;

		if (this.particleDash != null) 
			this.particleDash.SetActive(false);

        this.trail.time 	= 0;
        this.trail2.time 	= 0;
        this.trail.time 	= 1;
        this.trail2.time 	= 1;

    	MissionController.instance.ResetNonStackMissionCount();
    	FindChild(GameUIManager.instance.gameObject, "In Game UI").SetActive(false);
    	FindChild(GameUIManager.instance.gameObject, "Main").SetActive(true);

    	this.SetIdle();
    	TileController.instance.OnRestart();
        
		return;
	}

	public  void ShowDelayedGameOver()
	{
	    FindChild(GameUIManager.instance.gameObject, "Game Over UI").SetActive(true);
	}

    private void        ShowInterstitial()
    {
        MyAdManager.instance.ShowInterstitial();
    }

	private void OnCameraWalkingTween()
	{ 
		if ( UserInfo.instance.gameplayTutorialInitiated )
		{
			return;
		}

		AnimationEntity.OnAnimationFinishDelegate OnDisappear = delegate(AnimationEntity animationEntity)
        {
        	GameUIManager.instance.ResetUI();
	    	FindChild(GameUIManager.instance.gameObject, "In Game UI").SetActive(false);

		    MyAdManager.instance.IncreasePopupProbability();
		    bool increasePossibility = MyAdManager.instance.IsPopupAvailable() ;
		    if ( increasePossibility )
		    {
		        this.Invoke ("ShowInterstitial", 1f);
		    }
		    else if ( UserInfo.instance.IsReviewPopupNeeded() )
		    {
		        ReviewPopupEntity.instance.Show();
		        this.Invoke("ShowDelayedGameOver", 0.8f);
		    }
		    else
		    {
		    	this.ShowDelayedGameOver();
		    }
        };

        if ( MissionController.instance.IsRewardAvailable() )
        {
			MissionFeedbackController.instance.ShowMissionEnd(OnDisappear);
        }
        else
        {
        	MissionController.instance.ResetNonStackMissionCount();
        	OnDisappear(null);
        }
	}

	public  float 	GetCurrentStagePlayedTime()
	{
		return this.gameTimeSpan;
	}

	private void OnAutomationMove()
	{
		this.initialTurnTimer += Time.deltaTime;
		if ( (this.initialTurnTimer > _SETTING._.initialTurnTimeSpanInGame && this.tutorialCount == 0 ) )
		{
			this.ChangeDirection();
			this.tutorialCount++;
		}
		else if (  Mathf.Abs(this.transform.position.x) > _SETTING._.playerXBoundaryInMain )
		{
			bool left = -this.transform.position.x > _SETTING._.playerXBoundaryInMain && (this.horizontalSpeedTarget < 0 );
			bool right  = this.transform.position.x > _SETTING._.playerXBoundaryInMain && (this.horizontalSpeedTarget > 0 );
			if ( left || right )
				this.ChangeDirection();
		}

		this.Move();
		this.Accelerate();
		this.ProcessAnimator();
		this.RotateBoard();
		this.RotateDashEffect();

		this.emissionTimer += Time.deltaTime;
		if ( this.emissionTimer > this.emissionTimeSpan && this.directionalSnowEffect != null )
		{
			this.directionalSnowEffect.emissionRate = 0f;
		}
		this.gameTimeSpan += Time.deltaTime;
	}

	void Update () 
	{
		if( this.myAnimator == null )
		{
			this.myAnimator = this.GetComponent<Animator>();
		}

		if( GameInfo.instance.IsGamePaused() )
		{
			this.myAnimator.speed = 0;
			return;
		}	

		this.myAnimator.speed = 1;

		if ( this.IsFsmState("MainAuto") ) 
		{
			this.OnAutomationMove();
		}
		else if ( false == this.IsDead() ) //618,260,600
		{
			this.initialTurnTimer += Time.deltaTime;
			float dest = this.transform.localPosition.x + (this.horizontalSpeed * Time.deltaTime);
			bool condition = 	this.tutorialCount < _SETTING._.tutorialTurnPosition.Count ;

			if ( UserInfo.instance.gameplayTutorialInitiated && this.directionChangeApplied && condition )
			{
				bool left  = dest > _SETTING._.tutorialTurnPosition[this.tutorialCount]; //this.horizontalSpeedTarget > 0 && ;
				bool right = dest < _SETTING._.tutorialTurnPosition[this.tutorialCount]; //this.horizontalSpeedTarget < 0 && ;
				if ( left  && (this.horizontalSpeedTarget > 0 ) || 
				     right && (this.horizontalSpeedTarget < 0 ) || 
				     ( this.initialTurnTimer > _SETTING._.initialTurnTimeSpan && this.tutorialCount == 0 ))
				{
					//MyLogger.Red("ChangeDirection "+this.tutorialCount, string.Format("at x pos {0}, {1} next", 
					//	this.transform.localPosition.x, dest ) );
					this.tutorialCount++;
					this.ChangeDirection();
					TutorialMenuEntity.instance.AnimateFinger();
				}
			}
			 
			if ( !UserInfo.instance.gameplayTutorialInitiated)
			{
				if ( this.countTurned == 0 && this.initialTurnTimer > _SETTING._.initialTurnTimeSpanInGame && this.tutorialCount == 0 )
				{
					GameUIManager.instance.AnimateFinger();
					this.tutorialCount++;
				}
			}
			if ( this.countTurned > 1 )
			{

			}

			this.Move();
			this.Accelerate();
			this.ProcessAnimator();
			this.RotateBoard();
			this.RotateDashEffect();

			this.emissionTimer += Time.deltaTime;
			if ( this.emissionTimer > this.emissionTimeSpan && this.directionalSnowEffect != null )
			{
				this.directionalSnowEffect.emissionRate = 0f;
			}
			this.gameTimeSpan += Time.deltaTime;
		}
		else if ( this.fsmInterface.ActiveStateName == "CameraWalk" ) 
		{
			this.OnCameraWalkUpdate();
		}
		else if ( this.fsmInterface.ActiveStateName == "CameraRecovery" ) 
		{

		}

		this.previousState = this.myAnimator.GetCurrentAnimatorStateInfo(0).nameHash;		
	}

	public  void SendEvent (string state)
	{
		//  Logger.Red("SendEvent", state);
		if ( this.fsmInterface == null )
        	this.fsmInterface       = this.GetComponent<PlayMakerFSM>();
		this.fsmInterface.SendEvent(state);
	}

	public  bool IsFsmState(string state)
	{
		return this.fsmInterface.ActiveStateName == state;
	}

	public  bool IsIdle()
	{
		return this.IsState("Idle");
	}

	public  bool IsDead()
	{
		if ( this.isAlive == false || this.myAnimator == null || this.gameObject.activeInHierarchy == false)
			return true;

		if ( this.IsState("Die") 	|| this.IsState("DieIdle") ||  this.myAnimator.GetBool("Die")  )
		{
			return true;
		}
		return false;
	}

	public static void ChangeInstance(string prefabName)
	{
		GamePlayerEntity newPlayer = AbstractObjectPooler.instance.
						GetSngletonObject(prefabName).GetComponent<GamePlayerEntity>();

		if ( _instance != null )
		_instance.DestroySelf();
		_instance = newPlayer;
		newPlayer.isAlive 				= false;
        // newPlayer.myAnimator			= newPlayer.rootAnimation.GetComponent<Animator>();
	}

	private void ShowDiamondObtainEffect(int increment)
	{
        BaseLifeSpanObject disappear    = AbstractObjectPooler.instance.GetIdleObject(
        	"Diamond Disappear").GetComponent<BaseLifeSpanObject>();    
        disappear.gameObject.SetActive(true);
        disappear.transform.position     = this.myTransform.position;
        disappear.transform.localScale   = Vector3.one;
        disappear.Initialize();
        disappear.transform.Find("Label Text").GetComponent<UILabel>().text = "+"+increment;
	}

	private void ShowFollowerObtainEffect()
	{
        BaseLifeSpanObject follower    = AbstractObjectPooler.instance.GetIdleObject(
        	"Follower Obtain").GetComponent<BaseLifeSpanObject>();    
        follower.gameObject.SetActive(true);
        follower.transform.position     = this.myTransform.position;
        follower.transform.localScale   = Vector3.one;
        follower.Initialize();
	}

	public void OnExtreamColliderStay(Collider other)
	{
		if ( this.IsDead() )
		{
			//this.extreamTimer = 0;
		}
		else if ( "GAME_WALL" == other.gameObject.tag ) 
		{	
			this.extreamTimer += Time.deltaTime;
			if ( this.extreamTimer > _SETTING._.extreamTimeSpan )
			{
				this.ShowFollowerObtainEffect();
				this.IncrementScore(1f, true);
				
				this.extreamTimer = 0;
			}
		}
	}

	public void OnEatingColliderEnter(GameObject go)
	{
		if ( this.IsDead() )
		{
		}
		else if ( "GAME_GEM" == go.tag )
		{
			GameGemEntity gem = go.GetComponent<GameGemEntity>();
			gem.tile.DestroyGem(gem);


			UserInfo.SpinBonus 		   spinBonus = UserInfo.instance.spinBonus;
			UserInfo.SpinBonusBuffStat buffStat  = UserInfo.instance.currentBuffStat;

			int increment = 1;//(int)UserInfo.instance.GetBuffEffect(BUFF_TYPE.DIAMOND);
			if ( GameSystemInfo.instance.IsDiamondBoosted() )
				increment = 2;

			increment = increment * buffStat.buffMultiplier;

			Debug.Log("increment: " + increment);

			this.currentDiamond += increment;

			this.ShowDiamondObtainEffect(increment);

			if ( this.currentComboCount > _SETTING._.GetMaxComboScoreAppliedToSpeed() )
			{
				this.IncrementScore(_SETTING._.GetMaxComboScoreAppliedToSpeed()/2f, true);
			}
			else
			{
				this.IncrementScore(this.currentComboCount/2f, true);
			}

			if ( this.IsFsmState("MainAuto") || UserInfo.instance.gameplayTutorialInitiated )
				return;
			MissionController.instance.IncrementCount(MissionType.DIAMOND_TOTAL, increment);
			MissionController.instance.UpdateCount(MissionType.DIAMOND_GAME,  		   this.currentDiamond);

			this.IncrementCombo();

			this.scoreWithoutDiamond = 0;
		}
	}

	private float GetCurrentScoreFactor()
	{
		float log = Mathf.Log10( this.currentScore+1 );
		// MyLogger.Red("GetCurrentScoreFactor", "log :"+log);
		// MyLogger.Red("GetCurrentScoreFactor", "total :"+(_SETTING._.scoreToSpeed - log));
		return _SETTING._.scoreToSpeed - log;
	}

	public void OnDamageColliderEnter(Collider other)
	{
		if ( this.IsDead() )
		{
		}
		else if ( "GAME_WALL" == other.gameObject.tag ) 
		{	
			this.OnDamaged();
			
			if ( "Obstacle_01" == other.gameObject.name && false == UserInfo.instance.gameplayTutorialInitiated )
			UserInfo.instance.obstacleCount++;

			return;
		}
	}

	public float 		GetRadius()
	{
		if ( this.eatingRadius == 0)
		{
			this.eatingRadius = this.GetComponent<SphereCollider>("Eating Collider").radius;
		}
		return this.eatingRadius;
	}

	void Move() 
	{	
		if ( this.myTransform == null )
			this.myTransform = this.GetTransform();

		if ( this.IsFsmState("CameraWalk") )
		{
			float xSpeed 	= Mathf.Lerp( this.horizontalSpeed, 0, this.emissionTimer/_SETTING._.walkingTimeSpan );
			float ySpeed 	= Mathf.Lerp( this.verticalSpeed, 	0, this.emissionTimer/_SETTING._.walkingTimeSpan );

			this.myTransform.position -= Vector3.right * xSpeed * Time.deltaTime * _SETTING._.playerDeadSpeed;
			this.myTransform.position -= Vector3.down  * ySpeed * Time.deltaTime * _SETTING._.playerDeadSpeed;
		}
		else
		{
			this.myTransform.position += Vector3.right * this.horizontalSpeed * Time.deltaTime;
			//MyLogger.Red("Move", "verticalSpeed : "+this.verticalSpeed);
			this.myTransform.position += Vector3.down  * this.verticalSpeed   * Time.deltaTime;
		}
	}

    void OnSpeedChange(float speed)
    {
    	this.horizontalSpeed = speed;
    }

    void OnSlowTransitionFinished()
    {
    	if ( this.startSlowFlag ) 
		{
			this.startSlowFlag 	 = false;
			this.verticalSpeed 	 = _SETTING._.GetVerticalSpeedDefault(this);	
		}
    }
 
	void Accelerate()
	{
		if ( this.IsFsmState("CameraWalk") )
		{
			return;
		}

		this.startSlowTimer  += Time.deltaTime;

		if ( this.startSlowTimer < _SETTING._.startSlowTimeSpan ) 
		{
			this.verticalSpeed 	 = Mathf.Lerp(0,_SETTING._.GetVerticalSpeedDefault(this),
					(this.startSlowTimer / _SETTING._.startSlowTimeSpan));
		}
    	else if ( this.startSlowFlag ) 
		{
			// this.speedSlowFinished = this.horizontalSpeed;
			this.startSlowFlag 	 = false;
			this.verticalSpeed 	 = _SETTING._.GetVerticalSpeedDefault(this);	
		}

		this.elapsedSinceChangeDirection += Time.deltaTime;

		if ( this.initialStraightCourse )
		{

		}
		else if ( this.directionChangeApplied )
		{
			this.accelerateTimer += Time.deltaTime;

			if ( this.accelerateTimer > this.currentAccelerateTimeSpan )
			{
				this.horizontalSpeed 	= this.GetHorizontalSpeedTarget();
			}
			else
			{
				this.horizontalSpeed = Mathf.Lerp(0, this.GetHorizontalSpeedTarget(), 
					this.accelerateTimer/this.currentAccelerateTimeSpan);
			}
		}
		else
		{
			int sign = Mathf.CeilToInt( this.horizontalSpeedTarget / Mathf.Abs(this.horizontalSpeedTarget) );
			if ( this.elapsedSinceChangeDirection < _SETTING._.preventDoubleTap )
			{
				sign = -sign;
			}

			this.horizontalSpeed += _SETTING._.GetDirectionChangeSpeed(this) * sign * Time.deltaTime * this.GetVerticalToHorizontal();
			if ( sign * this.horizontalSpeed > 0 )
			{
				this.accelerateTimer = 0;
				float clamped = (this.horizontalSpeed / 2f);
				this.horizontalSpeed = clamped;//0;
				this.directionChangeApplied = true;
			}
		}
	}

	public  void ChangeDirection()
	{
		if ( this.IsDead() )
			return;

		this.countTurned++;
		this.PlaySkateSound( true );

		
		if ( this.directionChangeApplied == false )
		{
			this.elapsedSinceChangeDirection = 0;
		}
		this.accelerateTimer = 0;
		// this.changeDirectionTimer = 0;

		this.directionChangeApplied		= false;

		if ( this.horizontalSpeedTarget > 0)
		{
			this.directionalSnowEffect = this.transform.Find("Particle Right").GetComponent<ParticleSystem>();
		}
		else
		{
			this.directionalSnowEffect = this.transform.Find("Particle Left").GetComponent<ParticleSystem>();
		}

		this.directionalSnowEffect.emissionRate = 20f;
		this.emissionTimeSpan 	= this.horizontalSpeed / this.horizontalSpeedTarget;
		if ( this.emissionTimeSpan < 0 )
			this.emissionTimeSpan = 0.1f;
		this.emissionTimer 		= 0;

		float speed = this.verticalSpeed - _SETTING._.GetVerticalSpeedDefault(this);
		if ( speed < 0 )
			speed = 0;
		float pow 	= speed / _SETTING._.turnScoreFactor; 
		float score = Mathf.Pow( 2, pow );
		this.IncrementScore( score, false );
		MissionController.instance.UpdateCount(MissionType.SCORE_WITHOUT_DIAMOND,  this.scoreWithoutDiamond);
		// this.scoreWithoutDiamond += score;
		if ( this.initialStraightCourse )// && UserInfo.instance.gameplayTutorialInitiated == false )
		{
			this.initialStraightCourse = false;
			this.currentAccelerateTimeSpan  = _SETTING._.initialAccelerateTimeSpan;
			this.horizontalSpeedTarget 		= _SETTING._.GetHorizontalSpeedTarget(this);
		}
		else
		{
			// int sign = (int)this.horizontalSpeedTarget / (int)Mathf.Abs(this.horizontalSpeedTarget);
			this.horizontalSpeedTarget 		= -this.horizontalSpeedTarget;
			//_SETTING._.horizontalSpeedTargetDefault * -sign;
			this.currentAccelerateTimeSpan  = _SETTING._.accelerateTimeSpan;
		}

		this.ApplyCharacterRotataion();

		if ( !UserInfo.instance.gameplayTutorialInitiated )
		{
			// GameUIManager.instance.HideFinger();
			// MyLogger.Red("ChangeDirection pos"+this.tutorialCount, string.Format("at Y pos {0}", 
			// 	this.transform.localPosition.y) );
			// _SETTING._.tutorialTurnPosition[this.tutorialCount]  	= this.transform.localPosition.y;
			// this.tutorialCount++;
		}

		this.myAnimator.SetTrigger("Turn");
	}

	// public float changeDirectionTimer;

	void RotateDashEffect()
	{
		float sign = this.horizontalSpeed / Mathf.Abs(this.horizontalSpeed);
		float angleProb = Mathf.Abs(this.horizontalSpeed) / this.verticalSpeed;
		float x = Mathf.Lerp( 0, 45f, angleProb * 0.75f );
		// float z = 15f * x / 20f;
		if (!float.IsNaN(x) && !float.IsNaN(sign) )
		{
			if ( this.particleDash != null )
			this.particleDash.transform.rotation 	= Quaternion.Euler(0,0,sign*x);
		}
	}

	void RotateBoard()
	{
		float sign = this.horizontalSpeed / Mathf.Abs(this.horizontalSpeed);
		float angleProb = Mathf.Abs(this.horizontalSpeed) / this.verticalSpeed;
		//float x = Mathf.Lerp( 0, 70f, angleProb * 0.75f );
		float z = Mathf.Lerp( 0, 50f, angleProb );
		float x = Mathf.Lerp( 45f, 65f, angleProb );
		if ( !float.IsNaN(z) && !float.IsNaN(sign) )
		{
			this.skateBoard.rotation 	= Quaternion.Euler(x,0,sign*z);
			// this.skiRight.rotation 	= Quaternion.Euler(sign*x,0,sign*z);
		}
	}

	void ResetRotation()
	{
		if ( this.skateBoard != null )
		{
			this.skateBoard.rotation 	= Quaternion.Euler(45,0,0);
		}
		if ( this.particleDash != null )
		{
			this.particleDash.transform.rotation = Quaternion.Euler(0,0,0);
		}
	}

	void OnEnable()
	{
		if ( this.horizontalSpeedTarget > 0)
		{
			this.horizontalSpeedTarget = -this.horizontalSpeedTarget;
			this.ApplyCharacterRotataion();
		}
		this.ResetRotation();

		if ( this.particleDead != null )
        this.particleDead.SetActive(false);
	}

	public  void ResetCombo()
	{
		if ( this.currentComboCount != 0 )
		{
			this.currentComboCount = 0;
			GameUIManager.instance.ResetCombo();
		}
	}

	void IncrementCombo()
	{
		int increment = 1;
		this.currentComboCount += increment;
		if ( this.currentComboCount > this.maxCombo )
		{
			this.maxCombo = this.currentComboCount;
		}
		GameUIManager.instance.UpdateCombo(this.currentComboCount);

		// MissionController.instance.IncrementCount(MissionType.COMBO_TOTAL, increment);
		MissionController.instance.UpdateCount(MissionType.COMBO_GAME,  (int)this.currentComboCount);

		if ( this.verticalSpeed >= _SETTING._.dashEffectVerticalSpeed )
		{
	        if ( this.particleDash != null )
	        {
	        	this.particleDash.SetActive(true);
	        }
		}
	} 

	void OnProcessScore(float score)
	{
		this.currentScore = score;
		if ( score > UserInfo.instance.bestScore && UserInfo.instance.bestScore > 0 &&  
			 UserInfo.instance.gameplayTutorialInitiated == false && this.IsFsmState("MainAuto") == false )
		{
			GameUIManager.instance.ShowNewRecord();
		}
	}


	void IncrementScore(float score, bool applySpeed)
	{
		float increment  = score;// * UserInfo.instance.GetBuffEffect(BUFF_TYPE.SCORE);

		this.targetScore 	+= increment;
		float target 		= this.targetScore;
		float timeSpan 		= Mathf.Lerp( 0,_SETTING._.scoreTweenTimeSpan, increment/10f ); 

		LeanTween.cancel( this.gameObject, this.tweenId );
		this.tweenId = LeanTween.value ( this.gameObject, this.OnProcessScore, 
            this.currentScore,
            target, timeSpan).id;

		this.scoreWithoutDiamond += increment;
		if ( applySpeed == false)
		{
			score = 1f;
		}

		if ( TileController.instance.IsEnvironmentChangable(target) )// ( (this.targetScore % condition) + increment ) > condition )
		{
			TileController.instance.ChangeEnvironment();
		}
	
		if ( this.IsFsmState("MainAuto") || UserInfo.instance.gameplayTutorialInitiated )
			return;

		MissionController.instance.IncrementCount(MissionType.SCORE_TOTAL, increment);
		MissionController.instance.UpdateCount(MissionType.SCORE_GAME,     target);

		this.IncrementSpeed(score);
	}

	void IncrementSpeed(float speedDefault)
	{	
		float speedIncrement = speedDefault * this.GetCurrentScoreFactor();
		this.verticalSpeed 	+= speedIncrement;
		//MyLogger.Red("IncrementScore", "verticalSpeed : "+this.verticalSpeed);
	}

	private float GetHorizontalSpeedTarget()
	{
		float alpha = Mathf.Lerp( 0, this.horizontalSpeedTarget, this.startSlowTimer / _SETTING._.startSlowTimeSpan );
		return alpha * this.GetVerticalToHorizontal();
	}

	private float GetVerticalToHorizontal()
	{
		float ratio = this.verticalSpeed/_SETTING._.GetVerticalSpeedDefault(this);

		if ( ratio > 1f )
		{
			float res = 1f + ( (ratio-1f) * _SETTING._.verticalToHorizontalByMultiply );
			return res;
		}
		else 
		{
			if ( ratio > _SETTING._.verticalToHorizontalByMultiply )
				ratio = _SETTING._.verticalToHorizontalByMultiply;

			return ratio;
		}
	}

	private void ApplyCharacterRotataion()
	{
		if ( this.rootAnimation == null )
			return;

		if ( this.horizontalSpeedTarget > 0)
		{
			this.rootAnimation.rotation = Quaternion.Euler(0, 180, 0);
		}
		else
		{
			this.rootAnimation.rotation = Quaternion.identity;
		}
	}

	bool turn2;
	bool beforeTurn2;
	private void PlaySkateSound( bool dontPlayThreeInARow )
	{
		if ( beforeTurn2 && turn2 && dontPlayThreeInARow )
		{
			SoundController.instance.Play( "Turn", true);
			beforeTurn2 = turn2;
			turn2 = false;
		}
		else if ( (!beforeTurn2 && !turn2) && dontPlayThreeInARow )
		{
			SoundController.instance.Play( "Turn 02", true);
			beforeTurn2 = turn2;
			turn2 = true;
		}
		else if ( Random.Range (0,100) >= 50 )
		{
			SoundController.instance.Play( "Turn", true);
			beforeTurn2 = turn2;
			turn2 = false;
		}
		else
		{
			SoundController.instance.Play( "Turn 02", true);
			beforeTurn2 = turn2;
			turn2 = true;
		}
	}

	public float GetVelocity()
	{
		return Mathf.Sqrt ( (this.verticalSpeed * this.verticalSpeed) + (this.horizontalSpeed * this.horizontalSpeed) );
	}


	public 				int 						maxCombo;
	private 			int 						tweenId;
	public 				float 						currentAccelerateTimeSpan;

	private 			Camera 						mainCamera;
	private 			int 						previousState;
	private 			Transform 					myTransform;
	[SerializeField]  			float 						accelerateTimer;	
	private 			float 						startSlowTimer;
	[SerializeField] 			bool 						directionChangeApplied;
	private 			bool 						startSlowFlag;
	private 			ParticleSystem				directionalSnowEffect;	
	private 			Transform					skateBoard;		
	private				GameObject 					particleDead;

	public	 			ObscuredFloat 				currentScore;			//ui
	public	 			ObscuredFloat 				targetScore;
	private 			float 						scoreWithoutDiamond;
	private 			SphereCollider 				sphere;
	[HideInInspector]
	public  			Animator 					myAnimator;
	private 			PlayMakerFSM 				fsmInterface;

	private 			Transform 					rootAnimation;
	private 		 	float 						emissionTimer;
	private 			TrailRenderer 				trail;
	private 			TrailRenderer 				trail2;

	public 				ObscuredInt	 				currentComboCount;
	public 				ObscuredInt 				currentDiamond;
	public 				float 						eatingRadius = 0f;
	public 				float 						emissionTimeSpan;			// 1f	

	public float elapsedSinceChangeDirection;	
}