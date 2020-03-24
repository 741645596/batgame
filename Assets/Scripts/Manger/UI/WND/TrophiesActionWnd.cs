using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;
using System;
using DG.Tweening;

public class TrophiesActionWnd : WndTopBase
{

    public TrophiesActionWnd_h MyHead {
        get
        {
            return (base.BaseHead() as TrophiesActionWnd_h);
        }
    }
    public delegate void TrophiesShowFinish(int mode);
    
    public class ItemInfo
    {
        public int mGtype;
        public string mName;
        public int mGid;
        public int mCount;
        public bool mIsSoul = false;
        public bool mIsBook = false;
        public bool mNewVersion = false;	//为了兼容旧版的碎片判断方式（m_isSoulPis），先加个这个字段。
    }
    private List<ItemInfo> mItemInfos = new List<ItemInfo>();
    private List<GameObject> m_effect = new List<GameObject>();
    private GameObject m_Tropies = null;
    private TrophiesShowFinish m_Finishfun;
    private bool m_bFinishAni = false;
    private int m_mode = 0;
    /// <summary>
    /// 是否是酒馆抽到英雄/黑科技碎片.
    /// </summary>
    private bool m_isSoulPis = false;
    private int m_SoulPisNum = 0;
    /// <summary>
    /// 窗口类型 1（闪亮登场） 2（酒馆）3（炮弹兵未召唤）
    /// </summary>
    private int m_iWndType = 1;
    /// <summary>
    /// 是否执行左移动画
    /// </summary>
    private bool m_bNotTween = false;
    private Vector3 m_v3Ani1;
    private Vector3 m_v3Ani2;
    private bool m_IsTaskGuide = false;
    private int  m_taskid;
    private int  m_step ;
    
    // 最后一次点击确定按钮的响应事件
    public delegate void FinalClikHandler(UIButton sender);
    public event FinalClikHandler FinalEventClikHandler;
    
    public event CallBack BtnOKClickHandler;
    
    private GameObject GoSoldier = null;
    /// <summary>
    /// 设置窗口类型 1（闪亮登场） 2（酒馆/新手引导）3（炮弹兵未召唤） 4(陷阱未召唤) 5：设计图
    /// </summary>
    public void SetWndType(int type, bool isSoulPis = false, int ItemNum = 0)
    {
        m_iWndType = type;
        m_isSoulPis = isSoulPis;
        m_SoulPisNum = ItemNum;
        
        if (m_iWndType == 2 || m_iWndType == 3 || m_iWndType == 4 || m_iWndType == 5) {
            MyHead.BtnOK.OnClickEventHandler += BtnOK_OnClickEventHandler;
            GetTropies(0, null);
        }
        if (MyHead.LblDes != null) {
            MyHead.LblDes.gameObject.SetActive(m_isSoulPis);
        }
    }
    
    void BtnOK_FinalClickEventHandler(UIButton sender)
    {
        if (FinalEventClikHandler != null) {
            FinalEventClikHandler(null);
        }
        if (BtnOKClickHandler != null) {
            BtnOKClickHandler();
        }
    }
    /// <summary>
    /// 设置新手引导数据
    /// </summary>
    public void SetTaskGuideData(int taskid, int step)
    {
        m_IsTaskGuide = true;
        m_taskid = taskid;
        m_step = step;
    }
    
    /// <summary>
    /// 新手引导可用的简化接口
    /// </summary>
    public void SetTropiesData(int soldierTypeID)
    {
        s_itemtypeInfo itypeInfo = new s_itemtypeInfo();
        s_soldier_typeInfo soldierInfo = SoldierM.GetSoldierType(soldierTypeID);
        List<sdata.s_itemtypeInfo> l = new List<s_itemtypeInfo>();
        if (soldierInfo != null) {
            itypeInfo.name = soldierInfo.name;
            itypeInfo.gtype = 1;
            itypeInfo.gid = soldierTypeID;
        }
        if (itypeInfo != null) {
            l.Add(itypeInfo);
            AddTropiesData(l);
        }
    }
    
    public void AddTropiesData(List<sdata.s_itemtypeInfo> l)
    {
        foreach (sdata.s_itemtypeInfo itemData in l) {
            AddTropiesData(itemData.name, itemData.gtype, itemData.gid);
        }
    }
    
    public void ClearTropiesData()
    {
        mItemInfos.Clear();
    }
    
