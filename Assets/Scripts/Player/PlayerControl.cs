using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static LoadSaveManager.GameStateData;

[RequireComponent(typeof(CharacterController), typeof(AnimationManager))]

public class PlayerControl : MonoBehaviour, ProjectActions.IOverworldActions
{
    public static event Action GameOverZoneEntered;

    ProjectActions input;
    CharacterController cc;
    Camera mainCam;
    AnimationManager animMan;
    GameManager gm;

    #region Inspector Variables
    [Header("Movement Variables")]
    [SerializeField] private float initSpeed = .1f;
    [SerializeField] private float maxSpeed = 1.0f;
    [SerializeField] private float moveAccel = .5f;
    [SerializeField] private float rotationSpeed = 5f;
    private float curSpeed = 5.0f;
    private bool canMove = true;
    private bool isSavingEnabled = false;
    public bool isWeapon = false;

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

    //#region Game Save logic
    ////Function called when saving game
    //public void SaveGamePrepare()
    //{
    //    // Get Player Data Object
    //    //LoadSaveManager.GameStateData.DataPlayer data = GameManager.saveManager.gameState.player;

    //    // Fill in player data for save game
    //    data.collectedWeapon = isWeapon;

    //    data.transformData.posX = transform.position.x;
    //    data.transformData.posY = transform.position.y;
    //    data.transformData.posZ = transform.position.z;
    //    data.transformData.rotX = transform.rotation.eulerAngles.x;
    //    data.transformData.rotY = transform.rotation.eulerAngles.y;
    //    data.transformData.rotZ = transform.rotation.eulerAngles.z;
    //}

    //// Function called when loading is complete
    //public void LoadGameComplete()
    //{
    //    // Get Player Data Object
    //    //LoadSaveManager.GameStateData.DataPlayer data = GameManager.saveManager.gameState.player;

    //    //Load data back to Player
    //    isWeapon = data.collectedWeapon;

    //    //Give player weapon, activate and destroy weapon power-up
    //    if (isWeapon)
    //    {
    //        //Find weapon powerup in level
    //        GameObject weapon = GameObject.Find("Sword");

    //        //Send OnTriggerEnter message
    //        weapon.SendMessage("OnTriggerEnter", GetComponent<CharacterController>(), SendMessageOptions.DontRequireReceiver);
    //    }



    //    //Set position
    //    transform.position = new Vector3(data.transformData.posX, data.transformData.posY, data.transformData.posZ);

    //    //Set rotation
    //    transform.rotation = Quaternion.Euler(data.transformData.rotX, data.transformData.rotY, data.transformData.rotZ);

    //    //Set scale
    //    transform.localScale = new Vector3(data.transformData.scaleX, data.transformData.scaleY, data.transformData.scaleZ);
    //}
    //#endregion

    void Awake()
    {
        //GameStateData savedData = LoadSaveManager.GameStateData;
        //SpawnPlayer(savedData);
    }

    public void SetData(LoadSaveManager.GameStateData.DataPlayer data)
    {
        isWeapon = data.collectedWeapon;
        //health = data.health;
        //cash = data.cash;

        transform.position = new Vector3(data.transformData.posX, data.transformData.posY, data.transformData.posZ);
        transform.eulerAngles = new Vector3(data.transformData.rotX, data.transformData.rotY, data.transformData.rotZ);
        transform.localScale = new Vector3(data.transformData.scaleX, data.transformData.scaleY, data.transformData.scaleZ);

        if (isWeapon && weapon == null)
        {
            GameObject weaponObj = GameObject.FindWithTag("Weapon");
            if (weaponObj != null)
            {
                weapon = weaponObj.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.Equip(GetComponent<Collider>(), rightHandAttachPoint);
                }
            }
            else
            {
                Debug.LogWarning("SetData: Weapon was collected but no weapon object found in scene.");
            }
        }
    }

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        mainCam = Camera.main;
        animMan = GetComponent<AnimationManager>();
        animMan.OnToggleMovement += SetMovementEnabled;
        gm = FindAnyObjectByType<GameManager>();

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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SavePoint"))
        {
            isSavingEnabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SavePoint"))
        {
            isSavingEnabled = false;
        }
    }

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
            isWeapon = true;
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
            isWeapon = false;   
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

    public void OnInteract(InputAction.CallbackContext context)
    {
        if(isSavingEnabled)
        {
            gm.SaveGame();
            Debug.Log("Player Progress saved.");
        }
    }

    #endregion
}
