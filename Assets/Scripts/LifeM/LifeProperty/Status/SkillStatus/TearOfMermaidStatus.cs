using UnityEngine;
using System.Collections;

public class TearOfMermaidStatus : StatusM 
{
    Role mOwner;
	bool mStatusActive = false;
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
        m_OverData = StatusOverAction.CD | StatusOverAction.Die; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.TearOfMermaid;
		m_SkillImmune = ImmuneSkill.Normal;
		m_InterruptSkill = false;
		m_SelfInterruptSkill = false;
		m_AntiInterruptStatus = AntiInterruptStatus.Normal;
		base.Init(SceneID,ResalseSceneID,SkillID,Info);

        mOwner = CM.GetLifeM(SceneID, LifeMType.SOLDIER) as Role;
		mStatusActive = false;
	}
    public override void RemoveStatus(float duration)
    {
		if (mStatusActive) return;
        base.RemoveStatus(duration);
        if (IsStatusOver())
        {
			mStatusActive = true;
            SoldierSkill sourceSkill = new SoldierSkill();
            SkillM.GetSkillInfo(m_StatusInfo.exSkill2, m_StatusInfo.exSkillLevel2, ref sourceSkill);
            Role attacker = CM.GetLifeM(m_StatusInfo.Releasescentid, LifeMType.SOLDIER) as Role;
            RoleSkill.GlobalUseSkill(attacker, mOwner, sourceSkill, Vector3.zero);
        }
    }
    public override void InterruptStatus(LifeAction Action)
    {
		if (mStatusActive) return;
        base.InterruptStatus(Action);
        if (Action == LifeAction.Die)
        {
			mStatusActive = true;
            SoldierSkill sourceSkill = new SoldierSkill();
            SkillM.GetSkillInfo(m_StatusInfo.exSkill1, m_StatusInfo.exSkillLevel1, ref sourceSkill);
            Role attacker = CM.GetLifeM(m_StatusInfo.Releasescentid, LifeMType.SOLDIER) as Role;
            RoleSkill.GlobalUseSkill(attacker, mOwner, sourceSkill, Vector3.zero);
        }
    }
}
