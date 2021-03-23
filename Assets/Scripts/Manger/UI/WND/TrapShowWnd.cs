using UnityEngine;
using System.Collections.Generic;



public class TrapShowWnd : WndBase
{
    private enum WndType {
        ShuXing = 1,
        ShengJie = 2,
        TuJian = 3,
    };
    /// <summary>
    /// 屏蔽连续点击.
    /// </summary>
    public bool m_bIsShowQualityUp = false;
    
    public TrapShowWnd_h MyHead {
        get
        {
            return (base.BaseHead() as TrapShowWnd_h);
        }
    }
    
    public bool m_ClickUp = true;
    private Building m_buildlife = null;
    private BuildInfo m_backUpBuild = null;
    private BuildInfo m_Info;
    
    private WndType m_wndType;
    //
    private int m_NeedCoin = 0;
    private int m_haveCoin = 0;
    private int m_NeedWood = 0;
    private int m_NeedFragment = 0;
    private int m_HaveFragment = 0;
    
    private List<BuildInfo> m_BuildList = new List<BuildInfo>();
    
    public void SetEnableQuality(bool enable)
    {
        m_bIsShowQualityUp = enable;
    }
    public void SetBuildInfo(BuildInfo info, List<BuildInfo> lbuild)
    {
        m_Info = info;
        BackUpInfo();
        m_BuildList = lbuild;
        GetFragment();
        
        RefreshAllUI();
    }
    
    public void BackUpInfo()
    {
        m_backUpBuild = new BuildInfo();
        m_backUpBuild.m_levellimit = m_Info.m_levellimit;
        m_backUpBuild.Level = m_Info.Level;
        m_backUpBuild.StarLevel = m_Info.StarLevel;
        m_backUpBuild.Quality = m_Info.Quality;
        m_backUpBuild.ID = m_Info.ID;
        m_backUpBuild.m_name = m_Info.m_name;
        m_backUpBuild.BuildType = m_Info.BuildType;
        m_backUpBuild.m_hp = m_Info.m_hp;
        m_backUpBuild.m_DefensePower = m_Info.m_DefensePower;
        m_backUpBuild.m_Shape = m_Info.m_Shape;
    }
    // Use this for initialization
    public override void WndStart()
    {
        base.WndStart();
        
        m_wndType = WndType.ShuXing;
        //开启向上顶模式
        MyHead.HudText.bTopRank = true;
        
        RegisterHooks();
        
        
        
        DoWndEffect();
        
        MyHead.BtnGetSoulStone.OnClickEventHandler += BtnGetSoulStone_OnClickEventHandler;
        MyHead.BtnReturn.OnClickEventHandler += BtnClose_OnClickEventHandler;
        MyHead.BtnClickTrap.OnClickEventHandler += BtnClickTrap_OnClickEventHandler;
        MyHead.BtnShengJi.OnClickEventHandler += BtnShengJi_OnClickEventHandler;
        MyHead.BtnShengXing.OnClickEventHandler += BtnShengXing_OnClickEventHandler;
        MyHead.BtnPre.OnClickEventHandler += BtnPre_OnClickEventHandler;
        MyHead.BtnNext.OnClickEventHandler += BtnNext_OnClickEventHandler;
        MyHead.BtnChaiJie.OnClickEventHandler += BtnChaiJie_OnClickEventHandler;
        
        MyHead.BtnShuXing.bColorChange = false;
        MyHead.BtnShengJie.bColorChange = false;
        MyHead.BtnTuJian.bColorChange = false;
        
        EventDelegate.Add(MyHead.BtnShuXing.onChange, TogShuXing_OnValueChange);
        EventDelegate.Add(MyHead.BtnShengJie.onChange, TogShengJie_OnValueChange);
        EventDelegate.Add(MyHead.BtnTuJian.onChange, TogTuJian_OnValueChange);
        
    }
    private void SetUI()
    {
        MyHead.BtnPre.gameObject.SetActive(m_BuildList.Count > 1 ? true : false);
        MyHead.BtnNext.gameObject.SetActive(m_BuildList.Count > 1 ? true : false);
        
        SetData();
        SetName(m_Info.m_name, m_Info.Quality);
        NGUIUtil.SetStarLevelNum(MyHead.SprStars, m_Info.StarLevel);
        NGUIUtil.SetTrapTypeIcon(MyHead.SprRoleType, m_Info.m_RoomKind);
        MyHead.LblRoleTypeName.text =  NGUIUtil.GetTrapTypeName(m_Info.m_RoomKind);
        AddDetailInfo(m_Info, m_wndType);
        
        SetFragmentLabel();
        
        DoCanLevelUp();
        DoCanUpStar();
        
        if (m_Info.BuildType == 1300) {
            MyHead.BtnChaiJie.gameObject.SetActive(false);
        } else {
            MyHead.BtnChaiJie.gameObject.SetActive(true);
        }
        
    }
    private void DoCanLevelUp()
    {
        CanLevelResult result = buildingM.GetLevelCanUP(m_Info);
        if (result == CanLevelResult.CanUp) {
            TweenScale ts2 = MyHead.BtnShengJi.gameObject.GetComponent<TweenScale>();
            if (ts2 != null) {
                return;
            }
            MyHead.BtnShengJi.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            TweenScale ts = TweenScale.Begin(MyHead.BtnShengJi.gameObject, 0.2f, new Vector3(1.2f, 1.2f, 1.2f));
            ts.style = UITweener.Style.PingPong;
            
        } else {
            TweenScale ts = MyHead.BtnShengJi.gameObject.GetComponent<TweenScale>();
            if (ts != null) {
                Destroy(ts);
            }
        }
    }
    
