using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    // Visible //

    [ReadOnly][SerializeField] private int _playerNumber;

    [Header("Jumping")]
    [Tooltip("The Height of the jumps")]
    [SerializeField] private float _jump = 20;
    [Tooltip("The Falling Speed")]
    [SerializeField] private float _gravity = 10;

    [Header("Sliding")]
    [Tooltip("on What floor angle we start to slide and cant jump on")]
    [SerializeField] private float _slopeLimit = 45;
    [Tooltip("The Speed of sliding")]
    [SerializeField] private float _slideSpeed = 5;
    [Tooltip("Are we sliding?")]
    [ReadOnly][SerializeField] private bool _isSliding;
    [Tooltip("The Normal of the floor, (how steep is the floor)")]
    [ReadOnly][SerializeField] private Vector3 _hitNormal;

    [Header("Ground Check")]
    [Tooltip("the Y position of the Ground Checkbox")]
    [SerializeField] private float _checkboxY;
    [Tooltip("how wide the Ground Checkbox")]
    [SerializeField] private float _wide = 0;
    [Tooltip("how tall the Ground Checkbox")]
    [SerializeField] private float _height = .15f;
    [Tooltip("Are we On The Ground?")]
    [ReadOnly][SerializeField] private bool _isGrounded;

    [Header("References")]
    [Tooltip("Place The Player's Camera Here")]
    [SerializeField] private Transform _camTransform;
    [Tooltip("Place The Player's Cinemachine Here")]
    [SerializeField] private CinemachineFreeLook _cfl;

    [HideInInspector] public float FruitSpeedEffect=1;
    [HideInInspector] public float FruitJumpEffect=1;

    // Invisible //

    // Auto Referencing
    private CharacterController _cc => GetComponent<CharacterController>();
    private PlayerSlingshot _slingshot  => transform.GetChild(0).GetComponent<PlayerSlingshot>();
    private Camera _cam => _camTransform.GetComponent<Camera>();
    private InputHandler _inputHandler => _cfl.GetComponent<InputHandler>();
    private PlayerInput _playerInput => GetComponent<PlayerInput>();
    private GameManager _gm => GameManager.Instance;

    // Stored Data
    private Vector2 _movement;
    private bool _jumped;
    private float _gravityPull;
    private float _f;
    private LayerMask _jumpable;

    // Ground Check 
    private Vector3 _boxPosition => _cc.transform.position + (Vector3.up * _cc.bounds.extents.y) * _checkboxY;
    private Vector3 _boxSize => new Vector3(_cc.bounds.extents.x + _wide, _height * 2, _cc.bounds.extents.z + _wide);


    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        _loop+= GroundCheck;
        _loop+= Gravitation;
        _loop+= Jumping;
        _loop += PlayerMovement;
        _loop += Slide;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        _jumpable = _gm.CanJumpOn;
        SetUpPlayer();
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
    }

    /// <summary> Allows the player to walk. </summary>
    private void PlayerMovement()
    {
        Vector2 Movement = _movement.normalized; //Get input from player for movem

        float targetAngle = Mathf.Atan2(Movement.x, Movement.y) * Mathf.Rad2Deg + _camTransform.eulerAngles.y; //get where player is looking
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, !_slingshot.IsAiming ? targetAngle : _camTransform.eulerAngles.y, ref _f, 0.1f); //Smoothing
        //Debug.Log($"targetAngle: {targetAngle}, Angle: {Angle}");
        if (_slingshot.IsAiming)
            transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation



        if (Movement.magnitude > 0.1f)
        {
            _anim.SetBool("Walking", true);
            if (!_slingshot.IsAiming && Speed!=0)
                transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation
            Vector3 MoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            _cc.Move(MoveDir * GetSpeed() * Time.deltaTime);
        }
        else { 
        _anim.SetBool("Walking", false);
        }
    }

    /// <summary> Allows the player to jump. </summary>
    private void Jumping()
    {
        if (_jumped && _isGrounded && !_isSliding)
        {
            AddForce(Vector3.up, _jump * FruitJumpEffect );
        }
    }

    /// <summary> Takes care of Gravity </summary>
    private void Gravitation()
    {
        if (_isGrounded)
        {
            _gravityPull = .1f;
        }
        else if (_gravityPull < 1)
        {
            _gravityPull += .2f * Time.deltaTime;
        }
        _cc.Move(Vector3.down * _gravity * _gravityPull * Time.deltaTime);
    }

    /// <summary> Checking the ground and tell the player if he is grounded or sliding. </summary>
    private void GroundCheck()
    {
        _isGrounded = Physics.CheckBox(_boxPosition, _boxSize, quaternion.identity, _jumpable);
        
        _isSliding = (!(Vector3.Angle(Vector3.up, _hitNormal) <= _slopeLimit));
        _hitNormal = _hitNormal * .99f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (_jumpable == (_jumpable | (1 << hit.gameObject.layer)) && hit.point.y<transform.position.y)
        {
            _hitNormal = hit.normal;
        }
    }

    /// <summary> Making the player to slide on steep slopes. </summary>
    private void Slide()
    {
        if (_isSliding)
        {
            Vector3 slid = Vector3.zero;
            slid.x += ((1f - _hitNormal.y) * _hitNormal.x * _slideSpeed);
            slid.z += ((1f - _hitNormal.y) * _hitNormal.z * _slideSpeed);

            _cc.Move(slid*Time.deltaTime);
        }
    }

    /// <summary> When the Player Spawns, This takes care of him. </summary>
    private void SetUpPlayer()
    {
        _inputHandler.horizontal = _playerInput.actions.FindAction("Look"); // Set up Looking Controls 
        _playerNumber = _gm.AddPlayer(gameObject); // Give The Player a Number
        switch (_playerNumber) // Makes Cinemachine work with local Multiplayer
        {
            case 1: _cfl.gameObject.layer = LayerMask.NameToLayer("Player1"); _cam.cullingMask = _gm.PlayerMasks[0];
                break;
            case 2:
                _cfl.gameObject.layer = LayerMask.NameToLayer("Player2"); _cam.cullingMask = _gm.PlayerMasks[1];
                break;
            case 3:
                _cfl.gameObject.layer = LayerMask.NameToLayer("Player3"); _cam.cullingMask = _gm.PlayerMasks[2];
                break;
            case 4:
                _cfl.gameObject.layer = LayerMask.NameToLayer("Player4"); _cam.cullingMask = _gm.PlayerMasks[3];
                break;
            default: return;
        }
    }

    /// <summary> Triggers when the player disconnects his controller </summary>
    public void Leave()
    {
        if (_gm.LeaveOnDisconnect) // Only Remove Player if LeaveOnDisconnect is True
        {
            _gm.PlayerLeft(_playerNumber); // Tell The Game Manager we Removed a Player
            Destroy(gameObject); // Remove The Player
        }
    }

    protected override void ApplyingForce()
    {
        if (_forceStrength > 0)
        {
            _cc.Move(_forceDirection.normalized * _forceStrength * Time.deltaTime);
            _forceStrength -= 0.02f + _forceStrength * 2 * Time.deltaTime;
        }
    }

    // Inputs 
    public void OnMove(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
       _jumped  = context.action.triggered;
    }

    //Gizmos
    private void OnDrawGizmosSelected()
    {
        // Draw a Box in the Editor to show whether we are touching the ground, Blue is Touching, Red is Not Touching.
        Gizmos.color = _isGrounded ? Color.blue : Color.red; Gizmos.DrawCube(_boxPosition, _boxSize * 2);
    }

    public override float GetSpeed()
    {
        return Speed * FruitSpeedEffect * (1 - (_glubCurrentEffect / (_glubMax + (_glubMax / 10))));
    }
    public override void SetSpeed(float speed = -1)
    {
        if (speed != -1)
        {
            Speed = speed;
        }
        _anim.SetFloat("Speed", GetSpeed() / RegularAnimationSpeed);
    }

}

