using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Ch_Controller : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    Rigidbody2D playerRB;
    Animator playerAnim;
    Collider2D playerCollider;

    [Header("UI")]
    [SerializeField] float increaseTime;
    [SerializeField] float decreaseTime;

    [Header("Hit")]
    [SerializeField] Transform headPos;
    [SerializeField] Transform handPos;
    [SerializeField] Transform footPos;

    [Header("Colliders")]
    [SerializeField] Collider2D ballCollider;
    [SerializeField] Collider2D hitCollider;

    PlayerState state;
    PlayerState State
    {
        get => state;
        set
        {
            state = value;
            SetAnimation();
        }
    }

    public void SetAnimation()
    {
        switch (state)
        {
            case PlayerState.Ideal:
                playerAnim.SetTrigger("Ideal");
                break;

            case PlayerState.Run:
                playerAnim.SetTrigger("Run");
                break;

            case PlayerState.Air:
                playerAnim.SetTrigger("Air");
                break;

            default:
                break;
        }
    }

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


    private void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        State = PlayerState.Ideal;
    }

    private void Update()
    {
        HandleKeyboardInputs();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[8];
        int contactCount = playerCollider.GetContacts(contacts);

        for(int i=0; i< contactCount; i++)
        {
            float dotValue = Vector2.Dot(contacts[i].normal , Vector2.up);
            if(dotValue > 0.9f)
            {
                IsGrounded = true;
                return;
            }
        }

        IsGrounded = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        ContactPoint2D[] contacts = new ContactPoint2D[8];
        int contactCount = playerCollider.GetContacts(contacts);
        if (contactCount == 0)
        {
            IsGrounded = false;
            return;
        }

        for(int i=0; i< contactCount; i++)
        {
            float dotValue = Vector2.Dot(contacts[i].normal , Vector2.up);
            if(dotValue > 0.9f)
            {
                IsGrounded = true;
                return;
            }
        }

        IsGrounded = false;
    }
    
    void HandleKeyboardInputs()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            JumpButton();

        if (Input.GetKeyDown(KeyCode.P))
            AttackButton();

        float horizontal = Input.GetAxisRaw("Horizontal");
        if(horizontal != 0)
        {
            if (horizontal > 0)
                transform.rotation = Quaternion.identity;
            else
                transform.eulerAngles = new Vector3(0, 179.99f, 0);

            if(IsGrounded)
            {
                if(State != PlayerState.Run)
                {
                    State = PlayerState.Run;
                }
            }
        }
        else
        {
            if (IsGrounded)
            {
                if (State != PlayerState.Ideal)
                {
                    State = PlayerState.Ideal;
                }
            }
        }

        playerRB.velocity = new Vector2(horizontal * moveSpeed, playerRB.velocity.y);        
    }
    

    public void JumpButton()
    {
        if (IsGrounded)
        {
            playerRB.velocity += Vector2.up * jumpForce;           
        }
    }

    public void AttackButton()
    {        
        if (hitCollider.IsTouching(ballCollider))
        {
            float headSqrDis = (ballCollider.transform.position - headPos.position).sqrMagnitude;
            float handSqrDis = (ballCollider.transform.position - handPos.position).sqrMagnitude;
            float footSqrDis = (ballCollider.transform.position - footPos.position).sqrMagnitude;

            float nearest = Mathf.Min(headSqrDis, handSqrDis, footSqrDis);
            float x = transform.right.x > 0 ? 1 : -1;
            if(nearest == headSqrDis)
            {
                ballCollider.attachedRigidbody.velocity += new Vector2(x, -1) * 15;
                playerAnim.SetTrigger("Head Attack");
            }
            else if(nearest == handSqrDis)
            {
                ballCollider.attachedRigidbody.velocity += new Vector2(x, 0) * 15;
                playerAnim.SetTrigger("Hand Attack");
            }
            else
            {
                ballCollider.attachedRigidbody.velocity += new Vector2(x, 1) * 15;
                playerAnim.SetTrigger("Foot Attack");
            }
        }
        else
        {
            int randomNum = Random.Range(1, 4);
            switch (randomNum)
            {
                case 1:
                    playerAnim.SetTrigger("Head Attack");
                    break;
                case 2:
                    playerAnim.SetTrigger("Hand Attack");
                    break;
                case 3:
                    playerAnim.SetTrigger("Foot Attack");
                    break;

                default:
                    break;
            }
        }
    }
}
