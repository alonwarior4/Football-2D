using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public enum DancerType
{
    Human , Zombie
}

public class Dancer : MonoBehaviour
{
    [SerializeField] DancerType type;
    public DancerType Type => type;

    Animator dancerAnim;

    int danceID;
    int idealID;    

    public void OnStart()
    {
        dancerAnim = GetComponent<Animator>();

        danceID = Animator.StringToHash("Dance");
        idealID = Animator.StringToHash("Ideal");
    }

    public void SetSortingLayers(int id)
    {
        SpriteRenderer[] spRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spRenderers.Length; i++)
        {
            spRenderers[i].sortingOrder += id * 10;
        }
    }

    public void SetLocalScale()
    {
        int randomVal = Random.Range(0, 101);
        if (randomVal > 50) return;

        Vector3 scale = transform.localScale;
        scale.x = scale.x > 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    public void StartDance() => dancerAnim.SetTrigger(danceID);
    public void GoIdeal() => dancerAnim.SetTrigger(idealID);
    
}
