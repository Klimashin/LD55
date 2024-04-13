using JetBrains.Annotations;
using UnityEngine;

public class Dancer : MonoBehaviour
{
    [SerializeField] private Transform _rendererTransform;
    
    [CanBeNull] private Transform _lookTransform;
    
    public void SetLookTransform([CanBeNull] Transform t)
    {
        _lookTransform = t;
    }

    private void Update()
    {
        if (_lookTransform != null)
        {
            var lookDirection = (transform.position - _lookTransform.position).normalized;
            _rendererTransform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        }
    }
}
