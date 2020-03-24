using UnityEngine;
using System.Collections;

public class NdSprite : MonoBehaviour {
	
	protected Transform m_tParent;
	protected Transform m_thisT;
	public Transform ThisT{
		get{return m_thisT;}
	}
	private BaseAttribute m_attr;
	public BaseAttribute Attr
	{
		get{return m_attr;}
	}

	public virtual void Puase()
	{

	}
	public virtual void Play()
	{

	}
	public virtual void ChangeSpeed()
	{
	}
	public  void Start()
	{
		AttrEventCenter.RegisterHooks(AttrKeyName.Pause_Bool, AttrChange);
		AttrEventCenter.RegisterHooks(AttrKeyName.Speed_Float, AttrChange);
		m_attr = new BaseAttribute();
		SpriteStart();
	}
	public virtual void SpriteStart()
	{
	}
	public virtual void AttrChange(string attr,object value)
	{
		if (this == null)
			return ;
		switch(attr)
		{
		case AttrKeyName.Pause_Bool:
			if ((bool)value)
				Puase();
			else
				Play();
			break;
		case AttrKeyName.Speed_Float:
			ChangeSpeed();
			break;
		default:
			break;
		}
	}
}
