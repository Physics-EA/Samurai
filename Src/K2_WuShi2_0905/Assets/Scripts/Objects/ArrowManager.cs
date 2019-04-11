using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager :MonoSingleton<ArrowManager> 
{
    private Arrow prefabArrow;

    void Awake()
    {
        Instance = this;

        prefabArrow = Resources.Load<Arrow>("Prefabs/ProjectileArrow");
    }

    public void SpawnArrow(Agent owner,Vector3 pos, Vector3 dir, float speed, float damage)
    {
        Arrow arrow = Instantiate<Arrow>(prefabArrow);

        arrow.Init(owner,pos, dir, speed, damage);
    }
	
}
