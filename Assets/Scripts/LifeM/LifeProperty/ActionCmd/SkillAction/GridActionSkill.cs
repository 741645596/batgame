using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GridActionSkill : GridActionCmd {
	public virtual void RangeTarget(List<Life> llist)
	{
	}
	public override float GetSpeed ()
	{
		if (m_LifePrent != null)
			return m_LifePrent.m_Attr.AttackSpeed;
		return 1;
	}
}


public delegate bool StartAttackFun(int SceneID);
public delegate void DoAttackFun(SkillInfo skill,int times);
public delegate void DoQianyaoFun();

public class GridActionCmdAttack :GridActionSkill{
	// Update is called once per frame
	protected bool QianYaoPlayed  = false;
	protected bool m_Played  = false;
	protected float m_CastTime = 0;
	protected float m_EventTime = 0;
	protected StartAttackFun  m_StartAttack;
	protected DoAttackFun m_DoAttack;
	protected DoQianyaoFun m_QianYaoStatus;
	protected int m_effectcount;
	//产生次数
	protected int m_effecttime;
	//间隔
	protected List<float> m_timeinterval;
	protected SkillInfo m_skillinfo;
	public bool Played
	{
		get{return m_Played;}
		set{m_Played = value;}
	}
	public void SetAttackFun(DoAttackFun fun)
	{
		m_DoAttack = fun;
	}
	public GridActionCmdAttack(DoQianyaoFun qianyaofun,DoAttackFun fun,int AttackSceneId,WalkDir AttackDir,int deep,int skillid)
	{
		m_QianYaoStatus = qianyaofun;
		//m_Duration = RoleModelsM.GetSkillResourcesData(skillid,"TotalTime")/1000f;
		//m_EventTime = RoleModelsM.GetSkillResourcesData(skillid,"TriggerTime")/1000f;
		m_CastTime = m_EventTime;
		m_DoAttack = fun;
		m_AttackSceneID = AttackSceneId;
		m_Dir = AttackDir;
		m_RankDeep = deep;
		m_effectcount = 0;
		m_effecttime = 1;
		m_timeinterval = new List<float>();
		m_timeinterval.Add(0f);
	}
	public void SetAttackInfo(int effecttime,List<float> timeinterval)
	{
		m_effecttime = effecttime;
		m_timeinterval = timeinterval;
	}
	public virtual  void StartWithTarget(Life Parent,StartAttackFun StartAttack)
	{
		base.SetTarget(Parent);
		m_skillinfo = (Parent as Role).PropSkillInfo;

		m_Start = Parent.m_thisT.localPosition;
		if (StartAttack != null)
			StartAttack(m_AttackSceneID);
		//PlayAction(AnimatorState.Attack01,m_Start);
	}
	public override   void Update () {
		base.Update();
		if (m_TimeCount > m_CastTime && !QianYaoPlayed)
		{
			m_QianYaoStatus();
			DoEvent();
			QianYaoPlayed = true;
		}
		if (m_effectcount < m_effecttime && m_TimeCount > (m_EventTime + m_timeinterval[m_effectcount]/*m_timeinterval * m_effectcount*/))
		{
			if (m_TimeCount - m_Delatime < (m_EventTime + m_timeinterval[m_effectcount]/*m_timeinterval * m_effectcount*/))
			{
				Played = true;
				m_effectcount ++;
				DoAttack(m_effectcount);
				DoAttackEvent();
			}
			//DoEvent();
		}
		
		DoUpdate();
	}

