using UnityEngine;

[CreateAssetMenu(menuName = "Dance Movements/RoundMovement")]
public class RoundMovement : DanceMovement
{
    public bool IsClockwise;
    public float AngularVelocity;
    public AnimationCurve VelocityCurve;

    public override IDanceMovementHandler GetHandler(RoundDance dance, Dancer dancer)
    {
        return new RoundMovementHandler(this, dance, dancer);
    }
}

public class RoundMovementHandler : DanceMovementHandler<RoundMovement>
{
    public RoundMovementHandler(DanceMovement movement, RoundDance dance, Dancer dancer) : base(movement, dance, dancer)
    {
    }
    
    public override void OnStartSegment()
    {
        base.OnStartSegment();
        dancer.SetDressAnimate(true);
    }

    public override void OnEndSegment()
    {
        base.OnEndSegment();
        dancer.SetDressAnimate(false);
    }
    
    public override Vector3 HandleDancerPosition(float deltaTime, float normalizedTime)
    {
        var currentPosition = dancer.transform.position;
        var radius = (currentPosition - dance.Center).magnitude;
        var velocity = movement.VelocityCurve.Evaluate(normalizedTime) * Mathf.Deg2Rad * movement.AngularVelocity;
        var currentAngle = Mathf.Atan2(currentPosition.y, currentPosition.x);
        var deltaAngle = velocity * deltaTime;
        if (movement.IsClockwise)
        {
            deltaAngle *= -1;
        }

        var angle = currentAngle + deltaAngle;
        return Utils.GetPointOnCircle(dance.Center, radius, angle);
    }
}
