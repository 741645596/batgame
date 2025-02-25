//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UILocalize))]
#else
[CustomEditor(typeof(UILocalize), true)]
#endif
public class UILocalizeEditor : Editor
{
	List<string> mKeys;

	void OnEnable ()
	{
		Dictionary<string, string[]> dict = Localization.dictionary;

		if (dict.Count > 0)
		{
			mKeys = new List<string>();

			foreach (KeyValuePair<string, string[]> pair in dict)
			{
				if (pair.Key == "KEY") continue;
				mKeys.Add(pair.Key);
			}
			mKeys.Sort(delegate (string left, string right) { return left.CompareTo(right); });
		}

	}


	public override void OnInspectorGUI ()
	{
		serializedObject.Update();
		
		GUILayout.Space(6f);
		NGUIEditorTools.SetLabelWidth(80f);

		GUILayout.BeginHorizontal();
		// Key not found in the localization file -- draw it as a text field
		SerializedProperty sp = NGUIEditorTools.DrawProperty("Key", serializedObject, "key");

		string myKey = sp.stringValue;
		bool isPresent = (mKeys != null) && mKeys.Contains(myKey);
		GUI.color = isPresent ? Color.green : Color.red;
		GUILayout.BeginVertical(GUILayout.Width(22f));
		GUILayout.Space(2f);
#if UNITY_3_5
		GUILayout.Label(isPresent? "ok" : "!!", GUILayout.Height(20f));
#else
		GUILayout.Label(isPresent? "\u2714" : "\u2718", "TL SelectionButtonNew", GUILayout.Height(20f));
#endif
		GUILayout.EndVertical();
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		if (isPresent)
		{
			if (NGUIEditorTools.DrawHeader("Preview"))
			{
				NGUIEditorTools.BeginContents();

				string[] keys;
				string[] values;

				if (Localization.dictionary.TryGetValue("KEY", out keys) && Localization.dictionary.TryGetValue(myKey, out values))
				{
					for (int i = 0; i < keys.Length; ++i)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Label(keys[i], GUILayout.Width(70f));

						if (GUILayout.Button(values[i], "AS TextArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 110f)))
						{
							(target as UILocalize).value = values[i];
							GUIUtility.hotControl = 0;
							GUIUtility.keyboardControl = 0;
						}
						GUILayout.EndHorizontal();
					}
				}
				else
				{
					GUILayout.Label("No preview available");
				}

				NGUIEditorTools.EndContents();
			}
		}
		else if (mKeys != null && !string.IsNullOrEmpty(myKey))
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(80f);
			GUILayout.BeginVertical();
			GUI.backgroundColor = new Color(1f, 1f, 1f, 0.35f);

			int matches = 0;

			for (int i = 0, imax = mKeys.Count; i < imax; ++i)
			{
				if (mKeys[i].StartsWith(myKey, System.StringComparison.OrdinalIgnoreCase) || mKeys[i].Contains(myKey))
				{
#if UNITY_3_5
					if (GUILayout.Button(mKeys[i] + " \u25B2"))
#else
					if (GUILayout.Button(mKeys[i] + " \u25B2", "CN CountBadge"))
#endif
					{
						sp.stringValue = mKeys[i];
						GUIUtility.hotControl = 0;
						GUIUtility.keyboardControl = 0;
					}
					
					if (++matches == 8)
					{
						GUILayout.Label("...and more");
						break;
					}
				}
			}
			GUI.backgroundColor = Color.white;
			GUILayout.EndVertical();
			GUILayout.Space(22f);
			GUILayout.EndHorizontal();
		}
		if(!isPresent)
		{
			if (NGUIEditorTools.DrawHeader("No Exit Key,Need Add?"))
			{

				NGUIEditorTools.BeginContents();
				UILabel label = (target as UILocalize).transform.GetComponent<UILabel>();
				string values = label.text;
				GUILayout.BeginHorizontal();
				GUILayout.Label(myKey, GUILayout.Width(70f));

				if (GUILayout.Button(values, "AS TextArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 110f)))
				{
					(target as UILocalize).value = values;

					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}

				GUILayout.EndHorizontal();

				if (GUILayout.Button("Refreh", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width)))
				{
					Localization.Refresh();
				}
				
				NGUIEditorTools.EndContents();
			}
		}
		
		serializedObject.ApplyModifiedProperties();
	}
}
