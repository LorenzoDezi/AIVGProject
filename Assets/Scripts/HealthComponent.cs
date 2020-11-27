using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthChangeEvent : UnityEvent<float> { }

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;

    public float CurrHealth { get; private set; }
    public HealthChangeEvent HealthChangeEvent = new HealthChangeEvent();
    public UnityEvent DeathEvent = new UnityEvent();

    private void Start() {
        CurrHealth = maxHealth;
    }

    public void Damage(float damage) {
        if(damage < CurrHealth) {
            CurrHealth -= damage;
        } else {
            Die();
        }
        HealthChangeEvent.Invoke(CurrHealth);
    }

    public void Restore(float amount) {
        CurrHealth = Mathf.Clamp(CurrHealth + amount, 0f, maxHealth);
        HealthChangeEvent.Invoke(CurrHealth);
    }

    public void Die() {
        DeathEvent.Invoke();
        Destroy(this);
    }
}
