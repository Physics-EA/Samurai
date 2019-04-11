using UnityEngine;
using System.Collections.Generic;

public class CameraOffsetBehaviour : MonoBehaviour
{
    public Vector3 DefaultOffset = new Vector3(0, 8, -4);

    GameObject Offset;
    Transform OffsetTransform;
    Transform MyTransform;

    // Use this for initialization
    void Awake()
    {
        MyTransform = transform;

        Offset = new GameObject("CameraOffset");
        OffsetTransform = Offset.transform;
        OffsetTransform.position = OffsetTransform.TransformPoint(DefaultOffset);
    }

    public Vector3 GetCameraPosition()
    {
        return OffsetTransform.position * 0.9f + MyTransform.position;
    }

    void OnTriggerEnter(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log(this.ToString() + " exit " + other.ToString());
    }

    /// 
    void Activate(Transform t)
    {
        OffsetTransform.position = t.TransformDirection(DefaultOffset);
        CameraBehaviour.Instance.Reset();
    }

    void Deactivate()
    {

    }

}
