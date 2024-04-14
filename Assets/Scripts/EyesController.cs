using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

public class EyesController : MonoBehaviour
{
    [SerializeField] private GameObject[] _eyePrefabs;
    [SerializeField] private int _eyePerIntensityLevel;
    [SerializeField] private BoxCollider2D[] _spawnZones;
    [SerializeField] private int _intensityLevels;

    private ObjectPool<GameObject> _eyesPool;
    private int _currentIntensity = 0;
    private readonly List<GameObject> _eyes = new ();

    public void OnErrorRateUpdated(float rateNormalized)
    {
        var intensity = NormalizedRateToIntensityLevel(rateNormalized);
        ApplyIntensity(intensity);
    }

    private int NormalizedRateToIntensityLevel(float rateNormalized)
    {
        var scaledRate = rateNormalized * 10000;
        var delta = 10000 / _intensityLevels;
        return Mathf.RoundToInt(scaledRate / delta);
    }

    private Vector3 GetSpawnPoint()
    {
        return Utils.RandomPointInBounds(_spawnZones[Random.Range(0, _spawnZones.Length)].bounds);
    }

    private void ApplyIntensity(int intensity)
    {
        if (_currentIntensity == intensity)
        {
            return;
        }

        _currentIntensity = intensity;
        var targetEyesCount = _currentIntensity * _eyePerIntensityLevel;
        if (targetEyesCount < _eyes.Count)
        {
            int count = _eyes.Count;
            while (count > targetEyesCount && count > 0)
            {
                _eyesPool.Release(_eyes[count - 1]);
                _eyes.RemoveAt(count - 1);
                count--;
            }
        }
        else if (targetEyesCount > _eyes.Count)
        {
            int count = targetEyesCount - _eyes.Count;
            for (int i = 0; i < count; i++)
            {
                _eyes.Add(_eyesPool.Get());
            }
        }
    }

    private void Start()
    {
        _eyesPool = new ObjectPool<GameObject>(CreateEye, GetEye, ReleaseEye);
    }

    private GameObject CreateEye()
    {
        var prefab = _eyePrefabs[Random.Range(0, _eyePrefabs.Length)];
        prefab.transform.position = GetSpawnPoint();
        return Instantiate(prefab, transform);
    }

    private void GetEye(GameObject obj)
    {
        obj.gameObject.SetActive(true);
        obj.transform.position = GetSpawnPoint();
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(Vector3.one, 1f);
    }

    private void ReleaseEye(GameObject obj)
    {
        obj.gameObject.SetActive(false);
    }
}
