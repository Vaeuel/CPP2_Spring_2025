//using UnityEngine;
//using UnityEngine.AI;

//public class BooController : MonoBehaviour
//{
//    [Header("Chase Settings")]
//    [SerializeField] private Transform player;
//    [SerializeField] private float chaseSpeed = 3f;

//    [Header("Shooting Settings")]
//    [SerializeField] private GameObject fireballPrefab;
//    [SerializeField] private Transform fireballSpawnPoint;
//    [SerializeField] private float shootInterval = 2f;

//    private MeshRenderer booRenderer;
//    private NavMeshAgent agent;
//    private bool playerIsLooking = false;
//    private float shootTimer = 0f;

//    private void Awake()
//    {
//        booRenderer = GetComponentInChildren<MeshRenderer>();
//        agent = GetComponent<NavMeshAgent>();
//        agent.speed = chaseSpeed;
//    }

//    private void Update()
//    {
//        if (!playerIsLooking)
//        {
//            SetVisible(true);
//            ChasePlayer();
//            HandleShooting();
//        }
//        else
//        {
//            SetVisible(false);
//            agent.ResetPath();
//        }
//    }

//    public void SetPlayerIsLooking(bool isLooking)
//    {
//        playerIsLooking = isLooking;
//    }

//    private void SetVisible(bool visible)
//    {
//        if (booRenderer != null)
//            booRenderer.enabled = visible;
//    }

//    private void ChasePlayer()
//    {
//        if (agent != null && player != null)
//        {
//            agent.SetDestination(player.position);
//        }
//    }

//    private void HandleShooting()
//    {
//        shootTimer += Time.deltaTime;//Timer functionality
//        if (shootTimer >= shootInterval)
//        {
//            ShootFireball();
//            shootTimer = 0f;//reset timer
//        }
//    }

//    private void ShootFireball()
//    {
//        if (fireballPrefab != null && fireballSpawnPoint != null)
//        {
//            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
//            Vector3 direction = (player.position - fireballSpawnPoint.position).normalized;
//            fireball.GetComponent<Rigidbody>().linearVelocity = direction * 10f; // Adjust speed if needed
//        }
//    }
//}
