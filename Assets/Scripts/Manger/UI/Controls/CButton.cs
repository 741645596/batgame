using UnityEngine;
using System.Collections;

/// <summary>
/// the base button which has some base methods
/// author chenlj
/// </summary>
public class CButton : MonoBehaviour 
{
	// 按钮被按下的贴图
	public Material pressMat = null;
	
	/// <summary>
	/// the delegate of press event
	/// </summary>
	public delegate void Press(GameObject sender, bool pressed);
	
	/// <summary>
	/// the delegate of click event
	/// </summary>
	public delegate void Click(GameObject sender);
	
	/// <summary>
	/// the delegate of select event
	/// </summary>
	public delegate void Select(GameObject sender, bool selected);

    /// <summary>
    /// the delegate of hover event.
    /// </summary>
    public delegate void Hover(GameObject sender, bool selected);
	
	/// <summary>
	/// the event handler of press event
	/// </summary>
	public event Press PressHandler;
	
	/// <summary>
	/// the event handler of click event
	/// </summary>
	public event Click ClickHandler;
	
	/// <summary>
	/// the event handler of select event
	/// </summary>
	public event Select SelectHandler;

    /// <summary>
    /// the event handler of hover event
    /// </summary>
    public event Hover HoverHandler;
	
	/// <summary>
	/// 按钮点击冷却时间
	/// </summary>
	public float coolDownTime = 0.1f;
	
	// 是否可以点击
	bool canClick = true;

	void OnPress(bool pressed)
	{
		if (pressMat != null)
		{
			GetComponent<Renderer>().materials = pressed ? new Material[] { GetComponent<Renderer>().material, pressMat } :
				                           new Material[] { GetComponent<Renderer>().materials[0] };
		}
		
		if (PressHandler != null)
			PressHandler(gameObject, pressed);
	}
	
	void OnEnable()
	{
		canClick = true;
	}
	
	void OnClick()
	{
		if (!canClick)
			return;
		
		if (gameObject.activeInHierarchy)
		{
			canClick = false;
			Coroutine.DispatchService(CoolDown(), gameObject, null);
		}
		
		if (ClickHandler != null)
			ClickHandler(gameObject);
	}
	
	IEnumerator CoolDown()
	{
		yield return new WaitForMSeconds(coolDownTime*1000f);
		
		canClick = true;
	}
	
	void OnSelect(bool selected)
	{
		if (pressMat != null)
		{
			GetComponent<Renderer>().materials = new Material[] { GetComponent<Renderer>().materials[0] };
		}
		
		if (SelectHandler != null)
			SelectHandler(gameObject, selected);
	}

    void OnHover(bool isOver)
    {
         if (HoverHandler != null)
             HoverHandler(gameObject, isOver);
    }
}
