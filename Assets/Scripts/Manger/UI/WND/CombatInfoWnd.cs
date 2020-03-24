using UnityEngine;
using System.Collections;
using Logic;

public enum CombatInfoMode {
    view  = 0,
    combat = 1,
}

public class CombatInfoWnd : WndBase
{

    public CombatInfoWnd_h MyHead {
        get
        {
            return (base.BaseHead() as CombatInfoWnd_h);
        }
    }
    
    private bool m_bTweenColor = true;
    //ShipDamage
    private int m_iOriginalDamage = 0;
    private const int COMBAT_LIMIT = 180;// 每场战斗的限定时间
    
    private static float m_fCurTimeScale = 1.0f;
    
    private int m_iCoin;
    private int m_iWood;
    
    public static new string DialogIDD()
    {
        return "CombatInfoWnd";
    }
    
    public override void WndStart()
    {
        base.WndStart();
        
        MyHead.BtnPause.OnClickEventHandler += BtnPause_OnClickEventHandler;
        InvokeRepeating("SetLeftTime", 0.5f, 0.5f);
    }
    
    /// <summary>
    /// 设置倒计时UI
    /// </summary>
    void SetLeftTime()
    {
        int time = CombatScheduler.CombatTime;
        SetCombatTime(time);
    }
    /// <summary>
    /// 退出战斗
    /// </summary>
    void BtnPause_OnClickEventHandler(UIButton sender)
    {
        m_fCurTimeScale = Time.timeScale;
        CombatScheduler.PauseCombat();
        
        CombatSchedulerWnd wnd = WndManager.GetDialog<CombatSchedulerWnd>();
        
        //DialogWnd dialogWnd = WndManager.GetDialog<DialogWnd>();
        //if (dialogWnd)
        //{
        
        //	dialogWnd.SetDialogLable(NGUIUtil.GetStringByKey("88800066"), NGUIUtil.GetStringByKey("88800063"), NGUIUtil.GetStringByKey("88800064"));
        //	dialogWnd.YESButtonOnClick = YesCombatExit;
        //	dialogWnd.NOButtonOnClick = NoCombatExit;
        //	dialogWnd.ShowDialog();
        //}
    }
    
    private  void YesCombatExit(UIButton sender)
    {
        BSC.AntiAllRegisterHooks();
        Time.timeScale = m_fCurTimeScale;
        if (BattleEnvironmentM.GetBattleEnvironmentMode() == BattleEnvironmentMode.CombatPVE) {
            MainTownInit.s_currentState = MainTownState.StageMap ;
            //主动退出的时候需要发送事件
            StageDC.SendStageSettleRequest(StageDC.GetCompaignStageID(), null, null, true, false) ;
        }
        CombatWnd wnd = WndManager.FindDialog<CombatWnd>();
        if (wnd) {
            wnd.DestroyGuideFireEffect();
        }
        SceneM.Load(MainTownScene.GetSceneName(), false, null, false);
    }
    private  void NoCombatExit(UIButton sender)
    {
        Time.timeScale = m_fCurTimeScale;
        CombatScheduler.ResumeCombat();
    }
    
    public void SetWndMode(CombatInfoMode Mode)
    {
        if (MyHead.EnemyResourceTip) {
            string str = CmCarbon.DefenderInfo.Name;
            MyHead.EnemyResourceTip.text = str;
        }
        if (Mode == CombatInfoMode.view) {
            ShowUI(false);
            SetGold();
            SetWood();
            SetName();
            SetItem(CmCarbon.GetWinItem());
        } else {
            ResetUI();
            SetCombatTime(COMBAT_LIMIT);
        }
    }
    
    /// <summary>
    /// Restart 后重置UI
    /// </summary>
    public  void ResetUI()
    {
        SetGold();
        SetWood();
        SetName();
        MyHead.HourGlass.SetActive(true);
        SetItem(CmCarbon.GetWinItem());
    }
    
    public void HideMoneyEffect()
    {
        if (MyHead.OurMoneyPos) {
            Transform effect = MyHead.OurMoneyPos.transform.GetChild(0);
            if (effect) {
                effect.gameObject.SetActive(false);
            }
        }
        if (MyHead.EnemyMoneyEffect) {
            MyHead.EnemyMoneyEffect.gameObject.SetActive(false);
        }
    }
    
    
    public void TweenLeftTimeColor()
    {
        if (m_bTweenColor) {
            m_bTweenColor = false;
            if (MyHead.HourGlassLeftTime) {
                TweenColor tc = TweenColor.Begin(MyHead.HourGlassLeftTime.gameObject, 0.5f, Color.red);
                tc.style = UITweener.Style.PingPong;
            }
        }
    }
    /// <summary>
    /// 设置 战斗倒计时和船体损坏 显示和隐藏
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowUI(bool isShow)
    {
        MyHead.BG.SetActive(isShow);
        MyHead.HourGlass.SetActive(isShow);
    }
    
