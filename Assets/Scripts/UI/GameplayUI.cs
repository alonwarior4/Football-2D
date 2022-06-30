using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] GameObject actionPanel;
    [SerializeField] GameObject movePanel;

    public static Action OnGameFinished;

    private void Awake()
    {
        OnGameFinished += HandleOnGameFinished;
    }

    private void HandleOnGameFinished()
    {
        actionPanel.SetActive(false);
        movePanel.SetActive(false);
    }


    private void OnDestroy()
    {
        OnGameFinished -= HandleOnGameFinished;
    }
}
