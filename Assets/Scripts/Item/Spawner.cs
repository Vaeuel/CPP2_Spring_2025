using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public SpawnType spawnType;
    public GameObject[] prefabsToSpawn; //Array of prefabs to choose from
    public GameObject[] prefabPowerUp; //Array of prefabs to choose from
    public Transform spawnPoint;
    //public bool spawnOnStart = false; //**would co-exist with an if in start for spawning by button or other trigger
    public enum SpawnType { Item, Collectible, Enemy }

    void Start()
    {
        //if (spawnOnStart) TrySpawn(); //**could be useful fo rtriggers that cause spawning.

        TrySpawn();
    }

    public void TrySpawn()
    {
        Debug.Log("Spawner: Entered TrySpawn Function");
        try
        {
            GameObject selectedPrefab = null;

            switch (spawnType)
            {
                case SpawnType.Enemy:
                    //Debug.Log("Entered Enemy spawn Type");
                    ValidateArray(prefabPowerUp, "PowerUp prefab list");
                    //Debug.Log("Presumably passed Validation");
                    selectedPrefab = prefabPowerUp[UnityEngine.Random.Range(0, prefabPowerUp.Length)]; // Randomly select one prefab from the array
                    //Debug.Log($"Set {selectedPrefab}");
                    ValidateComponents(selectedPrefab);
                    //Debug.Log("Validation of selectedPreFab presumably passed");
                    break;

                case SpawnType.Item:
                    ValidateArray(prefabsToSpawn, "Item prefab list");
                    selectedPrefab = prefabsToSpawn[UnityEngine.Random.Range(0, prefabsToSpawn.Length)]; // Randomly select one prefab from the array
                    ValidateComponents(selectedPrefab);
                    break;

                case SpawnType.Collectible:
                    // Future logic can go here
                    return;
            }

            if (selectedPrefab != null)
            {
                Debug.Log("Spawner: entered Selected PreFab set up IF statement");
                Transform location = spawnPoint != null ? spawnPoint : transform; //Spawn the object at the spawn point
                GameObject clone = Instantiate(selectedPrefab, location.position, Quaternion.identity);
                clone.name = selectedPrefab.name;

                PowerUps powerUpScript = clone.GetComponent<PowerUps>();
                if (powerUpScript != null)
                {
                    PowerUpType randomType = (PowerUpType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(PowerUpType)).Length); // Assign a random type
                    powerUpScript.type = randomType;
                }
            }
        }

        catch (Exception ex)
        {
            Debug.LogError($"Spawner Error: {ex.Message}");
        }
    }

    private void ValidateArray(GameObject[] array, string arrayName)
    {
        if (array == null || array.Length == 0)
            throw new NullReferenceException($"{arrayName} is empty or null.");

        foreach (var prefab in array)
        {
            if (prefab == null)
                throw new NullReferenceException($"A prefab in {arrayName} is missing.");
        }
    }

    private void ValidateComponents(GameObject prefab)
    {
        if (prefab.GetComponent<Rigidbody>() == null)
            throw new MissingComponentException($"{prefab.name} is missing a Rigidbody.");
        if (prefab.GetComponent<Collider>() == null)
            throw new MissingComponentException($"{prefab.name} is missing a Collider.");
    }
}

#region Old Spawner logic
//using System;
//using UnityEngine;

//public class Spawner : MonoBehaviour
//{
//    [Header("Spawner Settings")]
//    public SpawnType spawnType;
//    public GameObject[] prefabsToSpawn;  // Array of prefabs to choose from
//    public GameObject[] prefabPowerUp; //Array of prefabs to choose from
//    public Transform spawnPoint;
//    //public bool spawnOnStart = true; **would co-exist with an if in start for spawning by button or other trigger
//    public enum SpawnType { Item, Collectible, Enemy}

//    void Start()
//    {
//        //if (spawnOnStart) **could be useful fo rtriggers that cause spawning.
//            TrySpawn();
//    }

//    public void TrySpawn()
//    {
//        if (spawnType == SpawnType.Enemy)
//        {
//            try
//            {
//                if (prefabPowerUp == null || prefabPowerUp.Length == 0)
//                    throw new NullReferenceException("Prefabs list wasn't edited."); //causes exception if the designer forgot to edit the List

//                GameObject powerUpToSpawn = prefabPowerUp[UnityEngine.Random.Range(0, prefabPowerUp.Length)];
//            }
//        }

//        if (spawnType == SpawnType.Item)
//        {
//            try
//            {
//                if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
//                    throw new NullReferenceException("Prefabs list wasn't edited."); //causes exception if the designer forgot to edit the List

//                foreach (var prefab in prefabsToSpawn)//Check the PrefabsToSpawn list for null elements
//                {
//                    if (prefab == null)
//                        throw new NullReferenceException("At least one prefab is missing from the list! Dummy up!");
//                }

//                GameObject prefabToSpawn = prefabsToSpawn[UnityEngine.Random.Range(0, prefabsToSpawn.Length)]; // Randomly select one prefab from the array

//                //checks that the prefab has a Rigidbody and Collider, then causes an exception if !true
//                if (prefabToSpawn.GetComponent<Rigidbody>() == null)
//                    throw new MissingComponentException($"{prefabToSpawn.name} is missing a Rigidbody component.");
//                if (prefabToSpawn.GetComponent<Collider>() == null)
//                    throw new MissingComponentException($"{prefabToSpawn.name} is missing a Collider component.");

//                Transform spawnLocation = spawnPoint != null ? spawnPoint : transform; //Spawn the object at the spawn point
//                GameObject clone = Instantiate(prefabToSpawn, spawnLocation.position, Quaternion.identity);
//                clone.name = prefabToSpawn.name;

//                //Debug.Log($"Spawned: {clone.name} at {spawnLocation.position}");
//            }
//            catch (NullReferenceException ex)
//            {
//                Debug.LogError($"Spawner Error (Null): {ex.Message}");
//            }
//            catch (MissingComponentException ex)
//            {
//                Debug.LogError($"Spawner Error (Component): {ex.Message}");
//            }
//            catch (Exception ex)
//            {
//                Debug.LogError($"Spawner Error (General): {ex.Message}");
//            }
//        }
//    }
//}
#endregion