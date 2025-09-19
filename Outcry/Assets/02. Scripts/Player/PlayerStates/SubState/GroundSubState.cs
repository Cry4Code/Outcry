using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSubState : IPlayerState
{
    public virtual void Enter(PlayerController controller)
    {
        controller.Animator.OnBoolParam(PlayerAnimID.SubGround);
    }

    public virtual void HandleInput(PlayerController player)
    {
        
    }

    public virtual void LogicUpdate(PlayerController player)
    {
        
    }

    public virtual void Exit(PlayerController player)
    {
        player.Animator.OffBoolParam(PlayerAnimID.SubGround);
    }
}
