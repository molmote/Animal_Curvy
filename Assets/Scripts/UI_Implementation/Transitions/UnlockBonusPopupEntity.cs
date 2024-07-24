using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnlockBonusPopupEntity : MyPopupEntity 
{
    /*
    private static UnlockBonusPopupEntity _instance = null;
    public static UnlockBonusPopupEntity instance 
    {
        get
        {
            return _instance;
        }
    }

	// Use this for initialization
	protected override void Awake()
	{
        _instance = this;
        base.Awake();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
        
        this.AddOnClickListener( this.gameObject, OnExitClick );
        this.unlockEffect = this.GetComponent<Transform>("Particle");
	}

    private void UpdateUnlockedStatus()
    {
        this.unlockEffect.gameObject.SetActive(UserInfo.instance.isUnlockBonusGivenRecently);
        if ( this.unlockBonusList == null )
        {
            this.unlockBonusList    = new List<BaseUnlockEntity>();
            for ( int i = 0 ; i < 6 ; i++ )
            {
                BaseUnlockEntity unlock = this.GetComponent<BaseUnlockEntity> (string.Format( "{0:d2}", i+1 ) );
                unlock.Initialize(false);
                this.unlockBonusList.Add ( unlock );
            }
            this.labelUnlockedCount = this.GetComponent<UILabel>("Label Animal Number");
        }
        int countUnlocked = UserInfo.instance.GetUnlockedCharacterCount();
        this.labelUnlockedCount.text = ""+countUnlocked;

        if ( countUnlocked >= 70 )
        {
            this.unlockBonusList[5].Unlock();
        }
        if ( countUnlocked >= 60 )
        {
            this.unlockBonusList[4].Unlock();
        }
        if ( countUnlocked >= 50 )
        {
            this.unlockBonusList[3].Unlock();
        }
        if ( countUnlocked >= 40 )
        {
            this.unlockBonusList[2].Unlock();
        }
        if ( countUnlocked >= 30 )
        {
            this.unlockBonusList[1].Unlock();
        }
        if ( countUnlocked >= 15 )
        {
            this.unlockBonusList[0].Unlock();
        }
        if ( UserInfo.instance.isUnlockBonusGivenRecently )
        {
            if ( countUnlocked >= 70 )
            {
                this.unlockBonusList[5].Lock();
                this.unlockBonusList[5].GetComponent<Animation>().Play();
            }
            else if ( countUnlocked >= 60 )
            {
                this.unlockBonusList[4].Lock();
                this.unlockBonusList[4].GetComponent<Animation>().Play();
            }
            else if ( countUnlocked >= 50 )
            {
                this.unlockBonusList[3].Lock();
                this.unlockBonusList[3].GetComponent<Animation>().Play();
            }
            else if ( countUnlocked >= 40 )
            {
                this.unlockBonusList[2].Lock();
                this.unlockBonusList[2].GetComponent<Animation>().Play();
                UserInfo.instance.AddSpin(1);
                UserInfo.instance.Save();
            }
            else if ( countUnlocked >= 30 )
            {
                this.unlockBonusList[1].Lock();
                this.unlockBonusList[1].GetComponent<Animation>().Play();
            }
            else if ( countUnlocked >= 15 )
            {
                this.unlockBonusList[0].Lock();
                this.unlockBonusList[0].GetComponent<Animation>().Play();
            }
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        this.UpdateUnlockedStatus();
    }

    private List<BaseUnlockEntity>  unlockBonusList;
    private UILabel                 labelUnlockedCount;
    private Transform               unlockEffect;
    */
}
