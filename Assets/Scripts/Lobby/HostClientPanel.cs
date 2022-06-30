using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HostClientPanel : MonoBehaviour
{
    MyNetworkManager networkManager;
    MyNetworkDiscovery networkDiscovery;

    private void Start()
    {
        networkManager = (MyNetworkManager)NetworkManager.singleton;
        networkDiscovery = networkManager.GetComponent<MyNetworkDiscovery>();
    }

    public void HostGame()
    {
        networkManager.StartHost();
        networkDiscovery.AdvertiseServer();          
    }    

    public void JoinButton()
    {
        networkDiscovery.StartDiscovery();
    }
    
}
