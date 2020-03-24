using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//[ExecuteInEditMode]
public class EffectEditM : EditorWindow {
	
	private static List<string> m_List = new List<string>();
	private static GameObject m_viewgo = null;
	//特效放置的文件路径
	private static string m_EffectPath = "Assets/Resources/effect/prefab";
	private static string m_Path = "effect/prefab/";
	private static string m_strSeach;
	//
	private GameObject SelEffect = null;

	private Vector2 scrollPosition;
	
	
	[@MenuItem("Effect/特效配置")]
	static void OpenEffectM()
	{
		EditorApplication.SaveScene(EditorApplication.currentScene);
		EditorApplication.OpenScene("Assets/Editor/Scene/EffectEdit.unity");
		GetFile ();
		EditorWindow.GetWindow(typeof(EffectEditM));
	}

	public  static  void GetFile()
	{
		m_viewgo = null;
		if (m_List == null)
			m_List = new List<string >();
		m_List.Clear ();


		
		foreach (string file in Directory.GetFiles(m_EffectPath))
		{
			if (file.EndsWith(".meta"))
				continue;
			
			string fileName = Path.GetFileName(file);
			if(fileName.EndsWith(".prefab"))
			{
				string Name =fileName.Replace(".prefab","");
				m_List.Add(Name);
			}
		}
	}


	void OnGUI()
	{
		this.title = "特效查看窗口";
		this.minSize = new Vector2(480, 500);
		int y = 60;
		scrollPosition = GUI.BeginScrollView(new Rect(10,10,Screen.width -10f,Screen.height),scrollPosition,new Rect(10,10,Screen.width - 110f,(m_List.Count*25+100)),false,false);   
		if (NGUIEditorTools.DrawHeader(""))
		{
			NGUIEditorTools.BeginContents();
			
			GUILayout.BeginVertical();
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			//				GUI.Label(new Rect(10, y, 300, 20), name);
			GUILayout.Label("查找", GUILayout.Width(70f));
			
			m_strSeach = EditorGUILayout.TextField(m_strSeach);
			
			
			GUILayout.EndHorizontal();
			
			
			foreach (string c in m_List)
			{
				if(!string.IsNullOrEmpty(m_strSeach))
				{
					if(!c.ToLower().Contains(m_strSeach.ToLower()))
					{
						continue;
					}
				}
				GUILayout.BeginVertical();
				GUILayout.Space(5f);
				GUILayout.EndVertical();
				GUILayout.BeginHorizontal();
				string name = "特效ID:";
				GUILayout.Space(20f);
				//				GUI.Label(new Rect(10, y, 300, 20), name);
				GUILayout.Label(name, GUILayout.Width(70f));
				
				if (GUILayout.Button(c, "AS TextArea", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 150f)))
				{
					
					ViewEffect(c);
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}
				if (GUILayout.Button("查看", GUILayout.MinWidth(50f), GUILayout.MaxWidth(50f)))
				{
					
					ViewEffect(c);
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;
				}
				
				GUILayout.EndHorizontal();
				
			}
			
			
			NGUIEditorTools.EndContents();
			
		}
		
		GUI.EndScrollView();
		
	}

	void ViewEffect(string EffectName)
	{
		if(m_viewgo == null )
		{
			m_viewgo= GameObjectLoader.LoadPath (m_Path, EffectName);
			Selection.activeGameObject = m_viewgo;
		}
		else if(m_viewgo.name != EffectName)
		{
			DelEffect();
			m_viewgo= GameObjectLoader.LoadPath (m_Path, EffectName);
			Selection.activeGameObject = m_viewgo;
		}
	}

	void DelEffect()
	{
		if(m_viewgo != null )
		{
			DestroyImmediate(m_viewgo);
			m_viewgo = null;
		}
	}

	void OnDestroy()
	{
		m_List.Clear ();
		DelEffect();
		Selection.activeGameObject = null;
	}


}
