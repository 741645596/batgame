using UnityEngine;
using System.Collections;

public class HPAciton : MonoBehaviour {
	public GameObject prefab;
	public Transform target;
	public HUDText mText = null;
	public HUDText mBaojiText;
	public HUDText mAngerText;
	public HUDText DodgeText;	// 闪避
	public HUDText BuffText;	// Buff
	public GameObject backGround;
	public GameObject forceGrond;
	public GameObject midGrond;
	public float value = 1;
	public float alpha = 0;
	protected float preValue= 1;
	protected float yellowSpeed;
	public bool isShowing;
	protected bool isShowingAnger;
	public float fadeTime = 0.5f;
	public float duration = 1.5f;
	public float midTime = 0.0f;
	protected float showTime;
	protected float cumulativeTime;
	public Texture2D midTex;
	public Texture2D hp3;
	public Texture2D hp4;
	protected Texture2D forceTex;
	public float JumbTextDuration = 0.5f;
	protected int m_nHp;
	protected int m_nFullHp;

	//0xd3fe
	Color mColorBuff;
	Color mColorDebuff;
    

	public static float s_startz = -66.4297f;
	// Use this for initialization
	public virtual void Start () {

      
		//m_nHp = m_nFullHp = 100;
		//preValue = value = 1;
		GameObject child = NGUITools.AddChild(WndManager.GetWndRoot(), prefab);
		mText = child.GetComponentInChildren<HUDText>();

		if(null==forceTex)
			forceTex = hp4;

		//forceGrond.GetComponent<SpriteRenderer>().sprite.border
		// Make the UI follow the target
		child.AddComponent<UIFollowTarget>().target = target;
		child.GetComponent<UIFollowTarget>().disableIfInvisible = false;
		
		if(mBaojiText != null)
		{
			child = NGUITools.AddChild(WndManager.GetWndRoot(), mBaojiText.gameObject);
			mBaojiText = child.GetComponentInChildren<HUDText>();
			child.AddComponent<UIFollowTarget>().target = target;
			child.GetComponent<UIFollowTarget>().disableIfInvisible = false;
		}
		if(mAngerText != null)
		{
			child = NGUITools.AddChild(WndManager.GetWndRoot(), mAngerText.gameObject);
			mAngerText = child.GetComponentInChildren<HUDText>();
			child.AddComponent<UIFollowTarget>().target = target;
			child.GetComponent<UIFollowTarget>().disableIfInvisible = false;
		}
		if (BuffText != null)
		{
			child = NGUITools.AddChild(WndManager.GetWndRoot(), BuffText.gameObject);
			BuffText = child.GetComponentInChildren<HUDText>();
			child.AddComponent<UIFollowTarget>().target = target;
			child.GetComponent<UIFollowTarget>().disableIfInvisible = false;
		}
		mColorBuff = NGUIText.ParseColor("d3fe78", 0);
		mColorDebuff = NGUIText.ParseColor("ff5163", 0);
	}

	// Update is called once per frame
	public virtual void  Update () {
		//backGround.GetComponent<SpriteRenderer>().sprite.
        Color c = new Color() ;
        if (isShowing)
        {
            cumulativeTime += Time.deltaTime;
            if (cumulativeTime > (showTime - fadeTime))
            {
                alpha = 1;//(showTime - cumulativeTime) / fadeTime;
                if (cumulativeTime > showTime)
                {
                    isShowing = false;
                    alpha = 0;
                }
            }
            else
                alpha = cumulativeTime / fadeTime;
            if (preValue > value)
                preValue -= Time.deltaTime * yellowSpeed;
			if(backGround!=null)
           		c = backGround.GetComponent<Renderer>().material.color;
            //alpha = 1;
            c.a = alpha > 1?1:alpha; //屏蔽血条alpha
			
			if(backGround!=null)
				backGround.GetComponent<Renderer>().material.color = c;
			if(forceGrond!=null)
				forceGrond.GetComponent<Renderer>().material.color = c;
			if(midGrond!=null)
            	midGrond.GetComponent<Renderer>().material.color = c;
			CreateMidSprite();
            //midGrond.transform.localScale = new Vector3(preValue,1,1);
            //forceGrond.transform.localScale = new Vector3(value,1,1);
            /*	int width = forceGrond.GetComponent<SpriteRenderer>().sprite.texture.width;
                int height = forceGrond.GetComponent<SpriteRenderer>().sprite.;
                forceGrond.GetComponent<SpriteRenderer>()*/
            if (mText != null)
            {
                //mText.SetFont((int)(500f * (60f / Camera.main.fieldOfView) * 0.4 / Mathf.Abs(Camera.main.transform.position.z)));
				mText.SetScale((15f / Camera.main.fieldOfView)  * s_startz/ Camera.main.transform.position.z);
            }
            if (mBaojiText != null)
            {
				mBaojiText.SetScale((15f / Camera.main.fieldOfView) * s_startz/ Camera.main.transform.position.z);
            }

            //mText.fontSize = (int)(135f/ Mathf.Abs(Camera.main.transform.position.z));
            //Debug.Log((135f/ Mathf.Abs(Camera.main.transform.position.z)) + "," + Camera.main.transform.position + Camera.main.gameObject);
        }

		if (mAngerText != null)
		{
			mAngerText.SetScale((15f/Camera.main.fieldOfView)* s_startz/ Camera.main.transform.position.z);
		}
		if (BuffText != null)
		{
			BuffText.SetScale((15f / Camera.main.fieldOfView) * s_startz / Camera.main.transform.position.z);
		}
	}
	public virtual void CreateForeceSprite()
	{		
		CreateSprite(forceTex,forceGrond,value);
	}
	
