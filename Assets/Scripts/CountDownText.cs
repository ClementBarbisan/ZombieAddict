using UnityEngine;
using System.Collections;
using TMPro;

public class CountDownText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private int   _startValue    = 5;
    [SerializeField] private float _interval      = 1f;
    [SerializeField] private float _animDuration  = 0.35f;

    private void Start() => StartCoroutine(CountdownRoutine());

    private IEnumerator CountdownRoutine()
    {
        for (int i = _startValue; i > 0; i--)
        {
            _countdownText.text = i.ToString();
            yield return StartCoroutine(PopAnimation());
            yield return new WaitForSeconds(_interval - _animDuration);
        }

        _countdownText.text = "GO!";
        yield return StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        float elapsed = 0f;
        Vector3 baseScale = Vector3.one;

        while (elapsed < _animDuration)
        {
            float t = elapsed / _animDuration;

            // Scale: 1.4 → 1 avec easing out
            float scale = Mathf.Lerp(1.4f, 1f, EaseOutBack(t));
            _countdownText.transform.localScale = baseScale * scale;

            // Alpha: 0 → 1
            _countdownText.alpha = Mathf.Lerp(0f, 1f, t * 3f); // fade rapide

            elapsed += Time.deltaTime;
            yield return null;
        }

        _countdownText.transform.localScale = baseScale;
        _countdownText.alpha = 1f;
    }

    private float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1 + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
