using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementZone : MonoBehaviour
{
    [SerializeField] private List<Rectangle> _rectangles;

    public bool IsPointInZone(Vector3 point)
    {
        return _rectangles.Any(r => r.IsPointInRectangle(point));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        foreach (var rectangle in _rectangles)
        {
            var size = (Vector3)rectangle.Size;
            size.z = 1f;
            Gizmos.DrawCube(rectangle.Center, size);
        }
    }
}

[Serializable]
public class Rectangle
{
    public Vector2 Center;
    public Vector2 Size;

    public bool IsPointInRectangle(Vector3 point)
    {
        return point.x >= Center.x - Size.x / 2f
               && point.y >= Center.y - Size.y / 2f
               && point.x <= Center.x + Size.x / 2f
               && point.y <= Center.y + Size.y / 2f;
    }
}
