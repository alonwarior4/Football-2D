using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using System;

public class RoomLobby : NetworkBehaviour
{
    [Header("Base Canvas Ref")]
    [SerializeField] GameObject lobbyCanvas;

    [Header("UI")]
    [SerializeField] Color teamColor1;
    [SerializeField] Color teamColor2;
    [SerializeField] Color noPlayerColor;
    [SerializeField] TMP_Text roomText;
    [SerializeField] GameObject startBtnObj;
    [SerializeField] GameObject matchTimerObj;
    [SerializeField] TMP_InputField inputTime;

    [Header("Player states")]
    [SerializeField] TMP_Text[] playerNames;
    [SerializeField] Image[] teamColors;
    [SerializeField] TMP_Text[] playerReadyStates;

    public bool isLeader;

    [SyncVar(hook = nameof(HandleSetRoomName))]
    string roomName;    

    [SyncVar(hook = nameof(HandleOnTeamChanged))]
    public int team;

    [SyncVar(hook = nameof(HandleOnReadyStateChanged))]
    public bool isReady;

    [SyncVar(hook = nameof(HandleOnPlayerNameChanged))]
    public string playerName;

    MyNetworkManager _networkManager;
    MyNetworkManager _NetworkManager
    {
        get
        {
            if (_networkManager == null)
                _networkManager = (MyNetworkManager)(NetworkManager.singleton);

            return _networkManager;
        }
    }    


    #region Server
    
    public void SetRoomName(string roomName) => this.roomName = roomName;

    public void SetTeam(int team) => this.team = team;

    public void SetReadyState(bool readyState) => isReady = readyState;

    [Command]
    private void CmdSetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }

    [Command]
    public void CmdChangeTeam()
    {
        team = team == 0 ? 1 : 0;
    }

    [Command]
    public void CmdChangeReadyState()
    {
        isReady = !isReady;
    }

    [Command]
    public void CmdStartGame()
    {
        _NetworkManager.StartGame();
    }

    #endregion

    private void ResetAllLobbyPlayerParts()
    {
        for(int i =0; i< 4; i++)
        {
            playerNames[i].text = "Waiting For Player ...";
            teamColors[i].color = noPlayerColor;
            playerReadyStates[i].text = "Not Ready";
        }
    }

    private void SetPlayerParts()
    {  
        for (int i = 0; i < _NetworkManager.RoomPlayers.Count; i++)
        {
            playerNames[i].text = _NetworkManager.RoomPlayers[i].playerName;
            teamColors[i].color = _NetworkManager.RoomPlayers[i].team == 0 ? teamColor1 : teamColor2;
            playerReadyStates[i].text = _NetworkManager.RoomPlayers[i].isReady ? "<color=green>Ready</color>" :
                "<color=red>Not Ready</color>";
        }
    }

    private void UpdateDisplay()
    {
        if (!hasAuthority)
        {
            for(int i=0; i< _NetworkManager.RoomPlayers.Count; i++)
            {
                if (_NetworkManager.RoomPlayers[i].hasAuthority)
                {
                    _NetworkManager.RoomPlayers[i].UpdateDisplay();
                    break;
                }
            }

            return;
        }

        ResetAllLobbyPlayerParts();
        SetPlayerParts();
    }


    #region Client

    public void SetMatchTime()
    {
        try
        {
            ScoreManager.matchTimeSec = int.Parse(inputTime.text);
        }
        catch(Exception e)
        {
            ScoreManager.matchTimeSec = -1;
        }
    }    

    private void HandleOnPlayerNameChanged(string oldValue, string newValue) => UpdateDisplay();
    private void HandleOnReadyStateChanged(bool oldValue, bool newValue) => UpdateDisplay();
    private void HandleOnTeamChanged(int oldValue, int newValue) => UpdateDisplay();

    private void HandleSetRoomName(string oldName , string newName)
    {        
        roomText.text = newName + " Room";
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(InputNamePanel.DisplayName);        
    }

    public override void OnStartClient()
    {
        if (!hasAuthority)
            lobbyCanvas.SetActive(false);

        _NetworkManager.RoomPlayers.Add(this);

        startBtnObj.SetActive(isLeader);
        matchTimerObj.SetActive(isLeader);

        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        _NetworkManager.RoomPlayers.Remove(this);

        UpdateDisplay();
    }
    
    public void ExitLobby()
    {
        if (isLeader)
        {
            _NetworkManager.StopHost();
            _NetworkManager.GetComponent<MyNetworkDiscovery>().StopDiscovery();

            LobbyMenu.OnStopHost?.Invoke();
        }
        else        
            _NetworkManager.StopClient();                                    
    }


    #endregion
}
