using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirSubState : IPlayerState
{
    public virtual void Enter(PlayerController player)
    {
        player.PlayerAnimator.OnBoolParam(PlayerAnimID.SubAir);
    }

    public virtual void HandleInput(PlayerController player)
    {
        
    }

    public virtual void LogicUpdate(PlayerController player)
    {
        
    }

    public virtual void Exit(PlayerController player)
    {
        player.PlayerAnimator.OffBoolParam(PlayerAnimID.SubAir);
    }
}
