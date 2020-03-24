using UnityEngine;
using System.Collections;

public class HeadItem : MonoBehaviour
{
	public UISprite SprHeadQuality;
	public UI2DSprite Spr2DHeadIcon;
	public UISprite SprHeadChoosed;
	public Transform ScaleRoot;
	public UISprite SprBoss;

	public UIEventListener BtnSelect;

	int mHeadID;
	int mHeadFrameID;
	int mHeadChartID;

	HeadChooseWnd mParent = null;
	public void Init(HeadChooseWnd parent)
	{
		mParent = parent;
	}

	public float Scale
	{
		set
		{
			ScaleRoot.localScale = Vector3.one * value;
		}
	}

	public bool IsBoss
	{
		set
		{
			SprBoss.gameObject.SetActive(value);
		}
	}

	// Use this for initialization
	void Awake ()
	{
		UserInfo userInfo = UserDC.GetPlayer();
		mHeadID = userInfo.HeadID;
		mHeadFrameID = userInfo.HeadFrameID;
		mHeadChartID = userInfo.HeadChartID;

		SprHeadChoosed.gameObject.SetActive(false);

		if (BtnSelect)
			BtnSelect.onClick = ClickSelect;
	}

	void ClickSelect(GameObject go)
	{
		mParent.SelectItem(this);
		UserDC.Send_HeadModifyRequest(mHeadID, mHeadFrameID, mHeadChartID);
	}

	public void SetSelectState(bool selected)
	{
		SprHeadChoosed.gameObject.SetActive(selected);
	}

	public void ShowPortrait(PortraitPartType portraitPartType, int portraitID)
	{
		Reset();
		if ((portraitPartType & PortraitPartType.Portrait) == PortraitPartType.Portrait)
		{
			mHeadID = portraitID ;
			NGUIUtil.Set2DSprite(Spr2DHeadIcon, "Textures/role/", portraitID.ToString());
			Spr2DHeadIcon.gameObject.SetActive(true);
			SprHeadQuality.gameObject.SetActive(true);
		}
		if ((portraitPartType & PortraitPartType.Frame) == PortraitPartType.Frame)
		{
			mHeadFrameID = portraitID;
			SprHeadQuality.spriteName = portraitID .ToString();
			SprHeadQuality.gameObject.SetActive(true);
		}
		//if ((portraitPartType | PortraitPartType.Background) == PortraitPartType.Background)
		//{
		//	mHeadID = portraitID;
		//	SprHeadQuality.spriteName = portraitID.ToString();
		//	SprHeadQuality.gameObject.SetActive(true);
		//}
	}

	public void Reset()
	{
		SprHeadQuality.gameObject.SetActive(false);
		Spr2DHeadIcon.gameObject.SetActive(false);
		SprHeadChoosed.gameObject.SetActive(false);
		SprBoss.gameObject.SetActive(false);
	}
}
