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
    [SerializeField] private Animator _animator;
    [SerializeField] private bool _useArrowControls;
    
    private Camera _camera;
    private float _currentSpeed;
    private static readonly int DressAnimateHash = Animator.StringToHash("AnimateDress");
    private static readonly int HandsAnimateHash = Animator.StringToHash("Hands");
    [CanBeNull] private Transform _lookTransform;

    [Inject]
    private void Inject(Camera mainCamera)
    {
        _camera = mainCamera;
    }

    public void SetLookTransform([CanBeNull] Transform t)
    {
        _lookTransform = t;

        if (_lookTransform != null)
        {
            var lookDirection = (_lookTransform.position - transform.position).normalized;
            _rendererTransform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        }
    }
    
    public void SetHandsPos(Dancer.HandsPos handsPos)
    {
        _animator.SetInteger(HandsAnimateHash, (int)handsPos);
    }

    private void Update()
    {
        if (_useArrowControls)
        {
            ArrowsControl();
            return;
        }

        MouseControl();
    }

    private void MouseControl()
    {
        _animator.SetBool(DressAnimateHash, false);
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
            _currentSpeed = 0f;
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
        var translationDistance = _currentSpeed * Time.deltaTime;
        var translation = (mouseWorldPoint - currentPosition).normalized * translationDistance;

        var lookDirection = _lookTransform != null
            ? (_lookTransform.position - currentPosition).normalized
            : translation.normalized;
        _rendererTransform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        transform.Translate(translation);
        _animator.SetBool(DressAnimateHash, translationDistance >= float.Epsilon);
    }

    private void ArrowsControl()
    {
        bool moveLeft = Input.GetKey(KeyCode.LeftArrow);
        bool moveRight = Input.GetKey(KeyCode.RightArrow);
        bool moveUp = Input.GetKey(KeyCode.UpArrow);
        bool moveDown = Input.GetKey(KeyCode.DownArrow);

        if (!moveLeft && !moveRight && !moveDown && !moveUp)
        {
            _currentSpeed = 0;
            _animator.SetBool(DressAnimateHash, false);
            return;
        }
        
        _animator.SetBool(DressAnimateHash, true);

        var direction = Vector3.zero;
        if (moveLeft)
            direction += Vector3.left;
        if (moveRight)
            direction += Vector3.right;
        if (moveDown)
            direction += Vector3.down;
        if (moveUp)
            direction += Vector3.up;

        direction = direction.normalized;
        _currentSpeed += _acceleration * Time.deltaTime;
        if (_currentSpeed > _maxSpeed)
        {
            _currentSpeed = _maxSpeed;
        }
        
        var currentPosition = transform.position;
        var translationDistance = _currentSpeed * Time.deltaTime;
        var translation = direction * translationDistance;

        var lookDirection = _lookTransform != null
            ? (_lookTransform.position - currentPosition).normalized
            : translation.normalized;
        _rendererTransform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
        transform.Translate(translation);
    }
}
