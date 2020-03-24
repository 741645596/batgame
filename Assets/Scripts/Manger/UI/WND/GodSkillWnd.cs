using UnityEngine;
using System.Collections.Generic;
public enum CaptionExpress {
    none = 0,
    idle = 1,
    fire = 2,
    playerdead = 3,
    enemydead = 4,
    release = 5,
    win = 6,
    faile = 7,
    click = 8,
    nomana = 9,
}
public enum GodSkillState {
    Idle, //平常闲置状态
    Click,//点击状态
    Select, //选中对象状态
}
public class GodSkillWnd : WndBase
{

    public GodSkillWnd_h MyHead {
        get
        {
            return (base.BaseHead() as GodSkillWnd_h);
        }
    }
    
    private bool m_MouseDown;
    private float m_MouseDownTime;
    private MapGrid m_MouseDownGrid;
    private Vector3 m_MouseDownWorldPos;
    private bool m_taskguide = false;
    
    public Vector3 LastPostion;
    private List<Transform> m_Path = new List<Transform>();
    float m_biaoqingduration;
    Bezier myBezier;
    bool iseffecttime = false;
    GodSkillState m_currentstate;
    CaptionExpress m_CurrExpress = CaptionExpress.none;
    CaptionInfo m_captioninfo = null;
    
    private bool m_ReleaseSkill = false;
    private bool m_ReadyReleaseSkill = false;
    
    public override void WndStart()
    {
        base.WndStart();
        MyHead.BtnRelease.gameObject.SetActive(true);
        ChangeBiaoqing((int)CaptionExpress.idle, true);
        m_currentstate = GodSkillState.Idle;
        if (MyHead.BtnRelease) {
            MyHead.BtnRelease.OnPressDownEventHandler += BtnRelease_OnClickEventHandler;
        }
        
        m_captioninfo = BlackScienceDC.GetCaption(CmCarbon.GetCaptainID(true));
        MyHead.lblLevel.text = m_captioninfo.m_level1.ToString();
        NGUIUtil.Set2DSprite(MyHead.m_BiaoQingParent, "Textures/role/", m_captioninfo.m_captionid.ToString());
        
        UISprite[] starSprites = U3DUtil.GetComponentsInChildren<UISprite>(MyHead.StarListParent);
        
        //NGUIUtil.SetStarLevelNum (starSprites,m_captioninfo.m_star);
        for (int i = 0; i < starSprites.Length; i++) {
            if (i >= m_captioninfo.m_star) {
                starSprites[i].gameObject.SetActive(false);
            }
        }
        UIGrid grid = MyHead.StarListParent.GetComponent<UIGrid>();
        grid.Reposition();
        if (m_captioninfo.GetGodSkillInfo().m_power2 > 0) {
            MyHead.LblDestroyPts.text = m_captioninfo.GetGodSkillInfo().m_power2.ToString();
        } else {
            MyHead.LblDestroyPts.transform.parent.gameObject.SetActive(false);
        }
    }
    void Update()
    {
        if (CombatScheduler.State == CSState.End) {
            //MyHead.m_fulleffect.SetActive(false);
            //MyHead.m_notfulleffect.SetActive(true);
            MyHead.Fire.SetActive(false);
            return;
        }
        
        if ((m_currentstate == GodSkillState.Click) && (Input.GetMouseButton(0) || Input.touchCount == 1)) {
            RaycastHit hit;
            if ((!WndManager.IsHitNGUI(out hit) || m_taskguide) && !m_MouseDown) {
                Vector3 ScreenPos = Vector3.zero;
                if (SystemInfo.deviceType == DeviceType.Desktop && Input.GetMouseButton(0)) {
                    ScreenPos = Input.mousePosition;
                } else if (Input.touchCount == 1) {
                    ScreenPos = Input.GetTouch(0).position;
                }
                Vector3 vGestyrScreenPlane = Camera.main.WorldToScreenPoint(Vector3.zero);
                Vector3 posWorld =  Camera.main.ScreenToWorldPoint(new Vector3(ScreenPos.x, ScreenPos.y, vGestyrScreenPlane.z));
                Transform start = BattleEnvironmentM.GetLifeMBornNode(true);
                Vector3 locapos = start.InverseTransformPoint(posWorld);
                int layer = (int)(locapos.y / MapGrid.m_heigth);
                int unit = (int)(locapos.x / MapGrid.m_width);
                //Debug.Log("calc  layer:"+ layer + "  unit:" + unit);
                MapGrid g = MapGrid.GetMG(layer, unit);
                if (g != null) {
                    //ReleaseSkill(g,posWorld);
                    m_MouseDownGrid = g;
                    m_MouseDownWorldPos = posWorld;
                    m_MouseDown = true;
                    m_MouseDownTime = Time.time;
                    m_ReleaseSkill = false;
                    m_ReadyReleaseSkill = true;
                    m_taskguide = false;
                } else {
                
                    NGUIUtil.ShowTipWndByKey("88800015", 1.0f);
                    if (m_taskguide == false) {
                        CancelSelect();
                    }
                }
            }
            
        } else if (m_MouseDown) {
            m_MouseDown = false;
            if ((Time.time - m_MouseDownTime) < 0.5f) {
                ReleaseSkill(m_MouseDownGrid, m_MouseDownWorldPos);
            }
        }
        
    }
    
