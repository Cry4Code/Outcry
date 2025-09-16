using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SkillNode
{
    public int skillId;
    public SkillSequenceNode skillNode;
}

[Serializable]  //임시, 지워야됨.
public class SkillNodeDataList: DataListBase<SkillNode>
{
    public override void Initialize()
    {
        dataList = new List<SkillNode>();
    }
    
    /// <summary>
    /// 데이터리스트에 id에 해당하는 스킬시퀀스노드가 있다면 true, 없다면 false 반환
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="skillSequenceNode"></param>
    /// <returns></returns>
    public bool GetSkillSequenceNode(int skillId, out SkillSequenceNode skillSequenceNode)
    {
        SkillNode node = dataList.FirstOrDefault(node => node.skillId == skillId);

        if (node == null)
        {
            skillSequenceNode = null;
            return false;
        }
        else
        {
            skillSequenceNode = node.skillNode;
            return true;
        }
    }
}
