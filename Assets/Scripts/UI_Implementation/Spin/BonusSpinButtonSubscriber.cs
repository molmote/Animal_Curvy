using UnityEngine;
using System.Collections;

namespace Waggle
{

public class BonusSpinButtonSubscriber : MonoBehaviour 
{
	public GameObject activeObject;

	public GameObject deactiveObject;
	public UILabel 	  timeLabel;

	public UserInfo.SpinBonus spinBonus;

	public void Awake()
	{
		// EventManager.Subscribe<UpdateBonusSpinCountEvent>(OnUpdateBonusSpinCount);
	}

	public void OnDestroy()
	{
		// EventManager.UnSubscribe<UpdateBonusSpinCountEvent>(OnUpdateBonusSpinCount);
	}

	public void OnEnable()
	{
		CheckAndActivate();
	}

	public void Start()
	{
			
	}

	private void Update()
	{
		if (deactiveObject.activeSelf)
		{
			UserInfo.SpinBonus spinBonus = UserInfo.instance.spinBonus;
			if (spinBonus.spinBonusActivate)
			{

			}
			else
			{
				if (spinBonus.CheckSpinTime())
				{
					CheckAndActivate();
					spinBonus.UpdateBonusTime();
				}
				else
				{
					timeLabel.text = TimeFormatUtility.TimeFormat(spinBonus.GetRemainSeconds());
				}
			}
		}
	}

	private void CheckAndActivate()
	{
		UserInfo.SpinBonus spinBonus = UserInfo.instance.spinBonus;
		if (spinBonus.spinBonusActivate)
		{
			activeObject.SetActive(true);
			deactiveObject.SetActive(false);
		}
		else
		{
			activeObject.SetActive(false);
			deactiveObject.SetActive(true);			
		}
	}

	public void OnSpinButtonClick()
	{
		UserInfo.SpinBonus spinBonus = UserInfo.instance.spinBonus;
		if (spinBonus.spinBonusActivate)
		{
			spinBonus.spinBonusActivate = false;
			EventManager.TriggerEvent(new BonusSpinButtonClickEvent());
			EventManager.TriggerEvent(new MainUIEnableEvent(false));	
		}
		else
		{

		}
	}
}

}