using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataModel
{
    public int maxHealth;
    public int maxStamina;
    public float rateStamina;
    public float fullStamina;
    public int specialAttackStamina;
    public int dodgeStamina;
    public float dodgeInvincibleTime;
    public int doubleJumpStamina;
    public int parryStamina;
    public float parryInvincibleTime;
    public int parryDamage;
    public int wallJumpStamina;
    public float invincibleTime;
    public int[] normalAttackDamage;
    public int jumpAttackDamage;
    public int downAttackDamage;
    public float jumpforce;
    public int skill_Ids;
    public float moveSpeed;

    public PlayerDataModel(
        int  maxHealth, int  maxStamina, float rateStamina, float fullStamina, int specialAttackStamina, int dodgeStamina, float dodgeInvincibleTime, int doubleJumpStamina, int parryStamina,  float parryInvincibleTime,
        int parryDamage, int wallJumpStamina,  float invincibleTime, int[] normalAttackDamage, int jumpAttackDamage, int downAttackDamage, float jumpforce, int skill_Ids, float moveSpeed
        )
    {
        this.maxHealth = maxHealth;
        this.maxStamina = maxStamina;
        this.rateStamina = rateStamina;
        this.fullStamina = fullStamina;
        this.specialAttackStamina = specialAttackStamina;
        this.dodgeStamina = dodgeStamina;
        this.dodgeInvincibleTime = dodgeInvincibleTime;
        this.doubleJumpStamina = doubleJumpStamina;
        this.parryStamina = parryStamina;
        this.parryInvincibleTime = parryInvincibleTime;
        this.parryDamage = parryDamage;
        this.wallJumpStamina = wallJumpStamina;
        this.invincibleTime = invincibleTime;
        this.normalAttackDamage = normalAttackDamage;
        this.jumpAttackDamage = jumpAttackDamage;
        this.downAttackDamage = downAttackDamage;
        this.jumpforce = jumpforce;
        this.skill_Ids = skill_Ids;
        this.moveSpeed = moveSpeed;
    }
}