    public bool CheckMana()
    {
        if (int.Parse(MyHead.CurMana.text) >= int.Parse(MyHead.RequireMana.text)) {
            //BtnRelease.gameObject.SetActive(true);
            //MyHead.m_fulleffect.SetActive(true);
            //MyHead.m_notfulleffect.SetActive(false);
            MyHead.Fire.SetActive(true);
            MyHead.CurMana.color = Color.green;
            MyHead.RequireMana.color = Color.green;
            return true;
        } else {
            //MyHead.m_fulleffect.SetActive(false);
            //MyHead.m_notfulleffect.SetActive(true);
            //BtnRelease.gameObject.SetActive(false);
            MyHead.CurMana.color = Color.white;
            MyHead.RequireMana.color = Color.white;
            MyHead.Fire.SetActive(false);
        }
        return false;
    }
    public void SetCurMana(int v)
    {
        MyHead.CurMana.text = v.ToString();
        CheckMana();
    }
    public void SetRequireMana(int v)
    {
        MyHead.RequireMana.text = v.ToString();
        CheckMana();
    }
    public void DoClick()
    {
    
    }
    public void UnDoClick()
    {
    
        m_MouseDown = false;
    }
    public void BtnRelease_OnClickEventHandler(UIButton sender)
    {
    
        if (CombatScheduler.State == CSState.End) {
            return;
        }
        /*if (!CheckMana())
        {
        	ChangeBiaoqing((int)CaptionExpress.nomana);
        	return;
        }*/
        if (m_currentstate == GodSkillState.Click) {
            m_currentstate = GodSkillState.Idle;
            Building.ShowAllHp(false);
            ChangeBiaoqing((int)CaptionExpress.idle, true);
            return ;
        }
        m_currentstate = GodSkillState.Click;
        Building.ShowAllHp(true);
        ChangeBiaoqing((int)CaptionExpress.click, true);
        DoClick();
#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
        if (Input.touchCount == 1) {
            LastPostion = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
        }
#endif
        m_MouseDown = true;
        //gunsight.SetActive(true);
        CombatWnd cw = WndManager.FindDialog<CombatWnd>();
        if (cw != null) {
            cw.CancelAllFire();
            GodSkill godSkill = CmCarbon.GetGodSkill(true);
            if (godSkill.m_godskill.m_type == 9001) {
                cw.Show9001(true);
            }
        }
    }
    /// <summary>
    /// 搜索格子最近的己方目标
    /// </summary>
    /// <param name="g"></param>
    Life SearchSelfNearestTarget(MapGrid g, LifeMCamp camp, float threshold)
    {
        List<Life> listSelfLife = new List<Life>();
        CM.SearchLifeMListByGrid(ref listSelfLife, null, LifeMType.SOLDIER, camp, MapSearchStlye.SameLayer, g, 0);
        Life result = listSelfLife.Find(life => (Mathf.Abs(life.GetLifeProp().transform.position.x - g.WorldPos.x) <= threshold));
        return result;
    }
    
