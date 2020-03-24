using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// 传送门
/// </summary>
/// <author>zhulin</author>
public class Building1605 : Building {
	public float m_fAttackTimer;//上一次技能触发时间
	public float m_fHitTimer;//上一次受击时间

	protected GameObject go1903011;//常态
	protected GameObject go1903031;//传送
	protected GameObject go1903041;//传送出口
	private MapGrid m_mgSendto;

    // 编辑模式使用
    private Int2 GateGrid;
	
	private bool m_bCDOver=true;
	
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		if(go1903011==null){
			go1903011 = GameObjectLoader.LoadPath("effect/prefab/", "1903011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
			go1903031 = GameObjectLoader.LoadPath("effect/prefab/", "1903031", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
			go1903041 = GameObjectLoader.LoadPath("effect/prefab/", "1903041", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
			go1903041.SetActive (false);
		}
		
	}
	// Use this for initialization
	public override void  InitBuild() {
		base.InitBuild();
	}
	/// <summary>
	/// 用于在NGUI处 显示特效
	/// </summary>
	public override void InitBuildUI()
	{
		base.InitBuildUI();
		go1903011 = GameObjectLoader.LoadPath("effect/prefab/", "1903011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.skinroot).transform);
		if (go1903011)
		{
			ParticalForNgui pNgui = go1903011.AddComponent<ParticalForNgui>();
			pNgui.Size = 50f;
			pNgui.DoScale();
		}
	}
	public override void BuildUpdate()
	{
		base.BuildUpdate ();
		if (isDead) 
		{
			if (TimeCheckAttackOver ()) 
			{
				go1903041.SetActive(false);
			}
			return;
		}
		if (TimeCheckSendOut()&&go1903041.activeSelf==false) 
		{
			go1903041.SetActive(true);
		}

		if (TimeCheckSendInOver())
		{
			go1903031.SetActive (false);
		}
		if (TimeCheckAttackOver ()) 
		{
			go1903041.SetActive(false);
		}
		if (!m_bCDOver && GetCDTime()<1f) 
		{
			m_bCDOver = true;
			go1903011.SetActive (true);
		}
	}
	public bool TimeCheckAttackOver()
	{
		if (Time.time - m_fAttackTimer <= 2.0f)
				return false;
		return true;
	}
	public bool TimeCheckSendInOver()
	{
		if (Time.time - m_fAttackTimer > 2.0f)
			return true;
		return false;
	}
	public bool TimeCheckSendOut()
	{
		if (Time.time - m_fAttackTimer > 0.2f)
			return true;
		return false;
	}
	public bool TimeCheckHitOver()
	{
		if (Time.time - m_fHitTimer > 0.5f)
			return true;
		return false;
	}
	public override void Hit(int damage)
	{
        return;
		if (TimeCheckAttackOver ())
					return;
		go1903011.SetActive (false);
		go1903031.SetActive (false);
		go1903041.SetActive (false);
		m_fHitTimer = Time.time;
	}


	/// <summary>
	/// 释放技能主逻辑
	/// </summary>
	protected override bool ReleaseSkill(ref List<Life> RoleList,ref int nAttackIndex)
	{
		if (null == m_mgSendto) 
		{
			m_mgSendto = MapGrid.GetMG (m_Attr.ShipPutdata1,m_Attr.ShipPutdata0-2);
			if(null==m_mgSendto)
			{
				m_mgSendto = MapGrid.GetMG (GetMapGrid().GridPos.Layer,GetMapGrid().GridPos.Unit+2);
			}
			Vector3 vEndEff = m_mgSendto.WorldPos;
			vEndEff = new Vector3 (vEndEff.x, vEndEff.y, vEndEff.z);
			go1903041.transform.position = vEndEff;
		}
		if (!TimeCheckAttackOver())
			return false;
		
		bool IsRelease = false;
		if (RoleList.Count > 0)
		{
			m_bCDOver = false;
			IsRelease=true;
			go1903011.SetActive (false);
			go1903031.SetActive (true);
			go1903041.SetActive (false);
			m_fAttackTimer = Time.time;
			m_fHitTimer = 0;

			Vector3 vStartTo = GetMapGrid().pos;
			vStartTo = new Vector3 (vStartTo.x, vStartTo.y + 1.5f, vStartTo.z);
			Vector3 vEnd = m_mgSendto.pos;
			Vector3 vEndFr = new Vector3 (vEnd.x, vEnd.y + 1.5f, vEnd.z);
			//传送门里有炮弹兵且传送门已经CD好
			foreach (Life l in RoleList)
			{
				if(l != null && l is Role)
				{
					Role w = l as Role;
					Vector3 vStart = m_thisT.localPosition;
					w.SendToGrid(vStart,vStartTo,vEndFr,vEnd,1.5f,m_mgSendto);
				}
				
				if(l != null && l is Pet)
				{
					Pet w = l as Pet;
					Vector3 vStart = m_thisT.localPosition;
					w.SendToGrid(vStart,vStartTo,vEndFr,vEnd,1.5f,m_mgSendto);
				}
			}
		}
		return IsRelease;
	}
	protected override void TriggerSkillTime(ref List<Life> RoleList,ref float fReleaseDelay)
	{
		fReleaseDelay = 0.25f;
		
	}
	public override void  Destroy()
	{
		go1903011.SetActive (false);
		go1903031.SetActive (false);
	}

	public override void SetEditModeSetting(BuildInfo Info, LifeEnvironment Environment)
	{
		//base.SetEditModeSetting(Info ,Environment);

		if(m_Property != null)
		{
			//(m_Property as Building1605_h).m_goEffectRoot = m_thisT.gameObject;
			if(go1903011==null)
			{
				InitBuildModel();
			}
			
			go1903041.AddComponent<NdHide>().ResetDuration(1000000f);
			go1903041.SetActive(false);
		}


		Int2 Pos = new Int2(Info.m_ShipPutdata0,Info.m_ShipPutdata1);
		Int2 grid = new Int2(Pos.Unit , Pos.Layer);
        GateGrid = grid;
        SetTransGate(grid);
	}
    /// <summary>
    /// 设置传送门的传送点
    /// </summary>
    /// <param name="grid"></param>
    public void SetTransGate(Int2 grid)
    {
        ShowTranGate(grid);
        TouchMove.g_bSetParaing = false;
    }
    /// <summary>
    /// 显示传送点
    /// </summary>
    /// <param name="grid"></param>
    public void ShowTranGate(Int2 grid)
    {
        if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.Edit)
        {
            Vector3 vEndEff = Vector3.zero;
            if (RoomMap.CheckHaveMap() == true)
            {
                vEndEff = RoomMap.GetRoomGridLocalPos(grid);
            }
            else
            {
                vEndEff = GenerateShip.GetbuildPos(grid);
            }
            vEndEff = U3DUtil.AddX(vEndEff, 1.0f);
            if (go1903041)
            {
                go1903041.SetActive(true);
                go1903041.transform.localPosition = vEndEff;
                if (go1903041.GetComponent<NdHide>() != null)
                {
                    go1903041.GetComponent<NdHide>().ResetDuration(3f);
                }
            }
        }
    }

	public override void Shake()
	{
		if(!isDead)
			m_thisT.DOShakePosition(0.5f);
	}

}
