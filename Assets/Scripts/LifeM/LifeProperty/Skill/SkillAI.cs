using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 技能AI
/// </summary>
/// <author>zhulin</author>
public class SkillAI
{

    private Role m_Parent = null;
    
    protected SoldierSkill m_SkillInfo = null;
    
    protected   float m_CdTime;          //CD即使时间
    /// <summary>
    /// 条件技能计时器
    /// </summary>
    protected float m_TimeCount  = 0.0f;
    /// <summary>
    /// 条件技能目标
    /// </summary>
    protected Life m_Target = null;
    /// <summary>
    /// 条件技能目标位置
    /// </summary>
    protected MapGrid m_TargetPos = null;
    /// <summary>
    /// 条件技能释放
    /// </summary>
    public bool m_Relsease = false;
    
    
    public bool m_IsBigSkill = false;
    
    
    private List<Life> m_AttackList = new List<Life>();
    
    public  virtual void Update(float deltaTime, float AttackSpeed)
    {
        if (m_CdTime > 0) {
            m_CdTime -= deltaTime * AttackSpeed;
            if (m_CdTime <= 0) {
                //释放技能
                m_CdTime = m_TimeCount;
                m_Relsease = true;
            }
        }
    }
    
    
    public  bool GetSkillTarget()
    {
        //获取攻击列表
        if (m_SkillInfo.m_target == (int)SkillTarget.Self || m_SkillInfo.m_target == (int)SkillTarget.FriendlyTeam || m_SkillInfo.m_target == (int)SkillTarget.EnemyTeam) {
            m_Target = m_Parent;
            m_TargetPos = m_Parent.GetMapGrid();
            return true;
        } else if (m_SkillInfo.m_target == (int)SkillTarget.EnemyGround ||  m_SkillInfo.m_target == (int)SkillTarget.FriendlyGround) {
            Int2 pos = m_Parent.GetMapPos();
            MapGrid lgrid = MapGrid.GetMG(pos.Layer, pos.Unit - m_SkillInfo.m_distance / MapGrid.m_Pixel);
            MapGrid rgrid = MapGrid.GetMG(pos.Layer, pos.Unit + m_SkillInfo.m_distance / MapGrid.m_Pixel);
            
            if (m_Parent.WalkDir == WalkDir.WALKLEFT && lgrid != null) {
                m_TargetPos = lgrid;
                return true;
            } else if (m_Parent.WalkDir == WalkDir.WALKRIGHT && rgrid != null) {
                m_TargetPos = rgrid;
                return true;
            }
        }
        
        if (!GetAttackList()) {
            m_Target = null;
            m_TargetPos = null;
            return false;
        }
        
        //从攻击列表中获取最佳攻击目标
        m_Target = GetBestAttackTarget();
        int id = -1;
        if (m_Target != null) {
            m_TargetPos = m_Target.GetMapGrid();
            return true;
        }
        return false;
    }
    
