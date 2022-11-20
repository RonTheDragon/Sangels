using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

//using UnityEngine.UIElements;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] float Speed = 10;
    [Header("Jumping")]
    [SerializeField] float Gravity = 10;
    CharacterController CC => GetComponent<CharacterController>();
    Camera MyCam => cam.GetComponent<Camera>();
    [SerializeField] Transform cam;
    [SerializeField] CinemachineFreeLook cfl;
    InputHandler inputHandler => cfl.GetComponent<InputHandler>();
    PlayerInput playerInput => GetComponent<PlayerInput>();

    float f;

    Vector3 ForceDirection;
    float   ForceStrength;

    [SerializeField] float Jump = 20;
    [SerializeField] float gravityPull;

    Vector3 hitNormal;

    [SerializeField] float slopeLimit = 80;

    [SerializeField] float slideFriction;

    Vector2 movement;
    Vector2 look;
    bool Jumped;

    public int PlayerNumber;

    public float Y;
    public float Wide = 0;
    public float Height = .15f;
    public LayerMask Jumpable;
    Vector3 BoxOrigin => CC.transform.position + (Vector3.up * CC.bounds.extents.y) * Y;
    Vector3 RayCastSize => new Vector3(CC.bounds.extents.x + Wide, Height * 2, CC.bounds.extents.z + Wide);

    bool isGrounded;
    public bool isSliding;
    GameManager GM => GameManager.instance;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        SetUpPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Jumping();
        Movement();
        AddForce();
        slide();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckBox(BoxOrigin, RayCastSize, quaternion.identity, Jumpable);
        if (isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(BoxOrigin + Vector3.up * 0.5f, Vector3.down, out hit, 1, Jumpable))
            {
                hitNormal = hit.normal;
                //Debug.Log(hit.transform.name);
            }
        }
        else
        {
            hitNormal = hitNormal * 0.99f;
        }
        isSliding = (!(Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit));
    }

    private void AddForce()
    {
        if (ForceStrength > 0)
        {
            CC.Move(ForceDirection.normalized * ForceStrength * Time.deltaTime);
            ForceStrength -= ForceStrength * 2 * Time.deltaTime;
        }
    }

    private void Jumping()
    {
        if (isGrounded)
        {
            gravityPull = 0.1f;
        }
        else if (gravityPull < 1)
        {
            gravityPull += 0.2f * Time.deltaTime;
        }
        CC.Move(Vector3.down * Gravity * gravityPull * Time.deltaTime);

        if (Jumped && isGrounded && !isSliding)
        {
            AddForce(Vector3.up, Jump);
        }
    }

    private void slide()
    {
        if (isSliding)
        {
            Vector3 slid = Vector3.zero;
            slid.x += ((1f - hitNormal.y) * hitNormal.x * (1f - slideFriction));
            slid.z += ((1f - hitNormal.y) * hitNormal.z * (1f - slideFriction));

            CC.Move(slid);
        }
    }

    void Movement()
    {
        Vector2 Movement = movement.normalized; //Get input from player for movem

        float targetAngle = Mathf.Atan2(Movement.x, Movement.y) * Mathf.Rad2Deg + cam.eulerAngles.y; //get where player is looking
        float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref f, 0.1f); //Smoothing
        transform.rotation = Quaternion.Euler(0, Angle, 0); //Player rotation

        if (Movement.magnitude > 0.1f)
        {
            Vector3 MoveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            CC.Move(MoveDir * Speed * Time.deltaTime);
        }
    }

    void AddForce(Vector3 dir, float force)
    {
        ForceDirection = dir;
        ForceStrength  = force;
    }

    void SetUpPlayer()
    {
        inputHandler.horizontal = playerInput.actions.FindAction("Look"); // Set up Looking Controls 

        PlayerNumber = GameManager.instance.PlayersAmount; // Give The Player a Number
        switch (PlayerNumber)
        {
            case 1: cfl.gameObject.layer = LayerMask.NameToLayer("Player1"); MyCam.cullingMask = GM.PlayerMasks[0];
                break;
            case 2:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player2"); MyCam.cullingMask = GM.PlayerMasks[1];
                break;
            case 3:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player3"); MyCam.cullingMask = GM.PlayerMasks[2];
                break;
            case 4:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player4"); MyCam.cullingMask = GM.PlayerMasks[3];
                break;
            default: return;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
       Jumped  = context.action.triggered;
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a line in the Editor to show whether we are touching the ground.
        Gizmos.color = isGrounded ? Color.blue : Color.red; Gizmos.DrawCube(BoxOrigin, RayCastSize * 2);
        //Debug.DrawLine(RaycastOrigin, RaycastOrigin + Vector3.down * RaycastDistance, isGrounded ? Color.white : Color.red);
    }
}
