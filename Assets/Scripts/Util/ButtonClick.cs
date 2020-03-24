using UnityEngine;
using System.Collections;

public class ButtonClick : MonoBehaviour {

	public delegate void OnClickHandler();
	public event OnClickHandler OnClickEventHandler;

	/// <summary>
	/// Call the listener function.
	/// </summary>

	protected virtual void OnClick()
	{
		//if (current == null && isEnabled)
		{

			if (OnClickEventHandler != null)
			{
				OnClickEventHandler();
			}

		}
	}
}
