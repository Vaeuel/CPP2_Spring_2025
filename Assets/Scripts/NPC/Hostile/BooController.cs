using UnityEngine;
using UnityEngine.AI;
using static Spawner;

public class BooController : MonoBehaviour
{
    [Header("Chase Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private int chaseSpeed = 3;
    [SerializeField] private int rotationSpeed = 2;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform fireballSpawnPoint;
    [SerializeField] private float shootInterval = 2f;

    //[Header("Ground & Gravity Settings")]
    //[SerializeField] private LayerMask groundMask;
    //[SerializeField] private float gravity = -9.81f;
    //[SerializeField] private float groundCheckDistance = 0.1f;
    //private float verticalVelocity;
    //private bool isGrounded;

    private AnimationManager animMan;
    private SkinnedMeshRenderer booRenderer;
    private EnemyPathFinder epf;
    private GameObject spawnerPreFab;

    private float shootTimer = 0f;
    private float velocity;

    private bool playerIsLooking = false;
    private Vector3 lastPOS;

    private void Start()
    {
        lastPOS = transform.position;
        animMan = GetComponent<AnimationManager>();
        epf = GetComponent<EnemyPathFinder>();
        //booRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        if (epf) epf.SetState(EnemyPathFinder.EnemyState.Patrol);

        if (animMan == null) Debug.Log("Boo Controller: Animation manager not found");
        if (epf == null) Debug.LogError("EnemyPathfinder missing from BooController!");
    }

    private void Update()
    {
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

        if (player != null)
        {
            if (epf.curState == EnemyPathFinder.EnemyState.Chase)
            {
                HandleShooting();
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

    public void OnDetected(GameObject who)
    {
        if (who.CompareTag("Player"))
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

    public void LostLOS() //Need to expand this logic if using it with multiple players or objects targeting or not targeting.
    {
        velocity = 0f;
        playerIsLooking = false;

        if (epf)
        {
            epf.SetState(EnemyPathFinder.EnemyState.Chase);
        }
    }

    private void SetVisible(bool visible)
    {
        if (booRenderer != null)
            booRenderer.enabled = visible;
    }

    private void HandleShooting()
    {
        shootTimer += Time.deltaTime;//Timer functionality
        if (shootTimer >= shootInterval)
        {
            animMan.EnemyAttack();
            shootTimer = 0f;//reset timer
        }
    }

    public void ShootFireball()
    {
        if (fireballPrefab != null && fireballSpawnPoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
            fireball.GetComponent<Rigidbody>().linearVelocity = fireballSpawnPoint.forward * 10f; // Adjust speed as needed
        }
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
            Debug.LogWarning("BooController: Spawner component not found on lootSpawn.");
        }
    }

    public void Death()
    {
        Debug.Log("Entered Death");
        epf.SetState(EnemyPathFinder.EnemyState.Freeze);
        Debug.Log("Changed Enemy State");
        LootSpawn();
        Debug.Log("Exited LootSpawn");
        //Destroy(gameObject);
    }
}

    //private void ChasePlayer()
    //{
    //    if (player != null)
    //    {
    //        Vector3 direction = (player.position - transform.position).normalized;
    //        Quaternion targetRotation = Quaternion.LookRotation(direction);
    //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    //        //GetComponent<Rigidbody>().MovePosition(transform.position + direction * chaseSpeed * Time.deltaTime);
    //        transform.position = Vector3.MoveTowards(transform.position, player.position, (chaseSpeed * 1.5f) * Time.deltaTime);
    //    }

    //    //if (agent != null && player != null)
    //    //{
    //    //    agent.SetDestination(player.position);
    //    //}
    //}