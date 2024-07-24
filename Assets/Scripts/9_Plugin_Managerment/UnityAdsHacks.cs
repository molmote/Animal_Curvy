
using UnityEngine;
using System.Collections;

public class UnityAdsHacks : MonoBehaviour 
{
	public bool usePauseOverride;

	protected static bool _isPaused;

	/*protected void OnApplicationFocus( bool isPaused )
	{
		if ( !isPaused )
		{
			//AdColony.pause();
			MyLogger.Red("OnApplicationFocus", "Pause");
			GameInfo.instance.Pause();
		}
		else
		{
			MyLogger.Red("OnApplicationFocus", "Resume");
			//AdColony.resume(this);	
			GameInfo.instance.Resume();
		}
	}*/
	
	protected void OnApplicationPause (bool isPaused)
	{
		if ( isPaused )
		{
			//AdColony.pause();
		}
		else
		{
			//AdColony.resume(this);	
		}		
		
		if (!usePauseOverride || isPaused == _isPaused) return;
		
		if (isPaused) MyLogger.Log ("App was paused.");
		else MyLogger.Log("App was resumed.");
		
		if (usePauseOverride) PauseOverride(isPaused);
	}

	public static void PauseOverride (bool pause)
	{
		if (pause) MyLogger.Log("Pause game while ad is shown.");
		else MyLogger.Log("Resume game after ad is closed.");
		
		if ( pause )
		{
			AudioListener.volume = 0f;	
		}
		else if ( GameSystemInfo.instance.isSoundEnabled )
		{
			AudioListener.volume = 1f;		
		}
		Time.timeScale = pause ? 0f : 1f;
		
		_isPaused = pause;
	}
}