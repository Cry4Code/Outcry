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
    private bool keyboardLeft = false;
    private bool lookLeft = false;
    
    #endregion
    
    #region 점프 관련
    [field : Header("Jump Settings")]
    [field : SerializeField] public float JumpForce { get; set; }
    [field : SerializeField] public float WallJumpForce { get; set; }
    [field : SerializeField] public float GroundThresholdForce { get; set; } // 땅으로 인식하는 법선 벡터 크기 조건
    [field: SerializeField] public float GroundThresholdDistance { get; set; } // 땅으로 인식하는 거리 최소 조건
    public LayerMask groundMask; 
    public bool isGroundJump = false; // 지상에서 첫 점프 했는지
    public bool isDoubleJump = false; // 더블점프 했는지
    private float wallCheckBoxX;
    private Collider2D currentWall;
    public bool isWallJumped = false; // 벽점 했는지
    public bool lastWallIsLeft = false; // 마지막에 부딛힌 벽이 왼쪽에 있는지
    public bool isGrounded = true;

    #endregion

    #region 시야 관련
    [Header("Look Settings")]
    
    
    #endregion
    
    private Camera mainCam;

    private Rigidbody2D rb;
    
    private SpriteRenderer spriteRenderer;
    private bool isDodged = false;
    
    public PlayerController Controller { get; set; }
    


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if(boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();
        Controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        wallCheckBoxX = boxCollider.size.x * 0.1f;    }


    /// <summary>
    /// JumpState 진입 시 한 번 불림.
    /// </summary>
    public void Jump()
    {
        isGrounded = false;
        Debug.Log("Jump!");
        if (!isGroundJump)
        {
            rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            isGroundJump = true;
        }
        
        

        //Debug.Log($"점프 카운트 : {airJumpCount}");

        //if (IsWallTouched(out bool isWallInLeft, out Collider2D wallHit) && !IsGrounded())
        //{
        //    if (currentWall != wallHit)
        //    {
        //        Debug.Log($"벽점!");
        //        rb.velocity = Vector2.zero;
        //        lastWallIsLeft = isWallInLeft;
        //        currentWall = wallHit;
        //        Vector2 jumpPower = ((isWallInLeft ? Vector2.right : Vector2.left) + Vector2.up).normalized *
        //                            WallJumpForce;

        //        rb.AddForce(jumpPower,ForceMode2D.Impulse);
        //        isWallJumped = true;

        //        return;
        //    }
        //}

        //// 2단 이상 점프 방지
        //if (!IsGrounded())
        //{
        //    airJumpCount++;
        //    if (airJumpCount > 1) return;
        //    rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        //}
        //else
        //{
        //    // 바닥에서 점프 안했을 때
        //    if (!groundJump)
        //    {
        //        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        //        groundJump = true;
        //    }
        //}
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

    public void WallJump()
    {
        Vector2 dir = lastWallIsLeft ? Vector2.right : Vector2.left;
        rb.velocity = new Vector2(dir.x * WallJumpForce, JumpForce);
        lastWallIsLeft = !lastWallIsLeft;
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

        //if (isWallJumped)
        //{
        //    if (curMoveInput.x == 0) return;
        //}
        //Vector2 dir = transform.right * curMoveInput.x;
        //dir *= MoveSpeed;


        //// 벽에 붙고, 땅이 아니면서, 벽의 방향과 입력 방향이 같고, 입력이 0이 아닐 때
        //if (IsWallTouched(out var isWallInLeft, out var hit) 
        //    && !IsGrounded() 
        //    && isWallInLeft == keyboardLeft 
        //    && curMoveInput.x != 0)
        //{
        //    rb.gravityScale = 0.5f;
        //}
        //else
        //{
        //    rb.gravityScale = 1f;

        //    dir.y = rb.velocity.y;
        //    rb.velocity = dir;
        //}
    }
   

    /// <summary>
    /// 벽에 닿았는지 체크. 
    /// </summary>
    /// <param name="isWallInLeft">왼쪽 벽이면 True</param>
    /// <param name="wallHit">벽 정보</param>
    /// <returns>벽에 닿았으면 True</returns>
    public bool IsWallTouched(out bool isWallInLeft, out Collider2D wallHit)
    {

        Vector2 boxcenter = (Vector2)transform.position
            + ((keyboardLeft ? Vector2.left : Vector2.right)
            * (boxCollider.size.x / 2f));

        Vector2 boxsize = new Vector2(wallCheckBoxX, boxCollider.size.y);
        
        wallHit = Physics2D.OverlapBox(boxcenter, boxsize, 0.0f, groundMask);

        isWallInLeft = keyboardLeft;
        
        if (wallHit != null)
        {
            /*Debug.Log("벽 터치");*/
            if(wallHit != currentWall)
                isWallJumped = false;
            return true;
        }

        return false;
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
        if (collision.gameObject.layer == groundMask)
            UpdateGrounded(collision);
    }

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    UpdateGrounded(collision);
    //}

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == groundMask)
            isGrounded = false; 
    }

    private void UpdateGrounded(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // 캐릭터가 아래로 향하는 충돌에서만 grounded
            if (contact.normal.y > GroundThresholdForce)    
            {

                isGrounded = true;
                isDoubleJump = false;
                isGroundJump = false;
                isWallJumped = false;
                currentWall = null;
                return;

            }
        }

        // 위쪽으로 부딪히거나 옆으로 부딪히면 grounded 안 됨
        isGrounded = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = IsWallTouched(out bool isWallInLeft, out var hit) ? Color.green : Color.red;

        
        Vector2 wallBoxcenter = (Vector2)transform.position
                            + ((keyboardLeft ? Vector2.left : Vector2.right)
                               * (boxCollider.size.x / 2f));

        Vector2 wallBoxsize = new Vector2(wallCheckBoxX, boxCollider.size.y);
        
        Gizmos.DrawWireCube(wallBoxcenter, wallBoxsize);
    }
#endif
}
