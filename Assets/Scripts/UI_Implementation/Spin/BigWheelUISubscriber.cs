using UnityEngine;
using System.Collections;

namespace Waggle
{

public class BigWheelUISubscriber : MonoBehaviour 
{
	public  UILabel 	freeSpinCount;
	public  Transform	spinAnchorTransform;

	// active spin button or deactive timer
	public 	GameObject 	activeSpinButton;
	public 	GameObject 	deactiveTimer;

	public  UILabel 	timeLabel;

	public  BigWheelController bigWheelController;


	// slot machine
	public  GameObject 	slotMachineRootObject;
	public 	SlotMachine slotMachine;

	public 	float 		slotMachineSpindelay;

	private void Awake()
	{
		EventManager.Subscribe<UpdateBuffCountEvent>(OnUpdateBuffCount);
		EventManager.Subscribe<BonusSpinButtonClickEvent>(OnBonusSpinButtonClick);

		spinAnchorTransform.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		EventManager.UnSubscribe<UpdateBuffCountEvent>(OnUpdateBuffCount);
		EventManager.UnSubscribe<BonusSpinButtonClickEvent>(OnBonusSpinButtonClick);
	}


	private void OnEnable()
	{
		Debug.Log(UserInfo.instance.GetCurrentDiamond() + " " + GamePlayerEntity.instance.currentDiamond);
		UserInfo.SpinBonus spinBonus = UserInfo.instance.spinBonus;
		
		if (spinBonus.bonusSpinCount > 0)
		{
			activeSpinButton.SetActive(true);
			deactiveTimer.SetActive(false);
		}
		else
		{
			activeSpinButton.SetActive(false);
			deactiveTimer.SetActive(true);
		}

		slotMachineRootObject.SetActive(false);
	}

	private void Update()
	{
		if (deactiveTimer.activeSelf)
		{
			UserInfo.SpinBonus spinBonus = UserInfo.instance.spinBonus;
			timeLabel.text = TimeFormatUtility.TimeFormat(spinBonus.GetRemainSeconds());
		}
	}


	private void OnUpdateBuffCount(UpdateBuffCountEvent evt)
	{
		// buffCount.text = "Remain: " + System.Convert.ToString(evt.buffSpinCount);
	}

	private void OnBonusSpinButtonClick(BonusSpinButtonClickEvent evt)
	{
		spinAnchorTransform.gameObject.SetActive(true);
	}

	public void OnBackButtonClick()
	{
		EventManager.TriggerEvent(new MainUIEnableEvent(true));
		gameObject.SetActive(false);
	}

	public bool isBonusSpinUnlock = false;

	public int debugGeneratedTarget = 0;
	public int debugBuffSpinCount 	= 1;

	public int genBuffSpinTarget = 0;
	public int genSpinCount 	 = 0;

	// 0
	// 1 3 5 7
	// 2 6
	// 4
	private int GetRandomMultiplier()
	{
		return UnityEngine.Random.Range(0,6);
	}

	private int GetSpinCount(int multiplier)
	{
		int spinCount = 0;
		if (multiplier == 1 || multiplier == 3 || multiplier == 5 || multiplier == 7)
		{
			spinCount = UnityEngine.Random.Range(2,4);
		}
		else if (multiplier == 2 || multiplier == 6)
		{
			spinCount = UnityEngine.Random.Range(0,2);	
		}
		else if (multiplier == 4)
		{
			spinCount = UnityEngine.Random.Range(0,1);
		}
		return spinCount;
	}

	public void OnSpinButtonClick()
	{
		EventManager.TriggerEvent(new SpinBigWheelEvent());
		activeSpinButton.SetActive(false);

		UserInfo.SpinBonus 			spinBonus = UserInfo.instance.spinBonus;
		UserInfo.SpinBonusBuffStat 	buffStat  = UserInfo.instance.currentBuffStat;

		int generatedTarget = GetRandomMultiplier();
		int buffSpinCount   = GetSpinCount(generatedTarget);

		SetMultiplier(generatedTarget);
		SetBuffSpinCount(buffSpinCount);

		bigWheelController.SetTarget(generatedTarget);
		bigWheelController.Spin();
	}

	public void OnBuffTakeButtonClick()
	{		
		// code here for not take buff
		UserInfo.instance.ApplyBuffStat();
		OnBackButtonClick();
	}

	public void OnBuffNotTakeButtonClick()
	{
		// code here for not take buff
		UserInfo.instance.MakeLastBuffStatEmpty();
		OnBackButtonClick();
	}

	public void Spin()
	{
		// UserInfo.instance.Save();
		slotMachine.SetTarget(UserInfo.instance.lastEarnedBuffStat.buffSpinCount-1);
		slotMachine.Spin();
	}

	public void OnBigWheelBonusFinished(GameObject gameObj)
	{
		if (genBuffSpinTarget == 0)
		{
			int index = UserInfo.instance.GetAvailableRandomCharacterIndex();
			// OnBackButtonClick();
			UnlockMenuEntity.instance.onBonusSpinUnlock = OnBackButtonClick;
			UnlockMenuEntity.instance.isBonusSpinUnlock = true;
			UnlockMenuEntity.instance.Show( index );
			// ResultPopupEntity.instance.ShowUnlockUI();
		}
		else
		{
			slotMachineRootObject.SetActive(true);
			Invoke("Spin", slotMachineSpindelay);
		}
	}

	public void OnSlotMachineBonusFinishded(GameObject gameObj)
	{

	}
	// 0
	// 1 3 5 7
	// 2 6
	// 4

	private void SetMultiplier(int target)
	{
		genBuffSpinTarget = target;
		int multiplier = 1;
		if  	(target == 1 || target == 3 || target == 5 || target == 7)
		{
			multiplier = 2;
		}
		else if (target == 2 || target == 6)
		{
			multiplier = 4;
		}
		else if (target == 4)
		{
			multiplier = 10;
		}
		UserInfo.instance.lastEarnedBuffStat.buffMultiplier = multiplier;
	}

	private void SetBuffSpinCount(int buffSpinCount)
	{
		genSpinCount = buffSpinCount;
		UserInfo.instance.lastEarnedBuffStat.buffSpinCount = buffSpinCount+1;
	}
}

}