	public virtual void CreateMidSprite()
	{		
		CreateSprite(midTex, midGrond, preValue);
	}
	void CreateSprite(Texture2D tex, GameObject go, float percentage)
	{
		SpriteRenderer ren = go.GetComponent<SpriteRenderer>();
		if (ren != null&&tex!=null)
		{
			Destroy(ren.sprite);
			Sprite s = Sprite.Create(tex,new Rect(15,0,(tex.width - 29) * percentage,tex.height),new Vector2(0,0.5f));
			ren.sprite = s;
		}
	}
	public void ShowAnger(int anger)
	{
		string txt = anger.ToString();
		if (anger > 0)
			txt = "+" + txt; 
		mAngerText.Add(txt, Color.yellow, JumbTextDuration);
	}
	public void ShowKillAnger(int anger)
	{
		string txt = anger.ToString();
		if (anger > 0)
		{
			txt = "+" + txt; 
			mAngerText.Add("击杀怒气 " + txt, Color.yellow, JumbTextDuration);
		}
		else
		{
			mAngerText.Add("击杀怒气 " + txt, new Color(0.96f,0.52f,1), JumbTextDuration);
		}
	}
	public virtual void ShowHP(int Hp ,int FullHp, int demage ,AttackResult result)
	{
		m_nHp = Hp;
		m_nFullHp = FullHp;
		float ratio = (float)m_nHp / m_nFullHp;
		ratio = ratio < 0 ? 0 : ratio;
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
		//preValue = value;
		yellowSpeed = preValue-ratio;
		value = ratio;
		CreateForeceSprite();
        // 为什么为null 需调查
		if (mText != null && result != AttackResult.Fire) 
		{
            if (demage < 0)//扣血
            {
                //mText.bitmapFont = m_CombatFontWhite.GetComponent<UIFont>();
                //Debug.Log("扣血" + demage + "  " + 				mText.
				if ( result == AttackResult.Crit)
				{
					if (mBaojiText != null)
						mBaojiText.Add(demage.ToString(), Color.white, JumbTextDuration);
				}
				else
				{
					mText.Add(demage.ToString(), Color.white, JumbTextDuration);
				}
            }
            else if (demage > 0)//加血
            {
				if (mBaojiText != null && result == AttackResult.Crit)
					mBaojiText.Add("+" + demage.ToString(), Color.white, JumbTextDuration);
				else
				{
					//mText.bitmapFont = m_CombatFontGreen.GetComponent<UIFont>();
					mText.Add("+" + demage.ToString(), Color.green, JumbTextDuration);
				}
                
            }
            else if (result == AttackResult.Miss)
			{
				mText.Add("未命中", Color.white, JumbTextDuration);//miss
			}
			else if (result == AttackResult.Immunity)
			{
				mText.Add("物理免疫", Color.white, JumbTextDuration);//miss
			}
		}
	}

	public void ShowBuff(string statusName)
	{
		mText.Add(statusName, mColorBuff, JumbTextDuration);
	}

	public void ShowDebuff(string statusName)
	{
		mText.Add(statusName, mColorDebuff, JumbTextDuration);
	}

	public virtual void SetPlayer(bool IsPlayer)
	{
		if (true == IsPlayer)
		{
			forceTex = hp3;
		}
		else
		{
			forceTex = hp4;
		}
	}
	public void DestroyHP()
	{
		if (null != mText) 
		{
				Destroy (mText.gameObject,1.5f);
				mText = null;
		}
	}
	public virtual void OnDestroy()
	{
		DestroyHP ();
	}
}
