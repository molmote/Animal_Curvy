using UnityEngine;
using System.Collections;


namespace Waggle
{

public class MainUIEventSubscriber : MonoBehaviour 
{
	private void Awake()
	{
		EventManager.Subscribe<MainUIEnableEvent>(OnMainUIEnable);
	}

	private void OnDestroy()
	{
		EventManager.UnSubscribe<MainUIEnableEvent>(OnMainUIEnable);
	}

	private void OnMainUIEnable(MainUIEnableEvent evt)
	{
		gameObject.SetActive(evt.isEnabled);
	    GamePlayerEntity.instance.gameObject.SetActive(evt.isEnabled);
        if (evt.isEnabled)
        {
	        TileController.instance.OnRestart();
	    }
	}
}

}