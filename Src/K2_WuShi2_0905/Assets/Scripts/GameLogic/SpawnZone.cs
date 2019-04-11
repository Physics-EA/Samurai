using UnityEngine;using System.Collections;using System.Collections.Generic;
using Assets.Scripts.GameData;[RequireComponent(typeof(BoxCollider))]public class SpawnZone : MonoBehaviour{
    /*  敌人的生成机制：
     *  1.分回合制，每回合生成固定数量的敌人;
     *  2.当前回合的敌人全部打败后才会有下一回合的敌人产生
     *  3.确定敌人的生成点
     *  4.所有敌人打败后，才会完成当前区域，可以进入到下一个区域。
     */

    [System.Serializable]
    public class RoundInfo
    {
        [System.Serializable]
        //  敌人信息类的属性抽象
        //  1.敌人类型；2.敌人的生成点；3.敌人生成的时间间隔；4.敌人自否始终朝向玩家 5.死亡后是否继续生成
        public class SpawnInfo
        {
            public E_EnemyType EnemyType;
            public SpawnPointEnemy[] SpawnPoint;
            public float SpawnDelay = 0;
            public bool RotateToPlayer = true;
            public bool WhenKilledStopSpawn = false;
        }

        public SpawnInfo[] Spawns;
        public float SpawnDelay = 0;
        public int MinEnemiesFomLastRound = 0;
    }

    public RoundInfo[] SpawnRounds;
    public SpawnPointEnemy[] SpawnPoints = null;
    public PadLock LockIn = null;
    public PadLock LockOut = null;
    public bool nextLevel = false;

    private GameObject RootEnemy;
    private GameObject GameObject;
    //  用List来记录活着敌人的信息
    [HideInInspector]
    public List<Agent> EnemiesAlive = new List<Agent>();	public bool IsActive() { return EnemiesAlive.Count > 0; }
    public Agent GetEnemy(int index) { return EnemiesAlive[index]; }
    public int GetEnemyCount() { return EnemiesAlive.Count; }

    public enum E_State
    {
        E_WAITING_FOR_START,
        E_SPAWNING_ENEMIES,
        E_IN_PROGRESS,
        E_FINISHED,
    }     public E_State State = E_State.E_WAITING_FOR_START;	void Awake()	{        GameObject = gameObject;
        RootEnemy = GameObject.Find("Enemy");	}

    public void Enable()
    {
        //Debug.Log(GameObject.name + " Enable");

        if (LockIn != null)
            LockIn.Unlock();
        if (LockOut != null)
            LockOut.Unlock();
    }     // We'll draw a gizmo in the scene view, so it can be found....    void OnDrawGizmos()    {        BoxCollider b = GetComponent<BoxCollider>();        if(b != null)        {            Gizmos.color = Color.cyan;            Gizmos.DrawWireCube(b.transform.position + b.center, b.size );        }        if(SpawnPoints != null)        {
            for (int i = 0; i < SpawnPoints.Length; i++)
            {
                if (b != null)
                   Gizmos.DrawLine(b.transform.position + b.center, SpawnPoints[i].transform.position);
                else
                    Gizmos.DrawLine(GameObject.transform.position , SpawnPoints[i].transform.position);
            }        }    }

    #region 测试代码

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < GetEnemyCount(); i++)
            {
                Destroy(EnemiesAlive[i].gameObject);
            }

            if (LockOut != null)
                LockOut.Unlock();
        }
    }

    #endregion

    // Update is called once per frame	void FixedUpdate()	{
        //  更新活着敌人的信息
        for (int i = EnemiesAlive.Count - 1; i >= 0; i--)
        {
            if (EnemiesAlive[i].IsAlive)
            {
                continue;
            }

            EnemiesAlive.RemoveAt(i);
        }        if (State != E_State.E_IN_PROGRESS)            return;		if (EnemiesAlive.Count == 0)		{            State = E_State.E_FINISHED;

            if (LockOut != null)                LockOut.Unlock();

            Player.Instance.GetComponent<AudioSource>().PlayDelayed(2.5f);
            //  打完这波玩家血量回满
            Player.Instance.HealToFullHealth();

            if (nextLevel)
            {
                GameController.Instance.LoadToNextLevel();
            }		}	}	public void Restart()	{
       // Debug.Log(GameObject.name + " Restart");		StopAllCoroutines();
        State = E_State.E_WAITING_FOR_START;

        if (LockIn != null)
            LockIn.Reset();
        if (LockOut != null)
            LockOut.Reset();
    }
    void OnTriggerEnter(Collider other)    {
        if (State != E_State.E_WAITING_FOR_START || other != Player.Instance.Agent.CharacterController)
            return;

        State = E_State.E_IN_PROGRESS;

        Player.Instance.GetComponent<AudioSource>().Stop();
        Player.Instance.currentSpawnZone = this;
        Player.Instance.comboHitNum = 0;

        if (SpawnRounds != null && SpawnRounds.Length > 0)
        {
           StartCoroutine(SpawnEnemiesInRounds());
        }
       
        else
        {
            StartCoroutine(SpawnEnemies());
        }

        // 上锁
        if (LockIn != null)
            LockIn.Lock();
        if (LockOut != null)
            LockOut.Lock();
    }

    IEnumerator SpawnEnemies()
    {
        State = E_State.E_SPAWNING_ENEMIES;

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < SpawnPoints.Length; i++)
        {
            yield return new WaitForSeconds(0.2f);

            //if (SpawnPoints[i].Difficulty > Game.Instance.GameDifficulty)
            //    continue;

            StartCoroutine(SpawnEnemy(SpawnPoints[i]));

            yield return new WaitForSeconds(Random.Range(0.1f, 0.4f));
        }

        yield return new WaitForSeconds(4.0f);

        State = E_State.E_IN_PROGRESS;
    }

    IEnumerator SpawnEnemy(SpawnPointEnemy spawnpoint)
    {
        CombatEffectsManager.Instance.PlaySpawnEffect(spawnpoint.Transform.position, spawnpoint.Transform.forward);

        yield return new WaitForSeconds(0.1f);

        //  根据怪物点，怪物的类型来实例化对应的怪物
        Agent enemy = GenerateEnemyOnType(spawnpoint.EnemyType, spawnpoint.Transform);

        if (enemy == null)
        {
            yield break;
        }

        enemy.PrepareForStart();
        enemy.SoundPlaySpawn();

        if (RootEnemy != null)
        {
            enemy.transform.SetParent(RootEnemy.transform);
        }
        
        if (spawnpoint.SpawnAnimation != null)
            enemy.GetComponent<Animation>().CrossFade(spawnpoint.SpawnAnimation.name,0.02f);
            
        EnemiesAlive.Add(enemy);
    }

    IEnumerator SpawnEnemiesInRounds()
    {
        State = E_State.E_SPAWNING_ENEMIES;

        Agent ImportantAgent = null;

        for (int i = 0; i < SpawnRounds.Length; i++)
        {
            RoundInfo round = SpawnRounds[i];

            float delay = round.SpawnDelay;

            while (delay > 0)
            {
                //  重要的boss杀死后，就直接结束产怪（一般是Boss）
                if (ImportantAgent != null && ImportantAgent.IsAlive == false)
                {
                    State = E_State.E_IN_PROGRESS;
                    yield break;
                }

                if (EnemiesAlive.Count == 0 || EnemiesAlive.Count <= round.MinEnemiesFomLastRound)
                    break;  // dont wait, when enemies are killed or less then required 

                yield return new WaitForSeconds(0.5f);
                delay -= 0.5f;
            }

            for (int j = 0; j < round.Spawns.Length; j++)
            {
                RoundInfo.SpawnInfo spawnInfo = round.Spawns[j];

                yield return new WaitForSeconds(spawnInfo.SpawnDelay);

                SpawnPointEnemy spawnpoint = GetAvailableSpawnPoint(spawnInfo.SpawnPoint == null || spawnInfo.SpawnPoint.Length == 0 ? SpawnPoints : spawnInfo.SpawnPoint);

                //  如果怪出生朝向玩家，就调整出生方位
                if (spawnInfo.RotateToPlayer)
                {
                    Vector3 dir = Player.Instance.transform.position - spawnpoint.Transform.position;
                    dir.Normalize();
                    spawnpoint.Transform.forward = dir;
                }

                Agent enemy = GenerateEnemyOnType(spawnInfo.EnemyType, spawnpoint.Transform);

                while (enemy == null)
                {
                    yield return new WaitForSeconds(0.2f);

                    enemy = GenerateEnemyOnType(spawnInfo.EnemyType, spawnpoint.Transform);                  
                }

                CombatEffectsManager.Instance.PlaySpawnEffect(spawnpoint.Transform.position, spawnpoint.Transform.forward);

                enemy.PrepareForStart();
                enemy.SoundPlaySpawn();

                if (RootEnemy != null)
                {
                    enemy.transform.SetParent(RootEnemy.transform);
                }

                EnemiesAlive.Add(enemy);

                if (spawnInfo.WhenKilledStopSpawn)
                    ImportantAgent = enemy;

                yield return new WaitForSeconds(0.1f);
            }
        }

        State = E_State.E_IN_PROGRESS;
    }

    SpawnPointEnemy GetAvailableSpawnPoint(SpawnPointEnemy[] spawnPoints)
    {
        Vector3 pos = Player.Instance.transform.position;

        float bestValue = 0;
        int bestSpawn = -1;

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            float value = 0;
            float dist = Mathf.Min(14, (spawnPoints[i].Transform.position - pos).magnitude);
            value = Mathfx.Hermite(0, 7, dist / 7);

            // Debug.Log(i + " Spawnpoint " + spawnPoints[i].name + " dist " + dist + " Value " + value);
            if (value <= bestValue)
                continue;

            bestValue = value;
            bestSpawn = i;
        }

        //Debug.Log("Best spaqwn point is " + bestSpawn);

        if (bestSpawn == -1)
            return spawnPoints[Random.Range(0, spawnPoints.Length)];

        return spawnPoints[bestSpawn];
    }

    //  根据不同的怪物类型来生成对应的怪物
    private Agent GenerateEnemyOnType(E_EnemyType type, Transform t)
    {
        switch (type)
        {            
            case E_EnemyType.SwordsMan:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemySwordsman");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
            case E_EnemyType.Peasant:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyPeasant");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
            case E_EnemyType.TwoSwordsMan:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyDoubleSwordsman");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
            case E_EnemyType.Bowman:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyBowman");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }

            case E_EnemyType.PeasantLow:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyPeasantEasy");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
                
            case E_EnemyType.MiniBoss01:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyMiniBoss");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
            case E_EnemyType.SwordsManLow:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemySwordsmanEasy");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
            case E_EnemyType.BossOrochi:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyBossOrochi");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
            default:
                {
                    Agent prefabEnemy = Resources.Load<Agent>("Prefabs/Enemy/EnemyPeasantEasy");
                    Agent enemy = Instantiate(prefabEnemy, t.position, t.rotation) as Agent;
                    return enemy;
                    break;
                }
        }    }}