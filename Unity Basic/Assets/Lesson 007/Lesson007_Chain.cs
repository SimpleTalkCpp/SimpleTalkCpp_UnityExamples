using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson007_Chain : MonoBehaviour
{
    public int NodeCount = 16;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0) {
            var src = transform.GetChild(0);

            Rigidbody lastRigidBody = src.GetComponent<Rigidbody>();

            for (int i = 1; i < NodeCount; i++) {
                var obj = Instantiate(src);
                obj.transform.SetParent(transform, false);
                obj.transform.localPosition = new Vector3(0,0, 1.2f * i);

                var joint = obj.GetComponent<Joint>();
                joint.connectedBody = lastRigidBody;

                lastRigidBody = obj.GetComponent<Rigidbody>();
            }
        }
    }
}
