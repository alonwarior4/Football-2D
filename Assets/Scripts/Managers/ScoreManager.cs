using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using TMPro;

public class ScoreManager : NetworkBehaviour
{
    [Header("Timer")]
    public static int matchTimeSec = -1;
    int totalRemainTime;
    float timePassed = 0;

    [SerializeField] int teamScore1;
    [SerializeField] int teamScore2;

    [Header("Refrences")]
    [SerializeField] TMP_Text scoreTxt1;
    [SerializeField] TMP_Text scoreTxt2;
    [SerializeField] TMP_Text timerTxt; 

    public static Action<int> OnTeamGoal;        //Zombie 1 , Human 2    



    #region Server...

    [Server]
    private void OnServerHandleTeamGoal(int team)
    {
        RpcHandleTeamGoal(team);
    }

    public override void OnStartServer()
    {
        totalRemainTime = matchTimeSec;
    }
    
    private void OnServerGameFinished()
    {
        int winnerTeam = default;

        if (teamScore1 > teamScore2)
            winnerTeam = 0;
        else if (teamScore2 > teamScore1)
            winnerTeam = 1;
        else
            winnerTeam = -1;

        RpcGameFinished(winnerTeam);
        RpcDisableUI();
        RpcDisablePlayerInput();
    }

    [ServerCallback]
    private void Update()
    {
        if (timePassed >= 1)
        {
            timePassed = 0;
            totalRemainTime--;
            RpcSyncTimer(totalRemainTime);

            if (totalRemainTime == 0)
                OnServerGameFinished();
        }
        else
            timePassed += Time.deltaTime;
    }

    #endregion


    #region Client...

    [ClientCallback]
    private void Start()
    {
        int min = matchTimeSec / 60;
        int sec = matchTimeSec - (min * 60);

        timerTxt.text = $"{min} : {sec}";
    }

    [ClientRpc]
    private void RpcDisablePlayerInput()
    {
        GameplayUI.OnGameFinished?.Invoke();
    }

    [ClientRpc]
    private void RpcDisableUI()
    {
        gameObject.SetActive(false);
    }

    [ClientRpc]
    private void RpcGameFinished(int winnerTeam)
    {
        SB_Player player = NetworkClient.connection.identity.GetComponent<SB_Player>();

        if (winnerTeam == -1)
        {            
            WinLoseUI.OnEven?.Invoke();
        }
        else
        {
            int playerTeam = player.PlayerType == PlayerType.Zombie ? 0 : 1;

            if (playerTeam == winnerTeam)
                WinLoseUI.OnWin?.Invoke();
            else
                WinLoseUI.OnLose?.Invoke();            
        }        
    }

    [ClientRpc]
    private void RpcSyncTimer(int remainTime)
    {
        SetTimerText(remainTime);
    }

    private void SetTimerText(int remainingTime)
    {
        int min = remainingTime / 60;
        int sec = remainingTime - (min * 60);

        timerTxt.text = $"{min} : {sec}";
    }

    public override void OnStartClient()
    {
        OnTeamGoal += OnServerHandleTeamGoal;
    }

    public override void OnStopClient()
    {
        OnTeamGoal -= OnServerHandleTeamGoal;
    }

    [ClientRpc]
    private void RpcHandleTeamGoal(int team)
    {
        if (team == 1)
        {
            teamScore1++;
            scoreTxt1.text = teamScore1.ToString();
        }
        else
        {
            teamScore2++;
            scoreTxt2.text = teamScore2.ToString();
        }

        if (teamScore1 > teamScore2)
            DancerManager.OnZombiesHighScore?.Invoke();
        else if (teamScore2 > teamScore1)
            DancerManager.OnHumanHighScore?.Invoke();
        else
            DancerManager.OnDancersIdeal?.Invoke();
    }

    #endregion   
    
}
