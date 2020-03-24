using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectAttribute :BaseAttribute {

	public EffectAttribute(){

	}

	public override bool TryGetValue (string key, ref object value)
	{
		return base.TryGetValue (key, ref value);
	}

	public override object GetValue (string key)
	{
		return base.GetValue (key);
	}
	public override void SetValue (string key, object value)
	{
		base.SetValue (key, value);
	}
}
