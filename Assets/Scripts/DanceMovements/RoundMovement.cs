using UnityEngine;

[CreateAssetMenu(menuName = "Dance Movements/RoundMovement")]
public class RoundMovement : DanceMovement
{
    public bool IsClockwise;
    public float AngularVelocity;
    public AnimationCurve VelocityCurve;

    public override Vector3 GetNextPosition(Vector3 center, float radiusOnStart, Vector3 currentPosition, float deltaTime, float normalizedTime)
    {
        var radius = (currentPosition - center).magnitude;
        var velocity = VelocityCurve.Evaluate(normalizedTime) * Mathf.Deg2Rad * AngularVelocity;
        var currentAngle = Mathf.Atan2(currentPosition.y, currentPosition.x);
        var deltaAngle = velocity * deltaTime;
        if (IsClockwise)
        {
            deltaAngle *= -1;
        }

        var angle = currentAngle + deltaAngle;
        var result = Utils.GetPointOnCircle(center, radius, angle);
        return result;
    }
}
