using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinKing : BossMonster
{
    protected override void Initialize()
    {
        monsterData = new();
        // 데어터 관리 메니저에서 json으로 MonsterData 생성하는 메서드 호출

        // skillDatas = new();
        // monsterData의 int[] skills를 기반으로 skillData 작성

        condition = gameObject.GetComponent<MonsterCondition>();

    }
}
