using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 임시 클래스들

//임시 클래스들
public class Player
{
    
}

public class MonsterData
{
    public int[] specialSkillIds;
    public int[] commonSkillIds;
}

public class SkillNode
{
    public int skillId;
    public SequenceNode skillNode;
}
public static class BehaviorTreeNodeData
{
    public static List<SkillNode> skillNodes = new List<SkillNode>()
    {
        new SkillNode() { skillId = 1, skillNode = new Skill1Sequence("skillId: 1") },
        new SkillNode() { skillId = 2, skillNode = new Skill1Sequence("skillId: 2") },
        new SkillNode() { skillId = 3, skillNode = new Skill1Sequence("skillId: 3") },
        new SkillNode() { skillId = 4, skillNode = new Skill1Sequence("skillId: 4") },
        new SkillNode() { skillId = 5, skillNode = new Skill1Sequence("skillId: 5") }
    };
}

public class Skill1Sequence : SequenceNode
{
    public Skill1Sequence(string nodeName)
    {
        this.nodeName = nodeName;
    }

    public override NodeState Tick()
    {
        Debug.Log(nodeName);
        return base.Tick();
    }
}

//이상 임시끝

#endregion
public class MonsterAI : MonoBehaviour
{
    private Node rootNode;
    private Player target;
    private bool isAttacking;

    private MonsterData monsterData;
    void Start()
    {
        monsterData = new MonsterData();
        monsterData.specialSkillIds = new int[] { 1, 2 }; //예
        monsterData.commonSkillIds = new int[] { 3, 4, 5 }; //예
        InitializeBehaviorTree();
    }
    void Update()
    {
        UpdateAI();
    }
    void InitializeBehaviorTree()
    {
        //monster root node
        //>ConditionNode: IsDead
        //>SequenceNode: AttackSequence
        //>>ConditionNode: CanAttack
        //>>SelectorNode: AttackSelector
        //>>>SelectorNode: SpecialSkillSelector
        //>>>>SequenceNode: SpecialSkill1Sequence
        //>>>>>ConditionNode: CanPerform
        //>>>>>ActionNode: SpecialSkill1Action
        //>>>>SequenceNode: SpecialSkill2Sequence
        //>>>>>ConditionNode: CanPerform
        //>>>>>ActionNode: SpecialSkill2Action
        //>>>SelectorNode: CommonSkillSelector  //여기는 하위 노드 순서들 셔플할것임
        //>>>>SequenceNode: CommonSkill1Sequence //갖고 있는 commonskillids 만큼 자식 시퀀스 생성
        //>>>>>ConditionNode: CanPerform
        //>>>>>ActionNode: CommonSkill1Action
        //>SelectorNode: ChaseSelector
        //>>SequenceNode: DashSequence
        //>>>ConditionNode: CanDash
        //>>>ActionNode: DashAction
        //>>ActionNode: ChaseAction
        //이렇게 만들어야함....
        //근데 이게 범용성 있게 사용할 수 있으면 좋겟단 말이지...
        //즉 몬스터마다 스킬이 다르니까 그걸로 트리를 짜야하는데...
        //흠... 몬스터 데이터에 스킬 아이디들 배열로 넣어놓고
        //그리고 그걸로 트리 짜야함
        
        //각 스킬 시퀀스를 아이디에 따라 찾아오는건?
        //스킬 이름을 클래스 이름으로 하면?
        //예를 들어 SharkSharkShark이라는 스킬 이름이 있다면 SharkSharkSharkSequence, SharkSharkSharkAction
        //이런식으로 클래스 이름을 짓는거지
        //그리고 리플렉션으로 클래스 찾아서 인스턴스 생성
        //이런식으로 하면 범용성 있게 트리 짤 수 있을듯????
        //흠... 근데 리플렉션이 성능에 영향이 있을까
        //걍 스킬 시퀀스를 담는 리스트를 생성하는건?? 스킬 아이디랑 맞춰서 딕셔너리로 만들까?
        //그럼 BehaviorTreeManager가 있어야하나?
        //BehaviorTreeManager는 넘 거창하고, 그냥 BehaviorTreeNodeData로 하면 될 듯
        //그리고 이제 기본 보스 패턴 구상했던 대로 구현하면 됨.
        //루트부터 생성하면 됨
        
        SelectorNode rootNode = new SelectorNode();
        
        //isDead
        ConditionNode isDeadNode = new ConditionNode(() => false); //임시
        rootNode.AddChild(isDeadNode);
        
        //AttackSequence
        SequenceNode attackSequenceNode = new SequenceNode();
        ConditionNode canAttackNode = new ConditionNode(() => true); //임시
        SelectorNode attackSelectorNode = new SelectorNode();
        ActionNode waitActionNode = new ActionNode(() => NodeState.Running); //대기 액션 노드 임시
        attackSequenceNode.AddChild(canAttackNode);
        attackSequenceNode.AddChild(attackSelectorNode);
        attackSequenceNode.AddChild(waitActionNode);
        
        rootNode.AddChild(attackSequenceNode);
        
        
        //스페셜 스킬 셀럭터 노드 자식들 생성.
        SelectorNode specialSkillSelectorNode = new SelectorNode();
        foreach (int id in monsterData.specialSkillIds)
        {
            SkillNode skillNode = BehaviorTreeNodeData.skillNodes.Find(x => x.skillId == id);
            
            if (skillNode != null)
            {
                specialSkillSelectorNode.AddChild(skillNode.skillNode);
            }
        }
        attackSelectorNode.AddChild(specialSkillSelectorNode);
        
        //일반 스킬 셀럭터 노드 자식들 생성.
        SelectorNode commonSkillSelectorNode = new SelectorNode();
        foreach (int id in monsterData.commonSkillIds)
        {
            SkillNode skillNode = BehaviorTreeNodeData.skillNodes.Find(x => x.skillId == id);
            if (skillNode != null)
            {
                commonSkillSelectorNode.AddChild(skillNode.skillNode);
            }
        }
        attackSelectorNode.AddChild(commonSkillSelectorNode);

        //chase
        SelectorNode chaseSelectorNode = new SelectorNode();
        
        //dash
        SequenceNode dashSequenceNode = new SequenceNode();
        ConditionNode canDashNode = new ConditionNode(() => false); //임시
        ActionNode dashActionNode = new ActionNode(() => NodeState.Running); //임시
        dashSequenceNode.AddChild(canDashNode);
        dashSequenceNode.AddChild(dashActionNode);
        chaseSelectorNode.AddChild(dashSequenceNode);
        
        //chase Action
        ActionNode chaseActionNode = new ActionNode(() => NodeState.Running); //임시
        chaseSelectorNode.AddChild(chaseActionNode);

        this.rootNode = rootNode;
    }
    
    void UpdateAI()
    {
        if (rootNode == null) return;
        rootNode.Tick();
    }

}
