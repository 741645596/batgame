using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 楼板属性
/// </summary>
/// <author>zhulin</author>
///
public class FloorAttribute : NDAttribute
{
    //关联房间的属性
    public List<NDAttribute> m_BuildRoomAttr = new List<NDAttribute>();
    public IggFloor FloorLife{
        get{return m_Parent as IggFloor;}
    }
    public override void Init(int SceneID, LifeMCore Core, Life Parent)
    {
        FloorType type  = FloorType.Normal;
        if (Parent == null) {
            return ;
        }
        base.Init(SceneID, Core, Parent);
        if (Parent is IggFloor) {
            type = (Parent as IggFloor).m_FloorType;
            m_BuildRoomAttr = GetBuildRoomAttr(type, (Parent as IggFloor).Layer, (Parent as IggFloor).StartUnit);
        } else {
            m_BuildRoomAttr.Add(GetBuildRoomAttr(Parent, (Parent as IggWall).Layer, (Parent as IggWall).StartUnit));
        }
        FloorInfo info = CmCarbon.GetFloor();
        if (info == null) {
            m_FullHp = 999999999;
            return ;
        }
        m_FullHp = info.m_hp;
        m_IsDamage = true;
        m_phy_defend = info.m_phydefend;
        m_magic_defend = info.m_magicdefend;
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
    
        NDAttribute roomattr = GetRoomAttr();
        if (roomattr != null) {
            return roomattr.FullHp;
        }
        return m_FullHp;
    }
    
    
    protected override int GetCritRatio()
    {
        return m_CritRatio;
    }
    
    
    protected List<NDAttribute> GetBuildRoomAttr(FloorType type, int layer, int unit)
    {
        List<NDAttribute> l = new List<NDAttribute>();
        Int2 Pos = new Int2(unit, layer);
        if (type == FloorType.top) {
            Pos.Layer = Pos.Layer - 1;
            if (Pos.Layer < 0) {
                Pos.Layer = 0;
            }
            Pos.Unit += MapGrid.m_UnitRoomGridNum / 2;
        } else if (type == FloorType.left) {
            Pos.Unit += MapGrid.m_UnitRoomGridNum / 2;
        } else if (type == FloorType.right) {
            Pos.Unit -= MapGrid.m_UnitRoomGridNum / 2;
        } else {
            Pos.Unit += MapGrid.m_UnitRoomGridNum / 2;
        }
        
        MapGrid m = MapGrid.GetMG(Pos);
        if (m != null) {
            int BuildRoomSceneID =  -1;
            if (m.GetBuildRoom(ref BuildRoomSceneID) == true) {
                Life buildRoom = CM.GetLifeM(BuildRoomSceneID, LifeMType.BUILD);
                //Debug.Log("floor: " + layer + "," + unit + "," + m_SceneID + ",buildroom:" + buildRoom.m_Attr.Pos + "," + buildRoom.SceneID);
                if (buildRoom != null) {
                    l.Add(buildRoom.m_Attr) ;
                    //FloorLife.m_rooms.Add(buildRoom as Building);
                }
            }
        }
        if (type == FloorType.Normal) {
        
            if (Pos.Layer > 0) {
                Pos.Layer = Pos.Layer - 1;
                Pos.Unit += MapGrid.m_UnitRoomGridNum / 2;
                MapGrid m1 = MapGrid.GetMG(Pos);
                if (m1 != null) {
                    int BuildRoomSceneID =  -1;
                    if (m1.GetBuildRoom(ref BuildRoomSceneID) == true) {
                        Life buildRoom = CM.GetLifeM(BuildRoomSceneID, LifeMType.BUILD);
                        //Debug.Log("floor: " + layer + "," + unit + "," + m_SceneID + ",buildroom:" + buildRoom.m_Attr.Pos + "," + buildRoom.SceneID);
                        if (buildRoom != null) {
                            l.Add(buildRoom.m_Attr) ;
                            //FloorLife.m_rooms.Add(buildRoom as Building);
                        }
                    }
                }
            }
        }
        return l;
    }
    protected NDAttribute GetBuildRoomAttr(Life parent, int layer, int unit)
    {
        Int2 Pos = new Int2(unit, layer);
        if (parent is LeftFloorWall) {
            Pos.Unit += 3;
        } else if (parent is rightFloorWall) {
            Pos.Unit -= 3;
        }
        
        MapGrid m = MapGrid.GetMG(Pos);
        if (m != null) {
            int BuildRoomSceneID =  -1;
            if (m.GetBuildRoom(ref BuildRoomSceneID) == true) {
                Life buildRoom = CM.GetLifeM(BuildRoomSceneID, LifeMType.BUILD);
                if (buildRoom != null) {
                    return buildRoom.m_Attr ;
                }
            }
        }
        return null;
    }
    public override int GetBear()
    {
    
        NDAttribute roomattr = GetRoomAttr();
        if (roomattr != null) {
            return roomattr.Bear;
        }
        return m_bear;
    }
    public override int GetCurBear()
    {
        NDAttribute roomattr = GetRoomAttr();
        if (roomattr != null) {
            return roomattr.CurBear;
        }
        return 0;
    }
    public NDAttribute GetRoomAttr()
    {
        for (int i = 0; i < m_BuildRoomAttr.Count;) {
            if (m_BuildRoomAttr[0] == null || m_BuildRoomAttr[0].idDead()) {
                m_BuildRoomAttr.RemoveAt(0);
            } else {
                return m_BuildRoomAttr[i];
            }
        }
        return null;
    }
}
