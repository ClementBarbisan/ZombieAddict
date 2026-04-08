using UnityEngine;
using UnityEngine.Events;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Settings")] 
    public Transform target;
    public float coolDownFire = .2f;
    public float hitDamage = 1f;
    [SerializeField] private Transform weaponPos;
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private LineRenderer hitLine;
    [SerializeField] private LayerMask layer;

    public UnityEvent<int> OnHitEnemy = new UnityEvent<int>();
    
    private float _cooldownTimer;
    private bool _isFiring;
    private Material _hitLineMat;
    private Color _hitLineColor;
    private float _alphaLineHit;
    private float  _sqrDetectionRange;
    private PlayerController _player;

    private void Start()
    {
        _hitLineMat = hitLine.material;
        _player = GetComponent<PlayerController>();
    }

    public void HandleFire(bool isPressed, WebsocketManager.InfosPlayer infos)
    {
        _isFiring = isPressed;
        if (isPressed)
            infos.shootFired++;
    }

    private void Update()
    {
        target = GetClosestEnemy(10f, out float sqrDist);
        _cooldownTimer -= Time.deltaTime;

        if (_isFiring && _cooldownTimer <= 0f)
        {
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
        if (Physics.Raycast(weaponPos.position, weaponPos.forward, out RaycastHit hit, layer))
        {
            // VFX impact
            hitVFX.transform.position = hit.point;
            hitVFX.transform.forward = hit.normal;
            hitVFX.Emit(1);
            
            hitLine.SetPosition(0,weaponPos.position);
            hitLine.SetPosition(1,hit.point);
            _alphaLineHit = 1f;

            if (hit.transform.TryGetComponent<IDamageable>(out var target))
            {
                hit.transform.GetComponent<IDamageable>().TakeDamage(hitDamage, _player);
                //OnHitEnemy?.Invoke((int)hitDamage);
            }
        }
    }
    
    private Transform GetClosestEnemy(float maxDistance, out float sqrDistToClosest)
    {
        float maxSqrDist    = maxDistance * maxDistance; 
        float closestSqrDist = maxSqrDist;               
        var   enemies        = EnemiesTracker.Instance.Enemies;
        sqrDistToClosest       = float.MaxValue; 

        Transform closest = null;

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            float sqrDist = (transform.position - enemy.position).sqrMagnitude;

            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest        = enemy;
                sqrDistToClosest = sqrDist;
            }
        }

        return closest; 
    }
}

