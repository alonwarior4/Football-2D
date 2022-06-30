using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    [Header("Game Config")]
    [SerializeField] int minPlayerCount = 2;

    [Header("UI")]
    [SerializeField] RoomLobby roomLobbyPrefab;

    [Header("Gameplay")]
    [SerializeField] SB_Player zombiePrefab;
    [SerializeField] SB_Player humanPrefab;
    [SerializeField] _Ball ballPref;
    [SerializeField] ScoreManager scoreManagerPref;

    [Header("Spawn Poses")]
    [SerializeField] Vector2[] leftPoses;
    [SerializeField] Vector2[] rightPoses;

    [SerializeField] List<RoomLobby> roomPlayers = new List<RoomLobby>();
    public List<RoomLobby> RoomPlayers => roomPlayers;

    [SerializeField] List<SB_Player> gamePlayers = new List<SB_Player>();
    public List<SB_Player> GamePlayers => gamePlayers;

    [Header("Scene Names")]
    [SerializeField] string lobbyScene;
    [SerializeField] string gamePlayScene;

    public override void OnClientConnect()
    {
        base.OnClientConnect();        

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(roomPlayers.Count == maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if(SceneManager.GetActiveScene().name != lobbyScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity != null)
        {
            var player = conn.identity.GetComponent<RoomLobby>();
            roomPlayers.Remove(player);
        }

        base.OnServerDisconnect(conn);        
    }

    string roomName;    

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        RoomLobby player = Instantiate(roomLobbyPrefab, Vector3.zero, Quaternion.identity);

        if (roomPlayers.Count == 0)
        {
            player.isLeader = true;
            roomName = InputNamePanel.DisplayName;                
        }

        player.SetRoomName(roomName);
        player.SetTeam(0);
        player.SetReadyState(false);

        NetworkServer.AddPlayerForConnection(conn, player.gameObject);        
    }

    public override void OnStopServer()
    {
        roomPlayers.Clear();
        gamePlayers.Clear();       
    }

    public void StartGame()
    {
        if (GetStartGameConditions() == false) return;

        ServerChangeScene(gamePlayScene);
        GetComponent<MyNetworkDiscovery>().StopDiscovery();        
    }

    public override void OnStopHost()
    {
        base.OnStopHost();
    }

    public override void ServerChangeScene(string newSceneName)
    {
        int leftIndex = 0;
        int rightIndex = 0;

        for(int i=0; i< roomPlayers.Count; i++)
        {
            var conn = roomPlayers[i].connectionToClient;

            var room = roomPlayers[i];
            Vector2 playerPos = default;
            SB_Player playerInstance = null;

            if (room.team == 0)
            {
                playerPos = leftPoses[leftIndex];
                playerInstance = Instantiate(zombiePrefab);                
                leftIndex++;
            }
            else
            {
                playerPos = rightPoses[rightIndex];
                playerInstance = Instantiate(humanPrefab);                
                rightIndex++;
            }

            playerInstance.transform.SetPositionAndRotation(playerPos , Quaternion.identity);            

            //TODO : Set Config for Each Player if needed
            NetworkServer.ReplacePlayerForConnection(conn, playerInstance.gameObject);    
        }

        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        for (int i = 0; i < roomPlayers.Count; i++)
        {
            NetworkServer.Destroy(roomPlayers[i].gameObject);
        }

        _Ball ball = Instantiate(ballPref , Vector3.zero , Quaternion.identity);
        NetworkServer.Spawn(ball.gameObject);

        ScoreManager scoreManager = Instantiate(scoreManagerPref, Vector3.zero, Quaternion.identity);
        NetworkServer.Spawn(scoreManager.gameObject);
    }

    private bool GetStartGameConditions()
    {
        if (roomPlayers.Count < minPlayerCount) return false;

        for (int i = 0; i < roomPlayers.Count; i++)
        {
            if (!roomPlayers[i].isReady)
                return false;
        }

        int team1Count = 0;
        int team2Count = 0;

        for (int i = 0; i < roomPlayers.Count; i++)
        {
            if (roomPlayers[i].team == 0)
                team1Count++;
            else
                team2Count++;
        }

        if (team1Count == 0 || team2Count == 0) return false;
        if (team1Count != team2Count) return false;
        if (ScoreManager.matchTimeSec == -1) return false;

        return true;
    }   
}
