using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public enum FOLLOWER_GRADE: int
{
    Bronze = 0,
    Silver,
    Gold,
    Diamond,
    VIP
}

public class FollowerMenuEntity : BaseTransitionPopupEntity 
{
    private static FollowerMenuEntity _instance = null;
    public static FollowerMenuEntity instance 
    {
        get
        {
            return _instance;
        }
    }

    private UILabel     labelCurrentDiamond;
    // private UILabel     labelCollectedCount;

    public  List<FollowerUICount> followerGradeData;

    private List<FollowerPropertyBar> followerBarList;
    private UIPanel                   gridPanel;
    public  UIScrollView              scrollView;
    public  float                     defaultMomentum;
    public  float                     maxMomentum;
    public  float                     minimumDamping;
    public  float                     dragSensibility;
    
	protected override void Awake()
	{
        _instance = this;
        base.Awake();
        this.LinkUI();
        this.gameObject.SetActive(false);
        this.GetComponent<UIPanel>().alpha = 1f;

        // UserInfo.instance.UpdateFollowerInfo(this.followerGradeData);
	}	

    public  bool IsReachedGoal(int _count, FOLLOWER_GRADE _grade)
    {
        int idx = (int) _grade;
        if ( _count >= this.followerGradeData[idx].count ) 
        {
            return true;
        }
        return false;
    }

    public FollowerUICount GetFollowerData(int index)
    {
        return this.followerGradeData[index];
    }

    private void LinkUI()
    {
        this.labelCurrentDiamond = this.GetComponent<UILabel>("Label Diamond");
        // this.labelCollectedCount = this.GetComponent<UILabel>("Label Collection Count");

        this.AddOnClickListener("Button Back",              OnMenuClick);
        this.AddOnClickListener("Button Diamond",           OnShopClick);
        
        this.gridPanel          = this.GetComponent<UIPanel>("List");
        this.scrollView         = this.gridPanel.GetComponent<UIScrollView>();
        
        this.followerBarList = new List<FollowerPropertyBar>();
        for ( int i = 0 ; i < 60 ; i ++ )
        {
            this.followerBarList.Add( this.GetComponent<FollowerPropertyBar>
                ( "List", string.Format( "{0:d2}", i+1 ) ) );
        }   
    }

    public  void ChangeMomentum(float m)
    {
        if ( this.scrollView.isDragging )
        this.scrollView.momentumAmount = m;
    }

    void Start()
    {
        int index = 0;
        foreach ( KeyValuePair<string,FollowerUICount> followerIndex in UserInfo.instance.followerCountDict )
        {           
            FollowerUICount follower = followerIndex.Value;

            this.followerBarList[index].Initialize ( followerIndex.Key, follower );
            index++;
        }   
            
        this.OnEnable();
    }

    void OnEnable()
    {   
        if ( this.labelCurrentDiamond == null )
            return;

        this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();

        // this.labelCollectedCount.text = UserInfo.instance.GetFollowerCollectionFormat();
        this.UpdateUnlockedFollower(true);
    }

    void OnUpdate()
    {
        this.labelCurrentDiamond.text = "" + UserInfo.instance.GetCurrentDiamond();

        // this.labelCollectedCount.text = UserInfo.instance.GetFollowerCollectionFormat();
        this.UpdateUnlockedFollower(false);
    }

    public  void        OnShopClick( GameObject go)
    {
        this.EnterSubMenu( FindChild(GameUIManager.instance.gameObject, "Shop UI") );
    }

    private void UpdateUnlockedFollower(bool changePos)
    {
        int first = 0;
        int idx   = 0;
        foreach ( KeyValuePair<string,FollowerUICount> followerIndex in UserInfo.instance.followerCountDict )
        {           
            FollowerUICount follower = followerIndex.Value;
            bool isNew = follower.status == LockStatus.NEW;
            this.followerBarList[idx].UpdateStatus( follower );

            if ( first == 0 && isNew )
                first = idx;
            idx++;
        }   

        if ( changePos )
        {
            float y = Mathf.FloorToInt(first/4f) * 235f;
            if ( y > 2370)
                y = 2370f;
            this.gridPanel.transform.localPosition  = new Vector3 ( 0, y-115 ,0 );
            this.gridPanel.clipOffset               = new Vector3 ( 0, -y ,0 );
        }
    }

    protected override void OnMenuClick(GameObject go )
    {
        if ( UserInfo.instance.isFollowerUnlockedRecently == false )
        {
           // this.gridPanel.clipOffset               = Vector2.zero;
           // this.gridPanel.transform.localPosition  = new Vector3 ( 0, -120 ,0 );
        }
        else
        {
            foreach ( KeyValuePair<string,FollowerUICount> followerIndex in UserInfo.instance.followerCountDict )
            {           
                FollowerUICount follower = followerIndex.Value;
                bool isNew = follower.status == LockStatus.NEW;

                if ( isNew )
                {
                    follower.SetNewlyUnlocked(false);
                }
            }   
        }

        this.ChangeMomentum( this.defaultMomentum );
        GameUIManager.instance.scrollDampingMomentum = 9f;
        base.OnMenuClick(go);
    }
}
*/
