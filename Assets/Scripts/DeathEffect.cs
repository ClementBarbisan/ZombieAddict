using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class DeathEffect : MonoBehaviour
{
    [Header("Target")]
    public Image targetImage;

    [Header("Jump")]
    public float jumpHeight = 40f;
    public float jumpDuration = 0.3f;

    [Header("Color")]
    public Color deathColor = Color.black;
    public float colorDuration = 0.15f;
    [Range(0,1)] public float tintStrength = 0.5f; // how strong the yellow is

    private Sequence _deathSequence;
    private RectTransform rect;

    void Awake()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        rect = targetImage.rectTransform;
    }

    public void PlayDeath()
    {
        _deathSequence?.Kill(true);

        Vector2 originalPos = rect.anchoredPosition;
        Color originalColor = targetImage.color;

        // Blend instead of full override
        Color tintedColor = Color.Lerp(originalColor, deathColor, tintStrength);

        _deathSequence = DOTween.Sequence();

        // 🔼 Jump up
        _deathSequence.Append(
            rect.DOAnchorPosY(originalPos.y + jumpHeight, jumpDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );


        // 🎨 Tint → back
        _deathSequence.Join(
            targetImage
                .DOColor(tintedColor, colorDuration)
                //.SetLoops(2, LoopType.Yoyo)
        );

        // scale
        //_deathSequence.Append(
        //    rect.Dos)

        _deathSequence.OnComplete(() =>
        {
            rect.anchoredPosition = originalPos;
            targetImage.color = originalColor;
        });

        _deathSequence.Play();
    }

    public void OnDeath() => PlayDeath();

    void OnDestroy() => _deathSequence?.Kill();
}