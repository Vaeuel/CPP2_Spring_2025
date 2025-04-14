using System;
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]

public class PlayerControl : MonoBehaviour
{
    public static event Action GameOverZoneEntered;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector3(hInput * 10, rb.linearVelocity.y, vInput * 10);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "GameOver")
        {
            GameOverZoneEntered?.Invoke();
        }
    }
}
