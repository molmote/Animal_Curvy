using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class SpinPopupEntity : BaseTransitionPopupEntity 
{
	/*int 	buffSize = 7;
	float   buffHeight = 630f;

    void ResetBuffOnCheckSpinCount()
    {
        this.buffSpinCursor.gameObject.SetActive(false);  
        this.buffSpinCursor.localPosition 		= Vector3.zero;
        this.buffSpin1.transform.localPosition  = Vector3.zero;
        this.buffSpin2.transform.localPosition  = new Vector3(0,-buffHeight,0);
        	// 9*9 81 -> 7*9 63
    }       

    private void ResetBuffSpinLabel()
    {
        for ( int i = 0 ; i < this.buffSize ; i++ )
        {
            int baseSize = GameInfo.instance.GetBuffInfoSize();
            BuffSerializable info = GameInfo.instance.GetBuffInfo(i);
            string text = info.name;
            if ( this.isBonus )
            {
                text = text.Replace("(x)", ""+info.megaAmount);
            }
            else
            {
                text = text.Replace("(x)", ""+info.normalAmount);
            }
            GameObject list1 = this.FindChild( this.buffSpin1.gameObject, string.Format("{0:d2}", i+1 ) );
            UILabel label1 = this.GetComponent<UILabel>( list1, "Label Buff");
            label1.text      = text;
            label1.color     = Color.white;

            GameObject list2 = this.FindChild( this.buffSpin2.gameObject, string.Format("{0:d2}", i+1 ) );
            UILabel label2 = this.GetComponent<UILabel>( list2, "Label Buff");
            label2.text      = text;
            label2.color     = Color.white;
        }
        this.GetComponent<Transform>("Reward", "Sprite Win").gameObject.SetActive(false);
    }

    void OnBuffSpin()
    {
        this.resultTransform.gameObject.SetActive(true);
        this.FindChild("Reward","Diamond").SetActive(false);
        this.FindChild("Reward","Spin").SetActive(false);

        this.FindChild("Reward","Buff").SetActive(true);  
        this.buffSpinCursor.gameObject.SetActive(true);  

        this.buffSelectedIndex      = GameInfo.instance.GetRandomBuff(this.isBonus);
        MyLogger.Red("OnBuffSpin", "buffSelectedIndex" + buffSelectedIndex);

        this.round  = Random.Range( (int)buffSpinRound.min, (int)buffSpinRound.max);
        this.randomTargetPosition   = this.buffSelectedIndex + (this.buffSize * round) - 5;

        float timeSpan = this.buffSpinTimeSpan.min;
        if ( round >= (int)buffSpinRound.min )
        {
            timeSpan = this.buffSpinTimeSpan.max;
        }

        LeanTween.moveLocal ( this.buffSpinCursor.gameObject, 
            new Vector3(0,90f * this.randomTargetPosition,0), timeSpan )
        //.setOnUpdate(this.OnSpinTween)
        .setOnComplete(this.OnSpinCursorStopped)
        .setEase(this.buffSpinTweenType);

        this.FindChild("Button Take").SetActive(false);
        this.FindChild("Buff Select").SetActive(false);
    }

    void OnSpinTween()
    { 
        if ( this.buffSpinCursor.localPosition.y > 2500f )
        {
            // if ( this.buffSpin2.transform.localPosition.y > this.buffSpin1.transform.localPosition.y )
            this.buffSpin2.transform.localPosition = new Vector3( 0, -buffHeight * 5f, 0);
        }
        else if ( this.buffSpinCursor.localPosition.y > 1800f )
        {
            // if ( this.buffSpin1.transform.localPosition.y > this.buffSpin2.transform.localPosition.y )
            this.buffSpin1.transform.localPosition = new Vector3( 0, -buffHeight * 4f, 0);
        }
        else if ( this.buffSpinCursor.localPosition.y > 1250f )
        {
            // if ( this.buffSpin2.transform.localPosition.y > this.buffSpin1.transform.localPosition.y )
            this.buffSpin2.transform.localPosition = new Vector3( 0, -buffHeight * 3f, 0);
        }
        else if ( this.buffSpinCursor.localPosition.y > 500f )
        {
            // if ( this.buffSpin1.transform.localPosition.y > this.buffSpin2.transform.localPosition.y )
            this.buffSpin1.transform.localPosition = new Vector3( 0, -buffHeight * 2f, 0);
        }
        this.ColorBuffLabel();
    }

    int sound;

    void ColorBuffLabel()
    {
        float cursor = this.buffSpinCursor.localPosition.y % buffHeight;
        bool  even   = (this.buffSpinCursor.localPosition.y / buffHeight) % 2 == 0;
        int   index  = Mathf.RoundToInt( cursor / 90f ) + 5;
        index = index % this.buffSize;
        
        if ( buffPrevIndex == index )
        {
            return;
        }
        
        SoundController.instance.Play( "Buff Spin Loop", true);

        buffPrevIndex = index;

        if ( buffPrevLabel1 != null )
            buffPrevLabel1.color = Color.white;

        if ( buffPrevLabel2 != null )
            buffPrevLabel2.color = Color.white;

        // MyLogger.Red("ColorBuffLabel1", "list"+index);
        UILabel labelBuff1 = this.FindChild("List 1", string.Format("{0:d2}", index+1 )).transform.Find("Label Buff").GetComponent<UILabel>();
        labelBuff1.color = new Color32 ( 104, 222, 179, 255 );
        buffPrevLabel1 = labelBuff1;

        UILabel labelBuff2 = this.FindChild("List 2", string.Format("{0:d2}", index+1 )).transform.Find("Label Buff").GetComponent<UILabel>();
        labelBuff2.color = new Color32 ( 104, 222, 179, 255 );
        buffPrevLabel2 = labelBuff2;
    }

    void OnSpinCursorStopped()
    {
        SoundController.instance.Play( "Buff Selected", true);
        this.GetComponent<Transform>("Reward", "Sprite Win").gameObject.SetActive(true);
        this.GetComponent<Animation>("Reward", "Buff").Play("Buff Win");
        fsmInterface.SendEvent("BuffSpinEnded");

        this.ColorBuffLabel();
        SocialManager.ReportAchievements( this.isBonus, "spinMegaBuff" );
        SocialManager.ReportAchievements( this.buffSelectedIndex == 2, "spinDiamond" );
        // LeanTween.move ( this.buffSpin2.gameObject, new Vector3(0,600f,0),  3f ).setOnComplete(this.OnSpin1Ended);// LTDescr
    }

    void OnBuffConfirm()
    {
        if ( UserInfo.instance.currentBuffStatus != null && 
             UserInfo.instance.currentBuffStatus.currentCount > 0 )
        {
            this.FindChild("Buff Select").SetActive(true);
        }
        else
        {
            this.FindChild("Button Take").SetActive(true);
            
            // MyLogger.Red("SpinPopupEntity", "OnBuffConfirm");
            BuffSerializable buff = GameInfo.instance.GetBuffInfo(this.buffSelectedIndex);
            int count             = buff.normalAmount;
            if ( this.isBonus )
                count = buff.megaAmount;
            UserInfo.instance.UpdateBuffStatus( buff, count );
        }
    }

    void OnBuffReplaceClick(GameObject go)
    {
        // MyLogger.Red("SpinPopupEntity", "OnBuffReplaceClick");
        BuffSerializable buff = GameInfo.instance.GetBuffInfo(this.buffSelectedIndex);
        int count             = buff.normalAmount;
        if ( this.isBonus )
            count = buff.megaAmount;
        UserInfo.instance.UpdateBuffStatus( buff, count );
    }

    int buffPrevIndex;
    UILabel buffPrevLabel1;
    UILabel buffPrevLabel2;

    [SerializeField] LeanTweenType buffSpinTweenType;
    [SerializeField] Range2 buffSpinTimeSpan;
    [SerializeField] Range2 buffSpinRound;
    [SerializeField] int randomTargetPosition;
    [SerializeField] int buffSelectedIndex;
    [SerializeField] int round;
    */
}