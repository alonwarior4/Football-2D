using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using System;

public class ServerPanel : MonoBehaviour
{
    [SerializeField] TMP_Text serverNameTxt;
    [SerializeField] TMP_Text playerCountTxt;
    DiscoveryResponse response;
    MyNetworkDiscovery networkDiscovery;


    private void Start()
    {
        Button panelButton = GetComponent<Button>();
        panelButton.onClick.AddListener(() => OnClickPanel());
    }

    private void OnClickPanel()
    {
        networkDiscovery.StopDiscovery();
        ((MyNetworkManager)NetworkManager.singleton).StartClient(response.uri);
    }

    public void SetPanelData(DiscoveryResponse response)
    {
        serverNameTxt.text = response.hostName;
        playerCountTxt.text = response.playerCount.ToString();

        this.response = response;
    }

    public void SetNetworkDiscovery(MyNetworkDiscovery discovery) => networkDiscovery = discovery;
}
