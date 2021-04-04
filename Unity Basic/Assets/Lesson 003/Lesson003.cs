using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lesson003 : MonoBehaviour
{
    public float _speed = 15;

    public float m_TestingAbc;
    public int   intValue;
    public double doubleValue;
    public Vector3 vectorValue;

    float _privateValue = 100; // only for code
    float privateValue {
        get { return _privateValue; }
        set { _privateValue = value; }
    }

    float _autoProp { get; set; } // only for code

    [UnityEngine.SerializeField]
    float _serializeFieldValue = 1; // Edited by game designer
    float serializeFieldValue => _serializeFieldValue;

    [System.Serializable]
    public struct MyData {
        public string name; // as element label in Inspector
        public int x, y;
    }
    public MyData myData;
    public MyData[] myDataArray = new MyData[2];
    public List<MyData> myDataList = new List<MyData>();



    void Start()
    {
        Application.targetFrameRate = 60;

        Debug.Log("Hello World");
//      Debug.LogWarning("Warn");
//      Debug.LogError("Error");
    }

    void Update()
    {
        //Debug.Log($"Update frameCount={Time.frameCount} deltaTime={Time.deltaTime}");
//      Transform t = GetComponent("Transform") as Transform;
//      Transform t = GetComponent(typeof(Transform)) as Transform;
        Transform t = transform;

        // rotate along Y
		Quaternion rot = t.localRotation;
        Vector3 e = rot.eulerAngles;
        e.y += Time.deltaTime * 90;
        t.localRotation = Quaternion.Euler(e);

        // move Z
		//Vector3 pos = t.localPosition;
		//pos.z += Time.deltaTime * _speed;
		//t.localPosition = pos;

        // move forward
		Vector3 pos = t.localPosition;
        var old_pos = pos;
		pos += transform.forward * Time.deltaTime * _speed;
		t.localPosition = pos;


        Debug.DrawLine(old_pos, pos, new Color(1,0,0), 2);
    }
}
