using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        player.SetAnimation(PlayerAnimID.Die, true);
        player.Inputs.Disable();
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
