using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Waggle
{

public class SlotMachine : MonoBehaviour 
{
	public List<GameObject> symbols = new List<GameObject>();

	public float spinTime;
	public int 	 spinCount;

	public bool  spinning;
	public float spinDistance;
	public float symbolSize;

	public float totalDistance;

	public float distance;

	public float normalizedSpinningTime;
	public float normalizedStopTime;

	public float beginTime;
	public float endTime;

	public AnimationCurve curve;

	public int target;
	public int prevTarget;


	public void Awake()
	{
		for (int count = 0; count < transform.childCount; ++count)
		{
			symbols.Add(transform.GetChild(count).gameObject);
			symbols[count].GetComponentInChildren<UILabel>().text = string.Format("FOR ({0}x) GAMES", (count+1));
		}
		spinDistance = symbolSize * symbols.Count;
	}


	public void Update()
	{
		if (spinning)
		{
			if (Time.time < endTime)
			{
				float normalizedTime  = (endTime - Time.time) / spinTime;
				float normalizedValue = curve.Evaluate(1.0f - normalizedTime);
				AddPosition((totalDistance * normalizedValue) - distance);
				distance = totalDistance * normalizedValue;
			}
			else
			{
				AddPosition(totalDistance - distance);
				gameObject.SendMessageUpwards("OnSlotMachineStopped", target, SendMessageOptions.DontRequireReceiver);
				spinning = false;
			}
		}
	}

	public void Spin()
	{
		spinning = true;
		beginTime = Time.time;
		endTime   = beginTime + spinTime;
		distance  = 0.0f;
		totalDistance = spinCount * spinDistance + (prevTarget - target) * symbolSize;
		prevTarget 	  = target;

	}

	public void Stop()
	{
		
	}

	public void SetTarget(int target)
	{
		this.target = target;
	}

	public void AddPosition(float delta)
	{
		for (int count = 0; count < transform.childCount; ++count)
		{
			symbols[count].transform.localPosition -= new Vector3(0, delta, 0);
		}
	}
}

}