
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour, IDamageable
{
    [Header("Settings")] 
    public float speed = 3.5f;

    [Header("References")]
    public Transform target;
    [SerializeField] private Renderer renderer;
    
    [Header("Stats")]
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    private bool _isDead = false;
    
    [Header("Detection")]
    [SerializeField] private float _detectionRange = 15f;
    [SerializeField] private float _refreshRate    = 0.2f;

    [Header("Events")]
    public UnityEvent<float> OnHit;       
    public UnityEvent<EnemyController> OnDeath;
    
    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int Move = Animator.StringToHash("Move");
    private Material _mat;
    private float  _sqrDetectionRange;

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
        InvokeRepeating(nameof(UpdateTarget), 0f, _refreshRate);
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _mat = renderer.material;
        _agent.speed = speed;
        _currentHealth = _maxHealth;
    }

    void Update()
    {
        if(target != null)
            _agent.SetDestination(target.position);
        else
            _agent.SetDestination(Vector3.zero);

        HandleAnimator();
        target = GetClosestPlayer();
    }

    private void HandleAnimator()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > 0.01f;
        _animator.SetBool(Move, isMoving);
    }
    
    public void TakeDamage(float amount)
    {
        if (_isDead) return;
        
        _mat.EnableKeyword("_EMISSION");
        Invoke(nameof(ResetMaterial), .05f);

        _currentHealth = Mathf.Clamp(_currentHealth - amount, 0f, _maxHealth);

        OnHit?.Invoke(_currentHealth);

        if (_currentHealth <= 0f)
            Die();
        else
            _animator.Play("BAKED_Spider_Hit");
    }
    
    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        OnDeath?.Invoke(this);
        _agent.speed = 0f;
        _animator.Play("BAKED_Spider_Death");
        Destroy(gameObject, 1.2f);
    }

    public float GetHealthPercent() => _currentHealth / _maxHealth;
    public bool IsDead() => _isDead;

    private void ResetMaterial()
    {
        _mat.DisableKeyword("_EMISSION");
    }
    
    private void UpdateTarget()
    {
        target = GetClosestPlayer();
    }
    
    private Transform GetClosestPlayer()
    {
        var players = PlayerTracker.Instance.Players;

        Transform closest      = null;
        float closestSqrDist   = _sqrDetectionRange;

        foreach (var player in players)
        {
            if (player == null) continue;

            float sqrDist = (transform.position - player.position).sqrMagnitude;

            if (sqrDist < closestSqrDist)
            {
                closestSqrDist = sqrDist;
                closest        = player;
            }
        }

        return closest; 
    }
}




