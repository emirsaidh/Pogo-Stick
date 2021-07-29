using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();
    private Transform targetWayPoint;
    private int targetWayPointIndex = 0;
    private float minDistance = 0.5f;
    private float lastWayPointIndex;
    [SerializeField]
    private float speed = 3.0f;
    [SerializeField]
    private float rotationSpeed = 2.0f;

    // 
    // private Rigidbody rb;   
    // private bool isGrounded = false;   
    // public float jumpForce = 350f;
    // public float turnSpeed = 10f;
    // public float slingForce = 50f;
    // public GameObject spring;
    // public GameObject upperBody;
    // public GameObject mainSpring;   
    // public Stack<GameObject> pogos;    
    // public int springCount = 1;
    // private float timer = 0f;
    // private int stackNo;

    void Start()
    {
        lastWayPointIndex = waypoints.Count - 1;
        targetWayPoint = waypoints[targetWayPointIndex];

        // pogos = new Stack<GameObject>();
        // rb = GetComponent<Rigidbody>();

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

        // 
        // if (isGrounded)
        // {
        //     rb.AddForce(Vector3.up * jumpForce);
        // }
        
    }

    //
    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Ground"))
    //     {
    //         Debug.Log("Collision Detected");
    //         isGrounded = true;
    //     }
        
    //     if (other.gameObject.CompareTag("Water"))
    //     {
    //         SceneManager.LoadScene(0);
    //     }
    // }

    // private void OnCollisionExit(Collision other)
    // {
    //     if (other.gameObject.CompareTag("Ground"))
    //     {
    //         isGrounded = false;
    //     }
    // }

    void CheckDistanceToWaypoint(float currentDistance){
        if(currentDistance <= minDistance){
            targetWayPointIndex++;
            UpdateTargetWayPoint();
        }
    }

    void UpdateTargetWayPoint(){
        if(targetWayPointIndex > lastWayPointIndex){
            targetWayPointIndex = 0;
        }
        targetWayPoint = waypoints[targetWayPointIndex];
    }
}
