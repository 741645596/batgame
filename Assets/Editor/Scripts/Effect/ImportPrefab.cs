﻿using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
[ExecuteInEditMode]
public class ImportPrefab
{

	[MenuItem("Assets/new Import Assets")]

	static void Build()
	{
		string inPrefabPath = "";
		inPrefabPath = EditorUtility.OpenFilePanel ("选择Unity文件","","unitypackage");


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