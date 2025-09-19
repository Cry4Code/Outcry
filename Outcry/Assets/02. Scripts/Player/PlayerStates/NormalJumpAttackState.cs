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
    
    public override void Enter(PlayerController controller)
    {
        base.Enter(controller);
        startStateTime = Time.time;
        controller.Animator.ClearBool();
        controller.Attack.HasJumpAttack = true;
        controller.Hitbox.Damage = controller.Data.jumpAttackDamage;
        controller.Animator.SetTriggerAnimation(PlayerAnimID.NormalAttack);
        controller.Inputs.Player.Move.Disable();
        controller.Move.rb.gravityScale = 0;
        animRunningTime = 0f;
        jumpAnimationLength = 
            controller.Animator.animator.runtimeAnimatorController
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
        if (player.Inputs.Player.Parry.triggered)
        {
            player.ChangeState<StartParryState>();
            return;
        }
    }

    public override void LogicUpdate(PlayerController player)
    {
        /*player.PlayerMove.rb.velocity = new Vector2(player.PlayerMove.rb.velocity.x, 0);*/
        
        player.Move.rb.velocity = Vector2.zero;
        animRunningTime += Time.deltaTime;
        
        if (Time.time - startStateTime > startAttackTime)
        {
            AnimatorStateInfo curAnimInfo = player.Animator.animator.GetCurrentAnimatorStateInfo(0);

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
        player.Move.rb.gravityScale = 1;
    }
}
