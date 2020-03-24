using UnityEngine;
using System.Collections;

/// <summary>
/// 海豚顶飞
/// </summary>
public class GridActionDophineFly :GridActionCmd
{
	Bezier bezier;
	float m_fDelay = 0.3f;
	Missile m; //启用翻滚
	public float m_curangle;
    private bool m_bRotate = true;
	private WalkDir dolphineDir;
	private Vector3 start;
    private string m_effectName;
    /// <summary>
    /// 炮弹兵落水位置
    /// </summary>
    private Vector3 m_v3SpawnWater;
	public GridActionDophineFly(Life Parent,WalkDir Dir , float duration,float delay,Vector3 v3SpawnWater )
	{
		m_fDelay = delay;
        m_v3SpawnWater = v3SpawnWater;
        if (Parent.m_Attr.AttrType == 100003 || Parent.m_Attr.AttrType == 101002)
        {
            m_bRotate = false;
        }
		 start = Vector3.zero;
		Vector3 end = Vector3.zero;
		
		if(BattleEnvironmentM.GetDolphineLine(Dir,ref start, ref end) == false)
			return ;
		if (v3SpawnWater!=Vector3.zero)
		{
			start = v3SpawnWater;
		}
		//海豚顶特效 这里的Dir是炮弹兵被顶飞的方向
         dolphineDir = (Dir == WalkDir.WALKLEFT ? WalkDir.WALKRIGHT : WalkDir.WALKLEFT);
         if (Dir == WalkDir.WALKRIGHT)
         {
             m_effectName = "2000201_01";
         }
         else
         {
             m_effectName = "2000201_02";
         }

		SetTarget(Parent);
		m = Parent.m_thisT.gameObject.AddComponent<Missile>();
        m.m_bParticleRotate = false; //分支        
		Parent.m_thisT.rotation = Quaternion.identity;
		//保证左侧顶飞头部着地
        if (Dir == WalkDir.WALKLEFT)
        {
            Dir = WalkDir.WALKRIGHT;
        }
		SoundPlay.Play("drop_sea", false, false);
		base.SetData(start, end, RoleState.HITFLY, duration, -1, dolphineDir, 0);
		bezier = new Bezier(start, new Vector3(0, 25, 0), new Vector3(0, 25, 0), end);
        
        (Parent.MoveAI as Fly).m_FlyInfo.Reset();
	}
	
	public override void Update()
	{
		base.Update();
		if (m_TimeCount < m_fDelay)
		{
			m_LifePrent.m_thisT.transform.position = start;
			return;
		}
		if (m_TimeCount<(m_fDelay +3.02f))
        {
			if ((m_TimeCount -m_Delatime) <= m_fDelay)
			{
                SoundPlay.Play("back_ship", false, false);

				//SkillEffects._instance.LoadEffect("effect/prefab/", "2000201", start, 1.5f, dolphineDir);

                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", m_effectName, start, null);
				if(gae != null)
				{
					GameObjectActionEffectInit effectinit = new GameObjectActionEffectInit();
					//effectinit.SetEffectMirror(dolphineDir);
					gae.AddAction(effectinit);
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.5f);
					gae.AddAction(ndEffect);
				}
			}
            Vector3 pos = bezier.GetPointAtTime((m_TimeCount - m_fDelay) / 3.0f);
			float Angles = NdUtil.V2toAngle(pos, bezier.GetPointAtTime((m_TimeCount - m_fDelay) / 3.0f + 0.01f), Vector3.right) + 180;
			m_curangle =  NdUtil.ClampAngle(Angles + 180, 0, 360f);
			//Debug.Log(m_curangle);
            if (m_bRotate)
            {
                if (dolphineDir == WalkDir.WALKRIGHT)
                {
                    m.SetAngle(-Angles);
                }
                else
                {
                    m.SetAngle(Angles);
                }
            }
            PlayAction(AnimatorState.Fly00000, pos, true);
            //NGUIUtil.DebugLog(m_TimeCount.ToString());
            //if (m_TimeCount >= 0.4f && m_TimeCount < 0.4f +Time.deltaTime)
            //{
            //    m_Skin.ProPerty.EnableTrail(true);
            //}
        }
        else
        {
			//PlayAction(AnimatorState.Fly00000, bezier.GetPointAtTime(1), true);
            //MoveAction(AnimatorState.Fly00000, Vector3.down * m_LifePrent.m_Attr.Flyspeed * Time.deltaTime, ActionMode.Delta);
            MoveAction(AnimatorState.Fly00000, Vector3.down * 10 * Time.deltaTime, ActionMode.Delta);


        }
	}
    public override void SetDone()
    {
        base.SetDone();
        Object.Destroy(m);
    }
	public FlyDir GetDir()
	{

		if (m_curangle > 90 && m_curangle < 180)
			return FlyDir.RightTop;
		if (m_curangle > 0 && m_curangle < 90)
			return FlyDir.LeftTop;
		if (m_curangle > 270 && m_curangle < 360)
			return FlyDir.LeftBottom;
		if (m_curangle > 180 && m_curangle < 270)
			return FlyDir.RightBottom;
		if (m_curangle == 90 )
			return FlyDir.Top;
		if (m_curangle == 270 )
			return FlyDir.Bottom;
		if (m_curangle == 180)
			return FlyDir.Right;
		if (m_curangle == 0)
			return FlyDir.Left;
		return FlyDir.none;
		
	}
	public override void Finish ()
	{
		base.Finish ();
		SkillStatusInfo ssi = SkillM.GetWetBodyStatusInfo();
		m_LifePrent.m_Status.AddStatus(0,0,ssi);
	}
}
