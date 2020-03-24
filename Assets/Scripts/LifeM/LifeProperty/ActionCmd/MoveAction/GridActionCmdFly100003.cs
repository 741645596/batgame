using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 蹦蹦与鹏大炮战飞行,点击可弹出蹦蹦来。
/// 
/// </summary>
/// <author>zhulin</author>
public class GridActionCmdFly100003 : GridActionCmdFly
{
	private bool m_bClickSend = false;
    private float m_fSendPetCounter = 0.0f;
    private float m_fPauseFlyCounter = 0.0f;
    private float m_fSpeedFactor = 1.0f;
    private bool m_bSendPet = false; 

	public GridActionCmdFly100003(Life Parent, List<Vector3> pos, float speed, int ModelType)
		: base(Parent, pos, speed, ModelType)
	{
       // Debug.Log("GridActionCmdFly100003");
        RoleSkin r = m_Skin as RoleSkin;
        if (r != null)
        {
			GameObject posgo = r.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.LeftHandPos);
			if (posgo!= null)
            {
				RolePropertyM rp = posgo.GetComponentInChildren<RolePropertyM>();
				if (rp != null)
					r.ChangeState(AnimatorState.Fly00000);
            }
        }
        r.EnableColider(true);
	}

	public override void SetLineDir(Vector3 FlyDir)
	{

	}

	public override void Update()
	{
		base.Update();
		if(m_bClickSend == false)
		{
			ClickAction();
		}
	}
	
	public override void TrailFly()
    {
        m_flyState = AnimatorState.Fly00000;
        PlayAction(m_flyState, m_FlyPos, true);
	}

	/// <summary>
	/// 点击蹦蹦与蹦大，跳出蹦蹦。
	/// </summary>
	private void ClickAction()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5-Camera.main.transform.position.z, 1 << LayerMask.NameToLayer("Role")))
			{
				Role w = hit.transform.GetComponentInParent<LifeProperty>().GetLife() as Role;
                if (w!=null && w.SceneID == m_LifePrent.SceneID)
                {
                    //Debug.DrawLine(ray.origin, hit.point,Color.red,10);
                    //射出蹦蹦，创建出宠物蹦蹦。
                    //崩大进入直线掉落，动作切换为落地动作
                    //关闭蹦大的点击碰撞器
                    (m_Skin as RoleSkin).EnableColider(ColiderType.Click, false);
                    FlyPet();
                    m_flyState = AnimatorState.FlyClick00300;
                    m_fPauseFlyCounter = 1.867f;
                    m_fSendPetCounter = 0.2f;
                    m_bClickSend = true;
                    m_FlyDir = Vector3.down;
                    m_Lastdir = m_FlyDir;
                    m_fSpeedFactor = 5f;
                    ClearFlyData();
                    m_Duration = 600;
                    m_TraiOver = true;
                    m_FlyCollisionInfo.m_flyFallStandState = AnimatorState.FlyFallStand01200;
                }
			}
		}
	}

    public override void DoFly()
	{
        if (m_TraiOver == false)
        {
			m_IsTraceFly = true;
            if (GetFlyData() == true)
            {
                TrailFly();
            }
            else
            {
                ClearFlyData();
                m_Duration = 600;
                m_TraiOver = true;
            }
        }
        else
		{
            if (m_fPauseFlyCounter>0)
			{
				m_IsTraceFly = true;
                m_fPauseFlyCounter -= Time.deltaTime;
                MoveAction(m_flyState, Vector3.zero, ActionMode.Delta);
                return;
            }
            else
			{
				m_IsTraceFly = false;
                MoveAction(m_flyState, m_Lastdir * m_fSpeed * m_fSpeedFactor * Time.deltaTime, ActionMode.Delta);
            }
        }
    }

	/// <summary>
	/// 飞行出蹦蹦
	/// </summary>
	private void FlyPet()
	{
		DelHandPet();
		Pet pet = new Pet();
		pet.CreateSkin(m_Skin.tRoot.parent, 1002, "1002", AnimatorState.FlyClick00300,m_LifePrent.m_Core.m_IsPlayer);
       // IGameRole i = GameRoleFactory.Create(m_Skin.tRoot.parent, 1002, "1002", AnimatorState.FlyClick00300);
        GameObject goPet = pet.PetSkinCom.tRoot.gameObject;
		GameObject navelpos = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.NavelPos);
		if (navelpos != null)
			goPet.transform.position = navelpos.transform.position;
        U3DUtil.SetZ(goPet.transform.position, -1f);
		LifeObj lo = goPet.AddComponent<LifeObj>();

        m_FlyCollisionInfo.SummonPet = goPet;
       
		pet.SetBorn(m_LifePrent,1002001,1002, m_LifePrent.m_thisT.position);
		pet.SetSkin();
		//pet.SetMoveState(MoveState.Fly);
		if (m_LifePrent is Role)
		{
			(m_LifePrent as Role).CurPet = pet;
		}
		lo.SetLife(pet ,pet.PetSkinCom.ProPerty);
	}
    
    private void DelHandPet()
    {
         GameObject go = U3DUtil.FindChildDeep(m_Skin.tRoot.gameObject, "1002@skin");
         Vector3 pos = Vector3.zero;
         if (go)
         {
              pos = go.transform.position;
              pos.z = -0.5f;
              //go.SetActive(false);
              Object.Destroy(go, 0.5f);
          }
    }
}