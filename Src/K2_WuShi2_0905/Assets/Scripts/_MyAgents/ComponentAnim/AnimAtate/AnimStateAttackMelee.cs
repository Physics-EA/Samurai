using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.GameData;

public class AnimStateAttackMelee : AnimState
{
    enum E_State
    {
        E_PREPARING,
        E_ATTACKING,
        E_FINISHED,
    }

    ActionAttack Action;
    AnimAttackData AnimAttackData;

    Quaternion FinalRotation;
    Quaternion StartRotation;
    Vector3 StartPosition;
    Vector3 FinalPosition;
    float CurrentRotationTime;
    float RotationTime;
    float MoveTime;
    float CurrentMoveTime;
    float EndOfStateTime;
    float HitTime;
    float AttackPhaseTime;

    bool RotationOk = false;
    bool PositionOK = false;
   // bool MovingToAttackPos;

    bool Critical = false;
    bool Knockdown = false;

    E_State State;

	public AnimStateAttackMelee(Animation anims, Agent owner): base(anims, owner)
	{

	}
    override public void OnActivate(ActionBase action)
    {
        /*if(Owner.IsPlayer == false)
            Time.timeScale = 0.2f;*/
        base.OnActivate(action);
    }

    override public void OnDeactivate()
    {
        Action.SetSuccess();
        Action = null;
        
        base.OnDeactivate();
    }


    override public bool HandleNewAction(ActionBase action)
    {
		 if (action as ActionAttack != null)
		 {
             if (Action != null)
             {
                 Action.AttackPhaseDone = true;
                 Action.SetSuccess();
             }

             Initialize(action);
			 return true;
		 }
		 return false;
    }

