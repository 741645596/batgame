#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 蹦蹦与蹦达技能，编号100003 普攻 1032
/// </summary>
public class GridActionCmd100003Skill01 :GridActionCmdAttack{

    

	public GridActionCmd100003Skill01(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		#if UNITY_EDITOR_LOG
		//Debug.Log("蹦达与蹦蹦普攻" + Time.time);
        #endif
		m_Duration = 1.667f;///3.667f;
		m_EventTime =1.3f;//2.367f;
		m_CastTime = 1.3f;//2.267f;
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		if ((Parent as Role).m_Pet == null)
			(Parent as Role).CreatePet();
		PlayAction(AnimatorState.Attack81000,m_Start);
		//动作光效 蹦大 挥棍		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003061", posgo.transform.position, posgo.transform);
			if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}

		}
		
		if (m_LifePrent is Role)
		{
			Pet pet = (m_LifePrent as Role).CurPet;
			if (pet != null && pet.PetMoveAI is PetWalk1002)
			{
				(pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.toFirePos;

			}
		}
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{

		base.DoEvent();
	}
	public override void DoAttack (int count)
	{
		Role role = m_LifePrent  as Role;
		if (role.CurPet!=null)
		{
			(role.CurPet.PetMoveAI as PetWalk1002).DoSkill(m_DoAttack,m_skillinfo as SoldierSkill,count);
		}
		//m_DoAttack(count);
	}
	
}
//怒气加持
public class GridActionCmd100003Skill02 :GridActionCmdAttack{
	public GridActionCmd100003Skill02(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
    {
        m_Duration = 1.667f;///3.667f;
        m_EventTime = 1f;//2.367f;
		m_CastTime =1f;//2.267f;
		#if UNITY_EDITOR_LOG
        NGUIUtil.DebugLog("蹦达与蹦蹦怒气加持","yellow");
		#endif
	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		//动作光效 着火效果		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
            //m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003091",
               // posgo.transform.position, m_Duration, posgo);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003091", posgo.transform.position, posgo.transform);
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
			gae.AddAction(ndEffect);
            m_effectgo = gae.gameObject;
		}
        PlayAction(AnimatorState.Attack81300, m_Start);
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{
		base.DoEvent();
	}
	
}
/// <summary>
/// 冰冻炸弹
/// </summary>
public class GridActionCmd100003Skill1029:GridActionCmdAttack
{
    public GridActionCmd100003Skill1029(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
    {
		
		m_Duration = 1.667f;///3.667f;
		m_EventTime =1.3f;//2.367f;
		m_CastTime = 1.3f;//2.267f;

	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack81000,m_Start);
		//动作光效 蹦大 挥棍		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003061", posgo.transform.position, posgo.transform);
			if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
			
		}
		if (m_LifePrent is Role)
		{
			Pet pet = (m_LifePrent as Role).CurPet;
			if (pet != null && pet.PetMoveAI is PetWalk1002)
			{
				(pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.toFirePos;
			}
		}
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{

		base.DoEvent();
	}
	
	public override void DoAttack (int count)
	{
		Role role = m_LifePrent  as Role;
		if (role.CurPet!=null)
		{
			(role.CurPet.PetMoveAI as PetWalk1002).DoSkill(m_DoAttack,m_skillinfo as SoldierSkill,count);
		}
		//m_DoAttack(count);
	}
}

/// <summary>
/// 火焰炸弹
/// </summary>
public class GridActionCmd100003Skill1031:GridActionCmdAttack
{
	public GridActionCmd100003Skill1031(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
		:base( qianyaofun, fun,AttackSceneId,AttackDir,deep,skillid)
	{
		
		m_Duration = 1.667f;///3.667f;
		m_EventTime =1.3f;//2.367f;
		m_CastTime = 1.3f;//2.267f;

	}
	public override  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.StartWithTarget(Parent,StartAttack);
		PlayAction(AnimatorState.Attack81000,m_Start);
		//动作光效 蹦大 挥棍		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003061", posgo.transform.position, posgo.transform);
			if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
			
		}
		if (m_LifePrent is Role)
		{
			Pet pet = (m_LifePrent as Role).CurPet;
			if (pet != null && pet.PetMoveAI is PetWalk1002)
			{
				(pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.toFirePos;
			}
		}
	}
	public override   void DoUpdate () {
		
	}
	
