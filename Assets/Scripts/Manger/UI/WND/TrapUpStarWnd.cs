using UnityEngine;
using System.Collections;

public class TrapUpStarWnd : WndTopBase {

	private BuildInfo m_build;
	private BuildInfo m_backUpInfo;
	private TrapShowWnd m_parent = null;
	public TrapUpStarWnd_h MyHead
	{
		get 
		{
			return (base.BaseHead () as TrapUpStarWnd_h);
		}
	}

	public override void WndStart () {
		base.WndStart ();
	
		transform.localPosition = new Vector3 (transform.localPosition.x,transform.localPosition.y,ConstantData.iDepBefore3DModel);
		MyHead.BtnClose.OnClickEventHandler += BtnClose_OnClickEventHandler;
		MyHead.BtnConfirm.OnClickEventHandler += BtnClose_OnClickEventHandler;

		DoWndEffect ();
	}


	void DestoryDialogCallBack(object o)
	{
		m_parent.SetEnableQuality(false);
		m_parent.UpReSetUI ();
		WndManager.DestoryDialog<TrapUpStarWnd>();	
	}
	
	void DoWndEffect()
	{
		
		WndEffects.DoWndEffect(gameObject);
		
	}

	private void BtnClose_OnClickEventHandler(UIButton sender)
	{
		WndEffects.DoCloseWndEffect(gameObject,DestoryDialogCallBack);
	}


	public void SetData(BuildInfo Info,BuildInfo BackInfo,TrapShowWnd wnd)
	{
		m_build = Info;
		m_backUpInfo = BackInfo;
		m_parent = wnd;
		SetUI();
	}

	/// <summary>
	/// 设置升星.
	/// </summary>
	private void SetUI()
	{
		if(m_build == null) return ;

		SetBuildProperty();

	}
	public void SetLevelSprite(UISprite spr,int quality)
	{
		int bigLevel = ConfigM.GetBigQuality(quality);
		NGUIUtil.SetSprite(spr, bigLevel.ToString());
	}

	/// <summary>
	/// 计算升星之后硬度增加.
	/// </summary>
	private void SetBuildProperty()
	{
		//硬度.
		float iSolidGrow = 0; 
		float iIntensityGrow = 0; 
		float iTenacityGrow = 0;
		buildingM.GetStarInfoGrow(m_backUpInfo,ref iSolidGrow,ref iIntensityGrow,ref iTenacityGrow);
		MyHead.LblSolidNum.text = iSolidGrow.ToString ("0.00");
		MyHead.LblTenaNum.text = iTenacityGrow.ToString ("0.00");
		MyHead.LblStrengNum.text = iIntensityGrow.ToString ("0.00");
	
		if(MyHead.AfterItem != null)
		{
			GameObject go = NDLoad.LoadWndItem("CanvasItem", MyHead.AfterItem.transform);
			if (go)
			{
				CanvasItem item = go.GetComponent<CanvasItem>();
				if (item)
				{
					item.SetCanvasItem(m_build,false);
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
					item.SetCanvasItem(m_backUpInfo,false);
					item.HiddenTrapNumLabel();
				}
			}
		}

		float iAfterSolidGrow = 0; 
		float iAfterIntensityGrow = 0; 
		float iAfterTenacityGrow = 0;
		buildingM.GetStarInfoGrow(m_build,ref iAfterSolidGrow,ref iAfterIntensityGrow,ref iAfterTenacityGrow);
		MyHead.LblSolidAfterNum.text = iAfterSolidGrow.ToString ("0.00");
		MyHead.LblTenaAfterNum.text = iAfterTenacityGrow.ToString ("0.00");
		MyHead.LblStrengAfterNum.text = iAfterIntensityGrow.ToString ("0.00");
	
		int oriStar = buildingM.GetMinBuildStar (m_build.BuildType);

		BuildInfo oriInfo = new BuildInfo();
		oriInfo.BuildType = m_build.BuildType;
		oriInfo.StarLevel = oriStar;

		float oriSolidGrow = 0;
		float oriIntensityGrow = 0; 
		float oriTenacityGrow = 0;
		buildingM.GetStarInfoGrow(oriInfo,ref oriSolidGrow,ref oriIntensityGrow,ref oriTenacityGrow);

		float iSolidAddNum = (iAfterSolidGrow - oriSolidGrow) +(iAfterSolidGrow - iSolidGrow) * (m_build.Level-1);

		float iSTenaAddNum = (iAfterTenacityGrow - oriTenacityGrow) + (iAfterTenacityGrow - iTenacityGrow) * (m_build.Level-1);

		float iStrengAddNum = (iAfterIntensityGrow - oriIntensityGrow) + (iAfterIntensityGrow - iIntensityGrow) * (m_build.Level-1);

		MyHead.LblSolidAfterAddNum.text = "(" +NGUIUtil.GetStringByKey(10000077) +"+"+iSolidAddNum.ToString("0.00")+")";
		MyHead.LblTenaAfterAddNum.text = "(" +NGUIUtil.GetStringByKey(10000079) +"+"+iSTenaAddNum.ToString("0.00")+")";
		MyHead.LblStrengAfterAddNum.text = "(" +NGUIUtil.GetStringByKey(10000078) +"+"+iStrengAddNum.ToString("0.00")+")";
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
	void Update()
	{
		UIPanel panel = GetComponent<UIPanel>();
//		MyHead.bg.SetMeshRender(panel.startingRenderQueue);
//		MyHead.bg.SetParticleRender(panel.startingRenderQueue);

	}

}
