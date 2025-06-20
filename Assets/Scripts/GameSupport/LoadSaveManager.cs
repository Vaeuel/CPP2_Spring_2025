using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LoadSaveManager : MonoBehaviour
{
    [XmlRoot("GameStateData")]
    public class GameStateData
    {
        
        public struct DataTrasform
        {
            public float posX;
            public float posY;
            public float posZ;
            public float rotX;
            public float rotY;
            public float rotZ;
            public float scaleX;
            public float scaleY;
            public float scaleZ;
        }

        public class DataEnemy
        {

        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
