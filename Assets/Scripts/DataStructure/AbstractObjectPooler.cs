using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbstractObjectPooler : MonoBehaviour 
{
	private static AbstractObjectPooler _instance;
	public  static AbstractObjectPooler instance 
	{
		get
		{
            if ( _instance == null )
            {
                _instance = GameObject.Find("_ObjectPooler").GetComponent<AbstractObjectPooler>();
                _instance.InitialLoad();
            }
			return _instance;
		}
	}
	[SerializeField]	
	private  bool 						GrowOnLimitReached = true;

	private Dictionary <string, AbstractObjectBlock> objectPool;

	[SerializeField]
	private Transform 					objectPositionWhenIdle;

	// Use this for initialization
	void Awake ()
	{
		//if ( _instance == null )
		//this.InitialLoad();
		//_instance = this;

		// this.Invoke( "SleepAfterInit", 0.5f );		
	} 

	private void InitialLoad()
	{
		this.objectPool = new Dictionary<string, AbstractObjectBlock>();
		
		this.LoadSingleAsset("3_Object/Object Diamond", 20);

		this.LoadSingleAsset("2_Environment/Tile Left 01", 4);
		this.LoadSingleAsset("2_Environment/Tile Left 02", 4);
		this.LoadSingleAsset("2_Environment/Tile Left 03", 4);
		this.LoadSingleAsset("2_Environment/Tile Left 04", 6);
		this.LoadSingleAsset("2_Environment/Tile Left 05", 4);
		this.LoadSingleAsset("2_Environment/Tile Right 01", 4);
		this.LoadSingleAsset("2_Environment/Tile Right 02", 4);
		this.LoadSingleAsset("2_Environment/Tile Right 03", 4);
		this.LoadSingleAsset("2_Environment/Tile Right 04", 6);
		this.LoadSingleAsset("2_Environment/Tile Right 05", 4);
		this.LoadSingleAsset("2_Environment/Tile Straight", 5);
		this.LoadSingleAsset("2_Environment/Tile Start", 1);

		this.LoadSingleAsset("2_Environment/Obstacle_01", 10);

		this.LoadSingleAsset("4_Effect/Diamond Disappear", 	10);
		this.LoadSingleAsset("4_Effect/Follower Obtain", 	40);

		this.LoadFolder("1_Character/InGame/", 1);
		this.LoadFolder("1_Character/UI/",1,GameUIManager.instance.unlockPreviewPosition,"Unlock");	
	}

	void SleepAfterInit()
	{
		foreach(KeyValuePair<string, AbstractObjectBlock> entry in objectPool)
		{
			entry.Value.SleepAfterInit();
		}
	}

	void LoadFolder(string pathUnderAssets, int size, string subParentName = "", string prefix = "")
	{		
        Transform subParent = null;
        if ( subParentName != "" )
        {
	        subParent 		= new GameObject().transform;
	        subParent.name 	= subParentName;	 
	      	subParent.parent = this.objectPositionWhenIdle;
	    }
	    this.LoadFolder(pathUnderAssets, size, subParent, prefix);
	}

	void LoadFolder(string pathUnderAssets, int size, Transform diffParent, string prefix = "")
	{		
        Object[] gamePrefabs = Resources.LoadAll(pathUnderAssets, typeof(GameObject));
        
        for ( int i = 0 ; i < gamePrefabs.Length ; i++ )
        {
        	GameObject prefab = gamePrefabs[i] as GameObject;
        	this.objectPool[prefix+prefab.name] = this.CreateObjectBlock(prefab,diffParent,prefix,size);        	
        }
	}

	public void ChangeParentFolder(string fullName,Transform newParent)
	{
		this.objectPool[fullName].ChangeParent( newParent );
	}

	void LoadSingleAsset(string pathUnderAssets, int size)
	{		
        GameObject gamePrefab = Resources.Load(pathUnderAssets, typeof(GameObject)) as GameObject;     
        // MyLogger.Red("LoadSingleAsset", gamePrefab.name);   
    	this.objectPool[gamePrefab.name] = this.CreateObjectBlock(gamePrefab, null, "", size);        
	}

	private AbstractObjectBlock CreateObjectBlock(GameObject blockPrefab, Transform subParent = null, string prefix = "", int size = 50)
	{
		GameObject folder 		= new GameObject();
		folder.name 			= blockPrefab.name;
        // MyLogger.Red("CreateObjectBlock", "blockPrefab :"+blockPrefab.name);
		if ( subParent == null )
		{
			subParent = this.objectPositionWhenIdle;	
		}
		else
		{
			// folder = subParent.gameObject;
		}

		folder.transform.parent = subParent;
		folder.transform.localPosition  = Vector3.zero;
        folder.transform.localRotation 	= Quaternion.identity;
		folder.transform.localScale 	= Vector3.one;

		AbstractObjectBlock block = new AbstractObjectBlock(blockPrefab, folder.transform, prefix, size);

		return block;
	}

	/*void Start () 
	{
		// GameInfo.instance.isGamePaused = false;
		
		this.objectPool = new Dictionary<string, AbstractObjectBlock>();
		this.LoadFolder("2_Monster/");
		// this.LoadFolder("3_Environment/2_Object/Common/");		
		//this.LoadFolder("4_Effect/1_Common/");
	}*/

	public void Clear()
	{		
		foreach(KeyValuePair<string, AbstractObjectBlock> entry in objectPool)
		{
		    entry.Value.Clear();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{		
	}

	public void Pause()
	{		
		foreach(KeyValuePair<string, AbstractObjectBlock> entry in objectPool)
		{
		    entry.Value.Pause();
		}
	}

	public void Resume()
	{		
		foreach(KeyValuePair<string, AbstractObjectBlock> entry in objectPool)
		{
		    entry.Value.Resume();
		}
	}

	public GameObject GetSngletonObject(string name)
	{
		try
		{
			return objectPool[name].GetSngletonObject();
		}
		catch
		{
			MyLogger.Red("GetSngletonObject", name);
			return null;
		}
	}

	public GameObject GetIdleObject(string name)
	{
		try
		{
			return objectPool[name].GetIdleObject();
		}
		catch
		{
			MyLogger.Red("GetIdleObject", name);
			return null;
		}
	}

    //<<-----------------------------------------------------------------------
    public T GetIdleObject<T>(string name) where T : GameEntity
    {
        return objectPool[name].GetIdleObject().GetComponent<T>();
    }

	public Transform GetInitialParent(string objectName)
	{
		objectName = objectName.Replace("(Clone)", "");
		return objectPool[objectName].GetInitialParent();
	}

	public void ResetUsedObject(GameEntity entity)
	{ 
        // MyLogger.Red("ResetUsedObject", entity.gameObject.name);
		entity.transform.parent 		= this.GetInitialParent(entity.gameObject.name);
		entity.transform.localScale 	= Vector3.one;
	}

	public bool IsGrowOn()
	{
		return this.GrowOnLimitReached;
	}
}

[System.Serializable]
public class AbstractObjectBlock
{	
	[SerializeField]	private  GameObject 		objectPrefab;
	
	private  int  									initialSize = 50; 
	private  Transform 								initialParent;
	
	private  List <GameEntity> 						objectsInPool;
	private  string 								objectName;

	public AbstractObjectBlock(GameObject _prefab, Transform _parent,  string prefix = "", int size = 50)
	{
		this.objectPrefab 	= _prefab;
		this.objectsInPool 	= new List<GameEntity>();
		this.initialParent 	= _parent;

		this.initialSize 	= size;

		this.objectName 	= prefix+_prefab.name;

		this.CreateInitialSize();			
	}

	public Transform GetInitialParent()
	{
		return this.initialParent;
	}

	private void CreateInitialSize()
	{
		this.objectsInPool = new List<GameEntity> ();
		for ( int i = 0 ; i < this.initialSize ; i++ )
		{
			this.AddNewGameEntity();
		}
	}

	private void AddNewGameEntity()
	{	
		GameObject clone = Creater.Clone(objectPrefab);
		clone.SetActive(false);
		clone.transform.parent  	= this.initialParent;
		clone.transform.localRotation 	= Quaternion.identity;
		clone.transform.localPosition   = Vector3.zero;
		clone.transform.localScale 		= Vector3.one;
		clone.name 				= this.objectName;

		GameEntity gameEntity = clone.GetComponent<GameEntity>();
		if ( gameEntity == null )
		{
			MyLogger.LogError("AddNewGameEntity", string.Format(" entity named {0} has no gameEntity property",this.objectName) );
			return;
		}
		gameEntity.SetAlive(false);
		this.objectsInPool.Add (gameEntity);
	}

	public void SleepAfterInit()
	{
		for ( int i = 0 ; i < this.initialSize ; i++ )
		{
			this.objectsInPool[i].DestroySelf();
		}
	}

	public void GrowSize(int sizeToGrow)
	{

		for ( int i = 0 ; i < sizeToGrow ; i++ )
		{
			this.AddNewGameEntity();
		}
	}

	public GameObject GetSngletonObject()
	{
		return this.objectsInPool[0].gameObject;
	}

	public GameObject GetIdleObject()
	{
		for ( int i = 0 ; i < this.objectsInPool.Count ; i++ )
		{
			if ( this.objectsInPool[i].IsAlive() == false )
			{
				this.objectsInPool[i].SetAlive(true);
				return this.objectsInPool[i].gameObject;
			}
		}

		if( AbstractObjectPooler.instance.IsGrowOn() )
		{
			MyLogger.LogError("OBJECT_POOL"+objectPrefab.name, "No Object Is Available, Grow object pool..");

			int firstUnusedIndex = this.objectsInPool.Count;

			this.GrowSize(this.initialSize / 2);

			this.objectsInPool[firstUnusedIndex].SetAlive(true);
			return this.objectsInPool[firstUnusedIndex].gameObject;
		}
		else
		{
			MyLogger.LogError("OBJECT_POOL"+objectPrefab.name, "No Object Is Available, Set GrowOnLimitReached on");
			return null;
		}
	}

	public string GetObjectName()
	{
		return this.objectName;
	}

	public string GetPrefabObjectName()
	{
		return this.objectPrefab.name;
	}

	public void ChangeParent (Transform newParent)
	{		
		this.initialParent.parent   = newParent.parent;
		this.initialParent 			= newParent;
	}

	public void Pause()
	{		
		for ( int i = 0 ; i < this.objectsInPool.Count ; i++ )
		{
			if ( this.objectsInPool[i].IsAlive() )
			{
				this.objectsInPool[i].GetComponent<GameEntity>().Pause();
			}
		}
	}

	public void Resume()
	{	
		for ( int i = 0 ; i < this.objectsInPool.Count ; i++ )
		{
			if ( this.objectsInPool[i].IsAlive() )
			{
				this.objectsInPool[i].GetComponent<GameEntity>().Resume();
			}
		}
	}

	public void Clear()
	{		
		for ( int i = 0 ; i < this.objectsInPool.Count ; i++ )
		{
			GameObject.Destroy(this.objectsInPool[i].gameObject);
		}

        this.objectsInPool.Clear();
        GameObject.Destroy(this.initialParent.gameObject);
	}

	void OnDestroy()
	{
		this.Clear();
	}
}