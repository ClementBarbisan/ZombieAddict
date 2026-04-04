using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    

    [Header("Settings")] 
    public float speed = 3.5f;
    public float life = 10f;
    
    [Header("References")]
    public Transform target;
    [SerializeField] private Renderer renderer;

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

    public void Hit()
    {
        life--;
        
    }
}
