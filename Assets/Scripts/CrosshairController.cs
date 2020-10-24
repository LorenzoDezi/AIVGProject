using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    private new Transform transform;

    void Awake() {
        transform = GetComponent<Transform>();
    }

    public void MoveAt(Vector2 position) {
        transform.position = position;
    }

    
}
