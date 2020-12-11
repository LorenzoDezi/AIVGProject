using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (!enemyLayerMask.ContainsLayer(collision.gameObject.layer))
            return;

        isEnemyNear = true;
        enemyHealth = collision.gameObject.GetComponent<HealthComponent>();
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (!enemyLayerMask.ContainsLayer(collision.gameObject.layer))
            return;

        isEnemyNear = false;
        enemyHealth = null;
    }

    public void Attack() {
        if (isEnemyNear)
            enemyHealth.Damage(damage);
    }
}
