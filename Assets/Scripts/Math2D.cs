using UnityEngine;

public class Math2D {
    /// <summary>
    /// Rotates a vector in 2D by the angle specified
    /// </summary>
    /// <param name="vector">The 2D vector in 3D (for multiple use)</param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector3 RotatedVector(Vector2 vector, float angle) {
        return new Vector3(
            vector.x * Mathf.Cos(angle * Mathf.Deg2Rad) + vector.y * -Mathf.Sin(angle * Mathf.Deg2Rad),
            vector.x * Mathf.Sin(angle * Mathf.Deg2Rad) + vector.y * Mathf.Cos(angle * Mathf.Deg2Rad), 0
            ).normalized;
    }
}
