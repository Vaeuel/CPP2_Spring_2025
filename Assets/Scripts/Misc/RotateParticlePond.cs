using UnityEngine;

public class RotateParticlePond : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f; //speed of rotation

    void Start()
    {
        if (rotationSpeed <=0)
        {
            Debug.LogWarning("ROtation speed needs to be greater than 0. setting speed to default of 10.");
            rotationSpeed = 10;
        }
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed *  Time.deltaTime);
    }
}
