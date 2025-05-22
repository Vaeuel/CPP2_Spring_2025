using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] prefabsToSpawn;  // Array of prefabs to choose from
    public Transform spawnPoint;
    //public bool spawnOnStart = true; **would co-exist with an if in start for spawning by button or other trigger

    void Start()
    {
        //if (spawnOnStart) **could be useful fo rtriggers that cause spawning.
            TrySpawn();
    }

    public void TrySpawn()
    {
        try
        {
            if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
                throw new NullReferenceException("Prefabs list wasn't edited."); //causes exception if the designer forgot to edit the List

            foreach (var prefab in prefabsToSpawn)//Check the PrefabsToSpawn list for null elements
            {
                if (prefab == null)
                    throw new NullReferenceException("At least one prefab is missing from the list! Dummy up!");
            }

            GameObject prefabToSpawn = prefabsToSpawn[UnityEngine.Random.Range(0, prefabsToSpawn.Length)]; // Randomly select one prefab from the array

            //checks that the prefab has a Rigidbody and Collider, then causes an exception if !true
            if (prefabToSpawn.GetComponent<Rigidbody>() == null)
                throw new MissingComponentException($"{prefabToSpawn.name} is missing a Rigidbody component.");
            if (prefabToSpawn.GetComponent<Collider>() == null)
                throw new MissingComponentException($"{prefabToSpawn.name} is missing a Collider component.");

            Transform spawnLocation = spawnPoint != null ? spawnPoint : transform; //Spawn the object at the spawn point
            GameObject clone = Instantiate(prefabToSpawn, spawnLocation.position, Quaternion.identity);
            clone.name = prefabToSpawn.name;

            //Debug.Log($"Spawned: {clone.name} at {spawnLocation.position}");
        }
        catch (NullReferenceException ex)
        {
            Debug.LogError($"Spawner Error (Null): {ex.Message}");
        }
        catch (MissingComponentException ex)
        {
            Debug.LogError($"Spawner Error (Component): {ex.Message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Spawner Error (General): {ex.Message}");
        }
    }
}
