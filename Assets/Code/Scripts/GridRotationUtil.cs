using UnityEngine;

public static class GridRotationUtil
{
    public static GridOrientation GetOrientation(float zRotation)
    {
        zRotation = NormalizeAngle(zRotation);

        // Handle both positive and negative rotations
        if (IsAngle(zRotation, 0f)) return GridOrientation.Up;
        if (IsAngle(zRotation, 90f) || IsAngle(zRotation, -270f)) return GridOrientation.Left;
        if (IsAngle(zRotation, 180f) || IsAngle(zRotation, -180f)) return GridOrientation.Down;
        if (IsAngle(zRotation, 270f) || IsAngle(zRotation, -90f)) return GridOrientation.Right;

        // Fallback
        return GridOrientation.Up;
    }

    private static float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0f) angle += 360f;
        return angle;
    }

    private static bool IsAngle(float a, float b, float tolerance = 1f)
    {
        return Mathf.Abs(a - b) <= tolerance;
    }
}