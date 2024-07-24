using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class LoopingMapController : MonoBehaviour 
{
    [SerializeField]
    List<LoopingMapObject> objList;

    [SerializeField]
    float distanceToCheckLoop;

    void Start()
    {
        this.Initialize();
    }

    public void Initialize()
    {        
        float dist = Mathf.Abs( objList[1].transform.position.y - objList[0].transform.position.y );
        for ( int i = 0; i < objList.Count ; i++ )
        {
            objList[i].Initialize(dist);
        }
        // this.previousLoopStart      = this.transform.position.y;
        // this.distanceBetweenClone   = Mathf.Abs( this.cloneToLoop.position.y - this.transform.position.y );
        // this.defaultPosition        = this.transform.position;
    }

    public void Reset()
    {
        foreach ( LoopingMapObject obj in objList )
        {
            obj.Reset();
        }
    }

    void Update () 
    {       
        Vector3 playerPosition = GamePlayerEntity.instance.GetTransform().position;

        foreach ( LoopingMapObject obj in objList )
        {
            if ( obj.IsEnded(playerPosition, this.distanceToCheckLoop) )
            {
            	obj.MoveEverythingByDistance(objList.Count);
        	}
        }
    }
}
