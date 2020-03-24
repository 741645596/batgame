using UnityEngine;
using DG.Tweening;

public class ResourceAction : MonoBehaviour {

    public float m_duration;
    public ResourceType m_type;
    public int m_count;

	private Camera m_gameCamera;
	private Camera m_uicamera;
	private float m_timeCount;
    private float m_fShowEffectCounter = 0;
	// Use this for initialization
	void Start () {
		if (m_uicamera == null) m_uicamera = WndManager.GetNGUICamera();
		if (m_gameCamera == null) m_gameCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		    m_timeCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(m_timeCount < m_duration && m_timeCount + Time.deltaTime >= m_duration)
		{
			FlyToUI();
		}
		
		m_timeCount += Time.deltaTime;

        if (m_fShowEffectCounter > 0)
        {
            m_fShowEffectCounter -= Time.deltaTime;
            if (m_fShowEffectCounter <= 0.2f && m_fShowEffectCounter > (0.2f - Time.deltaTime))
            {
                SetEffect();   
            }
        }
	}

    void SetEffect()
    {
		if (m_type == ResourceType.Gold )
        {
			CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
			if(Wnd != null)
			{
				Transform parent = Wnd.GetResourcePos(m_type);
				GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000301" ,parent.position ,parent);
				if(gae != null)
				{
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
					gae.AddAction(ndEffect);
				}
			}
           
        }
        else if (m_type == ResourceType.Wood)
        {
			CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
			if(Wnd != null)
			{
				Transform parent = Wnd.GetResourcePos(m_type);
				GameObjectActionExcute gae = EffectM.LoadEffect("effect/prefab/", "2000311" ,parent.position ,parent);
				if(gae != null)
				{
					GameObjectActionDelayDestory ndEffect = new GameObjectActionDelayDestory(0.5f);
					gae.AddAction(ndEffect);
				}
			}
        }
    }

	public void FlyToUI()
	{
        m_fShowEffectCounter = 0.6f;
		Vector3 pos = Vector3.zero;
		CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
		if(Wnd != null)
		{
			if (m_type == ResourceType.Gold)
				pos = m_uicamera.WorldToViewportPoint(Wnd.GetResourcePos(m_type).position);
			else if (m_type == ResourceType.Wood)
				pos = m_uicamera.WorldToViewportPoint(Wnd.GetResourcePos(m_type).position);
		}
		pos.z -=m_gameCamera.transform.position.z/2;

		
		pos = m_gameCamera.ViewportToWorldPoint (pos);
        //str += "," + pos;
		pos = m_gameCamera.transform.InverseTransformPoint(pos);
		//pos.y -= 0.5f;
		transform.parent = m_gameCamera.transform;
		//	Debug.Log(str);
		gameObject.transform.DOMove(pos, 1f);
		//Debug.DrawLine(transform.position,pos);
	}
	 void Destroy()
	{
		if (m_type == ResourceType.Gold)
		{
            //SoundPlay.Play("gold", false, false);
			CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
			if(Wnd != null)
			{
				Wnd.SetCombatGold(m_count,true);
				Wnd.SetCombatGold(-m_count,false);
			}

		}
		else if(m_type == ResourceType.Wood)
		{
//            SoundPlay.Play("battle_wood_pick", false, false);
			CombatInfoWnd Wnd = WndManager.FindDialog<CombatInfoWnd>();
			if(Wnd != null)
			{
				Wnd.SetCombatWood(m_count, true);
				Wnd.SetCombatWood(-m_count, false);
			}
		}
		Destroy(gameObject);
	}
}
