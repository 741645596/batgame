using UnityEngine;
using System.Collections;
/// <summary>
/// 道具属性
/// </summary>
/// <author>zhulin</author>
public class SummonProsAttribute : NDAttribute {

	private SummonProsInfo m_info;
	public override void Init(int SceneID, LifeMCore Core, Life Parent)
	{
		base.Init(SceneID,Core,Parent);
		m_info = CmCarbon.GetSummonProsInfo(Core);
		m_AttrType = m_info.m_modeltype;
		m_FullHp = m_info.m_hp;
		m_Hp = FullHp;


	}
}
