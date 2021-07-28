using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PogoStickController : MonoBehaviour
{
    private Rigidbody rb;
    
    private bool isGrounded = false;
    private bool isHolding = false;
    
    public float jumpForce = 350f;
    public float speed = 5f;
    public float turnSpeed = 10f;
    public float slingForce = 50f;

    public GameObject spring;
    public GameObject upperBody;
    public GameObject playerCamera;
    public GameObject mainSpring;
    
    public Stack<GameObject> pogos;
    
    public int springCount = 1;
    private float timer = 0f;

    private int stackNo;
    private void Start()
    {
        pogos = new Stack<GameObject>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * Time.deltaTime * turnSpeed);

        if (Input.GetButton("Jump") && springCount > 1)
        {
            isHolding = true;
            if (isGrounded)
            {
                timer += 4f * Time.deltaTime;
                rb.velocity = Vector3.zero;
                speed = 0f;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (springCount > 1 && isGrounded)
            {
                stackNo = Math.Min((int) timer, springCount);
                if (stackNo == 0 && isGrounded)
                {
                    stackNo = 1;
                }
                
                rb.AddForce(stackNo * slingForce * (Vector3.forward+Vector3.up+Vector3.forward).normalized);
                DestroyStack(stackNo);
            }

            timer = 0f;
            isHolding = false;
            speed = 5f;

            
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * speed + new Vector3(0.0f, rb.velocity.y, 0.0f);
        if (isGrounded && !isHolding)
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

        if (other.gameObject.CompareTag("Stack"))
        {
            Destroy(other.gameObject);
            StartCoroutine(MultiplyPogo(1));
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
            pogos.Push(temp);

            upperBody.transform.position = new Vector3(upperBody.transform.position.x,
                upperBody.transform.position.y + 0.1f, upperBody.transform.position.z);
        
            temp.transform.parent = gameObject.transform;
            spring = temp;

            
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x , playerCamera.transform.localPosition.y + 0.2f, playerCamera.transform.localPosition.z - 0.2f);
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void DestroyStack(int destNo)
    {
        Debug.Log(destNo);
        for (int i = 0; i < destNo; i++)
        {
            if (pogos.Count != 0)
            {
                Destroy(pogos.Pop());
                upperBody.transform.position = new Vector3(upperBody.transform.position.x, upperBody.transform.position.y - 0.1f, upperBody.transform.position.z);
                
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x , playerCamera.transform.localPosition.y - 0.2f, playerCamera.transform.localPosition.z +  0.2f);

                if (pogos.Count > 1)
                {
                    spring = pogos.Peek();
                }
            }
        }

        if (destNo != 1)
        {
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x , playerCamera.transform.localPosition.y - 0.2f, playerCamera.transform.localPosition.z +  0.2f);
        }
        
        if (pogos.Count == 0)
        {
            spring = mainSpring;
        }
        springCount -= destNo;
    }

}
