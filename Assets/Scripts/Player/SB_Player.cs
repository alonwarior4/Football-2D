using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum PlayerState
{
    Ideal, Run, Air , Fall
}

public enum PlayerType
{
    Human , Zombie
}

public class SB_Player : NetworkBehaviour
{
    [SerializeField] PlayerType playerType;
    public PlayerType PlayerType => playerType;

    [Header("Gameplay")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float ballKickVel = 20;
    [SerializeField] float fallGravityMultiplier = 10;
    [SerializeField] float groundHitBallVel = 15;
    [SerializeField] float air_BallKickVelMuli = 1.5f;
    
    Animator playerAnim;
    NetworkAnimator playerNetAnim;    

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

    [SerializeField] Collider2D playerHitCollider;    
    [SerializeField] Collider2D groundHitCollider;

    Collider2D ballCollider;
    Collider2D playerCollider;

    [Header("Hit")]
    [SerializeField] Transform headPos;
    [SerializeField] Transform footPos;

    Rigidbody2D playerRB;

    bool isGrounded;
    bool IsGrounded
    {
        get => isGrounded;
        set
        {
            isGrounded = value;
            if (!isGrounded)
                State = PlayerState.Air;
        }
    }

    PlayerState state;
    PlayerState State
    {
        get => state;
        set
        {
            state = value;
            SetAnimationByState().Forget();
        }
    }

    CancellationToken cToken;
    
    async UniTaskVoid SetAnimationByState()
    {
        await UniTask.NextFrame(cancellationToken : cToken);

        switch (state)
        {
            case PlayerState.Ideal:                
                playerAnim.SetTrigger(idealID);
                playerNetAnim.SetTrigger(idealID);
                break;

            case PlayerState.Run:                
                playerAnim.SetTrigger(runID);
                playerNetAnim.SetTrigger(runID);
                break;

            case PlayerState.Air:                
                playerAnim.SetTrigger(airID);
                playerNetAnim.SetTrigger(airID);
                break;

            case PlayerState.Fall:
                playerAnim.SetTrigger(fallID);
                playerNetAnim.SetTrigger(fallID);
                break;            

            default:
                break;
        }
    }

    //Anim Parameters
    int idealID;
    int runID;
    int airID;
    int headAttackID;
    int footAttackID;
    int fallID;    

    float defaultGravityScale = default;



    #region Server    

    [Command]
    private void CmdAddVelocityToBall(Vector2 velocity)
    {        
        ballCollider.attachedRigidbody.velocity += velocity;
    }

    [Command]
    private void CmdPlaySmokeAnimation(Vector3 pos)
    {
        RpcPlaySmokeAnimation(pos);
    }

    #endregion


    #region Client           

    public void SetPlayerType(PlayerType type) => playerType = type;    

    [ClientRpc]
    private void RpcPlaySmokeAnimation(Vector3 pos)
    {
        SmokeManager.OnPlaySmoke?.Invoke(pos);
    }

    [ClientCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[8];
        int contactCount = playerCollider.GetContacts(contacts);

        for (int i = 0; i < contactCount; i++)
        {
            float dotValue = Vector2.Dot(contacts[i].normal, Vector2.up);
            if (dotValue > 0.9f)
            {
                IsGrounded = true;
                CheckGroundHit();
                return;
            }
        }

        IsGrounded = false;
    }

    [ClientCallback]
    private void OnCollisionExit2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[8];
        int contactCount = playerCollider.GetContacts(contacts);

        if (contactCount == 0)
        {
            IsGrounded = false;
            return;
        }

        for (int i = 0; i < contactCount; i++)
        {
            float dotValue = Vector2.Dot(contacts[i].normal, Vector2.up);
            if (dotValue > 0.9f)
            {
                IsGrounded = true;
                return;
            }
        }

        IsGrounded = false;
    }

    [ClientCallback]
    public void SetAnimation()
    {
        if (!isLocalPlayer) return;

        SetAnimationByState().Forget();
    }

    [ClientCallback]
    private void Update()
    {
        if (!isLocalPlayer) return;
        if (!NetworkClient.ready) return;
        
        HandlePlayerMovement();
    }    

    private void OnClientHandleLeftSideMovement(int sign)
    {
        moveSign = sign;
    }

    private void OnClientHandleRightSideTap()
    {
        ClientCheckForAttack();
    }

    private void OnClientHandleRightSideDrag(int sign)
    {
        if (sign > 0)
            ClientJump();
        else
        {
            if (IsGrounded || State == PlayerState.Fall) return;
            playerRB.gravityScale *= fallGravityMultiplier;            

            State = PlayerState.Fall;
        }
    }

