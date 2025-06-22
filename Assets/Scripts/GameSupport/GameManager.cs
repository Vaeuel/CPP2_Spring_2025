using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

// Game Manager requires other manager components
public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject virtualCameraObject;

    private static GameManager instance = null; //Internal reference to single active instance of object - for singleton behaviour
    public static GameManager Instance //C# property to retrieve currently active instance of object, if any
    {
        get
        {
            if (!instance)
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            return instance;
        }
    }

    //Internal reference to Saveload Game Manager
    public static LoadSaveManager saveManager = null; //This seems odd. Need further explanation toward the logic of this.

    public static LoadSaveManager StateManager //C# property to retrieve save/load manager
    {
        get
        {
            if (!saveManager)
            {
                saveManager = instance.GetComponent<LoadSaveManager>();
                if (!saveManager)
                {
                    saveManager = instance.gameObject.AddComponent<LoadSaveManager>();
                }
            }

            return saveManager;
        }
    }

       void Awake()
    {
       
        if ((instance) && (instance.GetInstanceID() != GetInstanceID())) //Check if there is an existing instance of this object
            Destroy(gameObject); //Delete duplicate
        else
        {
            instance = this; //Make this object the only instance
            DontDestroyOnLoad(gameObject); //Set as do not destroy
        }
        if (saveManager == null) saveManager = GetComponent<LoadSaveManager>();
    }

    public void RestartGame() //Restart Game
    {
        SceneManager.LoadScene(1); //Load first level
    }

    public void ExitGame() //Exit Game
    {
        Application.Quit();
    }

    // Save Game
    public void SaveGame()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        Debug.Log(Application.persistentDataPath); //persistentDataPath will show an appData folder

        if (playerObj != null) //Call save game functionality
        {
            Transform pt = playerObj.transform;
            Debug.Log(saveManager);

            saveManager.gameState.player.transformData = new LoadSaveManager.GameStateData.DataTransform
            {
                posX = pt.position.x,
                posY = pt.position.y,
                posZ = pt.position.z,
                rotX = pt.eulerAngles.x,
                rotY = pt.eulerAngles.y,
                rotZ = pt.eulerAngles.z,
                scaleX = pt.localScale.x,
                scaleY = pt.localScale.y,
                scaleZ = pt.localScale.z
            };

            if (playerObj != null)
            {
                PlayerControl script = playerObj.GetComponent<PlayerControl>();
                if (script != null)
                {
                    saveManager.gameState.player.collectedWeapon = script.isWeapon;
                }
            }
        }
        saveManager.Save(Application.persistentDataPath + "/SaveGame.xml");
    }

    public void LoadGame() //Load Game
    {
        Debug.Log("GameManager: Entered LoadGame()");
        StateManager.Load("SaveGame.xml");
        RespawnPlayer();
    }

    public void RespawnPlayer()
    {
        saveManager.Load(); // Loads the GameStateData

        var playerData = saveManager.gameState.player;
        Vector3 position = new Vector3(playerData.transformData.posX, playerData.transformData.posY, playerData.transformData.posZ);
        Quaternion rotation = Quaternion.Euler(playerData.transformData.rotX, playerData.transformData.rotY, playerData.transformData.rotZ);
        Vector3 scale = new Vector3(playerData.transformData.scaleX, playerData.transformData.scaleY, playerData.transformData.scaleZ);

        GameObject newPlayer = Instantiate(playerPrefab, position, rotation);
        newPlayer.transform.localScale = scale;
        newPlayer.name = playerPrefab.name;

        // Pass playerData into the new player (assumes you have a Player script with SetData())
        var playerScript = newPlayer.GetComponent<PlayerControl>();
        playerScript.SetData(playerData);

        var cineCam = virtualCameraObject.GetComponent<CinemachineCamera>();
        if (cineCam != null)
        {
            cineCam.Follow = newPlayer.transform;
            //cineCam.LookAt = newPlayer.transform; //optional
        }
    }
}
