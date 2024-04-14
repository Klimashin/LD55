using UnityEngine;


public abstract class DanceMovement : ScriptableObject
{
    public abstract IDanceMovementHandler GetHandler(RoundDance dance, Dancer dancer);
}

public abstract class DanceMovementHandler<T> : IDanceMovementHandler where T : DanceMovement
{
    protected readonly RoundDance dance;
    protected readonly Dancer dancer;
    protected T movement;
    protected float radiusOnStart;

    public DanceMovementHandler(DanceMovement movement, RoundDance dance, Dancer dancer)
    {
        this.dance = dance;
        this.dancer = dancer;
        this.movement = (T) movement;
        radiusOnStart = (dancer.transform.position - dance.Center).magnitude;
    }
    
    public abstract Vector3 HandleDancerPosition(float deltaTime, float normalizedTime);
    
    public virtual void OnStartSegment() {}
    
    public virtual void OnEndSegment() {}
}

public interface IDanceMovementHandler
{
    public Vector3 HandleDancerPosition(float deltaTime, float normalizedTime);
    
    public void OnStartSegment() {}
    
    public void OnEndSegment() {}
}