    private void CheckGroundHit()
    {
        if (!NetworkClient.ready) return;

        if (State == PlayerState.Fall)
        {
            CmdPlaySmokeAnimation(transform.position);

            if (groundHitCollider.IsTouching(ballCollider))
            {
                CmdAddVelocityToBall(Vector2.up * groundHitBallVel);                
            }
        }
        else
            State = PlayerState.Ideal;
    }    

    public void SetBallCollider(Collider2D ballCollider)
    {
        this.ballCollider = ballCollider;
    }

    public override void OnStartClient()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        if (hasAuthority)
        {   
            defaultGravityScale = playerRB.gravityScale;        

            MovePanel.OnLeftSideDrag += OnClientHandleLeftSideMovement;
            AttackPanel.OnRightSideDrag += OnClientHandleRightSideDrag;
            AttackPanel.OnRightSideTap += OnClientHandleRightSideTap;            
        }
        else
        {            
            Destroy(playerRB);
            Destroy(playerCollider);
        }

        DontDestroyOnLoad(this);
        
        playerAnim = GetComponent<Animator>();
        playerNetAnim = GetComponent<NetworkAnimator>();
        DefineAnimatorParameterIDS();

        _NetworkManager.GamePlayers.Add(this);
        cToken = this.GetCancellationTokenOnDestroy();
    }

    private void DefineAnimatorParameterIDS()
    {
        idealID = Animator.StringToHash("Ideal");
        runID = Animator.StringToHash("Run");
        airID = Animator.StringToHash("Air");
        fallID = Animator.StringToHash("Fall");        

        headAttackID = Animator.StringToHash("Head Attack");
        footAttackID = Animator.StringToHash("Foot Attack");
    }

    public override void OnStopClient()
    {
        _NetworkManager.GamePlayers.Remove(this);

        if (hasAuthority)
        {
            MovePanel.OnLeftSideDrag -= OnClientHandleLeftSideMovement;
            AttackPanel.OnRightSideDrag -= OnClientHandleRightSideDrag;
            AttackPanel.OnRightSideTap -= OnClientHandleRightSideTap;
        }
    }

    private void ClientCheckForAttack()
    {
        Vector2 ballVelocity = default;
        
        bool isTouchingBall = playerHitCollider.IsTouching(ballCollider);        

        if (isTouchingBall)
        {
            Vector2 ballPos = ballCollider.transform.position;

            float headSqrDis = (ballCollider.transform.position - headPos.position).sqrMagnitude;            
            float footSqrDis = (ballCollider.transform.position - footPos.position).sqrMagnitude;

            Vector2 randomPointInGate = default;
            GatePos forwardGate = default;

            if (transform.right.x > 0)            
                forwardGate = FootballGate.instance.rightGate;                            
            else            
                forwardGate = FootballGate.instance.leftGate;                            

            randomPointInGate = new Vector2(
                    Random.Range(forwardGate.min.x, forwardGate.max.x),
                    Random.Range(forwardGate.min.y, forwardGate.max.y)
                    );

            Vector2 dir = (randomPointInGate - ballPos).normalized;
            ballVelocity = dir * ballKickVel * (State == PlayerState.Air? air_BallKickVelMuli : 1);

            if (footSqrDis < headSqrDis)
            {   
                playerAnim.SetTrigger(footAttackID);
                playerNetAnim.SetTrigger(footAttackID);            
            }
            else
            {   
                playerAnim.SetTrigger(headAttackID);
                playerNetAnim.SetTrigger(headAttackID);            
            }
        }
        else
        {
            int randomNum = Random.Range(0, 101);
            if(randomNum <= 50)
            {
                playerAnim.SetTrigger(headAttackID);
                playerNetAnim.SetTrigger(headAttackID);
            }
            else
            {
                playerAnim.SetTrigger(footAttackID);
                playerNetAnim.SetTrigger(footAttackID);
            }
        }

        CmdAddVelocityToBall(ballVelocity);                
    }

    private void ClientJump()
    {
        if (IsGrounded)
        {
            playerRB.gravityScale = defaultGravityScale;
            playerRB.velocity += Vector2.up * jumpForce;
        }
    }

    int moveSign = default;
    private void HandlePlayerMovement()
    {
        if (moveSign != 0)
        {
            if (moveSign > 0)
                transform.rotation = Quaternion.identity;
            else
                transform.eulerAngles = new Vector3(0, 179.99f, 0);

            if (IsGrounded)
            {
                if (State != PlayerState.Run)
                    State = PlayerState.Run;
            }
        }
        else
        {
            if (IsGrounded)
            {
                if (State != PlayerState.Ideal)
                    State = PlayerState.Ideal;
            }
        }

        playerRB.velocity = new Vector2(moveSign * moveSpeed, playerRB.velocity.y);
    }

    #endregion    
}
