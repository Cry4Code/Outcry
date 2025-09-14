using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    #region 컴포넌트 관련
    [field : Header ("Components")]
    public BoxCollider2D boxCollider;
    
    #endregion
    
    
    #region 이동 관련

    [field : Header("Movement Settings")] 
    [field : SerializeField] public float MoveSpeed { get; set; }
    private Vector2 curMoveInput;
    [HideInInspector] public bool keyboardLeft = false;
    [HideInInspector] public bool lookLeft = false;
    
    #endregion
    
    #region 점프 관련
    [field : Header("Jump Settings")]
    [field : SerializeField] public float JumpForce { get; set; }
    [field : SerializeField] public float WallJumpForce { get; set; }
    [field : SerializeField] public float GroundThresholdForce { get; set; } // 땅으로 인식하는 법선 벡터 크기 조건
    [field: SerializeField] public float GroundThresholdDistance { get; set; } // 땅으로 인식하는 거리 최소 조건
    public LayerMask groundMask;
    [HideInInspector] public bool isGroundJump = false; // 지상에서 첫 점프 했는지
    [HideInInspector] public bool isDoubleJump = false; // 더블점프 했는지
    [HideInInspector] public Collider2D curWall;
    [HideInInspector] public bool isWallJumped = false; // 벽점 했는지
    [HideInInspector] public bool lastWallIsLeft = false; // 마지막에 부딛힌 벽이 왼쪽에 있는지
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isWallTouched = false;
    private Vector2 rightWallCheckPos;
    private Vector2 leftWallCheckPos;
    private Vector2 wallCheckBoxSize;
    private float checkDistance;

    #endregion

    #region 시야 관련
    [Header("Look Settings")]
    
    
    #endregion
    
    private Camera mainCam;

    [HideInInspector] public Rigidbody2D rb;
    
    private SpriteRenderer spriteRenderer;
    private bool isDodged = false;
    
    public PlayerController Controller { get; set; }
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
        Controller = GetComponent<PlayerController>();
        curWall = null;
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        checkDistance = boxCollider.size.x * 0.5f;

        wallCheckBoxSize = new Vector2(boxCollider.size.x * 0.1f, boxCollider.size.y * 0.85f);
        
    }


    /// <summary>
    /// JumpState 진입 시 한 번 불림.
    /// </summary>
    public void Jump()
    {
        if (isGroundJump) return;
        isGrounded = false;
        Debug.Log("Jump!");
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        isGroundJump = true;
    }
    
    /// <summary>
    /// DoubleJumpState 진입 시에 한 번 불림
    /// </summary>
    public void DoubleJump()
    {
        if (isDoubleJump) return;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        isDoubleJump = true;
    }

    public bool CanWallJump()
    {
        // 벽점 당시에 왼쪽 벽인지 아닌지 확인한 다음에 벽 체크
        rightWallCheckPos = (Vector2)transform.position + Vector2.right * checkDistance;
        leftWallCheckPos = (Vector2)transform.position + Vector2.left * checkDistance;

        Collider2D hit = Physics2D.OverlapBox(keyboardLeft? leftWallCheckPos : rightWallCheckPos, wallCheckBoxSize, 0f, groundMask);
        if(hit != curWall)
        {
            return true;
        }
        return false;
    }

    public void WallJump()
    {
        rightWallCheckPos = (Vector2)transform.position + Vector2.right * checkDistance;
        leftWallCheckPos = (Vector2)transform.position + Vector2.left * checkDistance;

        curWall = Physics2D.OverlapBox(keyboardLeft ? leftWallCheckPos : rightWallCheckPos, wallCheckBoxSize, 0f, groundMask);

        Vector2 dir = ((lastWallIsLeft ? Vector2.right : Vector2.left) + Vector2.up).normalized * WallJumpForce;
        rb.AddForce(dir, ForceMode2D.Impulse);

    }   

    public void ChangeGravity(bool holdWall)
    {
        if (holdWall) rb.gravityScale = 0.5f;
        else rb.gravityScale = 1f;
    }

    /// <summary>
    /// (임시용) 커서 보이게 하기 위함
    /// </summary>
    /// <param name="context"></param>
    public void OnPause(InputAction.CallbackContext context)
    {
        CursorManager.Instance.SetInGame(!CursorManager.Instance.IsInGame);
    }


    public void Stop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }


    /// <summary>
    /// Input에 맞는 움직임
    /// </summary>
    public void Move()
    {
        Vector2 moveInput = Controller.Inputs.Player.Move.ReadValue<Vector2>();
        rb.velocity = new Vector2(moveInput.x * MoveSpeed, rb.velocity.y);
        if (moveInput.x < 0) keyboardLeft = true;
        else if (moveInput.x > 0) keyboardLeft = false;

    }
   

    public void SetAnimation(string animName)
    {
        // Animator 호출
    }


    public void Look()
    {
        // 플레이어는 오른쪽을 봐야함.
        if (CursorManager.Instance.mousePosition.x > transform.position.x)
        {
            lookLeft = false;
        }
        // 플레이어는 왼쪽을 봐야함.
        else
        {
            lookLeft = true;
        }

        spriteRenderer.flipX = lookLeft;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("벽");
            UpdateGrounded(collision);
            UpdateWallTouched(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            UpdateGrounded(collision);
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x != 0)
                {
                    isWallTouched = true;
                    
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            isWallTouched = false;
        }
            
    }

    private void UpdateGrounded(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Debug.Log($"법선벡터 : {contact.normal}");
            // 캐릭터가 아래로 향하는 충돌에서만 grounded
            if (contact.normal.y > GroundThresholdForce)    
            {

                isGrounded = true;
                isDoubleJump = false;
                isGroundJump = false;
                isWallJumped = false;
                isWallTouched = false;
                curWall = null;
                return;
            }
        }

        // 위쪽으로 부딪히거나 옆으로 부딪히면 grounded 안 됨
        isGrounded = false;
    }

    private void UpdateWallTouched(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {

            if (contact.normal.x != 0)
            {
                isWallTouched = true;
                // 벽면에서 나오는 방향이 법선벡터이기 때문에, 왼쪽 벽으로 부딛혔다면 (1,0) 이 나옴.
                lastWallIsLeft = contact.normal.x > 0; 
                //if (curWall != collision.collider)
                //{
                //    Debug.Log("다른 벽 건드림");
                //    prevWall = curWall;
                //    curWall = collision.collider;
                //    lastWallIsLeft = contact.normal.x > 0;
                //}
            }
        }
        isWallTouched = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = isWallTouched ? Color.green : Color.red;

        
        Vector2 wallBoxcenter = (Vector2)transform.position
                            + ((keyboardLeft ? Vector2.left : Vector2.right)
                               * (boxCollider.size.x / 2f));
        
        Gizmos.DrawWireCube(wallBoxcenter, wallCheckBoxSize);
    }
#endif
}
