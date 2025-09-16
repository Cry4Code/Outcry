using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalJumpAttackState : IPlayerState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    /*private float inAirTime = 0.1f;*/
    
    public void Enter(PlayerController player)
    {
        startStateTime = Time.time;
        player.PlayerAnimator.ClearBool();
        player.PlayerAttack.HasJumpAttack = true;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.NormalAttack);
        player.Inputs.Player.Move.Disable();
        player.PlayerMove.rb.gravityScale = 0;
    }

    public void HandleInput(PlayerController player)
    {
        
    }

    public void LogicUpdate(PlayerController player)
    {
        /*player.PlayerMove.rb.velocity = new Vector2(player.PlayerMove.rb.velocity.x, 0);*/
        
        player.PlayerMove.rb.velocity = Vector2.zero;
        
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.PlayerAnimator.animator.GetCurrentAnimatorStateInfo(0);

            if (curAnimInfo.IsName("NormalJumpAttack"))
            { 
                float animTime = curAnimInfo.normalizedTime;

                if (animTime >= 1.0f)
                {
                    player.ChangeState<IdleState>();
                    return;
                }
            }
        }
    }

    public void Exit(PlayerController player)
    {
        player.PlayerMove.rb.gravityScale = 1;
    }
}
