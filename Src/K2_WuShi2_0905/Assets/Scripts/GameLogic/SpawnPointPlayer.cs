using UnityEngine;using System.Collections;public class SpawnPointPlayer : SpawnPoint{
    public Transform player;
    // Use this for initialization    void Start()    {        enabled = true;
        //  初始化玩家的位置
        player.position = transform.position;    }    // We'll draw a gizmo in the scene view, so it can be found....    void OnDrawGizmos()    {        Gizmos.color = Color.blue;        Gizmos.DrawSphere(transform.position, 0.5f);          }}