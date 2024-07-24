using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GameDataEditor;

public class SpinPieceEntity : BaseWidgetEntity
{
    public  GDEspinListData 	    serialized;
    public  Transform 				reward;
    public  Transform 				rewardLucky;

    public  Transform 				original;
    public  Transform 				allUnlocked;

    public void OnResetPiece()
    {
    	this.reward.gameObject.SetActive(true);
    	if ( this.rewardLucky != null )
    	this.rewardLucky.gameObject.SetActive(false);

    }

    public void OnPieceNotAvailable()
    {
    	this.original.gameObject.SetActive(false);
    	this.allUnlocked.gameObject.SetActive(true);
    }
}