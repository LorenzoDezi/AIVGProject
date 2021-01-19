using GOAP;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LayerMaskExtensions {

    public static bool ContainsLayer(this LayerMask layerMask, int layer) {
        return layerMask == (layerMask | (1 << layer));
    }
}

public static class Math2D {

    public static Vector2 RotatedBy(this Vector2 original, float angle) {
        Vector2 result = new Vector2();
        result.x = original.x * Mathf.Cos(angle * Mathf.Deg2Rad) + original.y * -Mathf.Sin(angle * Mathf.Deg2Rad);
        result.y = original.x * Mathf.Sin(angle * Mathf.Deg2Rad) + original.y * Mathf.Cos(angle * Mathf.Deg2Rad);
        return result;

    }
}

public static class TransformExtensions {

    public static bool HasObstacleInBetween(this Transform from, Transform to, LayerMask obstacleLayerMask) {
        return Physics2D.Linecast(from.position, to.position, obstacleLayerMask);
    }

    public static float SqrDistance(this Transform start, Transform end) {
        return (end.position - start.position).sqrMagnitude;
    }

    public static float SqrDistance(this Transform start, Vector3 end) {
        return (end - start.position).sqrMagnitude;
    }
}
