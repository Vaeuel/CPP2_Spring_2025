using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    private AnimationManager animMan;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject hitboxPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float attackInterval = 2f;
    [SerializeField] private float meleeAttackRange = 2.5f;
    [SerializeField] private Transform pT;

    private float attackTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animMan = GetComponent<AnimationManager>();
    }

    public void SetTarget(Transform player)
    {
        pT = player;
    }

    public void Attack(string sourceName)
    {
        if (sourceName == "Sorceress")
        {
            attackInterval = 2.5f;
            TryAttack();
        }

        if (sourceName == "Swordsman" && pT != null)
        {
            attackInterval = 1f;
            float dist = Vector3.Distance(transform.position, pT.position);
            if (dist <= meleeAttackRange)
            {
                TryAttack();
            }
        }
    }

    private void TryAttack()
    {
        attackTimer += Time.deltaTime;//Timer functionality
        if (attackTimer >= attackInterval)
        {
            animMan.EnemyAttack();
            attackTimer = 0f;//reset timer
            Debug.Log("EnemyCombat: Attack sequence Tried to attack player because of range");
        }
    }

    public void ShootFireball()
    {
        if (fireballPrefab != null && spawnPoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, spawnPoint.position, spawnPoint.rotation);
            fireball.GetComponent<Rigidbody>().linearVelocity = spawnPoint.forward * 10f; // Adjust speed as needed
        }
    }

    public void SpawnHitBox()
    {
        if (hitboxPrefab != null && spawnPoint != null)
        {
            Instantiate(hitboxPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
