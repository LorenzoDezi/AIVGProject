using UnityEngine;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private Transform bar;
    private Vector3 currentSize = Vector3.zero;

    [SerializeField]
    private HealthComponent healthComponent;

    [SerializeField]
    private Vector3 followDistance;
    private new Transform transform;
    private Transform transformToFollow;

    private void Awake() {
        transform = GetComponent<Transform>();
        transformToFollow = healthComponent.transform;
    }

    private void Start() {
        healthComponent.HealthChange.AddListener(OnHealthChange);
        healthComponent.Death.AddListener(OnDeath);
        currentSize.y = 1f;
    }

    private void Update() {
        transform.position = transformToFollow.position + followDistance;
    }

    private void OnDeath() {
        gameObject.SetActive(false);
    }

    private void OnHealthChange(float currentHealth) {
        currentSize.x = currentHealth / healthComponent.MaxHealth;
        bar.localScale = currentSize;
    }
}
