using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sdata;

/// <summary>
/// 炮弹兵背包
/// <Author>QFord</Author>
/// </summary>
public class PdbycWnd : WndBase
{

    public PdbycWnd_h MyHead {
        get
        {
            return (base.BaseHead() as PdbycWnd_h);
        }
    }
    /// <summary>
    /// 数据
    /// </summary>
    private SoldierInfo m_soldierInfo;
    private SoldierInfo m_preSoldierInfo = new SoldierInfo();
    
    private int m_ID;
    /// <summary>
    /// 控制3D角色动作
    /// </summary>
    //private RolePropertyM m_rpm;
    private Role m_role;
    private int m_iRoleAniIndex = 0;
    /// <summary>
    /// 用来循环播放的炮弹兵动作
    /// </summary>
    private AnimatorState[] RoleAni = new AnimatorState[] { AnimatorState.Stand, AnimatorState.Idle, AnimatorState.Attack85000 };
    /// <summary>
    ///  用来左/右切换炮弹兵的数据
    /// </summary>
    private List<SoldierInfo> m_allExistSoldier = new List<SoldierInfo>();
    /// <summary>
    /// 存储装备ID（d_item ID）
    /// </summary>
    private int [] EquipID = new int[6];
    /// <summary>
    /// 星级信息
    /// </summary>
    s_soldierstar_typeInfo m_starInfo;
    /// <summary>
    /// 是否满足升星条件
    /// </summary>
    bool m_bStarLevelUp = false;
    
    bool m_bRunFirst = true;
    
    private List<EquipmentItem> m_lEquipItems = new List<EquipmentItem>();
    
    private bool m_bCanEquipsReady = false;
    //   private  List<Transform> m_lCanEquips = new List<Transform>();
    private  List<EquipmentItem> m_lCanEquips = new List<EquipmentItem>();
    /// <summary>
    /// 可通过 打副本得到
    /// </summary>
    private Transform m_tCanGet = null;
    private Transform m_tNotCanEquip = null;
    private bool m_jingjieFinish = false;
    
