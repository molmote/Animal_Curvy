using UnityEngine;
using System.Collections;

public class TutorialManager {

	private static TutorialManager _instance = new TutorialManager();
	public  static TutorialManager instance
	{
		get
		{
			return _instance;
		}
	}

	private bool isCallbackAlreadyCalled = false;

	public  delegate void OnTutorialFinishDelegate();
	public  OnTutorialFinishDelegate OnTutorialFinishCallback;

	public void CreateTutorial(string path, Transform _parent, OnTutorialFinishDelegate OnTutorialFinishCallback = null)
	{
		isCallbackAlreadyCalled = false;
		GameObject tutorial = Creater.Create(path);
		tutorial.SetActive(true);
		tutorial.transform.parent = _parent;
		tutorial.transform.localScale 		= Vector3.one;
		tutorial.transform.localPosition 	= Vector3.zero;

		this.OnTutorialFinishCallback = OnTutorialFinishCallback;
	}

	public void OnDestroyTutorial()
	{		
		if(false == isCallbackAlreadyCalled)
		{
	        //Me.instance.currentTutorial ++;
	        //DataSaver.instance.SaveUserData();
		}
		
		if(OnTutorialFinishCallback != null)
		{	
			OnTutorialFinishCallback();
			this.OnTutorialFinishCallback = null;
			isCallbackAlreadyCalled = true;
		}


		// every tutorial is finished
		// if(Me.instance.currentTutorial > TutorialStatus.COMPLETED)
	}
}
