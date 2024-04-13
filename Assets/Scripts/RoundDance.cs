using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class RoundDance : MonoBehaviour
{
    [SerializeField] private GameObject _dancerPrefab;
    [SerializeField] private int _dancersCount;
    [SerializeField] private int _playerPosition;
    [SerializeField] private List<DanceSegment> _danceSegments;
    [SerializeField] private Vector3 Center;
    [SerializeField] private float _initialRadius;
    [SerializeField] private Character _player;
    [SerializeField] private float _errorDelta = 0.5f;
    [SerializeField] private float _errorUpDistanceCoefficient = 3f;
    [SerializeField] private float _errorUpRate = 2f;
    [SerializeField] private float _errorDownRate = 1f;
    [SerializeField] private float _maxErrorRate = 1000f;
    

    private GameObject[] _dancers;
    private Vector3 PlayerExpectedPosition => _dancers[_playerPosition].transform.position;
    private DanceState State { get; set; } = DanceState.WaitingForPlayer;

    public float ErrorRate { get; private set; } = 0f;
    public float ErrorRateNormalized => ErrorRate / _maxErrorRate;

    private enum DanceState
    {
        WaitingForPlayer,
        Started,
        Finished
    }

    private void Start()
    {
        InstantiateDancers();
    }

    private void Update()
    {
        if (State == DanceState.WaitingForPlayer)
        {
            if (IsPlayerInPlace())
            {
                Dance().Forget();
            }

            return;
        }

        if (State == DanceState.Started)
        {
            HandlePlayerErrors();
        }
    }

    private bool IsPlayerInPlace()
    {
        return Vector3.Distance(_player.transform.position, PlayerExpectedPosition) < _errorDelta;
    }

    private void HandlePlayerErrors()
    {
        if (IsPlayerInPlace())
        {
            ErrorRate -= _errorDownRate;
            if (ErrorRate < 0f)
            {
                ErrorRate = 0f;
            }
        }
        else
        {
            var distance = Vector3.Distance(_player.transform.position, PlayerExpectedPosition);
            var error = _errorUpRate + distance * _errorUpDistanceCoefficient;
            ErrorRate += error;
        }
    }

    /*[Button]
    public void Play()
    {
        InstantiateDancers();
        
        Dance().Forget();
    }*/

    private async UniTask Dance()
    {
        State = DanceState.Started;

        var segmentsQueue = new Queue<DanceSegment>(_danceSegments);
        while (segmentsQueue.Count > 0)
        {
            var segment = segmentsQueue.Dequeue();
            await PlaySegment(segment);
        }

        State = DanceState.Finished;
    }

    private async UniTask PlaySegment(DanceSegment segment)
    {
        float duration = 0f;
        float[] radiusOnStart = new float[_dancers.Length];
        for (var i = 0; i < _dancers.Length; i++)
        {
            radiusOnStart[i] = (_dancers[i].transform.position - Center).magnitude;
        }

        while (duration < segment.Duration)
        {
            var normalizedTime = duration / segment.Duration;
            for (var i = 0; i < _dancers.Length; i++)
            {
                var deltaTime = Time.deltaTime;
                var dancerPosition = _dancers[i].transform.position;
                foreach (var segmentMovement in segment.Movements)
                {
                    dancerPosition = segmentMovement.GetNextPosition(Center, radiusOnStart[i], dancerPosition, deltaTime, normalizedTime);
                }

                _dancers[i].transform.position = dancerPosition;
            }

            duration += Time.deltaTime;

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
    }

    private void InstantiateDancers()
    {
        if (_dancers is { Length: > 0 })
        {
            foreach (var t in _dancers)
            {
                Destroy(t);
            }
        }
        
        _dancers = new GameObject[_dancersCount];
        float angle = 0f;
        float deltaAngle = 2 * Mathf.PI / _dancersCount;
        for (int i = 0; i < _dancersCount; i++)
        {
            var dancer = Instantiate(_dancerPrefab);
            dancer.transform.position = Utils.GetPointOnCircle(Center, _initialRadius, angle);
            _dancers[i] = dancer;
            angle += deltaAngle;

            if (i == _playerPosition)
            {
                dancer.gameObject.SetActive(false);
            }
        }
    }
}

[Serializable]
public class DanceSegment
{
    public float Duration;
    public List<DanceMovement> Movements;
}
