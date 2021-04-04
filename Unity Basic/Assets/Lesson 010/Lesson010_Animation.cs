using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson010_Animation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A)) {
            var ani = GetComponent<Animation>();
            if (ani) {
                ani.CrossFade("Anim2", 1);
            }
        }
    }

    void OnAnimEvent(int value) {
        Debug.Log("OnAnimEvent " + value);
    }

    void OnAnimEventObject(Object v) {
        var obj = v as Lesson009_ScriptableObject;
        if (obj) {
            Debug.Log("OnAnimEventObject " + obj.gizmosColor);
        }
    }

}