    override public void Update()
    {
        if (State == E_State.E_PREPARING)
        {
            bool dontMove = false;
            if (RotationOk == false)
            {
                //Debug.Log("rotate");
                CurrentRotationTime += Time.deltaTime;

                if (CurrentRotationTime >= RotationTime)
                {
                    CurrentRotationTime = RotationTime;
                    RotationOk = true;
                }

                float progress = CurrentRotationTime / RotationTime;
                Quaternion q = Quaternion.Lerp(StartRotation, FinalRotation, progress);
                Owner.transform.rotation = q;

                if (Quaternion.Angle(q, FinalRotation) > 20.0f)
                    dontMove = true;
            }

            if (dontMove == false && PositionOK == false)
            {
                CurrentMoveTime += Time.deltaTime;
                if (CurrentMoveTime >= MoveTime)
                {
                    CurrentMoveTime = MoveTime;
                    PositionOK = true;
                }

                if (CurrentMoveTime > 0)
                {
                    float progress = CurrentMoveTime / MoveTime;
                    Vector3 finalPos = Mathfx.Hermite(StartPosition, FinalPosition, progress);
                    //if (MoveToCollideWithEnemy(finalPos, Transform.forward) == false)
                    if (Move(finalPos - Transform.position) == false)
                    {
                        PositionOK = true;
                    }
                }
            }

            if (RotationOk && PositionOK)
            {
                State = E_State.E_ATTACKING;
                PlayAnim();
            }       
        }
        else if (State == E_State.E_ATTACKING)
        {
            CurrentMoveTime += Time.deltaTime;

            if (AttackPhaseTime < Time.timeSinceLevelLoad)
            {
                //Debug.Log(Time.timeSinceLevelLoad + " attack phase done");
                Action.AttackPhaseDone = true;
                State = E_State.E_FINISHED;             
            }

            if (CurrentMoveTime >= MoveTime)
               CurrentMoveTime = MoveTime;

            if (CurrentMoveTime > 0 && CurrentMoveTime <= MoveTime)
            {
                float progress = Mathf.Min(1.0f, CurrentMoveTime / MoveTime);
                Vector3 finalPos = Mathfx.Hermite(StartPosition, FinalPosition, progress);
                //if (MoveToCollideWithEnemy(finalPos, Transform.forward) == false)
                if (Move(finalPos - Transform.position, false) == false)
                {
                    CurrentMoveTime = MoveTime;
                }

               // Debug.Log(Time.timeSinceLevelLoad + " moving");
            }

            if(Action.Hit == false && HitTime <= Time.timeSinceLevelLoad)
            {
                Action.Hit = true;

                if (Owner.IsPlayer && AnimAttackData.FullCombo)
                {
                    //  在游戏UI上显示满的连招信息
                    Owner.SoundPlayKnockdown();
                    MainPanelCtrl.Instance.SkillShowMessage(AnimAttackData.ComboIndex);
                }

                //  显示攻击的光影效果
                if (AnimAttackData.LastAttackInCombo)
                {
                    Owner.StartCoroutine(ShowTrail(AnimAttackData, 1, 0.3f, Critical, MoveTime - Time.timeSinceLevelLoad));
                }              
                else
                {
                    Owner.StartCoroutine(ShowTrail(AnimAttackData, 2, 0.1f, Critical, MoveTime - Time.timeSinceLevelLoad));
                }
                
                //Debug.Log("DoMeleeDamage  " + (Action.AttackTarget != null ? Action.AttackTarget.name : "no target"));

                if (Action.Target == null)
                {
                    Owner.SoundPlayMiss();
                }

                if (Action.Target != null)
                {
                    //  当攻击完成，如果当前的攻击目标是敌人
                    //  通过当前的攻击位置，攻击方向和攻击范围，敌人是否仍在此攻击范围内，如果在就受到伤害，如果不在就不会受到伤害
                    //  当前的攻击位置和方位
                    //  就是计算攻击完成后当前怪物和人的相对位置，通过相对位置来判断

                    if (Owner.IsPlayer)
                    {
                        List<Agent> enemies = Player.Instance.currentSpawnZone.EnemiesAlive;
                        Agent enemy;
                        Vector3 dirToEnemy;

                        if (enemies==null || enemies.Count <= 0)
                        {
                            Owner.SoundPlayMiss();
                            return;
                        }

                        for (int i = 0; i < enemies.Count; i++)
                        {
                            enemy = enemies[i];
                            dirToEnemy = enemy.transform.position - Owner.Transform.position;

                            if (dirToEnemy.magnitude < Owner.BlackBoard.WeaponRange + 0.2f)
                            {
                                if (Vector3.Angle(Owner.Forward, dirToEnemy) < 50)
                                {
                                    //  判断敌人是否背对玩家，如果是就一刀必杀,慢动作，直接死(角度在120之内)
                                    //  根据是横向还是纵向中刀，判断不同的死亡动画
                                    if (Vector3.Angle(Owner.Forward,enemy.Forward) < 90)
                                    {
                                        Debug.Log("一刀必杀");
                                        Owner.SoundPlayKnockdown();
                                        MainPanelCtrl.Instance.ShowBloodPanel();
                                        Player.Instance.comboHitNum++;
                                        Player.Instance.coin += enemy.Experience;
                                        MainPanelCtrl.Instance.ShowComboMessage(Player.Instance.comboHitNum);

                                        // Action.Data.HitCriticalType
                                       
                                        enemy.BlackBoard.Health = 0;
                                        enemy.Status = E_CurrentStatus.E_Death;
                                        ActionDeath actionDeath = ActionFactory.Create(ActionFactory.E_Type.E_DEATH) as ActionDeath;
                                        enemy.GetComponent<AnimComponent>().HandleAction(actionDeath);
                                        CombatEffectsManager.Instance.PlayCriticalEffect(enemy.Transform.position, enemy.Forward);

                                        return;
                                    }
                                    //  技能格挡 
                                    else if (Vector3.Angle(Owner.Forward,enemy.Forward) > 165 && enemy.canBlock)
                                    {
                                        //  2/3概率格挡成功
                                        if (Random.Range(0,3) < 2)
                                        {
                                            ActionBlock actionBlock = ActionFactory.Create(ActionFactory.E_Type.E_BLOCK) as ActionBlock;
                                            actionBlock.Attacker = Owner;
                                            enemy.GetComponent<AnimComponent>().HandleAction(actionBlock);
                                            enemy.Status = E_CurrentStatus.E_Block;

                                            Owner.SoundPlayBlockHit();
                                            CombatEffectsManager.Instance.PlayBlockHitEffect(enemy.Transform.position, enemy.Forward);

                                            return;
                                        }                                    
                                    }

                                    Owner.SoundPlayHit();
                                    enemy.BlackBoard.Health -= Owner.Attack;

                                    //  判断敌人是否死亡,主UI面板显示血液
                                    if (enemy.BlackBoard.Health <= 0)
                                    {
                                        Player.Instance.coin += enemy.Experience;
                                        enemy.BlackBoard.Health = 0;
                                        enemy.Status = E_CurrentStatus.E_Death;
                                        ActionDeath actionDeath = ActionFactory.Create(ActionFactory.E_Type.E_DEATH) as ActionDeath;
                                        enemy.GetComponent<AnimComponent>().HandleAction(actionDeath);
                                        CombatEffectsManager.Instance.PlayBloodBigEffect(enemy.Transform.position, enemy.Forward);                                     
                                    }
                                    else
                                    {
                                        ActionInjury injury = ActionFactory.Create(ActionFactory.E_Type.E_INJURY) as ActionInjury;
                                        injury.DamageType = E_DamageType.Front;
                                        injury.Impuls = (dirToEnemy).normalized;

                                        enemy.GetComponent<AnimComponent>().HandleAction(injury);
                                        //  将敌人的状态切回Idle，不然切不回去
                                        enemy.WorldState.SetWSProperty(E_PropKey.E_IDLING, true);
                                        enemy.Status = E_CurrentStatus.E_Injury;

                                        //  播放流血效果
                                        CombatEffectsManager.Instance.PlayBloodEffect(enemy.Transform.position, enemy.Forward);
                                    }

                                    Player.Instance.comboHitNum++;
                                    MainPanelCtrl.Instance.ShowComboMessage(Player.Instance.comboHitNum);
                                }
                                else
                                {
                                    Owner.SoundPlayMiss();
                                }
                            }
                        }
                    }
                    //   如果目标是玩家
                    else
                    {
                        Vector3 dirToPlayer = Player.Instance.transform.position - Owner.Transform.position;

                        if (dirToPlayer.magnitude < Owner.BlackBoard.WeaponRange - 0.5f)
                        {
                            if (Vector3.Angle(dirToPlayer,Owner.Forward) < 40)
                            {
                                ActionInjury injury = ActionFactory.Create(ActionFactory.E_Type.E_INJURY) as ActionInjury;
                                injury.Impuls = (dirToPlayer).normalized;

                                Owner.SoundPlayHit();
             
                                Player.Instance.GetComponent<AnimComponent>().HandleAction(injury);
                                Player.Instance.Owner.BlackBoard.Health -= Owner.Attack;
                                Player.Instance.comboHitNum = 0;
                                MainPanelCtrl.Instance.ShowComboMessage(Player.Instance.comboHitNum);
                            }                         
                        }
                    }
                }

                //  判断玩家是否打中木桶的
                //  首先需要获得到当前场景中的所有木桶对象
                if (Owner.IsPlayer)
                {
                    if (Player.Instance.currentGameZone == null)
                    {
                        Debug.Log("不在游戏区域内");
                        return;
                    }

                    BreakableObject[] suds = Player.Instance.currentGameZone.GetComponentsInChildren<BreakableObject>();

                    if (suds == null || suds.Length <= 0)
                    {
                        return;
                    }

                    foreach (var sud in suds)
                    {
                        Vector3 dirToSud = sud.transform.position - Owner.Transform.position;
                        float angle = Vector3.Angle(Owner.Forward, dirToSud);

                        if (dirToSud.magnitude < Owner.BlackBoard.WeaponRange + 0.2f && angle < 60)
                        {
                            if (sud.Breakable == false)
                            {
                                sud.Break();                         
                                sud.Breakable = true;
                                Player.Instance.coin += 50;
                            }                         
                        }
                    }
                }

            }
        }
        else if (State == E_State.E_FINISHED && EndOfStateTime <= Time.timeSinceLevelLoad)
        {           
            Action.AttackPhaseDone = true;
            Owner.WorldState.SetWSProperty(E_PropKey.E_IDLING, true);
            //Debug.Log(Time.timeSinceLevelLoad + " attack finished");
            Release();
        }
    }

