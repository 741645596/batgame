using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeftFloorWall : IggWall
{

    public override void SetCore()
    {
        bool IsPlayer = CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE);
        m_Core = new LifeMCore(0, IsPlayer, LifeMType.WALL, LifeMCamp.DEFENSE, MoveState.Static);
        SetLifeCore(m_Core);
        
        m_Attr = new FloorAttribute();
        m_Attr.Init(SceneID, m_Core, this);
        m_Attr.StartPos = new Int2(StartUnit, Layer);
        
        
        HP = fullHP;
    }
    public override void CollisionProcByFire(Collision collision, int Damage/*,Vector2 dir*/, ref FlyCollisionInfo Info, FlyDir flydir)
    {
        Role.WakeEnemy(this);
        Info.bVertical = true;
        Info.bLeft = true;
        Info.lifemCollision = this;
        Info.FlyfallSTandGridPos = new Int2(StartUnit, Layer);
        SkillReleaseInfo sInfo = new SkillReleaseInfo();
        sInfo.m_InterruptSkill = false;
        sInfo.m_MakeStatus = new List<StatusType> ();
        sInfo.m_bImmunity = false;
        sInfo.m_Damage = Damage;
        sInfo.Result = AttackResult.Fire;
        ApplyDamage(sInfo, null);
        if (isDead) { //破了
            //GameObject go = SkillEffects._instance.LoadEffect("effect/prefab/", "1003021", MapGrid.GetMG(Info.FlyfallSTandGridPos).WorldPos, 1f);
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000291", MapGrid.GetMG(Info.FlyfallSTandGridPos).WorldPos, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
            effectinit.SetRotation(new Vector3(0, 0, 90));
            gae.AddAction(effectinit);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
            gae.AddAction(ndEffect);
            //go.transform.Rotate(new Vector3(0,0,90),Space.Self);
            Info.flyCollisionAction = FlyCollisionAction.PauseContinueFlyDirectional;
            Info.bApplyDamage = true;
            //EffectM.LoadEffect(EffectM.sPath, "zamen_02", transform.position, null);
        } else {      //没破
            shake();
            //GameObject go = SkillEffects._instance.LoadEffect("effect/prefab/", "1003011", MapGrid.GetMG(Info.FlyfallSTandGridPos).WorldPos, 1f);
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2000281", MapGrid.GetMG(Info.FlyfallSTandGridPos).WorldPos, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
            effectinit.SetRotation(new Vector3(0, 0, 90));
            gae.AddAction(effectinit);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
            gae.AddAction(ndEffect);
            //go.transform.Rotate(new Vector3(0,0,90),Space.Self);
            if (m_DefenceColider.IsInRightCollider(collision.collider)) {
                Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
                Info.bApplyDamage = false;
            } else if (m_DefenceColider.IsInLeftCollider(collision.collider)) {
                Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
                //设置左右板信息，并非移动方向
                Info.HitDir = WalkDir.WALKLEFT;
                Info.bApplyDamage = false;
            }
            GameObjectActionExcute gae1 = EffectM.LoadEffect(EffectM.sPath, "zamen_01", m_thisT.position, null);
            GameObjectActionDelayDestory ndEffect1 = new GameObjectActionDelayDestory(1f);
            gae1.AddAction(ndEffect1);
        }
    }
    public void shake()
    {
        Animator ani = GetComponent<Animator>();
        if (ani != null) {
            ani.SetTrigger("tLHit");
        }
    }
    /*	public override bool ApplyDamage(SkillReleaseInfo Info, Transform attackgo)
    	{
    		bool result = base.ApplyDamage(Info, attackgo);
    		Hit(Info.m_Damage);
    		return result;
    	}*/
    protected override void GetBuildRoom(int layer, int unit)
    {
        Int2 Pos = new Int2(unit, layer);
        Pos.Unit += 3;
        AddBuildRoom(Pos);
        
    }
}
