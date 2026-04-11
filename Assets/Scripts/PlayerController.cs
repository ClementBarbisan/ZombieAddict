using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    
    [FormerlySerializedAs("_maxHealth")] [Header("Stats")]
    public float maxHealth = 100f;

    [FormerlySerializedAs("_currentHealth")] public float currentHealth;
    public bool isDead = false;
    public WebsocketManager.InfosPlayer infos = new WebsocketManager.InfosPlayer();

    [Header("ColorRender")] 
    [SerializeField] private Renderer rendererBodyColor;
    [SerializeField] private SpriteRenderer UIPlayerColor;
    [SerializeField] private TMP_Text namePlayer;

    [Header("VFX")] 
    [SerializeField] private ParticleSystem vfxWalkSmoke;
    [SerializeField] private Light _lightPoint;

    [Header("Audios")] 
    [SerializeField] private AudioSource audioRun;
    [SerializeField] private AudioClip clipHit;
    
    [Header("Debug")] 
    public bool useInputUnity;
    
    [Header("Events")]
    public UnityEvent<float> OnHit;       
    public UnityEvent OnDeath;
    public UnityEvent OnKillEnemy;
    
    private static readonly int Move = Animator.StringToHash("Move");
    private Transform _grabbedObject;
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Vector2 _move;
    private Transform _cam;
    private bool _vfxWalkSmokePlaying;
    private PlayerWeapon _playerWeapon;
    private bool _canMove = true;
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    private Vector3 _oldPos;
    private Material _mat;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _cam = Camera.main.transform;
        _playerWeapon = GetComponent<PlayerWeapon>();
        _playerWeapon.OnHitEnemy?.AddListener((int x) => HitEnemy(x));
        namePlayer.transform.SetParent(null);
        currentHealth = maxHealth;
        _mat = rendererBodyColor.material;
    }
    public void Init(string name, Color color)
    {
        namePlayer.text = name;
        infos.name = name;
        namePlayer.color = color;
        rendererBodyColor.material.color = color;
        UIPlayerColor.color = color;
        _lightPoint.color = color;
    }
    private void FixedUpdate()
    {
        if (!_canMove)
            return;
        
        //Camera
        Vector3 camForward = _cam.forward;
        Vector3 camRight = _cam.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 currentPos = transform.position;
        // Move
        Vector3 move = camRight * _move.x + camForward * _move.y;
        Vector3 velocity = new Vector3(move.x * moveSpeed, _rb.linearVelocity.y, move.z * moveSpeed);
        _rb.linearVelocity = velocity;
        infos.walkDistance += Vector3.Distance(currentPos, _oldPos);
        // Rotation only if moving
        Vector3 horizontalMove = new Vector3(move.x, 0f, move.z);
        
        if (_playerWeapon.target == null)
        {
            if (horizontalMove.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(horizontalMove);
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, 0.2f));
            }   
        }
        else
        {
            // Auto AIM
            Vector3 direction = (_playerWeapon.target.position - transform.position).normalized;
            
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (direction.sqrMagnitude > 0.001f)
                _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, .2f));
        }
        _oldPos = transform.position;
    }
    private void Update()
    {
        if (_move != Vector2.zero && !_vfxWalkSmokePlaying)
        {
            _vfxWalkSmokePlaying = true;
            vfxWalkSmoke.Play();
        }
        else if (_move == Vector2.zero && _vfxWalkSmokePlaying)
        {
            _vfxWalkSmokePlaying = false;
            vfxWalkSmoke.Stop();
        }

        TextNamePosition();
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        bool isMoving = _rb.linearVelocity.sqrMagnitude > 0.01f;
        animator.SetBool(Move, isMoving);

        if (isMoving && !audioRun.isPlaying)
            audioRun.Play();
        else if(!isMoving && audioRun.isPlaying)
            audioRun.Stop();
    }
    private void TextNamePosition()
    {
        Vector3 p = transform.position + new Vector3(0f, 10f, -4f);
        namePlayer.transform.position = Vector3.Lerp(namePlayer.transform.position, p, Time.deltaTime * 17f);
    }
    public void HandleInputs(Vector2 move, bool button1, bool button2)
    {
        // MOVE
        _move = move;

        // ATTACK
        animator.SetTrigger(Shoot);
        _playerWeapon.HandleFire(button1, infos);
       

        // INTERACT 
        
    }
    
    #region Input Callbacks (New Input System)
    private void OnEnable()
    {
        _playerInput.onActionTriggered += HandleAction;
        PlayerTracker.Instance.Register(transform);
    }
    private void OnDisable()
    {
        _playerInput.onActionTriggered -= HandleAction;
        PlayerTracker.Instance.Unregister(transform);
    }
    private void HandleAction(InputAction.CallbackContext ctx)
    {
        if (!useInputUnity)
            return;
        
        if (ctx.action.name == "Move")
        {
            _move = ctx.ReadValue<Vector2>();
        }

        if (ctx.action.name == "Attack")
        {
            if (ctx.started)
            {
                animator.SetTrigger(Shoot);
                _playerWeapon.HandleFire(true, infos);
                infos.shootFired++;
            }
        }
        else
        {
            _playerWeapon.HandleFire(false, infos);
        }
        
        if (ctx.action.name == "Interact")
        {
            
        }
    }
    #endregion

    public void KillEnemy()
    {
        OnKillEnemy?.Invoke();
        infos.enemyKilled += 1;
    }
    
    public void HitEnemy(int damages)
    {
        infos.shootSuccessfull++;
        if (infos.shootFired > 0)
            infos.accuracy = infos.shootSuccessfull / infos.shootFired;
        infos.damagesEnemy += damages;
    }
    
    public void TakeDamage(float amount, PlayerController player)
    {
        _mat.EnableKeyword("_EMISSION");
        Invoke(nameof(ResetMaterial), .05f);
        
        if (isDead) return;
        
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        infos.damages += (int)amount;
        OnHit?.Invoke(currentHealth);

        if (currentHealth <= 0f)
            Die();
        else
            animator.Play("BAKED_Hit");
        
        AudioSource.PlayClipAtPoint(clipHit, transform.position);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();
        _canMove = false;
        animator.Play("BAKED_Death");
        WebsocketManager.Instance.zombiePlayerInfos.nbPlayerDead++;
        this.enabled = false;
    }
    
    private void ResetMaterial()
    {
        _mat.DisableKeyword("_EMISSION");
    }
}