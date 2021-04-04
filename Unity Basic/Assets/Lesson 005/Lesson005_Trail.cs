using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson005_Trail : MonoBehaviour
{
    public GameObject target;
    public float dampingTime = 0.5f;

    Vector3 _velocity;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target) {
            var newPos = Vector3.SmoothDamp(transform.position, target.transform.position, ref _velocity, dampingTime);
            transform.position = newPos;
        }
    }
}
