using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeController : MonoBehaviour {

    private Animator animator;
    [SerializeField]
    private float knifeLenght = 1f;

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

    private bool isEnemyNear;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Attack() {
        var hit = Physics2D.CircleCast(transform.position, knifeLenght, 
            transform.right, knifeLenght, enemyLayerMask);
        if(hit) {
            HealthComponent enemyHealth = hit.collider.GetComponent<HealthComponent>();
            enemyHealth.Damage(damage);
        }        
    }
}
