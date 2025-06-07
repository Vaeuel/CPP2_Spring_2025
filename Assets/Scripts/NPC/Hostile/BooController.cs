using UnityEngine;
using UnityEngine.AI;

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

    private MeshRenderer booRenderer;
    //private NavMeshAgent agent;
    private bool playerIsLooking = false;
    private float shootTimer = 0f;

    private void Awake()
    {
        booRenderer = GetComponentInChildren<MeshRenderer>();
        //agent = GetComponent<NavMeshAgent>();
        //agent.speed = chaseSpeed;
    }

    private void Update()
    {
        if (player != null)
        {
            if (!playerIsLooking)
            {
                SetVisible(true);
                ChasePlayer();
                HandleShooting();
            }
            else
            {
                SetVisible(false);
                //agent.ResetPath();
            }
        }
    }

    public void OnDetected(GameObject who)
    {
        if (who.CompareTag("Player"))
        {
            player = who.transform;
            playerIsLooking = true;
        }
    }

    public void LostLOS() //Need to expand this logic if using it with multiple players or objects targeting or not targeting.
    { 
        playerIsLooking = false; 
    }

    private void SetVisible(bool visible)
    {
        if (booRenderer != null)
            booRenderer.enabled = visible;
    }

    private void ChasePlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, player.position, (chaseSpeed * 1.5f) * Time.deltaTime);
        }
        //if (agent != null && player != null)
        //{
        //    agent.SetDestination(player.position);
        //}
    }

    private void HandleShooting()
    {
        shootTimer += Time.deltaTime;//Timer functionality
        if (shootTimer >= shootInterval)
        {
            ShootFireball();
            shootTimer = 0f;//reset timer
        }
    }

    private void ShootFireball()
    {
        if (fireballPrefab != null && fireballSpawnPoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballSpawnPoint.rotation);
            fireball.GetComponent<Rigidbody>().linearVelocity = fireballSpawnPoint.forward * 10f; // Adjust speed as needed
        }
    }
}
