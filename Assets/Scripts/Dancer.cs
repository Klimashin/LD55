using JetBrains.Annotations;
using UnityEngine;

public class Dancer : MonoBehaviour
{
    [SerializeField] private Transform _rendererTransform;
    [SerializeField] private Animator _animator;
    [CanBeNull] private Vector3? _lookTransform;
    
    private static readonly int DressAnimateHash = Animator.StringToHash("AnimateDress");
    private static readonly int HandsAnimateHash = Animator.StringToHash("Hands");

    public int Index { get; private set; }

    public void SetIndex(int index)
    {
        Index = index;
    }

    public void SetHandsPos(HandsPos handsPos)
    {
        _animator.SetInteger(HandsAnimateHash, (int)handsPos);
    }

    public void SetLookTransform([CanBeNull] Vector3? t)
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
            var lookDirection = ((Vector3)_lookTransform - transform.position).normalized;
            _rendererTransform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        }
    }

    public enum HandsPos
    {
        Hidden = 0,
        TPose = 1,
        Praise = 2
    }
}
