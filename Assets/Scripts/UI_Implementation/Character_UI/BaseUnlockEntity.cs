using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseUnlockEntity : BaseWidgetEntity
{			
 	private 		Transform 			unlocked;
	private 		Transform 			locked;

	public 		void 	Unlock()
	{		
		this.unlocked.gameObject.SetActive(true);
		this.locked.gameObject.SetActive(false);
	}

	public 		void    Lock()
	{
		this.unlocked.gameObject.SetActive(false);
		this.locked.gameObject.SetActive(true);
	}

	public 		void  	Initialize(bool unlocked)
	{
        this.unlocked           = this.GetComponent<Transform>("Unlock");
	    this.locked           	= this.GetComponent<Transform>("Lock");

		if ( unlocked )
		{
			this.Unlock();
		}
		else
		{
			this.Lock();
		}
	}
}