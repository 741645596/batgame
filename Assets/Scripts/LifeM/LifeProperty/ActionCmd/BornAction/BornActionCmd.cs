#if UNITY_EDITOR || UNITY_STANDALONE_WIN
#define UNITY_EDITOR_LOG
#endif
using UnityEngine;
using System.Collections;

/// <summary>
/// 出生动作命令
/// </summary>
/// <author>QFord</author>
public delegate void DoneCallBack();

public class BornActionCmd : GridActionCmd
{
    
    private DoneCallBack m_doneCallBack;
    protected float m_fPauseTime = 0.0f;
    private float m_fPauseTimeCounter = 0f;
    private Quaternion m_PauseRotation = Quaternion.identity;
    private AnimatorState m_state;
	/// <summary>
	/// 出生构造函数
	/// </summary>
	/// <param name="Parent">出生角色</param>
	/// <param name="JumpTarget">跳跃的位置</param>
	/// <param name="JumpDir">朝向</param>
	/// <param name="dcb">完成回调</param>
	public BornActionCmd(Life Parent, Vector3 JumpTarget, WalkDir JumpDir ,DoneCallBack dcb,float duration,AnimatorState state,bool bFallSmooth,string strBornEffect)
    {
        SetTarget(Parent);
        Transform t = m_Skin.tRoot;
		t.parent = BattleEnvironmentM.GetLifeMBornNode(true);
        m_Start = t.localPosition;
        m_PauseRotation = t.localRotation;
		m_End = JumpTarget;
		if (Parent.m_Attr.AttrType == 102003 || Parent.m_Attr.AttrType == 200009)
		{
			if (Parent.Anger <= 0)
			{
				t.localPosition = m_End;
				(Parent as Role).TurnsInto(false);
				m_Skin = Parent.GetSkin();
				Vector3 pos = m_Skin.tRoot.position;
				pos.z -= 2f;
				GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, Parent.m_Attr.AttrType == 102003 ? "1103111_01":"1401111_01", pos, BattleEnvironmentM.GetLifeMBornNode(true));
				GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.0f);
				gae.AddAction(ndEffect);
			}
		}
        else if (Parent.m_Attr.AttrType == 101004)
        {
            Vector3 pos = m_Skin.tRoot.position;
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1055021", pos, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.0f);
            gae.AddAction(ndEffect);
        }
        m_doneCallBack = dcb;
		m_Dir = JumpDir;
        m_Duration = duration;
        m_state = state;
        m_fPauseTime = 0f;
		//RolePlayAnimation(AnimatorState.Stand);
		//SetBornActionCmd();
        if (!bFallSmooth)
            m_Start = JumpTarget;
		m_Skin.tRoot.rotation = Quaternion.identity;

		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
		if (posgo != null&&strBornEffect.Length>0)
		{
			Vector3 pos = posgo.transform.position;
			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, strBornEffect, pos, BattleEnvironmentM.GetLifeMBornNode(true));
			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(duration);
			gae.AddAction(ndEffect);
		}
    }


	/*public virtual void SetBornActionCmd()
	{
		m_Duration = 2.05f;
		m_fPauseTime = 0f;
	}*/



    public override void Update()
    {
        if (m_fPauseTimeCounter < m_fPauseTime)
        {
            m_fPauseTimeCounter += Time.deltaTime;
            return;
        }

        base.Update();
        Vector3 pos = Vector3.Lerp(m_Start, m_End, m_TimeCount / m_Duration);
        PlayAction(m_state, pos, true);
    }



    public override bool IsDone()
    {
        if (base.IsDone() == true)
        {
            m_doneCallBack();
            return true;
        }
        else return false;
    }
}


public class BornActionCmdFactory
{
	/// <summary>
	/// 创建FireActionCmd
	/// </summary>
	public static BornActionCmd Create(Life Parent, Vector3 JumpTarget, WalkDir JumpDir ,DoneCallBack dcb,AnimatorState state,bool bFallSmooth)
	{
		string strBornEffect = "";
		int ModeType = Parent.m_Attr.ModelType;
		float fduration = 0.0f;
		if (ModeType == 100002|| ModeType == 200001)
			fduration = 1.167f;
		else if (ModeType == 100001|| ModeType == 200000)
			fduration=1.5f;
		else if (ModeType == 100003||ModeType == 100004||ModeType == 101001)
			fduration=1f;
        else if (ModeType == 101002)
        {       
            if (bFallSmooth)
            {
                fduration = 1.067f;
            }
            else
            {
                fduration = 4.167f;
            }
        }
        else if (ModeType == 102001)
			fduration=2.0f;
		else if (ModeType == 102002)
			fduration=1.333f;
		else if (ModeType == 102005)
			fduration=1.0f;
		else if (ModeType == 101003)
		{
			fduration=1.06f;
			strBornEffect="1054011";
		}
		else if (ModeType == 200002)
			fduration=1.0f;
		else if (ModeType == 200003)
			fduration=1.5f;
		else if (ModeType == 200004)
			fduration=1.167f;
		else if (ModeType == 200005)
			fduration=1.167f;
		else if (ModeType == 200006)
			fduration=1.333f;
		else if (ModeType == 200007)
			fduration=1.167f;
		else if (ModeType == 200008)
			fduration=1.0f;
		else if (ModeType == 102003 || ModeType == 200009)
			fduration=1.333f;
		else if (ModeType == 3000)
			fduration=1.5f;
		else if (ModeType == 102004)
            fduration = 1.167f;
        else if (ModeType == 101004)
        {
            fduration = 1f;
			(Parent.GetSkin() as RoleSkin).SetVisable(true);
			(Parent.GetSkin() as RoleSkin).EnableTrail(false);
        }
		else if (ModeType == 103003)
		{
			fduration = 1f;
			(Parent.GetSkin() as RoleSkin).SetVisable(true);
			(Parent.GetSkin() as RoleSkin).EnableTrail(false);
		}
		else if(ModeType == 103002)
		{
			fduration=1.167f;
		}
        else if (ModeType == 103001)
        {
            fduration = 1.167f;
			(Parent.GetSkin() as RoleSkin).SetVisable(true);
			GameObject posgo = Parent.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectBottomPos);
            if (posgo != null)
            {
                Vector3 pos = posgo.transform.position;
                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1201011", pos, posgo.transform);
                GameObjectActionRepeat ndEffect = new GameObjectActionRepeat();
                gae.AddAction(ndEffect);
            }
        }

        else
        {
            Debug.LogError("BornActionCmdFactory 请创建对应的炮战Action");
            return null;
        }
		#if UNITY_EDITOR_LOG
		FileLog.write(Parent.SceneID,"Born  "+  ModeType + "," + fduration );
		#endif
		return new BornActionCmd(Parent, JumpTarget, JumpDir, dcb, fduration, state, bFallSmooth,strBornEffect);
	}
}