	public override void DoEvent()
	{

		base.DoEvent();
	}
	
	public override void DoAttack (int count)
	{
		Role role = m_LifePrent  as Role;
		if (role.CurPet!=null)
		{
			(role.CurPet.PetMoveAI as PetWalk1002).DoSkill(m_DoAttack,m_skillinfo as SoldierSkill,count);
		}
		//m_DoAttack(count);
	}
}
//召唤炮弹兵
public class GridActionCmd100003ConditionSkill01 :GridActionCmdConditionSkill{
	public GridActionCmd100003ConditionSkill01(Vector3 start,Vector3 end,DoAttackFun fun,WalkDir dir,DoQianyaoFun qianyaofun)
	{
		//Debug.Log("蹦达与蹦蹦召唤蹦蹦");
		m_Start = start;
		m_End = end;
		m_CastTime = m_eventtime = 3.87f;
		m_Duration = 5f;
		m_Dir = dir;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
	}
	
	public override void UpdatePos()
	{
        if (m_TimeCount>=m_eventtime && m_TimeCount< m_eventtime+Time.time )
        {
            HideHandPet();
        }
        PlayAction(AnimatorState.Attack81100, m_Start);
        
	}

    private void HideHandPet()
    {
        GameObject go = U3DUtil.FindChildDeep(m_Skin.tRoot.gameObject, "1002@skin");
        if (go)
        {
            go.SetActive(false);
        }
    }
	
}
// 大招召唤炮弹兵
public class GridActionCmd100003ConditionSkill02 : GridActionCmdConditionSkill
{
	public GridActionCmd100003ConditionSkill02(Vector3 start, Vector3 end, DoAttackFun fun, WalkDir dir,DoQianyaoFun qianyaofun)
    {
        Debug.Log("大招召唤炮弹兵");
        m_Start = start;
        m_End = end;
      //  m_CastTime = 0f;//2.267f;
		m_CastTime = m_eventtime = 0f;
        m_Duration = 1f;
        m_Dir = dir;
		m_DoSkill = fun;
		m_QianYaoStatus = qianyaofun;
       
    }

    public override void UpdatePos()
    {
        PlayAction(AnimatorState.Attack81200, m_Start);

    }


}
//大招
public class GridActionCmd100003ActiveSkill :GridActionCmdActiveSkill{

	public GridActionCmd100003ActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base( qianyaofun, fun,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime)
	{
        //分支 
		m_Duration = 4.3f;///3.667f;
		m_EventTime = 1f;//2.367f;
		m_CastTime = 1.3f;//2.267f;
        //分支 end
		//NGUIUtil.DebugLog("蹦达与蹦蹦大招天雨散弹","blue");
	}
	
