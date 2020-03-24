#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    #define UNITY_EDITOR_LOG
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*public class GridActionCmdWalkToBack : GridActionCmd{
	public MapGrid m_des;
	public GridActionCmdWalkToBack(Vector3 start,Vector3 end, float duration ,WalkDir dir, int deep,MapGrid g)
	{
		//Debug.Log("start=" + start+",end="+end);
		base.SetData(start,end,RoleState.JUMP,duration,-1,dir,deep);

	}
	public override   void Update () {
		base.Update();
		Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);
		if (m_RankDeep >= 2)
			m_RankDeep = 1;
		pos.z = -0.7f * m_RankDeep;
		if (m_Dir == WalkDir.WALKSTOP)
			PlayAction(AnimatorState.Stand,pos,true);
		else
		{
			PlayAction(AnimatorState.Walk,pos,true);
		}

	}

	public override float GetSpeed ()
	{
		if (m_LifePrent != null)
			return m_LifePrent.m_Attr.SpeedPercent;
		return 1;
	}
	public override void Finish ()
	{
		base.Finish ();
		(m_LifePrent as Role).RoleWalk.Teleport(m_des);
	}
}*/
/// <summary>
/// 一刀喵技能，编号101001
/// </summary>
public class GridActionCmd101001Skill01 : GridActionCmdAttack
{
    public GridActionCmd101001Skill01(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
        : base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
    {
    
        m_CastTime = 0.35f;
        m_EventTime = 0.35f;
        m_Duration = 1.067f;
    }
    public override  void StartWithTarget(Life Parent, StartAttackFun StartAttack)
    {
        base.StartWithTarget(Parent, StartAttack);
        if (m_LifePrent.m_Attr.IsHide) {
            if (((m_LifePrent as Role).m_Skill.m_AttackTarget == (m_LifePrent as Role).Target) && (m_LifePrent as Role).m_Skill.m_AttackTarget is Role) {
                m_LifePrent.WalkDir = (m_LifePrent as Role).m_Skill.GetHideDir((m_LifePrent as Role).Target);
                (m_LifePrent as Role).RoleWalk.run.m_AttackDir = m_LifePrent.WalkDir;
                m_Dir = m_LifePrent.WalkDir;
            }
        }
        PlayAction(AnimatorState.Attack85000, m_Start);
    }
    public override   void DoUpdate()
    {
    
    }
    
    public override void DoEvent()
    {
        base.DoEvent();
        
        if (m_LifePrent.m_Attr.IsHide) {
            if ((m_LifePrent as Role).Target is Role) {
                GameObject posgo = (m_LifePrent as Role).Target.GetSkin().ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
                if (posgo != null) {
                    //GameObject go = SkillEffects._instance.LoadEffect("effect/prefab/", "1051151",posgo.transform.position,1f,true);
                    GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051061", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
                    GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.6f);
                    gae.AddAction(ndEffect);
                }
            }
        }
        SoundPlay.Play("act_yidao", false, false);
    }
    
}
public class GridActionCmd101001ConditionSkill01 : GridActionCmdConditionSkill
{
    public GridActionCmd101001ConditionSkill01(Vector3 start, Vector3 end, DoAttackFun fun, WalkDir dir, DoQianyaoFun qianyaofun)
    {
        m_Start = start;
        m_End = end;
        m_CastTime = m_eventtime = 0.666f;
        m_Duration = 1.667f;//0.833f;
        m_realDuration = m_Duration;
        m_DoSkill = fun;
        m_Dir = dir;
        m_QianYaoStatus = qianyaofun;
    }
    public override   void StartWithTarget(Life Parent)
    {
        base.StartWithTarget(Parent);
        GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectTopPos);
        if (posgo != null) {
            //GameObject go = SkillEffects._instance.LoadEffect("effect/prefab/", "1051151",posgo.transform.position,1f,true);
            GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051151", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
            gae.AddAction(ndEffect);
            GameObject go = gae.gameObject;
        }
        
    }
    public override void UpdatePos()
    {
        //Vector3 pos = Vector3.Lerp(m_Start,m_End,m_TimeCount / m_Duration);
        PlayAction(AnimatorState.Attack82000, m_Start);
        if (m_TimeCount >= m_eventtime) {
        
            //m_LifePrent.m_Attr.CanOpenDoor = true;
        }
    }
    
}
public class GridActionCmd101001Skill02 : GridActionCmdAttack
{
    int m_JumpCount;
    GridSpace m_gs;
    float m_oncejumptime;
    WalkDir m_desdir;
    Vector3 m_TargetPos;
    float m_MoveTime;
    float m_action3;
    float m_Delay;
    MapGrid m_TelePortGrid;
    public GridActionCmd101001Skill02(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
        : base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
    {
        m_Duration = 1.333f;
        m_CastTime = 0.4f;
        m_EventTime = 0.4f;
        /*m_MoveTime = 0.15f;
        m_Delay = 0.5f;
        m_CastTime =m_Delay+ 0.3f;
        m_EventTime =m_Delay+ 0.3f;
        m_Duration =m_Delay + 1.133f;*/
        m_Dir = AttackDir;
    }
    public override  void StartWithTarget(Life Parent, StartAttackFun StartAttack)
    {
        base.StartWithTarget(Parent, StartAttack);
        /*	Life emeny =  CM.GetAllLifeM(m_AttackSceneID,LifeMType.ALL);
        
        	GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051041", BattleEnvironmentM.Local2WorldPos(m_TargetPos), BattleEnvironmentM.GetLifeMBornNode(true));
        	GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_action3);
        	gae.AddAction(ndEffect);
            GameObject go = gae.gameObject;
        	go.transform.parent = m_Skin.tRoot;
        	go.transform.localPosition = Vector3.zero;
        	MapGrid gfrom = m_LifePrent.GetMapGrid();
        	m_TelePortGrid = gfrom;
        	MapGrid gstart = gfrom;
        	//m_Dir = WalkDir.WALKRIGHT;
        	MapGrid gto = emeny.GetTargetMapGrid();
        	int startunit = MapSize.GetGridStart(gto.GridPos.Layer);
        	int endtunit = startunit + MapSize.GetLayerSize(gto.GridPos.Layer) - 1;
        	//if (gto.GridPos.Layer > 0)
        	MapGrid start = MapGrid.GetMG(gto.GridPos.Layer,startunit);
        	MapGrid end = MapGrid.GetMG(gto.GridPos.Layer,endtunit);
        	MapGrid gl = gto.GetNextAttackStation(DIR.LEFT,false);
        	MapGrid gr = gto.GetNextAttackStation(DIR.RIGHT,false);
        	if (emeny is Role)
        		m_TargetPos = emeny.transform.localPosition;
        	else
        		m_TargetPos = gto.pos;
        	m_Start = m_Skin.tRoot.localPosition;
        	m_End = m_Start;
        	if (gfrom.GridPos.Unit > gto.GridPos.Unit)
        	{
        		m_Dir = WalkDir.WALKLEFT;
        		m_Start = gfrom.pos;
        		if (gr != null)
        			m_Start = gr.pos;
        		if (gl != null)
        		{
        			m_TelePortGrid = gl;
        			m_End = gl.pos;
        		}
        		else if (gr != null){
        			m_TelePortGrid = gr;
        			m_End = m_TelePortGrid.pos;
        		}
        		else
        			Debug.Log(gto.GridPos + "左右都没有攻击位");
        	}
        	else
        	{
        		m_Dir = WalkDir.WALKRIGHT;
        		m_Start = gfrom.pos;
        		if (gl != null)
        			m_Start = gl.pos;
        		if (gr !=null)
        		{
        			m_TelePortGrid = gr;
        			m_End = m_TelePortGrid.pos;
        		}
        		else if (gl != null)
        		{
        			m_TelePortGrid = gl;
        			m_End = m_TelePortGrid.pos;
        		}
        		else
        			Debug.Log(gto.GridPos + "左右都没有攻击位");
        	}
        	m_LifePrent.InBoat =false;
        	GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
        	if (posgo != null)
        	{
        		//GameObject gobj = SkillEffects._instance.LoadEffect("effect/prefab/", "1051161",posgo.transform.position,1.0f);
        		GameObjectActionExcute gae1 = EffectM.LoadEffect(EffectM.sPath, "1051161", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
        		GameObjectActionDelayDestory ndEffect1 = new GameObjectActionDelayDestory(1f);
        		gae1.AddAction(ndEffect1);
                GameObject gobj = gae1.gameObject;
        	}
        	m_Skin.ProPerty.SetVisable(false);*/
#if UNITY_EDITOR
        //Debug.Log("闪烁突袭 " + m_Start + "," + m_End + "," + m_TelePortGrid.GridPos + "," + m_Dir);
        //FileLog.write(m_LifePrent.SceneID, "闪烁突袭 "+ m_Start + "," + m_End + "," + m_TelePortGrid.GridPos + "," + m_Dir + "," + gfrom.GridPos + "," + gto.GridPos);
#endif
    }
    public override   void DoUpdate()
    {
        /*if (m_TimeCount < m_Delay)
        {
        
        }
        else if (m_TimeCount < m_Duration)
        {
        	m_Skin.ProPerty.SetVisable(true);
        	float cout = m_TimeCount - m_Delay;
        	float t1 = 0.33f;
        
        	float duration = m_MoveTime/2;
        	Vector3 effectpos = m_TargetPos;
        	effectpos.y += 0.8f;
        	Vector3 pos = m_Start;
            if (cout >= t1 && (cout - m_Delatime) < t1)
            {
        		GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051061", BattleEnvironmentM.Local2WorldPos(effectpos), BattleEnvironmentM.GetLifeMBornNode(true));
        		GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
        		gae.AddAction(ndEffect);
            }
        
        	if (cout<= m_MoveTime)
        	{
        		if (cout < duration)
        		{
        			pos = Vector3.Lerp(m_Start,m_TargetPos,(cout)/duration);
        		}
        		else
        			pos = Vector3.Lerp(m_TargetPos,m_End,(cout - duration) / duration );
        	}
        	else if (cout > m_MoveTime)
        		pos = m_End;
        	PlayAction(AnimatorState.Skill01,pos,true);
        	#if UNITY_EDITOR_LOG
        	FileLog.write(m_LifePrent.SceneID, "一刀喵闪烁突袭：" + pos);
        	#endif
        	if ((m_TimeCount - m_Delatime ) < m_Delay)
        	{
        		GameObject posgo = m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
        		if (posgo != null)
        		{
        			//SkillEffects._instance.LoadEffect("effect/prefab/", "1051161",posgo.transform.position,1.0f);
        			GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051161", posgo.transform.position, BattleEnvironmentM.GetLifeMBornNode(true));
        			GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
        			gae.AddAction(ndEffect);
        		}
        	}
        }*/
        PlayAction(AnimatorState.Attack83000, m_Start);
    }
    
    public override void DoEvent()
    {
        base.DoEvent();
    }
    public override void Finish()
    {
        base.Finish();
        /*m_Skin.ProPerty.SetVisable(true);
        m_LifePrent.InBoat = true;
        if (IsPlayed())
        {
        	(m_LifePrent as Role).RoleWalk.Teleport(m_TelePortGrid);
        }*/
    }
}
public class GridActionCmd101001LSkill01 : GridActionCmdAttack
{

