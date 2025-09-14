using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMove PlayerMove;
    public PlayerController PlayerController;
    public PlayerCondition PlayerCondition;
 

    private void Awake()
    {
        PlayerMove = GetComponent<PlayerMove>();
        PlayerController = GetComponent<PlayerController>();
        PlayerCondition = GetComponent<PlayerCondition>();

    }
}
