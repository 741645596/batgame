using UnityEngine;
using System.Collections;

/// <summary>
/// 皮肤表现属性
/// </summary>
/// <author>zhulin</author>

public class BuildingSkin : Skin {
	private bool m_bPlayEffectColor = false;
	private float m_fPlayEffectDuration ;
	private float m_bPlayEffectCounter = 0;



	/// <summary>
	/// 加载模型
	/// </summary>
	public override void SetGameRole(Life life,MoveState State,LifeMCamp Camp,bool IsPlayer)
	{
		if(life == null)  return ;
		m_SkinOwner = life ;
		m_State = State;
		m_IsPlayer = IsPlayer;
		m_Camp = Camp;
	}
	
	public override HPAciton MyHPAction()
	{
		if (m_SkinOwner!=null&&m_SkinOwner.isDead) 
		{
			if(m_hpa!=null)
				m_hpa.DestroyHP();
			m_hpa = null;
		} 
		else 
		{
			if(m_hpa==null)
			{
				GameObject posgo = m_SkinOwner.GetLifeProp().HelpPoint.GetVauleByKey(BuildHelpPointName.help_hp);
				GameObject objHP =  GameObjectLoader.LoadPath ("Prefabs/Buildings/","BuildHPSlider",posgo.transform);
				m_hpa = objHP.GetComponent<HPAciton>();
			}
		}
		return m_hpa;
	}
	public override void ShowHP(int RoomID,int hp,int nFullHP,int bear)
	{
		if (MyHPAction() != null)
		{
			if(MyHPAction() is BuildHPAciton)
			{
				MyHPAction().gameObject.SetActive(true);
				BuildHPAciton buildHpAction = m_hpa as BuildHPAciton;
				buildHpAction.SetRoomID(RoomID,hp,nFullHP,bear);
			}
		}
	}
	public override void SetHpAction()
	{
		if (m_hpa == null) 
		{
			m_hpa = U3DUtil.GetComponentInChildren<HPAciton> (m_SkinOwner.GetComponent<Transform>().gameObject, false);
		}
		if (m_hpa != null)  
		{
			m_hpa.SetPlayer(m_IsPlayer);
		}
	}
	
	/// <summary>
	/// 更新skin 效果
	/// </summary>
	public override void UpdataSkinEffect()
	{
		if ( UpdateEffectColor())
		{
			return;
		}
		
	}
	/// <summary>
	/// 执行特效颜色
	/// </summary>
	/// <returns></returns>
	private bool UpdateEffectColor()
	{
		if (m_bPlayEffectColor)
		{
			m_bPlayEffectCounter += Time.deltaTime;
			if (m_bPlayEffectCounter < m_fPlayEffectDuration)
			{
				return true;
			}
			else
			{
				ResetBodySkin();
				m_bPlayEffectColor = false;
				return false;
			}
		}
		return false;
	}

	
	/// <summary>
	/// 播放受击颜色表现
	/// </summary>
	public override void PlayEffectColor(SkinEffectColor effectColor,float duration)
	{
		if (m_bPlayEffectColor )
		{
			return;
		}
		
		m_bPlayEffectColor = true;
		m_bPlayEffectCounter = 0f;
		m_fPlayEffectDuration = duration;
		
		switch (effectColor)
		{
		case SkinEffectColor.BeHit:
			SetBeHitBlinkColor();
			break;

		}
		
	}
	private void SetBeHitBlinkColor()
	{
		SkinnedMeshRenderer[] rens = m_SkinOwner.m_thisT.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer r in rens)
		{
			int nMatCount = r.materials.Length;
			for (int nMatCnt=0; nMatCnt<nMatCount; nMatCnt++) 
			{
				Material mat = r.materials[nMatCnt];
				if(null!=mat)
				{
					mat.SetColor("_Color", RoleSkinColor.BeHitMain);
					mat.SetColor("_Emission", RoleSkinColor.BeHitEmission);
				}
			}
		}
	}
	
	public override void ResetSkin()
	{  
		SetHpAction();
		
	}
	public override void ResetBodySkin()
	{
		
		SkinnedMeshRenderer[] rens = m_SkinOwner.m_thisT.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		foreach(SkinnedMeshRenderer r in rens)
		{
			int nMatCount = r.materials.Length;
			for (int nMatCnt=0; nMatCnt<nMatCount; nMatCnt++) 
			{
				Material mat = r.materials[nMatCnt];
				if(null!=mat)
				{
					mat.SetColor("_Color", RoleSkinColor.Main);
					mat.SetColor("_Emission", RoleSkinColor.Emission);
				}
			}
		}
		SetHpAction();
		
	}
}
