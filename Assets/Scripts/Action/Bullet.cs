using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet : MonoBehaviour {
	public enum BulletType{
		Bullet     ,
		Snowball   = 1002,
		Grenade    = 1005,
		Missile    ,
		Poison     = 1019,
		ling       = 1015,
        bomb1028 = 1028,
	}
	public static List<Bullet> s_Bullet = new List<Bullet>();


	public WalkDir m_dir;
	public Vector3 m_Start;
	public Vector3 m_Destination;
	//public delegate void CallBackFun(SoldierSkill skill,int times);
	public DoAttackFun m_fun;
	public float m_Speed;
	public Vector3 m_fadeoutpos;
	public Vector3 m_fadeinpos;
	public BulletType m_type;
	public SoldierSkill m_skillinfo;
	public SkillEffectInfo m_info;
	float m_outtime;
	float m_intime;
	float m_TimeCount;
	float m_Duration;
	bool m_Skill;
	GameObject effect;
	Bezier myBezier;
	Bezier myBezier1;
	Bezier myBezier2;
	bool m_bpause = false;
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
	public void SetInfo(Vector3 dest,DoAttackFun fun,float speed,WalkDir dir,BulletType t,SoldierSkill skill, bool isPlaySound=true)
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
		m_type = t;
		//effect = SkillEffects._instance.LoadEffect("effect/prefab/", "1002011",transform.position,0.5f);



        if (isPlaySound)
        {
            SoundPlay.Play("fireGun", false, false);
        }
		m_info = m_skillinfo.m_skilleffectinfo;
		/*if (t == BulletType.Grenade || t == BulletType.Snowball|| t == BulletType.Poison || t == BulletType.ling)
		{
			//m_Destination.y -= 1f;
			myBezier = new Bezier( m_Start,  new Vector3(0,-3,0),  new Vector3(0,-3,0), m_Destination );
		}
		
		if (t == BulletType.Grenade || t == BulletType.Snowball|| t == BulletType.Poison )
		{
			//m_Destination.y -= 1f;
			myBezier = new Bezier( m_Start,  new Vector3(0,-3,0),  new Vector3(0,-3,0), m_Destination );
		}
		if (t == BulletType.ling)
		{
			//m_Destination.y -= 1f;
			m_Destination.y = m_Start.y;
			myBezier = new Bezier( m_Start,  new Vector3(0,-2,0),  new Vector3(0,-2,0), m_Destination );
		}*/
		
		if(skill.m_type == 1005)
		{
			float dis = m_Destination.x - m_Start.x;
			Vector3 pos = m_Destination;
			pos.x = m_Start.x + dis *0.7f; 
			myBezier = new Bezier( m_Start,  new Vector3(0,-m_info.m_locus,0),  new Vector3(0,-m_info.m_locus,0), pos );
			Vector3 pos1 = pos;
			pos1.x += dis * 0.2f;
			myBezier1 = new Bezier( pos,  new Vector3(0,-m_info.m_locus * 0.4f,0),  new Vector3(0,-m_info.m_locus*0.4f,0), pos1 );
			myBezier2 = new Bezier( pos1,  new Vector3(0,-m_info.m_locus *0.2f,0),  new Vector3(0,-m_info.m_locus*0.2f,0), m_Destination );
		}
		else
			myBezier = new Bezier( m_Start,  new Vector3(0,-m_info.m_locus,0),  new Vector3(0,-m_info.m_locus,0), m_Destination );

		//Debug.Log("bullet:" + transform.localPosition + ","  +m_Destination + "," + m_Speed + "," + m_Duration + ","+Mathf.Abs(transform.localPosition.x - m_Destination.x));
	}
	// Update is called once per frame
	void Update () {
		if (m_bpause)
			return;
		m_TimeCount += Time.deltaTime;
		if (m_type == BulletType.Missile)
		{ 
			Missile m = GetComponent<Missile>();
			Vector3 pos = transform.localPosition;
			if (m_TimeCount < m_outtime)
			{
				pos = Vector3.Lerp(m_Start,m_fadeoutpos,m_TimeCount/ m_outtime);
				transform.localPosition = pos;
				float Angles = NdUtil.V2toAngle(m_Start,m_fadeoutpos,Vector3.right);
				m.SetAngle(-Angles);
			}
			else if (m_TimeCount>= m_Duration)
			{
				//effect = SkillEffects._instance.LoadEffect("effect/prefab/", "1002061",transform.position,1.5f);
                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1002061", transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
				gae.AddAction(ndEffect);
                effect = gae.gameObject;
				SoundPlay.Stop("missile_launch",false);
                SoundPlay.Play("missile_explosion", false, false);
				m_fun(m_skillinfo,1);
				GameObject.Destroy(gameObject);
			}
			else
			{
				pos = myBezier.GetPointAtTime((m_TimeCount-m_outtime)/(m_Duration-m_outtime));
				float Angles = NdUtil.V2toAngle(pos,myBezier.GetPointAtTime((m_TimeCount-m_outtime)/(m_Duration-m_outtime)+0.01f),Vector3.right);
				m.SetAngle(-Angles);
				transform.localPosition = pos;
			}
		}
		else
		{
			if (m_TimeCount < m_Duration)
			{
			Vector3 pos = transform.localPosition;
			//pos.x = Mathf.Lerp(m_Start.x,m_Destination.x,m_TimeCount/ m_Duration);
			pos = myBezier.GetPointAtTime(m_TimeCount/m_Duration);
			transform.localPosition = pos;
			}
			else if (m_TimeCount>= m_Duration)
			{
				if (m_skillinfo.m_type == 1005)
				{
					if (m_TimeCount <= m_Duration + 0.5f)
					{
						Vector3 pos = transform.localPosition;
						pos = myBezier1.GetPointAtTime((m_TimeCount - m_Duration)/0.5f);
						transform.localPosition = pos;
					}
					else if (m_TimeCount < m_Duration +1f)
					{
						
						Vector3 pos = transform.localPosition;
						pos = myBezier2.GetPointAtTime((m_TimeCount - m_Duration - 0.5f)/0.5f);
						transform.localPosition = pos;
					}
				}
				if (m_TimeCount >= m_Duration + m_skillinfo.m_intone_speed * 0.001f)
				{

					//Debug.Log(m_type   +"," + transform.position);
					transform.localPosition = m_Destination;
					if (m_skillinfo.m_type != 1028)
						m_fun(m_skillinfo,1);

					//SkillEffects._instance.LoadEffect("effect/prefab/", m_info.m_hiteffect.ToString(),transform.position,1.5f);
	                if (m_info.m_hiteffect!=0)
	                {
						Vector3 pos = transform.position;
						pos.z = -1.5f;
						GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, m_info.m_hiteffect.ToString(), pos, BattleEnvironmentM.GetLifeMBornNode(true));
						GameObjectActionEffectInit init = new GameObjectActionEffectInit();
						init.SetEffectMirror(m_dir);
						gae.AddAction(init);
						GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
						gae.AddAction(ndEffect);
	                }
	                
					if (m_info.m_hitaudio != "")
	                    SoundPlay.Play(m_info.m_hitaudio, false, false);
					GameObject.Destroy(gameObject);
				}
			}
		}
		/*else if (m_type == BulletType.Bullet )
		{
			Vector3 pos = transform.localPosition;
			pos.x = Mathf.Lerp(m_Start.x,m_Destination.x,m_TimeCount/ m_Duration);
			transform.localPosition = pos;
			if (m_TimeCount>= m_Duration)
			{
				m_fun(m_skillinfo,1);
				GameObject.Destroy(gameObject);
			}
		}
		else if (m_type == BulletType.Grenade)
		{
			Vector3 pos = transform.localPosition;
			//pos.x = Mathf.Lerp(m_Start.x,m_Destination.x,m_TimeCount/ m_Duration);
			pos = myBezier.GetPointAtTime(m_TimeCount/m_Duration);
			transform.localPosition = pos;
			if (m_TimeCount>= m_Duration)
			{
				//Debug.Log(m_type   +"," + transform.position);
				transform.localPosition = m_Destination;
				m_fun(m_skillinfo,1);
				SkillEffects._instance.LoadEffect("effect/prefab/", "1002081",transform.position,1.5f);
                SoundPlay.Play("grenade_explosion",false ,false);
                GameObject.Destroy(gameObject);
			}
		}
		else if (m_type == BulletType.Snowball)
		{
			Vector3 pos = transform.localPosition;
			pos = myBezier.GetPointAtTime(m_TimeCount/m_Duration);
			transform.localPosition = pos;
			if (m_TimeCount>= m_Duration)
			{
				//Debug.Log(m_type +"," +transform.position);
				transform.localPosition = m_Destination;
				m_fun(m_skillinfo,1);
				SkillEffects._instance.LoadEffect("effect/prefab/", "1002101",transform.position,1.5f);
                GameObject.Destroy(gameObject);
			}
		}
		else if (m_type == BulletType.Poison)
		{
			Vector3 pos = transform.localPosition;
			pos = myBezier.GetPointAtTime(m_TimeCount/m_Duration);
			transform.localPosition = pos;
			//Debug.Log(m_Start + ",  " + m_Destination + ",  " + pos  + ", " + m_TimeCount + "," + m_Duration);
			if (m_TimeCount>= m_Duration)
			{
				//Debug.Log(m_type +"," +transform.position);
				transform.localPosition = m_Destination;
				m_fun(m_skillinfo,1);
				SkillEffects._instance.LoadEffect("effect/prefab/", "1052031",transform.position,1f);
				GameObject.Destroy(gameObject);
				SoundPlay.Play("skill_money_dy",false ,false);
			}
		}
		else if (m_type == BulletType.ling)
		{
			Vector3 pos = transform.localPosition;
			pos = myBezier.GetPointAtTime(m_TimeCount/m_Duration);
			transform.localPosition = pos;
			//Debug.Log(m_Start + ",  " + m_Destination + ",  " + pos  + ", " + m_TimeCount + "," + m_Duration);
			if (m_TimeCount>= m_Duration)
			{
				//Debug.Log(m_type +"," +transform.position);
				transform.localPosition = m_Destination;
				m_fun(m_skillinfo,1);
				//SkillEffects._instance.LoadEffect("effect/prefab/", "1052031",transform.position,1f);
				GameObject.Destroy(gameObject);
			}
		}
        else if (m_type == BulletType.bomb1028)
        {
            Vector3 pos = transform.localPosition;
            pos.x = Mathf.Lerp(m_Start.x, m_Destination.x, m_TimeCount / m_Duration);
            transform.localPosition = pos;
            if (m_TimeCount >= m_Duration)
            {
                //m_fun(m_skillinfo, 1);
                SkillEffects._instance.LoadEffect("effect/prefab/", "1003031", transform.position, 1.3f);
                GameObject.Destroy(gameObject);
            }
        }*/
	}
	public void SetSkillInfo(Vector3 dest,DoAttackFun fun,float speed,WalkDir dir,Vector3 outpos, Vector3 inpos,BulletType t,SoldierSkill skill)
	{
		SetInfo(dest,fun,speed,dir,t,skill,false);
		SoundPlay.Play("missile_launch", false, false);
		m_fadeinpos = inpos;
		m_fadeoutpos = outpos;
		m_fadeoutpos.z = 1.5f;
		m_Skill = true;
		m_outtime = 0.3f;//Vector3.Distance(transform.localPosition,outpos) /speed;
		m_intime = Vector3.Distance(inpos,dest) / speed;
		m_Duration = 1.3f;//m_outtime + m_intime;
		Vector3 pos = transform.localPosition;
		//myBezier = new Bezier( m_fadeoutpos,  new Vector3(0,-10,0),  new Vector3(0,-10,0), m_Destination );
		myBezier = new Bezier( m_fadeoutpos,  new Vector3(0,-m_info.m_locus,0),  new Vector3(0,-m_info.m_locus,0), m_Destination );
        //UnityEditor.Handles.DrawBezier(m_fadeoutpos, m_Destination, new Vector3(0, -10, 0), new Vector3(0, -10, 0),
        //    Color.red, null, 1f);
		//Debug.Log(m_Start +","+m_fadeoutpos +","+m_fadeinpos +","+m_Destination);
		//Debug.Log( m_fadeoutpos+","+  new Vector3(m_fadeoutpos.x,m_fadeoutpos.y -10,m_fadeoutpos.z)+","+   new Vector3(m_Destination.x,m_Destination.y -10,m_Destination.z)+","+  m_Destination);
		//Debug.Log("bullet:" + transform.localPosition + ","  +m_Destination + "," + m_Speed + "," + m_Duration + ","+Mathf.Abs(transform.localPosition.x - m_Destination.x));
	}
	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		if (myBezier != null)
		{
			string str = "start:";
			for(float i = 0; i <= 1; i+=0.01f)
			{
				str += myBezier.GetPointAtTime(i) + ",  ";
				Vector3 s  = myBezier.GetPointAtTime(i);
				Vector2 e =myBezier.GetPointAtTime(i+0.01f);
				Gizmos.DrawLine(BattleEnvironmentM.Local2WorldPos(s),
				                BattleEnvironmentM.Local2WorldPos(e));
			}
			//Debug.Log(str);
		}
	}

}

