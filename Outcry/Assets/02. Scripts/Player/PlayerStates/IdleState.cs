using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        // 애니메이션 설정 
        // player.SetAnimation("Idle");
    }

    public void HandleInput(PlayerController player)
    {
        var input = player.Inputs.Player.Move.ReadValue<Vector2>();        
        if (input.x != 0) player.ChangeState(new MoveState()); // 이동이 있으면 MoveState로 이동
        else if (player.Inputs.Player.Jump.triggered) player.ChangeState(new JumpState());
        // else if (player.Inputs.Player.Dodge.triggered) player.ChangeState(new DodgeState());
    }

    public void LogicUpdate(PlayerController player) { }
    public void Exit(PlayerController player) { }
}