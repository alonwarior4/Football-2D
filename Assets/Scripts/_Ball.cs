using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class _Ball : NetworkBehaviour
{
    [SerializeField] Collider2D ballCollider;
    [SerializeField] Rigidbody2D ballRB;
    [SerializeField] TrailRenderer trailRenderer;
    
    public Collider2D BallCollider => ballCollider;
    string gateTag = "Gate";


    public override void OnStartServer()
    {
        ballRB.simulated = true;
    }



    #region Server...
    
    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(gateTag))
        {
            ballRB.simulated = false;
            transform.position = Vector3.zero;
            ballRB.simulated = true;
            ballRB.velocity = Vector3.zero;

            if (collision.collider.transform.position.x < 0)
                ScoreManager.OnTeamGoal?.Invoke(2);
            else
                ScoreManager.OnTeamGoal?.Invoke(1);
        }
    }

    #endregion



    #region Client...    

    public override void OnStartClient()
    {
        MyNetworkManager networkManager = (MyNetworkManager)NetworkManager.singleton;

        for(int i=0; i<networkManager.GamePlayers.Count; i++)
        {
            networkManager.GamePlayers[i].SetBallCollider(ballCollider);
        }

        trailRenderer.enabled = true;        

        if (!isClientOnly) return;

        ballRB.simulated = true;
        ballRB.bodyType = RigidbodyType2D.Kinematic;
        ballCollider.isTrigger = true;        
    }

    #endregion
}