    public override void WndStart()
    {
        base.WndStart();
        
        MyHead.BtnSoldierNext.OnClickEventHandler += BtnSoldierNext_OnClickEventHandler;
        MyHead.BtnSoldierPrev.OnClickEventHandler += BtnSoldierPrev_OnClickEventHandler;
        
        MyHead.BtnClickSoldier.OnClickEventHandler += BtnClickSoldier_OnClickEventHandler;
        MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
        MyHead.BtnGetSoulStone.OnClickEventHandler += BtnGetSoulStone_OnClickEventHandler;
        MyHead.BtnJinSheng.OnClickEventHandler += BtnJinSheng_OnClickEventHandler;
        MyHead.BtnShengXing.OnClickEventHandler += BtnShengXing_OnClickEventHandler;
        MyHead.BtnTips.OnClickEventHandler += BtnHelp;
        MyHead.SprTipsInfo.gameObject.SetActive(false);
        
        EventDelegate.Add(MyHead.Toggle1.onChange, Toggle1);
        EventDelegate.Add(MyHead.Toggle2.onChange, Toggle2);
        EventDelegate.Add(MyHead.Toggle3.onChange, Toggle3);
        
        PdbbbWnd wnd = WndManager.FindDialog<PdbbbWnd>();
        if (wnd != null) {
            m_allExistSoldier = wnd.AllExistSoldier;
            if (m_allExistSoldier.Count < 2) {
                MyHead.BtnSoldierNext.gameObject.SetActive(false);
                MyHead.BtnSoldierPrev.gameObject.SetActive(false);
            }
        } else {
            MyHead.BtnSoldierNext.gameObject.SetActive(false);
            MyHead.BtnSoldierPrev.gameObject.SetActive(false);
        }
        RegisterHooks();
        DoWndEffect();
        
    }
    void DoWndEffect()
    {
        WndEffects.DoWndEffect(gameObject);
    }
    void EnablePrevNextBtn(bool isEnable)
    {
        MyHead.BtnSoldierNext.enabled = isEnable;
        MyHead.BtnSoldierPrev.enabled = isEnable;
        MyHead.BtnJinSheng.enabled = isEnable;
    }
    public void RegisterHooks()
    {
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_204, SoldierStarUpResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_210, SoldierQualityUpResponse);
        DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_208, SoldierEquipResponse);
    }
    
    public void AntiRegisterHooks()
    {
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_204, SoldierStarUpResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_210, SoldierQualityUpResponse);
        DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_208, SoldierEquipResponse);
    }
    private void SoldierStarUpResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            m_soldierInfo = SoldierDC.GetSoldiers(m_ID);
            SetData(m_soldierInfo);
            ShowStarAddGrow();
            UpLevelEffect();
            NGUIUtil.ExcuteWaitAction(gameObject, 1.2f, ShowUpStarWndCallBack);
        } else {
            NGUIUtil.DebugLog("炮弹兵升星失败 =" + nErrorCode);
        }
    }
    void ShowUpStarWndCallBack(object o)
    {
        SoldierUpStarWnd Wnd = WndManager.GetDialog<SoldierUpStarWnd>();
        if (Wnd != null) {
            Wnd.SetData(m_preSoldierInfo, m_soldierInfo);
        }
        m_preSoldierInfo.Copy(m_soldierInfo);
        EnablePrevNextBtn(true);
    }
    void ShowUpQualityWndCallBack(object o)
    {
        //		if(m_soldierInfo.SoldierTypeID == 100002 || m_soldierInfo.SoldierTypeID ==100004
        //		   || m_soldierInfo.SoldierTypeID ==101003 || m_soldierInfo.SoldierTypeID ==101004 || m_soldierInfo.SoldierTypeID == 102004)
        //		{
        //			SoldierUpQualityWnd Wnd = WndManager.GetDialog<SoldierUpQualityWnd>();
        //			if (Wnd != null)
        //			{
        //				Wnd.SetData(m_preSoldierInfo, m_soldierInfo);
        //			}
        //			m_jingjieFinish = true;
        //		}
        //		else
        {
            SoldierUpQualityWndOld Wnd2 = WndManager.GetDialog<SoldierUpQualityWndOld>();
            if (Wnd2 != null) {
                Wnd2.SetData(m_preSoldierInfo, m_soldierInfo);
            }
        }
        m_preSoldierInfo.Copy(m_soldierInfo);
        
        EnablePrevNextBtn(true);
    }
    /// <summary>
    /// 显示技能解锁界面
    /// </summary>
    void ShowFreeSkillWnd()
    {
        FreeNewSkillWnd wnd = WndManager.GetDialog<FreeNewSkillWnd>();
        wnd.SetData(1003, "天启导弹", "这里是很多技能的描述。\n换行啦！！！");
    }
    
    
    private void SoldierQualityUpResponse(int nErrorCode)
    {
        if (nErrorCode == 0) {
            m_soldierInfo = SoldierDC.GetSoldiers(m_ID);
            SetData(m_soldierInfo);
            float duration = ShowQualityAddGrow(m_preSoldierInfo, m_soldierInfo);
            UpLevelEffect();
            //播放炮弹兵动作 和 独白
            NGUIUtil.ExcuteWaitAction(gameObject, duration, ShowUpQualityWndCallBack);
        }
    }
    
    /// <summary>
    /// 穿装备回应
    /// </summary>
    /// <param name="nErrorCode"></param>
    private void SoldierEquipResponse(int nErrorCode)
    {
        m_bCanEquipsReady = false;
        //NGUIUtil.DebugLog("穿装备回应");
        if (nErrorCode == 0) {
            RefreshUI();
        }
        WndManager.DestoryDialog<EquipmentInfoWnd>();
        WndManager.DestoryDialog<EquipComposeWnd>();
    }
    
    /// <summary>
    /// 切换到下一个炮弹兵
    /// </summary>
    void BtnSoldierNext_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<ClickSkillDescWnd>();
        
        SoldierInfo info1 = m_allExistSoldier.NextOf<SoldierInfo>(m_soldierInfo);
        SetData(info1);
        
        m_preSoldierInfo.Copy(info1);
    }
    /// <summary>
    /// 切换到上一个炮弹兵
    /// </summary>
    void BtnSoldierPrev_OnClickEventHandler(UIButton sender)
    {
        WndManager.DestoryDialog<ClickSkillDescWnd>();
        
        SoldierInfo info1 = m_allExistSoldier.PrevOf<SoldierInfo>(m_soldierInfo);
        SetData(info1);
        m_preSoldierInfo.Copy(info1);
    }
    /// <summary>
    /// 点击炮弹兵播放动作
    /// </summary>
    void BtnClickSoldier_OnClickEventHandler(UIButton sender)
    {
        if (m_role != null) {
            m_iRoleAniIndex++;
            if (m_iRoleAniIndex > RoleAni.Length - 1) {
                m_iRoleAniIndex = 0;
            }
            AnimatorState state = RoleAni[m_iRoleAniIndex];
            m_role.RoleSkinCom.ChangeState(state);
            if (m_iRoleAniIndex != 0) {
                StartCoroutine(AutoToIdle());
            }
        }
    }
    //自动切换到休闲动作
    IEnumerator   AutoToIdle()
    {
        yield return new WaitForEndOfFrame();
        GameObjectActionExcute gae = GameObjectActionExcute.CreateExcute(MyHead.BtnClickSoldier.gameObject);
        gae.Stop();
        float aniLength = m_role.RoleSkinCom.GetAniLength();
        //NGUIUtil.DebugLog("index=" + m_iRoleAniIndex + "length=" + aniLength);
        GameObjectActionWait wait = new GameObjectActionWait(aniLength, PlayIdle);
        wait.Data1 = aniLength;
        gae.AddAction(wait);
    }
    
    void PlayIdle(object o)
    {
        AnimatorState state = RoleAni[0];
        m_role.RoleSkinCom.ChangeState(state);
    }
    /// <summary>
    /// 刷新UI（穿新装备）
    /// </summary>
    public void RefreshUI()
    {
        m_soldierInfo =  SoldierDC.GetSoldiers(m_soldierInfo.ID);
        SetData(m_soldierInfo);
        UpdateSoldierInList(m_soldierInfo.ID);
        ShowPutEquipEffect();
        DoCanJinSheng();
        DoCanShengXing();
    }
    private void DoCanShengXing()
    {
        if (m_bStarLevelUp) {
            TweenScale ts2 = MyHead.BtnShengXing.gameObject.GetComponent<TweenScale>();
            if (ts2 != null) {
                return;
            }
            MyHead.BtnShengXing.transform.localScale = new Vector3(1f, 1f, 1f);
            TweenScale ts = TweenScale.Begin(MyHead.BtnShengXing.gameObject, 0.2f, new Vector3(1.2f, 1.2f, 1.2f));
            ts.style = UITweener.Style.PingPong;
            
        } else {
            TweenScale ts = MyHead.BtnShengXing.gameObject.GetComponent<TweenScale>();
            if (ts != null) {
                Destroy(ts);
            }
        }
    }
    private void DoCanJinSheng()
    {
        if (CheckJinJie()) {
            TweenScale ts2 = MyHead.BtnJinSheng.gameObject.GetComponent<TweenScale>();
            if (ts2 != null) {
                return;
            }
            MyHead.BtnJinSheng.transform.localScale = new Vector3(1f, 1f, 1f);
            TweenScale ts = TweenScale.Begin(MyHead.BtnJinSheng.gameObject, 0.2f, new Vector3(1.2f, 1.2f, 1.2f));
            ts.style = UITweener.Style.PingPong;
            
        } else {
            TweenScale ts = MyHead.BtnJinSheng.gameObject.GetComponent<TweenScale>();
            if (ts != null) {
                Destroy(ts);
            }
        }
    }
    
    /// <summary>
    /// 刷新战斗力（技能升级）
    /// </summary>
    public void RefreshCombatPower()
    {
        m_soldierInfo = SoldierDC.GetSoldiers(m_soldierInfo.ID);
        MyHead.LblCombatPower.text = m_soldierInfo.m_combat_power.ToString();//设置战斗力
    }
    
    /// <summary>
    /// 穿装备后飘出增加的属性 文本
    /// </summary>
    private void ShowPutEquipEffect()
    {
        EquipmentInfoWnd wnd = WndManager.FindDialog<EquipmentInfoWnd>();
        if (wnd) {
            Dictionary<int, int> equipAddAttrs = wnd.GetEquipAddAttr();
            int count = 0;
            foreach (var item in equipAddAttrs) {
                string text = string.Format("99800{0:000}", item.Key);
                text = NGUIUtil.GetLocalizationStr(text);
                if (text != "") {
                    string value = item.Value.ToString();
                    NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, text + "+" + value, Color.green, count == 0 ? 0f : 0.8f);
                    count++;
                }
            }
        }
    }
    
    private void  UpdateSoldierInList(int id)
    {
        for (int i = 0; i < m_allExistSoldier.Count; i++) {
            if (m_allExistSoldier[i].ID == id) {
                m_allExistSoldier[i] = m_soldierInfo;
            }
        }
    }
    
    void ClearUI()
    {
        WndManager.DestoryDialogNow<AddSkillWnd>();
        WndManager.DestoryDialogNow<HeroIntroduceWnd>();
        U3DUtil.DestroyAllChild(MyHead.SoldierPos.gameObject);
        for (int i = 0; i < MyHead.SoldierEquipList.Length; i++) {
            //不删除晋升飞行的装备对象
            U3DUtil.DestroyAllChild(MyHead.SoldierEquipList[i], "Clone");
        }
        
        m_lCanEquips.Clear();
    }
    /// <summary>
    /// 设置数据并刷新UI
    /// </summary>
    /// <param name="info"></param>
    public void SetData(SoldierInfo info, bool isFirstLoad = false)
    {
        ClearUI();
        m_soldierInfo = info;
        
        m_ID = info.ID;
        m_starInfo = SoldierM.GetSoldierStarData(info.SoldierTypeID, info.StarLevel);
        if (m_soldierInfo == null) {
            NGUIUtil.DebugLog("SoldierInfoWnd.cs   m_soldierInfo==null !");
            return;
        }
        EquipID[0] = info.Equipment0;
        EquipID[1] = info.Equipment1;
        EquipID[2] = info.Equipment2;
        EquipID[3] = info.Equipment3;
        EquipID[4] = info.Equipment4;
        EquipID[5] = info.Equipment5;
        SetUI(isFirstLoad);
        DoCanJinSheng();
        DoCanShengXing();
        MyHead.Toggle2.startsActive = true;
        DoCanJinSheng();
        DoCanShengXing();
    }
    private void SetUI(bool isFirstLoad = false)
    {
        //设置角色的类型（力量0、敏捷1、智力2）
        MyHead.SprType.spriteName = string.Format("icon00{0}", m_soldierInfo.m_main_proerty + 1);
        
        MyHead.LblSmallQuality.text = NGUIUtil.GetSmallQualityStr(m_soldierInfo.Quality);
        MyHead.LblSoldierName.text = NGUIUtil.GetBigQualityName(m_soldierInfo.m_name, m_soldierInfo.Quality); //设置角色名称
        
        NGUIUtil.SetLableTextByKey<string>(MyHead.LblLocation, (10000146 + m_soldierInfo.m_loaction - 1).ToString());
        
        NGUIUtil.SetStarLevelNum(MyHead.SprStars, m_soldierInfo.StarLevel);
        
        NGUIUtil.SetSprite(MyHead.SprHeadQuality, ConfigM.GetBigQuality(m_soldierInfo.Quality).ToString());
        
        Create3DSoldier();
        GetEquimments();
        MyHead.LblLevel.text = m_soldierInfo.Level.ToString();//设置角色等级
        SetExp();
        MyHead.LblCombatPower.text = m_soldierInfo.m_combat_power.ToString();//设置战斗力
        SetSoulPercentage();
        if (!isFirstLoad) {
            LoadCurrentToggle1();
        }
        
    }
    
    /// <summary>
    /// 创建 3D 角色
    /// </summary>
    void Create3DSoldier()
    {
        if (m_soldierInfo == null) {
            return;
        }
        if (m_role == null) {
            m_role = new Role();
        }
        m_role.CreateSkin(MyHead.SoldierPos, m_soldierInfo.m_modeltype, m_soldierInfo.m_name, AnimatorState.Stand);
        //IGameRole i = GameRoleFactory.Create(MyHead.SoldierPos, m_soldierInfo.m_modeltype, m_soldierInfo.m_name, AnimatorState.Stand);
        if (m_role == null) {
            NGUIUtil.DebugLog("创建3D角色出错：modelType =" + m_soldierInfo.m_modeltype);
            return;
        }
        GameObject go = m_role.RoleSkinCom.tRoot.gameObject;
        go.name = m_soldierInfo.m_name;
        go.transform.localScale = new Vector3(100f, 100f, 100f);
        Vector3 pos = go.transform.localPosition;
        pos.z -= 270;
        pos.y += 20;
        go.transform.localPosition = pos;
        GameObjectLoader.SetGameObjectLayer(go, gameObject.layer);
        if (m_soldierInfo.m_modeltype == 100003) {
            CreatePet1002(go.transform);
        }
    }
    /// <summary>
    /// 创建蹦蹦
    /// </summary>
    void CreatePet1002(Transform parent)
    {
        Pet p = new Pet();
        p.CreateSkin(parent, 1002, "1002", AnimatorState.WinStart);
        // IGameRole i = GameRoleFactory.Create(parent, 1002, "1002", AnimatorState.WinStart);
        GameObject go = p.PetSkinCom.tRoot.gameObject;
        //		RolePropertyM peet = i.GetBodyComponent<RolePropertyM>(); ;
        //		if(peet != null)
        //		{
        //			peet.SetAnimatorEnable(false);
        //		}
        GameObjectLoader.SetGameObjectLayer(go, parent.gameObject.layer);
        Vector3 pos = p.PetSkinCom.tRoot.localPosition;
        pos = U3DUtil.AddX(pos, 0.9f);
        go.transform.localPosition = pos;
        go.transform.localScale = Vector3.one;
    }
    
    /// <summary>
    /// 获取装备情况
    /// </summary>
    private void GetEquimments()
    {
        for (int i = 0; i < EquipID.Length; i++) {
            CreateEquipItem(EquipID[i], i);
        }
        m_bCanEquipsReady = true;
    }
    /// <summary>
    /// 创建装备
    /// </summary>
    /// <param name="equipID">装备ID</param>
    /// <param name="posIndex">装备穿戴位置</param>
    private void CreateEquipItem(int dItemID, int posIndex)
    {
        GameObject equipPos = MyHead.SoldierEquipList[posIndex];
        GameObject go = NDLoad.LoadWndItem("EquipmentItem", equipPos.transform);
        EquipmentItem item = go.GetComponent<EquipmentItem>();
        if (item) {
            m_lEquipItems.Add(item);
            item.SetData(dItemID, posIndex, m_soldierInfo);
            if (item.EquipPutType == EquipmentPutType.CanPut) {
                m_lCanEquips.Add(item);
                item.bGuideSelect = true;
            } else if (item.EquipPutType == EquipmentPutType.None && m_tNotCanEquip == null) {
                m_tNotCanEquip = item.transform;
                item.bGuideSelect = true;
            } else if ((item.EquipPutType == EquipmentPutType.ReadyCombine || item.EquipPutType == EquipmentPutType.NoCanCombine) && m_tCanGet == null) {
                m_tCanGet = item.transform;
                item.bGuideSelect = true;
            }
        }
    }
    /// <summary>
    /// 刷新装备（合成后需重新设置装备状态）
    /// </summary>
    public void RefreshEquipItems()
    {
        foreach (EquipmentItem item in m_lEquipItems) {
            Destroy(item.gameObject);
        }
        m_lEquipItems.Clear();
        GetEquimments();
    }
    
    /// <summary>
    /// 设置经验值
    /// </summary>
    private void SetExp()
    {
        int NeedExp = SoldierM.GetUpLevelNeedExp(m_soldierInfo.Level);
        MyHead.LblExp.text = string.Format("{0}/{1}", m_soldierInfo.EXP, NeedExp);
    }
    
    /// <summary>
    /// 设置灵魂石
    /// </summary>
    private void SetSoulPercentage()
    {
        MyHead.BtnShengXing.gameObject.SetActive(true);
        if (MyHead.SprSoulPercentage == null || MyHead.LblSoulNum == null) {
            NGUIUtil.DebugLog("SoldierInfoWnd.cs SprSoulPercentage or LblSoulNum null !!!");
            return;
        }
        if (m_soldierInfo.StarLevel == ConstantData.MaxStarLevel) {
            MyHead.SprSoulPercentage.fillAmount = 1.0f;
            MyHead.LblSoulNum.text = NGUIUtil.GetStringByKey("88800091");
            m_bStarLevelUp = false;
            MyHead.BtnShengXing.gameObject.SetActive(false);
        } else {
            //判断碎片满足
            int NeedCoin = 0 ;
            int NeedNum = 0;
            SoldierM.GetUpStarNeed(m_soldierInfo.SoldierTypeID, m_soldierInfo.StarLevel + 1, ref NeedNum, ref  NeedCoin);
            int Have = m_soldierInfo.GetHaveFragmentNum();//当前灵魂石
            MyHead.SprSoulPercentage.fillAmount = (Have * 1.0f) / NeedNum;
            MyHead.LblSoulNum.text = string.Format("{0}/{1}", Have, NeedNum);
            if (Have >= NeedNum) {
                //屏蔽文字可进化（王振鑫）
                //NGUIUtil.SetLableTextByCode<int>(MyHead.LblSoulNum, 88800106);
                if (m_soldierInfo.StarLevel < ConstantData.MaxStarLevel) {
                    m_bStarLevelUp = true;
                } else {
                    m_bStarLevelUp = false;
                }
            } else {
                //MyHead.LblSoulNum.text = string.Format("{0}/{1}", Have, NeedNum);
                m_bStarLevelUp = false;
            }
            
        }
    }
    /// <summary>
    /// 升星
    /// </summary>
    private void BtnShengXing_OnClickEventHandler(UIButton sender)
    {
        //判断碎片满足
        int NeedCoin = 0 ;
        int NeedNum = 0;
        SoldierM.GetUpStarNeed(m_soldierInfo.SoldierTypeID, m_soldierInfo.StarLevel + 1, ref NeedNum, ref  NeedCoin);
        int Have = m_soldierInfo.GetHaveFragmentNum();//当前灵魂石
        
        
        if (m_soldierInfo.StarLevel == ConstantData.MaxStarLevel) {
            NGUIUtil.ShowTipWndByKey(30000026);
            return;
        } else if (UserDC.GetCoin() < NeedCoin) {
            NGUIUtil.ShowTipWndByKey(10000086);
            return;
        } else if (NeedNum > Have) {
            NGUIUtil.ShowTipWndByKey(10000087);
            return;
        }
        
        DialogWnd dialogWnd = WndManager.GetDialog<DialogWnd>();
        if (dialogWnd) {
            //升星金币消耗是使用下一级的
            string upStarCoin = string.Format(NGUIUtil.GetStringByKey("30000025"), NeedCoin);
            dialogWnd.SetDialogLable(upStarCoin, NGUIUtil.GetStringByKey("10000044"), NGUIUtil.GetStringByKey("88800068"));
            dialogWnd.YESButtonOnClick = YesUpStar;
            dialogWnd.ShowDialog();
        }
    }
    
    private void BtnHelp(UIButton sender)
    {
        if (MyHead.SprTipsInfo.gameObject.activeInHierarchy == true) {
            MyHead.SprTipsInfo.gameObject.SetActive(false);
        } else {
            MyHead.SprTipsInfo.gameObject.SetActive(true);
        }
    }
    
    private void YesUpStar(UIButton sender)
    {
    
        if (m_bStarLevelUp) {
            if (UserDC.GetCoin() >= m_starInfo.price) {
                //NGUIUtil.ShowTipWnd("可以升星啦");
                //                MyHead.BtnShengXing.isEnabled = false;
                EnablePrevNextBtn(false);
                m_preSoldierInfo.Copy(m_soldierInfo);
                SoldierDC.Send_SoldierStarUpRequest(m_soldierInfo.ID);
            } else { //金钱不够
                NGUIUtil.ShowTipWndByKey<int>(88800108, 1.0f, ConstantData.iDepBefore3DModel);
            }
        } else { //灵魂石不够
            NGUIUtil.ShowTipWndByKey<int>(88800107, 1.0f, ConstantData.iDepBefore3DModel);
        }
    }
    /// <summary>
    /// 升阶
    /// </summary>
    void BtnJinSheng_OnClickEventHandler(UIButton sender)
    {
        int levelLimit = 0;
        int equipID1 = 0;
        int equipID2 = 0;
        int equipID3 = 0;
        int equipID4 = 0;
        int equipID5 = 0;
        int equipID6 = 0;
        if (ConfigM.GetNextQuality(m_soldierInfo.Quality) == 0) {
            NGUIUtil.ShowTipWndByKey("30000017", 1.0f, ConstantData.iDepBefore3DModel);
            return;
        }
        
        SoldierM.GetUpQualityNeed(m_soldierInfo.SoldierTypeID, m_soldierInfo.Quality, ref levelLimit,
            ref equipID1, ref equipID2, ref equipID3,
            ref equipID4, ref equipID5, ref equipID6);
            
        if (CheckUpEquip(levelLimit, equipID1, equipID2, equipID3, equipID4, equipID5, equipID6) == false) {
            NGUIUtil.ShowTipWndByKey(30000019);
            return;
        }
        //可以进阶啦，发送进阶请求
        EnablePrevNextBtn(false);
        DoEquipsFly();
        m_preSoldierInfo.Copy(m_soldierInfo);
        SoldierDC.Send_SoldierQualityUpRequest(m_soldierInfo.ID);
    }
    /// <summary>
    /// 执行装备晋升表现（1.2f之内）
    /// </summary>
    void DoEquipsFly()
    {
        Vector3 toPos = MyHead.EquipFlyPoint.position;
        float delay = 0;
        foreach (GameObject item in MyHead.SoldierEquipList) {
            GameObject goOriginal = item.transform.GetChild(0).gameObject;
            GameObject go = Instantiate(goOriginal, goOriginal.transform.position, goOriginal.transform.rotation) as GameObject;
            go.transform.parent = item.transform;
            go.transform.localScale = goOriginal.transform.localScale;
            if (go) {
                TweenPosition tp = go.AddComponent<TweenPosition>();
                tp.from = go.transform.position;
                tp.to = toPos;
                tp.worldSpace = true;
                tp.duration = 0.3f;
                tp.delay = delay;
                delay += 0.15f;
                tp.ignoreTimeScale = false;
                
                TweenScale ts = go.AddComponent<TweenScale>();
                ts.from = Vector3.one;
                ts.to = new Vector3(0.25f, 0.25f, 0.25f);
                ts.duration = 0.3f;
                ts.delay = delay;
                ts.ignoreTimeScale = false;
                
                StartCoroutine(EquipDisappear(go, delay + 0.3f));
            }
        }
        //NGUIUtil.PauseGame();
    }
    
    IEnumerator EquipDisappear(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(go);
    }
    
    /// <summary>
    /// 检测是否可进阶
    /// </summary>
    /// <returns></returns>
    bool CheckJinJie()
    {
        int levelLimit = 0;
        int equipID1 = 0;
        int equipID2 = 0;
        int equipID3 = 0;
        int equipID4 = 0;
        int equipID5 = 0;
        int equipID6 = 0;
        if (ConfigM.GetNextQuality(m_soldierInfo.Quality) == 0) {
            return false;
        }
        
        SoldierM.GetUpQualityNeed(m_soldierInfo.SoldierTypeID, m_soldierInfo.Quality, ref levelLimit,
            ref equipID1, ref equipID2, ref equipID3,
            ref equipID4, ref equipID5, ref equipID6);
            
        if (CheckUpEquip(levelLimit, equipID1, equipID2, equipID3, equipID4, equipID5, equipID6) == false) {
            return false;
        }
        return true;
    }
    
    /// <summary>
    /// 检测装备是否满足进阶条件
    /// </summary>
    bool CheckUpEquip(int levelLimit, int equipID0, int equipID1, int equipID2, int equipID3, int equipID4, int equipID5)
    {
        if (m_soldierInfo.Level < levelLimit) { //等级是否满足进阶要求
            //NGUIUtil.ShowFreeSizeTipWnd(string.Format(NGUIUtil.GetStringByKey("30000018"), levelLimit));
            return false;
        }
        int eID0 = ItemDC.GetItemID2ItemType(m_soldierInfo.Equipment0);
        int eID1 = ItemDC.GetItemID2ItemType(m_soldierInfo.Equipment1);
        int eID2 = ItemDC.GetItemID2ItemType(m_soldierInfo.Equipment2);
        int eID3 = ItemDC.GetItemID2ItemType(m_soldierInfo.Equipment3);
        int eID4 = ItemDC.GetItemID2ItemType(m_soldierInfo.Equipment4);
        int eID5 = ItemDC.GetItemID2ItemType(m_soldierInfo.Equipment5);
        
        if (eID0 != equipID0 || eID1 != equipID1 || eID2 != equipID2 ||
            eID3 != equipID3 || eID4 != equipID4 || eID5 != equipID5) {
            //NGUIUtil.ShowTipWndByKey("30000019", 1.0f, ConstantData.iDepBefore3DModel);
            return false;
        }
        return true;
    }
    /// <summary>
    ///  显示获取灵魂石窗口
    /// </summary>
    void BtnGetSoulStone_OnClickEventHandler(UIButton sender)
    {
        s_itemtypeInfo info = ItemM.GetItemInfo(m_soldierInfo.fragmentTypeID);
        if (info != null) {
            ItemComeFromWnd wnd = WndManager.GetDialog<ItemComeFromWnd>();
            if (wnd != null) {
                wnd.SetData(info, m_soldierInfo, null, 2);
            }
        }
    }
    
    void BtnClose_OnClickEventHandler(UIButton sender)
    {
    
        WndManager.DestoryDialog<ClickSkillDescWnd>();
        //关闭动画会有卡顿感觉，和UE沟通后改成直接关闭
        //WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
        DestoryDialogCallBack(null);
    }
    void DestoryDialogCallBack(object o)
    {
        WndManager.DestoryDialog<PdbycWnd>();
        WndManager.DestoryDialog<HeroIntroduceWnd>();
        WndManager.DestoryDialog<AddSkillWnd>();
        PdbbbWnd wnd = WndManager.FindDialog<PdbbbWnd>();
        if (wnd) {
            wnd.RefreshUI();
        }
    }
    
    private void LoadHeroIntorduct()
    {
        HeroIntroduceWnd wnd = WndManager.GetDialog<HeroIntroduceWnd>();
        if (wnd != null) {
            wnd.transform.parent = MyHead.GoParent.transform;
            wnd.transform.localPosition = MyHead.GoParent.transform.localPosition;
            wnd.transform.localRotation = MyHead.GoParent.transform.localRotation;
            wnd.transform.localScale = MyHead.GoParent.transform.localScale;
            wnd.SetData(m_soldierInfo);
        }
    }
    
    private void LoadAddSkillWnd()
    {
        AddSkillWnd wnd = WndManager.GetDialog<AddSkillWnd>();
        if (wnd != null) {
            wnd.transform.parent = MyHead.GoParent.transform;
            wnd.transform.localPosition = MyHead.GoParent.transform.localPosition;
            wnd.transform.localRotation = MyHead.GoParent.transform.localRotation;
            wnd.transform.localScale = MyHead.GoParent.transform.localScale;
            wnd.SetData(m_soldierInfo);
            
        }
    }
    void LoadCurrentToggle1()
    {
        if (MyHead.Toggle1.value) {
            Toggle1();
            return;
        }
        if (MyHead.Toggle2.value) {
            Toggle2();
            return;
        }
        if (MyHead.Toggle3.value) {
            Toggle3();
            return;
        }
    }
    
    /// <summary>
    /// 属性 (英雄介绍)
    /// </summary>
    public void Toggle1()
    {
        WndManager.DestoryDialog<ClickSkillDescWnd>();
        if (MyHead.Toggle1.value == false) {
            return;
        }
        MyHead.LblLocation.gameObject.SetActive(true);
        U3DUtil.DestroyAllChild(MyHead.GoParent, false);
        LoadHeroIntorduct();
    }
    /// <summary>
    /// 技能
    /// </summary>
    public void Toggle2()
    {
        WndManager.DestoryDialog<ClickSkillDescWnd>();
        if (MyHead.Toggle2.value == false) {
            return;
        }
        MyHead.LblLocation.gameObject.SetActive(false);
        U3DUtil.DestroyAllChild(MyHead.GoParent, false);
        LoadAddSkillWnd();
    }
    /// <summary>
    /// 图鉴(资源未出)
    /// </summary>
    public void Toggle3()
    {
        WndManager.DestoryDialog<ClickSkillDescWnd>();
        if (MyHead.Toggle3.value == false) {
            return;
        }
        MyHead.LblLocation.gameObject.SetActive(true);
        U3DUtil.DestroyAllChild(MyHead.GoParent, false);
        
    }
    
    /// <summary>
    /// 显示升星文字特效
    /// </summary>
    private void ShowStarAddGrow()
    {
        float sGrow = (m_soldierInfo.m_strength_grow - m_preSoldierInfo.m_strength_grow) * 0.01f;
        float aGrow = (m_soldierInfo.m_agility_grow - m_preSoldierInfo.m_agility_grow) * 0.01f;
        float iGrow = (m_soldierInfo.m_intelligence_grow - m_preSoldierInfo.m_intelligence_grow) * 0.01f;
        
        NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, NGUIUtil.GetStringByKey(88800044) + "+" + Mathf.Abs(sGrow).ToString(), Color.green, 0f);
        NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, NGUIUtil.GetStringByKey(88800046) + "+" + Mathf.Abs(aGrow).ToString(), Color.green, 0.8f);
        NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, NGUIUtil.GetStringByKey(88800045) + "+" + Mathf.Abs(iGrow).ToString(), Color.green, 0.8f);
    }
    
    /// <summary>
    /// 显示升阶文字特效
    /// </summary>
    private float ShowQualityAddGrow(SoldierInfo preInfo, SoldierInfo curInfo)
    {
        int strength = curInfo.m_strength - preInfo.m_strength;
        int intelligence = curInfo.m_intelligence - preInfo.m_intelligence;
        int agi = curInfo.m_agility - preInfo.m_agility;
        float duration = 0;
        if (strength > 0) {
            NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, NGUIUtil.GetStringByKey(88800044) + "+" + strength.ToString(), Color.green, duration);
            duration += 0.8f;
        }
        if (intelligence > 0) {
            NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, NGUIUtil.GetStringByKey(88800046) + "+" + intelligence.ToString(), Color.green, duration);
        }
        if (agi > 0) {
            NGUIUtil.AddHudTextShow(gameObject, MyHead.HudText, NGUIUtil.GetStringByKey(88800045) + "+" + agi.ToString(), Color.green, duration);
        }
        
        return 1.2f;
    }
    public void UpStarEffect()
    {
        for (int i = 0; i < MyHead.SprStars.Length; i++) {
            if (MyHead.SprStars[i].spriteName == "icon033") {
                Vector3 pos = MyHead.SprStars[i].transform.localPosition;
                GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000741",
                        pos, MyHead.SprStars[i].transform.parent);
                gae.gameObject.AddComponent<SetRenderQueue>();
                if (gae != null) {
                    GameObjectLoader.SetGameObjectLayer(gae.gameObject, MyHead.SprStars[i].transform.parent.gameObject.layer);
                    GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(2f);
                    gae.AddAction(ndEffect);
                    gae.gameObject.transform.localPosition = pos;
                }
                MyHead.SprStars[i].spriteName = "icon032";
                SoundPlay.Play("hero_promotion", false, false);
                break;
            }
        }
    }
    
    /// <summary>
    /// 升级特效
    /// </summary>
    public void UpLevelEffect()
    {
        Vector3 pos = new Vector3(0, -10, ConstantData.iDepBefore3DModel);
        GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000711",
                pos, MyHead.SoldierPos);
        gae.gameObject.AddComponent<SetRenderQueue>();
        
        if (gae != null) {
            GameObjectLoader.SetGameObjectLayer(gae.gameObject, MyHead.SoldierPos.gameObject.layer);
            GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(1.8f);
            gae.AddAction(ndEffect);
            gae.gameObject.transform.localPosition = pos;
        }
        SoundPlay.Play("hero_promotion", false, false);
    }
    
    public void OnDestroy()
    {
        AntiRegisterHooks();
    }
    public void SetEquipDataNoReady()
    {
        m_bCanEquipsReady = false;
    }
    
    public  void DoTaskEvent(int staskid, int step)
    {
        if (staskid == 50021) {
            if (step == 3 || step == 5 || step == 7 || step == 9 || step == 11 || step == 13) {
                if (m_lCanEquips.Count > 0) {
                    m_lCanEquips[0].BtnItem_OnClickEventHandler(null);
                }
            } else if (step == 15) {
                BtnJinSheng_OnClickEventHandler(null);
            } else if (step == 17) {
                BtnClose_OnClickEventHandler(null);
            }
        }
        if (staskid == 50039) {
            if (step == 0) {
                MyHead.Toggle2.value = true;
            }
        }
    }
    
    void Update()
    {
        if (MyHead.SprTipsInfo.gameObject.activeInHierarchy == true) {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                if (WndManager.IsHitNGUI(out hit) == true) {
                    if (hit.collider.gameObject == MyHead.BtnTips.gameObject) {
                        return ;
                    }
                    
                    if (hit.collider.gameObject != MyHead.SprTipsInfo.gameObject) {
                        MyHead.SprTipsInfo.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
}
