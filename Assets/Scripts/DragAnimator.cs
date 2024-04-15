using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

public class DragAnimator : MonoBehaviour
{
    [SerializeField] private DragHand[] _dragHands;
    [SerializeField] private float _startPointOffset = 30f;
    
    public async UniTask AnimateDrag(Vector3 center, List<GameObject> obj)
    {
        Assert.IsTrue(obj.Count <= _dragHands.Length);
        List<UniTask> tasks = new();
        for (var i = 0; i < obj.Count; i++)
        {
            var startPosition = (obj[i].transform.position - center).normalized * _startPointOffset;
            _dragHands[i].gameObject.SetActive(true);
            tasks.Add(_dragHands[i].Drag(startPosition, obj[i], i != 0));
        }

        await UniTask.WhenAll(tasks);

        foreach (var dragHand in _dragHands)
        {
            dragHand.gameObject.SetActive(false);
        }
    }
}
