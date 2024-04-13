using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightsController : MonoBehaviour
{
    [SerializeField] private Light2D _globalLight;

    public async UniTask SetGlobalIntensity(float targetIntensity, float time)
    {
        var initialIntensity = _globalLight.intensity;
        float currentTime = 0f;
        while (currentTime <= time)
        {
            _globalLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, currentTime / time);
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
            currentTime += Time.deltaTime;
        }
    }
}
