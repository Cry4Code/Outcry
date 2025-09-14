using System;
using System.Collections.Generic;

public class Skill1Sequence : SequenceNode
{
    public Skill1Sequence(string nodeName)
    {
        this.nodeName = nodeName;
    }

    public override NodeState Tick()
    {
        UnityEngine.Debug.Log(nodeName);
        return base.Tick();
    }
}


[Serializable]
public class SkillNode
{
    public int skillId;
    public SequenceNode skillNode;
}
public static class SkillNodeDatabase
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