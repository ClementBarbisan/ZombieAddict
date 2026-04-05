
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Settings")] 
    public float speed = 3.5f;
    [SerializeField] private float attackRange = 3.8f;
    [SerializeField] private bool animateHit = true;

    [Header("References")]
    public Transform target;
    [SerializeField] private Renderer renderer;
    [SerializeField] private ParticleSystem vfxAttack;
    
    [Header("Stats")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    private bool _isDead = false;
    
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 15f;

    [Header("Audios")] 
    [SerializeField] private AudioClip[] clipsHit;
    [SerializeField] private AudioClip[] clipsAttack;
    [SerializeField] private AudioClip clipDeath;
    
    [Header("Events")]
    public UnityEvent<float> OnHit;       
    public UnityEvent<EnemyController> OnDeath;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int Move = Animator.StringToHash("Move");
    private Material _mat;
    private float  _sqrDetectionRange;
    private float _cooldownAttackTimer;
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private float _initSpeed;
    

    private void OnEnable()
    {
        EnemiesTracker.Instance.Register(transform);
    }
    private void OnDisable()
    {
        EnemiesTracker.Instance.Unregister(transform);
    }
    private void Start()
    {
        _sqrDetectionRange = _detectionRange * _detectionRange;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _mat = renderer.material;
        _agent.speed = speed;
        _currentHealth = _maxHealth;
    }
    void Update()
    {
        if (_agent.enabled)
        {
            if(target != null)
                _agent.SetDestination(target.position);
            else
                _agent.SetDestination(Vector3.zero);
        }
        
        HandleAnimator();
        
        target = GetClosestPlayer(out float sqrDist);
        if (target != null && sqrDist < attackRange)
        {
            Attack();
            _agent.enabled = false;
        }
        else
        {
            _cooldownAttackTimer = 0f;
            _agent.enabled = true;
        }
    }
    private void HandleAnimator()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > 0.01f;
        _animator.SetBool(Move, isMoving);
    }
    public void TakeDamage(float amount, PlayerController player)
    {
        _mat.EnableKeyword("_EMISSION");
        Invoke(nameof(ResetMaterial), .05f);
        
        if (_isDead) return;
        
        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0f, _maxHealth);

        OnHit?.Invoke(_currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
            player.KillEnemy();
        }
        else if (animateHit)
        {
            _agent.enabled = false;
        
            Invoke(nameof(ResetMove), .1f);
            _animator.Play("BAKED_Hit");
        }
        if(clipsHit.Length > 0)
            AudioSource.PlayClipAtPoint(clipsHit[Random.Range(0, clipsHit.Length)], transform.position);
    }
    private void ResetMove()
    {
        _agent.enabled = true;
    }
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        if(WebsocketManager.Instance != null)
            WebsocketManager.Instance.zombiePlayerInfos.nbZombieDead++;
        OnDeath?.Invoke(this);
        _agent.enabled = false;
        _animator.Play("BAKED_Death");
        if(clipDeath != null)
            AudioSource.PlayClipAtPoint(clipDeath, transform.position);
        Destroy(gameObject, 1.2f);
    }
    public float GetHealthPercent() => _currentHealth / _maxHealth;
    public bool IsDead() => _isDead;
    private void ResetMaterial()
    {
        _mat.DisableKeyword("_EMISSION");
    }
    private void Attack()
    {
        if (target != null)
        {
            _cooldownAttackTimer -= Time.deltaTime;
            if (_cooldownAttackTimer < 0f)
            {
                _cooldownAttackTimer = 1.1f;
                target.GetComponent<IDamageable>().TakeDamage(1f, null);
                _animator.SetTrigger(Shoot);
                vfxAttack.Play();
                if(clipsAttack.Length > 0)
                    AudioSource.PlayClipAtPoint(clipsAttack[Random.Range(0, clipsAttack.Length)], transform.position);
            }
        }
    }
    private Transform GetClosestPlayer(out float sqrDistToClosest)
    {
        var players = PlayerTracker.Instance.Players;

        Transform closest      = null;
        float closestSqrDist   = _sqrDetectionRange;
        sqrDistToClosest       = float.MaxValue; 

        foreach (var player in players)
        {
            if (player == null) continue;

            float sqrDist = (transform.position - player.position).sqrMagnitude;

            if (sqrDist < closestSqrDist)
            {
                closestSqrDist   = sqrDist;
                closest          = player;
                sqrDistToClosest = sqrDist;
            }
        }
        return closest;
    }
}




