using UnityEngine;
using System.Collections;

/// <summary>
/// 墙体属性
/// </summary>
/// <author>zhulin</author>
///
public class WallAttribute : NDAttribute
{

    public override void Init(int SceneID, LifeMCore Core, Life Parent)
    {
        WallType type  = WallType.Normal;
        if (Parent == null) {
            return ;
        }
        if (Parent is IggWall) {
            type = (Parent as IggWall).m_WallType;
        }
        base.Init(SceneID, Core, Parent);
        WallInfo info = WallM.GetWallData(type, Core.m_DataID);
        if (info == null) {
            return ;
        }
        
        m_StartPos.Unit = info.m_cx;
        m_StartPos.Layer = info.m_cy;
        m_Size = 2;
        m_FullHp = info.m_hp;
        m_IsDamage = true;
        m_wood = info.m_wood;
        m_stone = info.m_stone;
        m_steel = info.m_steel;
        m_phy_defend = info.m_phydefned;
        m_magic_defend = info.m_magicdefend;
        
        m_DoorState = false;
    }
    
    protected override int GetBaseAttrData(EffectType Type)
    {
        if (Type == EffectType.Strength) {
            return m_strength;
        } else if (Type == EffectType.Agility) {
            return m_agility;
        } else if (Type == EffectType.Intelligence) {
            return m_intelligence;
        } else if (Type == EffectType.Hp) {
            return m_FullHp;
        } else if (Type == EffectType.PhyAttack) {
            return m_phy_attack;
        } else if (Type == EffectType.PhyDefense) {
            return m_phy_defend;
        } else if (Type == EffectType.MagicAttack) {
            return m_magic_attack;
        } else if (Type == EffectType.MagicDefense) {
            return m_magic_defend;
        } else if (Type == EffectType.PhyCrit) {
            return 1;
        } else if (Type == EffectType.MagicCrit) {
            return 1;
        } else if (Type == EffectType.CutPhyDefend) {
            return m_CutPhyDefend;
        } else if (Type == EffectType.CutMagDefend) {
            return m_CutMagDefend;
        } else if (Type == EffectType.CutphyDamage) {
            return m_CutPhyDamage;
        } else if (Type == EffectType.CutMagDamage) {
            return m_CutMagDamage;
        } else if (Type == EffectType.Dodge) {
            return m_dodge;
        } else if (Type == EffectType.Vampire) {
            return m_Vampire;
        } else if (Type == EffectType.Hit) {
            return m_Hit;
        } else if (Type == EffectType.RecoHp) {
            return m_RecoHp;
        } else if (Type == EffectType.RecoAnger) {
            return m_RecoAnger;
        } else if (Type == EffectType.AddDoctor) {
            return m_AddDoctor;
        }
        
        return 0;
    }
    
    
    
    
    /// <summary>
    /// 获取力量
    /// </summary>
    protected virtual int GetStrength()
    {
        return m_strength;
    }
    
    /// <summary>
    /// 获取敏捷
    /// </summary>
    protected virtual int GetAgility()
    {
        return m_agility;
    }
    /// <summary>
    /// 获取智力
    /// </summary>
    protected virtual int GetIntelligence()
    {
        return m_intelligence;
    }
    protected override int GetType()
    {
        return 0;
    }
    /// <summary>
    /// 获取魔法防御
    /// </summary>
    protected override int GetMagicDefend()
    {
        return m_magic_defend;
    }
    
    /// <summary>
    /// 获取物理防御
    /// </summary>
    protected override int GetPhyDefend()
    {
        return m_phy_defend;
    }
    
    /// <summary>
    /// 获取满血
    /// </summary>
    protected override int GetFullHp()
    {
        return m_FullHp;
    }
    
    
    /// <summary>
    /// 获取建筑start位置
    /// </summary>
    protected override Int2 GetStartPos()
    {
        return m_StartPos;
    }
    
    /// <summary>
    /// 获取建筑大小，体型
    /// </summary>
    protected override int GetSize()
    {
        return m_Size;
    }
    
    /// <summary>
    /// 获取木材数量
    /// </summary>
    protected override int GetWood()
    {
        return m_wood;
    }
    
    
    
    /// <summary>
    /// 获取石头数量
    /// </summary>
    protected override int GetStone()
    {
        return m_stone;
    }
    
    
    /// <summary>
    /// 获取刚才数量
    /// </summary>
    protected override int GetSteel()
    {
        return m_steel;
    }
    protected override int GetCritRatio()
    {
        return m_CritRatio;
    }
    /// <summary>
    /// 获取满怒气
    /// </summary>
    protected override int GetFullAnger()
    {
        return 0;
    }
}
