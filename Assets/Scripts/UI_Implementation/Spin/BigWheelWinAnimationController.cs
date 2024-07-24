using UnityEngine;
using System.Collections;

namespace Waggle
{

public class BigWheelWinAnimationController : MonoBehaviour 
{
	public GameObject[] segments;

	public void OnWheelStop(int target)
	{
		segments[target].GetComponent<Animation>().Play();
	}
}

}