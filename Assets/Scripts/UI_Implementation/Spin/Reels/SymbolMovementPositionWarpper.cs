using UnityEngine;
using System.Collections;

namespace Waggle
{

public class SymbolMovementPositionWarpper : MonoBehaviour 
{
	public Vector3 	startPosition = new Vector3(0,0,0);
	public float 	deadLine;
	public System.Action onChanged = null;

	private void OnDestroy()
	{
		ClearChangedCallback();
	}

	public void Update()
	{
		Repeat();
	}

	private void Repeat()
	{
		float distance 		= Vector3.Distance(startPosition, transform.localPosition);
		if (Mathf.Epsilon + distance > deadLine)
		{
			Vector3 target = (transform.localPosition - startPosition).normalized * -(deadLine + 80.0f);

			Debug.Log("target: " + target);

			transform.localPosition += target;
			if (onChanged != null)
			{
				onChanged();
			}
		}
	}

	public void AddChangedCallback(System.Action onChanged)
	{
		this.onChanged += onChanged;
	}

	public void RemoveChangedCallback(System.Action onChanged)
	{
		this.onChanged -= onChanged;
	}

	public void ClearChangedCallback()
	{
		this.onChanged = null;
	}
}

}