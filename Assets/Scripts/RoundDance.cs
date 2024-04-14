using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundDance : MonoBehaviour
{
    [SerializeField] private Dancer _dancerPrefab;
    [SerializeField] private int _dancersCount;
    [SerializeField] private int _playerPosition;
    [SerializeField] private List<DanceSegment> _danceSegments;
    [SerializeField] private float _initialRadius;
    [SerializeField] private float _errorDelta = 0.5f;
    [SerializeField] private float _errorUpDistanceCoefficient = 3f;
    [SerializeField] private float _errorUpRate = 2f;
    [SerializeField] private float _errorDownRate = 1f;
    [SerializeField] private float _maxErrorRate = 1000f;
    [SerializeField] private float _positionError = 0.1f;
    [SerializeField] private AudioClip _audio1;

    private SoundSystem _soundSystem;
    private CameraController _cameraController;
    private LightsController _lightsController;
    private EyesController _eyesController;
    private Character _player;
    private Dancer[] _dancers;
    private Vector3 PlayerExpectedPosition => _dancers[_playerPosition].transform.position;
    private DanceState State { get; set; } = DanceState.WaitingForPlayer;

    public Vector3 Center => transform.position;
    public float ErrorRate { get; private set; } = 0f;
    public float ErrorRateNormalized => ErrorRate / _maxErrorRate;
    public bool IsGameOver => ErrorRate >= _maxErrorRate;

    [Inject]
    private void Inject(SoundSystem soundSystem, CameraController cameraController, Character player, LightsController lightsController, EyesController eyesController)
    {
        _soundSystem = soundSystem;
        _cameraController = cameraController;
        _lightsController = lightsController;
        _eyesController = eyesController;
        _player = player;
    }

    private enum DanceState
    {
        WaitingForPlayer,
        Started,
        Finished,
        GameOver
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
                DanceRoutine().Forget();
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
        
        _eyesController.OnErrorRateUpdated(ErrorRateNormalized);
    }

    private async UniTask DanceRoutine()
    {
        _cameraController.SetCamera(CameraController.CameraType.Dance);
        _soundSystem.PlayMusicClip(_audio1);
        _player.SetLookTransform(transform);
        State = DanceState.Started;

        var segmentsQueue = new Queue<DanceSegment>(_danceSegments);
        while (segmentsQueue.Count > 0)
        {
            var segment = segmentsQueue.Dequeue();
            await PlaySegment(segment);

            if (IsGameOver)
            {
                GameOverRoutine().Forget();
                return;
            }
        }

        GameCompletedRoutine().Forget();
    }

    private async UniTaskVoid GameOverRoutine()
    {
        State = DanceState.GameOver;
        _player.SetLookTransform(null);
        _soundSystem.FadeCurrentMusic(1f);
        await _lightsController.LightsOff(1f);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private async UniTaskVoid GameCompletedRoutine()
    {
        State = DanceState.Finished;
        _player.SetLookTransform(null);
        await _lightsController.SetGlobalIntensity(3f, 2f);
        _lightsController.DisableShadows();
    }

    private async UniTask PlaySegment(DanceSegment segment)
    {
        float duration = 0f;
        List<List<IDanceMovementHandler>> handlers = new();
        for (var i = 0; i < _dancers.Length; i++)
        {
            var dancerHandlers = new List<IDanceMovementHandler>();
            foreach (var segmentMovement in segment.Movements)
            {
                var handler = segmentMovement.GetHandler(this, _dancers[i]);
                dancerHandlers.Add(handler);
                handler.OnStartSegment();
            }
            
            handlers.Add(dancerHandlers);
        }

        while (duration < segment.Duration)
        {
            if (IsGameOver)
            {
                return;
            }
            
            var normalizedTime = duration / segment.Duration;
            var deltaTime = Time.deltaTime;
            foreach (var dancerHandlers in handlers)
            {
                foreach (var handler in dancerHandlers)
                {
                    handler.HandleDancerPosition(deltaTime, normalizedTime);
                }
            }

            duration += Time.deltaTime;

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        
        foreach (var dancerHandlers in handlers)
        {
            foreach (var handler in dancerHandlers)
            {
                handler.OnEndSegment();
            }
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
        
        _dancers = new Dancer[_dancersCount];
        float angle = 0f;
        float deltaAngle = 2 * Mathf.PI / _dancersCount;
        for (int i = 0; i < _dancersCount; i++)
        {
            var dancer = Instantiate(_dancerPrefab);
            dancer.SetIndex(i);
            dancer.transform.position = Utils.GetPointOnCircle(Center, _initialRadius, angle);
            if (i != _playerPosition)
            {
                dancer.transform.position += new Vector3(UnityEngine.Random.Range(-_positionError, _positionError),
                    UnityEngine.Random.Range(-_positionError, _positionError), 0f);
            }

            dancer.SetLookTransform(transform.position);
            _dancers[i] = dancer;
            angle += deltaAngle;

            if (i == _playerPosition)
            {
                dancer.gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}

[Serializable]
public class DanceSegment
{
    public float Duration;
    public List<DanceMovement> Movements;
}