	public virtual void DoAttack(int count)
	{
		if (count == 1)
		{
			SoldierSkill skill = m_skillinfo as SoldierSkill;
			if (skill.m_Screen == 1)
			{
				MainCameraM.s_Instance.transform.DOShakePosition(skill.m_ScreenTime);
			}
		}
		Life target = m_skillinfo.m_LifeTarget;//m_ConditionSkillTarget[PropSkillInfo.m_type];
		MapGrid pos = m_skillinfo.m_TargetPos;//m_ConditionSkillTargetPos[PropSkillInfo.m_type];
		if (m_skillinfo.m_target != 2 && target != null)
			pos = target.GetMapGrid();
		//if (PropSkillInfo.m_type == 1009 || PropSkillInfo.m_type == 1028)
		//	NGUIUtil.DebugLog( "doskill " + PropSkillInfo.m_type  + "," +  m_ConditionSkillTarget[PropSkillInfo.m_type] + "," + PropSkillInfo.m_name,"red");
		//播放攻击动画
		//受击掉血
		m_LifePrent.m_Attr.Attacked = true;
		
		if (m_skillinfo.m_skilleffectinfo != null && m_skillinfo.m_type != 1005)//RoleModelsM.GetSkillResourcesData(PropSkillInfo.m_type,"HasBullet") == 1)
		{
			GameObject posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.FirePos);

			/*if (m_skillinfo.m_type == 1041)
				posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
			else if (m_skillinfo.m_type == 1044)
				posgo = m_LifePrent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.bagPos);*/
			Transform fireplace = posgo.transform;
			if (fireplace != null)
			{
				string bulletname = m_skillinfo.m_skilleffectinfo.m_targeteffect;
				
				if (BattleEnvironmentM.GetLifeMBornNode(true) == null)
				{
					return;
				}
				GameObject go = GameObjectLoader.LoadPath("effect/prefab/",bulletname,fireplace.position,BattleEnvironmentM.GetLifeMBornNode(true));
				if (m_LifePrent.WalkDir == WalkDir.WALKRIGHT)
					go.transform.Rotate(0,180,0);
					//go.transform.localScale = new Vector3(-go.transform.localScale.x,go.transform.localScale.y,go.transform.localScale.z);
				Bullet bullet = go.AddComponent<Bullet>();
				if (bullet != null)
				{
					Bullet.BulletType t = Bullet.BulletType.Bullet;
					Vector3 vpos = pos.pos;
					if (m_skillinfo.m_skilleffectinfo.m_postion == 0)
						vpos.y = go.transform.localPosition.y;
					else
						vpos.y += 0.2f;
					bullet.SetInfo(vpos,m_DoAttack,5f,m_LifePrent.WalkDir,t,(m_skillinfo as SoldierSkill),true);
						//vpos,m_DoAttack,10f,m_LifePrent.WalkDir,t,m_skillinfo as SoldierSkill);
				}
			}
		}
		else
		{
			m_DoAttack(m_skillinfo as SoldierSkill,count);
		}
		//m_DoAttack(count);
	}

	public virtual void DoAttackEvent()
	{
	}
	public virtual void DoEvent()
	{
	}
	public virtual void DoUpdate()
	{
		
	}
	/*public override  void SetDone()
	{
		m_Duration = m_TimeCount;
		Finish();
	}*/
	
	public void SetDuration(float duration)
	{
		m_Duration = duration;
	}
	public bool IsPlayed()
	{
		return QianYaoPlayed;
	}
}



/// <summary>
/// 主动技能
/// </summary>
public class GridActionCmdActiveSkill :GridActionCmdAttack{
	
	protected bool m_bRunOnce;
	protected AudioSource audioSource = null;
	protected int m_iSceneID;
	protected float m_blackscreentime;
    LifeMCore m_lifeMCore;
	public float m_StartTime;
	public static List<GridActionCmdActiveSkill> s_activeskill = new List<GridActionCmdActiveSkill>();
	public GridActionCmdActiveSkill(DoQianyaoFun qianyaofun,DoAttackFun fun,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
		:base(qianyaofun,fun,AttackSceneId,AttackDir,deep,skillid)
	{
        m_iSceneID = sceneID;
		m_bRunOnce = true;
		m_LifePrent = CM.GetLifeM(m_iSceneID,LifeMType.SOLDIER);
		m_lifeMCore = m_LifePrent.m_Core;
		m_blackscreentime = blackscreentime;
		s_activeskill.Add(this);
        //Debug.Log("aaa:start="+ Time.time);
	}

	public virtual void ActiiveStart()
	{
	}

	public  void StartWithTarget(Life Parent)
	{
		m_StartTime = Time.time;
		Vector3 scale = new Vector3(1.25f,1.25f,1.25f);
		base.StartWithTarget(Parent,null);
		
		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null)
		{
			//m_effectgo = SkillEffects._instance.LoadEffect("effect/prefab/", "1000011",posgo.transform.position,1.5f);
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1000011", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);
            m_effectgo = gae.gameObject;
		}
		Role.RefreshSkin();
		//PlayAction(AnimatorState.Skill01,m_Start);
		
		(m_LifePrent as Role).SavePosZ();
        if (m_lifeMCore!=null)
        {
            if (m_lifeMCore.m_IsPlayer)
            {
				//Debug.Log(" 己方释放大招！！！");
				List<Life> listRAW = new List<Life>();
				m_LifePrent.SetDark(false);
				//CM.SearchLifeMListInBoat(ref listRAW, LifeMType.ALL);
				CM.SearchLifeMList(ref listRAW,null,LifeMType.ALL,LifeMCamp.ALL,MapSearchStlye.AllWorld,null,0);
				List<Role> rs = new List<Role>();
				bool ispause = false;
				foreach (Life item in listRAW)
				{

					if(item != m_LifePrent )
					{
						if (item is Role)
						{
							Role r = item as Role;
							if (r.CurrentAction != null && r.CurrentAction is GridActionCmdActiveSkill)
							{
								float starttime = (r.CurrentAction as GridActionCmdActiveSkill).m_StartTime;
								if (starttime >= 0)
								{
									if ( (m_StartTime - starttime) > ConfigM.GetBigSkillDelatime())
										ispause = true;
									rs.Add (r);
								}
								else
								{
									item.Pause();
									item.SetDark(true);
								}
							}
							else
							{
								item.Pause();
								item.SetDark(true);
							}
						}
						else
						{
							item.Pause();
							item.SetDark(true);

						}
					}
					 
				}
				foreach (Role r in rs)
				{
					if (ispause)
					{
						r.Pause();
						r.SetDark(true);
						(r.CurrentAction as GridActionCmdActiveSkill).m_StartTime = -1f;
					}
				}
				MaterialM.DarkBattleEnv();
				m_Skin.Scale(scale);
				ParticleEffect.StaticPlay(false);
				Bullet.PauseAll();
            }
		}
		ActiiveStart();
		if (m_skillinfo.m_LifeTarget != null)
			m_skillinfo.m_LifeTarget.SetDark(false);
		else
		{

		}
	}

