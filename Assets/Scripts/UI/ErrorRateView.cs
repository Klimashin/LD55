using System.Globalization;
using TMPro;
using UnityEngine;

public class ErrorRateView : MonoBehaviour
{
    [SerializeField] private RoundDance _roundDance;
    [SerializeField] private RectTransform _errorRateBar;
    [SerializeField] private RectTransform _errorRateBarFill;
    [SerializeField] private TextMeshProUGUI _errorRateText;

    private void Update()
    {
        _errorRateText.text = Mathf.RoundToInt(_roundDance.ErrorRate).ToString(CultureInfo.InvariantCulture);
        var fillRight = Mathf.Max(0f, _errorRateBar.rect.width * (1f - _roundDance.ErrorRateNormalized));
        _errorRateBarFill.SetRight(fillRight);
    }
}
