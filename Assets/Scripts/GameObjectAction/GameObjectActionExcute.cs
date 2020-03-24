using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameObjectActionExcute : MonoBehaviour {
	List<GameObjectAction> m_actionlist = new List<GameObjectAction>();
	public  bool isPlay = true;
	public float Speed = 1f;
	NdSprite m_sprite;
    /// <summary>
    /// 创建一个GameObjectActionExcute，如果已有则返回一个
    /// </summary>
    public static GameObjectActionExcute CreateExcute(GameObject go)
    {
        if (go == null)
        {
            return null;
        }
        GameObjectActionExcute gae = go.GetComponent<GameObjectActionExcute>();
        if (gae!=null)
        {
            return gae;
        }
        else
        {
            return go.AddComponent<GameObjectActionExcute>();
        }
    }

	// Use this for initialization
	void Start () {
		if(m_sprite == null)
			m_sprite = GetComponent<NdSprite>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!isPlay)
			return;
		if(m_actionlist.Count > 0)
		{
			if (m_actionlist[0].m_target == null)
				m_actionlist[0].SetTarget(gameObject);
			if (!m_actionlist[0].Update(Time.deltaTime * Speed))
			{
				m_actionlist.RemoveAt(0);
			}
		}
	
	}
	public void AddAction(GameObjectAction action)
	{
		m_actionlist.Add(action);
	}

	public GameObjectAction GetCurrentAction()
	{
		if (m_actionlist.Count > 0)
			return m_actionlist[0];
		return null;
	}
	
	public GameObjectAction GetNextAction()
	{
		if (m_actionlist.Count >= 2)
			return m_actionlist[1];
		return null;
	}

	public void GoNextAction()
	{
		m_actionlist.RemoveAt(0);
	}
	public void FinishCurAction()
	{
		if (m_actionlist.Count > 0)
		{
			m_actionlist[0].Finish();
			m_actionlist.RemoveAt(0);
		}
	}
	public void Play()
	{
		isPlay = true;
        if (m_sprite!=null)
			m_sprite.Play();
	}
	public void Pause()
	{
		isPlay = false;
        if (m_sprite!=null)
			m_sprite.Puase();
	}
	public void Stop()
	{
		if (m_actionlist.Count > 0)
		{
			//m_actionlist[0].Finish();
			m_actionlist.Clear();
		}
	}
	public void SetSpeed(float s)
	{
		Speed = s;
		//m_sprite.ChangeSpeed(s);
	}
}
