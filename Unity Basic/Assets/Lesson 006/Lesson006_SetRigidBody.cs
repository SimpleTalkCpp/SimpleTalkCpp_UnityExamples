using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson006_SetRigidBody : MonoBehaviour
{
    Rigidbody _rigidbody;

    public Vector3 velocity;
    public Vector3 angularVelocity;
    public Vector3 centerOfMass;


    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody) {
            _rigidbody.velocity = velocity;
            _rigidbody.angularVelocity = angularVelocity;
            _rigidbody.centerOfMass = centerOfMass;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void FixedUpdate()
	{
		
	}
}
