
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PatternTemplate : BaseWidgetEntity
{
    protected   Transform      startLeft;
    protected   Transform      endLeft;
    protected   Transform      startRight;
    protected   Transform      endRight;

    [SerializeField]    List<Transform>             gemPatternList;   

    void Awake()
    {
        this.startLeft  = this.GetComponent<Transform>("Anchor 01");
        this.startRight = this.GetComponent<Transform>("Anchor 02");
        this.endLeft    = this.GetComponent<Transform>("Anchor 03");
        this.endRight   = this.GetComponent<Transform>("Anchor 04");
    }  

    public  Transform   GetNewPattern()
    {        
        int random          = Random.Range(0,this.gemPatternList.Count);
        return this.gemPatternList[random];
    } 

    public  Vector3 GetPointFromLeft(float fx, float fy)
    {
        return Vector3.Lerp( this.GetPointInLeft(fy), this.GetPointInRight(fy), fx );
    }

    public  Vector3 GetPointInLeft(float frac)
    {
        return Vector3.Lerp(this.startLeft.position, this.endLeft.position, frac);
    }

    public  Vector3 GetPointInRight(float frac)
    {
        return Vector3.Lerp(this.startRight.position, this.endRight.position, frac);
    }
}