using UnityEngine;
using System.Collections;

public class LoopingMapObject : MonoBehaviour 
{
    private float       previousLoopStart;
    private Vector3     defaultPosition;
    
    //[SerializeField]    
    // private             Transform 	cloneToLoop;
    float distanceBetweenClone  = 0;

    void Start()
    {
        // this.Initialize();
    }

    public void Initialize( float _distanceBetweenClone)
    {        
        this.previousLoopStart      = this.transform.position.y;
        this.distanceBetweenClone   = _distanceBetweenClone; //Mathf.Abs( cloneToLoop.position.y - this.transform.position.y );
        this.defaultPosition        = this.transform.position;
       
        /*MyLogger.Red(this.transform.parent.name+"/"+this.name, 
            "default position : " + this.defaultPosition +
            "local position : " + this.transform.localPosition);     */
    }

    public void Reset()
    {
        this.transform.position    = this.defaultPosition;
        this.previousLoopStart          = this.transform.position.y;

        //MyLogger.Red(this.transform.parent.name+"/"+this.name, "Reset");
    }

    /*public void Update () 
    {       
        Vector3 playerPosition = GamePlayerEntity.instance.GetTransform().position;

        if ( playerPosition.y < this.previousLoopStart - this.distanceToCheckLoop )        
        {
            this.MoveEverythingByDistance();//-Mathf.Abs(displacement));
        }
    }*/

    public  bool IsEnded(Vector3 playerPosition, float distanceToCheckLoop)
    {
        return playerPosition.y < this.previousLoopStart - distanceToCheckLoop;
    }

    public  void MoveEverythingByDistance(int multi)
    {
       //MyLogger.Red("MOVE", "player position : " + GamePlayerEntity.instance.GetTransform().position);
       //MyLogger.Red("MOVE STARTED", this.name + " position : " + this.transform.position);
       float distanceToMove = this.distanceBetweenClone * multi;
       this.transform.Translate (0,-distanceToMove,0);
       //MyLogger.Red("MOVE FINISHED", this.name + " position : " + this.transform.position);
       this.previousLoopStart = this.transform.position.y;
    }
}
