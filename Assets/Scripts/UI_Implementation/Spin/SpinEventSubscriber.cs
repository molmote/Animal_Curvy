using UnityEngine;
using System.Collections;

namespace Waggle
{

public class SpinEventSubscriber : MonoBehaviour 
{
	private void Awake()
	{
		EventManager.Subscribe<SpinBigWheelEvent>(OnSpinBigWheel);
	}

	private void OnEnable()
	{
		gameObject.GetComponent<BoxCollider>().enabled = true;
	}

	private void OnDestroy()
	{
		EventManager.UnSubscribe<SpinBigWheelEvent>(OnSpinBigWheel);
	}

	private void OnSpinBigWheel(SpinBigWheelEvent evt)
	{
		Debug.Log("OnSpinBigWheel: " + gameObject.name);
		gameObject.GetComponent<BoxCollider>().enabled = false;
	}
}

}