    private void PlayAnim()
    {
        CrossFade(AnimAttackData.AnimName, 0.2f);
        // when to do hit !!!
        HitTime = Time.timeSinceLevelLoad + AnimAttackData.HitTime;

        StartPosition = Transform.position;
        FinalPosition = StartPosition + Transform.forward * AnimAttackData.MoveDistance;
        MoveTime = AnimAttackData.AttackMoveEndTime - AnimAttackData.AttackMoveStartTime;

        EndOfStateTime = Time.timeSinceLevelLoad + AnimEngine[AnimAttackData.AnimName].length * 0.9f;

        if (AnimAttackData.LastAttackInCombo)
            AttackPhaseTime = Time.timeSinceLevelLoad + AnimEngine[AnimAttackData.AnimName].length * 0.9f;
        else
            AttackPhaseTime = Time.timeSinceLevelLoad + AnimAttackData.AttackEndTime;

        CurrentMoveTime = -AnimAttackData.AttackMoveStartTime; // move a little bit later


        //  实现慢动作效果的,慢镜头回放
        if (Action.Target && Action.Target.IsAlive)
        {
            if (Critical)
            {
                CameraBehaviour.Instance.InterpolateTimeScale(0.25f, 0.5f);
                CameraBehaviour.Instance.InterpolateFov(25, 0.5f);
                CameraBehaviour.Instance.Invoke("InterpolateScaleFovBack", 0.7f);
            }
            else if (Action.AttackType == E_AttackType.Fatality)
            {
                CameraBehaviour.Instance.InterpolateTimeScale(0.25f, 0.7f);
                CameraBehaviour.Instance.InterpolateFov(25, 0.65f);
                CameraBehaviour.Instance.Invoke("InterpolateScaleFovBack", 0.8f);
            }
        }
    }

