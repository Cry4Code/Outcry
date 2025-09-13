using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerInputs Inputs { get; private set; }
    public PlayerMove PlayerMove { get; private set; }
    private IPlayerState currentState;

    private void Awake()
    {
        Inputs = new PlayerInputs();
        Inputs.Enable();
        PlayerMove = GetComponent<PlayerMove>();
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
        currentState.HandleInput(this);
        currentState.LogicUpdate(this);
    }

    private void LateUpdate()
    {
        PlayerMove.Look();
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    public void SetAnimation(string animName)
    {
        PlayerMove.SetAnimation(animName);
    }
}