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
        if (collision.collider.CompareTag("Player"))
        {
            Destroy(collision.collider.gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Destroy(gameObject);
        }
    }
}
