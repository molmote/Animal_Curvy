using UnityEngine;
using System.Collections;

public static class AnimatorExtensions 
{
    public static bool IsPlaying(this GameObject obj, int layerIndex = 0)
    {
        return obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(layerIndex).normalizedTime <= 1.0f;
    }
}
