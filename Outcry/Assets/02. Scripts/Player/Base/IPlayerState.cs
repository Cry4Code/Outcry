using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    void Enter(PlayerController player); // 상태 변화했을 때 돌아감 
    void HandleInput(PlayerController player); // 입력에 따른 상태 전환 등을 처리
    void LogicUpdate(PlayerController player); // 실제 로직이 돌아가는 부분
    void Exit(PlayerController player); // 상태에서 나갈 때 돌아감
}
