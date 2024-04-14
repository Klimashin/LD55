using Cysharp.Threading.Tasks;
using UnityEngine;

[ExecuteInEditMode]
public class DragHand : MonoBehaviour
{
    [SerializeField] private LineRenderer _armLineRenderer;
    [SerializeField] private Transform _hand;
    [SerializeField] private float _wonderTime;
    [SerializeField] private float _appearanceDuration;
    [SerializeField] private AnimationCurve _appearanceCurve;
    [SerializeField] private float _dragDuration;

    public async UniTask Drag(Vector3 startPos, GameObject target)
    {
        var time = 0f;
        _hand.position = startPos;
        _armLineRenderer.SetPosition(0, startPos);
        Vector3 targetPos = target.transform.position;
        Vector3 targetPosNearPoint = targetPos + (startPos - targetPos).normalized;
        Vector3 rightNearPoint = targetPosNearPoint + Vector3.Cross(startPos - targetPos, Vector3.forward).normalized;
        Vector3 leftNearPoint = targetPosNearPoint + Vector3.Cross(startPos - targetPos, Vector3.back).normalized;

        while (time <= _appearanceDuration)
        {
            float normalizedTime = time / _appearanceDuration;
            _hand.position = Vector3.Lerp(startPos, targetPosNearPoint, _appearanceCurve.Evaluate(normalizedTime));
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }

        float wonderTimeStep = _wonderTime / 4;
        time = 0f;
        while (time <= wonderTimeStep)
        {
            _hand.position = Vector3.Lerp(targetPosNearPoint, rightNearPoint, time / wonderTimeStep);
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        
        time = 0f;
        while (time <= 2 * wonderTimeStep)
        {
            _hand.position = Vector3.Lerp(rightNearPoint, leftNearPoint, time / 2 * wonderTimeStep);
            time += Time.deltaTime;
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
        }
        
        time = 0f;
        while (time <= wonderTimeStep)
        {
            _hand.position = Vector3.Lerp(leftNearPoint, targetPos, time / wonderTimeStep);
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
    }
}
