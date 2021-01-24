using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform transformToFollow = default;

    private new Transform transform;
    private Vector2 currentVelocity;
    private Vector3 currentPosition;
    private float startZValue;

    [SerializeField]
    private float smoothTime = 1f;
    [SerializeField]
    private float maxSpeed = 3f;

    private void Awake() {
        transform = GetComponent<Transform>();
        startZValue = transform.position.z;
    }

    void FixedUpdate() {
        Follow();
    }

    private void Follow() {
        Vector3 positionToFollow = transformToFollow.position;
        currentPosition = Vector2.SmoothDamp(
            transform.position, positionToFollow,
            ref currentVelocity, smoothTime, maxSpeed);
        currentPosition.z = startZValue;
        transform.position = currentPosition;
    }
}
