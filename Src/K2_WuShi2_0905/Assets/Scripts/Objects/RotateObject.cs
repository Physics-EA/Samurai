using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour 
{
    public float RotationSpeedX;
    public float RotationSpeedY;
    public float RotationSpeedZ;

    private Transform MyTransform;
    private Renderer Mesh;

    void Awake()
    {
        MyTransform = transform;
        Mesh = GetComponent<Renderer>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Mesh != null && Mesh.isVisible == false)
            return;
        MyTransform.Rotate(RotationSpeedX * Time.deltaTime, RotationSpeedY * Time.deltaTime, RotationSpeedZ * Time.deltaTime);
    }
}
