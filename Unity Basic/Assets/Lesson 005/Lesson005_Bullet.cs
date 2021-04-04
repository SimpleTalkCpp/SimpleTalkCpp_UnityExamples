using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Lesson005_Bullet : MonoBehaviour
{
    static Transform group;

    public float speed = 8;
    public float lifespan = 3;

    float _lifespanRemain;
    public GameObject model;

    void Start()
    {
        if (!group) {
            group = new GameObject("-- Bullets --").transform;
        }
        transform.SetParent(group, false);

        _lifespanRemain = lifespan;

        model = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        model.name = "Model";
        model.transform.SetParent(transform, false);
        model.transform.localScale = Vector3.one * 0.5f;
    }

    void Update()
    {
        _lifespanRemain -= Time.deltaTime;
        if (_lifespanRemain <= 0) {
            Destroy(gameObject);
        }
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