    public void StopTweenLeftTimeColor()
    {
        if (MyHead.HourGlassLeftTime) {
            TweenColor tc = MyHead.HourGlassLeftTime.GetComponent<TweenColor>();
            if (tc) {
                Destroy(MyHead.HourGlassLeftTime.GetComponent<TweenColor>());
            }
        }
    }
    
    
    public void SetCombatTime(int second)
    {
        if (second <= 15) {
            TweenLeftTimeColor();
        }
        if (second == 1) {
            StopTweenLeftTimeColor();
        }
        
        if (MyHead.HourGlassLeftTime != null) {
            MyHead.HourGlassLeftTime.text = NdUtil.TimeFormat(second);
        }
    }
    
    public  void SetName()
    {
        if (MyHead.LblUserName != null) {
            MyHead.LblUserName.text = UserDC.GetName() + NGUIUtil.GetStringByKey(88800038) +  UserDC.GetLevel().ToString();
        }
        
        
        if (MyHead.LblEnemyName != null) {
            MyHead.LblEnemyName.text = CmCarbon.DefenderInfo.Name + NGUIUtil.GetStringByKey(88800038) + CmCarbon.DefenderInfo.Level.ToString();
        }
        
    }
    
    public  void SetCombatWood(int wood, bool IsMy)
    {
        if (IsMy) {
            if (MyHead.OurResourceEnergy != null) {
                wood =  m_iWood + CmCarbon.GetWinWood();
                SetNum(wood, MyHead.OurResourceEnergy);
            }
        } else {
            if (MyHead.EnemyResourceEnergy != null) {
                wood = CmCarbon.DefenderInfo.Wood - CmCarbon.GetWinWood();
                if (wood < 0) {
                    wood = 0;
                }
                SetNum(wood, MyHead.EnemyResourceEnergy);
            }
        }
    }
    
    
    /*战斗金币设置*/
    public  void SetCombatGold(int gold, bool IsMy = true)
    {
        if (IsMy) {
            if (MyHead.OurResourceGold != null) {
                gold =  m_iCoin + CmCarbon.GetWinGold();
                SetNum(gold, MyHead.OurResourceGold);
            }
        } else {
            if (MyHead.EnemyResourceGold != null) {
                gold = CmCarbon.DefenderInfo.Coin - CmCarbon.GetWinGold();
                if (gold < 0) {
                    gold = 0;
                }
                SetNum(gold, MyHead.EnemyResourceGold);
            }
        }
    }
    
    
    public void SetCombatItem()
    {
        if (MyHead.OurResourceItem != null) {
            int Item =  CmCarbon.GetWinItem();
            SetNum(Item, MyHead.OurResourceItem);
        }
    }
    
    
    public  void SetItem(int Item)
    {
        if (MyHead.OurResourceItem != null) {
            SetNum(Item, MyHead.OurResourceItem);
        }
    }
    
    /*初始化时金币设置*/
    public  void SetGold()
    {
        m_iCoin = UserDC.GetCoin();
        
        if (MyHead.OurResourceGold != null) {
            SetNum(m_iCoin, MyHead.OurResourceGold);
        }
        
        if (MyHead.EnemyResourceGold != null) {
            SetNum(CmCarbon.DefenderInfo.Coin, MyHead.EnemyResourceGold);
        }
    }
    /*初始化木头设置*/
    private void SetWood()
    {
        m_iWood = UserDC.GetWood();
        if (MyHead.OurResourceEnergy != null) {
            SetNum(m_iWood, MyHead.OurResourceEnergy);
        }
        
        if (MyHead.EnemyResourceEnergy != null) {
            SetNum(CmCarbon.DefenderInfo.Wood, MyHead.EnemyResourceEnergy);
        }
    }
    
    private void SetNum(int num, UILabel label)
    {
        NGUIUtil.SetMoneyNumType(num, label);
    }
    /// <summary>
    /// 获取物品掉落位置
    /// </summary>
    public  Transform GetResourcePos(ResourceType Type)
    {
        if (CmCarbon.GetCamp2Player(LifeMCamp.ATTACK) == true) {
            if (Type == ResourceType.Gold) {
                return MyHead.OurMoneyPos;
            } else if (Type == ResourceType.Wood) {
                return MyHead.OurWoodPos;
            } else if (Type == ResourceType.Box) {
                return MyHead.OurBoxPos;
            }
        } else {
            if (Type == ResourceType.Gold) {
                return MyHead.EmemyMoneyPos;
            } else if (Type == ResourceType.Wood) {
                return MyHead.EmemyWoodPos;
            } else if (Type == ResourceType.Box)
                //return MyHead.EmemyBoxPos;
            {
                return MyHead.OurBoxPos;
            }
        }
        
        return null;
    }
    
    public void PlayResourceAni(ResourceType Type)
    {
        if (Type == ResourceType.Gold) {
            MyHead.OurMoneyPos.GetComponent<Animation>().Play();
        } else if (Type == ResourceType.Wood) {
            MyHead.OurWoodPos.GetComponent<Animation>().Play();
        } else if (Type == ResourceType.Box) {
            MyHead.OurBoxPos.GetComponent<Animation>().Play();
        }
        Transform t = GetResourcePos(Type);
        GameObjectActionExcute gae = EffectM.LoadEffect(EffectM.sPath, "2001321", t.position, t);
        GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
        gae.AddAction(ndEffect);
    }
    public void HidePauseButton()
    {
        MyHead.BtnPause.gameObject.SetActive(false);
    }
    
    
    
}
