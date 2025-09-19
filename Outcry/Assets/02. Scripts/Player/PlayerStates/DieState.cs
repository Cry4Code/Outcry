using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IPlayerState
{
    public void Enter(PlayerController controller)
    {
        controller.SetAnimation(PlayerAnimID.Die, true);
        controller.Inputs.Player.Dodge.Disable();
        controller.Inputs.Player.Move.Disable();
        controller.Inputs.Player.SpecialAttack.Disable();
        controller.Inputs.Player.NormalAttack.Disable();
        controller.Inputs.Player.Jump.Disable();
        controller.Inputs.Player.Parry.Disable();
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        
    }

    public void Exit(PlayerController player)
    {
        
    }
}
