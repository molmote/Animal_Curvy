using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PausePopupEntity : BaseTransitionPopupEntity
{

	protected override void Awake()
	{
        base.Awake();
	}

    public      void    OnToggleDash(GameObject go)
    {
    }

    public      void    Initialize(MissionSaveInfo missionInfo)
    {    
    }
    
    // being used in animation event
    public void InvokeRestart()
    {
    }

}
