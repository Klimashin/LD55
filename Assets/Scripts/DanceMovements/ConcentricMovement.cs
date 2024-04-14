using UnityEngine;

[CreateAssetMenu(menuName = "Dance Movements/ConcentricMovement")]
public class ConcentricMovement : DanceMovement
{
    public float TargetRadius;
    public bool OnlyOdd;
    public bool OnlyEven;

    public override IDanceMovementHandler GetHandler(RoundDance dance, Dancer dancer)
    {
        return new ConcentricMovementHandler(this, dance, dancer);
    }
}

public class ConcentricMovementHandler : DanceMovementHandler<ConcentricMovement>
{
    public ConcentricMovementHandler(DanceMovement movement, RoundDance dance, Dancer dancer) : base(movement, dance, dancer)
    {
    }

    public override void OnStartSegment()
    {
        base.OnStartSegment();
        if (movement.OnlyOdd && dancer.Index % 2 == 0
            || movement.OnlyEven && dancer.Index % 2 == 1)
        {
            return;
        }

        dancer.SetDressAnimate(true);
    }

    public override void OnEndSegment()
    {
        base.OnEndSegment();
        dancer.SetDressAnimate(false);
    }

    public override void HandleDancerPosition(float deltaTime, float normalizedTime)
    {
        if (movement.OnlyOdd && dancer.Index % 2 == 0
            || movement.OnlyEven && dancer.Index % 2 == 1)
        {
            return;
        }
        
        var radius = Mathf.Lerp(radiusOnStart, movement.TargetRadius, normalizedTime);
        var currentPosition = dancer.transform.position;
        var currentAngle = Mathf.Atan2(currentPosition.y, currentPosition.x);
        dancer.transform.position = Utils.GetPointOnCircle(dance.Center, radius, currentAngle);
    }
}
