using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PogoStickController : MonoBehaviour
{
    private Rigidbody rb;
    
    private bool isGrounded = false;
    
    public float jumpForce = 350f;
    public float speed = 5f;
    public float turnSpeed = 10f;

    
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * Time.deltaTime * turnSpeed);
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed + new Vector3(0.0f, rb.velocity.y, 0.0f);
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
