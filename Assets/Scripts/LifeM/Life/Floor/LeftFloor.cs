using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LeftFloor : IggFloor
{

    /// <summary>
    /// 船体两侧的船壳所在的GameObject
    /// </summary>
    public GameObject BoatBody;
    private string m_sTextureName;
    public Texture Wall1_l;
    public Texture Wall2_l;
    
    
    
    public override void SetFloorType()
    {
        m_FloorType = FloorType.left;
        m_sTextureName = GetTextureName(Layer);
    }
    /// <summary>
    /// 撞击处理
    /// </summary>
    /// <param name="Damage">撞击伤害</param>
    /// <param name="dir">撞击方向</param>
    /// <param name="Info">返回撞击信息</param>
    /// <returns></returns>
    public override void CollisionProcByFire(Collision collision, int Damage/*,Vector2 dir*/, ref FlyCollisionInfo Info, FlyDir flydir)
    {
        Role.WakeEnemy(this);
        Info.bVertical = true;
        Info.bLeft = true;
        Info.FlyfallSTandGridPos = new Int2(StartUnit, Layer);
        Info.lifemCollision = this;
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
            /*if (flydir == FlyDir.Left || flydir == FlyDir.LeftTop || flydir == FlyDir.LeftBottom)
            {
            	Info.flyCollisionAction = FlyCollisionAction.DropOutBoat;
            	//设置左右板信息，并非移动方向
            	Info.HitDir = WalkDir.WALKLEFT;
            	Info.bApplyDamage = false;
            
            }
            else if (flydir == FlyDir.Right || flydir == FlyDir.RightTop || flydir == FlyDir.RightBottom || flydir == FlyDir.Top || flydir == FlyDir.Bottom)
            {
            	Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
            	Info.bApplyDamage = false;
            }
            else
            {
            	if(m_DefenceColider.IsInRightCollider(collision.collider))
            	{
            		Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
            		Info.bApplyDamage = false;
            	}
            	else if(m_DefenceColider.IsInLeftCollider(collision.collider))
            	{
            		Info.flyCollisionAction = FlyCollisionAction.DropOutBoat;
            		//设置左右板信息，并非移动方向
            		Info.HitDir = WalkDir.WALKLEFT;
            		Info.bApplyDamage = false;
            	}
            }*/
            
            Info.flyCollisionAction = FlyCollisionAction.DropOutBoat;
            for (int i = Layer; i >= 0; i--) {
                MapGrid g = MapGrid.GetMG(i, StartUnit);
                if (g != null && (g.Type != GridType.GRID_HOLE && g.Type != GridType.GRID_HOLESTAIR)) {
                    Info.flyCollisionAction = FlyCollisionAction.DropInBoat;
                    break;
                }
            }
            
            Info.droppos = new Vector3(-1f, 0, 0);
            GameObjectActionExcute gae1 = EffectM.LoadEffect(EffectM.sPath, "zamen_01", m_thisT.position, null);
            GameObjectActionDelayDestory ndEffect1 = new GameObjectActionDelayDestory(1f);
            gae1.AddAction(ndEffect1);
        }
    }
    public void shake()
    {
        Animator ani = GetComponent<Animator>();
        if (ani != null) {
            ani.SetTrigger("tParam");
        }
    }
    
    string GetTextureName(int layer)
    {
        switch (layer) {
            case 0:
                return "Boat_Shell_8";
            case 1:
                return "Boat_Shell_7";
            case 2:
                return "Boat_Shell_6";
                
            default:
                return "";
        }
    }
    
    void ChangeTexture(Texture t)
    {
        Renderer ren = BoatBody.GetComponent<Renderer>();
        if (m_sTextureName == "" || BoatBody == null || ren == null) {
            Debug.Log("m_sTextureName=\"\"" + " or BoatBody==null" + " or Renderer==null");
            return;
        }
        foreach (Material m in ren.materials) {
            if (m.name.IndexOf(m_sTextureName) > -1) {
                m.mainTexture = t;
            }
        }
    }
    
    public override bool ApplyDamage(SkillReleaseInfo Info, Transform attackgo)
    {
        bool result = base.ApplyDamage(Info, attackgo);
        Hit(Info.m_Damage);
        return result;
    }
    
    public  void Hit(int damage)
    {
        if (BoatBody == null || BoatBody.GetComponent<Renderer>() == null) {
            //Debug.Log("BoatBody null or Renderer not found!!!");
            return;
        }
        
        if (HP < fullHP && HP > 0) {
            if (Wall1_l != null) {
                ChangeTexture(Wall1_l);
            }
        } else if (HP <= 0) {
            if (Wall2_l != null) {
                ChangeTexture(Wall2_l);
            }
        }
    }
}
