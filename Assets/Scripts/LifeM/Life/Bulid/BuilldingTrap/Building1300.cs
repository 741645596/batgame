using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 金库(金币)
/// </summary>
/// <author>zhulin</author>
public class Building1300 : Building {
	
	private float m_fGoldFactor = 1.0f;
	private int m_CurrentGlod;
	protected GameObject m_goEffect1901021;//闪光特效
	
	private GameObject m_goBody01;
	private GameObject m_goBody02;
	private GameObject m_goBody03;
	private GameObject m_goBody04;
	private GameObject m_goBody05;
	public override void NDStart()
	{
		base.NDStart();
	}
	public override void InitBuildModel()
	{
		base.InitBuildModel ();
		//临时做法
		if(LifeEnvironment.Edit == Environment)
			m_CurrentGlod = UserDC.GetCoin();
		else
			m_CurrentGlod = CmCarbon.DefenderInfo.Coin;
		int typeGold = ConfigM.GetGoldType(m_CurrentGlod);
		if (m_Property == null)
		{
			Debug.Log("Building1300 error");
			return;
		}
		
		m_goBody01=m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.body01);
		m_goBody02=m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.body02);
		m_goBody03=m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.body03);
		m_goBody04=m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.body04);
		m_goBody05=m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.body05);
		(m_Property as BuildProperty).SetAllBodyActive (false);
		if (typeGold == 1)
			m_goBody01.SetActive(true);
		else if (typeGold == 2)
			m_goBody02.SetActive(true);
		else if (typeGold == 3)
			m_goBody03.SetActive(true);
		else if (typeGold == 4)
			m_goBody04.SetActive(true);
		else
			m_goBody05.SetActive(true);
	}
	public override void InitBuild()
	{
		base.InitBuild ();
		SetGoldFactor() ;
        //临时
		m_goEffect1901021 =GameObjectLoader.LoadPath ("effect/prefab/", "1901021", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.boneroot).transform);
		HP = fullHP;
		CM.GoldBuild = this;
	}

	private void SetGoldFactor()
	{
		//受击扣70% 的金币，
		m_fGoldFactor = CmCarbon.DefenderInfo.Coin* 0.8f/m_Attr.FullHp;
	}

	public override bool ApplyDamage (SkillReleaseInfo Info, Transform attackgo)
	{
		//Hit(Info.m_Damage);
		if ((HP + Info.m_Damage) <= 0)
			Hit(Info.m_Damage);
		bool result = base.ApplyDamage (Info, attackgo);
		return result;
	}
	public override void Hit(int damage)
	{
		SetAnimator(Build_AnimatorState.Hit20000);
		if (damage < 0)
		{
			SoundPlay.Play("coin_hit",false ,false);
			SpawnJinkuHit();
			int loss=CalcSubGold(-damage);
			CmCarbon.SetAddWinGold(loss);
			SetAnimator(Build_AnimatorState.Hit20000);
			sdata.s_resourcedropInfo Info = ConfigM.GetBuildSourceDropIRow((int)ResourceType.Gold, loss);
			if(Info != null)
			{
				int num = Info.num1;
				GoldCoinEffect(num,loss);
			}
		}

		//base.Hit(damage);
	}
	public void GoldBomeCoinEffect(float per)
	{
		int loss=(int)(CmCarbon.DefenderInfo.Coin * per);
		CmCarbon.SetAddWinGold(loss);
		sdata.s_resourcedropInfo Info = ConfigM.GetBuildSourceDropIRow((int)ResourceType.Gold, loss);
		if(Info != null)
		{
			int num = Info.num1;
			GoldCoinEffect(num,loss);
		}

	}
	public override void Shake()
	{
		SetAnimator (Build_AnimatorState.Hit20000);
	}
	public void GoldCoinEffect(int num,int count)
	{
		for(int i = 0; i < num; i++)
		{
			
			Vector3 pos = MapGrid.GetMG(m_Attr.Pos).pos;//+new Vector3(Random.Range(1,m_Attr.Size*0.5f-2),-0.5f,1f);
			pos += new Vector3(0, 0.25f, 1.2f);
			pos = BattleEnvironmentM.Local2WorldPos(pos);
			int n = (int)count/num;
			if (i == 0)
				n += count - num * ((int)count/num);
			GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000041",EffectCamera.GetEffectPos(pos) ,BattleEnvironmentM.GetLifeMBornNode(true));
			//EffectCamera.AddFollowList(gae.transform,pos);
			if(gae != null)
			{				
				GameObjectActionResourceDrop gaw = new GameObjectActionResourceDrop(2f,pos, pos + new Vector3(Random.Range(-1.5f,1.5f),0,0));
				gae.AddAction(gaw);
				GameObjectActionResourceFlyToUI gar = new GameObjectActionResourceFlyToUI();
				gar.SetData(EffectCamera.camera,WndManager.GetNGUICamera(),n,ResourceType.Gold);
				gae.AddAction(gar);
				GameObject coin = U3DUtil.FindChild(gae.gameObject,"coin");
				if (coin != null)
				{
					coin.GetComponent<ParticleSystem>().startDelay = Random.Range(1f,1.5f);
				}
			}
		}
	}

	public void BombCall(object o)
	{
		GoldBomeCoinEffect(0.05f);
	}
	public override void BeforeDead()
	{
		int loss = CmCarbon.DefenderInfo.Coin - CmCarbon.GetWinGold();
		CmCarbon.SetAddWinGold(loss);
		CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
		if(Wnd != null)
		{
			Wnd.SetCombatGold(loss,true);
			Wnd.SetCombatGold(-loss,false);
		}
		GameObjectActionExcute gae = m_thisT.gameObject.AddComponent<GameObjectActionExcute>();
		for (int i = 0; i < 4;i++)
		{
			GameObjectActionWait gaw = new GameObjectActionWait(0.5f);
			gaw.m_complete = BombCall;
			gae.AddAction(gaw);
		}
		base.BeforeDead();
	}

	public int CalcSubGold(int  damage)
	{
		/*if (HP <= 0)
			return CmCarbon.DefenderInfo.Coin - CmCarbon.GetWinGold();
		else */
			return (int)(m_fGoldFactor * damage);
	}
    /// <summary>
    /// 生成攻击金库的特效
    /// </summary>
    void SpawnJinkuHit()
    {
		/*GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1901011", m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.attackpos).transform.position, m_Property.HelpPoint.GetVauleByKey(BuildHelpPointName.attackpos).transform);
		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
		gae.AddAction(ndEffect);*/
    }

	public override void  Destroy()
	{
		m_goEffect1901021.SetActive(false);
		CombatScheduler.GoldDestroy = true;
	}
	void OnDestroy() {
		if (m_goEffect1901021)
			m_goEffect1901021.SetActive(false);
	}
}
