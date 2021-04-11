using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MyTriangle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CreateTriangle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("CreateTriangle")]
    void CreateTriangle() {
        var mesh = new Mesh();

        var v0 = new Vector3(0,0,0);
        var v1 = new Vector3(0,1,0);
        var v2 = new Vector3(1,1,0);

        var vertices = new Vector3[] { v0, v1, v2 };

        mesh.vertices = vertices;
        mesh.colors = new Color[] {
            new Color(1,0,0),
            new Color(0,1,0),
            new Color(0,0,1)
        };

        var normal = Vector3.Cross(v0 - v1, v0 - v2).normalized; 

        mesh.normals = new Vector3[] { normal, normal, normal };

        mesh.triangles = new int[]{0,1,2};

        var mf = GetComponent<MeshFilter>();
        mf.sharedMesh = mesh;
    }
}
