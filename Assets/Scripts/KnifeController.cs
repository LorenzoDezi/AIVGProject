using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyNearEvent : UnityEvent<bool> { }

public class KnifeController : MonoBehaviour {

    private Animator animator;

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
    private HealthComponent enemyHealth;
    [NonSerialized]
    public EnemyNearEvent EnemyNear = new EnemyNearEvent();

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (!enemyLayerMask.ContainsLayer(collision.gameObject.layer))
            return;

        var healthComp = collision.gameObject.GetComponent<HealthComponent>();
        if(healthComp != null) {
            isEnemyNear = true;
            EnemyNear.Invoke(true);
            enemyHealth = healthComp;
        }        
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (!enemyLayerMask.ContainsLayer(collision.gameObject.layer))
            return;
        if(isEnemyNear && enemyHealth.GetInstanceID() == collision.GetInstanceID()) {
            isEnemyNear = false;
            EnemyNear.Invoke(false);
            enemyHealth = null;
        }        
    }

    public void Attack() {
        if (isEnemyNear)
            enemyHealth.Damage(damage);
    }
}
