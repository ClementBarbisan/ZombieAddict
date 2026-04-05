using UnityEngine;
using UnityEngine.InputSystem;

public class HitEffectTester : MonoBehaviour
{
    public DeathEffect deathEffect;

    void Update()
    {
        if (Mouse.current == null || deathEffect == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            deathEffect.OnDeath();
    }
}