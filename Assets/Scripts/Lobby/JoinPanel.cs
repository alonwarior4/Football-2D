using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Mirror;

public class JoinPanel : MonoBehaviour
{
    [SerializeField] GameObject mainMenuPanel;
    MyNetworkManager networkManager;
    [SerializeField] TMP_InputField ipAddressInput;
    [SerializeField] Button joinBtn;
    

    private void OnEnable()
    {
        MyNetworkManager.OnClientConnected += HandleOnClientConnected;
        MyNetworkManager.OnClientDisconnected += HandleOnClientDisconnected;
    }

    private void OnDisable()
    {
        MyNetworkManager.OnClientConnected -= HandleOnClientConnected;
        MyNetworkManager.OnClientDisconnected -= HandleOnClientDisconnected;
    }

    private void HandleOnClientDisconnected()
    {
        joinBtn.interactable = true;
    }

    private void HandleOnClientConnected()
    {
        joinBtn.interactable = true;
        gameObject.SetActive(false);
        mainMenuPanel.SetActive(false);
    }

    private void Start()
    {
        networkManager = (MyNetworkManager)NetworkManager.singleton;
        SetJoinButtonState();
    }

    public void SetJoinButtonState()
    {
        joinBtn.interactable = !string.IsNullOrEmpty(ipAddressInput.text);
    }

    public void JoinTheGame()
    {
        networkManager.networkAddress = ipAddressInput.text;
        networkManager.StartClient();

        joinBtn.interactable = false;
    }


}
