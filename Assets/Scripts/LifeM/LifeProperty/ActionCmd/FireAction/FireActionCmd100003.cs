using UnityEngine;
using System.Collections;

/// <summary>
/// 蹦大和蹦蹦 炮战
/// </summary>
/// <author>QFord</author>
public class FireActionCmd100003 : FireActionCmd {

    private bool m_bDolphineFly = false;

    public FireActionCmd100003(Life Parent, Vector3 FlyDir, float FlySpeed, bool isDolphineFly)
		:base(Parent ,FlyDir ,FlySpeed, false )
	{
        m_bDolphineFly = isDolphineFly;
        if (m_bDolphineFly)
        {
            HideHandPet();
        }
	}


	/// <summary>
	/// 撞击出生前置动作
	/// </summary>
	public override void FlyReboundAction(bool bVertical)
	{
		Transform t = m_Skin.tRoot;
		float pauseDuration =0.1f;
		SetCollEffect(bVertical);
		
		//撞击墙体跌落时调整
		if (bVertical && m_FlyDir == Vector3.down)
		{
			t.rotation = new Quaternion(0, 0, 0.7f, -0.7f);
			t.position += new Vector3(0, 0.5f, 0);
		}
		else
		{
			RolePlayAnimation(AnimatorState.Fly00000);
		}
		
		PauseFly(pauseDuration);
		if ((m_LifePrent as Role).CurPet == null && !m_bDolphineFly)
		{
			CreatePet();
		}
    }

    void CreatePet()
    {
		HideHandPet();
		Pet pet = new Pet();
		pet.CreateSkin(BattleEnvironmentM.GetLifeMBornNode(true), 1002, "1002", AnimatorState.FlyClick00300,m_LifePrent.m_Core.m_IsPlayer);
		//IGameRole i = GameRoleFactory.Create(BattleEnvironmentM.GetLifeMBornNode(true), 1002, "1002", AnimatorState.FlyClick00300);
        GameObject goPet = pet.PetSkinCom.tRoot.gameObject;
        goPet.transform.position = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.petFollowPos).transform.position;
		LifeObj lo = goPet.AddComponent<LifeObj>();
		Int2 toGrid = m_LifePrent.GetMapPos();
		pet.SetBorn(m_LifePrent, 1002001,1002, toGrid,m_LifePrent.WalkDir);
		pet.SetSkin();
		if (m_LifePrent is Role)
		{
			(m_LifePrent as Role).CurPet = pet;
			//pet.SetMoveState(MoveState.Walk);
			// pet.m_petState = Pet1002State.Follow;
		}
		lo.SetLife(pet , pet.PetSkinCom.ProPerty);
    }

    private void HideHandPet()
    {
        GameObject go = U3DUtil.FindChildDeep(m_Skin.tRoot.gameObject, "1002@skin");
        if (go)
        {
            go.SetActive(false);
        }
    }

	/// <summary>
	/// 撞传板暂停飞行
	/// </summary>
	public override void PauseContinueFlight(bool bVertical)
	{
        Transform t = m_Skin.tRoot;
		float pauseDuration =0.1f;
		SetCollEffect(bVertical);
		
		//撞击墙体跌落时调整
		if (bVertical && m_FlyDir == Vector3.down)
		{
			t.rotation = new Quaternion(0, 0, 0.7f, -0.7f);
			t.position += new Vector3(0, 0.5f, 0);
		}
		else
		{
            RolePlayAnimation(AnimatorState.Fly00000);
		}
		PauseFly(pauseDuration);
	}
}
