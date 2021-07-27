using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PogoStickController : MonoBehaviour
{
    private Rigidbody rb;
    
    private bool isGrounded = false;
    
    public float jumpForce = 350f;
    public float speed = 5f;
    public float turnSpeed = 10f;

    public GameObject spring;
    public GameObject upperBody;
    public GameObject playerCamera;

    public int springCount = 1; 
    
    
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
        if (other.gameObject.CompareTag("Boost"))
        {
            StartCoroutine(SpeedUp());
        }

        if (other.gameObject.CompareTag("Pogo"))
        {
            StartCoroutine(MultiplyPogo(Int16.Parse(other.gameObject.GetComponentInChildren<Text>().text)));
        }
    }
    
    
    IEnumerator SpeedUp() 
    {
        speed *= 2f;

        yield return new WaitForSeconds(3f);

        speed /= 2f;
    }

    IEnumerator MultiplyPogo(int add)
    {   
        springCount += add;

        for(int i=0; i<add; i++)
        {
            GameObject temp = Instantiate(spring,
                new Vector3(spring.transform.position.x, spring.transform.position.y + 0.1f, spring.transform.position.z), spring.transform.rotation);

            upperBody.transform.position = new Vector3(upperBody.transform.position.x,
                upperBody.transform.position.y + 0.1f, upperBody.transform.position.z);
        
            temp.transform.parent = gameObject.transform;
            spring = temp;

            if(springCount > 2)
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition,new Vector3(playerCamera.transform.localPosition.x , playerCamera.transform.localPosition.y + (add * 0.2f), playerCamera.transform.localPosition.z - (add * 0.2f)), 0.5f);
            }
            yield return new WaitForSeconds(0.1f);
        }

    }
    
    
}
