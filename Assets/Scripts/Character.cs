using System;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deadZone;

    private float _currentSpeed;

    private void Update()
    {
        bool isMoving = Input.GetMouseButton(0);
        if (!isMoving)
        {
            _currentSpeed = 0f;
            return;
        }

        var targetWorldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        if ((targetWorldPoint - transform.position).magnitude <= _deadZone)
        {
            return;
        }
        
        targetWorldPoint.z = 0f;
        _currentSpeed += _acceleration * Time.deltaTime;
        if (_currentSpeed > _maxSpeed)
        {
            _currentSpeed = _maxSpeed;
        }
        
        transform.Translate((targetWorldPoint - transform.position).normalized * (_currentSpeed * Time.deltaTime));
    }
}
