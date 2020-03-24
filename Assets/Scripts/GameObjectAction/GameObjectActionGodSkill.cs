using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameObjectActionGodSkill : GameObjectAction
{
	public delegate void DoSkill();
	public DoSkill m_DoAttackFun;
	public float m_CallBackTime;
	public float m_blackscreentime;
	public override void Start ()
	{
		base.Start ();
		Freeze();
	}
	public override void ActionUpdate(float deltatime)
	{
		base.ActionUpdate(deltatime);
		if (m_TimeCount > m_CallBackTime)
		{
			if ((m_TimeCount - deltatime) < m_CallBackTime)
			{
				m_DoAttackFun();
			}
		}
		if (m_TimeCount > m_blackscreentime)
		{
			if ((m_TimeCount - deltatime) < m_blackscreentime)
			{
				UnFreeze();
			}
		}

	}
	public void SetData(DoSkill fun,float blacktime)
	{
		m_DoAttackFun = fun;
		m_blackscreentime = blacktime;
	}
	public override void Finish()
	{
		base.Finish();
		GameObject.Destroy(m_target);
	}
	void Freeze()
	{
		List<Life> listRAW = new List<Life>();
		//CM.SearchLifeMListInBoat(ref listRAW, LifeMType.ALL);
		CM.SearchLifeMList(ref listRAW,null,LifeMType.ALL,LifeMCamp.ALL,MapSearchStlye.AllWorld,null,0);
		foreach (Life item in listRAW)
		{
			item.Pause();
			item.SetDark(true);
		}
		MaterialM.DarkBattleEnv();
		ParticleEffect.StaticPlay(false);
		Bullet.PauseAll();
	}
	void UnFreeze()
	{
		List<Life> listRAW = new List<Life>();
		//CM.SearchLifeMListInBoat(ref listRAW, LifeMType.ALL);
		CM.SearchLifeMList(ref listRAW,null,LifeMType.ALL,LifeMCamp.ALL,MapSearchStlye.AllWorld,null,0);
		foreach (Life item in listRAW)
		{
			item.Contiune();
			item.SetDark(false);
		}
		MaterialM.DarkRevertBattleEnv();
		ParticleEffect.StaticPlay(true);
		Bullet.ContinueAll();
	}
}
/// <summary>
/// 9000 天启导弹
/// </summary>
public class GameObjectActionGodSkill9000 : GameObjectActionGodSkill
{
	public float m_eventtime;
	public float m_eventendtime;
	public float m_fadetime;
	public float m_slowspeed;
	public float m_shaketime;
	public GameObjectActionGodSkill9000()
	{
		m_Duration = 2.8f;
		m_eventtime = 0.92f;
		m_eventendtime = 1f;
		m_CallBackTime = 2.1f;
		m_shaketime = 1.1f;
		m_fadetime = 0.01f;
		m_slowspeed = 0.1f;
	}
	
	public override void ActionUpdate(float deltatime)
	{
		base.ActionUpdate(deltatime);
		/*if (m_TimeCount > m_CallBackTime)
		{
			if ((m_TimeCount - deltatime) < m_CallBackTime)
			{
				m_DoAttackFun();
			}
		}*/
		if (m_TimeCount > m_shaketime)
		{
			if ((m_TimeCount - deltatime) < m_shaketime)
			{
				Camera.main.transform.DOShakePosition(0.8f);
			}
		}
		if (m_TimeCount > (m_eventendtime - m_fadetime))
		{
			Time.timeScale = Mathf.Lerp(m_slowspeed, 1, (m_TimeCount - m_eventendtime + m_fadetime) / m_fadetime);
		}
		else if (m_TimeCount > m_eventtime)
		{
			if (m_TimeCount < (m_eventtime + m_fadetime))
				Time.timeScale = Mathf.Lerp(1, m_slowspeed, (m_TimeCount - m_eventtime) / m_fadetime);
			else if (m_TimeCount > (m_eventtime + m_fadetime))
				Time.timeScale = m_slowspeed;
		}
		//Debug.Log(m_TimeCount + ",  " + Time.timeScale + "," + deltatime + "," + m_eventtime + "," + m_eventendtime + "," + (m_eventendtime - 0.05f));
	}
	public override void Finish()
	{
		
		Time.timeScale = 1;
		base.Finish();
		//GameObject.Destroy(m_target);
	}
}
/// <summary>
/// 使命召唤
/// </summary>
public class GameObjectActionGodSkill9001 : GameObjectActionGodSkill
{
	public float m_eventtime;
	public float m_eventendtime;
	public float m_fadetime;
	
	public Life SkillTarget;
	

	public GameObjectActionGodSkill9001()
	{
		m_Duration = 2.0f;//特效持续时间
		m_CallBackTime = 1.5f;//技能触发时间
	}
	
	public override void Start ()
	{
		base.Start ();
		SkillTarget.SetDark(false);
	}
	public override void ActionUpdate(float deltatime)
	{
		base.ActionUpdate(deltatime);
		if (SkillTarget != null && m_TimeCount <= m_CallBackTime)
		{
			Vector3 pos = SkillTarget.m_thisT.position;
			//pos.z ;
			m_target.transform.position =  EffectCamera.GetEffectPos(pos,40);
		}
       /* if (SkillTarget!=null && m_TimeCount>=m_CallBackTime && m_TimeCount< m_CallBackTime+Time.deltaTime)
        {
            CombatWnd wnd = WndManager.FindDialog<CombatWnd>();
            if (wnd)
            {

                wnd.ShowSkill9001Effect(SkillTarget.SceneID);
            }
        }*/
	}

	
}

