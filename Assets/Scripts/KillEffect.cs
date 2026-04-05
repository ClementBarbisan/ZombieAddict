using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class KillEffect : MonoBehaviour
{
    [Header("Target")]
    public Image targetImage;

    [Header("Jump")]
    public float jumpHeight = 40f;
    public float jumpDuration = 0.3f;

    [Header("Color")]
    public Color hitColor = Color.yellow;
    public float colorDuration = 0.15f;
    [Range(0,1)] public float tintStrength = 0.5f; // how strong the yellow is

    private Sequence _killSequence;
    private RectTransform rect;

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        rect = targetImage.rectTransform;
    }

    public void PlayKill()
    {
        _killSequence?.Kill(true);

        Vector2 originalPos = rect.anchoredPosition;
        Color originalColor = targetImage.color;

        // Blend instead of full override
        Color tintedColor = Color.Lerp(originalColor, hitColor, tintStrength);

        _killSequence = DOTween.Sequence();

        // 🔼 Jump up
        _killSequence.Append(
            rect.DOAnchorPosY(originalPos.y + jumpHeight, jumpDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );


        // 🎨 Tint → back
        _killSequence.Join(
            targetImage
                .DOColor(tintedColor, colorDuration)
                .SetLoops(2, LoopType.Yoyo)
        );

        _killSequence.OnComplete(() =>
        {
            rect.anchoredPosition = originalPos;
            targetImage.color = originalColor;
        });

        _killSequence.Play();
    }

    public void OnKill() => PlayKill();

    void OnDestroy() => _killSequence?.Kill();
}