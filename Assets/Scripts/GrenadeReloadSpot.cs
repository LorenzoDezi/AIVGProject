using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeReloadSpot : MonoBehaviour
{
    [SerializeField]
    private int grenadeCount = 5;
    private int currGrenadeCount;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Start() {
        currGrenadeCount = grenadeCount;
        animator.SetBool("Empty", false);
    }

    public int GetAmmo(int count) {
        int result = 0;
        if(count > currGrenadeCount) {
            result = currGrenadeCount;
            currGrenadeCount = 0;
        } else {
            result = count;
            currGrenadeCount -= count;
        }
        if(currGrenadeCount == 0)
            animator.SetBool("Empty", true);
        return result;
    }
}
