using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Dictionary<System.Type, IPlayerState> states; // 상태 저장용
    public PlayerInputs Inputs { get; private set; }
    public PlayerMove PlayerMove { get; private set; }
    private IPlayerState currentState;

    private void Awake()
    {
        Inputs = new PlayerInputs();
        Inputs.Enable();
        PlayerMove = GetComponent<PlayerMove>();

        states = new Dictionary<System.Type, IPlayerState>
        {
            { typeof(IdleState), new IdleState() },
            { typeof(MoveState), new MoveState() },
            { typeof(JumpState), new JumpState() },
            { typeof(DoubleJumpState), new DoubleJumpState() },
            { typeof(WallJumpState), new WallJumpState() },
            { typeof(WallHoldState), new WallHoldState() },
            { typeof(FallState), new FallState() },
        };
    }

    private void Start()
    {
        currentState = new IdleState();
        currentState.Enter(this);
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
        Debug.Log($"땅 : {PlayerMove.isGrounded} || 일반 점프 : {PlayerMove.isGroundJump} || 이단 점프 : {PlayerMove.isDoubleJump}");
        currentState.HandleInput(this);
        currentState.LogicUpdate(this);
    }

    private void LateUpdate()
    {
        PlayerMove.Look();
    }

    public void ChangeState<T>() where T : IPlayerState
    {
        currentState?.Exit(this);

        currentState = states[typeof(T)];
        currentState.Enter(this);
    }

    public void SetAnimation(string animName)
    {
        PlayerMove.SetAnimation(animName);
    }
}