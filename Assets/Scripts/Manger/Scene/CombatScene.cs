using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using sdata;

/// <summary>
/// 战斗场景
/// </summary>
/// <author>zhulin</author>
public class CombatScene : IScene
{
    public new static string GetSceneName()
    {
        return "Combat";
    }
    //private AsyncOperation async;
    /// <summary>
    /// 资源载入入口
    /// </summary>
    public override IEnumerator Load()
    {
        async = Application.LoadLevelAsync(CombatScene.GetSceneName());
        return null;
        //yield return async;
    }
    
    /// <summary>
    /// 准备载入场景
    /// </summary>
    public override void PrepareLoad()
    {
    }
    
    /// <summary>
    /// 资源卸载
    /// </summary>
    public override void Clear()
    {
        SoundPlay.PlayBackGroundSound("bgm_city_loop", true, false);
        MapM.ClearMap();
        CM.ExitCm();
        ShipBombRule.ClearBombData();
        WndManager.DestroyAllDialogs();
    }
    
    /// <summary>
    /// 是否已经载入完成
    /// </summary>
    private int okTime = 0;
    public override bool IsEnd()
    {
        if (async != null) {
            return async.isDone;
        } else {
            return false;
        }
    }
    
    public UserInfo m_oldUserInfo = new UserInfo();
    
    
    public override void BuildUI()
    {
        Life.Environment = LifeEnvironment.Combat;
        CombatScheduler.SetCSState(CSState.Ready);
        
        if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE) {
            if (StageDC.GetPveMode() == PVEMode.Attack) {
                WndManager.GetDialog<SelectSoldierwnd>();
                BattleEnvironmentM.BuildScene();
            } else {
                BattleEnvironmentM.BuildScene();
                WndManager.GetDialog<CombatWnd>();
                CombatInfoWnd wnd = WndManager.GetDialog<CombatInfoWnd>();
                if (wnd != null) {
                    wnd.SetWndMode(CombatInfoMode.combat);
                }
                if (CmCarbon.GetGodSkill(StageDC.GetPveMode() == PVEMode.Attack) != null) {
                    //船长技能设置
                    GodSkillWnd gsw = WndManager.GetDialog<GodSkillWnd>();
                    if (gsw != null) {
                        gsw.SetCurMana(CmCarbon.GetGodSkillMana(true));
                        gsw.SetRequireMana(CmCarbon.GetGodSkill(true).GetRequireMana());
                    }
                }
                //
            }
        }  else {
            WndManager.GetDialog<CombatWnd>();
            WndManager.GetDialog<CombatCountDownWnd>();
            CombatInfoWnd wnd = WndManager.GetDialog<CombatInfoWnd>();
            if (wnd != null) {
                wnd.SetWndMode(CombatInfoMode.view);
            }
            BattleEnvironmentM.BuildScene();
            
        }
        BattleEnvironmentM.AddFireSoldierCompent();
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;//prevent  phone from going to sleep
        m_oldUserInfo.Level = UserDC.GetLevel();
        m_oldUserInfo.Physical = UserDC.GetPhysical();
    }
    public override void BuildWorld()
    {
    
    }
    
    
    /// <summary>
    /// 场景start 接口
    /// </summary>
    public override void Start()
    {
    
    }
    /// <summary>
    /// 接管场景中关注对象的Update
    /// </summary>
    public override void Update(float deltaTime)
    {
        if (CombatScheduler.CheckCombatIng() == true) {
            MouseDownInTheScene();
            List<SceneObj> l = GetAllSceneObj();
            for (int i = 0 ; i < l.Count ; i ++) {
                if (l[i] != null) {
                    l[i].NDUpdate(deltaTime);
                }
            }
        }
    }
    
    
    /// <summary>
    /// 接管场景中关注对象的LateUpdate
    /// </summary>
    public override void LateUpdate(float deltaTime)
    {
        if (CombatScheduler.CheckCombatIng() == true) {
            List<SceneObj> l = GetAllSceneObj();
            for (int i = 0 ; i < l.Count ; i ++) {
                if (l[i] != null) {
                    l[i].NDLateUpdate(deltaTime);
                }
            }
        }
        
        CombatScheduler.Run(deltaTime);
    }
    
    /// <summary>
    /// 接管场景中关注对象的FixedUpdate
    /// </summary>
    public override void FixedUpdate(float deltaTime)
    {
        if (CombatScheduler.CheckCombatIng() == true) {
            MouseDownInTheScene();
            List<SceneObj> l = GetAllSceneObj();
            for (int i = 0 ; i < l.Count ; i ++) {
                if (l[i] != null) {
                    l[i].NDFixedUpdate(deltaTime);
                }
            }
        }
    }
    
    
    
    
    /// <summary>
    /// 点击场景角色或物件
    /// </summary>
    void MouseDownInTheScene()
    {
        //释放怒气
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Role"))) {
                Role w = hit.transform.GetComponentInParent<LifeProperty>().GetLife() as Role;
                if (w != null && w.m_Core.m_IsPlayer == true) {
                    if (w.Anger >= ConfigM.GetAngerK(1)) {
                        w.ReleaseSkillEffect();
                    }
                }
            }
            ray = EffectCamera.camera.ScreenPointToRay(Input.mousePosition);
            //拾取资源
            if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Resource"))) {
                GameObjectActionExcute gae = hit.transform.GetComponentInParent<GameObjectActionExcute>();
                
                if (gae != null) {
                    GameObjectAction ga = gae.GetCurrentAction();
                    if (ga is GameObjectActionWait || ga is GameObjectActionBoxWait) {
                        gae.GoNextAction();
                    }
                }
            }
        }
    }
    
    
    
    /// <summary>
    /// 服务端通知战斗结果信息
    /// </summary>
    void ShowCombatEnd(int nErrorCode)
    {
        CombatInfoWnd wndCombatInfo = WndManager.GetDialog<CombatInfoWnd>();
        wndCombatInfo.HidePauseButton();
        bool win = false;
        CheckLevelUp(0);
        
        
        win = StageDC.GetStageResult().win;
        PlayCombatEndAnimation(win);
        BSC.AntiAllRegisterHooks();
        
        ShipBomb(win);
    }
    
    void PlayCombatEndAnimation(bool isWin)
    {
        List<Life> list = new List<Life>();
        CM.SearchLifeMListInBoat(ref list, LifeMType.SOLDIER);
        GodSkillWnd gsw = WndManager.FindDialog<GodSkillWnd>();
        if (gsw != null) {
            if (isWin) {
                gsw.ChangeBiaoqing((int)CaptionExpress.win);
            } else {
                gsw.ChangeBiaoqing((int)CaptionExpress.faile);
            }
        }
        foreach (var lifeM in list) {
            lifeM.GameOver(lifeM.m_Core.m_IsPlayer ? isWin : !isWin);
        }
        
    }
    
    
    void CheckLevelUp(int nErrorCode)
    {
        if (nErrorCode == 0) {
            //			ShowCaptionUpWnd();
        }
        
    }
    
    void ShipBomb(bool win)
    {
        bool isDelayPlaySound = false;
        Transform t	= BattleEnvironmentM.GetLifeMBornNode(true);
        
        if (CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE) == false && win == true) {
            ShipBombAction action = t.gameObject.AddComponent<ShipBombAction>();
            action.SetFinishFun(ShowEndTalk, win);
            isDelayPlaySound = true;
        } else if (CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE) == true && win == false) {
            ShipBombAction action = t.gameObject.AddComponent<ShipBombAction>();
            action.SetFinishFun(ShowEndTalk, win);
            isDelayPlaySound = true;
        } else if (CmCarbon.GetCamp2Player(LifeMCamp.DEFENSE) && win) {
            DefenseEndAction action = t.gameObject.AddComponent<DefenseEndAction>();
            action.SetFinishFun(ShowEndTalk, win);
            isDelayPlaySound = true;
        } else {
            ShowEndTalk(win);
        }
        
        if (win) {
            SoundPlay.Play("battle_win", false, false, isDelayPlaySound ? 5f : 0f);
        } else {
            SoundPlay.Play("battle_lose", false, false, isDelayPlaySound ? 5f : 0f);
        }
    }
    
    void ShowEndTalk(bool win)
    {
        List<CounterPartDialogUnit> lEndTalk = CmCarbon.GetEndTalk();
        
        if (win == false || lEndTalk == null || lEndTalk.Count == 0) {
            //ShowCombatResult();
            ShowDesignAction();
        } else {
            //聊天内容
            DoStartTalks(lEndTalk, ShowDesignAction);
        }
    }
    
    /// <summary>
    /// 战后NPC对话
    /// </summary>
    private void DoStartTalks(List<CounterPartDialogUnit> ltalk, CallBack callBack)
    {
        List<int> lNpcID = new List<int>();
        List<string> lStrTalks = new List<string>();
        List<NpcDirection> lWndDirs = new List<NpcDirection>();
        foreach (CounterPartDialogUnit t in ltalk) {
            lNpcID.Add(t.npcid);
            lStrTalks.Add(t.talk);
            lWndDirs.Add((NpcDirection)t.position);
        }
        NpcTalksWnd wnd = WndManager.GetDialog<NpcTalksWnd>();
        wnd.SetData(lNpcID, lStrTalks, lWndDirs, callBack);
    }
    /// <summary>
    /// 显示船只设计图
    /// </summary>
    public void ShowDesignAction()
    {
        treasure.TreasureRobSettleResponse response = null;
        if (response != null) {
            int sShipCanvasID = response.shipdrawingid;
            StaticShipCanvas sInfo = ShipPlanM.GetShipCanvasInfo(sShipCanvasID);
            if (sInfo != null) {
                sdata.s_itemtypeInfo itypeInfo = new sdata.s_itemtypeInfo();
                itypeInfo.gid = sShipCanvasID;
                itypeInfo.name = sInfo.Name;
                itypeInfo.gtype = 5;
                List<sdata.s_itemtypeInfo> lInfo = new List<sdata.s_itemtypeInfo>();
                lInfo.Add(itypeInfo);
                
                TrophiesActionWnd TropWnd = WndManager.GetDialog<TrophiesActionWnd>();
                if (TropWnd) {
                    TropWnd.ClearTropiesData();
                    TropWnd.AddTropiesData(lInfo);
                    TropWnd.SetWndType(5);
                    TropWnd.MyHead.LblDes.gameObject.SetActive(false);
                    TropWnd.BtnOKClickHandler += CheckShowCombatResult;
                }
            } else {
                CheckShowCombatResult();
            }
        } else {
            CheckShowCombatResult();
        }
    }
    
    void CheckShowCombatResult()
    {
        CounterPartInfo info = StageDC.GetCounterPartInfo();
        int Times = StageDC.GetPassStageTimes(StageDC.GetCompaignStageType(), StageDC.GetCompaignStageID());
        if (Times == 0 && info.drop != null && info.drop.Length > 0) {
            StageDC.SendStageScriptDropRequest(StageDC.GetCompaignStageID());
            DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_716, ShowScriptCombatResult);
        } else {
            ShowCombatResult();
        }
    }
    
    void ShowScriptCombatResult(int nErrorCode)
    {
        CaptionUpgradeWnd cuw = WndManager.FindDialog<CaptionUpgradeWnd>();
        if (cuw != null) {
            WndManager.DestoryDialog<CaptionUpgradeWnd>();
        }
        WndManager.ShowAllWnds(false);
        StageResultWnd wnd = WndManager.GetDialog<StageResultWnd>();
        BattleEnvironmentMode battleMode = BattleEnvironmentM.GetBattleEnvironmentMode();
        if (wnd != null) {
            wnd.SetStageResult(StageDC.GetStageResult());
            List<StageDC.ScriptDropItem> items = StageDC.GetStageScriptDrops();
            foreach (StageDC.ScriptDropItem item in items) {
                wnd.SetScriptDropItem(item.mType, item.mID, item.mCount);
            }
        }
    }
    
    void ShowCombatResult()
    {
        CaptionUpgradeWnd cuw = WndManager.FindDialog<CaptionUpgradeWnd>();
        if (cuw != null) {
            WndManager.DestoryDialog<CaptionUpgradeWnd>();
        }
        WndManager.ShowAllWnds(false);
        StageResultWnd wnd = WndManager.GetDialog<StageResultWnd>();
        BattleEnvironmentMode battleMode = BattleEnvironmentM.GetBattleEnvironmentMode();
        if (wnd != null) {
            wnd.SetStageResult(StageDC.GetStageResult());
        }
    }
    
    /// <summary>
    /// 战斗结束操作
    /// </summary>
    public void DoCombatFinish()
    {
        CombatWnd combatWnd = WndManager.FindDialog<CombatWnd>();
        if (combatWnd) {
            combatWnd.ClearUIEffect();
        }
        ShowCombatEnd(0);
    }
    
}
