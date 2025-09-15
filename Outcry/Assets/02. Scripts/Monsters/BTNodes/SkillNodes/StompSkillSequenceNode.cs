// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class StompSkillSequenceNode : SkillSequenceNode
// {
//     protected override bool CanPerform()
//     {
//         //플레이어와의 거리 2m 이내에 있을때
//         bool result;
//         
//         //todo. 2f는 2m 이내. MonsterSkillModel에서 이걸 받아올 수 있도록 변경해야함
//         if (Vector2.Distance(monster.transform.position, target.transform.position) <= 2f)
//         {
//             result = true;
//         }
//         else
//         {
//             result = false;
//         }
//
//         return result;
//     }
//
//     protected override NodeState SkillAction()
//     {
//         //범위 - 보스 전방 근접 3m의 부채꼴 범위
//         //도끼가 땅에 닿는 순간 데미지 발생  (16~21 프레임 )
//         
//         
//         
//         //기본 피해 : HP 2칸 감소
//         //추가 효과 : 피격시 플레이어 다운
//         
//         // - **플레이어 대응**
//         //     - 회피 사용 가능
//         //     - 패링 사용 가능
//     }
// }
