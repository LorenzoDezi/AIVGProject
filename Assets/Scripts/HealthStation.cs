using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class HealthStation : MonoBehaviour {

    [SerializeField]
    private SpriteRenderer noRefillSprite;
    private SpriteRenderer sprite;

    [SerializeField]
    private int maxRefills = 5;
    private int currRefills;

    public bool CanRefill => currRefills > 0;
    public Transform Transform { get; private set; }

    public delegate void RefillsEmptyHandler();
    public event RefillsEmptyHandler RefillsEmpty;

    private void Awake() {

        currRefills = maxRefills;

        Transform = GetComponent<Transform>();
        sprite = GetComponent<SpriteRenderer>();
        noRefillSprite.enabled = false;
    }

    public bool UseRefill() {
        if(currRefills > 0) {
            currRefills--;
            if (currRefills == 0)
                RefillEnded();
            return true;
        }
        return false;
    }

    private void RefillEnded() {
        RefillsEmpty?.Invoke();
        var color = sprite.color;
        color.a = color.a/3f;
        sprite.color = color;
        noRefillSprite.enabled = true;
    }
}

