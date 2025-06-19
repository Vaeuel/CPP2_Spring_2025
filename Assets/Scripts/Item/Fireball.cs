using UnityEngine;
using UnityEngine.SceneManagement;

public class Fireball : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f; // lifetime in seconds
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        string targetTag = collision.collider.tag;

        if (collision.collider.CompareTag("Player"))
        {
            AnimationManager animMan = collision.collider.GetComponentInChildren<AnimationManager>();
            if (animMan != null)
            {
                animMan.Death(targetTag);
            }
            Destroy(gameObject);
        }
    }
}
