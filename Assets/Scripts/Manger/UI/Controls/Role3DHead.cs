/// <summary>
/// 角色3D头像
/// </summary>
/// <Author>QFord</Author>
/// <Data>2014-11-20   11:16</Data>
/// <Path>E:\Projs\SVN_Root\trunk\SeizeTheShip\Assets\Scripts\Client\UI\Controls</Path>

using UnityEngine;
using System.Collections;
/// <summary>
/// 角色3D头像
/// </summary>
public class Role3DHead  {

   /* private IGameRole m_GameRole = null;

	void Start () {
	
	}
     void Set3DHead(Transform head3D,int roleType, AnimatorState emAniState,string roleName)
    {
        //Debug.Log("head3D="+head3D.name + "    roleType=" +roleType + "    AnimatorState=" + emAniState);
         if (head3D == null)
         {
            Debug.Log("Role3DHead.cs->Set3DHead  head3D is null !!!");
             return;
         }
         Vector3 scale ;
         switch (roleType)
         {
             case 100001://企鹅刀兵和枪兵
                 scale = new Vector3(50f,50f,50f);
                 break;
             case 100002:
             case 101001://一刀喵
                 scale = new Vector3(35f, 35f, 35f);
                 break;

             default:
                 scale = new Vector3(40f, 40f, 40f);
                 break;
         }

        if (m_GameRole == null)
            m_GameRole = GameRoleFactory.Create(head3D, roleType, roleName, emAniState);
        else
            m_GameRole.ExcuteCmd(emAniState);
		//Debug.Log(emAniState);
        GameObjectLoader.SetGameObjectLayer(m_GameRole.GetGameRoleRootObj(), head3D.gameObject.layer);
        m_GameRole.GetGameRoleRootObj().transform.localScale = scale;
    }

    public void SetRoleType(Transform head3d,int roleModelType, AnimatorState emAniState,string roleName)
    {
        if (head3d == null)
        {
            Debug.Log("head3d is null !!!");
            return;
        }
        Set3DHead(head3d,roleModelType, emAniState, roleName);
    }*/
}
