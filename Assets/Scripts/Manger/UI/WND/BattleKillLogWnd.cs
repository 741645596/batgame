using UnityEngine;
using System.Collections;

using sdata;

public class BattleKillLogWnd : WndBase
{
	Animation mAnim;

	public BattleKillLogWnd_h MyHead
	{
		get
		{
			return (base.BaseHead() as BattleKillLogWnd_h);
		}
	}

	public override void WndStart()
	{
		base.WndStart();
		Init();
	}

	void Init()
	{
		mAnim = MyHead.gameObject.GetComponent<Animation>();
	}

	public void Show(int idAttacker, int idVictim)
	{
		SetAttacker(idAttacker);
		SetVictim(idVictim);
		mAnim.Play();

	}

	void SetAttacker(int sceneID)
	{
		Role r = CM.GetAllLifeM(sceneID, LifeMType.SOLDIER) as Role;
		SoldierInfo info = CmCarbon.GetSoldierInfo(r.m_Core);
		NGUIUtil.Set2DSprite(MyHead.Spr2DKillHero, "Textures/role/", info.m_modeltype);
		NGUIUtil.SetLableText<string>(MyHead.LblKillHero, info.m_name);
		NGUIUtil.SetStarLevelNum(MyHead.SprKillHeroStar, info.StarLevel);
	}

	void SetVictim(int sceneID)
	{
		Role r = CM.GetAllLifeM(sceneID, LifeMType.SOLDIER) as Role;
		SoldierInfo info = CmCarbon.GetSoldierInfo(r.m_Core);
		NGUIUtil.Set2DSprite(MyHead.Spr2DKilledHero, "Textures/role/", info.m_modeltype);
		NGUIUtil.SetLableText<string>(MyHead.LblKilledHero, info.m_name);
		NGUIUtil.SetStarLevelNum(MyHead.SprKilledHeroStar, info.StarLevel);
	}	
}
