using JetBrains.Annotations;
using UnityEngine;

public class Dancer : MonoBehaviour
{
    [SerializeField] private Transform _rendererTransform;
    [SerializeField] private Animator _animator;
    [CanBeNull] private Transform _lookTransform;

    private Vector3 _prevFramePos;
    private static readonly int DressAnimateHash = Animator.StringToHash("AnimateDress");
    
    public void SetLookTransform([CanBeNull] Transform t)
    {
        _lookTransform = t;
    }

    public void SetDressAnimate(bool isOn)
    {
        _animator.SetBool(DressAnimateHash, isOn);
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
