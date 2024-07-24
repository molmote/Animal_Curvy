using UnityEngine;
using System.Collections;


public class BuffPropertyUI : BaseWidgetEntity
{			
	private UISprite 				iconBuffSprite;
	private UILabel 				iconBuffMultiply;

	private UILabel 				textBuffLabel;
	private UILabel 				textBuffMultiply;

	private UILabel 				labelBuffRemain;

	void Awake()
	{
        this.iconBuffSprite 	= this.GetComponent<UISprite>("Buff Type Icon", "Sprite Buff Icon");
        this.iconBuffMultiply 	= this.GetComponent<UILabel> ("Buff Type Icon", "Label Multiply Number");

        this.textBuffLabel 		= this.GetComponent<UILabel>("Buff Type Text", "Label Buff Type" );
        this.textBuffMultiply 	= this.GetComponent<UILabel>("Buff Type Text", "Label Multiply Number");

        this.labelBuffRemain 	= this.GetComponent<UILabel>("Label Remain");
	}


	// exception for 1.5
	public 		void 	UpdateInfo ( UserInfo.BuffStatus buffInfo )
	{		
		if ( UserInfo.instance.gameplayTutorialInitiated )
			return;

		float multiply = buffInfo.serialized.effect;
		this.iconBuffSprite.transform.parent.gameObject.SetActive(false);
		this.textBuffLabel.transform.parent.gameObject.SetActive(false);

		this.iconBuffMultiply.text = string.Format("{0}", multiply);
		this.textBuffMultiply.text = string.Format("{0}", multiply);

		if ( this.labelBuffRemain != null )
		UserInfo.instance.SetRemainText(this.labelBuffRemain, buffInfo.currentCount);
        //this.labelBuffRemain .text = string.Format("{0}: {1}", UserInfo.GetLocal(""), buffInfo.currentCount);

        if ( buffInfo.serialized.type == BUFF_TYPE.DIAMOND )
        {
	        //string text = buffInfo.serialized.name.Replace("(x)", string.Format("({0}/{1})", buffInfo.currentCount) );
	        this.iconBuffSprite.spriteName = "Icon Diamond";
			this.iconBuffSprite.transform.parent.gameObject.SetActive(true);
        }
        else if ( buffInfo.serialized.type == BUFF_TYPE.COMBO )
        {
        	this.textBuffLabel.text = "COMBO";
			this.textBuffLabel.transform.parent.gameObject.SetActive(true);
        }
        else if ( buffInfo.serialized.type == BUFF_TYPE.SCORE )
        {
        	this.textBuffLabel.text = "SCORE";
			this.textBuffLabel.transform.parent.gameObject.SetActive(true);
        }
	}
}