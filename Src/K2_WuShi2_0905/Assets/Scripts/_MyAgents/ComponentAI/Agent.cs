using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GameData;

public class Agent : MonoBehaviour 
{
    private E_CurrentStatus status = E_CurrentStatus.E_Idle;
    public E_CurrentStatus Status { get; set; }

    public bool canBlock = false;   //  是否有格挡技能

    public int Experience = 0;  //  经验值
    public int Attack = 30;     //  攻击力
    public Material TransparentMaterial;
    public Material DiffuseMaterial;
    public AudioClip[] KnockDownSounds = null;
    public AudioClip[] SpawnSounds = null;
    public AudioClip[] PrepareAttackSounds = null;
    public AudioClip[] BerserkSounds = null;
    public AudioClip[] StepSounds = null;
    public AudioClip[] RollSounds = null;

    public AudioClip[] AttackMissSounds = null;
    public AudioClip[] AttackHitSounds = null;
    public AudioClip[] AttackBlockSounds = null;

    public AudioClip WeaponOn = null;
    public AudioClip WeaponOff = null;

    [System.NonSerialized]
    public AnimSet AnimSet;

    public BlackBoard BlackBoard = new BlackBoard();// { get { return BlackBoard; } private set { BlackBoard = value; } }

    [System.NonSerialized]
    public WorldState WorldState;// { get { return WorldState; } private set { WorldState = value; } }

    [System.NonSerialized]
    public CharacterController CharacterController;

    [System.NonSerialized]
    public Transform Transform;
    [System.NonSerialized]
    public GameObject GameObject;
    [System.NonSerialized]
    public AudioSource Audio;

    private SkinnedMeshRenderer Renderer;

    [System.NonSerialized]
    public E_EnemyType EnemyType = E_EnemyType.None;

    public bool IsAlive { get { return BlackBoard.Health > 0 && GameObject.activeSelf; } }
    public bool IsPlayer { get { return BlackBoard.IsPlayer; } }
    public bool IsVisible { get { return Renderer.isVisible; } }
    public bool IsAttacking { get { return false; } }
    public Vector3 Position { get { return Transform.position; } }
    public Vector3 Forward { get { return Transform.forward; } }
    public Vector3 Right { get { return Transform.right; } }

    public bool IsInvulnerable { get { return BlackBoard.Invulnerable; } }
    public bool IsBlocking { get { return BlackBoard.MotionType == E_MotionType.Block || BlackBoard.MotionType == E_MotionType.BlockingAttack; } }
    public bool IsKnockedDown { get { return BlackBoard.MotionType == E_MotionType.Knockdown && BlackBoard.KnockDownDamageDeadly; } }

    public Vector3 ChestPosition { get { return Transform.position + transform.up * 1.5f; } }

    public AudioClip KnockDownSound { get { if (KnockDownSounds == null || KnockDownSounds.Length == 0) return null; return KnockDownSounds[Random.Range(0, KnockDownSounds.Length)]; } }
    public AudioClip SpawnSound { get { if (SpawnSounds == null || SpawnSounds.Length == 0) return null; return SpawnSounds[Random.Range(0, SpawnSounds.Length)]; } }
    public AudioClip StepSound { get { if (StepSounds == null || StepSounds.Length == 0) return null; return StepSounds[Random.Range(0, StepSounds.Length)]; } }
    public AudioClip RollSound { get { if (RollSounds == null || RollSounds.Length == 0) return null; return RollSounds[Random.Range(0, RollSounds.Length)]; } }
    public AudioClip PrepareAttackSound { get { if (PrepareAttackSounds == null || PrepareAttackSounds.Length == 0) return null; return PrepareAttackSounds[Random.Range(0, PrepareAttackSounds.Length)]; } }
    public AudioClip BerserkSound { get { if (BerserkSounds == null || BerserkSounds.Length == 0) return null; return BerserkSounds[Random.Range(0, BerserkSounds.Length)]; } }
    public AudioClip AttackMissSound { get { if (AttackMissSounds == null || AttackMissSounds.Length == 0) return null; return AttackMissSounds[Random.Range(0, AttackMissSounds.Length)]; } }
    public AudioClip AttackHitSound { get { if (AttackHitSounds == null || AttackHitSounds.Length == 0) return null; return AttackHitSounds[Random.Range(0, AttackHitSounds.Length)]; } }
    public AudioClip AttackBlockSound { get { if (AttackBlockSounds == null || AttackBlockSounds.Length == 0) return null; return AttackBlockSounds[Random.Range(0, AttackBlockSounds.Length)]; } }

    private Vector3 CollisionCenter;

