using JetBrains.Annotations;
using Reflex.Attributes;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deadZone;
    [SerializeField] private float _speedCutDistance;
    [SerializeField] private Transform _rendererTransform;
    
    private Camera _camera;
    private float _currentSpeed;
    [CanBeNull] private Transform _lookTransform;

    [Inject]
    private void Inject(Camera mainCamera)
    {
        _camera = mainCamera;
    }

    public void SetLookTransform([CanBeNull] Transform t)
    {
        _lookTransform = t;
    }

    private void Update()
    {
        bool isMoving = !Input.GetMouseButton(0);
        if (!isMoving)
        {
            _currentSpeed = 0f;
            return;
        }

        var mouseWorldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPoint.z = 0f;

        var distance = Vector3.Distance(mouseWorldPoint, transform.position);
        if (distance <= _deadZone)
        {
            return;
        }

        var maxSpeed = _maxSpeed;
        if (distance <= _speedCutDistance)
        {
            maxSpeed = Mathf.Lerp(_maxSpeed, 0f, (_speedCutDistance - distance) / _speedCutDistance);
        }
        
        _currentSpeed += _acceleration * Time.deltaTime;
        if (_currentSpeed > maxSpeed)
        {
            _currentSpeed = maxSpeed;
        }

        var currentPosition = transform.position;
        var translation = (mouseWorldPoint - currentPosition).normalized * (_currentSpeed * Time.deltaTime);
        var targetPosition = currentPosition + translation;
        
        var lookDirection = _lookTransform != null
            ? (currentPosition - _lookTransform.position).normalized
            : translation.normalized;
        _rendererTransform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        transform.Translate(translation);
    }
}
