using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
[ExecuteInEditMode]
public class ExportPrefab
{

	[MenuItem("Assets/new Export Assets")]

	static void Build()
	{
		string outPrefabPath = "Assets/Tencent/OutUI";
		if(Selection.objects == null ) return;
		List<string> outPaths = new List<string> ();
		Dictionary<string,GameObject> objList = new Dictionary<string,GameObject>();

		string timePath = outPrefabPath  +"/" + DateTime.Now.ToString("yyyyMMddHHmmss");

		CreateDirectory(timePath);

		timePath += "/";

		foreach(UnityEngine.Object o in Selection.objects)
		{

			WndBase []gos = U3DUtil.GetComponentsInChildren<WndBase>((o as GameObject));
			if(gos != null)
			{
				foreach(WndBase obj in gos)
				{
					string name= obj.GetType().ToString();
					objList.Add(name,obj.gameObject);
					GameObject.DestroyImmediate(obj,true);
				}
			}


			WndBase wnd = (o as GameObject).GetComponent<WndBase>();

			if(wnd != null)
				GameObject.DestroyImmediate(wnd,true);

			string inStr =  AssetDatabase.GetAssetPath(o);
	
			string []paths2 = AssetDatabase.GetDependencies(new string[]{inStr});
			foreach(string pathItem in paths2)
			{
				string del = o.name + ".cs";
				if(o.name.EndsWith("Item") || o.name.EndsWith("item"))
				{
					outPaths.Add(pathItem);
				}
				else if(pathItem.EndsWith(del) ||  pathItem.Contains("Assets/NGUI/Scripts")) 
					continue;
				else
					outPaths.Add(pathItem);
			}
		}
		if(outPaths.Count < 1)return;

		string outUnit = timePath + Selection.objects[0].name + ".unitypackage";
//
		AssetDatabase.ExportPackage(outPaths.ToArray(), outUnit, ExportPackageOptions.Default);
		AssetDatabase.Refresh();

//		foreach(UnityEngine.Object o in Selection.objects)
//		{
////			Type type = GetType (o.name);
////			object baseinfo= Activator.CreateInstance (type);
//			Component com = (o as GameObject).GetComponent(o.name);
//			if(com == null)
//			{
//				(o as GameObject).AddComponent(o.name);
//			}
//
//		}
//		foreach(string key in objList.Keys)
//		{
//			GameObject obj;
//			objList.TryGetValue(key,out obj);
//			Component com = obj.GetComponent(key);
//			if(com == null)
//			{
//				obj.AddComponent(key);
//			}
//
//		}
	}

//	[MenuItem("Assets/new Script")]
//	static void CreateScript()
//	{
//
//	}

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
	public static Type GetType( string TypeName )
	{
		
		// Try Type.GetType() first. This will work with types defined
		// by the Mono runtime, in the same assembly as the caller, etc.
		var type = Type.GetType( TypeName );
		
		// If it worked, then we're done here
		if( type != null )
			return type;
		
		// If the TypeName is a full name, then we can try loading the defining assembly directly
		if( TypeName.Contains( "." ) )
		{
			
			// Get the name of the assembly (Assumption is that we are using 
			// fully-qualified type names)
			var assemblyName = TypeName.Substring( 0, TypeName.IndexOf( '.' ) );
			
			// Attempt to load the indicated Assembly
			var assembly = Assembly.Load( assemblyName );
			if( assembly == null )
				return null;
			
			// Ask that assembly to return the proper Type
			type = assembly.GetType( TypeName );
			if( type != null )
				return type;
			
		}
		
		// If we still haven't found the proper type, we can enumerate all of the 
		// loaded assemblies and see if any of them define the type
		var currentAssembly = Assembly.GetExecutingAssembly();
		var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
		foreach( var assemblyName in referencedAssemblies )
		{
			
			// Load the referenced assembly
			var assembly = Assembly.Load( assemblyName );
			if( assembly != null )
			{
				// See if that assembly defines the named type
				type = assembly.GetType( TypeName );
				if( type != null )
					return type;
			}
		}
		
		// The type just couldn't be found...
		return null;
		
	}

}