	public override void ActiiveStart()
	{
        //NGUIUtil.DebugLog("蹦达与蹦蹦大招 天雨散弹  ActiiveStart", "orange");
        //动作光效 蹦大 挥棍
		
		SoundPlay.Play("skill_voice_boom", false, false);
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null)
		{
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003061", posgo.transform.position, posgo.transform);
			if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration);
				gae.AddAction(ndEffect);
				m_effectgo = gae.gameObject;
			}
			else m_effectgo = null; 

		}
        if (m_LifePrent is Role)
        {
			Pet pet = (m_LifePrent as Role).CurPet;
			if (pet != null && pet.PetMoveAI is PetWalk1002)
			{
				(pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.bigSkill;
            }
        }
	}
	public override void DoEvent()
	{

	}
	public override void UpdatePos()
	{
		if (m_TimeCount < m_CastTime)
		{
			PlayAction(AnimatorState.PreSkill01,m_Start);
            if (m_LifePrent is Role)
            {
				Pet pet = (m_LifePrent as Role).CurPet;
				if (pet != null && pet.PetMoveAI is PetWalk1002)
				{
					(pet.PetMoveAI as PetWalk1002).DoQianYao();
                }
            }
		}
		else //if ()
		{
			PlayAction(AnimatorState.Skill01,m_Start);
            if (m_LifePrent is Role)
            {
				Pet pet = (m_LifePrent as Role).CurPet;
				if (pet != null && pet.PetMoveAI is PetWalk1002)
				{
					(pet.PetMoveAI as PetWalk1002).DoDaZhao();
                    if (m_TimeCount < m_CastTime + m_Delatime)
					{
						#if UNITY_EDITOR_LOG
                        NGUIUtil.DebugLog("蹦达与蹦蹦大招 天雨散弹  特效 "+Time.deltaTime, "orange");
						#endif
						//分支		
						GameObject posgo = pet.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
						if (posgo != null)
						{
                        //m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1003071",
                        //    posgo.transform.position, m_Duration - m_EventTime);

							GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1003071", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
							GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_Duration - m_EventTime);
							gae.AddAction(ndEffect);
                        m_effectgo = gae.gameObject;
						}
                        //分支end
                      
                    }
                }
            }
		}
	}

    public override void Finish()
    {
        base.Finish();
        if (m_LifePrent is Role)
        {
			Pet pet = (m_LifePrent as Role).CurPet;
			if (pet != null && pet.PetMoveAI is PetWalk1002)
			{
				(pet.PetMoveAI as PetWalk1002).m_petState = Pet1002State.Follow;
            }
        }
    }
	
	public override void RangeTarget (List<Life> llist)
	{
		base.RangeTarget (llist);
		
		if (llist.Count > 0)
		{
			DoSkill1028(llist[0].GetMapGrid(),(m_skillinfo as SoldierSkill));
		}
		else//如果没有目标,选择当前层最远格子位置
		{
			DoSkill1028(GetFastestGrid(m_LifePrent.GetMapGrid().GridPos.Layer, m_LifePrent.WalkDir),(m_skillinfo as SoldierSkill));
		}
	}
	
	void DoSkill1028(MapGrid pos, SoldierSkill skill)
	{
		if (pos == null)
		{
			return;
		}
		GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		Transform fireplace = posgo.transform;
		if (fireplace != null)
		{
			string bulletname = "1003051";
			
			GameObject go = GameObjectLoader.LoadPath("effect/prefab/", bulletname, BattleEnvironmentM.GetLifeMBornNode(true));
			go.transform.position = fireplace.position;
			if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
				go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
			Bullet bullet = go.AddComponent<Bullet>();
			if (bullet != null)
			{
				Bullet.BulletType t = Bullet.BulletType.bomb1028;
				
				Vector3 vpos = pos.pos;
				vpos.y = go.transform.localPosition.y;
				bullet.SetInfo(vpos, m_DoAttack, 10f, m_LifePrent.WalkDir, t, skill);
			}
			
		}
	}
	
	/// <summary>
	/// 根据移动方向获取最远格子
	/// </summary>
	private MapGrid GetFastestGrid(int layer, WalkDir walkDir)
	{
		MapGrid result = null;
		MapGrid minGrid = null;
		MapGrid maxGrid = null;
		
		Int2 posStart = new Int2(MapSize.GetGridStart(layer), layer);
		minGrid = MapGrid.GetMG(posStart);
		Int2 posEnd = new Int2(MapSize.GetLayerMaxGrid(layer), layer);
		maxGrid = MapGrid.GetMG(posEnd);
		
		if (walkDir == WalkDir.WALKLEFT)
		{
			result = minGrid;
		}
		else
		{
			result = maxGrid;
		}
		
		return result;
	}
	public override void DoAttack (int count)
	{
		
		m_DoAttack(m_skillinfo as SoldierSkill,count);
	}
}