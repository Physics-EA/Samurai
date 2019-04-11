using UnityEngine;
public class AnimSetEnemyPeasant : AnimSet
   // protected AnimAttackData AnimAttacksSwordL = new AnimAttackData("attackPeasant", null, 1.0f, 0.45f, 0.68f, 5, 20, 1, E_CriticalHitType.None, false);
    protected AnimAttackData AnimAttacksSwordL;
    
        AnimAttacksSwordL = new AnimAttackData("attackPeasant", null, 0.8f, 0.68f, 0.55f, 0.75f, 0.8f, 5, 20, 1, E_CriticalHitType.None, 0, false, false, false, false);
        anims["idle"].layer = 0;
        anims["idleSword"].layer = 0;
        anims["runSwordBackward"].layer = 0;
        anims["walk"].layer = 0;
        anims["combatMoveB"].layer = 0;
        anims["combatMoveR"].layer = 0;
        anims["combatMoveL"].layer = 0;
        anims["rotationLeft"].layer = 0;
        anims["rotationRight"].layer = 0;
        
        anims["death01"].layer = 2;
        anims["death02"].layer = 2;
        anims["injuryFront01"].layer = 1;
        anims["injuryFront02"].layer = 1;
        anims["injuryFront03"].layer = 1;
        anims["injuryFront04"].layer = 1;

        anims["attackPeasant"].layer = 0;
        anims["attackPeasant"].speed = 1.1f;

//        anims["spawn"].layer = 1;

    public override string GetIdleActionAnim(E_WeaponType weapon, E_WeaponState weaponState)
    {
        if(weapon == E_WeaponType.Katana)
            return "idleTount";

        return "idle";
    }

    public override string GetMoveAnim(E_MotionType motion, E_MoveType move, E_WeaponType weapon, E_WeaponState weaponState)
                return "walk";
            if (motion == E_MotionType.Walk)
            {
                if (move == E_MoveType.Forward)
                    return "combatMoveF";
                else if (move == E_MoveType.Backward)
                    return "combatMoveB";
                else if (move == E_MoveType.StrafeRight)
                    return "combatMoveR";
                else
                    return "combatMoveL";
            }
            else
            {
                if (move == E_MoveType.Forward)
                    return "runSword";
                else if (move == E_MoveType.Backward)
                    return "runSwordBackward";
            }

        return "idle";

    public override string GetRotateAnim(E_MotionType motionType, E_RotationType rotationType)
    {
        if (motionType == E_MotionType.Block)
        {
            if (rotationType == E_RotationType.Left)
                return "blockStepLeft";

            return "blockStepRight";
        }

        if (rotationType == E_RotationType.Left)
            return "rotationLeft";

        return "rotationRight";
    }

        return "idle";


    public override AnimAttackData GetFirstAttackAnim(E_WeaponType weapom, E_AttackType attackType)
    {
        return AnimAttacksSwordL;
    }

    public override AnimAttackData GetChainedAttackAnim(AnimAttackData parent, E_AttackType attackType)
    {
        return null;
    }



    public override string GetInjuryAnim(E_WeaponType weapon, E_DamageType type)
    {
        string[] anims = { "injuryFront01", "injuryFront02", "injuryFront03", "injuryFront04"};

        return anims[Random.Range(0, anims.Length)];
    }

    public override string GetDeathAnim(E_WeaponType weapon, E_DamageType type)
    {
        string[] anims = { "death01", "death02"};

        return anims[Random.Range(0, anims.Length)];
    }


    public override string GetKnockdowAnim(E_KnockdownState state, E_WeaponType weapon)
    {
        switch (state)
        {
            case E_KnockdownState.Down:
                return "knockdown";
            case E_KnockdownState.Loop:
                return "knockdownLoop";
            case E_KnockdownState.Up:
                return "knockdownUp";
            case E_KnockdownState.Fatality:
                return "knockdownDeath";
            default:
                return "";
        }
    }