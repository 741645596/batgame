using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

//[ExecuteInEditMode]

public class ShowPrefabList : EditorWindow {
	
	private static List<string> m_List = new List<string>();
	private static List<GameObject> m_objectList = new List<GameObject> ();
	private static GameObject m_viewgo = null;

	private static string m_WndPrefabPath = "Assets/Resources/Prefabs/UI/Wnd";
	private static string m_ItemPrefabPath = "Assets/Resources/Prefabs/UI/WndItems";
	private static string m_Path = "Prefabs/UI/Wnd/";
	private static string m_ItemPath = "Prefabs/UI/WndItems/";
	private static string outPrefabPath = "Assets/Tencent/OutUI";
	private static string timePath = outPrefabPath  +"/" + DateTime.Now.ToString("yyyyMMddHHmmss");

	private static string createClassPath = "Assets/Scripts/Manger/UI/WND";
	private static string createItemClassPath = "Assets/Scripts/Manger/UI/WndItems";
	private static GameObject m_wndRoot = null;
	private static string m_strSeach;

	private Vector2 scrollPosition;

	private static string wndBaseHScript = "using System;\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\n\npublic class myWndBase_h : WndBase_h \n" +
		"{\n\n}";
	private static string wndBaseScript = "using System;\nusing UnityEngine;\nusing System.Collections;\nusing System.Collections.Generic;\n\npublic class myWndBase : WndBase \n" +
		"{\n	public myWndBase_h MyHead \n	{\n		get\n		{ \n			return (base.BaseHead () as myWndBase_h);	\n		}	\n	}\n	public override void WndStart() \n	{\n		base.WndStart(); \n	}\n}";
	[MenuItem("Assets/new Script")]
	static void CreateScript()
	{
		CreateDirectory(createClassPath);
		foreach(UnityEngine.Object o in Selection.objects)
		{

			string content =wndBaseHScript.Replace("myWndBase",o.name);
			string content2 =wndBaseScript.Replace("myWndBase",o.name);
			string path = "";
			string path2 = "";
			if(o.name.EndsWith("item") || o.name.EndsWith("Item"))
			{
				path = Application.dataPath + createItemClassPath.Replace("Assets/","/") +"/" + o.name +"_h.cs";
				path2 = Application.dataPath + createItemClassPath.Replace("Assets/","/") +"/" + o.name +".cs";
			}
			else
			{
				path = Application.dataPath + createClassPath.Replace("Assets/","/") +"/" + o.name +"_h.cs";
				path2 = Application.dataPath + createClassPath.Replace("Assets/","/") +"/" + o.name +".cs";
			}

			if(!File.Exists(path))
				System.IO.File.WriteAllText(path, content);

			if(!File.Exists(path2))
				System.IO.File.WriteAllText(path2, content2);
			AssetDatabase.Refresh();

//			string filePath = createClassPath +"/" + o.name + "_h.cs";
//			WndBase_h wndBase = AssetDatabase.LoadAssetAtPath(filePath,typeof(WndBase_h)) as WndBase_h;
			string compName = o.name + "_h";
			UnityEngineInternal.APIUpdaterRuntimeServices.AddComponent((o as GameObject), "Assets/Editor/Scripts/Effect/ShowPrefabList.cs (66,4)", compName);
		}

	}



	[@MenuItem("Tool/预置预览")]
	static void OpenEffectM()
	{
		EditorApplication.SaveScene(EditorApplication.currentScene);
		EditorApplication.OpenScene("Assets/Editor/Scene/ShowPrefab.unity");
		GetFile ();
		EditorWindow.GetWindow(typeof(ShowPrefabList));

		m_wndRoot = GetWndRoot ();

		if (m_objectList == null)
			m_objectList = new List<GameObject >();
		m_objectList.Clear ();
		FindChileByGameObj ();
	}

	public  static  void GetFile()
	{
		m_viewgo = null;
		if (m_List == null)
			m_List = new List<string >();
		m_List.Clear ();


		
		foreach (string file in Directory.GetFiles(m_WndPrefabPath))
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
		//Item
		foreach (string file in Directory.GetFiles(m_ItemPrefabPath))
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
		this.title = "Prefab查看窗口";
		this.minSize = new Vector2(480, 500);
		int y = 30;
//		scrollPosition = GUILayout.BeginVertical(GUILayout.Width(22f));
		scrollPosition = GUI.BeginScrollView(new Rect(10,0,Screen.width -10f,Screen.height),scrollPosition,new Rect(10,0,Screen.width - 150f,(m_List.Count*35)),false,false);   
		if (NGUIEditorTools.DrawHeader("Prefab列表"))
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
				string name = "Name:";
				GUILayout.Space(20f);
//				GUI.Label(new Rect(10, y, 300, 20), name);
				GUILayout.Label(name, GUILayout.Width(70f));
				string textName = c;
				var newName = EditorGUILayout.TextField (textName, GUILayout.Width(Screen.width -250));
//				if(newName != textName)
//				{
//
//				}
				string showFind = "";
				bool find = IsContainObject(c);
				if(find) showFind = "√";
				
				if (GUILayout.Button(showFind,GUILayout.MinWidth(40f), GUILayout.MaxWidth(40f)))
				{
					
					ViewEffect(c);
					GUIUtility.hotControl = 0;
					GUIUtility.keyboardControl = 0;

				}

				GUILayout.EndHorizontal();

			}

			
			NGUIEditorTools.EndContents();

