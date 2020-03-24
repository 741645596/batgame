using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void SkillUseFun(SkillInfo skill,int times,List<Life> targetlist);
public class ThroughBullet : MonoBehaviour {

	public static List<ThroughBullet> s_Bullet = new List<ThroughBullet>();
	public List<Life> m_hitRole = new List<Life>();
	
	public WalkDir m_dir;
	public Vector3 m_Start;
	public Vector3 m_Destination;
	//public delegate void CallBackFun(SoldierSkill skill,int times);
	public SkillUseFun m_fun;
	public float m_Speed;
	public SoldierSkill m_skillinfo;
	public SkillEffectInfo m_info;
	float m_TimeCount;
	float m_Duration;
	bool m_Skill;
	GameObject effect;
	bool m_bpause = false;
	LifeMCamp m_camp;
	int m_layer = 0;
	// Use this for initialization
	void Start () {
		m_TimeCount = 0;
		s_Bullet.Add(this);
	}
	void OnDestroy()
	{
		s_Bullet.Remove(this);
	}
	public void Pause()
	{
		m_bpause = true;
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem p in ps)
		{
			p.Pause();
		}
	}
	public void Continue()
	{
		m_bpause = false;
		ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem p in ps)
		{
			p.Play();
		}
		
	}
	
	public static void PauseAll()
	{
		for(int i = 0; i < s_Bullet.Count; i++)
			s_Bullet[i].Pause();
	}
	public static void ContinueAll()
	{
		for(int i = 0; i < s_Bullet.Count; i++)
			s_Bullet[i].Continue();
	}
	public void SetInfo(Vector3 dest,SkillUseFun fun,float speed,WalkDir dir,SoldierSkill skill,LifeMCamp camp,int layer, bool isPlaySound=true)
	{
		m_dir = dir;
		m_Skill = false;
		m_skillinfo = skill;
		m_Start = transform.localPosition;
		m_Start.z = -0.5f; 
		m_Destination = dest;
		m_Destination.z = -0.5f;
		//m_Destination.y = m_Start.y;
		m_fun = fun;
		m_Speed = speed;
		m_Duration = Mathf.Abs(transform.localPosition.x - m_Destination.x) /speed;
		//effect = SkillEffects._instance.LoadEffect("effect/prefab/", "1002011",transform.position,0.5f);
		m_camp = camp;
		m_info = m_skillinfo.m_skilleffectinfo;
		m_layer = layer;
		
		//Debug.Log("bullet:" + transform.localPosition + ","  +m_Destination + "," + m_Speed + "," + m_Duration + ","+Mathf.Abs(transform.localPosition.x - m_Destination.x));
	}
	// Update is called once per frame
	void Update () {
		if (m_bpause)
			return;
		m_TimeCount += Time.deltaTime;
		if (m_TimeCount < m_Duration)
		{
			Vector3 pos = Vector3.Lerp(m_Start,m_Destination,m_TimeCount / m_Duration);
			transform.localPosition = pos;
			
			List<Life> l = new List<Life>();
			CM.SearchLifeMListInBoat(ref l,LifeMType.SOLDIER, m_camp);
			List<Life> hit = new List<Life>();
			for(int i = 0; i < l.Count; i++)
			{
				if (m_layer == l[i].GetMapPos().Layer)
				{
					if (transform.localPosition.x > (l[i].m_thisT.localPosition.x - 0.25f) && transform.localPosition.x > (l[i].m_thisT.localPosition.x + 0.25f))
					{
						if (!l[i].m_isDead && !m_hitRole.Contains(l[i]))
						{
							m_hitRole.Add(l[i]);
							hit.Add(l[i]);
							if (m_info.m_hiteffect!=0)
							{
								pos = l[i].m_thisT.position;
								pos.z = -1.5f;
								GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, m_info.m_hiteffect.ToString(), pos, BattleEnvironmentM.GetLifeMBornNode(true));
								GameObjectActionEffectInit init = new GameObjectActionEffectInit();
								init.SetEffectMirror(m_dir);
								gae.AddAction(init);
								GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
								gae.AddAction(ndEffect);
							}
						}
					}
				}
				
				//SkillEffects._instance.LoadEffect("effect/prefab/", m_info.m_hiteffect.ToString(),transform.position,1.5f);
			}
			
			m_fun(m_skillinfo,1,hit);
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
	}
}
