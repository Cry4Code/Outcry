using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalJumpAttackState : NormalJumpAttackSubState
{
    private float startStateTime;
    private float startAttackTime = 0.01f;
    private float jumpAnimationLength;

    private float animRunningTime = 0f;
    /*private float inAirTime = 0.1f;*/
    
    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        startStateTime = Time.time;
        player.PlayerAnimator.ClearBool();
        player.PlayerAttack.HasJumpAttack = true;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.NormalAttack);
        player.Inputs.Player.Move.Disable();
        player.PlayerMove.rb.gravityScale = 0;
        animRunningTime = 0f;
        jumpAnimationLength = 
            player.PlayerAnimator.animator.runtimeAnimatorController
            .animationClips.First(c => c.name == "NormalJumpAttack").length;
    }

    public override void HandleInput(PlayerController player)
    {
        if (player.Inputs.Player.SpecialAttack.triggered)
        {
            player.isLookLocked = false;
            player.ChangeState<SpecialAttackState>();
            return;
        }
        if (player.Inputs.Player.Dodge.triggered)
        {
            player.ChangeState<DodgeState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        /*player.PlayerMove.rb.velocity = new Vector2(player.PlayerMove.rb.velocity.x, 0);*/
        
        player.PlayerMove.rb.velocity = Vector2.zero;
        animRunningTime += Time.deltaTime;
        
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

            if (animRunningTime >= jumpAnimationLength)
            {
                player.ChangeState<IdleState>();
                return;
            }
                
        }
    }

    public override void Exit(PlayerController player)
    {
        base.Exit(player);
        player.PlayerMove.rb.gravityScale = 1;
    }
}
