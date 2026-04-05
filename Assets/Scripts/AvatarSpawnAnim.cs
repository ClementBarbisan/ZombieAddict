using DG.Tweening;
using UnityEngine;

public class AvatarSpawnAnim : MonoBehaviour
{
    public float duration = 0.35f;
    public Ease ease = Ease.OutBack;

    void OnEnable()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * 1f, duration).SetEase(ease);
    }

    void OnDestroy() => DOTween.Kill(transform);
}