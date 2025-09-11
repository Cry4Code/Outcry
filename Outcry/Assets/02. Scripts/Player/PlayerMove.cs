using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    #region 이동 관련

    [field : Header("Movement Settings")] 
    [field : SerializeField] public float MoveSpeed { get; set; }
    private Vector2 curMoveInput;
    private bool isLeft = false;
    
    #endregion
    
    #region 점프 관련
    [field : Header("Jump Settings")]
    [field : SerializeField] public float JumpForce { get; set; }

    [SerializeField] private float wallCheckRadius; 
    
    
    public LayerMask groundMask; 
    private float colliderHeightHalf;
    private int jumpCount = 0;
    
    #endregion

    #region 시야 관련
    [Header("Look Settings")]
    
    
    #endregion
    
    private Camera mainCam;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    public PlayerInputs Inputs { get; set; }
    private bool isDodged = false;
    private bool isWallJumped = false;


    private void Awake()
    {
        Inputs = new PlayerInputs();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        colliderHeightHalf = boxCollider.size.y / 2f;
    }

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// 플레이어 입력 관련 이벤트 연결
    /// </summary>
    private void OnEnable()
    {
        Inputs.Player.Move.performed += OnMove;
        Inputs.Player.Move.canceled += OnMoveStop;
        Inputs.Player.Jump.started += OnJump;
        
        
        Inputs.Player.Dodge.started += OnDodge;
        Inputs.Player.Pause.started += OnPause;

        
        
        Inputs.Enable();
    }

    
    /// <summary>
    /// 플레이어 입력 관련 이벤트 연결 해제
    /// </summary>
    private void OnDisable()
    {
        Inputs.Player.Move.performed -= OnMove;
        Inputs.Player.Move.canceled -= OnMoveStop;
        Inputs.Player.Jump.started -= OnJump;
        
        Inputs.Player.Dodge.started -= OnDodge;
        Inputs.Player.Pause.started -= OnPause;
        
        Inputs.Disable();
    }

    /// <summary>
    /// 이동, 점프 등 물리 관련 함수 실행
    /// </summary>
    private void FixedUpdate()
    {
        if (isDodged)
        {
            Debug.Log("회피!");
            isDodged = false;
        }
        
        Move();
        if (IsGrounded()) jumpCount = 0;
    }

    /// <summary>
    /// 물리 관련 함수 후 실행 되어야 하는 내용들 (마우스 움직임에 따른 좌우반전 등)
    /// </summary>
    private void LateUpdate()
    {
        
    }

    /// <summary>
    /// 이동 입력 시 자동 실행 됨
    /// </summary>
    /// <param name="context"></param>
    private void OnMove(InputAction.CallbackContext context)
    {
        curMoveInput = context.ReadValue<Vector2>();
    }

    
    /// <summary>
    /// 이동 입력 멈추면 자동 실행 됨
    /// </summary>
    /// <param name="context"></param>
    private void OnMoveStop(InputAction.CallbackContext context)
    {
        curMoveInput = Vector2.zero;
    }
    
    /// <summary>
    /// 점프 키 입력 시 자동 실행 됨
    /// </summary>
    /// <param name="context"></param>
    private void OnJump(InputAction.CallbackContext context)
    {
        // 2단 이상 점프 방지
        if (jumpCount > 2) return;
        rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }
    
    /// <summary>
    /// 회피 키 입력 시 자동 실행 됨
    /// </summary>
    /// <param name="context"></param>
    private void OnDodge(InputAction.CallbackContext context)
    {
        isDodged = true;
    }

    
    /// <summary>
    /// (임시용) 커서 보이게 하기 위함
    /// </summary>
    /// <param name="context"></param>
    private void OnPause(InputAction.CallbackContext context)
    {
        CursorManager.Instance.SetInGame(!CursorManager.Instance.IsInGame);
    }

    /// <summary>
    /// Input에 맞는 움직임
    /// </summary>
    void Move()
    {
        Vector2 dir = transform.right * curMoveInput.x;
        dir *= MoveSpeed;
        dir.y = rb.velocity.y;
        rb.velocity = dir;
    }
    
    
    
    /// <summary>
    /// 땅에 닿았는지 체크
    /// </summary>
    /// <returns>닿았으면 true</returns>
    bool IsGrounded()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, colliderHeightHalf + 0.01f, groundMask))
        {
            return true;
        }
        
        return false;
    }

    
    /// <summary>
    /// 벽에 닿았는지 체크
    /// </summary>
    /// <returns>닿았으면 true</returns>
    bool IsWallTouched()
    {
        return false;
    }


    void Look()
    {
        
        
    }
}