    private void DoCanUpStar()
    {
        CanStarResult result = buildingM.GetCanUpStar(m_Info);
        if (result == CanStarResult.CanUp) {
            TweenScale ts = MyHead.BtnShengXing.gameObject.GetComponent<TweenScale>();
            if (ts != null) {
                return;
            }
            MyHead.BtnShengXing.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            TweenScale ts2 = TweenScale.Begin(MyHead.BtnShengXing.gameObject, 0.2f, new Vector3(1.2f, 1.2f, 1.2f));
            ts2.style = UITweener.Style.PingPong;
        } else {
            TweenScale ts = MyHead.BtnShengXing.gameObject.GetComponent<TweenScale>();
            if (ts != null) {
                Destroy(ts);
            }
        }
    }
    /// <summary>
    /// Refreshs all UI 上下切换时候调用.
    /// </summary>
    private void RefreshAllUI()
    {
        GameObject go = GameObject.Find("dropHeroChild");
        if (go != null) {
            GameObject.DestroyImmediate(go);
        }
        
        CreateBuilding(m_Info);
        GetFragment();
        SetUI();
    }
    /// <summary>
    /// Ups the re set UI升星升级升阶时候调用.
    /// </summary>
    public void UpReSetUI()
    {
        UpDateBuildInfo();
        RefreshAllUI();
    }
    private void SetData()
    {
        MyHead.LblLv.text = m_Info.Level.ToString();
        
        //		BuildSkillInfo afterInfo = SkillM.GetBuildSkill(m_Info.BuildType,ConfigM.GetBigQuality(m_Info.Quality));
        MyHead.LblDefandPower.text = m_Info.m_DefensePower.ToString();
        //
        //		if(m_Info.m_Skill != null )
        //			MyHead.LblSkill.text = m_Info.m_Skill.m_name ;
        
        
        buildingM.GetUpLevelNeed(m_Info.BuildType, m_Info.Level, ref m_NeedCoin, ref m_NeedWood) ;
        string strCoinColor = "";
        if (UserDC.GetCoin() >= m_NeedCoin) {
            strCoinColor = "[FFFFFF]{0}[-]";
        } else {
            strCoinColor = "[FF0000]{0}[-]";
        }
        MyHead.LblGoldNum.text = string.Format(strCoinColor, m_NeedCoin);
        
        if (UserDC.GetWood() >= m_NeedWood) {
            strCoinColor = "[FFFFFF]{0}[-]";
        } else {
            strCoinColor = "[FF0000]{0}[-]";
        }
        MyHead.LblWoodNum.text = string.Format(strCoinColor, m_NeedWood);
        
        MyHead.BtnShuXing.startsActive = true;
    }
    public void SetName(string name, int quality)
    {
        int bigLevel = ConfigM.GetBigQuality(quality);
        
        MyHead.LblTrapQuality.text = NGUIUtil.GetSmallQualityStr(quality);
        NGUIUtil.SetLableText<string>(MyHead.LblTrapName, NGUIUtil.GetBigQualityName(name, quality));
        NGUIUtil.SetSprite(MyHead.SprQuality, bigLevel.ToString());
    }
    /// <summary>
    /// 设置碎片进度条
    /// </summary>
    private void SetFragmentLabel()
    {
        /// <summary>
        /// 设置灵魂石
        /// </summary>
        if (m_Info.StarLevel == 5) {
            MyHead.SprSoulPercentage.fillAmount = 1.0f;
            MyHead.LblSoulNum.text = string.Format("[FFFFFF]" + NGUIUtil.GetStringByKey(10000085) + "[-]");
            
            MyHead.BtnShengXing.gameObject.SetActive(false);
        } else {
            MyHead.SprSoulPercentage.fillAmount = (m_HaveFragment * 1.0f) / m_NeedFragment;
            MyHead.LblSoulNum.text = string.Format("[FFFFFF]{0}/{1}[-]", m_HaveFragment, m_NeedFragment);
            MyHead.BtnShengXing.gameObject.SetActive(true);
        }
    }
    
    
    
