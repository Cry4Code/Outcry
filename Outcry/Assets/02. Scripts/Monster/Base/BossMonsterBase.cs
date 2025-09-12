using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossMonsterBase : MonoBehaviour
{
    protected BossMonsterData monsterData;
    protected List<MonsterSkillData> skillDatas;
    protected MonsterCondition condition;

    protected abstract void Initialize();
}
