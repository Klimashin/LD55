using UnityEngine;


public abstract class DanceMovement : ScriptableObject
{
    public abstract Vector3 GetNextPosition(Vector3 center, float radiusOnStart, Vector3 currentPosition, float deltaTime, float normalizedTime);
}
