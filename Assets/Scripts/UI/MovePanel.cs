using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MovePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Movement UI")]
    [SerializeField] GameObject circle;
    [SerializeField] GameObject arrow;

    [Header("Move Config")]
    [SerializeField] float minDelta;
    [SerializeField] float maxDelta;

    [Header("Arrow Size")]
    [SerializeField] Vector2 arrowMinSize;
    [SerializeField] Vector2 arrowMaxSize;

    RectTransform arrowTrans;
    RectTransform circleTrans;
    Vector3 defaultRot;

    Vector2 firstPos = default;

    public static Action<int> OnLeftSideDrag;

    private void Awake()
    {
        arrowTrans = arrow.GetComponent<RectTransform>();
        circleTrans = circle.GetComponent<RectTransform>();

        defaultRot = arrowTrans.localEulerAngles;

        circle.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        firstPos = eventData.position;

        circleTrans.position = firstPos;
        circle.SetActive(true);             
    }

    bool isRotationSet = false;
    int prevDeltaSign = 0;    

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = (eventData.position - firstPos).x;        
        
        if(deltaX > 0)
        {            
            if (!isRotationSet || (isRotationSet && prevDeltaSign != 1))
            {
                SetArrowRotation(1);
                OnLeftSideDrag?.Invoke(1);
            }
        }
        else
        {
            if (!isRotationSet || (isRotationSet && prevDeltaSign != -1))
            {
                SetArrowRotation(-1);
                OnLeftSideDrag?.Invoke(-1);
            }
        }            
            
        float t = Mathf.Clamp(InverseLerp(minDelta, maxDelta, Mathf.Abs(deltaX)) , 0 , 1);       
        
        arrowTrans.sizeDelta = Vector2.Lerp(arrowMinSize, arrowMaxSize, t);
    }

    void SetArrowRotation(int sign)
    {        
        Vector3 rot = defaultRot;

        if (sign == 1)
        {
            rot.y = 0;
            prevDeltaSign = 1;
        }
        else
        {
            rot.y = 180;
            prevDeltaSign = -1;
        }

        arrowTrans.localEulerAngles = rot;
        isRotationSet = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        circle.SetActive(false);

        prevDeltaSign = 0;
        OnLeftSideDrag?.Invoke(0);
    }

    private float InverseLerp(float min , float max , float value)
    {
        return (value - min) / (max - min);
    }
}
