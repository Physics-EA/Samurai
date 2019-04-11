using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveRandom : MonoBehaviour 
{
    [Header("particle Attributes")]
    [Tooltip("左边界")]
    public float left;
    [Tooltip("右边界")]
    public float right;
    [Tooltip("上边界")]
    public float up;
    [Tooltip("下边界")]
    public float down;
    [Tooltip("移动速度")]
    public float moveSpeed;
    [Tooltip("移动时间间隔")]
    public float invertalTime;
    //  起始时间
    private float startTime = 0;
    private Vector3 targetPos;
	// Update is called once per frame
	void Update () 
	{
        startTime += Time.deltaTime;

        if (startTime >= invertalTime)
        {
            startTime = 0;
            targetPos = new Vector3(Random.Range(left, right), Random.Range(down, up), transform.position.z);       
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
	}
}
