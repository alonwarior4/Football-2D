using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectPoolSupport : MonoBehaviour
{
    Animation_SS anim_ss;

    private void Awake()
    {
        anim_ss = GetComponent<Animation_SS>();
        anim_ss.OnAnimationEnds += Reserve;
    }

    public void Reserve()
    {
        SmokeManager.OnSmokeEnds?.Invoke(gameObject);
    }

    private void OnDestroy()
    {
        anim_ss.OnAnimationEnds -= Reserve;
    }
}
