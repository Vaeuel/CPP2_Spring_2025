using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(AnimationManager))]

public class PlayerControl : MonoBehaviour, ProjectActions.IOverworldActions
{
    public static event Action GameOverZoneEntered;

    ProjectActions input;
    CharacterController cc;
    Camera mainCam;
    AnimationManager animMan;

    #region Inspector Variables
    [Header("Movement Variables")]
    [SerializeField] private float initSpeed = .1f;
    [SerializeField] private float maxSpeed = 1.0f;
    [SerializeField] private float moveAccel = .5f;
    [SerializeField] private float rotationSpeed = 5f;
    private float curSpeed = 5.0f;
    private bool canMove = true;

    [Header("Weapon Variables")]
    [SerializeField] private Transform rightHandAttachPoint;
    [SerializeField] private Transform leftHandAttachPoint;
    Weapon weapon = null;

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
    public Vector2 lookDelta;
    private float yaw;
    private bool isJumpPressed = false;

    #endregion

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        mainCam = Camera.main;
        animMan = GetComponent<AnimationManager>();
        animMan.OnToggleMovement += SetMovementEnabled;

        timeToJumpApex = jumpTime / 2;
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        initJumpVelocity = -(gravity * timeToJumpApex);
        yaw = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (canMove)
        {
            Vector2 groundVel = new Vector2(velocity.x, velocity.z); //Takes the gravityless velocity.
            animMan.LocoVel(groundVel.magnitude);
        }
        //Vector2 groundVel = new Vector2(velocity.x, velocity.z); //Takes the gravityless velocity.
        //animMan.LocoVel(groundVel.magnitude);
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
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
        Vector3 desiredMoveDirection = ProjectedMoveDirection(); //Calls PMD and Sets DMD equal to the result of PMD
        Vector3 camEuler = mainCam.transform.eulerAngles;

        if (canMove)
        {
            cc.Move(UpdateCharacterVelocity(desiredMoveDirection));
            yaw += lookDelta.x * rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
            //transform.rotation = Quaternion.Euler(0f, camEuler.y, 0f);
        }
        //cc.Move(UpdateCharacterVelocity(desiredMoveDirection));
        //transform.rotation = Quaternion.Euler(0f, camEuler.y, 0f);

        //UpdateCharacterVelocity(ProjectedMoveDirection()); //Higher order function: takes the returned value from the lower order function and applies a transformation to it
        //cc.Move(Velocity);
        //if (direction.magnitude > 0)
        //{
        //    float timeStep = rotationSpeed * Time.fixedDeltaTime;
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), timeStep);
        //}
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
        //Gets camera transforms so we can apply cam relative movement
        Vector3 cameraRight = mainCam.transform.right;
        Vector3 cameraForward = mainCam.transform.forward;

        //Removes Y component so there is no Y Rot being applied.
        cameraRight.y = 0;
        cameraForward.y = 0;

        //Normalized to return consistant unit vectors
        cameraRight.Normalize();
        cameraForward.Normalize();

        return cameraForward * direction.y + cameraRight * direction.x;//Formula to project movement vector onto the cameras forward and right plane **should allow the player to move along the cameras POV**
    }

    private float CheckJump()
    {
        if(isJumpPressed) return initJumpVelocity;
        else return -cc.minMoveDistance; //Constantly applies -Y velocity to ensure ground check is working
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("PowerUp"))
    //    {
    //        PowerUps powerUp = other.GetComponent<PowerUps>();
    //        if (powerUp != null)
    //        {
    //            powerUp.ApplyEffect(gameObject);
    //            Destroy(other.gameObject);
    //        }
    //    }
    //}

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("PowerUp"))
        {
            PowerUps powerUp = hit.collider.GetComponent<PowerUps>();
            if (powerUp != null)
            {
                powerUp.ApplyEffect(gameObject);
                Destroy(hit.gameObject);
            }
        }
        if (hit.collider.CompareTag("Weapon") && weapon == null)
        {
            weapon = hit.gameObject.GetComponent<Weapon>();
            weapon.Equip(GetComponent<Collider>(), rightHandAttachPoint);
        }
        if (hit.collider.tag == "GameOver")
        {
            GameOverZoneEntered?.Invoke();
        }
    }
    #region Input Functions

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (context.control.name == "leftButton")
        {
            if (weapon)
                animMan.Attacking("Primary"); //Primary and secondary allows for weapon swaps
        }
        else if (context.control.name == "rightButton")
        {
            animMan.Attacking("Secondary"); //Primary and secondary allows for weapon swaps
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (weapon)
        {
            weapon.Drop(GetComponent<Collider>(), transform.forward);
            weapon = null;
        }
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed) direction = context.ReadValue<Vector2>();
        if (context.canceled) direction = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) => isJumpPressed = context.ReadValueAsButton(); //

    #endregion
}
