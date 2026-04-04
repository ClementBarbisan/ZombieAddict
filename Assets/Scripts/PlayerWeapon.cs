
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Settings")] 
    public float coolDownFire = .2f;
    [SerializeField] private Transform weaponPos;
    [SerializeField] private ParticleSystem hitVFX;
    
    private float _cooldownTimer;
    private bool _isFiring;

    public void HandleFire(bool isPressed)
    {
        _isFiring = isPressed;
    }

    private void Update()
    {
        _cooldownTimer -= Time.deltaTime;

        if (_isFiring && _cooldownTimer <= 0f)
        {
            Shoot();
            _cooldownTimer = coolDownFire;
        }
    }

    private void Shoot()
    {
        if (Physics.Raycast(weaponPos.position, weaponPos.forward, out RaycastHit hit))
        {
            // VFX impact
            if (hitVFX != null)
            {
                hitVFX.transform.position = hit.point;
                hitVFX.transform.forward = hit.normal;
                hitVFX.Emit(1);
            }

            Debug.Log("Fire 🔫");

            Debug.DrawRay(
                weaponPos.position,
                weaponPos.forward * hit.distance,
                Color.red,
                0.5f
            );
        }
    }
}

