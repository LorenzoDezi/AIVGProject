using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
    private Slider slider;
    private HealthComponent healthComponent;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    private void Start() {
        healthComponent = GameManager.Player.GetComponent<HealthComponent>();
        slider.maxValue = healthComponent.MaxHealth;
        slider.value = slider.maxValue;
        healthComponent.HealthChanged += OnHealthChange;
    }

    private void OnHealthChange(float currHealth) {
        slider.value = currHealth;
    }
}