[System.Serializable]

public class Bezier : System.Object
	
{
	
	public Vector3 p0;
	
	public Vector3 p1;
	
	public Vector3 p2;
	
	public Vector3 p3;
	
	public float ti = 0f;
	
	private Vector3 b0 = Vector3.zero;
	
	private Vector3 b1 = Vector3.zero;
	
	private Vector3 b2 = Vector3.zero;
	
	private Vector3 b3 = Vector3.zero;
	
	private float Ax;
	
	private float Ay;
	
	private float Az;
	
	private float Bx;
	
	private float By;
	
	private float Bz;
	
	private float Cx;
	
	private float Cy;
	
	private float Cz;
	
	// Init function v0 = 1st point, v1 = handle of the 1st point , v2 = handle of the 2nd point, v3 = 2nd point
	
	// handle1 = v0 + v1
	
	// handle2 = v3 + v2
	/// <summary>
	/// 贝塞尔曲线
	/// </summary>
	/// <param name="v0">start</param>
	/// <param name="v1">startTangent</param>
	/// <param name="v2">endTangent</param>
	/// <param name="v3">end</param>
	public Bezier( Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3 )
		
	{
		
		this.p0 = v0;
		
		this.p1 = v1;
		
		this.p2 = v2;
		
		this.p3 = v3;
		
	}
	
