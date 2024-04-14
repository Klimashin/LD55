using UnityEngine;

[CreateAssetMenu(menuName = "Dance Movements/InPlaceCircleMovement")]
public class InPlaceCircleMovement : DanceMovement
{
    public float Radius;

    public override IDanceMovementHandler GetHandler(RoundDance dance, Dancer dancer)
    {
        return new InPlaceCircleMovementHandler(this, dance, dancer);
    }
}

public class InPlaceCircleMovementHandler : DanceMovementHandler<InPlaceCircleMovement>
{
    private readonly Vector3 _circleCenter;
    private readonly float _initialAngle;
    
    public InPlaceCircleMovementHandler(DanceMovement movement, RoundDance dance, Dancer dancer) : base(movement, dance, dancer)
    {
        var currentPos = dancer.transform.position;
        _circleCenter = currentPos + (dancer.transform.position - dance.Center).normalized * base.movement.Radius;
        _initialAngle = Mathf.Atan2(currentPos.y - _circleCenter.y, currentPos.x - _circleCenter.x);
    }

    public override void OnStartSegment()
    {
        base.OnStartSegment();
        dancer.SetDressAnimate(true);
        dancer.SetLookTransform(dancer.transform.position);
    }

    public override void OnEndSegment()
    {
        base.OnEndSegment();
        dancer.SetDressAnimate(false);
        dancer.SetLookTransform(dance.transform.position);
    }

    public override void HandleDancerPosition(float deltaTime, float normalizedTime)
    {
        var angle = _initialAngle + normalizedTime * 2 * Mathf.PI;
        dancer.transform.position = Utils.GetPointOnCircle(_circleCenter, movement.Radius, angle);
    }
}
