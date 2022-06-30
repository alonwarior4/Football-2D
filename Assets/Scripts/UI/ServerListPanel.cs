using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class ServerListPanel : MonoBehaviour
{
    [SerializeField] Transform contentParent;
    [SerializeField] ServerPanel serverPanelPref;

    public static Action<DiscoveryResponse> OnServerFound;

    MyNetworkDiscovery networkDiscovery;

    List<ServerPanel> serverPanels = new List<ServerPanel>();

    Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();

    private void Awake()
    {
        OnServerFound += HandleOnServerFound;
    }

    private void Start()
    {
        networkDiscovery = ((MyNetworkManager)NetworkManager.singleton).GetComponent<MyNetworkDiscovery>();
    }

    public void HandleOnServerFound(DiscoveryResponse response)
    {
        if (discoveredServers.ContainsKey(response.serverId)) return;

        ServerPanel panel = Instantiate(serverPanelPref, contentParent);

        panel.SetPanelData(response);
        panel.SetNetworkDiscovery(networkDiscovery);

        serverPanels.Add(panel);

        discoveredServers.Add(response.serverId, response);
    }

    public void RefreshServerList()
    {
        networkDiscovery.StopDiscovery();
        ClearServerList();
        networkDiscovery.StartDiscovery();        
    }

    public void ClearServerList()
    {
        discoveredServers.Clear();

        for (int i = 0; i < serverPanels.Count; i++)
        {
            ServerPanel panel = serverPanels[i];
            serverPanels.Remove(panel);
            Destroy(panel.gameObject);
        }
    }

    private void OnDestroy()
    {
        OnServerFound -= HandleOnServerFound;
    }
}
