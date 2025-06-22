using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Security.Cryptography;

public class LoadSaveManager : MonoBehaviour
{
    
    [XmlRoot("GameStateData")]
    public class GameStateData
    {
        [System.Serializable]
        public struct DataTransform
        {
            public float posX, posY, posZ;
            public float rotX, rotY, rotZ;
            public float scaleX, scaleY, scaleZ;
        }
        [System.Serializable]
        public class DataEnemy
        {
            public DataTransform transformData;
            public int enemyID;
            public int health;
        }
        [System.Serializable]
        public class DataPlayer
        {
            public DataTransform transformData;
            public float cash;
            public bool collectedWeapon;
            public int health;
        }

        public List<DataEnemy> enemies = new List<DataEnemy>();
        public DataPlayer player = new DataPlayer();
    }

    public GameStateData gameState = new GameStateData();

    private static readonly byte[] encryptionKey = Encoding.UTF8.GetBytes("MySecretKey12345"); //16-char key for AES-128 (must be exactly 16 chars)

    public void Save(string filename = "SaveGame.xml")
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));

        string xmlString; //Need to convert gameState to XML string
        using (StringWriter textWriter = new StringWriter())
        {
            serializer.Serialize(textWriter, gameState);
            xmlString = textWriter.ToString();
        }

        byte[] encryptedData = EncryptStringToBytes_Aes(xmlString, encryptionKey); //Encrypt the XMl String to Bytes
        File.WriteAllBytes(path, encryptedData); //Write encrypted Bytes to file

        //using (FileStream fileStream = new FileStream(path, FileMode.Create))
        //{
        
        //    serializer.Serialize(fileStream, gameState);
        //}
    }

    public void Load(string filename = "SaveGame.xml")
    {
        string path = Path.Combine(Application.persistentDataPath, filename);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"LoadSaveManager: Save file not found at {path}, load aborted.");
            return;
        }

        try
        {
            // Read encrypted bytes from file
            byte[] encryptedData = File.ReadAllBytes(path);

            // Decrypt bytes to XML string
            string xmlString = DecryptStringFromBytes_Aes(encryptedData, encryptionKey);

            // Deserialize XML string back into gameState
            XmlSerializer serializer = new XmlSerializer(typeof(GameStateData));
            using (StringReader reader = new StringReader(xmlString))
            {
                gameState = serializer.Deserialize(reader) as GameStateData;
            }

            Debug.Log($"LoadSaveManager: Loaded and decrypted game state from {path}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoadSaveManager: Failed to load or decrypt save file at {path}. Exception: {e}");
        }
    }
    //public void Load(string filename = "SaveGame.xml") //Defaults to SaveGame.xml if no file name is provided during function call.
    //{
    //    string path = Path.Combine(Application.persistentDataPath, filename); //"Path.Combine" will safely combine folder and filename into a valid path ** Folder being the platform safe persistant storage area for unity.
    //    XmlSerializer serializer = new XmlSerializer(typeof(GameStateData)); //xmlSerializer is C# builtin class that handles XML, "Type of" is the target object type. 
    //    //So th above line should create a xml object called serializer and it should store a new XML object that will be converted to a (User defined) GameStateData object. Still empty I think.
    //    using (FileStream fileStream = new FileStream(path, FileMode.Open)) //"Using" ensures the file is closed after reading, "FileStream" handles the actualy reading of the file(Takes the valid file path from earlier,
    //    { //"FileMode.Open" is responsible for opening the file and throwing arrors if it doesn't exist.
    //        //Below Should set gameState "defined above" = to the resulting GameStateData object as a result of deserializing the fileStream variable.
    //        gameState = serializer.Deserialize(fileStream) as GameStateData; //Deserialize passes in an object of type. **Base C# object. "as GameStateData" is know as casting...
    //    }
    //    Debug.Log($"LoadSaveManager: load Function has returned the following object: {gameState}");
    //}

    #region Helper Functions //Need to better understand this before professional implementation!!
    // Helper: Encrypt string to bytes with AES (prepends random IV)
    private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key)
    {
        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.Mode = CipherMode.CBC;
        aesAlg.GenerateIV(); // Random IV

        using MemoryStream msEncrypt = new MemoryStream();
        msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length); // Write IV first

        using (var csEncrypt = new CryptoStream(msEncrypt, aesAlg.CreateEncryptor(), CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        return msEncrypt.ToArray();
    }

    // Helper: Decrypt bytes to string with AES (reads IV from first bytes)
    private static string DecryptStringFromBytes_Aes(byte[] cipherTextCombined, byte[] Key)
    {
        using MemoryStream msDecrypt = new MemoryStream(cipherTextCombined);

        byte[] iv = new byte[16];
        msDecrypt.Read(iv, 0, iv.Length); // Read IV from start

        using Aes aesAlg = Aes.Create();
        aesAlg.Key = Key;
        aesAlg.IV = iv;
        aesAlg.Mode = CipherMode.CBC;

        using var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
    #endregion
}