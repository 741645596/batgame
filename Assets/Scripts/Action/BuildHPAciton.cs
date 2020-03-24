using UnityEngine;
using System.Collections;

public class BuildHPAciton : HPAciton {
	public SpriteRenderer sRender;
	public bool bAlwayShow = false;
	public TextMesh BearText;
	public Transform BearPlane;
	public GameObject BearParent;
	public int nBear = 0;
	public Texture2D beartex1;
	public Texture2D beartex2;
	public Texture2D beartex3;
	public override void CreateForeceSprite()
	{
		float fPercent = value;
		//backGround = sRender.transform.parent.gameObject;
		//forceGrond = sRender.gameObject;
		if (!bAlwayShow)
		{
			if(!isShowing)
			{
				isShowing = true;
				showTime = duration;
				cumulativeTime = 0;
			}
			else
			{
				showTime  += duration - fadeTime *2;
			}
		}
		else
		{
			isShowing = false;
		}
		if (sRender != null&&forceTex!=null&&m_nFullHp>0)
		{
			CalcBear();
			/*int nGridHP = ConfigM.GetBuildHPType();
			int nMaxGrid = m_nFullHp/nGridHP;
			int nHpTemp = m_nFullHp%nGridHP;
			float fHpGridPercent = ((float)nHpTemp)/((float)nGridHP);
			if(nMaxGrid>20)
			{
					nMaxGrid = 20;
					fHpGridPercent = 0;
			}
			float fForceTexWidth = ((float)(nMaxGrid+fHpGridPercent-1)*10f+9f);
			float fScaleX= 159f/fForceTexWidth;
			//if(fScaleX<1.0f)
			//	fScaleX = 1.0f;
			sRender.transform.localScale = new Vector3(-fScaleX,1.0f,1.0f);
			Sprite spriteTemp = sRender.sprite;
			sRender.sprite = Sprite.Create(forceTex,new Rect(0,0,fForceTexWidth* fPercent,forceTex.height),new Vector2(0,0.5f));
			if(spriteTemp!=null)
				Destroy(spriteTemp);*/
		}
	}
	public override void CreateMidSprite()
	{
	}
	public override void Start () {
		base.Start ();
		}
	public override void  Update () 
	{
		if (!bAlwayShow)
		{
			base.Update ();
			if (isShowing != BearText.gameObject.activeSelf)
				BearText.gameObject.SetActive(isShowing);
		}
	}

	public void SetRoomID(int nRoomID,int hp,int nFullHP,int bear)
	{
		m_nHp = hp;
		m_nFullHp = nFullHP;
		preValue = value = 1;
		nBear = bear;
		BearParent.SetActive(true);
		backGround = BearParent.transform.Find("bg").gameObject;
		forceGrond = BearPlane.gameObject;
		CreateForeceSprite ();
		ManualShowHp (false);
		showTime = 0;
		cumulativeTime = 0;
		isShowing = false;

	}
	public override void SetPlayer(bool IsPlayer)
	{
		base.SetPlayer (IsPlayer);

		CreateForeceSprite ();
	}

	public void ManualShowHp(bool bshow)
	{
		
		Color c = new Color() ;
		if(backGround!=null)
			c = backGround.GetComponent<Renderer>().material.color;
		c.a = bshow?1:0; //屏蔽血条alpha
		if(backGround!=null)
			backGround.GetComponent<Renderer>().material.color = c;
		if(forceGrond!=null)
			forceGrond.GetComponent<Renderer>().material.color = c;
		BearText.gameObject.SetActive(bshow);
		CalcBear();
	}
	public void CalcBear()
	{
		float curbear =  m_nHp < 0 ? 0 : m_nHp * 1.0f/ m_nFullHp;
		int curb = Mathf.CeilToInt(nBear * curbear);
		curb = curb < 0 ? 0 : curb;
		BearText.text = curb.ToString();
		BearPlane.localScale = new Vector3(1,1,curbear);
		BearPlane.localPosition = new Vector3(0,0.5f,(1-curbear)/0.2f);
		BearPlane.GetComponent<Renderer>().material.mainTextureScale = new Vector2(1,curbear);
		if (curbear > 0.66f)
			BearPlane.GetComponent<Renderer>().material.mainTexture = beartex1;
		else if (curbear > 0.33f)
			BearPlane.GetComponent<Renderer>().material.mainTexture = beartex2;
		else
			BearPlane.GetComponent<Renderer>().material.mainTexture = beartex3;
	}
	public void ShowAlwayHp(bool bShow)
	{
		bAlwayShow = bShow;
		if (bAlwayShow)
			ManualShowHp(true);
		else
		{
			ManualShowHp(false);
		}
	}
}
