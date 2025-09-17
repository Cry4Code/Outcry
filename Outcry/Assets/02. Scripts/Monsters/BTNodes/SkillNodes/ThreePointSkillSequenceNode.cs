
using UnityEngine;

public class ThreePointSkillSequenceNode : SkillSequenceNode
{
    private float skillStartTime;
    private float coolTime;
    private bool skillTriggered = false;   
    private Animator animator;

    private const float ATTACK_RANGE = 2f; // 공격 범위 (플레이어 감지)
    private const float ANIMATION_FRAME_RATE = 20f; // 이 애니메이션 클립의 초당 프레임 수

    // 전체 애니메이션 길이 (38개 스프라이트 = 0~37번 인덱스)
    private const float ANIMATION_TOTAL_DURATION = (1.0f / ANIMATION_FRAME_RATE) * 38;

    public override void InitializeSkillSequenceNode(MonsterBase monster, Player target, MonsterSkillModel skillData)
    {
        base.InitializeSkillSequenceNode(monster, target, skillData);

        this.nodeName = "ThreePointSkillSequenceNode";
        animator =  monster.Animator;

        if (skillData != null)
        {
            coolTime = skillData.cooldown;
        }

    }



    protected override bool CanPerform()
    {
        // 쿨다운이 다 차지 않았을 때만 시간 더함
        if (coolTime < skillData.cooldown)
        {
            coolTime += Time.deltaTime;
        }

        // 플레이어와의 거리 확인
        float distanceToTarget = Vector3.Distance(monster.transform.position, target.transform.position);
        bool isInRange = (distanceToTarget <= ATTACK_RANGE);

        // 쿨다운 확인
        bool isCooldownComplete = (coolTime >= skillData.cooldown);

        // 두 조건이 모두 만족해야 스킬 사용 가능
        return isInRange && isCooldownComplete;
    }

    protected override NodeState SkillAction()
    {
        // 스킬이 아직 발동되지 않았다면 트리거 켜기
        if (!skillTriggered)
        {
            // 몬스터를 기준으로 플레이어가 어느 방향에 있는지 계산
            float directionToTarget = Mathf.Sign(target.transform.position.x - monster.transform.position.x);

            // 스킬 시작할 때 플레이어를 바라보게 만듦
            // Mathf.Abs를 사용하여 기존 스케일의 크기 유지
            monster.transform.localScale = new Vector3(
                Mathf.Abs(monster.transform.localScale.x) * directionToTarget,
                monster.transform.localScale.y,
                monster.transform.localScale.z
            );

            animator.SetTrigger(AnimatorStrings.MonsterParameter.ThreePoint);

            // 상태 초기화 및 애니메이션 시작 시간 기록
            skillTriggered = true;
            skillStartTime = Time.time;
            coolTime = 0f; // 스킬을 사용했으므로 쿨다운 타이머 리셋
        }

        // 애니메이션 경과 시간 계산
        float elapsedTime = Time.time - skillStartTime;

        // 스킬 종료 처리
        // 총 애니메이션 길이만큼 시간이 지났다면 스킬을 종료
        if (elapsedTime >= ANIMATION_TOTAL_DURATION)
        {
            skillTriggered = false; // 다음 스킬 사용을 위해 플래그 리셋
            monster.AttackController.SetDamage(0); //데미지 초기화
            return NodeState.Success;
        }

        // 위의 종료 조건에 해당하지 않으면 스킬이 아직 진행 중인 것
        return NodeState.Running;
    }


}
