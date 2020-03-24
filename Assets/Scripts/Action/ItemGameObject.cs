#if REFLECTION_SUPPORT
using System.Reflection;
#endif

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemGameObject
{
	[SerializeField] GameObject mTarget;
	[SerializeField] string mid;
	//[SerializeField] GameObject mParent;
	public GameObject target { get { return mTarget; } set { mTarget = value;} }

	/// <summary>
	/// Event delegate's method name.
	/// </summary>

	public string ID { get { return mid; } set { mid = value;} }
	//public GameObject Parent { get { return mParent; } set { mParent = value;} }


	public ItemGameObject () { mid = ""; }
	public ItemGameObject (GameObject target, string id/*, GameObject parent*/) { Set(target, id/*, parent*/); }
	public void Set (GameObject target, string id/*,GameObject parent*/)
	{
		this.mTarget = target;
		this.mid = id;
		/*this.mParent = parent;*/
	}

}