    public GridActionCmd101001LSkill01(DoQianyaoFun qianyaofun, DoAttackFun fun, int AttackSceneId, WalkDir AttackDir, int deep, int skillid)
        : base(qianyaofun, fun, AttackSceneId, AttackDir, deep, skillid)
    {
        m_Duration = 1;
        m_CastTime = 0.2333f;
        m_EventTime = 0.2333f;
        m_Dir = AttackDir;
    }
    public override  void StartWithTarget(Life Parent, StartAttackFun StartAttack)
    {
        base.StartWithTarget(Parent, StartAttack);
        
    }
    public override   void DoUpdate()
    {
    
        PlayAction(AnimatorState.Attack81000, m_Start);
    }
    
}
public class GridActionCmd101001ActiveSkill : GridActionCmdActiveSkill
{

    int m_JumpCount;
    List<Vector3> lpos;
    GridSpace m_gs;
    Vector3 m_TargetPos;
    float m_oncejumptime;
    GameObject m_tail;
    float m_JumpTime;
    float m_MoveTime;
    float m_action2;
    float m_action3;
    MapGrid m_TelePortGrid;
    MapGrid m_PreTelePortGrid;
    public GridActionCmd101001ActiveSkill(DoQianyaoFun qianyaofun, DoAttackFun fun, int sceneID, int AttackSceneId, WalkDir AttackDir, int deep, int skillid, float blackscreentime)
        : base(qianyaofun, fun, sceneID, AttackSceneId, AttackDir, deep, skillid, blackscreentime)
    {
        lpos = new List<Vector3>();
        m_JumpTime = 0.5f;//0.8f;
        m_MoveTime = 0.15f;
        m_CastTime = 0.7f;
        m_EventTime = m_JumpTime + m_CastTime;//1.5f;//2.333f;
        m_action2 = m_EventTime + 0.3f;
        m_action3 = m_action2 + 1f;//2.3f;//2.5f;//1.5f;
        m_Duration = m_action3 + 0.833f;//3.633f;//3.833f;//3.833f;
    }
    public override void ActiiveStart()
    {
        Life  w = m_LifePrent.m_Skill.m_SkillTarget ;
        if (w is Role) {
            //Transform EffectPos = (w as Role).m_Skin.ProPerty.m_EffectPos;
            GameObject posgo = (w as Role).m_Skin.ProPerty.HelpPoint.GetVauleByKey(HelpPointName.EffectPos);
            if (posgo != null) {
                Vector3 pos = posgo.transform.position;
                pos.z  = -1.5f;
                //SkillEffects._instance.LoadEffect("effect/prefab/", "1002051",pos,1f);
                GameObjectActionExcute gae1 = EffectM.LoadEffect(EffectM.sPath, "1002051", pos, BattleEnvironmentM.GetLifeMBornNode(true));
                GameObjectActionDelayDestory ndEffect1 = new GameObjectActionDelayDestory(1f);
                gae1.AddAction(ndEffect1);
            }
        }
        SoundPlay.Play("skill_voice_yidao", false, false);
        SoundPlay.Play("skill_yidao_sstx", false, false);
        
        Life emeny =  CM.GetAllLifeM(m_AttackSceneID, LifeMType.ALL);
        
        GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051041", BattleEnvironmentM.Local2WorldPos(m_TargetPos), BattleEnvironmentM.GetLifeMBornNode(true));
        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(m_action3);
        gae.AddAction(ndEffect);
        GameObject go = gae.gameObject;
        go.transform.parent = m_Skin.tRoot;
        go.transform.localPosition = Vector3.zero;
        //m_LifePrent.InBoat  =false;
        MapGrid gfrom = m_LifePrent.GetMapGrid();
        m_TelePortGrid = gfrom;
        MapGrid gstart = gfrom;
        //m_Dir = WalkDir.WALKRIGHT;
        MapGrid gto = emeny.GetTargetMapGrid();
        int startunit = MapSize.GetGridStart(gto.GridPos.Layer);
        int endtunit = startunit + MapSize.GetLayerSize(gto.GridPos.Layer) - 1;
        //if (gto.GridPos.Layer > 0)
        MapGrid start = MapGrid.GetMG(gto.GridPos.Layer, startunit);
        MapGrid end = MapGrid.GetMG(gto.GridPos.Layer, endtunit);
        MapGrid gl = gto.GetNextAttackStation(DIR.LEFT, false);
        MapGrid gr = gto.GetNextAttackStation(DIR.RIGHT, false);
        if (gl == null && gr == null) {
            Debug.LogError("xxxxxxxxxxxxxxxxXXXXXXXXXXXXXX " + gto.GridPos);
        }
        
        if (emeny is Role) {
            m_TargetPos = emeny.m_thisT.localPosition;
        } else {
            m_TargetPos = gto.pos;
        }
        if (gfrom.GridPos.Unit < startunit) {
            gstart = start;
        } else if (gfrom.GridPos.Unit > endtunit) {
            gstart = end;
        }
        if (gto.GridPos.Layer > 0) {
            int upstartunit = MapSize.GetGridStart(gto.GridPos.Layer - 1);
            int upendtunit = upstartunit + MapSize.GetLayerSize(gto.GridPos.Layer - 1) - 1;
            MapGrid upstart = MapGrid.GetMG(gto.GridPos.Layer - 1, upstartunit);
            MapGrid upend = MapGrid.GetMG(gto.GridPos.Layer - 1, upendtunit);
            
            if (gfrom.GridPos.Unit < upstartunit) {
                gstart = upstart;
            } else if (gfrom.GridPos.Unit > upendtunit) {
                gstart = upend;
            }
        }
        m_gs = (m_LifePrent as Role).CurrentGS;
        if (m_gs == GridSpace.Space_UP) {
            m_LifePrent.m_thisT.localScale = new Vector3(m_LifePrent.m_thisT.localScale.x, -m_LifePrent.m_thisT.localScale.y, m_LifePrent.m_thisT.localScale.z);
        }
        if (gr != null && gstart.GridPos.Unit > gr.GridPos.Unit) {
            m_Dir = WalkDir.WALKLEFT;
            m_PreTelePortGrid = gr;
            if (gl != null) {
                /*MapGrid temp = gl.GetNextAttackStation(DIR.LEFT,false);
                if ( temp!= null)
                	gl = temp;*/
                //w.Teleport(gl);
                m_TelePortGrid = gl;
                m_End = gl.pos;
                //m_Dir = WalkDir.WALKLEFT;
            } else {
                m_TelePortGrid = gr;
                //w.Teleport(gr);
                m_End = gr.pos;
            }
            MapGrid oldgrid = null;
            if (gstart.GridPos.Unit - gr.GridPos.Unit <= 3) {
                oldgrid = MapGrid.GetMG(gr.GridPos.Layer, gstart.GridPos.Unit);
                if (m_gs == GridSpace.Space_DOWN) {
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gr.Uppos;
                    upos.x	+= (oldgrid.Uppos.x - gr.Uppos.x) / 2;
                    lpos.Add(upos);
                    lpos.Add(gr.pos);
                } else {
                    lpos.Add(gfrom.Uppos);
                    lpos.Add(gr.pos);
                }
            } else {
                oldgrid = MapGrid.GetMG(gr.GridPos.Layer, gstart.GridPos.Unit);
                if (m_gs == GridSpace.Space_DOWN) {
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gr.Uppos;
                    upos.x	+= (oldgrid.Uppos.x - gr.Uppos.x) * 3 / 4;
                    lpos.Add(upos);
                    lpos.Add(gr.pos + new Vector3((oldgrid.pos.x - gr.pos.x) / 2, 0, 0));
                    lpos.Add(gr.Uppos + new Vector3((oldgrid.Uppos.x - gr.Uppos.x) / 4, 0, 0));
                    lpos.Add(gr.pos);
                } else {
                    lpos.Add(gfrom.Uppos);
                    
                    lpos.Add(gr.pos);
                }
            }
        } else if (gl != null && gstart.GridPos.Unit < gl.GridPos.Unit) {
            m_Dir = WalkDir.WALKRIGHT;
            m_PreTelePortGrid = gl;
            if (gr != null) {
                /*MapGrid temp = gr.GetNextAttackStation(DIR.RIGHT,false);
                if ( temp!= null)
                	gr = temp;*/
                //w.Teleport(gr);
                m_TelePortGrid = gr;
                m_End = gr.pos;
            } else {
                //w.Teleport(gl);
                m_TelePortGrid = gl;
                m_End = gl.pos;
            }
            MapGrid oldgrid = null;
            if (gl.GridPos.Unit - gstart.GridPos.Unit <= 3) {
                oldgrid = MapGrid.GetMG(gl.GridPos.Layer, gstart.GridPos.Unit);
                if (m_gs == GridSpace.Space_DOWN) {
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gl.Uppos;
                    upos.x	+= (oldgrid.Uppos.x - gl.Uppos.x) / 2;
                    lpos.Add(upos);
                    lpos.Add(gl.pos);
                } else {
                    lpos.Add(gfrom.Uppos);
                    lpos.Add(gl.pos);
                }
            } else {
                oldgrid = MapGrid.GetMG(gl.GridPos.Layer, gstart.GridPos.Unit);
                if (m_gs == GridSpace.Space_DOWN) {
                    lpos.Add(gfrom.pos);
                    Vector3 upos = oldgrid.Uppos;
                    upos.x	+= (gl.Uppos.x - oldgrid.Uppos.x) / 4;
                    lpos.Add(upos);
                    lpos.Add(oldgrid.pos + new Vector3((gl.pos.x - oldgrid.pos.x) / 2, 0, 0));
                    lpos.Add(oldgrid.Uppos + new Vector3((gl.Uppos.x - oldgrid.pos.x) * 3 / 4, 0, 0));
                    lpos.Add(gl.pos);
                } else {
                    lpos.Add(gfrom.Uppos);
                    
                    lpos.Add(gl.pos);
                }
            }
        } else {
            if (gl != null && gfrom.GridPos.Unit < gto.GridPos.Unit) {
                m_Dir = WalkDir.WALKRIGHT;
                m_Start = gl.pos;
                m_End = gl.pos;
                
                m_PreTelePortGrid = gl;
                if (gr != null) {
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gl.Uppos;
                    upos.x	+= (gr.Uppos.x - gl.Uppos.x) / 2;
                    lpos.Add(upos);
                    lpos.Add(gr.pos);
                    lpos.Add(upos);
                    lpos.Add(gl.pos);
                    //w.Teleport(gr);
                    
                    m_TelePortGrid = gr;
                    m_End = gr.pos;
                } else {
                    m_TelePortGrid = gl;
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gl.Uppos;
                    lpos.Add(upos);
                    lpos.Add(gl.pos);
                    lpos.Add(upos);
                    lpos.Add(gl.pos);
                }
            } else if (gr != null && gfrom.GridPos.Unit > gto.GridPos.Unit) {
            
                m_PreTelePortGrid = gr;
                m_Dir = WalkDir.WALKLEFT;
                m_Start = gr.pos;
                m_End = gr.pos;
                if (gl != null) {
                
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gl.Uppos;
                    upos.x	+= (gr.Uppos.x - gl.Uppos.x) / 2;
                    lpos.Add(upos);
                    lpos.Add(gl.pos);
                    lpos.Add(upos);
                    lpos.Add(gr.pos);
                    //w.Teleport(gl);
                    m_TelePortGrid = gl;
                    m_End = gl.pos;
                } else {
                    m_TelePortGrid = gr;
                    lpos.Add(gfrom.pos);
                    Vector3 upos = gr.Uppos;
                    lpos.Add(upos);
                    lpos.Add(gr.pos);
                    lpos.Add(upos);
                    lpos.Add(gr.pos);
                }
            } else {
            
                m_PreTelePortGrid = gto;
                m_Start = gto.pos;
                m_End = gto.pos;
                //Debug.Log("左右都没有攻击位" + gl.GridPos + "," + gr.GridPos + "," +gfrom.GridPos + "," + gto.GridPos.Unit);
            }
        }
        //PlayAction(AnimatorState.Skill01,m_Start,true);
#if UNITY_EDITOR_LOG
        string str = "lujin:";
        for (int i = 0; i < lpos.Count; i++) {
            str += lpos[i] + ", ";
        }
        //Debug.Log(str + ", mend " +m_End);
#endif
        if (lpos.Count > 0) {
        
            m_Start = lpos[lpos.Count - 1];
            m_oncejumptime = m_JumpTime / (lpos.Count - 1);
            m_AniSpeed = 0.8f / m_oncejumptime;
            m_gs = (m_LifePrent as Role).CurrentGS;
            m_JumpCount = 0;
        }
        
#if UNITY_EDITOR_LOG
        FileLog.write(m_LifePrent.SceneID, "能量燃烧 " + m_Start + "," + m_End + "," + m_TelePortGrid.GridPos + "," + m_Dir + "," + gfrom.GridPos + "," + gto.GridPos);
#endif
    }
    
