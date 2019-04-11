using UnityEngine;using System.Collections;using System.Collections.Generic;
using Assets.Scripts.GameData;[RequireComponent(typeof(BoxCollider))]public class SpawnZone : MonoBehaviour{
    /*  ���˵����ɻ��ƣ�
     *  1.�ֻغ��ƣ�ÿ�غ����ɹ̶������ĵ���;
     *  2.��ǰ�غϵĵ���ȫ����ܺ�Ż�����һ�غϵĵ��˲���
     *  3.ȷ�����˵����ɵ�
     *  4.���е��˴�ܺ󣬲Ż���ɵ�ǰ���򣬿��Խ��뵽��һ������
     */

    [System.Serializable]
    public class RoundInfo
    {
        [System.Serializable]
        //  ������Ϣ������Գ���
        //  1.�������ͣ�2.���˵����ɵ㣻3.�������ɵ�ʱ������4.�����Է�ʼ�ճ������ 5.�������Ƿ��������
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
    //  ��List����¼���ŵ��˵���Ϣ
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

    #region ���Դ���

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
        //  ���»��ŵ��˵���Ϣ
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
            //  �����Ⲩ���Ѫ������
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

        // ����
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

        //  ���ݹ���㣬�����������ʵ������Ӧ�Ĺ���
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
                //  ��Ҫ��bossɱ���󣬾�ֱ�ӽ������֣�һ����Boss��
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

                //  ����ֳ���������ң��͵���������λ
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

    //  ���ݲ�ͬ�Ĺ������������ɶ�Ӧ�Ĺ���
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