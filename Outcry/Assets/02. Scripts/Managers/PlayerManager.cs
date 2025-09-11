using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public Player player;
    private void OnEnable()
    {
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length == 0)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다.");
        }
        player = players[0];
        for (int i = 1; i < players.Length; i++)
        {
            Destroy(players[i]);
        }
    }
}