    public override void DoEvent()
    {
    
    }
    public override void UpdatePos()
    {
        if (m_TimeCount < m_CastTime) {
            PlayAction(AnimatorState.PreSkill01, m_Skin.tRoot.localPosition);
        } else if (m_TimeCount < m_EventTime) { //m_action3 )
            //if ((m_TimeCount-m_Delatime) < m_CastTime)
            (m_Skin as RoleSkin).SetVisable(false);
            //m_bodyobj.SetActive(false);
            
            if (lpos.Count > 0) {
                float timecout = m_TimeCount - m_CastTime;
                if (timecout >= m_oncejumptime * m_JumpCount) {
                    if (timecout >= lpos.Count - 1) {
                        timecout = lpos.Count - 2;
                    }
                    Vector3 pos = Vector3.Lerp(lpos[m_JumpCount], lpos[m_JumpCount + 1], (timecout - m_oncejumptime * m_JumpCount) / m_oncejumptime);
                    if (m_gs == GridSpace.Space_DOWN) {
                        PlayAction(AnimatorState.JumpUp, pos, true);
                    } else {
                        PlayAction(AnimatorState.JumpDown, pos, true);
                    }
                    if (timecout >= m_oncejumptime * (m_JumpCount + 1)) {
                        m_AniSpeed = 0.8f / m_oncejumptime;
                        if (m_gs == GridSpace.Space_DOWN) {
                            m_gs = GridSpace.Space_UP;
                        } else {
                            m_gs = GridSpace.Space_DOWN;
                        }
                        GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051051", BattleEnvironmentM.Local2WorldPos(lpos[m_JumpCount + 1]), BattleEnvironmentM.GetLifeMBornNode(true));
                        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
                        gae.AddAction(ndEffect);
                        
                        m_JumpCount ++;
                    }
                }
            }
            
        } else if (m_TimeCount < m_action3) {
        
            if (m_TimeCount - m_Delatime < m_action2 && m_TimeCount > m_action2) {
                (m_LifePrent as Role).RoleWalk.Teleport(m_PreTelePortGrid, false);
                m_LifePrent.m_thisT.localScale = new Vector3(m_LifePrent.m_thisT.localScale.x, Mathf.Abs(m_LifePrent.m_thisT.localScale.y), m_LifePrent.m_thisT.localScale.z);
                Vector3 effectpos = m_TargetPos;
                effectpos.y += 0.8f;
                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051111", BattleEnvironmentM.Local2WorldPos(effectpos), BattleEnvironmentM.GetLifeMBornNode(true));
                GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.75f);
                gae.AddAction(ndEffect);
            }
        } else if (m_TimeCount < m_Duration) {
            (m_Skin as RoleSkin).SetVisable(true);
            
            (m_Skin as RoleSkin).StealthMode(false);
            m_AniSpeed = 1f;
            float cout = m_TimeCount - m_action3 ;
            float t1 = 0f;
            float duration = m_MoveTime / 2;
            Vector3 effectpos = m_TargetPos;
            effectpos.y += 0.8f;
            Vector3 pos = m_Start;
            if (cout >= t1 && (cout - m_Delatime) < t1) {
                GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "1051121", BattleEnvironmentM.Local2WorldPos(effectpos), BattleEnvironmentM.GetLifeMBornNode(true));
                GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
                gae.AddAction(ndEffect);
                
            }
            
            if (cout < m_MoveTime) {
                if (cout < duration) {
                    pos = Vector3.Lerp(m_Start, m_TargetPos, (cout) / duration);
                } else {
                    pos = Vector3.Lerp(m_TargetPos, m_End, (cout - duration) / duration);
                }
                
            } else if (cout >= m_MoveTime) {
                if (cout - m_Delatime < m_MoveTime) {
                    (m_LifePrent as Role).RoleWalk.Teleport(m_TelePortGrid, false);
                }
                pos = m_End;
            }
            PlayAction(AnimatorState.Skill01, pos, true);
            
        }
    }
    public override void Finish()
    {
        base.Finish();
        m_LifePrent.m_thisT.localScale = new Vector3(m_LifePrent.m_thisT.localScale.x, Mathf.Abs(m_LifePrent.m_thisT.localScale.y), m_LifePrent.m_thisT.localScale.z);
        (m_Skin as RoleSkin).SetVisable(true);
        m_LifePrent.InBoat = true;
        /*if(IsPlayed())
        {
        	Debug.Log ( "m_TelePortGrid" + m_TelePortGrid.GridPos);
        	(m_LifePrent as Role).RoleWalk.Teleport(m_TelePortGrid,false);
        }*/
    }
}