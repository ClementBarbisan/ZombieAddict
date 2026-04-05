using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("ColorRender")] 
    [SerializeField] private Renderer rendererBodyColor;
    [SerializeField] private SpriteRenderer UIPlayerColor;
    [SerializeField] private TMP_Text namePlayer;

    [Header("VFX")] 
    [SerializeField] private ParticleSystem vfxWalkSmoke;
    
    [Header("Debug")] 
    public bool useInputUnity;
    public UnityEvent OnHit;
    
    private Transform _grabbedObject;
    private Rigidbody _rb;
    private PlayerInput _playerInput;
    private Vector2 _move;
    private Transform _cam;
    private bool _vfxWalkSmokePlaying;
    private PlayerWeapon _playerWeapon;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerInput = GetComponent<PlayerInput>();
        _cam = Camera.main.transform;
        _playerWeapon = GetComponent<PlayerWeapon>();
        namePlayer.transform.SetParent(null);
    }
    public void Init(string name)
    {
        namePlayer.text = name;
        Color c = Random.ColorHSV();
        namePlayer.color = c;
        rendererBodyColor.material.color = c;
        UIPlayerColor.color = c;
    }
    private void FixedUpdate()
    {
        //Camera
        Vector3 camForward = _cam.forward;
        Vector3 camRight = _cam.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // Move
        Vector3 move = camRight * _move.x + camForward * _move.y;
        Vector3 velocity = new Vector3(move.x * moveSpeed, _rb.linearVelocity.y, move.z * moveSpeed);
        _rb.linearVelocity = velocity;

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
        _playerWeapon.HandleFire(button1);
        
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
            if(ctx.started)
                _playerWeapon.HandleFire(true);
        }
        else
        {
            _playerWeapon.HandleFire(false);
        }
        
        if (ctx.action.name == "Interact")
        {
            
        }
    }
    #endregion
}