    private bool GetAttackList()
    {
        m_AttackList.Clear();
        int distant = m_SkillInfo.m_distance / MapGrid.m_Pixel;
        LifeMCamp camp = m_Parent.m_Core.m_Camp == LifeMCamp.ATTACK ? LifeMCamp.DEFENSE : LifeMCamp.ATTACK;
        if (m_SkillInfo.m_target == 4) {
            camp = m_Parent.m_Core.m_Camp;
        }
        if (m_SkillInfo.m_condition != (int)SkillCondition.HaveDoor) {
            if (m_SkillInfo.m_distance > 0) {
                if (m_Parent.Target != null && !m_Parent.Target.isDead) {
                    if (CheckAttackTarget(m_Parent.Target, m_SkillInfo.m_sort, distant)) {
                        m_AttackList.Add(m_Parent.Target);
                    }
                    /*else
                    {
                    	if (!m_Parent.run.CheckHaveIdleAttackPosInPath(m_Parent.SceneID))
                    	{
                    		//m_Parent.run.Pass = false;
                    		int ldistant = m_SkillInfo.m_ldistance/MapGrid.m_Pixel;
                    		if (CheckAttackTarget(m_Parent.Target,m_SkillInfo.m_sort,ldistant))
                    			m_AttackList.Add(m_Parent.Target);
                    	}
                    	else
                    	{
                    		//m_Parent.run.Pass = true;
                    	}
                    }*/
                }
            } else {
                List<Life> l = new List<Life>();
                ///需要排除隐形 。。。。。。。。。。。。。。。。。。
                CM.SearchLifeMListInBoat(ref l, LifeMType.SOLDIER, camp);
                foreach (Role r in l) {
                    if (m_Parent.m_Attr.IsHide && !m_IsBigSkill) {
                        if (m_Parent.Target != null && !m_Parent.Target.isDead && CheckAttackTarget(m_Parent.Target, m_SkillInfo.m_sort, distant)) {
                            m_AttackList.Add(m_Parent.Target);
                        }
                    } else if (!r.m_Attr.IsHide && CheckCanAttack(r.CurrentAction) && CheckAttackTarget(r, m_SkillInfo.m_sort, distant)) {
                        m_AttackList.Add(r);
                    }
                }
                /*List<Life> lp = new List<Life>();
                CM.SearchLifeMListInBoat(ref lp,LifeMType.PET, camp);
                foreach(Life p in lp)
                {
                
                	if (m_Parent.m_Attr.IsHide && !m_IsBigSkill)
                	{
                		if (m_Parent.Target != null && !m_Parent.Target.isDead && CheckAttackTarget(m_Parent.Target,m_SkillInfo.m_sort,distant))
                			m_AttackList.Add(m_Parent.Target);
                	}
                	else if (CheckAttackTarget(p,1,distant) )
                		m_AttackList.Add(p);
                }*/
                //if (m_parent.m_Core.m_Camp == LifeMCamp.ATTACK)
                //{
                List<Life> lb = new List<Life>();
                CM.SearchLifeMListInBoat(ref lb, LifeMType.BUILD, camp);
                foreach (Building b in lb) {
                
                    if (m_Parent.m_Attr.IsHide && !m_IsBigSkill) {
                        if (m_Parent.Target != null && !m_Parent.Target.isDead && CheckAttackTarget(m_Parent.Target, m_SkillInfo.m_sort, distant)) {
                            m_AttackList.Add(m_Parent.Target);
                        }
                    } else if (b.m_Attr.IsDamage && CheckAttackTarget(b, 1, distant)) {
                        m_AttackList.Add(b);
                    }
                }
            }
        }
        List<Life> lw = new List<Life>();
        CM.SearchLifeMListInBoat(ref lw, LifeMType.WALL, camp);
        foreach (IggWall w in lw) {
            if (CheckAttackTarget(w, 1, distant) && !w.m_Attr.DoorState) {
                m_AttackList.Add(w);
            }
        }
        //}
        CM.SearchAttackLifeMList(ref m_AttackList, m_Parent);
        
        return true;
    }
    
    public bool CheckAttackTarget()
    {
        return false;
    }
    
    
    public virtual bool CheckCanAttack(GridActionCmd action)
    {
        if (action is GridActionCmdStair || action is GridActionCmdJump || action is GridActionCmdSpecialJump || action is GridActionCmdFall || action is GridActionCmdSendToPos) {
            return false;
        }
        return true;
    }
    
    private Life GetBestAttackTarget()
    {
        if (m_AttackList == null || m_AttackList.Count == 0) {
            return null;
        }
        
        if (m_AttackList.Count == 1) {
            return m_AttackList [0];
        }
        
        
        Life m = null;
        List<Life> l = new List<Life>();
        CM.SearchAttackLifeMList(ref m_AttackList, m_Parent);
        if (m_AttackList.Count > 0) {
        
        
            l = GetSkillTarget(m_AttackList, m_SkillInfo);
            if (l.Count == 1) {
                m = l[0];
            }
            if (m != null) {
                return m;
            }
            if (l.Count > 1) {
                m_AttackList  = l;
            }
            //目标一致优先
            /*m = GetAttack2Target();
            if(m != null) return m;
            
            //目标一致优先
            m = GetAttackTargetLock();
            if(m != null) return m;*/
            //距离最近
            m = GetShortAttackTarget();
            if (m != null) {
                return m;
            }
        }
        
        return null;
    }
    
    //获取距离最近的目标
    private Life GetShortAttackTarget()
    {
        int ii = 0;
        float dis = Life.CalcDistance(m_AttackList[0], m_Parent);
        for (int i = 1; i < m_AttackList.Count; i++) {
            float d = Life.CalcDistance(m_AttackList[i], m_Parent);
            if (dis > d) {
                dis  = d;
                ii = i;
            }
        }
        return m_AttackList [ii];
    }
    
    //Sort行为分类，详细内容在技能系统——炮弹兵技能数据结构说明
    public virtual bool CheckAttackTarget(Life target, int Sort, int distant)
    {
        return CheckDoAttackTarget(target, Sort, distant);
        
    }
    