    /// <summary>
    /// 添加属性,升阶,图鉴
    /// </summary>
    /// <param name="Info">Info.</param>
    /// <param name="wndType">Window type.</param>
    private void AddDetailInfo(BuildInfo Info, WndType wndType)
    {
        if (wndType == WndType.ShuXing) {
            WndManager.DestoryDialog<TrapShengJieIntroduceWnd>();
            TrapShuXingIntroduceWnd wnd = WndManager.FindDialog<TrapShuXingIntroduceWnd>();
            if (wnd == null) {
                wnd = WndManager.GetDialog<TrapShuXingIntroduceWnd>();
                wnd.transform.parent = MyHead.SprTrapIntorduct.transform;
                wnd.transform.localScale = MyHead.SprTrapIntorduct.transform.localScale;
                wnd.transform.localPosition = Vector3.zero;
                wnd.transform.localRotation = MyHead.SprTrapIntorduct.transform.localRotation;
            }
            if (wnd != null) {
                wnd.SetBuildInfo(Info);
            }
            
        } else if (wndType == WndType.ShengJie) {
            WndManager.DestoryDialog<TrapShuXingIntroduceWnd>();
            TrapShengJieIntroduceWnd wnd = WndManager.FindDialog<TrapShengJieIntroduceWnd>();
            if (wnd == null) {
                wnd = WndManager.GetDialog<TrapShengJieIntroduceWnd>();
                wnd.transform.parent = MyHead.SprTrapIntorduct.transform;
                wnd.transform.localScale = MyHead.SprTrapIntorduct.transform.localScale;
                wnd.transform.localPosition = Vector3.zero;
                wnd.transform.localRotation = MyHead.SprTrapIntorduct.transform.localRotation;
            }
            if (wnd != null) {
                wnd.SetBuildInfo(Info, this);
            }
            
        } else if (wndType == WndType.TuJian) {
            WndManager.DestoryDialog<TrapShuXingIntroduceWnd>();
            WndManager.DestoryDialog<TrapShengJieIntroduceWnd>();
        }
        
        
    }
    /// <summary>
    ///	关闭窗体回调 call back.
    /// </summary>
    /// <param name="o">O.</param>
    void DestoryDialogCallBack(object o)
    {
        WndManager.DestoryDialog<TrapShowWnd>();
        
    }
    void BtnGetSoulStone_OnClickEventHandler(UIButton sender)
    {
        sdata.s_itemtypeInfo itemInfo = ItemM.GetItemInfo(m_Info.fragmentTypeID);//当前灵魂石
        ItemComeFromWnd wnd = WndManager.GetDialog<ItemComeFromWnd>();
        wnd.SetData(itemInfo, null, m_Info, 4);
    }
    private void BtnClose_OnClickEventHandler(UIButton sender)
    {
        WndEffects.DoCloseWndEffect(gameObject, DestoryDialogCallBack);
    }
    /// <summary>
    /// 点击模型播放动画.
    /// </summary>
    /// <param name="sender">Sender.</param>
    private void BtnClickTrap_OnClickEventHandler(UIButton sender)
    {
        if (m_buildlife != null) {
            m_buildlife.PlayClickAni();
        }
    }
    /// <summary>
    /// 升级
    /// </summary>
    /// <param name="sender">Sender.</param>
    private void BtnShengJi_OnClickEventHandler(UIButton sender)
    {
        //.0能升级.1金币不足.2其他材料不足.3满级.
        CanLevelResult result = buildingM.GetLevelCanUP(m_Info);
        if (result == CanLevelResult.CanUp) {
            BackUpInfo();
            BuildDC.Send_BuildlevelUpRequest(m_Info.ID);
        }
        
    }
    private void DoShengXing(UIButton sender)
    {
        SetEnableQuality(true);
        BackUpInfo();
        BuildDC.Send_BuildStarUpRequest(m_Info.ID);
    }
    /// <summary>
    /// 升星
    /// </summary>
    /// <param name="sender">Sender.</param>
    private void BtnShengXing_OnClickEventHandler(UIButton sender)
    {
        if (m_bIsShowQualityUp) {
            return;
        }
        
        CanStarResult result = buildingM.GetCanUpStar(m_Info);
        if (result == CanStarResult.CanUp) {
            DialogWnd wnd = WndManager.GetDialog<DialogWnd>();
            if (wnd) {
                int iCoin = 0;
                int NeedFragmentNum = 0;
                
                buildingM.GetUpStarNeed(m_Info.BuildType, m_Info.StarLevel + 1, ref NeedFragmentNum, ref iCoin);
                wnd.SetDialogLable(string.Format("[ae690f]" + NGUIUtil.GetStringByKey("10000158") + "[-]", iCoin), "", "");
                wnd.YESButtonOnClick = DoShengXing;
                wnd.ShowDialog();
            }
        }
    }
    
