using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMove PlayerMove;
    public PlayerController PlayerController;
    public PlayerCondition PlayerCondition;
    public AttackHitbox PlayerWeapon;
    public PlayerAttack PlayerAttack;

    private void Awake()
    {
        PlayerMove = GetComponent<PlayerMove>();
        PlayerController = GetComponent<PlayerController>();
        PlayerCondition = GetComponent<PlayerCondition>();
        PlayerAttack = GetComponent<PlayerAttack>();
        PlayerWeapon = GetComponentInChildren<AttackHitbox>();
    }

    private void Start()
    {
        PlayerWeapon = GetComponentInChildren<AttackHitbox>();
        PlayerWeapon.Init(this);
    }
}
