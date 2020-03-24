using UnityEngine;
using System.Collections;

public class FireShieldStatus : StatusM 
{
    Role mOwner;
	bool mStatusActive = false;
	float mcd = 0;
	public override void Init(int SceneID,int ResalseSceneID,int SkillID,SkillStatusInfo Info)
	{
        m_OverData = StatusOverAction.CD | StatusOverAction.Die; 
		m_AddRole = AddRule.Replace;
		m_type = StatusType.FireShield;
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
        base.RemoveStatus(duration);
		mcd -= duration;
        if (mcd <= 0 )
        {
			mStatusActive = true;
            SoldierSkill sourceSkill = new SoldierSkill();
            SkillM.GetSkillInfo(m_StatusInfo.exSkill1, m_StatusInfo.exSkillLevel1, ref sourceSkill);
			mcd = sourceSkill.m_cd * 0.001f;
            Role attacker = CM.GetLifeM(m_StatusInfo.Releasescentid, LifeMType.SOLDIER) as Role;
            RoleSkill.GlobalUseSkill(attacker, mOwner, sourceSkill, Vector3.zero);
        }
    }
}
