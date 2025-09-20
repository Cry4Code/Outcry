using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleOfRuinSkillSequenceNode : SkillSequenceNode
{
    private Vector2 targetPos;
    private bool isAscending = false;
    
    private float ascendSpeed = 50f;
    public RumbleOfRuinSkillSequenceNode(int skillId) : base(skillId)
    {
        this.nodeName = "RumbleOfRuinSkillSequenceNode";
    }

    public override void InitializeSkillSequenceNode(MonsterBase monster, PlayerController target)
    {        
        this.monster = monster;
        this.target = target;
        
        ConditionNode canPerform = new ConditionNode(CanPerform);
        ActionNode jumpAction = new ActionNode(JumpAction);
        ActionNode skillAction = new ActionNode(SkillAction);
        
        
        //노드 이름 설정 (디버깅용)
        canPerform.nodeName = "CanPerform";
        jumpAction.nodeName = "JumpAction";
        skillAction.nodeName = "SkillAction";
        
        children.Clear();
        AddChild(canPerform);
        AddChild(jumpAction);
        AddChild(skillAction);

        nodeName = skillData.skillName + skillData.skillId;
        lastUsedTime = Time.time;
    }

    protected override bool CanPerform()
    {
        bool result;
        bool isCooldownComplete;
        bool isLowHealth;
        if (skillTriggered) //한번 실행되면 다시 실행될 일 없음. 실행 끝난 이후 리셋 X
        {
            return false;
        }
        
        //체력이 일정 이하일때
        isLowHealth = monster.Condition.CurrentHealth < skillData.triggerHealth * monster.Condition.MaxHealth;

        skillData.cooldown = 2f;    //debug용으로 쿨다운 2초로 설정.
        //쿨다운 확인
        isCooldownComplete = Time.time - lastUsedTime >= skillData.cooldown;
        

        result = (isLowHealth || isCooldownComplete) && !skillTriggered;
        Debug.Log($"Skill {skillData.skillName} used? {result} : {Time.time - lastUsedTime} / {skillData.cooldown} || {monster.Condition.CurrentHealth} / {monster.Condition.MaxHealth}");
        return result;
    }
    private NodeState JumpAction()
    {
        if (!isAscending)
        {
            isAscending = true;
            // float direction = monster.Rb2D.transform.localScale.x > 0 ? 1f : -1f;
            // float forwardDistance = 5f;
            float ascendHeight = 10f;
            targetPos = new Vector2(
                monster.Rb2D.position.x, //+ forwardDistance * direction,
                monster.Rb2D.position.y + ascendHeight
            );
            skillTriggered = true;
            lastUsedTime = Time.time;
            monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.RumbleOfRuin);

            monster.Rb2D.velocity = Vector2.zero;
            monster.Rb2D.angularVelocity = 0f;
            monster.Rb2D.MovePosition(monster.Rb2D.position);
            monster.Rb2D.bodyType = RigidbodyType2D.Kinematic;

            return NodeState.Running;
        }

        if (Time.time - lastUsedTime < 0.1f)
            return NodeState.Running;

        AnimatorStateInfo stateInfo = monster.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("RumbleOfRuinStart") && stateInfo.normalizedTime < 0.95f)
        {
            // Vector2 nextPos = Vector2.MoveTowards(monster.Rb2D.position, new Vector2(targetPos.x, monster.Rb2D.position.y), 1f * Time.fixedDeltaTime);
            // monster.Rb2D.MovePosition(nextPos);
            return NodeState.Running;
        }

        if (monster.Rb2D.position.y < targetPos.y)
        {
            Vector2 nextPos = Vector2.MoveTowards(monster.Rb2D.position, targetPos, ascendSpeed * Time.fixedDeltaTime);
            monster.Rb2D.MovePosition(nextPos);
            return NodeState.Running;
        }

        var sr = monster.SpriteRenderer;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0);
        monster.Rb2D.bodyType = RigidbodyType2D.Dynamic;
        return NodeState.Success;
    }
    
    protected override NodeState SkillAction()
    {
        NodeState state;

        
        monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.IsReady);
        //todo.
        // 1. 현재 그라운드에서 점프해서 사라짐

        return NodeState.Failure;


        // monster.transform.Translate(Vector3.up * Time.deltaTime); 
        //    > 위치를 하늘에 고정시키면 되려나?
        //todo.
        // 2. warning 컷씬 출력 
        //todo.
        // 3. 땅 위로 떨어짐.
        //    > orderinLayer를 조정하기.
        //     SetTrigger(AnimatorStrings.MonsterParameter.IsReady);
        //todo.
        // 4. 돌 덩어리를 야구 볼 배트로 치는 듯한 애니메이션. 3번이 끝나고 난 뒤.
        //    > 돌 덩어리를 생성해서 카메라로 돌진.
        //    > 애니메이션 끝나면 잠깐 안보이게
        //todo.
        // 5. 돌 덩어리 이펙트가 다 끝나고 나서 다시 그라운드로 착지.
        //    > orderinLayer 원상복구.
        // monster.Animator.SetTrigger(AnimatorStrings.MonsterParameter.RumbleOfRuin);
        // skillTriggered = true;


        //
        // if (Time.time - lastUsedTime < 0.1f) //시작 직후는 무조건 Running
        // {
        //     return NodeState.Running;
        // }
        //
        // //스킬 애니메이션 적절할때 데미지 주기
        //
        // bool isSkillAnimationPlaying = Random.Range(0, 2) == 0; //= IsSkillAnimationPlaying()
        // if (isSkillAnimationPlaying)
        // {
        //     Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
        //     state = NodeState.Running;
        // }
        // else
        // {
        //     Debug.Log($"Skill End: {skillData.skillName} (ID: {skillData.skillId})");
        //     state = NodeState.Success;
        // }
        //
        // return state;
    }
}
