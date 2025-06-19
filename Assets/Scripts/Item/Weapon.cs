using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]

public class Weapon : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;

    private void Start()
    {
        rb = GetComponent <Rigidbody>();
        bc = GetComponent <BoxCollider>();
    }

    public void Equip(Collider playerCollider, Transform weaponAttackPoint)
    {
        //Setting ridgid body to kinematic to ignore physics
        rb.isKinematic = true;
        bc.isTrigger = true;
        transform.SetParent(weaponAttackPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Physics.IgnoreCollision(playerCollider, bc);
    }

    public void Drop(Collider playerCollider, Vector3 playerForward)
    {
        transform.parent = null;
        rb.isKinematic = false;
        bc.isTrigger = false;
        rb.AddForce(playerForward * 10, ForceMode.Impulse);
        StartCoroutine(DropCoolDown(playerCollider));
    }

    private void OnTriggerEnter(Collider other)
    {
        string targetTag = other.gameObject.tag;
        //Debug.Log($"Collision detected on weapon: {targetTag}");
        if (other.gameObject.CompareTag("Boo"))
        {
            Physics.IgnoreCollision(other, bc, true);

            AnimationManager animMan = other.gameObject.GetComponentInChildren<AnimationManager>();
            if (animMan != null)
            {
                animMan.Death(targetTag);
            }

            StartCoroutine(DropCoolDown(other));
        }
    }

    IEnumerator DropCoolDown(Collider playerCollider)
    {
        yield return new WaitForSeconds(2);

        //Enable collisions
        if (playerCollider != null && bc != null)
        {
            Physics.IgnoreCollision(playerCollider, bc, false);
        }
    }
}