    public void AddTropiesData(string itemName, int itemGtype, int itemGid, bool isSoul = false, bool isBook = false, bool newVersion = false, int count = 0)
    {
        ItemInfo item = new ItemInfo();
        item.mName = itemName;
        item.mGtype = itemGtype;
        item.mGid = itemGid;
        item.mCount = count;
        item.mIsBook = isBook;
        item.mIsSoul = isSoul;
        item.mNewVersion = newVersion;
        mItemInfos.Add(item);
    }
    
    public void GetTropies(int mode, TrophiesShowFinish f)
    {
        m_mode = mode;
        m_Finishfun = f;
        if (mItemInfos.Count > 0) {
            Step1(null);
        } else {
            DoFinish();
        }
    }
    
    public override void WndStart()
    {
        base.WndStart();
        m_v3Ani1 = MyHead.m_lblTroptype.transform.parent.position;
        m_v3Ani2 = MyHead.MoveParent.transform.localPosition;
        if (m_iWndType == 2 || m_iWndType == 3) {
            MyHead.BtnOK.OnClickEventHandler += BtnOK_OnClickEventHandler;
            //            GetTropies(0, null);
        }
        if (MyHead.LblDes != null) {
            MyHead.LblDes.gameObject.SetActive(m_isSoulPis);
        }
        NGUIUtil.SetActive(MyHead.SkillInfo, false);
    }
    
    public void GetNextReward()
    {
        if (m_bFinishAni == true) {
            if (mItemInfos.Count > 0) {
                Step1(null);
            } else {
                DoFinish();
            }
        }
    }
    
    private void SetTrophies(int type, string Name)
    {
        if (MyHead.m_lblTroptype != null) {
            string text = NGUIUtil.GetStringByKey(99400000 + type);
            NGUIUtil.SetLableText(MyHead.m_lblTroptype, text);
        }
        
        if (MyHead.m_TropName != null) {
            MyHead.m_TropName.text = Name;
        }
    }
    /// <summary>
    /// 设置技能UI
    /// </summary>
    /// <param name="soldierID"></param>
    private void SetSkillShow(int soldierID)
    {
        SoldierInfo info = SoldierM.GetSoldierInfo(soldierID);
        List<SoldierSkill> skillList = info.m_Skill.GetUpdateSkills();
        for (int i = 0; i < skillList.Count; i++) {
            if (skillList[i].m_level <= 0 || i == 0) {
                continue;
            }
            if (i == 1) {
                MyHead.bigSkillItem.SetData(skillList[i]);
            } else {
                CreateSkillItem(skillList[i]);
            }
        }
        NGUIUtil.RepositionTable(MyHead.SkillTable.gameObject);
    }
    
    private void CreateSkillItem(SoldierSkill soldierSkill)
    {
        GameObject go = NDLoad.LoadWndItem("BigSkillItem", MyHead.SkillTable);
        if (go) {
            BigSkillItem item = go.GetComponent<BigSkillItem>();
            if (item != null) {
                item.SetData(soldierSkill, false);
            }
        }
    }
    
    private void SetStarsLevel(int starLevel)
    {
        if (starLevel < 0 || starLevel > ConstantData.MaxStarLevel) {
            Debug.Log("SetStarLevel=" + starLevel + " 的值非法");
        }
        
        NGUIUtil.SetStarLevelNum(MyHead.StarSprites, starLevel);
    }
    
    /// <summary>
    /// 设置角色的类型（力量、敏捷、智力）
    /// </summary>
    /// <param name="type">0 按力量，1：按敏捷，2 按智力</param>
    private void SetRoleType(int roleType)
    {
        NGUIUtil.SetActive(MyHead.SprRoleType.gameObject, true);
        NGUIUtil.SetRoleType(MyHead.SprRoleType, roleType);
    }
    
    private void DoFinish()
    {
        DestoryEffect(true);
        if (MyHead.Content != null) {
            MyHead.Content.SetActive(false);
        }
        if (m_Finishfun != null) {
            m_Finishfun(m_mode);
        }
    }
    
