using System.Net;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DiscoveryRequest : NetworkMessage { }

[Serializable]
public class DiscoveryResponse : NetworkMessage
{    
    public string hostName;
    public int playerCount;    

    public IPEndPoint endPoint { get; set; }
    public Uri uri;
    public long serverId;
}

public class MyNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{   
    public long serverId { get; private set; }
    public Transport transport;

    MyNetworkManager _networkManager;
    MyNetworkManager _NetworkManager
    {
        get
        {
            if (_networkManager == null)
                _networkManager = (MyNetworkManager)NetworkManager.singleton;

            return _networkManager;
        }
    }

    public override void Start()
    {
        serverId = RandomLong();

        if (transport == null)
            transport = Transport.activeTransport;

        base.Start();
    }

    #region Server   
    
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint) 
    {
        try
        {
            DiscoveryResponse response = new DiscoveryResponse();

            response.hostName = InputNamePanel.DisplayName;
            response.playerCount = _NetworkManager.numPlayers;
            response.serverId = serverId;
            response.uri = transport.ServerUri();

            return response;
        }
        catch (NotImplementedException)
        {
            print("Transport does not support");
            throw;
        }
    }

    #endregion

    #region Client

    protected override DiscoveryRequest GetRequest()
    {
        return new DiscoveryRequest();
    }

    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        response.endPoint = endpoint;

        UriBuilder builder = new UriBuilder(response.uri)
        {
            Host = response.endPoint.Address.ToString()
        };
        response.uri = builder.Uri;
        
        ServerListPanel.OnServerFound?.Invoke(response);
    }

    #endregion
}