    public  bool CheckDoAttackTarget(Life target, int Sort, int distant)
    {
        if (target == null || target.isDead) {
            return false;
        }
        //单人
        //if (Sort == 1)
        {
            Life l = m_Parent;
            if (m_Parent is Role) {
                Role parent = l as Role;
                float radius = distant;//PropSkillInfo.m_distance/MapGrid.m_Pixel;
                if (radius <= 0) {
                    return true;
                }
                if (!NdUtil.IsSameMapLayer(parent.GetMapPos(), target.GetMapPos())) {
                    return false;
                }
                //radius += parent.m_Attr.Radius;
                float x1 = parent.m_thisT.localPosition.x;
                MapGrid g = MapGrid.GetMG(target.GetMapPos());
                if (g == null) {
                    //Debug.LogError(target + "," + target.GetMapPos() + "," + target.InBoat);
                    return false;
                }
                float x2 = g.pos.x;
                
                if (target is Role) {
                    //radius += (int)(target as Role).m_Attr.Radius;
                    x2 = target.m_thisT.localPosition.x;
                }
                if (target is IggWall) {
                    radius += 1;
                }
                radius += 0.25f;
                if (parent.WalkDir == WalkDir.WALKLEFT) {
                    if (x1 >= x2 && (x1 - x2) <= (radius * MapGrid.m_width)) {
                        return true;
                    }
                } else if (parent.WalkDir == WalkDir.WALKRIGHT) {
                
                    if (x1 <= x2 && (x2 - x1) <= (radius * MapGrid.m_width)) {
                        return true;
                    }
                } else if (Mathf.Abs(x1 - x2) < radius * MapGrid.m_width) {
                    return true;
                }
            }
        }
        return false;
    }
    
    //目标做为攻击目标
    public static List<Life> GetSkillTarget(List<Life> Attacklist, SoldierSkill info)
    {
        int term1 = info.m_term1;
        int term2 = info.m_term2;
        int term3 = info.m_term3;
        int index = -1;
        int value  = -1;
        List<Life> l = new List<Life>();
        for (int i = 0; i < Attacklist.Count; i++) {
            if (Attacklist[i] is Role) {
                Role w = Attacklist[i] as Role;
                int newvalue = GetTerm1Vaule(w, term1);
                if (term1 >= 1 && term1 <= 3) {
                    if (i == 0) {
                        value = newvalue;
                        index = i;
                        l.Add(Attacklist[i]);
                    } else {
                        if (CheckCondition(value, newvalue, term2, term3)) {
                            value = newvalue;
                            index = i;
                            l[0] = Attacklist[i];
                        }
                    }
                } else if (term1 > 3) {
                    bool ret = CheckCondition(newvalue, 0, term2, term3);
                    if (ret) {
                        l.Add(Attacklist[i]);
                    }
                    
                } else if (term1 == 0) {
                    l.Add(Attacklist[i]);
                }
            } else if (Attacklist[i] is Pet) {
                Pet w = Attacklist[i] as Pet;
                int newvalue = GetTerm1Vaule(w, term1);
                if (term1 >= 1 && term1 <= 3) {
                    if (i == 0) {
                        value = newvalue;
                        index = i;
                        l.Add(Attacklist[i]);
                    } else {
                        if (CheckCondition(value, newvalue, term2, term3)) {
                            value = newvalue;
                            index = i;
                            l[0] = Attacklist[i];
                        }
                    }
                } else if (term1 > 3) {
                    if (CheckCondition(newvalue, 0, term2, term3)) {
                        l.Add(Attacklist[i]);
                    }
                    
                } else if (term1 == 0) {
                    l.Add(Attacklist[i]);
                }
            } else {
                // Pet w = Attacklist[i] as Pet;
                int newvalue = GetTerm1Vaule(Attacklist[i], term1);
                if (term1 > 3) {
                    if (CheckCondition(newvalue, 0, term2, term3)) {
                        l.Add(Attacklist[i]);
                    }
                    
                } else if (term1 == 0) {
                    l.Add(Attacklist[i]);
                }
            }
        }
        /*if (index >= 0)
        	return mAttacklist[index];*/
        
        return l;
    }
    public static  int GetTerm1Vaule(Life w, int term1)
    {
        switch (term1) {
            case 1:
                return w.m_Attr.Strength;
            //break;
            case 2:
                return w.m_Attr.Agility;
            //break;
            case 3:
                return w.m_Attr.Intelligence;
            //break;
            case 4:
                return w.m_Attr.Hp;
            //break;
            case 5:
                return w.m_Attr.Anger;
            //break;
            case 6:
                return (int)((float)w.m_Attr.Hp / w.m_Attr.FullHp * 100);
            //break;
            case 7:
                return (int)((float)w.m_Attr.Anger / w.m_Attr.FullAnger * 100);
            //break;
            default:
                break;
        }
        return -1;
    }
    public static  bool CheckCondition(int left, int right, int term2, int term3)
    {
        if (term2 == 0 && term3 == 0) {
            if (right < left) {
                return true;
            }
        } else if (term2 == 100 && term3 == 100) {
            if (right > left) {
                return true;
            }
        }
        
        if (left >= term2 && left <= term3) {
            return true;
        }
        
        return false;
    }
    
}