    private void LoadTrophies(int gtype, int gid, bool isSoul = false, bool isNewVersion = false, int num = 0, bool isBook = false)
    {
        NGUIUtil.SetActive(MyHead.SkillInfo, false);
        NGUIUtil.SetActive(MyHead.Building, false);
        
        if (isNewVersion) {
            if (MyHead.LblDes != null) {
                MyHead.LblDes.gameObject.SetActive(false);
            }
        }
        if (gtype == 1) { //炮弹兵
            SoldierInfo Info = SoldierM.GetStartSoldierInfo(gid);
            CreateSoldier(Info);
            
            if (isNewVersion) {
                if (isSoul && num > 0) {
                    int startLevel = SoldierM.GetSoldierStarLevel(Info.SoldierTypeID);
                    MyHead.LblDes.text = string.Format(NGUIUtil.GetStringByKey(10000119), startLevel, num);
                    MyHead.LblDes.gameObject.SetActive(true);
                }
            } else if (m_isSoulPis && m_SoulPisNum > 0) {
                int startLevel = SoldierM.GetSoldierStarLevel(Info.SoldierTypeID);
                MyHead.LblDes.text = string.Format(NGUIUtil.GetStringByKey(10000119), startLevel, m_SoulPisNum);
            }
            m_bNotTween = false;
        } else if (gtype == 3) { // 陷阱/建筑物
            BuildInfo Info = buildingM.GetStartBuildInfo(gid);
            if (Info != null) {
                CreateBuilding(Info);
            }
            
            m_bNotTween = true;
        } else if (gtype == 4) { //黑科技
            NGUIUtil.SetActive(MyHead.BlackScience, true);
            CaptionInfo info = new CaptionInfo();
            GodSkillM.GetCaption(gid, ref info);
            CreateBlackScience(info);
            if (isNewVersion) {
                if (isBook && num > 0) {
                    int star = GodSkillM.GetCaptionMinStarLevel(info.m_godskilltype1);
                    MyHead.LblDes.text = string.Format(NGUIUtil.GetStringByKey(30000060), star, num);
                    MyHead.LblDes.gameObject.SetActive(true);
                }
            } else if (m_isSoulPis && m_SoulPisNum > 0) {
                int star = GodSkillM.GetCaptionMinStarLevel(info.m_godskilltype1);
                MyHead.LblDes.text = string.Format(NGUIUtil.GetStringByKey(30000060), star, m_SoulPisNum);
            }
            m_bNotTween = true;
        } else if (gtype == 5) { //设计图
            StaticShipCanvas cInfo = ShipPlanM.GetShipCanvasInfo(gid);
            CreateDesignPic(cInfo);
            m_bNotTween = true;
        } else if (gtype == 2) { //碎片
            CreateTrophyItem(gtype, gid);
        }
        SoundPlay.Play("get_item", false, false);
    }
    
    //闪黄
    void Step1(object o)
    {
        if (MyHead.Content != null) {
            MyHead.Content.SetActive(false);
        }
        DestoryEffect(true);
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000401_04",
                Vector3.zero, MyHead.Drop);
        gae.gameObject.AddComponent<SetRenderQueue>();
        
