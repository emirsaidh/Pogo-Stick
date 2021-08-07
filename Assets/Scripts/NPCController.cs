using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NPCController : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private Transform targetWayPoint;
    private int targetWayPointIndex = 0;
    private Rigidbody rb;
    private float minDistance = 0.5f;
    private bool isGrounded = false;
    private float lastWayPointIndex;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float jumpForce = 100.0f;

    public GameObject spring;
    public GameObject upperBody;
    public GameObject mainSpring;
    public Stack<GameObject> pogos;
    public GameObject waypointsParent;
    public int springCount = 1;
    private float timer = 0f;
    private int stackNo;


    void Start()
    {
        lastWayPointIndex = waypoints.Count - 1;
        targetWayPoint = waypoints[targetWayPointIndex];
        pogos = new Stack<GameObject>();
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();
    }


    void Update()
    {
        float movementStep = speed * Time.deltaTime;
        float rotationStep = speed * Time.deltaTime;

        Vector3 directionToTarget = targetWayPoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, rotationToTarget, rotationStep);

        float distance = Vector3.Distance(transform.position, targetWayPoint.position);
        CheckDistanceToWaypoint(distance);

        transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, movementStep);

        waypointsParent.transform.position = new Vector3(waypointsParent.transform.position.x, transform.position.y - 1.0f, waypointsParent.transform.position.z);


    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce);
            //waypointsParent.transform.position = new Vector3(waypointsParent.transform.position.x, transform.position.y, waypointsParent.transform.position.z);
        }
    }







    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
        {
            isGrounded = true;
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            Debug.Log("Collider works");
            speed = 0;
            jumpForce = 0;
            isGrounded = false;
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
        speed *= 1.25f;

        yield return new WaitForSeconds(3f);

        speed /= 1.25f;
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

            yield return new WaitForSeconds(0.1f);
        }
    }













    void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance <= minDistance)
        {
            targetWayPointIndex++;
            UpdateTargetWayPoint();
        }
    }

    void UpdateTargetWayPoint()
    {
        if (targetWayPointIndex > lastWayPointIndex)
        {
            targetWayPointIndex = 0;
        }
        targetWayPoint = waypoints[targetWayPointIndex];
    }


}
