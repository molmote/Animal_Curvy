using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
public class FollowerPlatform : BaseWidgetEntity
{
    List<UILabel>     countObtained;
    List<UISprite>    spriteObtained;
    List<Transform>   spriteNew;

    public void Initialize(int size)
    {
        this.countObtained  = new List<UILabel>();
        this.spriteObtained = new List<UISprite>();
        this.spriteNew      = new List<Transform>();
        for ( int index = 0; index < size ; index ++ )
        {   
            this.countObtained. Add( this.GetComponent<UILabel>(string.Format( "Label Count {0:d2}", index+1)) );
            this.spriteObtained.Add( this.GetComponent<UISprite>(string.Format( "Sprite Follower {0:d2}", index+1)) );
            this.spriteNew.Add     ( this.GetComponent<Transform>(string.Format( "New {0:d2}", index+1)) );
        }
    }

    public void UpdateInfo ( int index, int input )
    {
        this.countObtained[index].text     = "" + GamePlayerEntity.instance.currentFollowerTypes[input].count;
        string name = GamePlayerEntity.instance.currentFollowerTypes[input].name;
        this.spriteObtained[index].spriteName  = name;
        this.spriteObtained[index].MakePixelPerfect();

        this.spriteNew[index].gameObject.SetActive( UserInfo.instance.followerCountDict[name].status == LockStatus.NEW );
    }
}
*/