using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SimpleCharacterController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 5f;
    private Animator animator;

    private bool isGrounded = false;

    public float turnSpeed = 10f;
    
    public GameObject pogoStick = null;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            animator.SetBool("isJumpPressed", true);
            rb.AddForce(Vector3.up*700f);
        }
        
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("isJumpPressed", false);
        }

       transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * Time.deltaTime * turnSpeed);
        
        
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed + new Vector3(0.0f, rb.velocity.y, 0.0f);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (other.gameObject.CompareTag("Water"))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pogo"))
        {
            pogoStick.SetActive(true);
            pogoStick.transform.position = transform.position;
            pogoStick.transform.rotation = transform.rotation;
            gameObject.SetActive(false);
        }

        if (other.gameObject.CompareTag("Boost"))
        {
            StartCoroutine(SpeedUp());
        }
    }

    IEnumerator SpeedUp()
    {
        speed *= 2f;

        yield return new WaitForSeconds(3f);

        speed /= 2f;
    }
    
}

