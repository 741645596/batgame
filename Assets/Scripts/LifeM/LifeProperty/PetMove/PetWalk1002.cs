using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Pet1002State
{
	fly = 0,//飞行
	waitKick = 1,//等待被踢
	toFollow = 2,//走到跟随点
	Follow = 3,//跟随
	Free = 4,//自主阶段
	toFirePos = 5,//到被点的位置
	toAttackPos = 6,//到攻击位置
	bigSkill = 7,
	goStair = 8, //下楼梯
	stand = 9,
}
public class PetWalk1002 : PetWalk {
	
	//public delegate void DoAttackFun(SoldierSkill info, int times);
	
	public DoAttackFun m_DoAttackFun;

	private bool m_bStop = false;
	private bool m_bGotoFirePos = false;
	private bool m_bGotoAttackPos = false;
	private SoldierSkill m_skillInfo;
	private int m_times;
	/// <summary>
	/// 弹点燃状态引线火星效果
	/// </summary>
	private GameObject m_effectGo = null;
	
	private Vector3 m_vFollowPos;
	public SoldierSkill m_skill;
	public Pet1002State m_petState = Pet1002State.fly;
	
	public PetFollow m_petFollow;
	private GameObject m_effect = null;
	// Use this for initialization
	
	public override void Init (Pet pet) {
		base.Init(pet);
		
		m_run = new PetGridRun(m_Owner);
		GameObject posgo =  m_Owner.m_Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.petFollowPos);
		if (posgo != null)
		{
			m_vFollowPos = posgo.transform.position;
		}
	}

	// Update is called once per frame
	public override void Update () {
		m_Owner.m_Skin.SetMirror(m_Owner.m_Parent.WalkDir);
		switch(m_petState)
		{
		case Pet1002State.Follow:
			FollowMother();
			break;
			
		case Pet1002State.toFirePos:
			DoToFirePos();
			break;
			
		case Pet1002State.toAttackPos:
			DoToAttackPos();
			break;
			
		case Pet1002State.bigSkill:
			GotoAttackPosNow();
			break;
			
		case Pet1002State.goStair:
			ReturnMotherEmbrace();
			break;
			
		case Pet1002State.stand:
			DoStand();
			break;
		}
	}
	/// <summary>
	/// 跟随母体
	/// </summary>
	void FollowMother()
	{
		m_Owner.m_Skin.SetVisable(true);
		if (m_petFollow == null)
		{
			m_petFollow = m_Owner.m_Skin.tRoot.gameObject.AddComponent<PetFollow>();
		}
		m_petFollow.m_IsFollow = true;
		GameObject posgo =  m_Owner.m_Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.petFollowPos);
		m_petFollow.SetFollowTarget(posgo.transform);
		AnimatorState state = AnimatorState.Walk;
		m_Owner.m_Skin.Move(state, Vector3.zero, ActionMode.Delta);
		if (m_Owner.m_Parent is Role)
		{
			RoleGridRun run = (m_Owner.m_Parent as Role).run;
			Int2 Pos = m_Owner.m_Parent.GetMapPos();
			if (run != null)
			{
				int flag = (run.WalkDIR == WalkDir.WALKLEFT)? -1 : 1;
				Pos.Unit += flag;
			}
			m_Owner.MapPos = Pos;
		}
	}
	/// <summary>
	/// 到点火位置
	/// </summary>
	void DoToFirePos()
	{
		m_Owner.m_Skin.SetVisable(true);
		if (m_petFollow)
		{
			m_petFollow.m_IsFollow = false;
		}
		
		if (!m_bGotoFirePos)
		{
			ToFirePos();
		}
		if (m_CurrentAction == null)
		{
			
		}
		else if (m_CurrentAction.IsDone())
		{
			// m_petState = Pet1002State.toAttackPos;
		}
		if (m_CurrentAction != null)
		{
			m_CurrentAction.Update();
		}
	}
	/// <summary>
	/// 向点燃位置移动
	/// </summary>
	void ToFirePos()
	{
		//NGUIUtil.DebugLog("向点燃位置移动", "red");
		m_bGotoFirePos = true;
		m_Owner.m_Skin.tRoot.transform.parent = BattleEnvironmentM.GetLifeMBornNode(true);
		Vector3 start = m_Owner.m_Parent.GetSkin().tRoot.localPosition;
		GameObject posgo =  m_Owner.m_Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.PetPos);
		Vector3 end = posgo.transform.position;
		end = BattleEnvironmentM.World2LocalPos(end);
		m_CurrentAction = new GridActionCmdWalk(start, end, 0.5f, m_Owner.m_Parent.WalkDir, 0);
		m_CurrentAction.SetTarget(m_Owner);
	}
	/// <summary>
	/// 到攻击目标位置
	/// </summary>
	private void DoToAttackPos()
	{
		m_Owner.m_Skin.SetVisable(true);
		if (m_petFollow)
		{
			m_petFollow.m_IsFollow = false;
		}
		
		if (!m_bGotoAttackPos)
		{
			m_bGotoAttackPos = true;
			GameObject posgo = m_Owner.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
			if (posgo != null)
			{
				GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "1003041" ,posgo.transform.position ,posgo.transform);
				if(gae != null)
				{
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(10f);
					gae.AddAction(ndEffect);
					m_effect = gae.gameObject;
				}
			}
		}
		if (m_CurrentAction == null)
		{
			m_CurrentAction = m_run.GetNextAction();
		}
		else if (m_CurrentAction.IsDone())
		{
			GridActionCmd preaction = m_CurrentAction;
			m_CurrentAction = m_run.GetNextAction();
			
			DoPathEvent(preaction);
		}
		if (m_CurrentAction != null && m_petState!= Pet1002State.Follow)
		{
			m_CurrentAction.Update();
		}
		else
		{
			DoAttack();
		}
	}
	/// <summary>
	/// 执行攻击
	/// </summary>
	private void DoAttack()
	{
		if (m_effect != null)
		{
			/*m_effect.DestroyEffect();
			m_effect = null;*/
			GameObject.Destroy(m_effect);
		}
		GameObject.Destroy(m_effectGo);
		GameObject posgo = m_Owner.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);
		if (posgo != null && m_Owner.m_target!=null)
		{ 
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "1003031" ,posgo.transform.position ,m_Owner.m_target.m_thisT);
			if(gae != null)
			{
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.3f);
				gae.AddAction(ndEffect);
			}
		}
		m_Owner.m_Skin.tRoot.position = m_Owner.m_Parent.GetSkin().tRoot.position;
		m_DoAttackFun(m_skillInfo, m_times);
		m_petState = Pet1002State.stand;
		m_CurrentAction = null;
		m_bGotoFirePos = false;
		m_bGotoAttackPos = false;
	}
	public void DoPathEvent(GridActionCmd preaction)
	{
		LifeMCamp Camp = (m_Owner.m_Core.m_Camp == LifeMCamp.ATTACK) ? LifeMCamp.DEFENSE :LifeMCamp.ATTACK;
		List<Life> RoleList = new List<Life>();
		CM.SearchLifeMListInBoat(ref RoleList,LifeMType.SOLDIER | LifeMType.SUMMONPET,Camp);
		if (m_CurrentAction is GridActionCmdStair || m_CurrentAction is GridActionCmdJump || m_CurrentAction is GridActionCmdFall )
			AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_INA),RoleList);
		else if (preaction is GridActionCmdStair ||preaction is GridActionCmdJump ||preaction is GridActionCmdSpecialJump
		         || m_CurrentAction is GridActionCmdFallDown )
			AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_FMA),RoleList);
		else if (m_CurrentAction is GridActionCmdWalk)
			AIPathConditions.AIPathEvent(new AIEventData(AIEvent.EVENT_TM),RoleList);
	}
	void GotoAttackPosNow()
	{
		m_Owner.m_Skin.SetVisable(true);
		if (m_petFollow)
		{
			m_petFollow.m_IsFollow = false;
		}
		
		GameObject posgo = m_Owner.m_Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.PetPos);
		Vector3 end = posgo.transform.position;
		m_Owner.m_Skin.tRoot.position = end;
		//分支
		//m_Skin.Move(AnimatorState.Skill01, Vector3.zero, ActionMode.Delta);
		//分支end
	}
	/// <summary>
	/// 回到母体怀抱
	/// </summary>
	void ReturnMotherEmbrace()
	{
		m_Owner.m_Skin.SetVisable(true);
		if (m_petFollow!= null)
		{
			m_petFollow.m_IsFollow = true;
			GameObject posgo =  m_Owner.m_Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.NavelPos);
			m_petFollow.SetFollowTarget(posgo.transform);
			AnimatorState state = AnimatorState.Stand;
			m_Owner.m_Skin.Move(state, Vector3.zero, ActionMode.Delta);
		}
	}
	void DoStand()
	{
		FollowMother();
		m_Owner.m_Skin.SetVisable(false);
	}
	public void DoQianYao()
	{
		m_Owner.m_Skin.Move(AnimatorState.PreSkill01, Vector3.zero, ActionMode.Delta);
	}
	public void DoDaZhao()
	{
		m_Owner.m_Skin.Move(AnimatorState.Skill01, Vector3.zero, ActionMode.Delta);
	}
	/// <summary>
	/// 攻击技能释放
	/// </summary>
	public void DoSkill(DoAttackFun DoAttackFun,SoldierSkill info ,int times)
	{
		if (info.m_type == 1029 || info.m_type == 1031)
		{
			string name =  "1003101";
			if (info.m_type == 1031)
				name =  "1003111";
			GameObject posgo = m_Owner.m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
			if (posgo != null)
			{
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath,name, posgo.transform.position, posgo.transform);
				m_effectGo = gae.gameObject;
				if(gae != null)
				{
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
					gae.AddAction(ndEffect);
				}
				
			}
		}
		m_petState = Pet1002State.toAttackPos;
		m_DoAttackFun = DoAttackFun;
		m_skillInfo = info;
		m_times = times;
		m_Owner.m_target = m_Owner.m_Parent.m_Skill.m_AttackTarget;
		Int2 mappos = m_Owner.m_Parent.GetMapPos();
		if ((m_Owner.m_Parent as Role).WalkDir == WalkDir.WALKLEFT) 
			mappos.Unit -= 3;
		else
			mappos.Unit +=3;
		m_run.SetBorn(mappos, 0);
		m_run.UpdataPath();
	}
}
