using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using GameDataEditor;

public enum REWARD_LIST : int
{
    DIA100 = 0,
    DIA200 = 1,
    DIA300 = 2,
    DIA500 = 3,
    GIFT   = 4,
    SPIN   = 5,
    BUFF   = 6,
    BUFF_BONUS = 7,
    SIZE
}

public enum SPIN_UI_STATUS : int
{
    IDLE,
    GAUGE,
    SPINNING,
    SPIN_RESULT
}

public partial class SpinPopupEntity : BaseTransitionPopupEntity 
{       
/*
    protected override void Awake()
    {
        base.Awake();
        this.Serialize();
        // put serialize code here
        // this.listPartition      = new List<SpinPieceSerializable>();
        this.gaugeSpirte        = this.GetComponent<UISprite>("Sprite Gage");
        // this.gaugeSpirteBack       = this.GetComponent<UISprite>("Sprite Base");
        this.roundPiece         = this.GetComponent<Transform>("Pieces");
        this.arrow              = this.GetComponent<Transform>("Arrow", "Anchor");
        this.arrowShadow        = this.GetComponent<Transform>("Arrow", "Anchor Shadow");
        this.animatorArrow      = this.GetComponent<Animator>("Arrow");
        this.fsmInterface       = this.GetComponent<PlayMakerFSM>();
        this.labelRemainSpin    = this.GetComponent<UILabel>("Label Remain");
        this.bonusChangeAnimation = this.GetComponent<AnimationEntity>("Pieces");
        this.resultTransform    = this.GetComponent<Transform>("Result");
        this.resultTransform.gameObject.SetActive(false);
        this.buffSpin1          = this.GetComponent<Transform>("List 1");
        this.buffSpin2          = this.GetComponent<Transform>("List 2");
        this.buffSpinCursor     = this.GetComponent<Transform>("Cursor");

        this.AddOnClickListener( "Buff Select","Button Yes", this.OnBuffReplaceClick );
        this.AddOnClickListener( "Button Diamond",  this.OnShopClick );
        this.AddOnClickListener( "Button Back",     this.OnMenuClick );
        this.labelCurrentDiamond = this.GetComponent<UILabel>("Button Diamond", "Label Diamond");
        this.labelTimeNextSpinAvailable = this.GetComponent<UILabel>("Disabled", "Label Time");

        this.listBonusChance    = new List<BonusChance>();
        for ( int i = 0 ; i < 4 ; i++ )
        {
            this.listBonusChance.Add( this.GetComponent<BonusChance>("Lucky 0"+ (i+1) ));
        }

        this.ResetBuffSpinLabel();

        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
        this.isFocused = false;
    }

    private void Serialize()
    {
        string jsonSpin        = FileIOController.instance.LoadText( Defines.DATABASE_PATH+"spinParameters");
        MyLogger.LogObject("SpinPopupEntity", jsonSpin);
        // this.listPartition = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpinPieceSerializable>>(jsonSpin);
        
        GDEDataManager.Init("gde_data");    
        this.listPartition = new List<GDEspinListData>();
        for ( int i = 0 ; i < 8 ; i++ )
        {
            GDEspinListData loaded;
            GDEDataManager.DataDictionary.TryGetCustom(string.Format("spin_{0}",i), out loaded);
            this.listPartition.Add(loaded);
             MyLogger.LogObject("SpinPopupEntity", this.listPartition);
        }

        this.sumWeight = 0;
        for ( int i = 0 ; i < listPartition.Count ; i++ )
        {
            this.sumWeight += listPartition[i].weight;
        }

        this.sumBonusWeight = 0;
        for ( int i = 0 ; i < listPartition.Count ; i++ )
        {
            this.sumBonusWeight += listPartition[i].weight_bonus;
        }
    }

    void OnEnable()
    {
        this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();
    
        this.gaugePositionAngle          = 0f;
        this.gaugeSpeedTimer             = 0f;
        this.gaugeSpirte.fillAmount      = 0f;
        this.gaugeSpirte.invert          = true;

        UserInfo.instance.SetRemainText(this.labelRemainSpin, UserInfo.instance.GetCurrentSpinCount());
        // this.labelRemainSpin.text   = string.Format("REMAIN: {0}" ,UserInfo.instance.GetCurrentSpinCount());

        this.currentPositionAngle        = 0f;
        this.roundPiece.rotation = Quaternion.Euler ( 0,0, this.currentPositionAngle );
    }

    void OnRefresh()
    {
        this.OnEnable();
        this.GetComponent<UIPanel>().alpha = 1f;
    }

    protected override void        OnMenuClick(GameObject go)
    {
        if ( this.fsmInterface.ActiveStateName == "NoMore" || this.fsmInterface.ActiveStateName == "Idle" )
        {
            if ( this.bonusChange != null )
                this.bonusChange.gameObject.SetActive(false);
            base.OnMenuClick(null);
        }
    }
    
    void OnIdle()
    {
        int indexBonusChange        = UnityEngine.Random.Range(0,4); 

        if ( this.bonusChange != null )
            this.bonusChange.gameObject.SetActive(false);

        this.bonusChange            = listBonusChance[indexBonusChange];
        this.bonusChange.gameObject.SetActive(true);
        this.isBonus                = false;

        this.gaugePositionAngle          = 0f;
        this.gaugeSpeedTimer             = 0f;
        this.gaugeSpirte.fillAmount      = 0f;
        this.gaugeSpirte.invert          = true;

        UserInfo.instance.SetRemainText(this.labelRemainSpin, UserInfo.instance.GetCurrentSpinCount());
        //this.labelRemainSpin.text   = string.Format("REMAIN: {0}" ,UserInfo.instance.GetCurrentSpinCount());

        this.currentPositionAngle        = 0f;
        this.roundPiece.rotation = Quaternion.Euler ( 0,0, this.currentPositionAngle );
    }

    void OnGage()
    {
        UserInfo.instance.OnSpin();
    }

    void OnCheckSpinCount()
    {
        if (  REWARD_LIST.DIA100 == this.reward || REWARD_LIST.DIA200 == this.reward
            ||REWARD_LIST.DIA300 == this.reward || REWARD_LIST.DIA500 == this.reward )
        {
            UILabel labelDia = this.GetComponent<UILabel>("Button Diamond", "Label Diamond");
            labelDia.text    = ""+UserInfo.instance.BuyDiamond(this.rewardValue);
        }
        else if ( REWARD_LIST.SPIN == this.reward )
        {
            UserInfo.instance.AddSpin (this.rewardValue);
            UserInfo.instance.Save();
        }
        this.rewardValue = -1;

        this.FindChild("Button Take").SetActive(false);
        this.FindChild("Reward","Diamond").SetActive(false);
        this.FindChild("Reward","Spin").SetActive(false);

        this.ResetBuffOnCheckSpinCount();

        AnimationEntity.OnAnimationFinishDelegate OnResultDisappear = delegate(AnimationEntity animationEntity)
        {
            this.resultTransform.gameObject.SetActive(false);
        };
        this.resultTransform.GetComponent<AnimationEntity>().Play("Spin Result Disappear", OnResultDisappear, true);

        for ( int i = 0 ; i < 8; i++ )
        {
            SpinPieceEntity piece = this.GetComponent<SpinPieceEntity>(string.Format( "Piece {0:d2}", i+1 ) );
            piece.OnResetPiece();
            if ( UserInfo.instance.GetRandomGiftCharacterIndex() == -1 && ( i == 0 ) )
            {
                piece.OnPieceNotAvailable();
            }
        }

        this.animatorArrow.SetTrigger("idle");

        if ( UserInfo.instance.GetCurrentSpinCount() < 1 )
        {
            fsmInterface.SendEvent("NoMoreSpin");
        }else
        {
            this.FindChild("Disabled").SetActive(false);
        }
    }

    void OnCheckBonus()
    {
        float gaugeAbsolute = (this.gaugePositionAngle % 360f) / 360f;
        if ( this.bonusChange.IsGaugeInValue(gaugeAbsolute) )
        {
            this.isBonus = true;
            SocialManager.ReportAchievements( true, "spinLucky" );

            AnimationEntity.OnAnimationFinishDelegate OnHighlightEnded = delegate(AnimationEntity animationEntity)
            {
                this.Invoke("InvokePieceChange", 0.5f);
            };

            this.bonusChange.GetComponent<AnimationEntity>().Play("Lucky Chance Win", OnHighlightEnded, true);
        }
        else
        {
            this.SendStartSpinEvent();
        }
        this.ResetBuffSpinLabel();
    }

    void InvokePieceChange()
    {
        SpinPieceEntity piece = this.GetComponent<SpinPieceEntity>( "Piece 05" );
        if ( UserInfo.instance.GetRandomGiftCharacterIndex() == -1 )
        {
            piece.OnPieceNotAvailable();
        }

        AnimationEntity.OnAnimationFinishDelegate OnPieceChanged = delegate(AnimationEntity animationEntity)
        {
            this.Invoke( "SendStartSpinEvent", .5f );
        };
        this.bonusChangeAnimation.Play("Spin Change", OnPieceChanged, true);
    }

    [SerializeField] Vector3 tweenMaxSpeed;
    // [SerializeField] float tweenTimeSpan;
    [SerializeField] LeanTweenType spinWheelTweenType;

    void SendStartSpinEvent()
    {
        fsmInterface.SendEvent("StartSpin");
    }

    [SerializeField] float spinBrakeTimeSpan;
    [SerializeField] LeanTweenType spinBrakeTweenType;

    void TriggerBrake()
    {
        float timespan = this.spinBrakeTimeSpan;//UnityEngine.Random.Range( this.spinBrakeTimeSpan, this.spinBrakeTimeSpan*1.5f );
        LeanTween.value( this.roundPiece.gameObject, OnSpeedChange, tweenMaxSpeed.y, tweenMaxSpeed.z, timespan )
        .setEase(this.spinBrakeTweenType)
        .setOnComplete(this.TriggerBackward);
        //this.Invoke("TriggerBackward", maxSpeedSpan);
    }

    [SerializeField] float tweenBackTimeSpan;
    [SerializeField] LeanTweenType spinBackTweenType;

    void TriggerBackward()
    {
        float timespan = this.tweenBackTimeSpan;//UnityEngine.Random.Range( this.tweenBackTimeSpan, this.tweenBackTimeSpan*1.5f );
        LeanTween.value( this.roundPiece.gameObject, OnSpeedChange, tweenMaxSpeed.z, 0, timespan )
        .setEase(this.spinBackTweenType)
        .setOnComplete(this.OnEndSpin);
    }

    void OnEndSpin()
    {
        fsmInterface.SendEvent("SpinEnded");
    }

    void OnSpeedChange( float speed )
    {
        this.currentSpeed = speed;
    }

    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Escape) )
        {     
            this.CloseByAndroidBack();
        }
        
        if ( this.fsmInterface.ActiveStateName == "Gage" )
        {
            this.gaugeSpeedTimer += Time.deltaTime;
            this.gaugeSpeed      = Mathf.Lerp( this.gaugeSpeedLimit.min, 
                                               this.gaugeSpeedLimit.max, 
                                               this.gaugeSpeedTimer/this.gaugeSpeedTimeSpan);

            this.gaugePositionAngle += this.gaugeSpeed * Time.deltaTime;//Mathf.LerpAngle( 0, 360, this.gaugeSpeed * Time.deltaTime );
            
            float angle360  = this.gaugePositionAngle % 360f;

            if ( Mathf.CeilToInt(this.gaugePositionAngle/360f) % 2 == 1 )
            {
                this.gaugeSpirte.invert = true;
                this.gaugeSpirte.fillAmount = (angle360) / 360f;
            }
            else
            {
                this.gaugeSpirte.invert = false;
                this.gaugeSpirte.fillAmount = 1f - ((angle360) / 360f);
            }
        }
        else if ( this.fsmInterface.ActiveStateName == "Spin" )
        {
            this.currentPositionAngle += this.currentSpeed * Time.deltaTime;
            this.roundPiece.rotation   = Quaternion.Euler( 0, 0, -currentPositionAngle );     
            this.RotateArrow();
        }
        else if ( this.fsmInterface.ActiveStateName == "BuffSpin")
        {
            this.OnSpinTween();
        }
        else if ( this.fsmInterface.ActiveStateName == "NoMore" )
        {
            if ( UserInfo.instance.GetNextSpinIfAvailable() )
            {
                fsmInterface.SendEvent("SpinRecharged");
                this.FindChild("Disabled").SetActive(false);
            }
            else
            {
                TimeSpan ts = UserInfo.instance.GetTimeRemainToNextSpin();
                this.labelTimeNextSpinAvailable.text = string.Format("{0:f0}:{1:d2}:{2:d2}", 
                    ts.Hours, ts.Minutes, ts.Seconds);
            }
        }
    }  

    private void EndSpin()
    {
        // MyLogger.Red("SpinPopupEntity", "EndSpin");
        fsmInterface.SendEvent("SpinEnded");
    }

    [SerializeField] float leftTreshold;        //40f
    [SerializeField] float rightTreshold;       //5f
    [SerializeField] float middleTreshold;
    [SerializeField] float angle45;       //5f
    [SerializeField] float middleArrowSpeed;       //5f

    private void RotateArrow()
    {
        this.angle45       = (this.currentPositionAngle % 45f);
        // this.animatorArrow.speed = Mathf.Lerp( 0.5f, 1f, this.currentSpeed / this.tresholdSpeed );

        if ( this.currentSpeed < 0 )
        {
            if ( this.leftTriggered )
            {

            }
            else if ( angle45 > this.leftTreshold )
            {
                 //MyLogger.Red("RotateArrow", "leftTreshold40");
                //this.animationArrow.PlayQueued( "Arrow Left", QueueMode.PlayNow);
                this.animatorArrow.SetTrigger("left");
                this.leftTriggered  = true;
            }
            else if ( angle45 < 45f - this.leftTreshold )
            {
                // MyLogger.Blue("RotateArrow", "leftTreshold5");
                //this.animationArrow.PlayQueued( "Arrow Left", QueueMode.PlayNow);
                this.animatorArrow.SetTrigger("left");
                this.leftTriggered  = true;
            }
        }
        else if ( angle45 < this.middleTreshold && this.currentSpeed > middleArrowSpeed )
        {
            if ( this.rightTriggered == false )
            {
                rightTriggered = true;
                this.animatorArrow.SetTrigger("middle");
            }
        }
        else if ( angle45 < this.rightTreshold && this.currentSpeed < middleArrowSpeed )
        {
            if ( this.rightTriggered == false )
            {
                rightTriggered = true;
                // MyLogger.Log("RotateArrow", "right");
                this.animatorArrow.SetTrigger("right");
            }
        }
        else
        {
            this.rightTriggered = false;
            this.leftTriggered  = false;
        }
    }

    bool leftTriggered;
    bool rightTriggered;

    private  void OnShopClick( GameObject go )
    {
        if ( this.fsmInterface.ActiveStateName == "NoMore" || this.fsmInterface.ActiveStateName == "Idle" )
        {
            this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Shop UI") );
        }
    }

    void OnSpinWin()
    {
        int selected  = 9 - Mathf.CeilToInt( (this.currentPositionAngle % 360f)/45f);
        if ( selected >= 8)
            selected = 0;

        GDEspinListData selectedPiece = this.listPartition[selected];
        MyLogger.Red("OnSpin", string.Format("Spin Piece index {0} is selected: ",selected) ); 

        AnimationEntity.OnAnimationFinishDelegate OnSpinWinEnded = delegate(AnimationEntity animationEntity)
        {
            if ( REWARD_LIST.BUFF == this.reward || 
                 REWARD_LIST.BUFF_BONUS == this.reward )
            {
                fsmInterface.SendEvent("RewardBuff");
            }
            else if ( REWARD_LIST.GIFT == this.reward )
            {
                int gift_char = UserInfo.instance.GetRandomGiftCharacterIndex();
                if ( gift_char != -1 )
                {
                    SocialManager.ReportAchievements(true, "spinAnimal");
                    UserInfo.instance.UnlockCharacter(gift_char);

                    if ( UserInfo.instance.GetRandomGiftCharacterIndex() == -1 )
                    {
                        SocialManager.ReportAchievements(true, "spinAnimalAll");
                    }

                    this.GetComponent<UIPanel>().alpha = 0;
                    UnlockMenuEntity.instance.Show(GameInfo.instance.GetCharacterInfo(gift_char), this);
                    fsmInterface.SendEvent("RewardGiftCharacter");
                }
                else
                {
                    this.reward = REWARD_LIST.DIA500;
                    fsmInterface.SendEvent("RewardOthers");
                }
            }
            else
            {
                fsmInterface.SendEvent("RewardOthers");
            }
             //   fsmInterface.SendEvent("RewardBuff");
        };

        this.reward = selectedPiece.reward;
        if ( this.isBonus )
        {
            this.reward = selectedPiece.reward_bonus;
        }

        MyLogger.Red("OnSpinWin", string.Format("piece{0} is selected", selected+1) );
        this.GetComponent<AnimationEntity>("Piece 0"+(selected+1)).Play("Spin Win", OnSpinWinEnded, true);
    }

    void OnRewardConfirm()
    {
        this.resultTransform.gameObject.SetActive(true);
        this.FindChild("Reward","Buff").SetActive(false);  
        this.buffSpinCursor.gameObject.SetActive(false);  
        this.FindChild("Buff Select").SetActive(false);

        this.FindChild("Button Take").SetActive(true);

        this.UpdateLabel();
    }

    void UpdateLabel()
    {
        UILabel labelReward = this.GetComponent<UILabel>("Diamond", "Label Reward");

        if (  REWARD_LIST.DIA100 == this.reward )
        {
            this.rewardValue = 100;
        }
        else if ( REWARD_LIST.DIA200 == this.reward )
        {
            this.rewardValue = 200;
        }
        else if ( REWARD_LIST.DIA300 == this.reward )
        {
            this.rewardValue = 300;
        }
        else if ( REWARD_LIST.DIA500 == this.reward )
        {
            this.rewardValue = 500;
        }
        
        if ( REWARD_LIST.SPIN == this.reward )
        {
            this.rewardValue = 1;
            labelReward      = this.GetComponent<UILabel>("Spin", "Label Reward");
            labelReward.text = string.Format("+{0} SPIN", this.rewardValue);
        }
        else
        {
            labelReward.text = ""+this.rewardValue;
        }

        labelReward.transform.parent.gameObject.SetActive(true);
    }

    void OnSpin()
    {
        int size        = 8;
        float weight    = this.sumWeight;
        float sum       = 0;
        int selected    = 0;

        if ( this.isBonus )
        {   
            weight = this.sumBonusWeight;
        }
        int random  = UnityEngine.Random.Range(0,(int)weight);

        for ( int i = 0 ; i < size ; i++ )
        {
            if ( this.isBonus )
                sum += this.listPartition[i].weight_bonus;
            else
                sum += this.listPartition[i].weight;

            if ( sum > random )
            {
                MyLogger.Blue("OnSpin", string.Format("Spin Piece index {0} is selected: ",i) ); 
                selected = i;
                break;
            }
        }
        //this.goalPositionIndex          = selected;
        this.additionalRound            = Mathf.RoundToInt( ((this.gaugePositionAngle % 360f)-30f) / 90f);
        
        if ( UserInfo.instance.GetSpinUsedTotal() <= 1 )
        {
            selected                = 0;
            this.additionalRound    = 2;
        }
        this.currentSpeed               = 0;
        this.goalPiece                  = this.listPartition[selected];

        float timespan = this.goalPiece.round1;
        this.tweenMaxSpeed  = new Vector3( 0, 600f, 250f );
        if ( additionalRound == 1 )
        {
            this.tweenMaxSpeed  = new Vector3( 0, 700, 250f );
            timespan            = this.goalPiece.round2;
        }
        else if ( additionalRound == 2 )
        {
            this.tweenMaxSpeed  = new Vector3( 0, 800, 350f );
            timespan            = this.goalPiece.round3;
        }
        else if ( additionalRound == 3 )
        {
            this.tweenMaxSpeed  = new Vector3( 0, 850, 400f );
            timespan            = this.goalPiece.round4;
        }
        // this.tweenTimeSpan = timespan;
        LeanTween.value( this.roundPiece.gameObject, OnSpeedChange, tweenMaxSpeed.x, tweenMaxSpeed.y, timespan )
        .setEase(this.spinWheelTweenType)
        .setOnComplete(this.TriggerBrake);
    }
    */

