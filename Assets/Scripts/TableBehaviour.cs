using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TableBehaviour : MonoBehaviour
{
    [SerializeField, 
        Tooltip("The name of the cover layer, set when the table is turned")]
    private string coverLayerName = "Cover";
    private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    public void Turn() {
        animator.SetTrigger("Turn");
        gameObject.layer = LayerMask.NameToLayer(coverLayerName);
    }
}
