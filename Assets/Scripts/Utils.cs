using UnityEngine;

public static class Utils
{
    public static Vector3 GetPointOnCircle(Vector3 center, float radius, float angle)
    {
        var x = center.x + radius * Mathf.Cos(angle);
        var y = center.y + radius * Mathf.Sin(angle);
        return new Vector3(x, y, center.z);
    }
}

public static class RectTransformExtensions
{
    public static void SetLeft(this RectTransform rt, float left)
    {
        rt.offsetMin = new Vector2(left, rt.offsetMin.y);
    }

    public static void SetRight(this RectTransform rt, float right)
    {
        rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
    }

    public static void SetTop(this RectTransform rt, float top)
    {
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    public static void SetBottom(this RectTransform rt, float bottom)
    {
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
