using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	private Transform camTransform;
	
	// How long the object should shake for.
	public float shake = 0.1f;
	private float shakeInside = 0f;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	
	Vector3 originalPos;
	
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = camTransform.localPosition;
		shakeInside = shake;
	}
	
	void Update()
	{
		if (shakeInside > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
			
			shakeInside -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeInside = 0f;
			camTransform.localPosition = originalPos;
			this.enabled = false;
		}
	}
}