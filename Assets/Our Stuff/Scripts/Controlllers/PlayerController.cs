using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controllers
{
    // Visible //

    [ReadOnly][SerializeField] int PlayerNumber;

    [Header("Jumping")]
    [Tooltip("The Height of the jumps")]
    [SerializeField] float _jump = 20;
    [Tooltip("The Falling Speed")]
    [SerializeField] float _gravity = 10;

    [Header("Sliding")]
    [Tooltip("on What floor angle we start to slide and cant jump on")]
    [SerializeField] float slopeLimit = 45;
    [Tooltip("The Speed of sliding")]
    [SerializeField] float SlideSpeed = 5;
    [Tooltip("Are we sliding?")]
    [ReadOnly][SerializeField] bool isSliding;
    [Tooltip("The Normal of the floor, (how steep is the floor)")]
    [ReadOnly][SerializeField] Vector3 hitNormal;

    [Header("Ground Check")]
    [Tooltip("the Y position of the Ground Checkbox")]
    [SerializeField] float Y;
    [Tooltip("how wide the Ground Checkbox")]
    [SerializeField] float Wide = 0;
    [Tooltip("how tall the Ground Checkbox")]
    [SerializeField] float Height = .15f;
    [Tooltip("Are we On The Ground?")]
    [ReadOnly][SerializeField] bool isGrounded;

    [Header("References")]
    [Tooltip("Place The Player's Camera Here")]
    [SerializeField] Transform cam;
    [Tooltip("Place The Player's Cinemachine Here")]
    [SerializeField] CinemachineFreeLook cfl;

    [HideInInspector] public float FruitSpeedEffect=1;
    [HideInInspector] public float FruitJumpEffect=1;

    // Invisible //

    // Auto Referencing
    CharacterController CC => GetComponent<CharacterController>();
    PlayerSlingshot SlingShot  => transform.GetChild(0).GetComponent<PlayerSlingshot>();
    Camera _cam => cam.GetComponent<Camera>();
    InputHandler _inputHandler => cfl.GetComponent<InputHandler>();
    PlayerInput _playerInput => GetComponent<PlayerInput>();
    GameManager GM => GameManager.instance;

    // Stored Data
    Vector2 _movement;
    bool _jumped;
    float _gravityPull;
    float f;
    LayerMask Jumpable;

    // Ground Check 
    Vector3 _boxPosition => CC.transform.position + (Vector3.up * CC.bounds.extents.y) * Y;
    Vector3 _boxSize => new Vector3(CC.bounds.extents.x + Wide, Height * 2, CC.bounds.extents.z + Wide);


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

        Loop+= groundCheck;
        Loop+= gravitation;
        Loop+= jumping;
        Loop += movement;
        Loop += slide;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        Jumpable = GM.CanJumpOn;
        setUpPlayer();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
    }

    /// <summary> Allows the player to walk. </summary>
    void movement()
    {
        Vector2 Movement = _movement.normalized; //Get input from player for movem

        float targetAngle = Mathf.Atan2(Movement.x, Movement.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //get where player is looking
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, !SlingShot.IsAiming ? targetAngle : cam.eulerAngles.y, ref f, 0.1f); //Smoothing
        //Debug.Log($"targetAngle: {targetAngle}, Angle: {Angle}");
        if (SlingShot.IsAiming)
            transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation



        if (Movement.magnitude > 0.1f)
        {
            anim.SetBool("Walking", true);
            if (!SlingShot.IsAiming && Speed!=0)
                transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation
            Vector3 MoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            CC.Move(MoveDir * GetSpeed() * Time.deltaTime);
        }
        else { 
        anim.SetBool("Walking", false);
        }
    }

    /// <summary> Allows the player to jump. </summary>
    private void jumping()
    {
        if (_jumped && isGrounded && !isSliding)
        {
            AddForce(Vector3.up, _jump * FruitJumpEffect );
        }
    }

    /// <summary> Takes care of Gravity </summary>
    private void gravitation()
    {
        if (isGrounded)
        {
            _gravityPull = .1f;
        }
        else if (_gravityPull < 1)
        {
            _gravityPull += .2f * Time.deltaTime;
        }
        CC.Move(Vector3.down * _gravity * _gravityPull * Time.deltaTime);
    }

    /// <summary> Checking the ground and tell the player if he is grounded or sliding. </summary>
    private void groundCheck()
    {
        isGrounded = Physics.CheckBox(_boxPosition, _boxSize, quaternion.identity, Jumpable);
        
        isSliding = (!(Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit));
        hitNormal = hitNormal * .99f;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Jumpable == (Jumpable | (1 << hit.gameObject.layer)) && hit.point.y<transform.position.y)
        {
            hitNormal = hit.normal;
        }
    }

    /// <summary> Making the player to slide on steep slopes. </summary>
    private void slide()
    {
        if (isSliding)
        {
            Vector3 slid = Vector3.zero;
            slid.x += ((1f - hitNormal.y) * hitNormal.x * SlideSpeed);
            slid.z += ((1f - hitNormal.y) * hitNormal.z * SlideSpeed);

            CC.Move(slid*Time.deltaTime);
        }
    }

    /// <summary> When the Player Spawns, This takes care of him. </summary>
    private void setUpPlayer()
    {
        _inputHandler.horizontal = _playerInput.actions.FindAction("Look"); // Set up Looking Controls 
        PlayerNumber = GM.AddPlayer(gameObject); // Give The Player a Number
        switch (PlayerNumber) // Makes Cinemachine work with local Multiplayer
        {
            case 1: cfl.gameObject.layer = LayerMask.NameToLayer("Player1"); _cam.cullingMask = GM.PlayerMasks[0];
                break;
            case 2:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player2"); _cam.cullingMask = GM.PlayerMasks[1];
                break;
            case 3:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player3"); _cam.cullingMask = GM.PlayerMasks[2];
                break;
            case 4:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player4"); _cam.cullingMask = GM.PlayerMasks[3];
                break;
            default: return;
        }
    }

    /// <summary> Triggers when the player disconnects his controller </summary>
    public void Leave()
    {
        if (GM.LeaveOnDisconnect) // Only Remove Player if LeaveOnDisconnect is True
        {
            GM.PlayerLeft(PlayerNumber); // Tell The Game Manager we Removed a Player
            Destroy(gameObject); // Remove The Player
        }
    }

    protected override void applyingForce()
    {
        if (_forceStrength > 0)
        {
            CC.Move(_forceDirection.normalized * _forceStrength * Time.deltaTime);
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
    void OnDrawGizmosSelected()
    {
        // Draw a Box in the Editor to show whether we are touching the ground, Blue is Touching, Red is Not Touching.
        Gizmos.color = isGrounded ? Color.blue : Color.red; Gizmos.DrawCube(_boxPosition, _boxSize * 2);
    }

    public override float GetSpeed()
    {
        return Speed * FruitSpeedEffect * (1 - (GlubCurrentEffect / (GlubMax + (GlubMax / 10))));
    }
    public override void SetSpeed(float speed = -1)
    {
        if (speed != -1)
        {
            Speed = speed;
        }
        anim.SetFloat("Speed", GetSpeed() / RegularAnimationSpeed);
    }

}

