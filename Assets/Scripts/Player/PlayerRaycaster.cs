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
