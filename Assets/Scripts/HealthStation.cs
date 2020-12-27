using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class HealthStation : MonoBehaviour {
    [SerializeField]
    private int maxRefills = 5;
    private int currRefills;

    public bool CanRefill => currRefills > 0;
    public Transform Transform { get; private set; }

    public UnityEvent RefillEndEvent { get; } = new UnityEvent();

    private void Awake() {
        currRefills = maxRefills;
        Transform = GetComponent<Transform>();
    }

    public bool UseRefill() {
        if(currRefills > 0) {
            currRefills--;
            if (currRefills == 0)
                RefillEndEvent.Invoke();
            return true;
        }
        return false;
    }
}