    void UnFreeze()
    {
		m_Skin.Scale(new Vector3(1.0f, 1.0f, 1.0f));
		List<Life> listRAW = new List<Life>();
		//CM.SearchLifeMListInBoat(ref listRAW, LifeMType.ALL);
		CM.SearchLifeMList(ref listRAW,null,LifeMType.ALL,LifeMCamp.ALL,MapSearchStlye.AllWorld,null,0);
		foreach (Life item in listRAW)
		{
			item.Contiune();
			item.SetDark(false);
		}
		MaterialM.DarkRevertBattleEnv();
		(m_LifePrent as Role).ResetPos();
		ParticleEffect.StaticPlay(true);
		Bullet.ContinueAll();
    }

	public virtual void UpdatePos()
	{
	}

	public override  void DoUpdate () {

		if (m_TimeCount >= m_blackscreentime)
		{
            if (m_bRunOnce)
            {
                m_bRunOnce = false;
                if (m_lifeMCore.m_IsPlayer)
                {
					
					List<Life> listRAW = new List<Life>();
					CM.SearchLifeMListInBoat(ref listRAW, LifeMType.SOLDIER,m_lifeMCore.m_Camp);
					bool isunfreeze = true;
					foreach (Life item in listRAW)
					{
						if(item != m_LifePrent )
						{
							Role r = item as Role;
							if (r.CurrentAction is GridActionCmdActiveSkill)
							{
								if ((r.CurrentAction as GridActionCmdActiveSkill).m_StartTime >= 0 && !(r.CurrentAction as GridActionCmdActiveSkill).IsBlackDone())
								{
									isunfreeze = false;
								}
							}
						}
					}
					if (isunfreeze)
						UnFreeze();
					//UnFreeze();
				}
				else
					(m_LifePrent as Role).ResetPos();
            }
          
		}

        //if(m_TimeCount>2f)//锤子音效只播放前面的0.5秒 临时方案
        //{
        //    if(audioSource)
        //        audioSource.Stop();
        //}
		UpdatePos();
        if (m_TimeCount <= m_CastTime)
        {
			for(int i = 0; i < s_activeskill.Count;)
			{
				if (s_activeskill[i] == null || s_activeskill[i].m_LifePrent == null)
					s_activeskill.RemoveAt(i);
				else
				{
					if (s_activeskill[i]  == this)
					{
						(m_LifePrent as Role).SetTopMostPos(i);
						break;
					}
					i++;
				}
			}
              
        }
	}
	
	public override void Finish()
	{
		//2015.4.22 by lqf add
		//分支start
		base.Finish();
		//分支end
        m_Skin.Scale(Vector3.one);
		UnFreeze();
		s_activeskill.Remove(this);
	}
	public bool IsBlackDone()
	{
		//Debug.Log(m_LifePrent + "," + m_TimeCount + "," + m_blackscreentime);
		return m_TimeCount >= m_blackscreentime;
	}

	
}



