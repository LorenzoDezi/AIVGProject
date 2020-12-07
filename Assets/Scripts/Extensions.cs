using UnityEngine;

public static class Math2D {

    public static Vector3 RotatedBy(this Vector2 vector, float angle) {
        return new Vector3(
            vector.x * Mathf.Cos(angle * Mathf.Deg2Rad) + vector.y * -Mathf.Sin(angle * Mathf.Deg2Rad),
            vector.x * Mathf.Sin(angle * Mathf.Deg2Rad) + vector.y * Mathf.Cos(angle * Mathf.Deg2Rad), 0
            ).normalized;
    }
}

public static class TransformExtensions {
    public static bool HasObstacleInBetween(this Transform from, Transform to, LayerMask obstacleLayerMask) {
        return Physics2D.Linecast(from.position, to.position, obstacleLayerMask);
    }

    public static float SqrDistance(this Transform start, Transform end) {
        return (end.position - start.position).sqrMagnitude;
    }
}