    public void SelectRole(int sceneid)
    {
    
        CombatWnd cw = WndManager.FindDialog<CombatWnd>();
        if (cw != null) {
            cw.Show9001(false);
        }
        Life target = CM.GetLifeM(sceneid, LifeMType.SOLDIER);
        ReleaseSkill(target.GetMapGrid(), target.GetMapGrid().WorldPos, target);
        
        CombatWnd wnd = WndManager.FindDialog<CombatWnd>();
        if (wnd) {
            wnd.ShowSkill9001Effect(sceneid);
        }
    }
    public void ReleaseSkill(MapGrid g, Vector3 pos, Life target = null)
    {
        if (!CheckMana()) {
            CancelSelect();
            return;
        }
        //Building.ShowAllHp(false);
        GodSkill godSkill = CmCarbon.GetGodSkill(true);
        int selectTarget = godSkill.m_godskill.m_selectTarget;
        List<Life> listSelfLife = new List<Life>();
        switch (selectTarget) {
            case 0://不需要选择目标
                //NGUIUtil.DebugLog("不需要选择目标");
                break;
            case 1://需要己方单位目标
                //NGUIUtil.DebugLog("需要己方单位目标");
                if (target != null)
                
                {
                    listSelfLife.Add(target);
                } else {
                    Life life = SearchSelfNearestTarget(g, godSkill.Camp, 1f);
                    listSelfLife.Add(life);
                    if (life == null) {
                        //NGUIUtil.ShowTipWnd(string.Format("{0} 需要己方单位目标！",godSkill.m_godskill.m_name),1.0f);
                        return;
                    } else {
                        //NGUIUtil.ShowTipWnd(string.Format("{0} 己方单位目标！", life.transform.name), 1.0f);
                    }
                }
                
                break;
            case 2://需要敌方单位目标
                //NGUIUtil.DebugLog("需要敌方单位目标");
                break;
                
            default:
                break;
        }
        m_ReleaseSkill = true;
        m_ReadyReleaseSkill = false;
        Vector3 vpos = MyHead.Vaule.transform.position;
        vpos.z -= 1;
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000581", vpos, MyHead.Vaule.transform.parent);
        iseffecttime = true;
        if (gae != null) {
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1f);
            gae.gameObject.AddComponent<SetRenderQueue>();
            ndEffect.m_complete = EffectFinish;
            gae.AddAction(ndEffect);
            
        }
        
        if (CSState.Start == CombatScheduler.State) {
            CombatScheduler.SetCSState(CSState.Combat);
        }
        godSkill.ListSkillTarget = listSelfLife;
        godSkill.ReleaseGodSkill(g, pos);
        
        int require = int.Parse(MyHead.RequireMana.text);
        SetRequireMana(godSkill.GetRequireMana());
        CmCarbon.SubGodSkillMana(true, require) ;
        SetCurMana(CmCarbon.GetGodSkillMana(true));
        UnDoClick();
        
        CancelSelect();
        //Fire.SetActive(false);
        
    }
    public void EffectFinish(object o)
    {
        iseffecttime = false;
        CheckMana();
    }
    //取消船长技能
    public void CancelSelect()
    {
        if (m_currentstate == GodSkillState.Click) {
            UnDoClick();
            m_currentstate = GodSkillState.Idle;
            Building.ShowAllHp(false);
            ChangeBiaoqing((int)CaptionExpress.idle, true);
            
            CombatWnd cw = WndManager.FindDialog<CombatWnd>();
            if (cw != null) {
                GodSkill godSkill = CmCarbon.GetGodSkill(true);
                if (godSkill.m_godskill.m_type == 9001) {
                    cw.Show9001(false);
                }
            }
        }
        CheckMana();
    }
    public void ChangeBiaoqing(int type, bool isloop = false)
    {
        //GodSkill godSkill = CmCarbon.GetGodSkill(true);
        
        if (type == (int)CaptionExpress.click) {
            MyHead.SelectEffect.SetActive(true);
        } else {
            MyHead.SelectEffect.SetActive(false);
        }
        
    }
    
    
}
