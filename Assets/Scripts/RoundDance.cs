using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RoundDance : MonoBehaviour
{
    [SerializeField] private List<Dancer> _dancerPrefabs;
    [SerializeField] private int _dancersCount;
    [SerializeField] private int _playerPosition;
    [SerializeField] private List<DanceSegment> _danceSegments;
    [SerializeField] private float _initialRadius;
    [SerializeField] private float _initialErrorDelta = 0.5f;
    [SerializeField] private float _errorDelta = 0.3f;
    [SerializeField] private float _errorUpDistanceCoefficient = 3f;
    [SerializeField] private float _errorUpRate = 2f;
    [SerializeField] private float _errorDownRate = 1f;
    [SerializeField] private float _maxErrorRate = 1000f;
    [SerializeField] private float _positionError = 0.1f;
    [SerializeField] private int _maxDraggedDancers = 3;
    [SerializeField] private float _noErrorCooldownAfterDrag = 5f;
    [SerializeField] private float _freeMovementAfterDrag = 2f;
    [SerializeField] private float _sunDawnTime = 7f;
    [SerializeField] private AudioClip _ambientAudio;
    [SerializeField] private AudioClip _morningAudio;

    private SoundSystem _soundSystem;
    private CameraController _cameraController;
    private LightsController _lightsController;
    private EyesController _eyesController;
    private DragAnimator _dragAnimator;
    private GameplaySoundSystem _gameplaySoundSystem;
    private Character _player;
    private Dancer[] _dancers;
    private Vector3 PlayerExpectedPosition => _dancers[_playerPosition].transform.position;
    private DanceState State { get; set; } = DanceState.WaitingForPlayer;
    private int DraggedDancers { get; set; } = 0;
    private bool IsGameOver => DraggedDancers > _maxDraggedDancers;
    public Vector3 Center => transform.position;
    public float ErrorRate { get; private set; } = 0f;
    public float ErrorCooldown { get; private set; } = 0f;
    public float ErrorRateNormalized => ErrorRate / _maxErrorRate;
    public bool IsErrorRateOverCap => ErrorRate >= _maxErrorRate;

    [Inject]
    private void Inject(
        SoundSystem soundSystem,
        CameraController cameraController,
        Character player,
        LightsController lightsController,
        EyesController eyesController,
        GameplaySoundSystem gameplaySoundSystem,
        DragAnimator dragAnimator)
    {
        _soundSystem = soundSystem;
        _gameplaySoundSystem = gameplaySoundSystem;
        _cameraController = cameraController;
        _lightsController = lightsController;
        _eyesController = eyesController;
        _dragAnimator = dragAnimator;
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
        if (!_soundSystem.IsPlayingClip(_ambientAudio))
        {
            _soundSystem.PlayMusicClip(_ambientAudio, 3f);
        }
    }

    [Button]
    private void TestDrag(int count = 1)
    {
        var maxTargets = _dancers.Count(d => d.gameObject.activeSelf);
        Assert.IsTrue(count <= maxTargets);
        List<GameObject> targets = new List<GameObject>();
        while (targets.Count < count)
        {
            var dancer = _dancers[Random.Range(0, _dancers.Length)];
            if (!dancer.gameObject.activeSelf || targets.Contains(dancer.gameObject))
            {
                continue;
            }
            
            targets.Add(dancer.gameObject);
        }
        
        _dragAnimator.AnimateDrag(Center, targets).Forget();
    }

    private void Update()
    {
        if (State == DanceState.WaitingForPlayer)
        {
            if (IsPlayerInPlace())
            {
                DanceRoutine().Forget();
            }
        }
    }

    private bool IsPlayerInPlace()
    {
        if (State == DanceState.WaitingForPlayer)
        {
            return Vector3.Distance(_player.transform.position, PlayerExpectedPosition) < _initialErrorDelta;
        }

        return Vector3.Distance(_player.transform.position, PlayerExpectedPosition) < _errorDelta;
    }

    private void HandlePlayerErrors()
    {
        _eyesController.OnErrorRateUpdated(ErrorRateNormalized);
        
        if (ErrorCooldown > 0f)
        {
            ErrorCooldown -= Time.deltaTime;
            return;
        }
        
        if (IsPlayerInPlace())
        {
            ErrorRate -= _errorDownRate * Time.deltaTime;
            if (ErrorRate < 0f)
            {
                ErrorRate = 0f;
            }
        }
        else
        {
            var distance = Vector3.Distance(_player.transform.position, PlayerExpectedPosition);
            var error = (_errorUpRate + distance * _errorUpDistanceCoefficient) * Time.deltaTime;
            ErrorRate += error;
        }
    }

    private async UniTask DanceRoutine()
    {
        _cameraController.SetCamera(CameraController.CameraType.Dance);
        _soundSystem.FadeCurrentMusic(3f);
        _lightsController.FadeAdditionalLights(3f);
        _player.SetLookTransform(transform);
        State = DanceState.Started;

        _player.enabled = false;
        
        await UniTask.Delay(TimeSpan.FromSeconds(3f), DelayType.DeltaTime, PlayerLoopTiming.Update,
            destroyCancellationToken);
        
        _player.enabled = true;
        
        await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.DeltaTime, PlayerLoopTiming.Update,
            destroyCancellationToken);

        var segmentsQueue = new Queue<DanceSegment>(_danceSegments);
        while (segmentsQueue.Count > 0)
        {
            var segment = segmentsQueue.Dequeue();
            _gameplaySoundSystem.SetLoopTracks(0, segment.GetTracks());
            await PlaySegment(segment);

            if (IsGameOver)
            {
                _gameplaySoundSystem.SetLoopTracks(0, new int[]{7});
                GameOverRoutine().Forget();
                return;
            }
        }

        GameCompletedRoutine().Forget();
    }

    private async UniTaskVoid GameOverRoutine()
    {
        State = DanceState.GameOver;
        _player.enabled = false;
        List<GameObject> dragTarget = _dancers.Where(d => d.gameObject.activeSelf).Select(d => d.gameObject).ToList();
        dragTarget.Add(_player.gameObject);

        _lightsController.LightsOff(10f).Forget();
        await _dragAnimator.AnimateDrag(Center, dragTarget);
        
        await UniTask.Delay(TimeSpan.FromSeconds(3f), DelayType.DeltaTime, PlayerLoopTiming.Update,
            destroyCancellationToken);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private async UniTaskVoid GameCompletedRoutine()
    {
        _gameplaySoundSystem.SetLoopTracks(0, new int[]{});
        _soundSystem.PlayMusicClip(_morningAudio, _sunDawnTime / 2f);
        State = DanceState.Finished;
        ErrorRate = 0f;
        _eyesController.OnErrorRateUpdated(ErrorRate);
        _player.SetLookTransform(null);
        await _lightsController.SetGlobalIntensity(6f, _sunDawnTime);
        _lightsController.DisableShadows();
        
        await UniTask.Delay(TimeSpan.FromSeconds(1f), DelayType.DeltaTime, PlayerLoopTiming.Update,
            destroyCancellationToken);

        SceneManager.LoadScene(2);
    }

    private async UniTask PlaySegment(DanceSegment segment)
    {
        float duration = 0f;
        Dictionary<Dancer, List<IDanceMovementHandler>> handlers = new();
        for (var i = 0; i < _dancers.Length; i++)
        {
            _dancers[i].SetHandsPos(segment.HandsPos);
            _player.SetHandsPos(segment.HandsPos);
            var dancerHandlers = new List<IDanceMovementHandler>();
            foreach (var segmentMovement in segment.Movements)
            {
                var handler = segmentMovement.GetHandler(this, _dancers[i]);
                dancerHandlers.Add(handler);
                handler.OnStartSegment();
            }
            
            handlers.Add(_dancers[i], dancerHandlers);
        }

        while (duration < segment.Duration)
        {
            if (IsErrorRateOverCap)
            {
                DraggedDancers += 1;

                if (IsGameOver)
                {
                    return;
                }
                
                await DragDancersAway(segment, 1);
            }

            HandlePlayerErrors();

            var normalizedTime = duration / segment.Duration;
            var deltaTime = Time.deltaTime;
            foreach (var (dancer , dancerHandlers) in handlers)
            {
                foreach (var handler in dancerHandlers)
                {
                    dancer.transform.position = handler.HandleDancerPosition(deltaTime, normalizedTime);
                }
            }

            duration += Time.deltaTime;

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        
        foreach (var dancerHandlers in handlers.Values)
        {
            foreach (var handler in dancerHandlers)
            {
                handler.OnEndSegment();
            }
        }
    }

    private async UniTask DragDancersAway(DanceSegment segment, int count)
    {
        _player.enabled = false;
        _gameplaySoundSystem.SetLoopTracks(0, new int[]{7});
        var maxTargets = _dancers.Count(d => d.gameObject.activeSelf);
        Assert.IsTrue(count <= maxTargets);
        
        ErrorCooldown = _noErrorCooldownAfterDrag;
        
        List<GameObject> targets = new List<GameObject>();
        while (targets.Count < count)
        {
            var dancer = _dancers[Random.Range(0, _dancers.Length)];
            if (!dancer.gameObject.activeSelf || targets.Contains(dancer.gameObject))
            {
                continue;
            }
            
            targets.Add(dancer.gameObject);
        }
        
        await _dragAnimator.AnimateDrag(Center, targets);
        
        _gameplaySoundSystem.SetLoopTracks(0, segment.GetTracks());
        
        ErrorRate = 0f;
        _eyesController.OnErrorRateUpdated(ErrorRate);

        _player.enabled = true;
        
        await UniTask.Delay(TimeSpan.FromSeconds(_freeMovementAfterDrag), DelayType.DeltaTime, PlayerLoopTiming.Update,
            destroyCancellationToken);
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
            var prefab = _dancerPrefabs[Random.Range(0, _dancerPrefabs.Count)];
            var dancer = Instantiate(prefab);
            dancer.SetIndex(i);
            dancer.transform.position = Utils.GetPointOnCircle(Center, _initialRadius, angle);
            if (i != _playerPosition)
            {
                dancer.transform.position += new Vector3(Random.Range(-_positionError, _positionError), Random.Range(-_positionError, _positionError), 0f);
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
    public string Name;
    public float Duration;
    [Title("Active Tracks")]
    [EnumToggleButtons] public TrackNumFlags Tracks;
    public Dancer.HandsPos HandsPos = Dancer.HandsPos.Hidden;
    public List<DanceMovement> Movements;

    public int[] GetTracks()
    {
        var flags = new List<int>();
        if (Tracks.HasFlag(TrackNumFlags.Track1))
            flags.Add(0);

        if (Tracks.HasFlag(TrackNumFlags.Track2))
            flags.Add(1);
        
        if (Tracks.HasFlag(TrackNumFlags.Track3))
            flags.Add(2);
        
        if (Tracks.HasFlag(TrackNumFlags.Track4))
            flags.Add(3);
        
        if (Tracks.HasFlag(TrackNumFlags.Track5))
            flags.Add(4);
        
        if (Tracks.HasFlag(TrackNumFlags.Track6))
            flags.Add(5);
        
        if (Tracks.HasFlag(TrackNumFlags.Track7))
            flags.Add(6);
        
        return flags.ToArray();
    }
    
    [System.Flags]
    public enum TrackNumFlags
    {
        Track1 = 1 << 0,
        Track2 = 1 << 1,
        Track3 = 1 << 2,
        Track4 = 1 << 3,
        Track5 = 1 << 4,
        Track6 = 1 << 5,
        Track7 = 1 << 6,
    }
}
