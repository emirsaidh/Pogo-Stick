using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    private Rigidbody rb;

    public float speed = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed ;
        rb.AddForce(Vector3.down * 9.81f);
    }
    
}
