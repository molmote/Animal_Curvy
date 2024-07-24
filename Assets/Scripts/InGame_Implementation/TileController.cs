using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class TileEnvironment
{
    public string      name;
    public Vector3     color;
}


public class TileController : MonoBehaviour 
{
    private static TileController _instance = null;
    public static TileController instance 
    {
        get
        {   
            return _instance;
        }
    }
  
    [SerializeField] 				List<TileEntity> 		tilePrefabList;
    private 						TileEntity 				tileLast;
    public                          TileEntity              tileStart;
    public                          Queue<TileEntity>       tilePassedBy;
    // [SerializeField]                Transform               bottomTilePosition;
    [SerializeField]                float                 	leastDistanceFromLastTile;

    public 							List<int>				previousAngles;

    public                          List<TileEnvironment>   environmentNameList;  
        // {"Grass Theme", "Desert Theme", "Desert Theme Theme", "Forest Theme", "Spring Theme", "Beach Theme"};

    private RandomListInteger randomList = null;
    private int prevIndex = 0;
    public  Material     currentWallColor;

    void Awake()
    {
        _instance              = this;
        this.OnRestart();
    }

    public bool IsEnvironmentChangable(float score)
    {
        if ( this.environmentIndex >= _SETTING._.colorChnageScore.Count )
            return false;

        return score > _SETTING._.colorChnageScore[this.environmentIndex];
    }

    void Update () 
    {   
        if ( GamePlayerEntity.instance.IsDead() )
            return;

        if ( GamePlayerEntity.instance.isLoading )
            return;

        if ( AbstractObjectPooler.instance == null )
        	return;

        Vector3 playerPos = GamePlayerEntity.instance.GetTransform().position;
        if ( this.IsBottomReached(playerPos) && playerPos.y < -500 )
        {
            this.tilePassedBy.Enqueue (this.tileLast);
            this.PositionNewTile();
            if ( this.tilePassedBy.Count > 2 )
            {
                this.ResetTilesPassedBy( this.tilePassedBy.Peek() );
                this.tilePassedBy.Dequeue();
            }
        }    
    }

    private bool IsBottomReached(Vector3 playerPos)
    {
    	Vector3 bottom = this.tileLast.anchor.transform.position;
    	return playerPos.y < bottom.y + this.leastDistanceFromLastTile;
    }

    private RandomListInteger ResetRandomList( int prevIndex, int tileSize)
    {
        List<int> intList   = new List<int>();
        int     ls          = 1;
        int     rs          = Mathf.FloorToInt(tileSize / 2f)+1;

        if ( this.isLastTileRight )
        {
            for ( int i = ls ; i < rs ; i++)
            {
                intList.Add(i);
            }
        }
        else
        {
            for ( int i = rs ; i < tileSize ; i++)
            {
                intList.Add(i);
            }
        }

        if ( this.prevIndex != 0 )
            intList.Add(0);

        //for ( int i = 0 ; i < intList.Count ; i++)
        //MyLogger.BlueObject("ResetRandomList", intList);
        //MyLogger.BlueObject("ResetRandomList", "--------------------");

        return new RandomListInteger(intList);
    }

    private void ResetTilesPassedBy( TileEntity tile )
    {
        if ( tile != null )
        tile.DestroySelf();
    }

    bool isLastTileRight = false;
    // string nextTileName = "Desert Theme";

    int  environmentIndex;

    public  void ChangeEnvironment()
    {
        this.environmentIndex++;
        if ( this.environmentIndex >= this.environmentNameList.Count)
        {
            this.environmentIndex = this.environmentNameList.Count -1;
        }
        else
        {
            LeanTween.value ( this.gameObject, this.OnColorChange, 
                this.environmentNameList[this.environmentIndex-1].color,
                this.environmentNameList[this.environmentIndex].color,
                 _SETTING._.colorChangeTimeSpan)
            .setDelay(_SETTING._.colorChangeTimeDelay);
            
            if ( this.environmentIndex == 2 )
            {
                GameUIManager.instance.ChangeColorToSnow( new Color32 (195,231,246,255) );
            }
            else //if ( this.environmentIndex == 0 || this.environmentIndex == 3 )// snowwhite
            {
                GameUIManager.instance.ChangeColorToSnow( Color.white );
            }

        }
    }
    
