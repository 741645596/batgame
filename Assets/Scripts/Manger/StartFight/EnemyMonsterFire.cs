using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// 非玩家发射炮弹兵
/// </summary>
/// <author>zhulin</author>
public class EnemyMonsterFire : MonoBehaviour
{

	private float m_timeElapse = 0.0f;
	private int m_RoleID = -1;
	private List<int> m_WatingRoleIDs = new List<int>();
	//private List<int> m_FiringRoleIDs = new List<int>();
	private Dictionary<int, SoldierInfo> m_FiringRoleMaps = new Dictionary<int, SoldierInfo>();
	private Dictionary<int, List<Vector3>> m_FlyPostionMap = new Dictionary<int, List<Vector3>>();
	private Dictionary<int, float> m_CountDownMap = new Dictionary<int, float>();
	private SoldierInfo m_Soldier = new SoldierInfo();
	private List<Vector3> m_vFlyPostions = new List<Vector3>();

	BattleIncomingWnd mBattleIncomingWnd = null;
	float mWaintTime = 5f;

	void Start()
	{
		PlayWarning();

		mBattleIncomingWnd = WndManager.GetDialog<BattleIncomingWnd>();
		mBattleIncomingWnd.MonsterCount = CmCarbon.GetFireSoldierCount();
	}

	void Update()
	{
		if (CombatScheduler.State == CSState.Pause)
		{
			return;
		}
		if (CombatScheduler.State == CSState.End)
		{
			Destroy(this);
			return;
		}

		if (CombatScheduler.State == CSState.Start || CombatScheduler.State == CSState.Ready)
		{
			CombatScheduler.SetCSState(CSState.Combat);
		}

		m_timeElapse += Time.deltaTime;

		CreateWatingSoldiers();
		UpdateWatingSoliderUI();
		FrieSoldiers();
	}

	void CreateWatingSoldiers()
	{
		List<int> watingRoleIDs = CmCarbon.GetFireSoldiers(m_timeElapse, mWaintTime);
		for (int i = 0; i < watingRoleIDs.Count; i++)
		{
			int watingID = watingRoleIDs[i];
			if (!m_WatingRoleIDs.Contains(watingID))
			{
				m_WatingRoleIDs.Add(watingID);
				m_CountDownMap[watingID] = m_timeElapse;

				mBattleIncomingWnd.CreateEnemySoldier(watingID);
			}
		}
	}

	void UpdateWatingSoliderUI()
	{
		for (int i = 0; i < m_WatingRoleIDs.Count; i++)
		{
			int watingID = m_WatingRoleIDs[i];
			float ratio = (m_timeElapse - m_CountDownMap[watingID]) / mWaintTime;
			mBattleIncomingWnd.SetEnemyProgress(watingID, ratio);
		}
	}

	void FrieSoldiers()
	{
		List<int> firingRoleIDs = CmCarbon.GetFireSoldiers(m_timeElapse, 0f);
		for (int i = 0; i < firingRoleIDs.Count; i++)
		{
			int firingID = firingRoleIDs[i];

			if (m_WatingRoleIDs.Contains(firingID))
			{
				m_WatingRoleIDs.Remove(firingID);
				m_CountDownMap.Remove(firingID);
				mBattleIncomingWnd.DestroyEnemySoldier(firingID);
			}

			if (!m_FiringRoleMaps.ContainsKey(firingID))
			{
				SoldierInfo soldier = CmCarbon.GetSoldierInfo(LifeMCamp.ATTACK, firingID);
				m_FiringRoleMaps[firingID] = soldier;

				List<Vector3> FlyLine = new List<Vector3>();
				Vector3 BornPos = Vector3.zero;
				FireAI.GetFlyBorn(m_Soldier.m_FireAI, ref FlyLine, ref BornPos);

				m_FlyPostionMap[firingID] = FlyLine;
				PlayEnemyFlyHitPoint(BornPos);
			}
		}

		List<int> removeList = new List<int>();
		foreach (KeyValuePair<int, SoldierInfo> keyVal in m_FiringRoleMaps)
		{
			int soldierID = keyVal.Key;
			SoldierInfo soldier = keyVal.Value;

			List<Vector3> FlyLine = m_FlyPostionMap[soldierID];
			if (FlyLine != null && soldier != null)
			{
				if (SoldierFire.Fire(FlyLine, soldierID, soldier, false) == true)
				{
					CmCarbon.SetBorn(soldierID);
					removeList.Add(soldierID);
				}
			}
		}

		for (int i = 0; i < removeList.Count; i++)
		{
			int removeID = removeList[i];
			m_FiringRoleMaps.Remove(removeID);
		}
	}


	/// <summary>
	/// 播放敌方来袭
	/// </summary>
	private void PlayWarning()
	{
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000501_01",
														Vector3.zero, WndManager.GetNGUIRoot().transform);
		if (gae != null)
		{
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2.0f);
			gae.AddAction(ndEffect);
		}
	}

	/// <summary>
	/// 防御战 战前警报 “感叹号”敌方飞入点提示
	/// </summary>
	private void PlayEnemyFlyHitPoint(Vector3 pos)
	{
		pos = U3DUtil.SetZ(pos, -1f);//保证特效在炮弹兵前
		GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000501_02",
														pos, BattleEnvironmentM.GetLifeMBornNode(true));
		if (gae != null)
		{
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
			gae.AddAction(ndEffect);
		}
	}


}
