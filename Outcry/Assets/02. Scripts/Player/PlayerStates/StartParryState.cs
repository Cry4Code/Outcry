using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StartParryState : IPlayerState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    public void Enter(PlayerController player)
    {
        player.isLookLocked = false;
        player.PlayerMove.ForceLook(CursorManager.Instance.mousePosition.x - player.transform.position.x < 0);
        player.PlayerMove.rb.velocity = Vector2.zero;
        player.PlayerAnimator.ClearTrigger();
        player.PlayerAnimator.ClearInt();
        player.PlayerAnimator.ClearBool();
        player.Inputs.Player.Move.Disable();
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.StartParry);
        
        player.isLookLocked = true;
        player.PlayerAttack.isStartParry = true;
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        if (player.PlayerAttack.successParry)
        {
            player.ChangeState<SuccessParryState>();
            return;
        }
        
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.PlayerAnimator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("StartParry"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
                    else player.ChangeState<FallState>();
                    return;
                }
            }
        }
    }

    public void Exit(PlayerController player)
    {
        player.PlayerAttack.isStartParry = false;
    }
}
