using UnityEngine;
using System.Collections;

public class BeiBaoItem : MonoBehaviour {

	private ItemTypeInfo Info;
	private int m_num ;

	private BeiBaoWnd m_parent;

	public BeiBaoItem_h MyHead
	{
		get
		{
			return GetComponent<BeiBaoItem_h>();
		}
	}

	public ItemTypeInfo GetInfo()
	{
		return Info;
	}

    public  void BindEvents()
    {
        if (MyHead.BtnSelect)
        {
            MyHead.BtnSelect.OnClickEventHandler += BtnSelect_OnClickEventHandler;   
        }
    }

    void BtnSelect_OnClickEventHandler(UIButton sender)
    {
		if (Info == null )
        {
            NGUIUtil.DebugLog("BeiBaoItem.cs Info == null!!!");
            return;
        }
		else
		{
			if(m_parent != null)
			  m_parent.ItemClickCallBack(Info);
		}
        
    }
	public void BtnToggValueChange()
	{
		if(MyHead.TogSelect.value)
		{
			NGUIUtil.SetSprite(MyHead.SprSelect,"icon101");
		}
		else
		{
			NGUIUtil.SetSprite(MyHead.SprSelect,"");
		}
	}

	public void SetData(ItemTypeInfo I ,int num ,BeiBaoWnd wnd)
	{
		Info= I ;
		m_num = num;
		m_parent = wnd;
	}

	// Use this for initialization
	void Start () 
    {
        SetUI();
		BindEvents ();
	}

    private void SetUI()
    {
		if (Info == null )
        {
            NGUIUtil.DebugLog("BeiBaoItem.cs Info == null!!!");
            return;
        }

		if(MyHead.SprType != null )
           NGUIUtil.Set2DSprite(MyHead.SprType, "Textures/item/",Info.m_Icon.ToString());
        //设置物品品阶
		if(MyHead.SprQuality != null)
			NGUIUtil.SetSprite(MyHead.SprQuality, ConfigM.GetBigQuality(Info.m_Quality).ToString());
        //设置物品数量
		if(MyHead.LblCounts != null)
			NGUIUtil.SetLableText<int>(MyHead.LblCounts, m_num);
    }
	
}
