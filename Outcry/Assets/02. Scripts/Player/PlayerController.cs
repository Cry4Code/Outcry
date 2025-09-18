using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Dictionary<System.Type, IPlayerState> states; // 상태 저장용
    public PlayerInputs Inputs { get; private set; }
    public PlayerMove PlayerMove { get; private set; }
    public PlayerAttack PlayerAttack { get; private set; }
    
    public PlayerCondition Condition { get; private set; }
    
    private IPlayerState currentState;
    [HideInInspector] public bool isLookLocked = false;
    
    public PlayerAnimator PlayerAnimator { get; private set; }

    private void Awake()
    {
        Inputs = new PlayerInputs();
        Inputs.Enable();
        PlayerMove = GetComponent<PlayerMove>();
        PlayerAttack = GetComponent<PlayerAttack>();
        Condition = GetComponent<PlayerCondition>();
        
        states = new Dictionary<System.Type, IPlayerState>
        {
            { typeof(IdleState), new IdleState() },
            { typeof(MoveState), new MoveState() },
            { typeof(JumpState), new JumpState() },
            { typeof(DoubleJumpState), new DoubleJumpState() },
            { typeof(WallJumpState), new WallJumpState() },
            { typeof(WallHoldState), new WallHoldState() },
            { typeof(FallState), new FallState() },
            { typeof(NormalAttackState), new NormalAttackState() },
            { typeof(NormalJumpAttackState), new NormalJumpAttackState()},
            { typeof(DownAttackState), new DownAttackState()},
            { typeof(SpecialAttackState), new SpecialAttackState()},
            { typeof(DodgeState), new DodgeState()},
            { typeof(StartParryState), new StartParryState()},
            { typeof(SuccessParryState), new SuccessParryState()},
            { typeof(DamagedState), new DamagedState()},
        };
    }

    private void Start()
    {
        PlayerAnimator = GetComponentInChildren<PlayerAnimator>();
        ChangeState<IdleState>();
    }

    private void OnEnable()
    {
        Inputs.Player.Look.performed += CursorManager.Instance.OnLook;
        Inputs.Player.Pause.started += PlayerMove.OnPause;
    }

    private void OnDisable()
    {
        Inputs.Player.Look.performed -= CursorManager.Instance.OnLook;
        Inputs.Player.Pause.started -= PlayerMove.OnPause;
    }

    private void Update()
    {
        Debug.Log($"상태 : {currentState.GetType().Name}");
        Debug.Log($"벽 터치 : {PlayerMove.isWallTouched}");
        // Debug.Log($"땅 : {PlayerMove.isGrounded} || 일반 점프 : {PlayerMove.isGroundJump} || 이단 점프 : {PlayerMove.isDoubleJump}");
        currentState.HandleInput(this);
        currentState.LogicUpdate(this);
    }

    private void LateUpdate()
    {
        if(!isLookLocked) PlayerMove.Look();
    }

    public void ChangeState<T>() where T : IPlayerState
    {
        currentState?.Exit(this);

        currentState = states[typeof(T)];
        currentState.Enter(this);
    }

    public bool IsCurrentState<T>() where T : IPlayerState
    {
        return currentState is T;
    }

    public void SetAnimation(int animHash, bool isTrigger = false)
    {
        if (isTrigger) PlayerAnimator.SetTriggerAnimation(animHash);
        else  PlayerAnimator.SetBoolAnimation(animHash);
    }
}