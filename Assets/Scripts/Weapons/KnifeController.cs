using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour {

    private Animator animator;
    [SerializeField]
    private float knifeLenght = 1f;

    [SerializeField]
    private LayerMask hitLayerMask;
    [SerializeField]
    private LayerMask enemyLayerMask;

    [SerializeField]
    private float damage = 10f;

    private bool isAttacking;
    public bool IsAttacking {
        get => isAttacking;
        set {
            isAttacking = value;
            animator.SetBool("IsAttacking", value);
        }
    }

    public float WeaponRange => knifeLenght * 2;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack() {
        var hit = Physics2D.CircleCast(transform.position, knifeLenght, 
            transform.right, knifeLenght, 
            hitLayerMask
        );
        if(hit) {
            var hitGameObj = hit.collider.gameObject;
            if(enemyLayerMask.ContainsLayer(hitGameObj.layer)) {
                HealthComponent enemyHealth = hitGameObj.GetComponent<HealthComponent>();
                enemyHealth.Damage(damage);
            }            
        }        
    }
}
