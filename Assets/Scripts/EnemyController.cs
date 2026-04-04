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

    [Header("Events")]
    public UnityEvent<float> OnHit;       
    public UnityEvent OnDeath;

    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int Move = Animator.StringToHash("Move");
    private Material _mat;

    private void Start()
    {
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

    }

    private void HandleAnimator()
    {
        bool isMoving = _agent.velocity.sqrMagnitude > 0.01f;
        _animator.SetBool(Move, isMoving);
    }
    
    public void TakeDamage(float amount)
    {
        Debug.Log(_currentHealth);
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

        OnDeath?.Invoke();
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
    
}
