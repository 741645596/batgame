//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Attaching this script to an object will let you trigger remote functions using NGUI events.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Event Trigger")]
public class UIEventTrigger : MonoBehaviour
{
	static public UIEventTrigger current;

	//public List<EventDelegate> onHoverOver = new List<EventDelegate>();
	//public List<EventDelegate> onHoverOut = new List<EventDelegate>();
	//public List<EventDelegate> onPress = new List<EventDelegate>();
	//public List<EventDelegate> onRelease = new List<EventDelegate>();
	//public List<EventDelegate> onSelect = new List<EventDelegate>();
	//public List<EventDelegate> onDeselect = new List<EventDelegate>();
	//public List<EventDelegate> onClick = new List<EventDelegate>();
	//public List<EventDelegate> onDoubleClick = new List<EventDelegate>();
	//public List<EventDelegate> onDragOver = new List<EventDelegate>();
	//public List<EventDelegate> onDragOut = new List<EventDelegate>();


	public delegate void OnEventHandler (UIEventTrigger sender);
	public event OnEventHandler onHoverOver;
	public event OnEventHandler onHoverOut;
	public event OnEventHandler onPress;
	public event OnEventHandler onRelease;
	public event OnEventHandler onSelect;
	public event OnEventHandler onDeselect;
	public event OnEventHandler onClick;
	public event OnEventHandler onDoubleClick;
	public event OnEventHandler onDragOver;
	public event OnEventHandler onDragOut;


	void OnHover (bool isOver)
	{
		if (current != null) return;
		current = this;
		//if (isOver) EventDelegate.Execute(onHoverOver);
		//else EventDelegate.Execute(onHoverOut);
		if (isOver) 
		{
			if (onHoverOver != null)
				onHoverOver (current);
				
		} 
		else 
		{
			if (onHoverOut != null)
				onHoverOut (current);
		}
		current = null;
	}

	void OnPress (bool pressed)
	{
		if (current != null) return;
		current = this;
		//if (pressed) EventDelegate.Execute(onPress);
		//else EventDelegate.Execute(onRelease);
		if (pressed) 
		{
			if (onPress != null)
				onPress (current);
			
		} 
		else 
		{
			if (onRelease != null)
				onRelease (current);
		}
		current = null;
	}

	void OnSelect (bool selected)
	{
		if (current != null) return;
		current = this;
		//if (selected) EventDelegate.Execute(onSelect);
		//else EventDelegate.Execute(onDeselect);
		if (selected) 
		{
			if (onSelect != null)
				onSelect (current);
			
		} 
		else 
		{
			if (onDeselect != null)
				onDeselect (current);
		}
		current = null;
	}

	void OnClick ()
	{
		if (current != null) return;
		current = this;
		//EventDelegate.Execute(onClick);
		if (onClick != null)
			onClick (current);
		current = null;
	}

	void OnDoubleClick ()
	{
		if (current != null) return;
		current = this;
		//EventDelegate.Execute(onDoubleClick);
		if (onDoubleClick != null)
			onDoubleClick (current);
		current = null;
	}

	void OnDragOver (GameObject go)
	{
		if (current != null) return;
		current = this;
		//EventDelegate.Execute(onDragOver);
		if (onDragOver != null)
			onDragOver (current);
		current = null;
	}

	void OnDragOut (GameObject go)
	{
		if (current != null) return;
		current = this;
		//EventDelegate.Execute(onDragOut);
		if (onDragOut != null)
			onDragOut (current);
		current = null;
	}
}
