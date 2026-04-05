using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DeathEffect : MonoBehaviour
{
    [Header("Frame Target")]
    public Image targetImage;
    [Header("Photo Target")]
    public Image photoImage;

    [Header("Jump")]
    public float jumpHeight = 40f;
    public float jumpDuration = 0.3f;

    [Header("Color")]
    public Color deathColor = Color.black;
    public float colorDuration = 0.15f;
    [Range(0,1)] public float tintStrength = 0.5f;

    [Header("Rotation")]
    public float rotateDegrees = 180f;
    public float rotateDuration = 0.35f;
    public Ease rotateEase = Ease.OutQuad;

    [Header("Squish")]
    public Vector3 squishScale = new Vector3(1.4f, 0.6f, 1f);
    public float squishDuration = 0.15f;
    public Ease squishEase = Ease.OutQuad;

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

        Vector2 originalPos    = rect.anchoredPosition;
        Vector3 originalScale  = rect.localScale;
        float   originalRot    = rect.localEulerAngles.z;
        Color   originalColor  = photoImage.color;
        Color   tintedColor    = Color.Lerp(originalColor, deathColor, tintStrength);

        _deathSequence = DOTween.Sequence();

        // Jump up → back
        _deathSequence.Append(
            rect.DOAnchorPosY(originalPos.y + jumpHeight, jumpDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
        );

        // Tint
        _deathSequence.Join(
            targetImage.DOColor(tintedColor, colorDuration)
        );

        // Squish → back (yoyo)
        _deathSequence.Join(
            rect.DOScale(squishScale, squishDuration)
                .SetEase(squishEase)
                .SetLoops(2, LoopType.Yoyo)
        );

        // Rotate 180°
        _deathSequence.Join(
            rect.DOLocalRotate(new Vector3(0f, 0f, originalRot + rotateDegrees), rotateDuration)
                .SetEase(rotateEase)
        );

        _deathSequence.OnComplete(() =>
        {
            rect.anchoredPosition  = originalPos;
            rect.localScale        = originalScale;
            rect.localEulerAngles  = new Vector3(0f, 0f, originalRot);
            //targetImage.color      = originalColor;
        });

        _deathSequence.Play();
    }

    public void OnDeath() => PlayDeath();

    void OnDestroy() => _deathSequence?.Kill();
}