public class GridActionCmdAttackFactory
{
	/// <summary>
	/// 创建GridActionCmdAttack
	/// </summary>
	public static GridActionCmdAttack Create(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid, int SoldierID,float blackscreentime)
	{
		switch(SoldierID)
		{
		case 100001:
			return Create100001(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 100002:
			return Create100002(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 100003:
			return Create100003(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 100004:
			return Create100004(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 101001:
			return Create101001(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 101003:
			return Create101003(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
        case 101004:
            return Create101004(qianyaofun, NormalSkill, BigSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime);
            break;
        case 103001:
            return Create103001(qianyaofun, NormalSkill, BigSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime);
            break;
		case 103003:
			return Create103003(qianyaofun, NormalSkill, BigSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime);
			break;
		case 102001:
			return Create102001(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 102002:
			return Create102002(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 102005:
			return Create102005(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200000:
			return Create100001(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200001:
			return Create100002(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200002:
			return Create200002(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 102003:
			return Create102003(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200009:
			return Create200009(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 3000:
			return Create3000(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200004:
			return Create200004(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200005:
			return Create200005(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200006:
			return Create200006(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
		case 200007:
			return Create200007(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
		case 102004:
			return Create102004(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
		case 103002:
			return Create103002(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		case 200008:
			return Create200008(qianyaofun,NormalSkill,BigSkill,sceneID,AttackSceneId,AttackDir,deep,skillid,blackscreentime);
			break;
		default: 
				return null;
			break;
		}
		return null;
	}
	/// <summary>
	/// 豆豆技能 编号100001
	/// </summary>
	private static GridActionCmdAttack Create100001(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1000)
			return new GridActionCmd100001ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1001)
			return new GridActionCmd100001Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1002)
			return new GridActionCmd100001Skill02(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1036)
			return new GridActionCmd100001Skill03(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1037)
			return new GridActionCmd100001Skill1037(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1047)
			return new GridActionCmd100001LSkill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 火枪瑞哥技能 编号100002
	/// </summary>
	private static GridActionCmdAttack Create100002(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1003)
			return new GridActionCmd100002ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1004)
			return new GridActionCmd100002Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1005)
			return new GridActionCmd100002Skill02(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1039)
			return new GridActionCmd100002Skill1039(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 蹦蹦与蹦大技能，编号100003
	/// </summary>
	private static GridActionCmdAttack Create100003(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1028)
			return new GridActionCmd100003ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1032)
			return new GridActionCmd100003Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1030)
			return new GridActionCmd100003Skill02(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
        else if (skillid == 1029)//冰冻炸弹
        {
            return new GridActionCmd100003Skill1029(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        }
        else if (skillid == 1031)//火焰炸弹
        {
			return new GridActionCmd100003Skill1031(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        }

		return null;
	}

	/// <summary>
	///  二白技能 编号100004
	/// </summary>
	private static GridActionCmdAttack Create100004(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1052)
			return new GridActionCmd100004ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1051)
			return new GridActionCmd100004Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 一刀喵技能，编号101001
	/// </summary>
	private static GridActionCmdAttack Create101001(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1007)
			return new GridActionCmd101001ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1006)
			return new GridActionCmd101001Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1009)
			return new GridActionCmd101001Skill02(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1048)
			return new GridActionCmd101001LSkill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}

	/// <summary>
	/// 魔术喵菲尔技能，编号101003
	/// </summary>
	private static GridActionCmdAttack Create101003(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1057)
			return new GridActionCmd101003ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		if(skillid == 1056)
			return new GridActionCmd101003Skill1056(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1058)
			return new GridActionCmd101003Skill1058(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1059)
			return new GridActionCmd101003Skill1059(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
    /// <summary>
    /// 魔术喵菲尔技能，编号101004
    /// </summary>
    private static GridActionCmdAttack Create101004(DoQianyaoFun qianyaofun, DoAttackFun NormalSkill, DoAttackFun BigSkill, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
    {
        if (skillid == 1081)
            return new GridActionCmd101004Skill1081(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        if (skillid == 1084)
            return new GridActionCmd101004Skill1084(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        if (skillid == 1082)
            return new GridActionCmd101004ActiveSkill(qianyaofun, BigSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime);
        return null;
    }

    /// <summary>
    /// 美人鱼技能，编号103001
    /// </summary>
    private static GridActionCmdAttack Create103001(DoQianyaoFun qianyaofun, DoAttackFun NormalSkill, DoAttackFun BigSkill, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
    {
        if (skillid == 1091)
            return new GridActionCmd103001Skill1091(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        if (skillid == 1093)
            return new GridActionCmd103001Skill1093(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        if (skillid == 1094)
            return new GridActionCmd103001Skill1094(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
        if (skillid == 1092)
            return new GridActionCmd103001ActiveSkill(qianyaofun, BigSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime);
        return null;
    }

	/// <summary>
	/// 火狐技能，编号103003
	/// </summary>
	private static GridActionCmdAttack Create103003(DoQianyaoFun qianyaofun, DoAttackFun NormalSkill, DoAttackFun BigSkill, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
	{
		if (skillid == 1111)
			return new GridActionCmd103003Skill1111(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
		if (skillid == 1114)
			return new GridActionCmd103003Skill1114(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
		if (skillid == 1113)
			return new GridActionCmd103003Skill1113(qianyaofun, NormalSkill, AttackSceneId, AttackDir, deep, skillid);
		if (skillid == 1112)
			return new GridActionCmd103003ActiveSkill(qianyaofun, BigSkill, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime);
		return null;
	}

	/// <summary>
	/// 熊孩子技能 编号102001
	/// </summary>
	private static GridActionCmdAttack Create102001(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1022)
			return new GridActionCmd102001ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1021)
			return new GridActionCmd102001Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1049)
			return new GridActionCmd102001LSkill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 大奶熊阿厨技能 编号102001
	/// </summary>
	private static GridActionCmdAttack Create102002(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1042)
			return new GridActionCmd102002ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1041)
			return new GridActionCmd102002Skill01(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1044)
			return new GridActionCmd102002Skill03(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 小奶熊阿厨技能 编号102005
	/// </summary>
	private static GridActionCmdAttack Create102005(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1042)
			return new GridActionCmd102005ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1041)
			return new GridActionCmd102005Skill1041(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1044)
			return new GridActionCmd102005Skill1044(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 巫师企鹅技能 编号200002
	/// </summary>
	private static GridActionCmdAttack Create200002(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 5000)
			return new GridActionCmd200002Skill5000(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 绿巨熊技能 编号102003
	/// </summary>
	private static GridActionCmdAttack Create102003(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1063)
			return new GridActionCmd102003ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1061)
			return new GridActionCmd102003Skill1061(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1062)
			return new GridActionCmd102003Skill1062(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 绿巨熊技能 编号102003
	/// </summary>
	private static GridActionCmdAttack Create200009(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1061)
			return new GridActionCmd200009Skill1061(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1062)
			return new GridActionCmd200009Skill1062(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 5011)
			return new GridActionCmd200009Skill5011(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 5012)
			return new GridActionCmd200009Skill5012(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 拆解机器人技能 编号200002
	/// </summary>
	private static GridActionCmdAttack Create3000(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 7000)
			return new GridActionCmd3000Skill7000(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 医师企鹅技能 编号200004
	/// </summary>
	private static GridActionCmdAttack Create200004(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 5005)
			return new GridActionCmd200004Skill5005(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 刀盾企鹅技能 编号200005
	/// </summary>
	private static GridActionCmdAttack Create200005(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 5008)
			return new GridActionCmd200005Skill5008(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}


	/// <summary>
	/// 弩炮熊技能 编号200006
	/// </summary>
	private static GridActionCmdAttack Create200006(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 5009)
			return new GridActionCmd200006Skill5009(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		return null;
	}

	/// <summary>
	/// 弓箭企鹅技能 编号200007
	/// </summary>
	private static GridActionCmdAttack Create200007(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 5007)
			return new GridActionCmd200007Skill5007(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}

	
	/// <summary>
	/// 维京熊技能 编号102004
	/// </summary>
	private static GridActionCmdAttack Create102004(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1072)
			return new GridActionCmd102004ActiveSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1071)
			return new GridActionCmd102004Skill1071(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1074)
			return new GridActionCmd102004Skill1074(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1072)
			return new GridActionCmd102004Skill1072(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}

	/// <summary>
	/// 黑胡子技能 编号103002
	/// </summary>
	private static GridActionCmdAttack Create103002(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 1102)
			return new GridActionCmd103002BigSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1101)
			return new GridActionCmd103002SkillNomalAttack(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1103)
			return new GridActionCmd103002Skill1103(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 1104)
			return new GridActionCmd103002Skill1104(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}
	/// <summary>
	/// 黑胡子Boss技能 编号200008
	/// </summary>
	private static GridActionCmdAttack Create200008(DoQianyaoFun qianyaofun,DoAttackFun NormalSkill,DoAttackFun BigSkill,int sceneID,int AttackSceneId,WalkDir AttackDir,int deep,int skillid,float blackscreentime)
	{
		if(skillid == 5019)
			return new GridActionCmd200008BigSkill(qianyaofun,BigSkill,sceneID, AttackSceneId, AttackDir, deep,skillid,blackscreentime);
		else if(skillid == 1101)
			return new GridActionCmd200008SkillNomalAttack(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		else if(skillid == 5020)
			return new GridActionCmd200008Skill1101(qianyaofun,NormalSkill, AttackSceneId, AttackDir, deep,skillid);
		return null;
	}

}