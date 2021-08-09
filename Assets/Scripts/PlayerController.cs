using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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
    public TextMeshProUGUI coinsEarned;


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
    private int currentLevel = 0;
    private Boolean doubleTouch;
    [SerializeField]
    private GameObject levelEndCanvas;

    public Transform upperTransform;


    bool right;
    bool left;
    [SerializeField] float min = -1f;
    [SerializeField] float max = 1f;

    public float speed_touch = 2f;


    private void Start()
    {
        pogos = new Stack<GameObject>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        xAngle = 0.0f;
        //this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
    }

    private void Update()
    {
        /*if (Input.touchCount > 0)
        {
            //Touch began, save position
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                firstpoint = Input.GetTouch(0).position;
                xAngTemp = xAngle;
            }
            //Move finger by screen
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                secondpoint = Input.GetTouch(0).position;
                //Mainly, about rotate camera. For example, for Screen.width rotate on 180 degree
                xAngle = xAngTemp + (secondpoint.x - firstpoint.x) * 180.0f / Screen.width;
                //Rotate camera
                this.transform.rotation = Quaternion.Euler(0.0f, xAngle, 0.0f);
            }
        }*/

        // TOUCH FUNCTION

        TouchMovement();

        transform.Rotate(Input.GetAxis("Horizontal") * Vector3.up * Time.deltaTime * turnSpeed);



        //Input.GetButton("Jump") || 
        /*if (doubleTouch && springCount > 1)
        {
            animator.enabled = false;
            isHolding = true;
            if (!isGrounded)
            {
                springY = spring.transform.position.y;
                float heightDiff = springY - 1.5f;
                transform.position = new Vector3(transform.position.x, transform.position.y - heightDiff, transform.position.z);
                isGrounded = true;
            }
            if (isGrounded)
            {
                timer += 4f * Time.deltaTime;
                rb.velocity = Vector3.zero;
                speed = 0f;
            }
        }*/


        /*if (Input.GetButton("Jump") && isGrounded)
        {
                animator.SetBool("isHolding", true);
                timer += 4f * Time.deltaTime;
                rb.velocity = Vector3.zero;
                speed = 0f;
        }
        
        if (Input.GetButtonUp("Jump") || Input.GetMouseButtonUp(0))
        {
            animator.enabled = false;

            if (springCount > 1 && isGrounded)
            {
                stackNo = Math.Min((int)timer, springCount);
                if (stackNo == 0 && isGrounded)
                {
                    stackNo = 1;
                }

                rb.AddForce(stackNo * slingUpForce * Vector3.up);
                rb.AddForce(stackNo * slingForwardForce * Vector3.forward, ForceMode.Acceleration);
                DestroyStack(stackNo);
            }

            timer = 0f;
            animator.SetBool("isHolding", false);
            isHolding = false;
            speed = 5f;
        }*/


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
            SceneManager.LoadScene(currentLevel);
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

        if (other.gameObject.CompareTag("Finish"))
        {
            rb.velocity = Vector3.zero;
            speed = 0f;
            score = Int16.Parse(other.gameObject.GetComponentInChildren<Text>().text);
            Debug.Log(score);
            coinsEarned.text = score.ToString();
            currentLevel++;
            Instantiate(confetti, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z + 1.0f), transform.rotation);
            Instantiate(confetti, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z + 1.0f), transform.rotation);
            Instantiate(confetti, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z + 1.0f), transform.rotation);
            levelEndCanvas.SetActive(true);

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

        for (int i = 0; i < add; i++)
        {
            GameObject temp = Instantiate(spring,
            new Vector3(spring.transform.position.x, spring.transform.position.y + 0.1f, spring.transform.position.z), spring.transform.rotation);
            pogos.Push(temp);

            upperBody.transform.position = new Vector3(upperBody.transform.position.x,
                upperBody.transform.position.y + 0.1f, upperBody.transform.position.z);

            temp.transform.parent = gameObject.transform;
            spring = temp;


            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y + 0.2f, playerCamera.transform.localPosition.z - 0.2f);

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator EndGame()
    {
        rb.velocity = Vector3.zero;
        speed = 0f;
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(currentLevel);
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

                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y - 0.2f, playerCamera.transform.localPosition.z + 0.2f);

                if (pogos.Count > 0)
                {
                    spring = pogos.Peek();
                }
            }
        }

        if (destNo != 1)
        {
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, playerCamera.transform.localPosition.y - 0.2f, playerCamera.transform.localPosition.z + 0.2f);
        }

        if (pogos.Count == 0)
        {
            spring = mainSpring;
        }

        springCount -= destNo;
    }

    public static bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 1;
        float VariancePosition = 1;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
                result = true;
        }
        return result;
    }

    private void TouchMovement()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {

            Touch finger = Input.GetTouch(0);

            // RIGHT-LEFT MOVEMENT STARTED

            Vector3 go_right = new Vector3(max, 0, 0);
            Vector3 go_left = new Vector3(min, 0, 0);

            if (finger.deltaPosition.x > 1.0f)
            {
                right = true;
                left = false;
            }

            if (finger.deltaPosition.x < -1.0f)
            {
                right = false;
                left = true;
            }

            transform.Rotate(finger.deltaPosition.x * Vector3.up * Time.deltaTime * turnSpeed);

            // RIGHT-LEFT MOVEMENT ENDED


        }
    }

    public void ReadySwing()
    {
        if (springCount > 1)
        {
            animator.enabled = false;
            isHolding = true;
            if (isGrounded)
            {
                upperTransform.localRotation = Quaternion.Slerp(upperTransform.localRotation, Quaternion.Euler(Mathf.Clamp(upperTransform.localRotation.x - ((springCount - 1) * 5f), -50f, 0f), upperTransform.localRotation.y, upperTransform.localRotation.z), 2 * Time.deltaTime);
                timer += 4f * Time.deltaTime;
                rb.velocity = Vector3.zero;
                speed = 0f;
            }
        }
    }

    public void StopSwing()
    {
        animator.enabled = false;
        upperTransform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (springCount > 1 && isGrounded)
        {
            stackNo = Math.Min((int)timer, springCount);
            if (stackNo == 0 && isGrounded)
            {
                stackNo = 1;
            }

            rb.AddForce(stackNo * slingUpForce * Vector3.up);
            rb.AddForce(stackNo * slingForwardForce * Vector3.forward, ForceMode.Acceleration);
            DestroyStack(stackNo);
        }

        timer = 0f;
        animator.SetBool("isHolding", false);
        isHolding = false;
        speed = 5f;
    }

    public void LoadNextLevel()
    {
        StartCoroutine(EndGame());
    }
}
