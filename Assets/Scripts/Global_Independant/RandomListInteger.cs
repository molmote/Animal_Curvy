using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[System.Serializable]
public class RandomListInteger//<int>
{
    public RandomListInteger(){}
    public RandomListInteger(List<int> list)
    {
        this.randomList     = list;
        this.range          = list.Count;
    } 

    public RandomListInteger( Range2 range)
    {
        this.randomList = new List<int>();
        for ( int i = (int)range.min; i < (int)range.max ; i++ )
        {
            this.randomList.Add(i);
        }
        this.range = this.randomList.Count;
    }
    
    public static int GetRandom(Range2 range, int exception)
    {
        // RandomListInteger random = new RandomListInteger(range);

        List<int> randomList = new List<int>();
        for ( int i = (int)range.min; i < (int)range.max ; i++ )
        {
            if ( i != exception )
            randomList.Add(i);
        }
        int _range           = randomList.Count;

        int index           = Next(_range);
        int value           = randomList[index];

        MyLogger.BlueObject("RandomListInteger", randomList);
        return value;
    }

    public void     Add(int num)
    {
        if( this.randomList == null )
        {
            this.randomList = new List<int>();
        }
        this.randomList.Add(num);
        this.range = this.randomList.Count;
    }

    public      List<int>   randomList;
    public      int         range;

    public  int     GetRandom()
    {
        if ( this.range < 1 )
        {
            MyLogger.LogError("GetRandom", "range Out of Range, fed up");
            return -1;
        }
        int index           = Next(this.range);
        int value           = randomList[index];
        this.range --;
        randomList[index]   = randomList[this.range];

        return value;
    }

    public  int     GetRandomSafe()
    {
        int value           = randomList[UnityEngine.Random.Range(0,this.range)];
        return value;
    }

    private static int     Next(int _range)
    {
        return UnityEngine.Random.Range(0,_range);
    }
}