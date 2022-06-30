using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    public static int menuState = 0;
    [Header("menu refrences")]
    [SerializeField] GameObject inputNamePanel;
    [SerializeField] GameObject hostPanel;
    [SerializeField] GameObject serverListaPanel;    


    public static Action OnStopHost;

    private void Awake()
    {
        OnStopHost += HandleOnStopHost;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene , LoadSceneMode mode)
    {
        if(InputNamePanel.DisplayName != "")
        {
            HandleOnStopHost();
        }
    }    

    private void HandleOnStopHost()
    {
        gameObject.SetActive(true);

        inputNamePanel.SetActive(false);
        serverListaPanel.SetActive(false);

        hostPanel.SetActive(true);
    }

    public void BackToHostPanel()
    {
        serverListaPanel.SetActive(false);
        hostPanel.SetActive(true);
    }

    private void OnDestroy()
    {
        OnStopHost -= HandleOnStopHost;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }
}
