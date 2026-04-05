using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FloatAnim : MonoBehaviour
{
    [Header("Float")]
    public float amplitude = 20f;
    public float duration = 1.2f;
    public Ease ease = Ease.InOutSine;

    private RectTransform _rect;
    private Tween _tween;

    void Awake() => _rect = GetComponent<RectTransform>();

    void OnEnable()
    {
        _tween = _rect.DOAnchorPosY(_rect.anchoredPosition.y + amplitude, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable() => _tween?.Kill();
}