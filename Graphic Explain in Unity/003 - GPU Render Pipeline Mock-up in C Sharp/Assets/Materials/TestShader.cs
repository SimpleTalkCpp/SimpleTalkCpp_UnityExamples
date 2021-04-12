using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestShader : MonoBehaviour
{
    void Update()
    {
        var light = GetComponent<Light>();

        if (light) {
            Shader.SetGlobalVector("_WorldSpaceLightPos0", -light.transform.forward);
        }
    }
}
