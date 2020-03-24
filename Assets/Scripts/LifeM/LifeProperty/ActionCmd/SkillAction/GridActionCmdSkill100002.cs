using UnityEngine;
using System.Collections;
/// <summary>
/// 鐏?灙鐟炲摜鎶鑳歼缂栧彿100002
/// </summary>
/// 1004鏅?敾
public class GridActionCmd100002Skill01 :GridActionCmdAttack{
	public GridActionCmd100002Skill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_CastTime = 0.6f;
		m_EventTime = 0.6f;
		m_Duration = 1.5f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack85000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1002011",posgo.transform.position,BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
			gae.AddAction(ndEffect);
			if (m_Dir == WalkDir.WALKRIGHT)
			{
				gae.transform.Rotate(new Vector3(0,180,0),Space.Self);
				Transform shot = gae.transform.Find("Shot_01");
				if (shot.GetComponent<Renderer>()!= null)
				{
					shot.GetComponent<Renderer>().material.mainTextureScale = new Vector2(-1f,1f);
				}
			}
		}
	}
	
}
//1005 寤舵椂鐐稿脊
public class GridActionCmd100002Skill02 :GridActionCmdAttack{
	public GridActionCmd100002Skill02(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 0.633f;
		m_EventTime = 0.633f;
		m_Duration = 1.5f;
		
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack81000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();

        SoundPlay.Play("grenade", false, false);
	}
	
}
//1039 姝讳骸鏍囪?
public class GridActionCmd100002Skill1039 :GridActionCmdAttack{
	public GridActionCmd100002Skill1039(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		m_CastTime = 1f;
		m_EventTime = 1f;
		m_Duration = 1.5f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		
		PlayAction(AnimatorState.Attack82000,m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1002121",posgo.transform.position,BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			if (m_Dir == WalkDir.WALKRIGHT)
			{
				gae.transform.Rotate(new Vector3(0,180,0),Space.Self);
			}
		}
		if (m_skillinfo.m_LifeTarget is Role)
			posgo = m_skillinfo.m_LifeTarget.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		else
			posgo = null;
		if (posgo != null)
		{
			Vector3 pos = posgo.transform.position;
			pos.z  = -1.5f;
			GameObjectActionExcute  gae= EffectM.LoadEffect(EffectM.sPath,"1002131",posgo.transform.position,posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
			if (m_Dir == WalkDir.WALKRIGHT)
			{
				gae.transform.Rotate(new Vector3(0,180,0),Space.Self);
			}
		}
	}
	
}
public class GridActionCmd100002ActiveSkill :GridActionCmdActiveSkill{

	
	public GridActionCmd100002ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
		m_Duration = 2.667f;///3.667f;
		m_EventTime = 1.4f;//2.367f;
		m_CastTime = 1.4f;//2.267f;
	}

	public override void ActiiveStart()
	{
		SoundPlay.Play("skill_voice_rui", false, false);
		Life  w = m_LifePrent.m_Skill.m_SkillTarget ;
		if( w is Role)
		{
			//Transform EffectPos = (w as Role).m_Skin.ProPerty.m_EffectPos;		
			GameObject posgo = (w as Role).m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
			if (posgo != null)
			{
				Vector3 pos = posgo.transform.position;
				pos.z  = -1.5f;
				//SkillEffects._instance.LoadEffect("effect/prefab/", "1002051",pos,1f);
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1002051", pos, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
				gae.AddAction(ndEffect);
			}
		}
		else
		{
			MapGrid g = w.GetMapGrid();
			Vector3 pos = g.WorldPos;
			pos.y += 1f;
			
			//SkillEffects._instance.LoadEffect("effect/prefab/", "1002051",pos,1f);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1002051", pos, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
			gae.AddAction(ndEffect);
		}
		GameObject pgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
		if (pgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1002031",posgo.transform.position,m_Duration);
			string name = "1002151_0";
			if (!m_LifePrent.m_Core.m_IsPlayer)
				name = "1002151_1";
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, pgo.transform.position, m_LifePrent.GetSkin().ProPerty.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);
		}
        SoundPlay.Play("missile_ready", false, false);
	}
	public override void DoEvent()
	{
		//Transform t = m_Skin.ProPerty.m_EffectPos;		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1002031",posgo.transform.position,m_Duration);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1002031", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
            m_effectgo = gae.gameObject;
			if (m_Dir == WalkDir.WALKRIGHT)
			{
				m_effectgo.transform.Rotate(new Vector3(0,180,0),Space.Self);
				Renderer[] rens = m_effectgo.GetComponentsInChildren<Renderer>();
				foreach(Renderer ren in rens)
				{
					ren.material.mainTextureScale = new Vector2(-1f,1f);
				}
			}
		}
	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_CastTime)
		{

			PlayAction(AnimatorState.Skill01,m_Start);
		}
		else //if ()
		{
			PlayAction(AnimatorState.Skill01,m_Start);
		}
	}
	public override void DoAttack(int count)
	{
		Life target = m_skillinfo.m_LifeTarget;//m_ConditionSkillTarget[PropSkillInfo.m_type];
		MapGrid pos = m_skillinfo.m_TargetPos;//m_ConditionSkillTargetPos[PropSkillInfo.m_type];
		if (m_skillinfo.m_target != 2 && target != null)
			pos = target.GetMapGrid();
		//if (PropSkillInfo.m_type == 1009 || PropSkillInfo.m_type == 1028)
		//	NGUIUtil.DebugLog( "doskill " + PropSkillInfo.m_type  + "," +  m_ConditionSkillTarget[PropSkillInfo.m_type] + "," + PropSkillInfo.m_name,"red");
		//鎾?斁鏀诲嚮鍔ㄧ敾
		//鍙楀嚮鎺夎?
		m_LifePrent.m_Attr.Attacked = true;
		
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		Transform fireplace = posgo.transform;
		if (fireplace != null && target != null)
		{
			string name = "";
			if (m_LifePrent.m_Core.m_IsPlayer == true)
				name = "1002041_0";
			else
				name = "1002041_1";
			
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, name, fireplace.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
			/*if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(5f);
				gae.AddAction(ndEffect);
			}*/
			Bullet bullet = gae.gameObject.AddComponent<Bullet>();
			if (bullet != null)
			{
				
				Vector3 outpos = gae.gameObject.transform.localPosition;
				outpos.x += m_LifePrent.WalkDir == WalkDir.WALKLEFT? -5f : 5f;
				
				Vector3 inpos = pos.pos;
				inpos.x += target.GetMapPos().Unit < m_LifePrent.GetMapPos().Unit? -2f : 2f;
				bullet.SetSkillInfo(pos.pos,m_DoAttack,10f,m_LifePrent.WalkDir,outpos,inpos,Bullet.BulletType.Missile,m_skillinfo as SoldierSkill);
			}
		}
	}

}