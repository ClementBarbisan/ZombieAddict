using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent _agent;
    private Animator _animator;
    private static readonly int Move = Animator.StringToHash("Move");

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
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
}
