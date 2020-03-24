 using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GridActionCmdFly103003 : GridActionCmdFly
{
	public GridActionCmdFly103003(Life Parent, List<Vector3> pos, float speed, int ModelType)
        : base(Parent, pos, speed, ModelType)
    {

        m_flyState = AnimatorState.Fly00000;
        m_bEnableRotate = false;
       // m_Skin.Scale(Vector3);
        (m_Skin as RoleSkin).SetVisable(false);
        RolePlayAnimation(m_flyState);
    }
}