using UnityEngine;
using System.Collections;

public class StaticController : MonoBehaviour {

	// Use this for initialization
	void Awake () 
	{
		DontDestroyOnLoad(gameObject);

#if ANDROID_BUILD
		gameObject.AddComponent<AndroidKeyEventController>();
#endif

	}
	
	// Update is called once per frame
	void Update () 
	{

		//! 
		//NetworkRequestController.instance.OnUpdate(this);

		//! Game play time update
		GameSystemInfo.instance.UpdateTotalApplicationRuntime();		
	}


	void OnDestroy()
	{
		GameSystemInfo.instance.Save();
	}
}