    public  float                       currentPositionAngle;
    public  int                         additionalRound;
    public  float                       currentSpeed;

    public  List<GDEspinListData>       listPartition;
    public  GDEspinListData             goalPiece;
    private float                       sumWeight;
    private float                       sumBonusWeight;
    // public  List<BonusChance>           listBonusChance;
    // public  BonusChance                 bonusChange;

    public  Range2                      gaugeSpeedLimit;
    public  float                       gaugeSpeed;
    public  float                       gaugeSpeedTimeSpan;
    public  float                       gaugeSpeedTimer;
    public  float                       gaugePositionAngle;   
    private UISprite                    gaugeSpirte;
    private bool                        isBonus;
    private AnimationEntity             bonusChangeAnimation;

    private Transform                   roundPiece;
    public  Transform                   arrow;
    public  Transform                   arrowShadow;
    private Animator                    animatorArrow;
    private float                       arrowRotationAngle = 0f;

    // public  int                         currentSpinCount;
    private UILabel                     labelRemainSpin;
    private PlayMakerFSM                fsmInterface;

    private REWARD_LIST          reward;
    private Transform                   resultTransform;
    private Transform                   buffSelect;

    private Transform                   buffSpin1;
    private Transform                   buffSpin2;
    private Transform                   buffSpinCursor;

    private UILabel                 labelCurrentDiamond;
    private UILabel                 labelTimeNextSpinAvailable;

    public float tresholdSpeed;
    private int rewardValue = 0;
}