using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeManager : MonoBehaviour
{
    [SerializeField] GameObject smokePref;

    public static Action<Vector2> OnPlaySmoke;
    public static Action<GameObject> OnSmokeEnds;

    Queue<GameObject> smokePool = new Queue<GameObject>();

    private void Awake()
    {
        OnPlaySmoke += PlaySmoke;
        OnSmokeEnds += EndSmoke;
    }

    private void PlaySmoke(Vector2 pos)
    {
        GameObject smokePref = GetSmokePref();
        smokePref.transform.position = pos;
        smokePref.SetActive(true);
    }

    private void EndSmoke(GameObject smokeObj)
    {
        smokeObj.SetActive(false);
        smokePool.Enqueue(smokeObj);
    }

    private GameObject GetSmokePref()
    {
        if (smokePool.Count > 0)
            return smokePool.Dequeue();
        else
        {
            GameObject smokeObj = Instantiate(smokePref);
            return smokeObj;
        }
    }

    private void OnDestroy()
    {
        OnPlaySmoke -= PlaySmoke;
        OnSmokeEnds -= EndSmoke;
    }
}
