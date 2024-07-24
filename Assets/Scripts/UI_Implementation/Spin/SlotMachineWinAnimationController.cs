using UnityEngine;
using System.Collections;

namespace Waggle
{

public class SlotMachineWinAnimationController : MonoBehaviour 
{
	public GameObject buttonTake;
	public GameObject overlap;

	private void Awake()
	{

	}

	private void OnDestroy()
	{

	}

	private void OnEnable()
	{
		buttonTake.SetActive(false);
		overlap.SetActive(false);
	}

	private void OnDisable()
	{

	}

	public void OnSlotMachineStopped(int target)
	{
		int buffSpinCount = UserInfo.instance.currentBuffStat.buffSpinCount;
		gameObject.GetComponent<Animation>().Play("Spin Result Win");
		if (buffSpinCount > 0)
		{
			overlap.SetActive(true);
		}
		else
		{
			buttonTake.SetActive(true);
		}
	}
}

}