    override protected void Initialize(ActionBase action)
    {
        base.Initialize(action);
        SetFinished(false);

        State = E_State.E_PREPARING;
        Owner.BlackBoard.MotionType = E_MotionType.Attack;

        Action = action as ActionAttack;
        Action.AttackPhaseDone = false;
        Action.Hit = false;

        if (Action.Data == null)

            Action.Data = Owner.AnimSet.GetFirstAttackAnim(Owner.BlackBoard.WeaponSelected, Action.AttackType);

        AnimAttackData = Action.Data;

        if (AnimAttackData == null)
            Debug.LogError("AnimAttackData == null");

        StartRotation = Transform.rotation;
        StartPosition = Transform.position;

        float angle = 0;

        bool backstab = false;

        float distance = 0;

        if (Action.Target != null)
        {
            Vector3 dir = Action.Target.Position - Transform.position;
            distance = dir.magnitude;

            if (distance > 0.1f)
            {
                dir.Normalize();
                angle = Vector3.Angle(Transform.forward, dir);

                //attacker uhel k cili je mensi nez 40 and uhel enemace a utocnika je mensi nez 80 stupnu

                if (angle < 40 && Vector3.Angle(Owner.Forward, Action.Target.Forward) < 80)
                    backstab = true;
            }
            else
            {
                dir = Transform.forward;
            }

            FinalRotation.SetLookRotation(dir);

            if (distance < Owner.BlackBoard.WeaponRange)
                FinalPosition = StartPosition;
            else
                FinalPosition = Action.Target.Transform.position - dir * Owner.BlackBoard.WeaponRange;
   
            MoveTime = (FinalPosition - StartPosition).magnitude / 20.0f;
            RotationTime = angle / 720.0f;
        }
        else
        {
            FinalRotation.SetLookRotation(Action.AttackDir);

            RotationTime = Vector3.Angle(Transform.forward, Action.AttackDir) / 720.0f;
            MoveTime = 0;
        }

        RotationOk = RotationTime == 0;
        PositionOK = MoveTime == 0;

        CurrentRotationTime = 0;
        CurrentMoveTime = 0;

        if (Owner.IsPlayer && AnimAttackData.HitCriticalType != E_CriticalHitType.None && Action.Target && Action.Target.BlackBoard.CriticalAllowed &&
            Action.Target.IsBlocking == false && Action.Target.IsInvulnerable == false && Action.Target.IsKnockedDown == false)
        {
            if (backstab)
                Critical = true;
            else
            {
                // Debug.Log("critical chance" + Owner.GetCriticalChance() * AnimAttackData.CriticalModificator * Action.Target.BlackBoard.CriticalHitModifier);
                Critical = Random.Range(0, 100) < Owner.GetCriticalChance() * AnimAttackData.CriticalModificator * Action.Target.BlackBoard.CriticalHitModifier;
            }
        }
        else
            Critical = false;

        Knockdown = AnimAttackData.HitAreaKnockdown && Random.Range(0, 100) < 60 * Owner.GetCriticalChance();
    }
}
