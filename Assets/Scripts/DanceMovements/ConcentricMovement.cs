using UnityEngine;

[CreateAssetMenu(menuName = "Dance Movements/ConcentricMovement")]
public class ConcentricMovement : DanceMovement
{
    public float TargetRadius;
    
    public override Vector3 GetNextPosition(Vector3 center, float radiusOnStart, Vector3 currentPosition, float deltaTime, float normalizedTime)
    {
        var radius = Mathf.Lerp(radiusOnStart, TargetRadius, normalizedTime);
        var currentAngle = Mathf.Atan2(currentPosition.y, currentPosition.x);
        var result = Utils.GetPointOnCircle(center, radius, currentAngle);
        return result;
    }
}
