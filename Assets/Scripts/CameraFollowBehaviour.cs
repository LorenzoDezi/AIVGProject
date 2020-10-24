using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform transformToFollow;

    private new Transform transform;
    private Vector3 currentPosition;

    private void Awake() {
        transform = GetComponent<Transform>();
        currentPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 positionToFollow = transformToFollow.position;
        currentPosition.x = positionToFollow.x;
        currentPosition.y = positionToFollow.y;
        transform.position = currentPosition;
    }
}
