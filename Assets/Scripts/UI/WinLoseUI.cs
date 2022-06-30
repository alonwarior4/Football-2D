using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] GameObject ExitButton;
    [SerializeField] UnityEvent onWinGame;
    [SerializeField] UnityEvent onLoseGame;
    [SerializeField] UnityEvent onEvenGame;

    public static Action OnWin;
    public static Action OnLose;
    public static Action OnEven;

    private void Awake()
    {
        OnWin += WinGame;
        OnLose += LoseGame;
        OnEven += EvenGame;
    }

    private void WinGame()
    {
        onWinGame?.Invoke();
        SB_Player player = NetworkClient.connection.identity.GetComponent<SB_Player>();
        if (!player.isClientOnly)
            ExitButton.SetActive(true);
    }

    private void LoseGame()
    {
        onLoseGame?.Invoke();
        SB_Player player = NetworkClient.connection.identity.GetComponent<SB_Player>();
        if (!player.isClientOnly)
            ExitButton.SetActive(true);
    }

    private void EvenGame()
    {
        onEvenGame?.Invoke();
        SB_Player player = NetworkClient.connection.identity.GetComponent<SB_Player>();
        if (!player.isClientOnly)
            ExitButton.SetActive(true);
    }

    public void ExitGame()
    {
        NetworkManager networkManager = (MyNetworkManager)NetworkManager.singleton;
        networkManager.StopHost();        
        SceneManager.LoadScene("Lobby Scene");
    }

    private void OnDestroy()
    {
        OnWin -= WinGame;
        OnLose -= LoseGame;
        OnEven -= EvenGame;
    }
}
