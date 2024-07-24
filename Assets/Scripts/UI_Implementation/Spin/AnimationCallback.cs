using UnityEngine;
using System.Collections;

namespace Waggle
{

public class AnimationCallback : MonoBehaviour 
{
    public GameObject rootObject;

    private void FinishedAnimation(string eventName)
    {
        rootObject.SendMessage(
            eventName, 
            gameObject,
            SendMessageOptions.DontRequireReceiver);
    }
}

}