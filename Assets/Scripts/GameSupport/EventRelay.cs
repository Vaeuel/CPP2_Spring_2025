using System.Reflection;
using UnityEngine;

public class EventRelay : MonoBehaviour
{
    [Tooltip("Drag the Parent Object with the script that needs to be called.")]
    [SerializeField] private MonoBehaviour targetScript;

    [Tooltip("Name of the method to call (must be public and use exact spelling).")]
    [SerializeField] private string methodName;

    private MethodInfo cachedMethod;

    private void Start()
    {
        if (targetScript == null) Debug.LogWarning("Relay failed: missing target.");   
        
        if (cachedMethod == null) Debug.LogWarning($"EventRelay: Method not found on {targetScript.GetType().Name}.");

        if (targetScript != null && !string.IsNullOrEmpty(methodName)) //Should only work if the designer has added BOTH components required
        {
            cachedMethod = targetScript.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); //gets method info from methodName if assigned.
        }
    }

    public void Relay() // Called by animation events
    {
        if (cachedMethod != null)
        {
            cachedMethod.Invoke(targetScript, null);
        }

        else
        {
            Debug.LogWarning("EventRelay: Method not cached or invalid.");
        }
    }
}
