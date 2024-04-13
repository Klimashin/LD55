using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deadZone;
    [SerializeField] private float _speedCutDistance;

    private float _currentSpeed;

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
        
        transform.Translate((mouseWorldPoint - transform.position).normalized * (_currentSpeed * Time.deltaTime));
    }
}
