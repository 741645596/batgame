using UnityEngine;
using System.Collections;

public class TrapUpQualityWnd : WndTopBase {

	// Use this for initialization.
	private BuildInfo m_buildInfo;
	private BuildInfo m_oldBuildInfo;

	private TrapShowWnd m_parent;

	public TrapUpQualityWnd_h MyHead
	{
		get
		{
			return (base.BaseHead () as TrapUpQualityWnd_h);
		}
	}
	public override void WndStart ()
	{
		base.WndStart ();
		transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y,ConstantData.iDepBefore3DModel);

		DoWndEffect ();
		MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;

	}
	public void SetData(BuildInfo info,BuildInfo oldInfo,TrapShowWnd wnd)
	{
		m_buildInfo = info;
		m_oldBuildInfo = oldInfo;
		m_parent = wnd;
		SetUI ();
	}
	private void SetUI()
	{

		MyHead.LblDefanNum.text = m_oldBuildInfo.m_DefensePower.ToString ();
		MyHead.LblAfterDefanNum.text = m_buildInfo.m_DefensePower.ToString ();

		MyHead.LblHpNum.text = m_oldBuildInfo.m_hp.ToString();
		MyHead.LblAfterHpNum.text = m_buildInfo.m_hp.ToString();

		if(MyHead.AfterItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.AfterItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(m_buildInfo,false);
					item.HiddenTrapNumLabel();
				}
			}
			
		}
		if(MyHead.PreItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.PreItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(m_oldBuildInfo,false);
					item.HiddenTrapNumLabel();
				}
			}
		}

		SetSkill ();

	}

	/// <summary>
	/// 设置 角色名称 + 彩色品质等级 + 设置角色品质框（背景） + 战斗力.
	/// </summary>
	/// <param name="name">角色名称 s_soldierType</param>
	/// <param name="quality">角色品质 d_soldier</param>
	public void SetSmallQuality(UILabel labl,int quality)
	{
		labl.text = NGUIUtil.GetSmallQualityStr(quality);
		
	}
	public void SetLevelSprite(UISprite spr,int quality)
	{
		int bigLevel = ConfigM.GetBigQuality(quality);
		NGUIUtil.SetSprite(spr, bigLevel.ToString());
	}
	public void SetSkill()
	{
		int oldBigQua = ConfigM.GetBigQuality (m_oldBuildInfo.Quality);
		int oldSmall = ConfigM.GetSmallQuality (m_oldBuildInfo.Quality);

		int bigQua = ConfigM.GetBigQuality (m_buildInfo.Quality);
		int Small = ConfigM.GetSmallQuality (m_buildInfo.Quality);
		if(bigQua > oldBigQua && Small == 0)
		{
			BuildSkillInfo afterInfo = SkillM.GetBuildSkill(m_buildInfo.BuildType,bigQua);
			if(afterInfo != null)
			{
				MyHead.LblAfterSkill.text = afterInfo.m_desc;
			}

			BuildSkillInfo befInfo = SkillM.GetBuildSkill(m_oldBuildInfo.BuildType,oldBigQua);
			if(befInfo != null)
			{
				MyHead.LblForeSkill.text = befInfo.m_desc;
			}

			if(befInfo != null && afterInfo != null)
			{
				MyHead.GoSkill.SetActive(true);
			}
			else
			{
				MyHead.GoSkill.SetActive(false);
			}
		}
		else
		{
			MyHead.GoSkill.SetActive(false);
		}
	}

	private void BtnClose_OnClickEventHandler(UIButton sender)
	{
		m_parent.SetEnableQuality(false);
		WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
	}
	void DestoryDialogCallBack(object o)
	{
		WndManager.DestoryDialog<TrapUpQualityWnd>();		
	}

	void DoWndEffect()
	{
		
		WndEffects.DoWndEffect(gameObject);
		
	}
	void Update()
	{
		UIPanel panel = GetComponent<UIPanel>();
//		MyHead.bg.SetMeshRender(panel.startingRenderQueue);
//		MyHead.bg.SetParticleRender(panel.startingRenderQueue);
		
	}

}
