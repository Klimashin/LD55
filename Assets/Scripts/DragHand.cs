using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;

[ExecuteInEditMode]
public class DragHand : MonoBehaviour
{
    [SerializeField] private LineRenderer _armLineRenderer;
    [SerializeField] private Transform _hand;
    [SerializeField] private float _wonderTime;
    [SerializeField] private float _grabTime = 0.5f;
    [SerializeField] private float _grabPrepTime = 0.5f;
    [SerializeField] private float _grabPrepOffset = 3f;
    [SerializeField] private float _wonderPointOffset = 2f;
    [SerializeField] private float _wonderRange = 0.5f;
    [SerializeField] private float _grabPointOffset = 1f;
    [SerializeField] private int _wonderCount = 3;
    [SerializeField] private float _appearanceDuration;
    [SerializeField] private AnimationCurve _appearanceCurve;
    [SerializeField] private float _dragDuration;
    [SerializeField] private AudioClip[] _sfx;

    private SoundSystem _soundSystem;

    [Inject]
    private void Inject(SoundSystem soundSystem)
    {
        _soundSystem = soundSystem;
    }

    public async UniTask Drag(Vector3 startPos, GameObject target, bool skipSfx = true)
    {
        var time = 0f;
        _hand.position = startPos;
        _armLineRenderer.SetPosition(0, startPos);
        Vector3 targetPos = target.transform.position;
        Vector3 wonderPoint = targetPos + (startPos - targetPos).normalized * _wonderPointOffset;
        Vector3 grabPrepPoint = targetPos + (startPos - targetPos).normalized * _grabPrepOffset;
        Vector3 grabPoint = targetPos + (startPos - targetPos).normalized * _grabPointOffset;

        while (time <= _appearanceDuration)
        {
            float normalizedTime = time / _appearanceDuration;
            _hand.position = Vector3.Lerp(startPos, wonderPoint, _appearanceCurve.Evaluate(normalizedTime));
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }

        for (int i = 0; i < _wonderCount; i++)
        {
            time = 0f;
            Vector3 wonderEndPoint = Utils.GetPointOnCircle(wonderPoint, _wonderRange, Random.Range(0f, 2*Mathf.PI));
            Vector3 wonderStartPoint = _hand.position;
            while (time <= _wonderTime)
            {
                _hand.position = Vector3.Lerp(wonderStartPoint, wonderEndPoint, time / _wonderTime);
                time += Time.deltaTime;
                await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
            }
        }

        if (!skipSfx)
        {
            _soundSystem.PlayOneShot(_sfx[Random.Range(0, _sfx.Length)]);
        }
        time = 0f;
        Vector3 grabPrepStartPosition = _hand.position;
        while (time <= _grabTime)
        {
            _hand.position = Vector3.Lerp(grabPrepStartPosition, grabPrepPoint, time / _grabPrepTime);
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        
        time = 0f;
        Vector3 grabStartPosition = _hand.position;
        while (time <= _grabTime)
        {
            _hand.position = Vector3.Lerp(grabStartPosition, grabPoint, time / _grabTime);
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }

        var col = target.GetComponentInChildren<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        time = 0f;
        var dragFromPos = _hand.position;
        while (time <= _dragDuration)
        {
            _hand.position = Vector3.Lerp(dragFromPos, startPos, time / _dragDuration);
            target.transform.position = _hand.position;
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        
        target.SetActive(false);
    }
    
    private void Update()
    {
        _armLineRenderer.SetPosition(1, _hand.position);
        var lookDirection = Quaternion.AngleAxis(90, Vector3.forward) * (_armLineRenderer.GetPosition(0) - _hand.transform.position).normalized;
        _hand.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDirection);
    }
}
