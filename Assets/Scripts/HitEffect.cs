using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HitEffect : MonoBehaviour
{
    [Header("Target")]
    public Image targetImage;

    [Header("Scale")]
    public float scaleUpAmount = 1.3f;
    public float scaleDuration = 0.15f;

    [Header("Shake")]
    public float shakeStrength = 18f;
    public int shakeVibrato = 20;
    public float shakeDuration = 0.3f;

    [Header("Color")]
    public Color hitColor = Color.red;
    public float colorDuration = 0.15f;

    private Sequence _hitSequence;

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
    }

    public void PlayHit()
    {
        // Kill any in-progress hit tween before starting a new one
        _hitSequence?.Kill(complete: true);

        Vector3 originalScale = targetImage.rectTransform.localScale;
        Color originalColor   = targetImage.color;

        _hitSequence = DOTween.Sequence();

        // Scale up → back (yoyo)
        _hitSequence.Append(
            targetImage.rectTransform
                .DOScale(originalScale * scaleUpAmount, scaleDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );

        // Shake (runs in parallel with scale)
        _hitSequence.Join(
            targetImage.rectTransform
                .DOShakeAnchorPos(shakeDuration, shakeStrength, shakeVibrato)
                .SetEase(Ease.OutQuad)
        );

        // Colorize to red → back (yoyo)
        _hitSequence.Join(
            targetImage
                .DOColor(hitColor, colorDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );

        // Guarantee original state is restored on complete or kill
        _hitSequence.OnComplete(() =>
        {
            targetImage.rectTransform.localScale = originalScale;
            targetImage.color = originalColor;
        });

        _hitSequence.Play();
    }

    // Call this from your damage / combat system
    public void OnHit(float damages, Slider health)
    { 
        PlayHit();
        health.value -= damages;
    } 

    void OnDestroy() => _hitSequence?.Kill();
}