using UnityEngine;
using System.Collections;

namespace Waggle
{

public class Spring : MonoBehaviour 
{	
	// anchor position in model space (local space)
	public Vector3 anchoredPosition;

	// Rest length and spring constant
	public float len;
	public float k = 0.2f;

	// Calculate spring force
	public Vector3 Connect(Vector3 position) 
	{
		// Vector pointing from anchor.localPosition to bob transform.localPosition
		Vector3 force = position - anchoredPosition;
		// What is distance
		float d = force.magnitude; 
		// Stretch is difference between current distance and rest length
		float stretch = d - len;

		// Calculate force according to Hooke's Law
		// F = k * stretch
		force = force.normalized * (-1 * k * stretch);
		
		return force; // b.applyForce(force);
	}
}

}