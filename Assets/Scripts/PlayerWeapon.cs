using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Settings")] 
    public float coolDownFire = .2f;
    [SerializeField] private Transform weaponPos;
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private LineRenderer hitLine;
    
    private float _cooldownTimer;
    private bool _isFiring;
    private Material _hitLineMat;
    private Color _hitLineColor;
    private float _alphaLineHit;

    private void Start()
    {
        _hitLineMat = hitLine.material;
    }

    public void HandleFire(bool isPressed)
    {
        Debug.Log("isPress"+ isPressed);
        _isFiring = isPressed;
    }

    private void Update()
    {
        _cooldownTimer -= Time.deltaTime;

        if (_isFiring && _cooldownTimer <= 0f)
        {
            Debug.Log("FIRE");
            Shoot();
            _cooldownTimer = coolDownFire;
        }
        
        if(_alphaLineHit > 0f)
        {
            _alphaLineHit -= Time.deltaTime * 5f;
            if (_alphaLineHit < 0f)
                _alphaLineHit = 0f;
            
            _hitLineMat.color = new Color(_hitLineColor.r, _hitLineColor.g, _hitLineColor.b, _alphaLineHit);
        }
    }

    private void Shoot()
    {
        if (Physics.Raycast(weaponPos.position, weaponPos.forward, out RaycastHit hit))
        {
            // VFX impact
            hitVFX.transform.position = hit.point;
            hitVFX.transform.forward = hit.normal;
            hitVFX.Emit(1);
            
            hitLine.SetPosition(0,weaponPos.position);
            hitLine.SetPosition(1,hit.point);
            _alphaLineHit = 1f;
        }
    }
}

