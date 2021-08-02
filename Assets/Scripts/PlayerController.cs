using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator;
    
    private bool isGrounded = false;
    private bool isHolding = false;
    
    public float jumpForce = 350f;
    public float speed = 5f;
    public float turnSpeed = 10f;
    public float slingUpForce = 50f;
    public float slingForwardForce = 350f;

    public GameObject spring;
    public GameObject upperBody;
    public GameObject playerCamera;
    public GameObject mainSpring;
    public GameObject confetti;

    private int score = 0;

    private String name = null;
    
    public Stack<GameObject> pogos;
    
    public int springCount = 1;
    private float timer = 0f;

    private int stackNo;
    private bool isTouched = false;

    private Vector3 firstpoint; //change type on Vector3
    private Vector3 secondpoint;
    private float xAngle; //angle for axes x for rotation
    private float xAngTemp = 0.0f; //temp variable for angle

    
    private void Start()
    {
        pogos = new Stack<GameObject>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        xAngle = 0.0f;
        this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
    }

    private void Update()
    {
        if(Input.touchCount > 0) {
        //Touch began, save position
        if(Input.GetTouch(0).phase == TouchPhase.Began) {
            firstpoint = Input.GetTouch(0).position;
            xAngTemp = xAngle;
            //yAngTemp = yAngle;
        }
        //Move finger by screen
        if(Input.GetTouch(0).phase==TouchPhase.Moved) {
            secondpoint = Input.GetTouch(0).position;
            //Mainly, about rotate camera. For example, for Screen.width rotate on 180 degree
            xAngle = xAngTemp + (secondpoint.x - firstpoint.x) * 180.0f / Screen.width;
            //Rotate camera
            this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
        }
    }   

        //transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * Time.deltaTime * turnSpeed);              

        

        if (Input.GetButton("Jump") || Input.GetMouseButton(0) && springCount > 1)
        {
            animator.enabled = false;
            isHolding = true;
            if (isGrounded)
            {
                timer += 4f * Time.deltaTime;
                rb.velocity = Vector3.zero;
                speed = 0f;
            }
        }

        if (Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(0))
        {
            animator.enabled = false;
            
            if (springCount > 1 && isGrounded)
            {
                stackNo = Math.Min((int) timer, springCount);
                if (stackNo == 0 && isGrounded)
                {
                    stackNo = 1;
                }
                
                rb.AddForce(stackNo * slingUpForce * Vector3.up);
                rb.AddForce(stackNo * slingForwardForce * Vector3.forward, ForceMode.Acceleration);
                DestroyStack(stackNo);
            }

            timer = 0f;
            isHolding = false;
            speed = 5f;
        }



        if (springCount == 1)
        {
            spring = mainSpring;
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
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
            animator.enabled = false;
            animator.enabled = true;
            animator.Play(0);
            
        }
        
        if (other.gameObject.CompareTag("Water"))
        {
            SceneManager.LoadScene(0);
        }

        if (other.gameObject.CompareTag("Platform"))
        {
            isTouched = true;
            if (name != other.gameObject.name)
            {
                isTouched = false;
            }
                
            name = other.gameObject.name;

            if (!isTouched)
            {
                StartCoroutine(MultiplyPogo(4));
            }
        }
        
        if (other.gameObject.name == "EndgameGround")
        {
            StartCoroutine(EndGame());
        }
        
    }
    

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
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
        
        switch (other.gameObject.name)
        {
            case "Line1":
                score = 100;
                Instantiate(confetti);
                break;
            case "Line2":
                score = 200;
                Instantiate(confetti);
                break;
            case "Line3":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line4":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line5":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line6":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line7":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line8":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line9":
                score = 300;
                Instantiate(confetti);
                break;
            case "Line10":
                score = 300;
                Instantiate(confetti);
                break;
            
            
            
            default:
                score = 0;
                break;
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

    IEnumerator EndGame()
    {
        Debug.Log("Your score is:" + score);
        rb.velocity = Vector3.zero;
        speed = 0f;


        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(0);
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

                if (pogos.Count > 0)
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
