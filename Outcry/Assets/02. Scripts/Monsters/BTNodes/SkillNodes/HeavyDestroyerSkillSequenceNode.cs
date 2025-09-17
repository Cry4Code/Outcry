using UnityEngine;

public class HeavyDestroyerSkillSequenceNode : SkillSequenceNode
{
    private float stateEnterTime; // 스킬(상태)에 진입한 시간
    [SerializeField] private float cooldownTimer = 0f; // 쿨다운 계산을 위한 타이머
    private bool skillTriggered = false;

    // 컴포넌트 참조
    private Animator animator;

    // 상수
    private const float MOVE_SPEED = 50f;   // 이동 속도
    private const float ATTACK_RANGE = 10f;  // 공격 범위(플레이어 감지)

    // 애니메이션 설정값
    private const float ANIMATION_FRAME_RATE = 20f; // 이 애니메이션 클립의 초당 프레임 수

    // 이동 시작/종료 시간 계산 (스프라이트 기준)
    // 10번째 스프라이트 = 9번 인덱스. 0프레임부터 시작하므로 인덱스 9는 9프레임이 지난 시점
    private const float MOVE_START_TIME = (1.0f / ANIMATION_FRAME_RATE) * 5;
    // 25번째 스프라이트 = 24번 인덱스
    private const float MOVE_END_TIME = (1.0f / ANIMATION_FRAME_RATE) * 9;
    // 전체 애니메이션 길이 (33개 스프라이트 = 0~32번 인덱스)
    private const float ANIMATION_TOTAL_DURATION = (1.0f / ANIMATION_FRAME_RATE) * 28;

    public override void InitializeSkillSequenceNode(MonsterBase monster, Player target, MonsterSkillModel skillData)
    {
        base.InitializeSkillSequenceNode(monster, target, skillData);
        this.nodeName = "HeavyDestroyerSkillSequenceNode";
        animator = monster.Animator;

        // 게임 시작 시 바로 스킬을 사용할 수 있도록 쿨다운을 초기화
        if (skillData != null)
        {
            cooldownTimer = skillData.cooldown;
        }
    }

    protected override bool CanPerform()
    {
        // 쿨다운이 다 차지 않았을 때만 시간 더함
        if (cooldownTimer < skillData.cooldown)
        {
            cooldownTimer += Time.deltaTime;
        }

        // 플레이어와의 거리 확인 (거리가 10 이상일때)
        float distanceToTarget = Vector3.Distance(monster.transform.position, target.transform.position);
        bool isInRange = (distanceToTarget >= ATTACK_RANGE);

        // 쿨다운 확인
        bool isCooldownComplete = (cooldownTimer >= skillData.cooldown);

        // bool isPlayerHeal 플레이어가 힐을 할 때

        // 세 조건이 모두 만족해야 스킬 사용 가능
        return isInRange && isCooldownComplete/*isPlayerHeal*/;
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

            animator.SetTrigger(AnimatorStrings.MonsterParameter.HeavyDestroyerHash);
            monster.AttackController.SetDamage(skillData.damage1);

            // 상태 초기화 및 애니메이션 시작 시간 기록
            skillTriggered = true;
            stateEnterTime = Time.time;
            cooldownTimer = 0f; // 스킬을 사용했으므로 쿨다운 타이머 리셋
        }

        // 애니메이션 경과 시간 계산
        float elapsedTime = Time.time - stateEnterTime;

        // 애니메이션 경과 시간에 따른 이동 처리
        // 이동 시작 시간과 종료 시간 사이에만 이동 로직을 실행
        if (elapsedTime >= MOVE_START_TIME && elapsedTime < MOVE_END_TIME)
        {
            float direction = Mathf.Sign(monster.transform.localScale.x);
            // Vector3.right를 사용하여 월드 좌표계의 오른쪽 방향을 기준으로 이동
            // direction 값에 따라 왼쪽 또는 오른쪽으로 움직임
            monster.transform.Translate(Vector3.right * direction * MOVE_SPEED * Time.deltaTime);
        }

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
