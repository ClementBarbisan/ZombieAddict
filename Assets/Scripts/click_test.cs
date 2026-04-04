using UnityEngine;
using UnityEngine.InputSystem;

public class HitEffectTester : MonoBehaviour
{
    public HitEffect hitEffect;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            hitEffect.OnHit();
    }
}