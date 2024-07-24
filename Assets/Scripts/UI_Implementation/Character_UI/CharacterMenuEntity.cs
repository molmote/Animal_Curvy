using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMenuEntity : BaseTransitionPopupEntity 
{
    private static CharacterMenuEntity _instance = null;
    public  static CharacterMenuEntity instance 
    {
        get
        {   
            return _instance;
        }
    }

    private UILabel                                 characterTotalCount;


    private UILabel                                 currentDiamond;

    public  List<CharacterPropertyBar>              characterBarList;
    public  UIPanel                                 gridPanel;
    public  UIScrollView                            scrollView;  

    private Transform                               btnSelect;   
    private Transform                               btnLocked; 
    private Transform                               spriteNew;
    private UILabel                                 labelCondition;
    private UILabel                                 labelName;

    public  float                                   defaultMomentum;
    public  float                                   maxMomentum;
    public  Range2                                  dampingRange;
    public  float                                   dragSensibility;
    private List<BaseUnlockEntity>                  unlockBonusPreviewList;

    private int     currentIndex;
    private int     cellWidth;
    private bool    isDragging;

    public  Vector3  focusedSize;
    public  Vector3  bigCharacterScale;
    public  float    upScaleTimeSpan;
    public  float    downScaleTimeSpan;
    public  float    spacingTimeSpan;

    bool isRealDragging;
    public float thresholdToStop;

    public float dragSetBackTime;
    
    public  Range2      scrollClampArea;

    bool    isInit   = false;

    protected override void Awake()
    {
        _instance = this;
        base.Awake();
        this.LinkUI();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;
    }    

    private void LinkUI()
    {
        this.currentDiamond     = this.GetComponent<UILabel>("Label Diamond");

        this.AddOnClickListener("Button Back",          OnMenuClick);
        this.AddOnClickListener("Button Diamond",       OnBuyDiamond);
        this.AddOnClickListener("Button Select",        OnCharacterSelect);
        this.btnSelect          = this.GetComponent<Transform>("Button Select");
        this.btnLocked          = this.GetComponent<Transform>("Button Lock");
        this.spriteNew          = this.GetComponent<Transform>("New");
        this.labelCondition     = this.GetComponent<UILabel>  ("Label Condition");
        this.labelName          = this.GetComponent<UILabel>  ("Label Name");

        this.characterTotalCount = this.GetComponent<UILabel>("Label Total");

        
        this.gridPanel          = this.GetComponent<UIPanel>("List");
        this.scrollView         = this.gridPanel.GetComponent<UIScrollView>();
        this.cellWidth          = (int)this.GetComponent<UIGrid>("Grid").cellWidth;
        this.scrollView.onDragStarted   += this.OnDragStart;
        this.scrollView.onDragFinished  += this.OnDragEnd;
        this.scrollView.onStoppedMoving  += this.OnMomentumZero;
    }

    void OnDragStart()
    {
        this.isDragging = true;
        this.isRealDragging = true;
    }

    void OnDragging()
    {
        int index = this.GetIndexAtCurrentPosition();
        this.UpdateBarStatus(index);
        //this.currentIndex = index;

        MyLogger.Red("OnDragging", ""+this.scrollView.currentMomentum);
        if ( Mathf.Abs(this.scrollView.currentMomentum.x) <= this.thresholdToStop && this.isRealDragging == false )
        {
            int x = this.currentIndex * this.cellWidth;

            Vector3 dest = new Vector3 ( -x, this.gridPanel.transform.localPosition.y ,0 );
            LeanTween.moveLocal( this.gridPanel.gameObject, dest, this.dragSetBackTime )
            .setOnComplete(this.ResetOffset);

            this.scrollView.currentMomentum = Vector3.zero;
            this.isRealDragging = true;
        }
    }

    private void ResetOffset()
    {
        int x = this.currentIndex * this.cellWidth;

        this.gridPanel.clipOffset               = 
            new Vector3 ( 290+x, this.gridPanel.clipOffset.y ,0 );
    }

    void OnDragEnd()
    {
        this.isRealDragging = false;
        this.OnDragging();
    }

    void OnMomentumZero()
    {
        this.OnDragging();
        this.isDragging = false;
    }

    public void UpdateBarStatus()
    {
        MyLogger.Red("UpdateBarStatus", "currentIndex"+this.currentIndex);
        this.UpdateBarStatus(this.currentIndex);
    }

    public void UpdateBarStatus(int index)
    {   
        GameCharacterInformation info = GameInfo.instance.GetCharacterInfo(index);
        this.labelName.text = GameInfo.instance.GetLocalizedCharacterName(index);

        this.characterTotalCount.text = string.Format("({0}/{1})", UserInfo.instance.GetUnlockedCharacterCount(), 
                                                                   UserInfo.instance.characterStatusList.Count);

        this.labelCondition.gameObject.SetActive(false);
        if ( this.currentIndex != index )
        {
 MyLogger.Red("UpdateBarStatus", ""+index);
            this.characterBarList[this.currentIndex].OnFocusChanged(false);
            this.characterBarList[index].OnFocusChanged(true);
        }
        else
        {
            this.characterBarList[index].OnFocusChanged(true);
        }

        this.btnSelect.gameObject.SetActive(true);
        this.btnLocked.gameObject.SetActive(false);
        if ( UserInfo.instance.IsCharacterLocked(index) )
        {
            if ( index > 1 )
            {
                this.btnSelect.gameObject.SetActive(false);
                this.btnLocked.gameObject.SetActive(true);
            }
            else if ( PURCHASE_TYPE.TWITTER == info.purchaseType )
            {
                this.labelName.text = UserInfo.GetLocal("18").ToUpper();
            }
            else if ( PURCHASE_TYPE.FACEBOOK == info.purchaseType)
            {
                this.labelName.text = UserInfo.GetLocal("17").ToUpper();
            }
        }

        this.labelCondition.gameObject.SetActive(false);
        GetConditionText(info,this.labelCondition);

        this.currentIndex = index;
    }

    public  static void GetConditionText(GameCharacterInformation info, UILabel _labelCondition)
    {
        if ( PURCHASE_TYPE.COMBO == info.purchaseType )
        {
            _labelCondition.gameObject.SetActive(true);
            _labelCondition.text = string.Format( UserInfo.GetLocal("36") + " ({1}/{0})", 
                info.unlockCondition, UserInfo.instance.GetBestDiamond() );
            _labelCondition.bitmapFont    = UserInfo.instance.GetLocalizationFont();
        }
        else if ( PURCHASE_TYPE.SCORE == info.purchaseType)
        {
            _labelCondition.gameObject.SetActive(true);
            _labelCondition.text = string.Format( UserInfo.GetLocal("37") + " ({1}/{0})", 
                info.unlockCondition, UserInfo.instance.GetBestScore() );
            _labelCondition.bitmapFont    = UserInfo.instance.GetLocalizationFont();
        }
        else if ( PURCHASE_TYPE.OBSTACLE == info.purchaseType )
        {
            _labelCondition.gameObject.SetActive(true);
            _labelCondition.text = string.Format( UserInfo.GetLocal("38") + " ({1}/{0})", 
                info.unlockCondition, UserInfo.instance.obstacleCount );
            // + " ({0}/{1})", UserInfo.instance.obstacleCount , info.unlockCondition);
            _labelCondition.bitmapFont    = UserInfo.instance.GetLocalizationFont();
        }
    }

    public  void ChangeMomentum(float m)
    {
        if ( this.scrollView.isDragging )
        this.scrollView.momentumAmount = m;
    }

    void Update ()
    {
        if ( this.isDragging )
        {
            this.OnDragging();
        }

        if ( this.scrollView.transform.localPosition.x < scrollClampArea.min )
        {
            GameUIManager.instance.scrollDampingMomentum = 15f;
            // MyLogger.Red("currentMomentum", ""+this.scrollView.currentMomentum);
        }
        else if ( this.scrollView.transform.localPosition.x > scrollClampArea.max )
        {
            GameUIManager.instance.scrollDampingMomentum = 15f;
            // MyLogger.Red("currentMomentum", ""+this.scrollView.currentMomentum);
        }
    }

    private     float   GetAutoDragPercentage()
    {       
        float per = -this.scrollView.transform.localPosition.x / this.cellWidth;
        int   tmp = Mathf.FloorToInt(per);

        float result = per - tmp;
        MyLogger.Red("GetAutoDragPercentage..",string.Format("per:{0},tmp:{1},result:{2}", per, tmp, result));
        return result;
    }

    public  void OnCharacterSelect(GameObject go)
    {
        /*if ( ( this.isDragging && this.GetAutoDragPercentage() < 0.8f ) || this.isClosing )                      
        {
            MyLogger.Red("OnCharacterSelect","isDragging");
            return;
        }    */

        if ( UserInfo.instance.IsCharacterLocked( this.currentIndex ) )
        {
            MyLogger.Red("OnCharacterSelect","Locked");
            if ( this.currentIndex == 0 )
            {
                this.OnPurchaseTry(PURCHASE_TYPE.TWITTER);
            }
            else if ( this.currentIndex == 1 )
            {
                this.OnPurchaseTry(PURCHASE_TYPE.FACEBOOK);
            }
        }
        else
        {
            UserInfo.instance.SelectCharacter(this.currentIndex);
            SoundController.instance.Play(UserInfo.instance.GetCharacterInfo().unlockSound);

            this.UpdateBarStatus(this.currentIndex);
            this.OnMenuClick(go);
        }
    }

    private     int     GetIndexAtCurrentPosition()
    {   
        float pos = -this.scrollView.transform.localPosition.x ;//+offset;
        int a = Mathf.FloorToInt( pos / this.cellWidth );
        int b = Mathf.CeilToInt ( pos / this.cellWidth );
        
        int index = a;
        
        if ( a != b )
        {
            float diff = pos % this.cellWidth; // 0 ~ 959.999f
            if ( diff > ( this.cellWidth / 2) )
                index = b;

            // MyLogger.Red("Additional math..",string.Format("a:{0},b:{1},diff:{2}", a,b, diff));
        }

        // MyLogger.Red("Result..",string.Format("a:{0},b:{1},selected:{2}", a,b, index));
        if ( index < 0 )
            index = 0;
        if ( index >= this.characterBarList.Count )    
            index = this.characterBarList.Count-1;
        return index;
    }

    public void UpdateDiamond( int dia )
    {  
        this.currentDiamond.text    = dia.ToString();
    }

    void OnEnable()
    {
        if ( this.isInit )
        {           
            this.ResetMenuEntity();
            //GoogleAnalytics.Client.SendScreenHit("CharacterMenu");
        }
        else
        {
            Transform placement = this.GetComponent<Transform>("Grid");

            int charSize = GameInfo.instance.GetCharacterTotal();
            this.characterBarList[0].Initialize ( GameInfo.instance.GetCharacterInfo(0),UserInfo.instance.IsCharacterLocked(0) ); 
            this.characterBarList[1].Initialize ( GameInfo.instance.GetCharacterInfo(1),UserInfo.instance.IsCharacterLocked(1) ); 
            for ( int i = 2 ; i < charSize;  i++ )
            {
                GameObject copy = Creater.Create( "5_UI/CharacterListSample" );

                copy.name       = string.Format("{0:d2}", i+1);
                copy.transform.parent = placement;
                copy.transform.localScale = Vector3.one;
                copy.transform.localPosition = Vector3.zero;

                CharacterPropertyBar added = copy.GetComponent<CharacterPropertyBar>();   
                added.Initialize ( GameInfo.instance.GetCharacterInfo(i),UserInfo.instance.IsCharacterLocked(i) ); 
                this.characterBarList.Add( added );
            }
            this.isInit = true;

            this.GetComponent<UIGrid>("Grid").enabled = true;
            this.ResetMenuEntity();
        }
    }

    private void ResetMenuEntity()
    {
        this.UpdateDiamond(UserInfo.instance.GetCurrentDiamond());

        this.currentIndex = UserInfo.instance.GetCurrentCharacterIndex();

        for ( int j = 0 ; j < this.characterBarList.Count ; j++ )
        {
            if ( false == UserInfo.instance.IsCharacterLocked(j) )
            {
                this.characterBarList[j].Unlock();
            }
            else
            {
                this.characterBarList[j].Lock();
            }
        }

        List<int> listIndex = UserInfo.instance.GetNewlyUnlockedSpecialCharacter();

        if ( listIndex.Count > 0 )
        {
            this.UpdateMainPosition(listIndex[0]);
            this.UpdateBarStatus(listIndex[0]);
            UnlockMenuEntity.instance.ShowMultipleUnlock(listIndex);
        }
        else
        {
            this.UpdateMainPosition(this.currentIndex);
            this.UpdateBarStatus(this.currentIndex);
        }

        this.characterBarList[this.currentIndex].OnFocusChanged(true);

        this.labelName.bitmapFont    = UserInfo.instance.GetLocalizationFont();
    }

    public void UpdateMainPosition()
    {
        MyLogger.Red("UpdateMainPosition", "currentIndex"+this.currentIndex);
        this.UpdateMainPosition(this.currentIndex);
    }

    public void UpdateMainPosition(int targetIndex)
    {
        int x = targetIndex * this.cellWidth;

        this.gridPanel.transform.localPosition  = 
            new Vector3 ( -x, this.gridPanel.transform.localPosition.y ,0 );
        this.gridPanel.clipOffset               = 
            new Vector3 ( 290+x, this.gridPanel.clipOffset.y ,0 );
    }

    protected override void OnMenuClick(GameObject go )
    {
        if ( UserInfo.instance.isCharacterUnlockedRecently )
        {
            for ( int idx = 0 ; idx < this.characterBarList.Count ; idx ++ )
            {           
                if ( false == UserInfo.instance.IsCharacterLocked(idx) )
                {   
                    bool isNew = UserInfo.instance.IsCharacterUnlockedRecently(idx);
                    if ( isNew )
                    {
                        UserInfo.instance.SetNewlyUnlockedCharacter(idx, false); 
                    }
                }
            }  
        }
        this.gridPanel.clipOffset               = Vector2.zero;
        this.gridPanel.transform.localPosition  = Vector3.zero;
        
        this.ChangeMomentum( this.defaultMomentum );
        GameUIManager.instance.scrollDampingMomentum = 9f;

        for ( int i = 0 ; i < this.characterBarList.Count ; i++ )
        {
            this.characterBarList[i].OnFocusChanged(false);

            this.characterBarList[i].ResetPosition();
        }

        base.OnMenuClick(go);
    }

    public  void OnPurchaseTry(PURCHASE_TYPE type)
    {
        if ( type == PURCHASE_TYPE.TWITTER )
        {
            SocialManager.instance.OpenTwitterLink();
            // UserInfo.instance.UnlockCharacter(0);
            this.UpdateBarStatus(0);
            this.characterBarList[0].Unlock();
            UnlockMenuEntity.instance.Show(0, true);
        }
        else if ( type == PURCHASE_TYPE.FACEBOOK )
        {
            SocialManager.instance.OpenFacebookURL();
            // UserInfo.instance.UnlockCharacter(1);
            this.UpdateBarStatus(1);
            this.characterBarList[1].Unlock();
            UnlockMenuEntity.instance.Show(1, true);

        }
    }

    private void OnBuyDiamond(GameObject btn)
    {        
        this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Shop UI") );
    }    
}