	// 0.0 >= t <= 1.0
	
	public Vector3 GetPointAtTime( float t )
	{

		this.CheckConstant();
		
		float t2 = t * t;
		
		float t3 = t * t * t;
		
		float x = this.Ax * t3 + this.Bx * t2 + this.Cx * t + p0.x;
		
		float y = this.Ay * t3 + this.By * t2 + this.Cy * t + p0.y;
		
		float z = this.Az * t3 + this.Bz * t2 + this.Cz * t + p0.z;
		
		return new Vector3( x, y, z );
		
	}
	
	private void SetConstant()
		
	{
		
		this.Cx = 3f * ( ( this.p0.x + this.p1.x ) - this.p0.x );
		
		this.Bx = 3f * ( ( this.p3.x + this.p2.x ) - ( this.p0.x + this.p1.x ) ) - this.Cx;
		
		this.Ax = this.p3.x - this.p0.x - this.Cx - this.Bx;
		
		this.Cy = 3f * ( ( this.p0.y + this.p1.y ) - this.p0.y );
		
		this.By = 3f * ( ( this.p3.y + this.p2.y ) - ( this.p0.y + this.p1.y ) ) - this.Cy;
		
		this.Ay = this.p3.y - this.p0.y - this.Cy - this.By;
		
		this.Cz = 3f * ( ( this.p0.z + this.p1.z ) - this.p0.z );
		
		this.Bz = 3f * ( ( this.p3.z + this.p2.z ) - ( this.p0.z + this.p1.z ) ) - this.Cz;
		
		this.Az = this.p3.z - this.p0.z - this.Cz - this.Bz;
		
	}
	
	// Check if p0, p1, p2 or p3 have changed
	
	private void CheckConstant()
		
	{
		
		if( this.p0 != this.b0 || this.p1 != this.b1 || this.p2 != this.b2 || this.p3 != this.b3 )
			
		{
			
			this.SetConstant();
			
			this.b0 = this.p0;
			
			this.b1 = this.p1;
			
			this.b2 = this.p2;
			
			this.b3 = this.p3;
			
		}
		
	}
	
}
