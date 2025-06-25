using UnityEngine;
using UnityEngine.AI;
using static Spawner;

public class EnemyController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private int chaseSpeed = 3;
    [SerializeField] private int rotationSpeed = 2;

    private AnimationManager animMan;
    private SkinnedMeshRenderer booRenderer;
    private CapsuleCollider cc;
    private EnemyPathFinder epf;
    private EnemyCombat ec;
    private GameObject spawnerPreFab;

    private float velocity;

    private bool playerIsLooking = false;
    private bool isDead = false;
    private Vector3 lastPOS;

    private void Start()
    {
        lastPOS = transform.position;
        animMan = GetComponent<AnimationManager>();
        epf = GetComponent<EnemyPathFinder>();
        cc = GetComponent<CapsuleCollider>();
        ec = GetComponent<EnemyCombat>();

        if (epf) epf.SetState(EnemyPathFinder.EnemyState.Patrol);

        if (animMan == null) Debug.Log("Enemy Controller: Animation manager not found");
        if (epf == null) Debug.LogError("EnemyPathfinder missing from BooController!");
    }

    private void Update()
    {
        if (!isDead)
        {
            if (player != null)
            {
                if (epf.curState == EnemyPathFinder.EnemyState.Chase)
                {
                    string baseName = gameObject.name.Replace(" (Clone)", "").Split(' ')[0];
                    ec.SetTarget(player);
                    ec.Attack(baseName);
                }
                if (!playerIsLooking)
                {
                    SetVisible(true);
                    //ChasePlayer();
                    //HandleShooting();
                }
                else
                {
                    SetVisible(false);
                    //if (epf) epf.GetAgent().ResetPath();
                }
            }

            //transform.position += new Vector3(0f, verticalVelocity * Time.deltaTime, 0f); // Apply vertical movement
            Vector3 vel = transform.position - lastPOS;
            velocity = new Vector2(vel.x, vel.z).magnitude / Time.deltaTime * .1f;
            animMan.LocoVel(velocity);
            //Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * (groundCheckDistance + 0.1f), Color.red);

            lastPOS = transform.position;
            //Debug.Log($"Boo controller: lastPOS is {lastPOS}");
            //Debug.Log($"Boo controller: Velocity is {velocity}");
        }
    }

    public void OnDetected(GameObject who)
    {
        if (who.CompareTag("Player"))
        {
            if (!isDead)
            {
                player = who.transform;
                playerIsLooking = true;
                //Debug.Log("BooController: OnDetected has confirmed it's been detected by the Player");

                if (epf)
                {
                    epf.SetPlayer(player);
                    epf.SetState(EnemyPathFinder.EnemyState.Freeze);
                }
            }

        }
    }

    public void LostLOS() //Need to expand this logic if using it with multiple players or objects targeting or not targeting.
    {
        velocity = 0f;
        playerIsLooking = false;
        if (!isDead)
        {
            if (epf)
            {
                epf.SetState(EnemyPathFinder.EnemyState.Chase);
            }
        }
    }

    private void SetVisible(bool visible)
    {
        if (booRenderer != null)
            booRenderer.enabled = visible;
    }

    public void Death()
    {
        Debug.Log("Entered Death");
        player = null;
        playerIsLooking = false;
        isDead = true;
        cc.enabled = false;
        epf.SetState(EnemyPathFinder.EnemyState.Dead);
        Debug.Log("Changed Enemy State");
        LootSpawn();
        Debug.Log("Exited LootSpawn");
        //Destroy(gameObject);
    }

    public void LootSpawn()
    {
        if (spawnerPreFab == null)
        {
            //Debug.Log("Spawner prefab is null");
            spawnerPreFab = Resources.Load<GameObject>("Prefabs/Spawner");
            //Debug.Log($"Set SpawnerPreFab equal to {spawnerPreFab}");
        }

        GameObject lootSpawn = Instantiate(spawnerPreFab, transform.position, Quaternion.identity);
        //Debug.Log($"Ran Instantiation and set {lootSpawn} Based on {spawnerPreFab}, {transform.position}, and {Quaternion.identity}");

        Spawner spawnerScript = lootSpawn.GetComponent<Spawner>();
        //Debug.Log($"Set spawnerScript equal to {spawnerScript}");
        if (spawnerScript != null)
        {
            spawnerScript.spawnType = SpawnType.Enemy;
            //Debug.Log("Set Spawn Type to Enemy");
            spawnerScript.TrySpawn();
            //Debug.Log("Should've run TrySpawn()");
        }
        else
        {
            Debug.LogWarning("EnemyController: Spawner component not found on lootSpawn.");
        }
    }
}
#region Unused ground Check logic
//[Header("Ground & Gravity Settings")]
//[SerializeField] private LayerMask groundMask;
//[SerializeField] private float gravity = -9.81f;
//[SerializeField] private float groundCheckDistance = 0.1f;
//private float verticalVelocity;
//private bool isGrounded;

// This is used by Update
//Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
//isGrounded = Physics.Raycast(ray, groundCheckDistance + 0.1f, groundMask);

//if (!isGrounded) // Gravity
//{
//    verticalVelocity += gravity * Time.deltaTime;
//}
//else
//{
//    verticalVelocity = 0f;
//}
#endregion