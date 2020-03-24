using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodSkill  {
	int m_ReleaseTimes;
	MapGrid m_targetg;
	Vector3 m_targetpos;
    List<Life> m_listSkillTarget = new List<Life>();
    public LifeMCamp m_Camp;
    
	public LifeMCamp Camp
	{
		get{return m_Camp;}
		set{m_Camp = value;}
	}
    public List<Life> ListSkillTarget
    {
        get { return m_listSkillTarget; }
        set { m_listSkillTarget = value; }
    }

	public GodSkillInfo m_godskill;
	public void SetSkill(GodSkillInfo skill)
	{
		m_ReleaseTimes = 0;
		m_godskill = skill;
	}

	public int GetRequireMana()
	{
		if (m_ReleaseTimes < m_godskill.m_mana.Count)
			return m_godskill.m_mana[m_ReleaseTimes];
		else
			return m_godskill.m_mana[m_godskill.m_mana.Count - 1];
	}

	public bool CheckTimes()
	{
		return m_ReleaseTimes < m_godskill.m_times;
	}
	public void ReleaseGodSkill(MapGrid g,Vector3 pos)
	{
		m_ReleaseTimes ++;
		m_targetg = g;
		m_targetpos = pos;

        if (m_godskill.m_type == 9000)
        {
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1000041_01", EffectCamera.GetEffectPos(pos), BattleEnvironmentM.GetLifeMBornNode(true));
			EffectCamera.AddFollowList(gae.transform,pos);
            //gae.gameObject.transform.localPosition = new Vector3(gae.gameObject.transform.localPosition.x,gae.gameObject.transform.localPosition.y,gae.gameObject.transform.localPosition.z-1f);
            GameObjectActionGodSkill9000 ndEffect = new GameObjectActionGodSkill9000();
			ndEffect.SetData(DoGodSkill,m_godskill.m_blackscreentime);
            //ndEffect.m_complete = DoGodSkill;
			SoundPlay.Play("apocalypse_missile_01",false,false);
			SoundPlay.JoinPlayQueue("apocalypse_missile_02",0f);
			SoundPlay.JoinPlayQueue("apocalypse_missile_03",0.0f);
            gae.AddAction(ndEffect);
        }
        else if (m_godskill.m_type == 9001)
        {
            //NGUIUtil.DebugLog("使命召唤表现");
            

			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1000051_01", EffectCamera.GetEffectPos(pos), BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionGodSkill9001 ndEffect = new GameObjectActionGodSkill9001();
			ndEffect.SetData(DoGodSkill,m_godskill.m_blackscreentime);
            if (m_listSkillTarget.Count > 0)
            {
                Life lifeTarget = m_listSkillTarget[0];
                ndEffect.SkillTarget = lifeTarget;
            }
			SoundPlay.Play("call_of_duty",false,false);
            gae.AddAction(ndEffect);
		
             
		}
		else if (m_godskill.m_type == 9002)
		{
			DoGodSkill();
		}
	}
	public void DoGodSkill()
	{
		//Debug.Log("2 & 2  "+ (2 & 2) + ",  7 & 2  " + (7 & 2));

		List<Life>  ltarget = GetRangeAttackList(m_godskill.m_target,m_godskill.m_multiple,m_targetpos,m_godskill.m_range,m_Camp);
      /*  if (ltarget.Count == 0)//目标不是来自技能
        {

        }*/

		if (m_godskill.m_action == 1)
		{
	        foreach(Life l in ltarget)
			{
				if (l.m_Core.m_Camp == m_Camp)
				{
					StatusBuff(l);
				}
				else
				{
					SkillReleaseInfo Info = new SkillReleaseInfo(); 
					Info.m_InterruptSkill = false;
					Info.m_MakeStatus = new List<StatusType> ();
					Info.m_bImmunity = false;
					if (l is Building)
					{
						Info.m_Damage = -Mathf.CeilToInt(m_godskill.m_power2 * l.fullHP * 1.0f/ l.m_Attr.Bear);
					}
					else
						Info.m_Damage = -m_godskill.m_power;
					Info.m_struckeffect = "";
					Info.Result = AttackResult.Normal;
					if(StatusDeBuff(l))
						Info.m_InterruptSkill = true;
					//else Damage = Defense.HP;
					//判断受击方受到的伤害值，是否大于可被打断伤害值。
					int LimitHp = l.m_Attr.Hp * ConfigM.GetSkillData0() /100;
					if(m_godskill.m_power > LimitHp) 
						Info.m_InterruptSkill = true;
					l.ApplyDamage(Info,null);
				}
			}
		}
		else if (m_godskill.m_action == 2)
		{
			ltarget = m_listSkillTarget;
			if (m_listSkillTarget.Count>0)
			{
				Life life = m_listSkillTarget[0];
				Role role = life as Role;
				if (role != null)
				{
					role.ReBorn();
				}
			}
		}
		else if (m_godskill.m_action == 3)
		{
			CreatePet(m_godskill.m_power,m_targetg);
		}
        //NGUIUtil.DebugLog("name = " + ltarget[0].transform.name + " hp =" + ltarget[0].HP);
	}
	/// <summary>
	/// 创建宠物
	/// </summary>
	private SummonPet CreatePet(int PetDataID,MapGrid StartGrid)
	{
		SummonpetInfo info = SummonM.GetSummonPetInfo(PetDataID); 
		SummonPet pet = new SummonPet();
		pet.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty,true);
		//IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), info.m_modeltype, info.m_modeltype.ToString(), AnimatorState.Empty);
		GameObject go = pet.RoleSkinCom.tRoot.gameObject;
		Vector3 pos = Vector3.zero;
		string posname = "";
		LifeObj lo = go.AddComponent<LifeObj>();
		pet.SetSummonPetLife(info,pet.RoleSkinCom.ProPerty,LifeEnvironment.Combat);
		pet.SetLifeCore(new LifeMCore(PetDataID ,true,LifeMType.SUMMONPET,Camp,MoveState.Walk));
		pet.SetSkin();
		pet.SetBornPos(StartGrid.GridPos,0);
		lo.SetLife(pet , pet.RoleSkinCom.ProPerty);
		go.transform.parent = BattleEnvironmentM.GetLifeMBornNode(true);
		Vector3 start = StartGrid.WorldPos;
		start.z = Camera.main.transform.position.z;
		go.transform.position = start;
		return pet;
	}
	/// <summary>
	/// 状态产生判断
	/// </summary>
	private bool CheckCondition(NDAttribute attr,SkillStatusInfo StatusInfo)
	{
		if(attr == null || StatusInfo == null)
			return false;
		//为0 直接产生状态。
		EffectType s = (EffectType)StatusInfo.m_condition;
		if(s == EffectType.None ) return true;
		int data0 = StatusInfo.m_data0;
		int data1 = StatusInfo.m_data1;
		int value = attr.GetAttrData(s);
		if (value > data0 && value <= data1)
			return true;
		return false;
	}
	/// <summary>
	/// 产生DeBuff
	/// </summary>
	protected bool StatusDeBuff(Life Defense)
	{
		if(Defense == null || Defense.m_Attr == null || Defense.m_Status == null)
			return  false;
		if(m_godskill == null || m_godskill.m_enemy_status_info == null)
			return false;
		
		bool InterruptSkill = false;
		List<SkillStatusInfo>l = m_godskill.m_enemy_status_info;
		
		for(int i = 0; i <l.Count ; i++)
		{
			if (CheckCondition(Defense.m_Attr, l[i]))
			{
				if(Defense.m_Status.AddStatus(-1,m_godskill.m_type,l[i]) == true)
				{
					InterruptSkill = true;
				}
			}
		}
		return InterruptSkill;
	}
	/// <summary>
	/// 产生Buff
	/// </summary>
	protected bool StatusBuff(Life Defense)
	{
		if (Defense == null || Defense.m_Attr == null || Defense.m_Status == null)
			return false;
		if (m_godskill == null || m_godskill.m_own_status_info == null)
			return false;
		
		bool InterruptSkill = false;
		List<SkillStatusInfo> l = m_godskill.m_own_status_info;
		
		for (int i = 0; i < l.Count; i++)
		{
			if (CheckCondition(Defense.m_Attr, l[i]))
			{
				if (Defense.m_Status.AddStatus(-1, m_godskill.m_type, l[i]) == true)
				{
					InterruptSkill = true;
				}
			}
		}
		return InterruptSkill;
	}
	public  bool CheckRangeAttackTarget(Life target, Vector3 pos,float radus)
	{
		Building buildlife = target as Building;
		if (buildlife != null) 
		{
			BuildInfo info = CmCarbon.GetBuildInfo(buildlife.m_Core.m_DataID);
			for (int i = 0; i < info.m_Shape.height; i++)
			{
				for(int j = 0; j < info.m_Shape.width * MapGrid.m_UnitRoomGridNum; j++)
				{
					MapGrid bg = MapGrid.GetMG(i + info.m_cy, j + info.m_cx);
					Vector3 tbpos = bg.WorldPos;
					float bdis = Vector2.Distance(new Vector2(tbpos.x,tbpos.y), new Vector2(pos.x,pos.y));
					if (bdis <= radus)
						return true;
					bdis = Vector2.Distance(new Vector2(tbpos.x,tbpos.y + 3f), new Vector2(pos.x,pos.y));
					if (bdis <= radus)
						return true;
				}
			}
			return false;
		}
		MapGrid g = target.GetMapGrid();
		Vector3 tpos = g.WorldPos;
		float dis = Vector2.Distance(new Vector2(tpos.x,tpos.y), new Vector2(pos.x,pos.y));
		if (dis <= radus)
			return true;
		return false;
	}
	public  List<Life> GetRangeAttackList(int targetype,int count, Vector3 g,float radus,LifeMCamp camp)
	{
		
		List<Life> l = new List<Life>();
		List<Life> lr = new List<Life>();
		if ((targetype & 4)  == 4)
		{
			CM.SearchLifeMListInBoat(ref lr,LifeMType.SOLDIER, camp);
			foreach(Role r in lr)
			{
				if ( CheckRangeAttackTarget(r,g,radus))
					l.Add(r);
			}
			
			List<Life> lb = new List<Life>();
			CM.SearchLifeMListInBoat(ref lb,LifeMType.BUILD, camp);
			foreach(Building b in lb)
			{
				
				if (b.m_Attr.IsDamage && CheckRangeAttackTarget(b, g, radus))
					l.Add(b);
			}		
			/*List<Life> lw = new List<Life>();
			CM.SearchLifeMListInBoat(ref lw,LifeMType.WALL, camp);
			foreach(Wall w in lw)
			{
				if (CheckRangeAttackTarget(w, g, radus) )
					l.Add(w);
			}*/
		}
		if ((targetype & 2)  == 2)
		{
			
			CM.SearchLifeMListInBoat(ref lr,LifeMType.SOLDIER, camp == LifeMCamp.ATTACK? LifeMCamp.DEFENSE:LifeMCamp.ATTACK);
			foreach(Role r in lr)
			{
				if ( CheckRangeAttackTarget(r,g,radus))
					l.Add(r);
			}
		}
		if ((targetype & 1)  == 1)
		{
			
			List<Life> lb = new List<Life>();
			CM.SearchLifeMListInBoat(ref lb,LifeMType.BUILD, camp == LifeMCamp.ATTACK? LifeMCamp.DEFENSE:LifeMCamp.ATTACK);
			foreach(Building b in lb)
			{
				
				if (b.m_Attr.IsDamage && CheckRangeAttackTarget(b, g, radus))
					l.Add(b);
			}		
			List<Life> lw = new List<Life>();
			/*CM.SearchLifeMListInBoat(ref lw,LifeMType.WALL,  camp == LifeMCamp.ATTACK? LifeMCamp.DEFENSE:LifeMCamp.ATTACK);
			foreach(Wall w in lw)
			{
				if (CheckRangeAttackTarget(w, g, radus) )
					l.Add(w);
			}*/
		}


		
		LimitAttcklist(ref l,count);
		return l;
	}
	public void LimitAttcklist(ref List<Life> rolelist,int count)
	{
		if (count <= 0)
			return;
		else
		{
			while(rolelist.Count > count)
			{
				rolelist.RemoveAt(rolelist.Count - 1);
			}
		}
	}
}
