using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapUplevWnd : WndBase {


	private BuildInfo m_build;
	private TrapShowWnd m_parentwnd = null;
	//需要的材料
	private Dictionary<int ,int> m_lNeed = new Dictionary<int ,int>();
	//需要的金币
	private int m_NeedCoin = 0;
	private int m_NeedWood = 0;
	private int m_HaveCoin = 0;
	private int m_HaveWood = 0;


	public TrapUplevWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TrapUplevWnd_h);
		}
	}

	public void SetData(BuildInfo Info ,TrapShowWnd wnd)
	{
		m_build = Info;
		m_parentwnd = wnd;
		GetNeedItem();
	}

	public override void WndStart ()
	{
		base.WndStart ();

		transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y,ConstantData.iDepBefore3DModel);

		RegisterHooks();
		MyHead.BtnUpLevle.OnClickEventHandler += OnUpLevle;
		MyHead.BtnCancel.OnClickEventHandler += OnCancle;
		SetUI(m_build) ;
		SetData ();


	}

	private void SetData()
	{
		MyHead.LblHardNum.text = m_build.m_Solidity.ToString();
		MyHead.LblStrengthNum.text = m_build.m_Intensity.ToString();
		MyHead.LblResilienceNum.text = m_build.m_Tenacity.ToString();
		MyHead.LblCurLv.text = NGUIUtil.GetStringByKey(88800038)+m_build.Level.ToString ();
		MyHead.LblNextLv.text = NGUIUtil.GetStringByKey(88800038)+ (m_build.Level + 1).ToString ();
		MyHead.LblTrapName.text = m_build.m_name + NGUIUtil.GetStringByKey (10000046);
	}
	private void OnCancle(UIButton sender)
	{
		WndManager.DestoryDialog<TrapUplevWnd>();
	}

	private void OnUpLevle(UIButton sender)
	{
		SetUpLevelNeed();
	}

	/// <summary>
	/// 设置升星
	/// </summary>
	private void SetUI(BuildInfo Info)
	{
		if(Info == null) return ;
		SetLevelUI(Info.StarLevel);
		SetBuildProperty(Info);

		GameObject goFirst = NDLoad.LoadWndItem("NeedItemNumItem", MyHead.TabTable.transform);
		NeedItemNumItem  item = goFirst.GetComponent<NeedItemNumItem>();
		item.SetData (m_HaveCoin ,m_NeedCoin ,1);
		
		GameObject goTwo = NDLoad.LoadWndItem("NeedItemNumItem", MyHead.TabTable.transform);
		NeedItemNumItem  itemTwo = goTwo.GetComponent<NeedItemNumItem>();
		itemTwo.SetData ( m_HaveWood ,m_NeedWood,2);
	}

	/// <summary>
	/// 设置升级
	/// </summary>
	private void SetLevelUI(int level)
	{
		
	}

	/// <summary>
	/// 设置三围
	/// </summary>
	private void SetBuildProperty(BuildInfo Info)
	{
		if(Info == null)
			return ;
		//硬度
		//强度
		//韧性
	}

	/// <summary>
	/// 设置升级需要材料
	/// </summary>
	private void SetUpLevelNeed()
	{
		if(m_HaveCoin >= m_NeedCoin && m_HaveWood >= m_NeedWood)
		{
			BuildDC.Send_BuildlevelUpRequest(m_build.ID);
		}
		else //弹出提示框
		{
            NGUIUtil.ShowFreeSizeTipWnd(NGUIUtil.GetStringByKey(10000073),null,0, 2f, ConstantData.iDepBefore3DModel);
		}
	}


	/// <summary>
	/// 确认能否升星
	/// </summary>
	private void GetNeedItem()
	{
		if(m_build == null) return ;
		//获取升级需要的资源。
		int NeedCoin = 0;
		buildingM.GetUpLevelNeed(m_build.BuildType,m_build.Level + 1,ref m_NeedCoin ,ref m_NeedWood) ;
		//获取已有的资源。
		m_HaveCoin = UserDC.GetCoin() ;
		m_HaveWood = UserDC.GetWood() ;
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
		DataCenter.RegisterHooks((int)gate.Command.CMD.CMD_902, TrapLevelUp_Resp);
	}
    
	/// <summary>
	/// 反注册事件
	/// </summary>
	public   void AntiRegisterHooks()
	{
		DataCenter.AntiRegisterHooks((int)gate.Command.CMD.CMD_902, TrapLevelUp_Resp);
	}


	/// <summary>
	/// 902 升级
	/// </summary>
	/// <param name="nErrorCode"></param>
	void TrapLevelUp_Resp(int nErrorCode)
	{
		if (nErrorCode==0)
		{
			WndManager.DestoryDialog<TrapUplevWnd>();
			m_parentwnd.UpLevelEffect();
		}
		
	}
}
