using UnityEngine;
using System.Collections;
/// <summary>
/// 宠物皮肤表现属性
/// </summary>
/// <author>zhulin</author>
public class PetSkin : RoleSkin {


	
	/// <summary>
	/// 更新skin 效果
	/// </summary>
	public override void UpdataSkinEffect()
	{
		if(CheckAttrChange() == false) return ;
		
		if(ProPerty == null) return;
		//设置颜色
	}


	public override void ResetSkin()
	{  
		SetIRole(m_SkinOwner.m_Attr.AttrType);
		if(null!=ProPerty)
			SetCampModel(m_IsPlayer );

	}
	
}



/// <summary>
/// 宠物皮肤颜色定义
/// </summary>
public class PetSkinColor 
{
	//Camp Color
	public static Color AttackMain  = new Color(1.0f,0f,0f);                   //攻击方阵营颜色
	public static Color AttackEmission  = new Color(1.0f,0.196f,0.196f);               //攻击方阵营颜色
	public static Color DefenseMain  = new Color(0,0,1f);                       //防守方阵营颜色
	public static Color DefenseEmission  = new Color(0.196f,0.196f,1f);             //防守方阵营颜色
	//body Color
	public static Color Main = new Color(1.0f,1.0f,1.0f);                          //默认Main颜色
	public static Color Emission = new Color(0.196f,0.196f,0.196f);                //默认Emission颜色
	public static Color IcePointMain = new Color(1.0f,1.0f,1);                      //默认Main颜色
	public static Color IcePointEmission = new Color(0.2f,0.41f,0.82f);        //默认Emission颜色
};