using System;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Dance Movements/ZigZagMovement")]
public class ZigZagMovement : DanceMovement
{
    public ZigZagType Type;
    [ShowIf("Type", ZigZagType.Concentric)] public float Delta;
    [ShowIf("Type", ZigZagType.Round)] public float AngularVelocity;
    public int TimesPerSegment;

    public enum ZigZagType
    {
        Round,
        Concentric
    }

    public override IDanceMovementHandler GetHandler(RoundDance dance, Dancer dancer)
    {
        return new ZigZagMovementHandler(this, dance, dancer);
    }
}

public class ZigZagMovementHandler : DanceMovementHandler<ZigZagMovement>
{
    public ZigZagMovementHandler(DanceMovement movement, RoundDance dance, Dancer dancer) : base(movement, dance, dancer)
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
        var currentStepData = GetStepByNormalizedTime(normalizedTime);
        if (movement.Type == ZigZagMovement.ZigZagType.Concentric)
        {
            
            float radius;
            switch (currentStepData.step)
            {
                case 0:
                    radius = Mathf.Lerp(radiusOnStart, radiusOnStart + movement.Delta, currentStepData.stepNormalizedTime);
                    break;
                case 1:
                    radius = Mathf.Lerp(radiusOnStart + movement.Delta, radiusOnStart, currentStepData.stepNormalizedTime);
                    break;
                case 2:
                    radius = Mathf.Lerp(radiusOnStart, radiusOnStart - movement.Delta, currentStepData.stepNormalizedTime);
                    break;
                case 3:
                    radius = Mathf.Lerp(radiusOnStart - movement.Delta, radiusOnStart, currentStepData.stepNormalizedTime);
                    break;
                default:
                    throw new Exception();
            }

            var currentPosition = dancer.transform.position;
            var currentAngle = Mathf.Atan2(currentPosition.y, currentPosition.x);
            return Utils.GetPointOnCircle(dance.Center, radius, currentAngle);
        }
        else
        {
            var currentPosition = dancer.transform.position;
            var radius = (currentPosition - dance.Center).magnitude;
            var currentAngle = Mathf.Atan2(currentPosition.y, currentPosition.x);
            var deltaAngle = Mathf.Deg2Rad * movement.AngularVelocity * deltaTime * (currentStepData.step % 2 == 0 ? 1 : -1);
            var angle = currentAngle + deltaAngle;
            return Utils.GetPointOnCircle(dance.Center, radius, angle);
        }
    }

    private (int step, float stepNormalizedTime) GetStepByNormalizedTime(float normalizedTime)
    {
        int scaledNormalizedTime = Mathf.FloorToInt(normalizedTime * 1000000);
        int zigZagStepDelta = 1000000 / 4 / movement.TimesPerSegment;
        int stepCount = scaledNormalizedTime / zigZagStepDelta;
        int step = stepCount % 4;
        float stepNormalizedTime = (float)(scaledNormalizedTime - zigZagStepDelta * stepCount) / zigZagStepDelta;
        return (step, stepNormalizedTime);
    }
}