        if (gae != null) {
            GameObjectLoader.SetGameObjectLayer(gae.gameObject, MyHead.Drop.gameObject.layer);
            m_effect.Add(gae.gameObject);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.27f);
            ndEffect.m_complete = Step2;
            gae.AddAction(ndEffect);
        }
    }
    
    //出兵
    void Step2(object o)
    {
        DestoryEffect(false);
        if (MyHead.Content != null) {
            MyHead.Content.SetActive(true);
        }
        
        if (mItemInfos.Count > 0) {
            SetTrophies(mItemInfos[0].mGtype, mItemInfos[0].mName);
            LoadTrophies(mItemInfos[0].mGtype, mItemInfos[0].mGid, mItemInfos[0].mIsSoul, mItemInfos[0].mNewVersion, mItemInfos[0].mCount, mItemInfos[0].mIsBook);
            mItemInfos.RemoveAt(0);
            m_bFinishAni = true;
        }
        RewardAni();
        Step3(null);
    }
    
    /// <summary>
    /// 移动动画
    /// </summary>
    void Step3(object o)
    {
        if (m_bNotTween) {
            return;
        }
        StartCoroutine(TweenMoveParent(2.0f));
    }
    
    IEnumerator TweenMoveParent(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        NGUIUtil.SetActive(MyHead.m_lblTroptype.transform.parent.gameObject, false);
        TweenPosition tp = MyHead.MoveParent.GetComponent<TweenPosition>();
        if (tp) {
            tp.AddOnFinished(ShowSkill);
            tp.from = MyHead.MoveParent.transform.localPosition;
            tp.to = MyHead.MoveLeft.localPosition;
            tp.enabled = true;
            tp.PlayForward();
        }
    }
    
    void ShowSkill()
    {
        NGUIUtil.SetActive(MyHead.SkillInfo, true);
        NGUIUtil.SetActive(MyHead.BtnOK.gameObject, true);
        if (GoSoldier != null) {
            SpinWithMouse spinHero = MyHead.WndBg.AddComponent<SpinWithMouse>();
            if (spinHero != null) {
                spinHero.target = GoSoldier.transform;
            }
        }
    }
    
    /// <summary>
    /// 创建炮弹兵
    /// </summary>
    void CreateSoldier(SoldierInfo Info)
    {
        if (Info == null) {
            return ;
        }
        Role r = new Role();
        r.CreateSkin(MyHead.DropHero, Info.m_modeltype, Info.m_name, AnimatorState.Stand);
        //IGameRole i = GameRoleFactory.Create(MyHead.DropHero,Info.m_modeltype, Info.m_name, AnimatorState.Stand);
        GoSoldier = r.RoleSkinCom.tRoot.gameObject;
        GoSoldier.name = Info.m_name;
        GoSoldier.transform.localScale = new Vector3(120f, 120f, 120f);
        Vector3 pos = GoSoldier.transform.localPosition ;
        pos.z -= 270;
        pos.y += 20;
        GoSoldier.transform.localPosition = pos;
        GameObjectLoader.SetGameObjectLayer(GoSoldier, gameObject.layer);
        m_Tropies = GoSoldier;
        if (Info.m_modeltype == 100003) {
            r.RoleSkinCom.ShowLeftHand(false);
            CreatePet1002(GoSoldier.transform);
        }
        SetStarsLevel(Info.StarLevel);
        SetRoleType(Info.m_main_proerty);
        SetSkillShow(Info.SoldierTypeID);
        NGUIUtil.SetActive(MyHead.HeroInfo, true);
        NGUIUtil.SetActive(MyHead.BtnOK.gameObject, false);
    }
    /// <summary>
    /// 创建蹦蹦
    /// </summary>
    void CreatePet1002(Transform parent)
    {
        Pet pet = new Pet();
        pet.CreateSkin(parent, 1002, "1002", AnimatorState.WinStart);
        // IGameRole i = GameRoleFactory.Create(parent, 1002, "1002", AnimatorState.WinStart);
        GameObject go = pet.PetSkinCom.tRoot.gameObject;
        GameObjectLoader.SetGameObjectLayer(go, parent.gameObject.layer);
        Vector3 pos = pet.PetSkinCom.tRoot.localPosition;
        pos = U3DUtil.AddX(pos, 0.9f);
        go.transform.localPosition = pos;
        go.transform.localScale = Vector3.one;
    }
    /// <summary>
    /// 创建建筑
    /// </summary>
    void CreateBuilding(BuildInfo roomBuild)
    {
        // 创建地形
        if (roomBuild == null) {
            return;
        }
        GameObject dropHeroChild = new GameObject("dropHeroChild");
        dropHeroChild.transform.parent = MyHead.DropHero;
        Vector2 size = new Vector2(roomBuild.m_Shape.width, roomBuild.m_Shape.height);
        Vector3 pos = new Vector3(-50f * size.x, 40f, -600f);
        Vector3 scaleSize = new Vector3(35f, 35f, 35f);
        
        dropHeroChild.transform.localPosition = pos;
        dropHeroChild.transform.localScale = scaleSize;
        
        Vector3 local = Vector3.zero;
        Vector3 world = dropHeroChild.transform.TransformPoint(local);
        roomBuild.m_cx = 0 ;
        roomBuild.m_cy = 0 ;
        Building buildlife = LifeFactory.CreateBuilding(roomBuild, 0, dropHeroChild.transform, world, LifeEnvironment.View);
        if (buildlife != null) {
            buildlife.PlayClickAni();
        }
        // 创建房间外壳
        if (roomBuild.m_RoomType != RoomType.DeckTrap) { //非甲板房间需加壳
            GenerateShip.GenerateShipBuildingShellWithouClear(dropHeroChild.transform, roomBuild.GetBuildCanvasInfo());
        }
        m_Tropies = dropHeroChild;
        SetStarsLevel(roomBuild.StarLevel);
        GameObjectLoader.SetGameObjectLayer(dropHeroChild, gameObject.layer);
        
        NGUIUtil.SetActive(MyHead.Building, true);
        string[] s = roomBuild.m_Desc.Split(new string[] { "\\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (s.Length > 1) {
            string text = string.Format("{0}{1}", NGUIUtil.GetStringByKey(88800039), s[0]);
            NGUIUtil.SetLableText(MyHead.LblBuildDesc, text);
            NGUIUtil.SetLableText(MyHead.LblBuildEffect, s[1]);
        }
    }
    /// <summary>
    /// 创建黑科技
    /// </summary>
    void CreateBlackScience(CaptionInfo info)
    {
        if (info == null) {
            NGUIUtil.DebugLog("CreateBlackScience info == null");
            return;
        }
        GodSkillInfo gInfo = new GodSkillInfo();
        //黑科技抽取是1级，王振鑫确认
        GodSkillM.GetGodSkill(info.m_godskilltype1, 1, ref gInfo);
        NGUIUtil.SetLableText(MyHead.LblBlackScienceDesc, gInfo.m_explain);
        string icon = "Textures/role/" + info.m_captionid;
        NGUIUtil.Set2DSprite(MyHead.Spr2dBSIcon, icon);
        int star = GodSkillM.GetCaptionMinStarLevel(info.m_godskilltype1);
        SetStarsLevel(star);
    }
    /// <summary>
    ///  创建设计图（在金银岛掠夺结算掉落，且只会掉一张）
    /// </summary>
    void CreateDesignPic(StaticShipCanvas info)
    {
        NGUIUtil.SetActive(MyHead.SprDesignQuality.gameObject, true);
        NGUIUtil.SetActive(MyHead.DesignPic, true);
        NGUIUtil.SetSprite(MyHead.SprDesignQuality, info.Quality + "_ico");
        NGUIUtil.SetSprite(MyHead.SprDesignQuality1, info.Quality);
        NGUIUtil.SetLableText(MyHead.LblDesignCellCount, info.Cell);
        MyHead.mShipDesignUnitItem.SetData(info.Id);
        SetStarsLevel(info.StarLevel);
    }
    
    private void CreateTrophyItem(int type, int gid)
    {
        GameObject go = NDLoad.LoadWndItem("TrophyItem", MyHead.DropHero);
        if (go) {
            TrophyItem item = go.GetComponent<TrophyItem>();
            if (item) {
                item.SetData(type, gid);
            }
            
            Vector3 pos = go.transform.localPosition ;
            pos.y += 50;
            go.transform.localPosition = pos;
            
            Vector3 scale = go.transform.localScale ;
            scale = 2 * scale;
            go.transform.localScale = scale;
            m_Tropies = go;
        }
    }
    
    void BtnOK_OnClickEventHandler(UIButton sender)
    {
        if (m_iWndType == 3) {
            PdbbbWnd wnd = WndManager.FindDialog<PdbbbWnd>();
            if (wnd) {
                wnd.RefreshUI();
            }
        } else if (m_iWndType == 5) {
            if (BtnOKClickHandler != null) {
                BtnOKClickHandler();
            }
            WndManager.DestoryDialog<TrophiesActionWnd>();
            return;
        }
        
        if (mItemInfos.Count == 0) {
            WndManager.DestoryDialog<TrophiesActionWnd>();
            if (FinalEventClikHandler != null) {
                FinalEventClikHandler(null);
            }
        } else {
            ResetAni();
            GetTropies(0, null);
        }
    }
    /*
    
    */
    private void RewardAni()
    {
        // -50 ,100,0  -> -50 190 0 -> -50 170 0
        // 0.5 0.1
        float x = -288f;
        if (m_iWndType == 3 || m_iWndType == 2 || m_iWndType == 5) {
            x = -80f;
        }
        Vector3[] path = new Vector3[] { new Vector3(x, 500f, 0f), new Vector3(x, 260f, 0f), new Vector3(x, 270f, 0f), new Vector3(x, 260f, 0f) };
        if (MyHead.m_TropName) {
            Transform p = MyHead.m_lblTroptype.transform.parent;
            if (p) {
                GameObject go = p.gameObject;
                go.transform.DOPath(path, 0.8f);
            }
        }
    }
    
    
    /// <summary>
    /// 连续播放重置
    /// </summary>
    private void ResetAni()
    {
        MyHead.m_lblTroptype.transform.parent.position = m_v3Ani1;
        MyHead.MoveParent.transform.localPosition = m_v3Ani2;
    }
    private void DestoryEffect(bool IsDestroyTrop)
    {
        foreach (GameObject g in m_effect) {
            GameObject.Destroy(g);
        }
        m_effect.Clear();
        
        if (m_Tropies != null && IsDestroyTrop == true) {
            GameObject.Destroy(m_Tropies);
            m_Tropies = null;
        }
    }
    
}
