using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    private new Transform transform;
    private Vector2 maxScreenPosition;

    void Awake() {
        transform = GetComponent<Transform>();
        maxScreenPosition = new Vector2(Screen.width, Screen.height);
    }

    public void MoveAt(Vector2 screenPosition, Camera mainCamera) {
        screenPosition.x = Mathf.Clamp(screenPosition.x, 0f, maxScreenPosition.x);
        screenPosition.y = Mathf.Clamp(screenPosition.y, 0f, maxScreenPosition.y);
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(screenPosition);
        transform.position = worldMousePosition;
    }

    
}
