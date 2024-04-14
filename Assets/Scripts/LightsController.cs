using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightsController : MonoBehaviour
{
    [SerializeField] private Light2D _globalLight;
    [SerializeField] private List<Light2D> _allLights;

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

    public void DisableShadows()
    {
        foreach (var light2D in _allLights)
        {
            light2D.shadowsEnabled = false;
        }
    }

    public async UniTask LightsOff(float time)
    {
        float currentTime = 0f;
        float[] initialIntensity = _allLights.Select(l => l.intensity).ToArray();
        foreach (var light2D in _allLights)
        {
            var animator = light2D.GetComponentInParent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
        }

        while (currentTime <= time)
        {
            for (int i = 0; i < _allLights.Count; i++)
            {
                _allLights[i].intensity = Mathf.Lerp(initialIntensity[i], 0f, currentTime / time);
            }

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, destroyCancellationToken);
            currentTime += Time.deltaTime;
        }
    }
}
