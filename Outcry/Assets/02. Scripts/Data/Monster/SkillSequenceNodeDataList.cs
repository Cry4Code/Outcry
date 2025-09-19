using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]  //임시, 지워야됨.
public class SkillSequenceNodeDataList: DataListBase<SkillSequenceNode>
{
    /// <summary>
    /// Json 데이터로 불러오지 않기 때문에, 스킬 노드를 생성할때마다 이 곳에 추가해줘야함.
    /// </summary>
    public override void Initialize()
    {
        dataList = new List<SkillSequenceNode>();
        
        //스킬 시퀀스 노드 생성
        dataList.Add(new MetalBladeSkillSequenceNode(103001));
        dataList.Add(new HeavyDestroyerSkillSequenceNode(103002));
        dataList.Add(new ThreePointSkillSequenceNode(103003));
        dataList.Add(new EarthquakeSkillSequenceNode(103004));
        dataList.Add(new StompSkillSequenceNode(103005));
        dataList.Add(new UpperSlashSequenceNode(103006));
        
    }
    
    /// <summary>
    /// 데이터리스트에 id에 해당하는 스킬시퀀스노드가 있다면 true, 없다면 false 반환
    /// </summary>
    /// <param name="skillId"></param>
    /// <param name="skillSequenceNode"></param>
    /// <returns></returns>
    public bool GetSkillSequenceNode(int skillId, out SkillSequenceNode skillSequenceNode)
    {
        var tempData = dataList.FirstOrDefault(node => node.SkillId == skillId);

        switch (tempData)
        {
            case MetalBladeSkillSequenceNode:
                skillSequenceNode = new MetalBladeSkillSequenceNode(skillId);
                break;
            case EarthquakeSkillSequenceNode:
                skillSequenceNode = new EarthquakeSkillSequenceNode(skillId);
                break;
            case StompSkillSequenceNode:
                skillSequenceNode = new StompSkillSequenceNode(skillId);
                break;
            case UpperSlashSequenceNode:
                skillSequenceNode = new UpperSlashSequenceNode(skillId);
                break;
            case HeavyDestroyerSkillSequenceNode:
                skillSequenceNode = new HeavyDestroyerSkillSequenceNode(skillId);
                break;
            case ThreePointSkillSequenceNode:
                skillSequenceNode = new ThreePointSkillSequenceNode(skillId);
                break;
            default:
                skillSequenceNode = null;
                break;
        }
        
        if (tempData == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
