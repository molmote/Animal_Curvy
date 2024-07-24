using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

// 기획 상에서 정리 되지 않은(Undefined) 각종 변수들을 인스펙터에서 설정
public class _SETTING : MonoBehaviour {
    
	private static _SETTING _instance = null;
	// private static _SETTING _ = null;
	public static _SETTING instance 
	{
		get
		{
			if ( _instance == null )
			{
				_instance = GameObject.Find("Game Setting").GetComponent<_SETTING>();
			}
			return _instance;
		}
	}

	public static _SETTING _ 
	{
		get
		{
			if ( _instance == null )
			{
				_instance = GameObject.Find("Game Setting").GetComponent<_SETTING>();
			}
			return _instance;
		}
	}

	//public void SetInstance(_SETTING init)
	void Awake()
	{
		_instance = this;
	}

	void Update()
	{
		this.time = GamePlayerEntity.instance.gameTimeSpan;
		this.pos  = GamePlayerEntity.instance.GetTransform().localPosition;
	}

	/* ENEMY GLOBAL INFORMATION */
	//    public  float      			initialDistanceFromFirstMonster;

    /* Game System Information  */
    public  			float 						tileGemProbablity;

    public  			float 						initialAccelerateTimeSpan; 	// 2f
	public  			float 						accelerateTimeSpan;		 	// 1f
	public 				float 						verticalToHorizontalByMultiply;	//.75f

	public 				float 						scoreTweenTimeSpan;			// 1f
	public				float 						scoreToSpeed;				// 4f
	public 				float 						playerDeadSpeed;			// 0.5f
	public 				float 		 				recoveryTimeSpan;			// 0.5f

	public 				float 						walkingTimeSpan; 	// 0.3f
	public 				LeanTweenType 				inDeadTweenType; 	//easyoutquad
	public 				LeanTweenType 				outDeadTweenType;	//easyinoutquad
	public 				float 						extreamTimeSpan 	= 0.1f;
	[SerializeField] 	ObscuredFloat 				maxComboScoreAppliedToSpeed;

	public 				float 						colorChangeTimeDelay;
	public 				float 						colorChangeTimeSpan;
	public 				List<float> 				colorChnageScore;
	public 				float 						dashEffectVerticalSpeed;

	public 				float 						startSlowTimeSpan; 	// 3f
	public 				float 						turnScoreFactor; 	// 100

	public 				float 						time;
	public 				float 						initialTurnTimeSpan;
	public 				float 						initialTurnTimeSpanInGame;
	public 				Vector3 					pos;
	public 				List<float> 				tutorialTurnPosition;
	public 				int 						tutorialTileIndex; 	// 3

	public 				Vector3 					magneticDistance; 	// 250f
	public 				Range2 						magneticSpeed;
	public 				float 						magneticMinDistance;

	public 				float 						obstacleProbablityInPercent;
	public 				float 						playerXBoundaryInMain;
	public 				float 						horizontalSpeedTargetDefault;		// 2500
	public 				float 						horizontalSpeedTargetAuto;		// < 2500

	public 				float 						verticalSpeedDefault; 		// 700
	public 				float 						verticalSpeedAuto;		// < 2500

	public 				float 						directionChangeSpeedDefault;		// 2500
	public 				float 						directionChangeSpeedAuto;		// < 2500

	[SerializeField] 	ObscuredFloat 				minFreeGiftimumDiamond;
	[SerializeField] 	ObscuredFloat 				maxFreeGiftimumDiamond;

	public int GetFreeGiftDiamond()
	{
		return 10 * (int)(Random.Range(minFreeGiftimumDiamond,maxFreeGiftimumDiamond) / 10);
	}

	public float GetMaxComboScoreAppliedToSpeed()
	{
		return this.maxComboScoreAppliedToSpeed;
	}

	public float GetMagneticDistance()
	{
		int index = (int) UserInfo.instance.GetCharacterInfo().size;
		return this.magneticDistance[index];
	}

	public float GetVerticalSpeedDefault( GamePlayerEntity player )
	{
		if ( player.IsFsmState("MainAuto") )
			return this.verticalSpeedAuto;

		return this.verticalSpeedDefault;
	}

	public float GetDirectionChangeSpeed( GamePlayerEntity player )
	{
		if ( player.IsFsmState("MainAuto") )
			return this.directionChangeSpeedAuto;

		return this.directionChangeSpeedDefault;
	}

	public float GetHorizontalSpeedTarget( GamePlayerEntity player )
	{
		if ( player.IsFsmState("MainAuto") )
			return this.horizontalSpeedTargetAuto;

		return this.horizontalSpeedTargetDefault;
	}

	public 				List<ObscuredInt> 	freeGiftProbs;
	public 				Range2 				dangerDelayRange;

	[HideInInspector]
	public 				float 				preventDoubleTap;
}

[System.Serializable]
public class tutorialSave
{
	public 	float 	pos;
	public 	bool  	twoTurn;
}
