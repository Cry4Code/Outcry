using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHoldState : IPlayerState
{
    public void Enter(PlayerController player)
    {
        Debug.Log("벽짚기 진입");
    }

    public void Exit(PlayerController player)
    {
        Debug.Log("벽짚기 탈출");
    }

    public void HandleInput(PlayerController player)
    {
        Debug.Log("벽짚기 키 입력 처리");
    }

    public void LogicUpdate(PlayerController player)
    {
        Debug.Log("벽짚기 로직 실행");
    }
}
