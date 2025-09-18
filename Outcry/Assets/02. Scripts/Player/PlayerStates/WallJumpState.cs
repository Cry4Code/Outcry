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
    private float wallJumpDistance = 8f;

    private Vector2 wallJumpDirection;
    private Vector2 startPos;
    private Vector2 targetPos;
    private Vector2 newPos;
    private Vector2 curPos;
    
    private float t;

    public override void Enter(PlayerController player)
    {
        base.Enter(player);
        // Debug.Log("벽점!");
        // 벽점할 때에는 벽 반대방향 봐야됨
        player.PlayerMove.ForceLook(!player.PlayerMove.lastWallIsLeft);
        player.isLookLocked = true;
        // 벽점했으니까 강제로 벽 터치 취소
        player.PlayerAnimator.ClearBool(); // WallHold 끄려고
        player.PlayerMove.isWallTouched = false;
        player.PlayerAnimator.SetTriggerAnimation(PlayerAnimID.WallJump);
        
        wallJumpStartTime = Time.time;
        
        animRunningTime = 0f;
        /*player.PlayerMove.WallJump();*/
        wallJumpAnimationLength = 
            player.PlayerAnimator.animator.runtimeAnimatorController
                .animationClips.First(c => c.name == "WallJump").length;
        
        wallJumpDirection = new Vector2((player.PlayerMove.lastWallIsLeft ?  1.5f : -1.5f),1f).normalized;
        startPos = player.transform.position;
        targetPos = startPos + (wallJumpDirection * wallJumpDistance);
    }

    public override void HandleInput(PlayerController player) 
    {
        //if (player.Inputs.Player.Move.ReadValue<Vector2>().x != 0)
        //{
        //    player.PlayerMove.Move();
        //    return;
        //}

        var moveInputs = player.Inputs.Player.Move.ReadValue<Vector2>();

        if (player.Inputs.Player.Jump.triggered && !player.PlayerMove.isDoubleJump)
        {
            player.ChangeState<DoubleJumpState>();
            return;
        }

        if(Time.time - wallJumpStartTime > wallHoldAbleTime && player.PlayerMove.isWallTouched)
        {
            player.ChangeState<WallHoldState>();
            return;
        }
        else
        {
            player.PlayerMove.isWallTouched = false;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && moveInputs.y < 0)
        {
            player.isLookLocked = true;
            player.ChangeState<DownAttackState>();
            return;
        }
        
        if (player.Inputs.Player.NormalAttack.triggered && !player.PlayerAttack.HasJumpAttack)
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
            Physics2D.Raycast(player.transform.position, direction, distance, player.PlayerMove.groundMask);
        
        if (hit.collider != null)
        {
            player.PlayerMove.rb.MovePosition(hit.point - direction * 0.01f);
            if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
            else player.ChangeState<FallState>();
            return;
        }
        
        
        player.PlayerMove.rb.MovePosition(newPos);
        
        if (Vector2.Distance(newPos, targetPos) < 0.01f)
        {
            player.PlayerMove.rb.velocity = Vector2.zero;
            if (player.PlayerMove.isGrounded) player.ChangeState<IdleState>();
            else player.ChangeState<FallState>();
            return;
        }


        /*
        if (animRunningTime >= startFallTime)
        {
            if (player.PlayerMove.rb.velocity.y < 0)
            {
                player.ChangeState<FallState>();
                return;
            }
        }*/
        
        
        if (player.PlayerMove.isGrounded)
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
