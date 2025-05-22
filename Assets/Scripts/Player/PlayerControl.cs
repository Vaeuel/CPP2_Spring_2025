using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]

public class PlayerControl : MonoBehaviour, ProjectActions.IOverworldActions
{
    public static event Action GameOverZoneEntered;

    ProjectActions input;
    CharacterController cc;
    Camera mainCam;

    #region Inspector Variables
    [Header("Movement Variables")]
    [SerializeField] private float initSpeed = .2f;
    [SerializeField] private float maxSpeed = 2.0f;
    [SerializeField] private float moveAccel = .2f;
    [SerializeField] private float rotationSpeed = 10f;
    private float curSpeed = 5.0f;

    [Header("Jump Variables")]
    [SerializeField] private float jumpHeight = .1f;
    [SerializeField] private float jumpTime = .3f;
    #endregion

    #region Physics calculations
    //values calculated based on jump stats
    private float timeToJumpApex;
    private float initJumpVelocity;

    private float gravity;//Calculated based on jump values **used to apply jump force (Y velocity)
    #endregion

    #region Character Movement
    Vector2 direction;
    Vector3 velocity;
    private bool isJumpPressed = false;
    #endregion

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        mainCam = Camera.main;

        timeToJumpApex = jumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initJumpVelocity = -(gravity * timeToJumpApex);
    }

    void OnEnable()
    {
        input = new ProjectActions();
        input.Enable();
        input.Overworld.SetCallbacks(this);
    }

    void OnDisable()
    {
        input.Disable();
        input.Overworld.RemoveCallbacks(this);
    }

    void FixedUpdate()
    {
        Vector3 desiredMoveDirection = ProjectedMoveDirection();

        //UpdateCharacterVelocity(desiredMoveDirection);

        cc.Move(UpdateCharacterVelocity(desiredMoveDirection));

        if (direction.magnitude > 0)
        {
            float timeStep = rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), timeStep);
        }
    }

    private Vector3 UpdateCharacterVelocity(Vector3 desiredDirection)
    {
        if (direction == Vector2.zero) curSpeed = initSpeed;

        velocity.x = desiredDirection.x * curSpeed;
        velocity.z = desiredDirection.z * curSpeed;

        curSpeed += moveAccel * Time.deltaTime;
        curSpeed = Mathf.Clamp(curSpeed, initSpeed, maxSpeed);

        if (!cc.isGrounded) velocity.y += gravity * Time.fixedDeltaTime;//checks ground and applies gravity if false
        else velocity.y = CheckJump();//else checks for jump command

        return velocity;
    }

    private Vector3 ProjectedMoveDirection()
    {
        Vector3 cameraRight = mainCam.transform.right;
        Vector3 cameraForward = mainCam.transform.forward;

        cameraRight.y = 0;
        cameraForward.y = 0;

        cameraRight.Normalize();
        cameraForward.Normalize();

        return cameraForward * direction.y + cameraRight * direction.x;//Formula to project movement vector onto the cameras forward and right plane **should allow the player to move along the cameras POV**
    }

    private float CheckJump()
    {
        if(isJumpPressed) return initJumpVelocity;
        else return -cc.minMoveDistance; //Constantly applies -Y velocity to ensure ground check is working
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "GameOver")
        {
            GameOverZoneEntered?.Invoke();
        }
    }
    #region Input Functions
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed) direction = context.ReadValue<Vector2>();
        if (context.canceled) direction = Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext context) => isJumpPressed = context.ReadValueAsButton(); //

    #endregion
}
