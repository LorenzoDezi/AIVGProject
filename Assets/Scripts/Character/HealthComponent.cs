using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    private float maxHealth;
    public float MaxHealth => maxHealth;

    public float CurrHealth { get; private set; }
    public delegate void HealthChangedHandler(float currHealth);
    public event HealthChangedHandler HealthChanged;

    public UnityEvent Death = new UnityEvent();

    private void Start() {
        CurrHealth = maxHealth;
    }

    public void Damage(float damage) {
        if (damage < CurrHealth) {
            CurrHealth -= damage;
        } else if (CurrHealth > 0) {
            Die();
        }
        HealthChanged?.Invoke(CurrHealth);
    }

    public void Restore(float amount) {
        CurrHealth = Mathf.Clamp(CurrHealth + amount, 0f, maxHealth);
        HealthChanged?.Invoke(CurrHealth);
    }

    public void Die() {
        CurrHealth = 0f;
        Death.Invoke();
    }
}
