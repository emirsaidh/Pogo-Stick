using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePogo : MonoBehaviour
{
    [SerializeField]
    private float turnSpeed;
    public List<GameObject> pogoStacks = new List<GameObject>();

    void Update()
    {
        for(int i = 0; i < pogoStacks.Count - 1; i++){
            pogoStacks[i].transform.Rotate(new Vector3(0f,turnSpeed,0f));
        }
        
    }
}