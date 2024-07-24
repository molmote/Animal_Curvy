using UnityEngine;
using System.Collections;

namespace Waggle
{

public class BigWheelController : MonoBehaviour 
{
	public float spinTime;
	public int spinCount;

	public bool  spinning;
	public float spinAngle;
	

	public float normalizedSpinningTime;
	public float normalizedStopTime;

	public float beginTime;
	public float endTime;

	public AnimationCurve curve;

	public int target;

	private void OnEnable()
	{
		target = 0;
		transform.eulerAngles = Vector3.zero;
	}

	public void Update()
	{
		if (spinning)
		{
			if (Time.time < endTime)
			{
				float normalizedTime  = (endTime - Time.time) / spinTime;
				float normalizedValue = curve.Evaluate(1.0f - normalizedTime);
				transform.eulerAngles = Vector3.forward * spinAngle * normalizedValue;
			}
			else
			{
				spinning = false;
				transform.eulerAngles = Vector3.forward * spinAngle;
				OnStop();
			}
		}
	}

	public void SetTarget(int target)
	{
		this.target = target;
	}

	public void Spin()
	{
		beginTime = Time.time;
		endTime   = beginTime + spinTime;
		spinAngle = spinCount * 360.0f + target * 45.0f - 22.5f;

		spinning = true;
	}

	public void OnStop()
	{
		gameObject.SendMessage("OnWheelStop", target, SendMessageOptions.DontRequireReceiver);
	}
}

}