			GUILayout.BeginVertical();
			GUILayout.Space(5f);
			GUILayout.EndVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Space(20f);
			//				GUI.Label(new Rect(10, y, 300, 20), name);
			if (GUILayout.Button("导出预置", GUILayout.MinWidth(80f), GUILayout.MaxWidth(Screen.width - 100f),GUILayout.MinHeight(30f),GUILayout.MaxHeight(30f)))
			{
				ExportPrefab();

			}
			
			GUILayout.EndHorizontal();


			
		}

		GUI.EndScrollView();

	}

	void ViewEffect(string EffectName)
	{

		if(IsContainObject(EffectName))
		{
			DestoryGameObject(EffectName);
			return;
		}
		m_viewgo = GameObjectLoader.LoadPath (EffectName.ToLower().EndsWith("item")?m_ItemPath:m_Path, EffectName ,m_wndRoot.transform);
		if(m_viewgo)
		{
			m_objectList.Add(m_viewgo);
		}
	}
	/// <summary>
	/// 获取窗口，从窗口根节点下一层获取，获取失败则创建窗口
	/// </summary>
	/// <typeparam name="T">窗口实现类</typeparam>
	/// <param name="strDialogIDD">strDialogIDD 窗口ID</param>
	/// <param name="parent">parent 父节点</param>
	/// <returns></returns>
	public static GameObject GetWndRoot()
	{
		GameObject []gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
		foreach(GameObject obj in gos)
		{
			if(obj.name == "WndRoot(这下面挂载UI界面)")
			{
				return obj;
			}
		}
		return null;

	}
	public static void FindChileByGameObj()
	{
		foreach(Transform tran in m_wndRoot.transform)
		{
			m_objectList.Add(tran.gameObject);
		}
	}
	public static bool IsContainObject(GameObject obj)
	{
		return m_objectList.Contains (obj);
	}
	public static bool IsContainObject(string objName)
	{
		foreach(GameObject obj in m_objectList)
		{
			//添加之后手动在面板删除这里还保存有，需要清除
			if(obj == null) 
			{
				m_objectList.Remove(obj);
				return false;
			}
			if(obj.name == objName)
			{
				return true;
			}
		}
		return false;
	}

	private static void ExportPrefab()
	{
		int count = 0;
		List<string> list = new List<string> ();
		foreach(GameObject item in m_objectList)
		{
			string inStr =  item.name.ToLower().EndsWith("item")?m_ItemPrefabPath:m_WndPrefabPath + "/" + item.name + ".prefab";
			string []paths2 = AssetDatabase.GetDependencies(new string[]{inStr});
			foreach(string pathItem in paths2)
			{
				string del = item.name + ".cs";
				if(item.name.ToLower().EndsWith("item"))
				{
					list.Add(pathItem);
				}
				else if(pathItem.EndsWith(del) || pathItem.Contains("Assets/NGUI/Scripts")) 
					continue;
				else
					list.Add(pathItem);
			}
			count++;
		}
		if(count < 1) return;
		CreateDirectory(timePath);
		string outUnit = timePath +"/"+ m_objectList[0].name + ".unitypackage";
		//
		AssetDatabase.ExportPackage(list.ToArray(), outUnit, ExportPackageOptions.Default);
		AssetDatabase.Refresh();

	}
	public static void CreateDirectory(string inPath)
	{
		if(inPath.Contains("Assets/"))
		{
			
			inPath = inPath.Replace("Assets/","/");
		}
		string path = Application.dataPath + inPath;
		if(!System.IO.Directory.Exists(path))
		{
			// 目录不存在，建立目录
			System.IO.Directory.CreateDirectory(path);
		}
	}
	public static void DestoryGameObject (string objDel)
	{

		GameObject []gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
		foreach(GameObject obj in gos)
		{
			if(obj.name == objDel)
			{
				GameObject.DestroyImmediate(obj);
				m_objectList.Remove(obj);
				return;
			}
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
		if(m_objectList.Count > 0)
			m_objectList.RemoveRange(0,m_objectList.Count -1);
		DelEffect();
		Selection.activeGameObject = null;
	}


}
