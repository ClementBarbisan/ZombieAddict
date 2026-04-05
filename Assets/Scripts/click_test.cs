using UnityEngine;
using UnityEngine.InputSystem;

public class HitEffectTester : MonoBehaviour
{
    public KillEffect killEffect;

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            killEffect.OnKill();
    }
}