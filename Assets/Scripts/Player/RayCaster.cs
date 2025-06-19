#region Previous attempts
//using UnityEngine;

//public class PlayerRaycaster : MonoBehaviour
//{
//    [SerializeField] private float rayDistance = 20f;
//    [SerializeField] private LayerMask booLayer;

//    private void Update()
//    {
//        Ray ray = new Ray(transform.position, transform.forward);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, rayDistance, booLayer))
//        {
//            BooController boo = hit.collider.GetComponent<BooController>();
//            if (boo != null)
//            {
//                boo.SetPlayerIsLooking(true);
//            }
//        }
//        else
//        {
//            // Optional: Only if you want the Boo to know when NOT being looked at.
//            BooController[] allBoos = FindObjectsOfType<BooController>();
//            foreach (BooController boo in allBoos)
//            {
//                boo.SetPlayerIsLooking(false);
//            }
//        }
//    }
//}
//using UnityEngine;

//public class PlayerRaycaster : MonoBehaviour
//{
//    [SerializeField] private float rayDistance = 20f;
//    [SerializeField] private LayerMask raycastLayer; // "RayCast" Layer

//    private BooController[] allBoos;

//    private void Start()
//    {
//        // Cache all Boos at start (assumes Boo doesn't spawn/despawn mid-game)
//        allBoos = Object.FindObjectsByType<BooController>(FindObjectsSortMode.None);
//    }

//    private void Update()
//    {
//        Ray ray = new Ray(transform.position, transform.forward);
//        RaycastHit hit;

//        if (Physics.Raycast(ray, out hit, rayDistance, raycastLayer))
//        {
//            // Check what was hit
//            if (hit.collider.CompareTag("Boo"))
//            {
//                BooController boo = hit.collider.GetComponent<BooController>();
//                if (boo != null)
//                {
//                    boo.SetPlayerIsLooking(true);
//                }
//            }
//            else
//            {
//                // Hit something else (Wall, Switch, etc.)
//                DisableAllBoos();
//            }
//        }
//        else
//        {
//            // Nothing hit — player is looking into empty space
//            DisableAllBoos();
//        }
//    }

//    private void DisableAllBoos()
//    {
//        foreach (BooController boo in allBoos)
//        {
//            boo.SetPlayerIsLooking(false);
//        }
//    }
//}
#endregion

using System.Runtime.CompilerServices;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    [Header("Collision mark")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Transform rayCOP;
    [SerializeField] private int distance = 10;

    private GameObject lastDetected;

    private void Start()
    {
        if (!rayCOP) rayCOP = this.gameObject.transform; // Could be shortended even further to just = transform; but I don't like reading it that way!
    }


    void Update()
    {
        Ray ray = new Ray(rayCOP.transform.position, this.transform.forward); //Sets origin point manually in inspector, and always casts forward of this game object
        RaycastHit hitInfo; //will hold function defined variables based on if true (Trans,Point,normal,etc.)

        Debug.DrawLine(rayCOP.transform.position, rayCOP.transform.position + (this.transform.forward * distance), Color.red); //This will draw a line in the editor from start to end position

        if (Physics.Raycast(ray, out hitInfo, distance, collisionMask))
        {
            GameObject detected = hitInfo.transform.root.gameObject; //Gets the top-most parent of the object hit
            //Debug.Log($"RayCaster: Ray caster has detected {detected.name}");
            GameObject raycasterRoot = transform.root.gameObject; //Gets the top-most parent of the object doing the detection


            if (detected != lastDetected) //this will prevent unneccessary messaging. Once Ondetected is called it doesn't need to be called again
            {
                if (lastDetected != null) //If the target has changed since initialization then this will run
                {
                    lastDetected.SendMessage("LostLOS", raycasterRoot, SendMessageOptions.DontRequireReceiver);
                }

                //This should only run once when a valid target is found.
                detected.SendMessage( //SendMessage broadcasts to the detected root transforms game object.
                   "OnDetected", raycasterRoot, //"OnDected" is the name of the method within the root game object, and we're passing our detectors info to it.
                    SendMessageOptions.DontRequireReceiver); //This will remove any error if the function doesn't exist
                lastDetected = detected; //This should prevent the entire If statement from running continuously
            }

            Debug.Log(hitInfo.transform.gameObject); //We don't have access to the game object hit but we can find it through the transform values we do get
        }

        else if (lastDetected != null) //This should run once Raycast fails if a valid target was previously marked
        {
            GameObject raycasterRoot = transform.root.gameObject; //Gets the top-most parent of the object doing the detection
            lastDetected.SendMessage("LostLOS", raycasterRoot, SendMessageOptions.DontRequireReceiver); //provides the raycasters info to the last targeted object through the LostLOS function
            lastDetected = null; //Resets the lastDetected variable.
        }

        #region Alternative detection logic
        /*This logic can be done more efficiently by using and interface and searching for scripts with the required interface:

        public interface IPrimaryLogic
        {
            void OnDetected(GameObject detector);
        }

        public class EnemyAI : MonoBehaviour, IPrimaryLogic
        {
            public void OnDetected(GameObject detector)
            {
                Debug.Log($"{name} detected by {detector.name}");
            }
        }

        IPrimaryLogic mainScript = detectedRoot.GetComponent<IPrimaryLogic>(); //This chunk would belong to the rayCaster script.
        if (mainScript != null)
        {
            mainScript.OnDetected(raycasterRoot);
        }

         */
        #endregion
    }

}
