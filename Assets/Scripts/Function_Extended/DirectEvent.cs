using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectEvent : MonoBehaviour
{
    public DirectEvent()
    {
        this.animationList      = null;
        this.gameObjectList     = null;
        this.gameObjectIndex    = 0;
        this.aniIndex           = 0;
    }


    [System.Serializable]
    public class AnimationObject
    {
        public string AnimationName;
        public GameObject AnimationTarget;
    }

    public  List<AnimationObject>            animationList;
    public  List<GameObject>                 gameObjectList;
    public  float                            gameObjectIndex;
    private int                              aniIndex;

    public void PlayAnimation()
    {
        if (animationList.Count != 0 && animationList.Count > aniIndex)
        {
            AnimationObject aniObj = animationList[aniIndex++];

            if (aniObj.AnimationTarget != null)
            {
                if (aniObj.AnimationTarget.GetComponent<Animation>() != null)
                {
                    if (aniObj.AnimationName != "")
                    {
                        aniObj.AnimationTarget.GetComponent<Animation>().Play(aniObj.AnimationName);
                    }
                    else
                    {
                        aniObj.AnimationTarget.GetComponent<Animation>().Play();
                    }
                }
                else
                {
                    MyLogger.Log("DEBUG","Target animation is null");
                }
            }
            else
            {
				MyLogger.Log("DEBUG","Target is null");
            }
        }
    }

    public void SetActiveTrue()
    {
        int gameObjectIndex = (int)this.gameObjectIndex;
        if (gameObjectList.Count > gameObjectIndex)
        {
            if (gameObjectIndex >= 0)
            {
                gameObjectList[gameObjectIndex].SetActive(true);
            }
            else
            {
				MyLogger.Log("DEBUG","GameObject Index error");
            }
        }
        else
        {
			MyLogger.Log("DEBUG","GameObject Index error");
        }
    }

    public void SetActiveFalse()
    {
        int gameObjectIndex = (int)this.gameObjectIndex;
        if (gameObjectList.Count > gameObjectIndex)
        {
            if (gameObjectIndex >= 0)
            {
                gameObjectList[gameObjectIndex].SetActive(false);
            }
            else
            {
				MyLogger.Log("DEBUG","GameObject Index error");
            }
        }
        else
        {
			MyLogger.Log("DEBUG","GameObject Index error");
        }
    }
}
