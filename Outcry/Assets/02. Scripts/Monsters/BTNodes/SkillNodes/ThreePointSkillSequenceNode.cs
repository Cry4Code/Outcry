
using Unity.VisualScripting;
using UnityEngine;

public class ThreePointSkillSequenceNode : SkillSequenceNode
{
    private Animator animator;

    private const float ANIMATION_FRAME_RATE = 20f; // 이 애니메이션 클립의 초당 프레임 수

    // 전체 애니메이션 길이 (38개 스프라이트 = 0~37번 인덱스)
    private const float ANIMATION_TOTAL_DURATION = (1.0f / ANIMATION_FRAME_RATE) * 38;

    public ThreePointSkillSequenceNode(int skillId) : base(skillId)
    {
        this.nodeName = "ThreePointSkillSequenceNode";
    }

    public override void InitializeSkillSequenceNode(MonsterBase monster, Player target)
    {
        base.InitializeSkillSequenceNode(monster, target);

        this.nodeName = "ThreePointSkillSequenceNode";
        animator =  monster.Animator;
    }
    
    protected override bool CanPerform()
    {
        bool result;
        bool isInRange;
        bool isCooldownComplete;
        
        // 쿨다운이 다 차지 않았을 때만 시간 더함
        if(Time.time - lastUsedTime >= skillData.cooldown)
        {
            isCooldownComplete = true;
        }
        else
        {
            isCooldownComplete = false;
        }
        
        // 플레이어와의 거리 확인
        if (Vector2.Distance(monster.transform.position, target.transform.position) <= skillData.range)
        {
            isInRange = true;
        }
        else
        {
            isInRange = false;
        }

        // 두 조건이 모두 만족해야 스킬 사용 가능
        result = isInRange && isCooldownComplete;
        Debug.Log($"Skill {skillData.skillName} used? {result} : {Time.time - lastUsedTime} / {skillData.cooldown}");
        return result;
    }

    protected override NodeState SkillAction()
    {
        // 스킬이 아직 발동되지 않았다면 트리거 켜기
        if (!skillTriggered)
        {
            Debug.Log("Setting Parameter: ThreePoint");
            animator.SetTrigger(AnimatorStrings.MonsterParameter.ThreePoint);

            // 상태 초기화 및 애니메이션 시작 시간 기록
            skillTriggered = true;
            FlipCharacter();
            monster.AttackController.SetDamages(skillData.damage1);
            lastUsedTime = Time.time;
        }

        // 애니메이션 경과 시간 계산
        float elapsedTime = Time.time - lastUsedTime;

        // 스킬 종료 처리
        // 총 애니메이션 길이만큼 시간이 지났다면 스킬을 종료
        if (elapsedTime >= ANIMATION_TOTAL_DURATION)
        {
            skillTriggered = false; // 다음 스킬 사용을 위해 플래그 리셋
            monster.AttackController.ResetDamages(); //데미지 초기화
            return NodeState.Success;
        }

        // 위의 종료 조건에 해당하지 않으면 스킬이 아직 진행 중인 것
        return NodeState.Running;
    }


}
