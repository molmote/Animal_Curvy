using UnityEngine;
using System.Collections;

public class ReviewPopupEntity : MyPopupEntity //BaseWidgetEntity 
{
    private static ReviewPopupEntity _instance = null;
    public static ReviewPopupEntity instance 
    {
        get
        {
            return _instance;
        }
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
        AnimationEntity.OnAnimationFinishDelegate OnEnableAnimation = delegate(AnimationEntity animationEntity)
        {
            this.colliderRate.enabled   = true;
            this.isAniFinished          = true;
        };
        this.animationOpen.Play("Rate", OnEnableAnimation);
    }

    private void OnReviewClick(GameObject go)
    {
        UserInfo.instance.SetReviewed();
        SocialManager.instance.OpenReviewLink();
        this.Exit();
    }

    private void Exit()
    {
        if ( this.isAniFinished )
        {
            this.gameObject.SetActive(false);
            // ResultPopupEntity.instance.SetButtonActive(true);
                this.colliderRate.enabled = false;
                this.isAniFinished = false;

            ResultPopupEntity.instance.OnReviewFinished();
        }
    }

    protected override void OnExitClick(GameObject go)
    {
        
        this.Exit();
    }

    // Use this for initialization
    protected override void Awake()
    {
        _instance = this;
        base.Awake();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;

        this.colliderRate = this.GetComponent<BoxCollider>("Button Rate");
            this.colliderRate.enabled = false;
            this.isAniFinished = false;
        this.AddOnClickListener("Button Rate",  OnReviewClick);
        this.AddOnClickListener("Collider",     OnExitClick);
        this.animationOpen = this.GetComponent<AnimationEntity>();
    }

    private BoxCollider         colliderRate;
    private bool                isAniFinished;
    private AnimationEntity     animationOpen;
}