    ///下一个
    ///
    private void BtnNext_OnClickEventHandler(UIButton sender)
    {
        BuildInfo info   = m_BuildList.NextOf<BuildInfo>(m_Info);
        if (info.ID == m_Info.ID) {
            return;
        } else {
            m_Info = info;
        }
        RefreshAllUI();
    }
    ///上一个
    ///
    private void BtnPre_OnClickEventHandler(UIButton sender)
    {
        BuildInfo info  = m_BuildList.PrevOf<BuildInfo>(m_Info);
        if (info.ID == m_Info.ID) {
            return;
        } else {
            m_Info = info;
        }
        RefreshAllUI();
    }
    
    private void BtnChaiJie_OnClickEventHandler(UIButton sender)
    {
        if (m_Info.CheckSplit() == false) {
            NGUIUtil.ShowTipWndByKey(10000181);
        } else {
            TrapSplitWnd wnd =  WndManager.GetDialog<TrapSplitWnd>();
            if (wnd != null) {
                wnd.SetData(m_Info);
            }
        }
    }
    /// <summary>
    /// 获取需要的碎片及拥有数量。
    /// </summary>
    private void GetFragment()
    {
        //获取升级需要的资源。
        buildingM.GetUpStarNeed(m_Info.BuildType, m_Info.StarLevel + 1, ref  m_NeedFragment, ref  m_NeedCoin) ;
        m_HaveFragment = ItemDC.GetItemCount(m_Info.fragmentTypeID);//当前灵魂石
        m_haveCoin = UserDC.GetCoin() ;
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
        dropHeroChild.transform.parent = MyHead.BtnClickTrap.transform;
        
        Vector2 size = new Vector2(roomBuild.m_Shape.width, roomBuild.m_Shape.height);
        Vector3 pos = new Vector3(-50f * size.x, -47.6f, -600f);
        Vector3 scaleSize = new Vector3(35f, 35f, 35f);
        if (size.y >= 2) {
            scaleSize = new Vector3(20f, 20f, 20f);
            pos = new Vector3(-50f * size.x + 20f, -47.6f, -600f);
        }
        
        dropHeroChild.transform.localPosition = pos;
        dropHeroChild.transform.localScale = scaleSize;
        
        Vector3 local = Vector3.zero;
        Vector3 world = dropHeroChild.transform.TransformPoint(local);
        roomBuild.m_cx = 0 ;
        roomBuild.m_cy = 0 ;
        m_buildlife = LifeFactory.CreateBuilding(roomBuild, 0, dropHeroChild.transform, world, LifeEnvironment.View);
        if (m_buildlife != null) {
            m_buildlife.PlayViewAni();
            if (roomBuild.BuildType == 1605) {
                ParticleEffect effect = m_buildlife.m_thisT.gameObject.AddComponent<ParticleEffect>();
                effect.EnabelScale = true;
            }
        }
        // 创建房间外壳
        if (roomBuild.m_RoomType != RoomType.DeckTrap) { //非甲板房间需加壳
            GenerateShip.GenerateShipBuildingShell(dropHeroChild.transform, roomBuild.GetBuildCanvasInfo());
        }
        
        GameObjectLoader.SetGameObjectLayer(dropHeroChild, gameObject.layer);
    }
    /// <summary>
    /// 属性
    /// </summary>
    public void TogShuXing_OnValueChange()
    {
        if (!MyHead.BtnShuXing.value) {
            return;
        }
        bool change = m_wndType != WndType.ShuXing;
        m_wndType = WndType.ShuXing;
        AddDetailInfo(m_Info, m_wndType);
    }
    /// <summary>
    /// 技能
    /// </summary>
    public void TogShengJie_OnValueChange()
    {
        if (!MyHead.BtnShengJie.value) {
            return;
        }
        bool change = m_wndType != WndType.ShengJie;
        m_wndType = WndType.ShengJie;
        AddDetailInfo(m_Info, m_wndType);
    }
    /// <summary>
    /// 图鉴
    /// </summary>
    public void TogTuJian_OnValueChange()
    {
        if (!MyHead.BtnTuJian.value) {
            return;
        }
        U3DUtil.DestroyAllChild(MyHead.SprTrapIntorduct as UnityEngine.GameObject);
        
    }
    
    
    private void UpDateBuildInfo()
    {
        m_Info = BuildDC.GetBuilding(m_Info.ID);
    }
    
