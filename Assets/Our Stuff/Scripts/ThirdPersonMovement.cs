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
    [SerializeField] float Speed = 10;
    [SerializeField] float Gravity = 10;
    [SerializeField] float MouseXSpeed = 10;
    [SerializeField] float MouseYSpeed = 10;
    CharacterController CC;
    Camera MyCam;
    [SerializeField] Transform cam;
    [SerializeField] CinemachineFreeLook cfl;

    float f;

    Vector3 ForceDirection;
    float   ForceStrength;

    [SerializeField] float Jump = 20;
    [SerializeField] float gravityPull;

    bool Grounded;
    bool G;

    Vector3 hitNormal;

    [SerializeField] float slopeLimit = 80;

    [SerializeField] float slideFriction;

    Vector2 movement;
    Vector2 look;
    bool Jumped;

    public int PlayerNumber;
    public LayerMask[] masks = new LayerMask[4];

    private void Awake()
    {
        CC = GetComponent<CharacterController>();
        MyCam = cam.GetComponent<Camera>();
    }

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
        Jumping();
        Movement();
        AddForce();
        Look();
        slide();
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
        if (Grounded)
        {
            gravityPull = 0.1f;
        }
        else if (gravityPull < 1)
        {
            gravityPull += 0.2f * Time.deltaTime;
        }
        CC.Move(Vector3.down * Gravity * gravityPull * Time.deltaTime);

        G = !CC.isGrounded;

        if (Jumped && Grounded)
        {
            AddForce(Vector3.up, Jump);
        }
    }

    private void slide()
    {
        Grounded = ((Vector3.Angle(Vector3.up, hitNormal) <= slopeLimit) && !G);
        if (!Grounded && !G)
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
    void Look()
    {
        cfl.m_XAxis.Value += look.x * MouseXSpeed * Time.deltaTime;
        cfl.m_YAxis.Value -= look.y * MouseYSpeed * Time.deltaTime;
    }
    

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }

    void AddForce(Vector3 dir, float force)
    {
        ForceDirection = dir;
        ForceStrength  = force;
    }

    void SetUpPlayer()
    {
        PlayerNumber = GameManager.instance.PlayersAmount;
        switch (PlayerNumber)
        {
            case 1: cfl.gameObject.layer = LayerMask.NameToLayer("Player1"); MyCam.cullingMask = masks[0];
                break;
            case 2:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player2"); MyCam.cullingMask = masks[1];
                break;
            case 3:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player3"); MyCam.cullingMask = masks[2];
                break;
            case 4:
                cfl.gameObject.layer = LayerMask.NameToLayer("Player4"); MyCam.cullingMask = masks[3];
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
}