    private void OnColorChange(Vector3 color)
    {
        this.currentWallColor.color = new Color32 ( 
            (byte)color.x, (byte)color.y, (byte)color.z, 255 );
    }

    public  void PositionNewTile()// int stage, int random )
    {        
        this.randomList = this.ResetRandomList(this.prevIndex, this.tilePrefabList.Count);
        int random = this.randomList.GetRandom(); 

        if ( GamePlayerEntity.instance.IsFsmState("MainAuto") )
        {
            random = 0;
        }
        else if ( UserInfo.instance.gameplayTutorialInitiated )
        {
            if ( this.isLastTileRight )
            {
                random = _SETTING._.tutorialTileIndex;
            }
            else
            {
                random = _SETTING._.tutorialTileIndex+5;
            }
        }
        else
        {
            if (random == 0)
            {
                random = this.randomList.GetRandom();
            }
        }

        if ( random != 0 )            
            this.isLastTileRight = !this.isLastTileRight;
        GameObject next 	= AbstractObjectPooler.instance.GetIdleObject(this.tilePrefabList[random].name);
        TileEntity nextTile = next.GetComponent<TileEntity>();
        this.prevIndex      = random;

        nextTile.transform.position  = this.tileLast.anchor.transform.position;

        nextTile.Initialize( this.environmentNameList[this.environmentIndex].name);

    	MyLogger.Red("PositionNewTile", string.Format("idx:{0}, position:({1},{2})", 
    				random, next.transform.position.x, next.transform.position.y));
                     
        this.tileLast 			    = nextTile;
        // this.tileLast.ClearObjects();

        if ( GamePlayerEntity.instance.IsFsmState("MainAuto") )
            return;

        float prob = Random.Range(0,100);
        if ( prob < _SETTING._.tileGemProbablity )
        {
            this.PositionGemToTile(this.tileLast);
        }

        float obstacleProb = Random.Range(0,100);
        if ( obstacleProb < _SETTING._.obstacleProbablityInPercent )
        {
            this.PositionObstacle(this.tileLast);
        }
    }

    //[SerializeField]        int         obstacleProb;

   	[SerializeField] 		Range2 		followerDistanceFromTile;
   	[SerializeField] 		int 		maxItemPerTile;
   	[SerializeField] 		int 		maxFollowerPerTile;
   	[SerializeField] 		int 		maxGemPerTile;

    private void PositionGemToTile(TileEntity tile)
    {
        if ( tile.pattern == null )
        {
            MyLogger.Red("PositionGemToTile", "null pattern");
            return;
        }

        Transform newPattern = tile.pattern.GetNewPattern();
        Transform relative = tile.transform;
         
        foreach(Transform t in newPattern)
        {  
            tile.RegisterNewGem(t);
        }
    }

    public void OnRestart()
    {
        if ( tilePassedBy == null )
        {
            this.tilePassedBy      = new Queue<TileEntity>();

            if ( this.currentWallColor == null )
            {
                this.currentWallColor = 
                this.tileStart.FindChild("Border Cover").GetComponent<Renderer>().sharedMaterial;
            }
        }
        else
        {
            foreach ( TileEntity tile in this.tilePassedBy )
            {
                tile.DestroySelf();
            }
            this.tileLast.DestroySelf();
            this.tilePassedBy.Clear();
        }

        LeanTween.cancel( this.gameObject );

        this.tileStart.gameObject.SetActive(true);
        this.tileLast          = this.tileStart;
        this.currentWallColor.color  = new Color32(
           (byte) this.environmentNameList[0].color.x, 
           (byte) this.environmentNameList[0].color.y, 
           (byte) this.environmentNameList[0].color.z, 255 );

        this.environmentIndex = 0;
        // if ( UserInfo.instance.gameplayTutorialInitiated )
        this.isLastTileRight  = false;
        this.prevIndex        = 0;
    }

    private void PositionObstacle( TileEntity tile )
    {  
        bool right = tile.RegisterNewObstacle();
        GameUIManager.instance.InvokeDanger(right);
    }

}