    public void OnDestroy()
    {
        AntiRegisterHooks();
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    public   void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_906, TrapStarUp_Resp);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_902, TrapLevelUp_Resp);
    }
    
    /// <summary>
    /// 反注册事件
    /// </summary>
    public   void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_906, TrapStarUp_Resp);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_902, TrapLevelUp_Resp);
    }
    private void ShowStarAddGrow()
    {
        //硬度
        float iSolidGrow = 0;
        float iIntensityGrow = 0;
        float iTenacityGrow = 0;
        buildingM.GetStarInfoGrow(m_backUpBuild, ref iSolidGrow, ref iIntensityGrow, ref iTenacityGrow);
        
        
        float iAfterSolidGrow = 0;
        float iAfterIntensityGrow = 0;
        float iAfterTenacityGrow = 0;
        buildingM.GetStarInfoGrow(m_Info, ref iAfterSolidGrow, ref iAfterIntensityGrow, ref iAfterTenacityGrow);
        
        AddHudTextShow(NGUIUtil.GetStringByKey(10000082) + " +" + (iAfterSolidGrow - iSolidGrow).ToString("0.00"));
        AddHudTextShow(NGUIUtil.GetStringByKey(10000083) + " +" + (iAfterIntensityGrow - iIntensityGrow).ToString("0.00"), 0.8f);
        AddHudTextShow(NGUIUtil.GetStringByKey(10000084) + " +" + (iAfterTenacityGrow - iTenacityGrow).ToString("0.00"), 0.8f);
    }
    void ShowUpStarWndCallBack(object o)
    {
        TrapUpStarWnd Wnd = WndManager.GetDialog<TrapUpStarWnd>();
        
        if (Wnd != null) {
            Wnd.SetData(m_Info, m_backUpBuild, this);
        }
    }
    /// <summary>
    /// 906 升星
    /// </summary>
    /// <param name="nErrorCode"></param>
    void TrapStarUp_Resp(int nErrorCode)
    {
        if (nErrorCode == 0) {
            UpStarEffect();
            
            UpReSetUI();
            ShowStarAddGrow();
            
            NGUIUtil.ExcuteWaitAction(gameObject, 1.2f, ShowUpStarWndCallBack);
        }
        
    }
    
    
    
    public void UpStarEffect()
    {
        SoundPlay.Play("hero_promotion", false, false);
        for (int i = 0; i < MyHead.SprStars.Length; i++) {
            if (MyHead.SprStars[i].spriteName == "gong_003ic") {
                Vector3 pos = MyHead.SprStars[i].transform.localPosition;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000741",
                        pos, MyHead.SprStars[i].transform.parent);
                gae.gameObject.AddComponent<SetRenderQueue>();
                
                if (gae != null) {
                    GameObjectLoader.SetGameObjectLayer(gae.gameObject, MyHead.SprStars[i].transform.parent.gameObject.layer);
                    GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.8f);
                    gae.AddAction(ndEffect);
                    gae.gameObject.transform.localPosition = pos;
                }
                MyHead.SprStars[i].spriteName = "cz_001ic";
                break;
            }
            
        }
    }
    void ShowUpQuilityWndCallBack(object o)
    {
    
        TrapUpQualityWnd Wnd = WndManager.GetDialog<TrapUpQualityWnd>();
        
        if (Wnd != null) {
            Wnd.SetData(m_Info, m_backUpBuild, this);
        }
    }
    
    public void UpQualityEffect()
    {
        SetEnableQuality(true);
        
        SoundPlay.Play("hero_promotion", false, false);
        UpReSetUI();
        
        Vector3 pos = new Vector3(0, -10, ConstantData.iDepBefore3DModel);
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000721",
                pos, MyHead.EffectParent.gameObject.transform);
        gae.gameObject.AddComponent<SetRenderQueue>();
        
        if (gae != null) {
            GameObjectLoader.SetGameObjectLayer(gae.gameObject, MyHead.EffectParent.gameObject.layer);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
            gae.AddAction(ndEffect);
            gae.gameObject.transform.localPosition = pos;
        }
        
        int iHpAdd = m_Info.m_hp - m_backUpBuild.m_hp;
        int iPower = m_Info.m_DefensePower - m_backUpBuild.m_DefensePower;
        
        AddHudTextShow(NGUIUtil.GetStringByKey(10000080) + " +" + iPower.ToString("0.00"));
        AddHudTextShow(NGUIUtil.GetStringByKey(10000081) + " +" + iHpAdd.ToString("0.00"), 0.8f);
        
        NGUIUtil.ExcuteWaitAction(gameObject, 1.8f, ShowUpQuilityWndCallBack);
    }
    
    /// <summary>
    /// 902 升级
    /// </summary>
    /// <param name="nErrorCode"></param>
    void TrapLevelUp_Resp(int nErrorCode)
    {
        if (nErrorCode == 0) {
            UpLevelEffect();
            
            float iAfterSolidGrow = 0;
            float iAfterIntensityGrow = 0;
            float iAfterTenacityGrow = 0;
            buildingM.GetStarInfoGrow(m_Info, ref iAfterSolidGrow, ref iAfterIntensityGrow, ref iAfterTenacityGrow);
            
            AddHudTextShow(NGUIUtil.GetStringByKey(70000002) + "+" + iAfterSolidGrow.ToString("0.00"));
            AddHudTextShow(NGUIUtil.GetStringByKey(70000003) + "+" + iAfterIntensityGrow.ToString("0.00"), 0.8f);
            AddHudTextShow(NGUIUtil.GetStringByKey(70000004) + "+" + iAfterTenacityGrow.ToString("0.00"), 0.8f);
        } else {
            NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey(10000073), null, 0, 1f, ConstantData.iDepBefore3DModel);
        }
        
    }
    /// <summary>
    /// 升级特效
    /// </summary>
    public void UpLevelEffect()
    {
        SoundPlay.Play("hero_promotion", false, false);
        Vector3 pos = new Vector3(0, -10, ConstantData.iDepBefore3DModel);
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000711",
                pos, MyHead.EffectParent.gameObject.transform);
        gae.gameObject.AddComponent<SetRenderQueue>();
        
        if (gae != null) {
            GameObjectLoader.SetGameObjectLayer(gae.gameObject, MyHead.EffectParent.gameObject.layer);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
            gae.AddAction(ndEffect);
            gae.gameObject.transform.localPosition = pos;
        }
        UpReSetUI();
    }
    
    private void AddHudTextShow(string showText, float delay = 0f)
    {
    
        NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, showText, Color.green, delay);
    }
    
    void DoWndEffect()
    {
    
        WndEffects.DoWndEffect(gameObject);
        
    }
}
