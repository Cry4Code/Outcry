using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class StoneController : MonoBehaviour, ICountable
{
    [SerializeField] private LayerMask playerLayer;
    private int damage = 1;
        
    private void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float clipLength = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, clipLength);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("OnTriggerStay2D: " + other.gameObject.name);

        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0) //(other.gameObject.layer == playerLayer)
        {
            Debug.Log("Playerlayer hit");
            Player damagable = other.gameObject.GetComponentInParent<Player>();
            if (damagable != null && damage > 0)
            {
                //todo. Player IDamagable 구현 후 데미지 주기
                // damagable.TakeDamage(damage);
                Debug.Log("Player took " + damage + " damage from " + gameObject.name);
            }
        }
    }

    public void CounterAttacked()
    {
        Debug.Log("Stone CounterAttacked");
        //돌이 부딪힘. 아직까지는 구현예정인 기능 없음.
    }
}
