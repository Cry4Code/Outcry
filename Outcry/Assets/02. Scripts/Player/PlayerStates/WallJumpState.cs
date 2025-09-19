using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WallJumpState : AirSubState
{
    private float wallJumpStartTime;
    private float wallHoldAbleTime = 0.5f;
    private float startFallTime = 0.2f;
    
    private float animRunningTime = 0f;
    private float wallJumpAnimationLength;
    private float wallJumpSpeed = 5f;

    private Vector2 wallJumpDirection;
    private Vector2 startPos;
    private Vector2 targetPos;
    private Vector2 newPos;
    private Vector2 curPos;
    
    private float t;

    public override void Enter(PlayerController controller)
    {
        if (!controller.Condition.TryUseStamina(controller.Data.wallJumpStamina))
        {
            if (controller.Move.isGrounded)
            {
                controller.ChangeState<IdleState>();
                return;
            }
            else
            {
                controller.ChangeState<FallState>();
                return;
            }
        }
        base.Enter(controller);
        // 벽점할 때에는 벽 반대방향 봐야됨
        controller.Move.ForceLook(!controller.Move.lastWallIsLeft);
        controller.isLookLocked = true;
        // 벽점했으니까 강제로 벽 터치 취소
        controller.Animator.ClearBool(); // WallHold 끄려고
        controller.Move.isWallTouched = false;
        controller.Animator.SetTriggerAnimation(PlayerAnimID.WallJump);
        
        wallJumpStartTime = Time.time;
        
        animRunningTime = 0f;
        wallJumpAnimationLength = 
            controller.Animator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == "WallJump").length;
        
        wallJumpDirection = new Vector2((controller.Move.lastWallIsLeft ?  1.5f : -1.5f),1f).normalized;
        startPos = controller.transform.position;
        targetPos = startPos + (wallJumpDirection * controller.Data.wallJumpDistance);
    }

    public override void HandleInput(PlayerController player) 
    {

        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();

        if (player.Inputs.Player.Jump.triggered && !player.Move.isDoubleJump)
        {
            player.ChangeState<DoubleJumpState>();
            return;
        }

        if(Time.time - wallJumpStartTime > wallHoldAbleTime && player.Move.isWallTouched)
        {
            player.ChangeState<WallHoldState>();
            return;
        }
        else
        {
            player.Move.isWallTouched = false;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && moveInputs.y < 0)
        {
            player.isLookLocked = true;
            player.ChangeState<DownAttackState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.Attack.HasJumpAttack)
        {
            player.isLookLocked = true;
            player.ChangeState<NormalJumpAttackState>();
            return;
        }
        
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
        animRunningTime += Time.deltaTime;
        t = animRunningTime / wallJumpAnimationLength;

        newPos = Vector2.Lerp(startPos, targetPos, t);

        curPos = player.transform.position;
        
        
        // 현재 위치에서 이동할 위치만큼 선 하나 그어서, 그게 벽에 닿으면 벽 끝에까지만 가고 상태 바뀌게함
        Vector2 direction = (newPos - curPos).normalized;
        float distance = Vector2.Distance(curPos, newPos);
        
        RaycastHit2D hit =
            Physics2D.Raycast(player.transform.position, direction, distance, player.Move.groundMask);
        
        if (hit.collider != null)
        {
            player.Move.rb.MovePosition(hit.point - direction * 0.01f);
            if (player.Move.isGrounded) player.ChangeState<IdleState>();
            else player.ChangeState<FallState>();
            return;
        }
        
        
        player.Move.rb.MovePosition(newPos);
        
        if (Vector2.Distance(newPos, targetPos) < 0.01f)
        {
            player.Move.rb.velocity = Vector2.zero;
            if (player.Move.isGrounded) player.ChangeState<IdleState>();
            else player.ChangeState<FallState>();
            return;
        }

        
        if (player.Move.isGrounded)
        {
            player.ChangeState<IdleState>();
            return;
        }
    }

    public override void Exit(PlayerController player) 
    {
        base.Exit(player);
        player.isLookLocked = false;
    }
}
