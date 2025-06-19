using UnityEngine;

public enum PowerUpType { Health, Stamina, XP }

public class PowerUps : MonoBehaviour
{
    public PowerUpType type;

    public void ApplyEffect(GameObject player)
    {
        // You can replace this with actual stat calls
        switch (type)
        {
            case PowerUpType.Health:
                Debug.Log("Health Power-Up Applied");
                // player.GetComponent<PlayerStats>().AddHealth(25);
                break;

            case PowerUpType.Stamina:
                Debug.Log("Stamina Power-Up Applied");
                // player.GetComponent<PlayerStats>().RestoreStamina(15);
                break;

            case PowerUpType.XP:
                Debug.Log("XP Power-Up Applied");
                // player.GetComponent<PlayerStats>().AddXP(50);
                break;

            default:
                Debug.LogWarning("Unknown PowerUpType");
                break;
        }
    }
}
