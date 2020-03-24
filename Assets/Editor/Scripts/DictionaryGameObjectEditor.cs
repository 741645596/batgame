using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DictionaryGameObject))]
public class DictionaryGameObjectEditor : Editor {
	string tempstr = "";
	public override void OnInspectorGUI ()
	{
		GUILayout.Space(6f);
		EditorGUIUtility.LookLikeControls(120f);
		DictionaryGameObject utt = target as DictionaryGameObject;
		GUI.changed = false;

		string type = EditorGUILayout.TextField("Type", utt.type);
		if (GUI.changed)
		{
			NGUIEditorTools.RegisterUndo("Root", utt);
			utt.type = type;
			UnityEditor.EditorUtility.SetDirty(utt);
		}
		
		if (NGUIEditorTools.DrawHeader("Items"))
		{
			NGUIEditorTools.BeginContents();
			DictionaryGameObject ut = target as DictionaryGameObject;
			Field1(ut,ut.Perfabs/*,ut.DicPerfabs*/);
			NGUIEditorTools.EndContents();
		}
	}

	public bool Field1 (Object undoObject, ItemGameObject del , bool removeButton = true)
	{
		if (del == null) return false;
		bool prev = GUI.changed;
		GUI.changed = false;
		bool retVal = false;
		GameObject target = null;
		GameObject parent = null;
		string ID = "";
		int number = 0;
		if (removeButton && del.ID  != "")
		{
			target = EditorGUILayout.ObjectField("key", del.target, typeof(GameObject), true) as GameObject;

			GUILayout.Space(-20f);
			GUILayout.BeginHorizontal();
			GUILayout.Space(36f);
			if (del.ID == "")
			{
				ID = "";//"new point" ;
			}
			else
				ID = del.ID;

#if UNITY_3_5
			if (GUILayout.Button("X", GUILayout.Width(20f)))
#else
			if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f)))
#endif
			{
				target = null;
				ID = "";
			}
			GUILayout.EndHorizontal();
			if (ID != "")
			{
				
				GUILayout.Space(-20f);
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(56f);
				ID = EditorGUILayout.TextField("",ID,GUILayout.Width(60f));
				EditorGUILayout.EndHorizontal();
			}
		}
		else
		{
			ID = del.ID;
			string controlName = "point";
			
			tempstr = EditorGUILayout.TextField("key",tempstr);
			if (Event.current.isKey) {
				if (Event.current.keyCode == KeyCode.Return){
					ID = tempstr;
					tempstr = "";
					//Debug.Log("yay");
					Event.current.Use();
				}
			}
		}
		
		if (del.target != target  || del.ID != ID)
		{
			
			NGUIEditorTools.RegisterUndo("del", undoObject);
			del.target = target;
			del.ID = ID;
			UnityEditor.EditorUtility.SetDirty(undoObject);
		}
		if (del.target != null&& del.ID == "")
		{
			GUILayout.Space(6f);
			EditorGUILayout.HelpBox("请设置key，否着无法新增", MessageType.Warning, true);
			GUILayout.Space(6f);
		}
		GUI.changed = prev;

		retVal = GUI.changed;
		GUI.changed = prev;
		return retVal;
	}



	/// <summary>
	/// Draw a list of fields for the specified list of delegates.
	/// </summary>

	public void Field1 (Object undoObject, List<ItemGameObject> list/*,Dictionary<string,HlepPoint> dictionary*/)
	{
		bool targetPresent = false;
		bool isValid = false;
		
		DictionaryGameObject ut = undoObject as DictionaryGameObject;
		ItemGameObject tempdel;
		// Draw existing delegates
		for (int i = 0; i < list.Count; )
		{
			ItemGameObject del = list[i];
			/*if (!dictionary.TryGetValue(del.ID,out tempdel))
			{
				dictionary.Add(del.ID,del);
			}*/
			if (del == null || del.ID == "")
			{
				//dictionary.Remove(del.ID);
				list.RemoveAt(i);
				continue;
			}
			string ID = del.ID;
			Field1(undoObject, del);
			EditorGUILayout.Space();
			if(ID != del.ID)
			{
				if (ut.CheckKey(del.ID) > 1)
				{
					del.ID = ID;
				}
				/*else
				{
					dictionary.Remove(ID);
					dictionary.Add(del.ID,del);
				}*/
			}
			/*if(ID != del.ID)
			{
				if (dictionary.TryGetValue(del.ID,out tempdel))
				{
					del.ID = ID;
				}
				else
				{
					dictionary.Remove(ID);
					dictionary.Add(del.ID,del);
				}
			}*/

			++i;
		}

		// Draw a new delegate
		ItemGameObject newDel = new ItemGameObject();
		Field1(undoObject, newDel);
		
		if (newDel.ID != "")
		{
			targetPresent = true;
			if (ut.CheckKey(newDel.ID) == 0)
				list.Add(newDel);			
			/*if (!dictionary.TryGetValue(newDel.ID,out tempdel))
			{
				dictionary.Add(newDel.ID,newDel);
			}*/

		}
	}
}


