
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Settings")] 
    public float coolDownFire = .2f;
    [SerializeField] private Transform weaponPos;
    [SerializeField] private ParticleSystem hitVFX;
    
    private float _timerCooldown;
    private bool _canShoot = true;
    public void HandleFire(bool valueInput)
    {
        Debug.Log(valueInput);
        if (valueInput && _canShoot)
            Fire();
    }

    private void Fire()
    {
        _timerCooldown += Time.deltaTime;
        
        if (_timerCooldown > coolDownFire)
        {
            _timerCooldown = 0f;
            if (Physics.Raycast(weaponPos.position, weaponPos.forward, out RaycastHit hit))
            {
                hitVFX.transform.position = hit.point;
                hitVFX.Play();
            }
        }
    }
}
