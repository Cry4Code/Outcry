using UnityEngine;

public class HeavyDestroyerSkillSequenceNode : SkillSequenceNode
{
    private float stateEnterTime; // 스킬(상태)에 진입한 시간
    [SerializeField] 
    private float cooldownTimer = 0f; // 쿨다운 계산을 위한 타이머
    private bool skillTriggered = false;
    //애니메이터 추가
    private Animator animator;
    // 상수
    private const float MOVE_SPEED = 50f;   // 이동 속도



    public HeavyDestroyerSkillSequenceNode(int skillId) : base(skillId) // 뭔지 모르겠음
    {
        this.nodeName = "HeavyDestroyerSkillSequenceNode";
    }

    public override void InitializeSkillSequenceNode(MonsterBase monster, Player target)
    {
        base.InitializeSkillSequenceNode(monster, target);
        this.nodeName = "HeavyDestroyerSkillSequenceNode";
        animator = monster.Animator;

        // 게임 시작 시 바로 스킬을 사용할 수 있도록 쿨다운을 초기화
        if (skillData != null)
        {
            cooldownTimer = skillData.cooldown;
        }
    }

    protected override bool CanPerform() // 이해 완료, 트리거 되는거 까지 완료
    {
        // 쿨다운이 다 차지 않았을 때만 시간 더함
        if (cooldownTimer < skillData.cooldown)
        {
            cooldownTimer += Time.deltaTime;
        }

        // 플레이어와의 거리 확인 (거리가  이상일때만 사용) 황상욱
        bool isInRange = Vector2.Distance(monster.transform.position, target.transform.position) > skillData.range;

        // 쿨다운 확인
        bool isCooldownComplete = (cooldownTimer >= skillData.cooldown);

        // bool isPlayerHeal 플레이어가 힐을 할 때

        // 세 조건이 모두 만족해야 스킬 사용 가능
        return isInRange && isCooldownComplete/*isPlayerHeal*/;
    }

    protected override NodeState SkillAction()
    {
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

            monster.Animator.SetTrigger("HeavyDestroyerStart"); // 돌진 시작 애니메이션 트리거 on
            monster.AttackController.SetDamages(skillData.damage1);

            // 상태 초기화 및 애니메이션 시작 시간 기록
            skillTriggered = true;
            stateEnterTime = Time.time;
            cooldownTimer = 0f; // 스킬을 사용했으므로 쿨다운 타이머 리셋
        }

        //시작 애니메이션이 끝났으면 바로 다음 돌진 애니메이션 켜기
        if (IsSkillAnimationEnd("HeavyDestroyerStart"))
        {

            Debug.Log("돌진 시작 끝");
            animator.SetTrigger("HeavyDestroyerMove");
        }


        if (target.transform.position.x != monster.transform.position.x && !IsSkillAnimationPlaying("HeavyDestroyerStart"))/* 스킬 시작 시x 좌표가 타겟x 좌표와 같아질 때 까지 루프 애니메이션 호출하면서 이동*/
        {
            animator.SetTrigger(AnimatorStrings.MonsterParameter.HeavyDestroyerMove);
            float direction = Mathf.Sign(monster.transform.localScale.x);
            // Vector3.right를 사용하여 월드 좌표계의 오른쪽 방향을 기준으로 이동
            // direction 값에 따라 왼쪽 또는 오른쪽으로 움직임
            monster.transform.Translate(Vector3.right * direction * MOVE_SPEED * Time.deltaTime);
        }
        else
        { 
            animator.SetTrigger("HeavyDestroyerIsArrived");
        }






        if (IsSkillAnimationPlaying("HeavyDestroyerEnd"))
        {
            monster.AttackController.SetDamages((int)skillData.damage1);
        }

        // 스킬 종료 처리
        // 엔드 애니메이션 실행이 끝나면 스킬 종료 
        if (IsSkillAnimationEnd("HeavyDestroyerEnd"))
        {
            skillTriggered = false; // 다음 스킬 사용을 위해 플래그 리셋
            monster.AttackController.SetDamages(0); //데미지 초기화
            return NodeState.Success;
        }

        // 위의 종료 조건에 해당하지 않으면 스킬이 아직 진행 중인 것
        return NodeState.Running;
    }

    private bool IsSkillAnimationPlaying(string animationName) // 해당 이름의 애니메이션이 재생중인지 확인
    {
        //스킬 애니메이션이 끝났는지 확인.
        bool isSkillAnimationPlaying = monster.Animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);

        if (isSkillAnimationPlaying)
        {
            Debug.Log($"Running skill: {skillData.skillName} (ID: {skillData.skillId})");
            return true;
        }
        else
        {
            Debug.Log($"Using skill: {skillData.skillName} (ID: {skillData.skillId})");
            return false;
        }
    }

    private bool IsSkillAnimationEnd(string animationName) // 해당 애니메이션이 끝났는지
    {
        var stateInfo = monster.Animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1f;
    }


}
