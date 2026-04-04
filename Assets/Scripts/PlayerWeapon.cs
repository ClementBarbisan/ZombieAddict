
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
            _canShoot = true;
    }

    private void Update()
    {
        if (!_canShoot)
            return;
        
        _timerCooldown += Time.deltaTime;
        
        if (_timerCooldown > coolDownFire)
        {
            _timerCooldown = 0f;
            if (Physics.Raycast(weaponPos.position, weaponPos.forward, out RaycastHit hit))
            {
                _canShoot = false;
                hitVFX.transform.position = hit.point;
                hitVFX.Emit(1);
                
                Debug.Log("Fire");
                Debug.DrawRay(weaponPos.position, weaponPos.forward * hit.distance, Color.red);
            }
        }
    }
}