    void Awake()
    {
        Transform = transform;
        GameObject = gameObject;
        Audio = GetComponent<AudioSource>();
        CharacterController = Transform.GetComponent<CharacterController>();
        CollisionCenter = CharacterController.center;

        BlackBoard.Owner = this;
        BlackBoard.myGameObject = GameObject;

        AnimSet = GetComponent<AnimSet>();

        WorldState = new WorldState();

        ResetAgent();

        WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

        WorldState.SetWSProperty(E_PropKey.E_IDLING, true);
        WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
        WorldState.SetWSProperty(E_PropKey.E_ATTACK_TARGET, false);
        WorldState.SetWSProperty(E_PropKey.E_LOOKING_AT_TARGET, false);
        WorldState.SetWSProperty(E_PropKey.E_USE_WORLD_OBJECT, false);
        WorldState.SetWSProperty(E_PropKey.E_PLAY_ANIM, false);

        WorldState.SetWSProperty(E_PropKey.E_IN_DODGE, false);
        WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);
        WorldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
        WorldState.SetWSProperty(E_PropKey.E_IN_BLOCK, false);
        WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
        WorldState.SetWSProperty(E_PropKey.E_IN_COMBAT_RANGE, false);
        WorldState.SetWSProperty(E_PropKey.E_AHEAD_OF_ENEMY, false);
        WorldState.SetWSProperty(E_PropKey.E_BEHIND_ENEMY, false);
        WorldState.SetWSProperty(E_PropKey.MoveToRight, false);
        WorldState.SetWSProperty(E_PropKey.MoveToLeft, false);
        WorldState.SetWSProperty(E_PropKey.E_TELEPORT, false);

        WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);
    }

    void Start()
    {
        //Debug.Log("start");
        Renderer = GameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
    }

    void Activate(Transform t)
    {
        //Debug.Log(name + " Aactivate");

        Reset();

        RaycastHit hit;
        if (Physics.Raycast(t.position + Vector3.up, -Vector3.up, out hit, 5, 1 << 10) == false)
            Transform.position = t.position;
        else
            Transform.position = hit.point;

        Transform.rotation = t.rotation;

        WorldState.SetWSProperty(E_PropKey.E_ORDER, AgentOrder.E_OrderType.E_NONE);

        WorldState.SetWSProperty(E_PropKey.E_IDLING, true);
        WorldState.SetWSProperty(E_PropKey.E_AT_TARGET_POS, true);
        WorldState.SetWSProperty(E_PropKey.E_IN_DODGE, false);
        WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
        WorldState.SetWSProperty(E_PropKey.E_ATTACK_TARGET, false);

        WorldState.SetWSProperty(E_PropKey.E_IN_WEAPONS_RANGE, false);
        WorldState.SetWSProperty(E_PropKey.E_WEAPON_IN_HANDS, false);
        WorldState.SetWSProperty(E_PropKey.E_PLAY_ANIM, false);
        WorldState.SetWSProperty(E_PropKey.E_IN_BLOCK, false);
        WorldState.SetWSProperty(E_PropKey.E_ALERTED, false);
        WorldState.SetWSProperty(E_PropKey.E_IN_COMBAT_RANGE, false);
        WorldState.SetWSProperty(E_PropKey.E_AHEAD_OF_ENEMY, false);
        WorldState.SetWSProperty(E_PropKey.E_BEHIND_ENEMY, false);
        WorldState.SetWSProperty(E_PropKey.MoveToRight, false);
        WorldState.SetWSProperty(E_PropKey.MoveToLeft, false);

        WorldState.SetWSProperty(E_PropKey.E_TELEPORT, false);

        WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);

        StartCoroutine(FadeIn());

        SoundPlay(SpawnSound);

    }

    void Deactivate()
    {
        //Debug.Log(name + " Deactivate");
        StopAllCoroutines();
        BlackBoard.Reset();
    }

    void LateUpdate()
    {
        if (IsPlayer == false)
        {
            return;
        }

        UpdateAgent();
    }
    void FixedUpdate()
    {
        if (IsPlayer)
            return;

        UpdateAgent();
    }

    void UpdateAgent()
    {
        if (BlackBoard.DontUpdate == true)
            return;

        //update blackboard
        BlackBoard.Update();
    }

    public void PrepareForStart()
    {
        BlackBoard.Reset();
    }

    // could be called after death.. when agent should disappear
    void ResetAgent()
    {
        WorldState.SetWSProperty(E_PropKey.E_EVENT, E_EventTypes.None);

        StopAllCoroutines();
        BlackBoard.Reset();
        WorldState.Reset();
        //BlackBoard.GameObject.SetActiveRecursively(false);
    }

    public void PlayAnim(string animName)
    {
        if (animName != null)
        {
            BlackBoard.DesiredAnimation = animName;
            WorldState.SetWSProperty(E_PropKey.E_PLAY_ANIM, true);
        }
    }

    public void Teleport(Transform destination)
    {
        Transform.position = destination.position;
        Transform.rotation = destination.rotation;
    }

    void SpawnBlood()
    {
        //  SpriteEffectsManager.Instance.CreateBloodSlatter(Transform, 1, 3);
    }

    protected IEnumerator FadeIn()
    {
        if (TransparentMaterial == null)
            yield break;

        yield return new WaitForEndOfFrame();

        //Material old = Renderer.material;

        Renderer.material = TransparentMaterial;

        Color color = new Color(1, 1, 1, 0);
        TransparentMaterial.SetColor("_Color", color);

        while (color.a < 1)
        {
            color.a += Time.deltaTime * 4;
            if (color.a > 1)
                color.a = 1;

            TransparentMaterial.SetColor("_Color", color);
            yield return new WaitForEndOfFrame();
        }

        color.a = 1;
        TransparentMaterial.SetColor("_Color", color);

        Renderer.material = DiffuseMaterial;
    }
    protected IEnumerator Fadeout(float delay)
    {
        if (TransparentMaterial == null)
            yield break;

        yield return new WaitForSeconds(delay);

        CombatEffectsManager.Instance.PlayDisappearEffect(Transform.position, Transform.forward);

       // SpriteEffectsManager.Instance.ReleaseShadow(GameObject);

        //Material old = Renderer.material;

        Renderer.material = TransparentMaterial;

        Color color = new Color(1, 1, 1, 1);
        TransparentMaterial.SetColor("_Color", color);

        while (color.a > 0)
        {
            color.a -= Time.deltaTime * 4;
            if (color.a < 0)
                color.a = 0;

            TransparentMaterial.SetColor("_Color", color);
            yield return new WaitForEndOfFrame();
        }

        color.a = 0;
        TransparentMaterial.SetColor("_Color", color);

        // Mission.Instance.ReturnHuman(GameObject);
    }

    public void Reset()
    {
        if (TransparentMaterial != null)
        {
            if (Renderer == null)
                Renderer = (gameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer);

            Renderer.material = TransparentMaterial;
            //Renderer.material = TransparentMaterial;

            Color color = new Color(1, 1, 1, 0);
            TransparentMaterial.SetColor("_Color", color);
        }
        ResetAgent();

        EnableCollisions();
    }

    public float GetCriticalChance()
    {
        return 18;
    }

    public void SoundPlay(AudioClip clip)
    {
        if (clip)
            Audio.PlayOneShot(clip);
    }

    public void SoundPlaySpawn()
    {
        SoundPlay(SpawnSound);
    }

    public void SoundPlayStep()
    {
        SoundPlay(StepSound);
    }

    public void SoundPlayRoll()
    {
        SoundPlay(RollSound);
    }

    public void SoundPlayKnockdown()
    {
        SoundPlay(KnockDownSound);
    }

    public void SoundPlayBerserk()
    {
        SoundPlay(BerserkSound);
    }

    public void SoundPlayHit()
    {
        SoundPlay(AttackHitSound);
    }

    public void SoundPlayMiss()
    {
        SoundPlay(AttackMissSound);
    }

    public void SoundPlayBlockHit()
    {
        SoundPlay(AttackBlockSound);
    }

    public void SoundPlayPrepareAttack()
    {
        SoundPlay(PrepareAttackSound);
    }

    public void SoundPlayWeaponOff()
    {
        SoundPlay(WeaponOff);
    }

    public void PlayLoopSound(AudioClip clip, float delay, float time, float fadeInTime, float fadeOutTime)
    {
        StartCoroutine(_PlayLoopSound(clip, delay, time, fadeInTime, fadeOutTime));
    }

    IEnumerator _PlayLoopSound(AudioClip clip, float delay, float time, float fadeInTime, float fadeOutTime)
    {
        Audio.volume = 0;
        Audio.loop = true;
        Audio.clip = clip;

        yield return new WaitForEndOfFrame();

        yield return new WaitForSeconds(delay);

        Audio.Play();

        float step = 1 / fadeInTime;
        while (Audio.volume < 1)
        {
            Audio.volume = Mathf.Min(1.0f, Audio.volume + step * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(time - fadeInTime - fadeOutTime);

        step = 1 / fadeInTime;
        while (Audio.volume > 0)
        {
            Audio.volume = Mathf.Max(0.0f, Audio.volume - step * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        Audio.Stop();

        yield return new WaitForEndOfFrame();

        Audio.volume = 1;
    }


    public void DisableCollisions()
    {
        CharacterController.detectCollisions = false;
        CharacterController.center = Vector3.up * -20;
    }

    public void EnableCollisions()
    {
        CharacterController.detectCollisions = true;
        CharacterController.center = CollisionCenter;
    }
}
