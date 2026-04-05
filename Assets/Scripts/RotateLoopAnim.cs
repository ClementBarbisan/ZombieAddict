using DG.Tweening;
using UnityEngine;

public class RotateLoop : MonoBehaviour
{
    public float duration = 3f;
    public Ease ease = Ease.Linear;

    Tween _tween;

    void OnEnable()
    {
        _tween = transform
            .DORotate(new Vector3(0f, 359f, 0f), duration, RotateMode.FastBeyond360)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Restart);
    }

    void OnDisable() => _tween?.Kill();
}