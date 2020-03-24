
using UnityEngine;
using UnityEditor;

using System.Collections;
using System.IO;
using System;


public class CodeGeneratorManager
{
	string mSystemName = string.Empty;
	string mAuthorName = string.Empty;

	string mTemplatePath = "Scripts/Test/CodeTemplate";
	string mSystemModulePath = "Scripts/Test/GenCode";


	string mSignature = @"
/**
 *  FileName:	    FILE_NAME
 *  Author:	        AUTHOR_NAME
 *  Date:           CREATION_DATE
 *  Description:    
 */

";
	static CodeGeneratorManager mInst = new CodeGeneratorManager();
	public static CodeGeneratorManager Instance
	{
		get { return mInst; }
	}

	public void GenSystem(string systemName, string authorName)
	{
		if (string.IsNullOrEmpty(systemName))
		{
			Debug.LogError("EMPTY systemName");
			return;
		}
		mSystemName = systemName;
		mAuthorName = authorName;

		GenCode(mSystemModulePath);

	}

	private void GenCode(string outputPath)
	{
		string fromDir = string.Format("{0}/{1}", Application.dataPath, mTemplatePath);
		string toDir = string.Format("{0}/{1}/{2}", Application.dataPath, outputPath, mSystemName);
		Directory.CreateDirectory(toDir);

		string[] files = Directory.GetFiles(fromDir, "*.cs.txt");
		foreach (var file in files)
		{
			string fromText = File.ReadAllText(file);
			string toText = fromText.Replace("#ClassName#", mSystemName);
			string toName = Path.GetFileName(file).Replace("Template", mSystemName);
			toName = toName.Replace(".cs.txt", ".cs");

			string signature = mSignature.Replace("AUTHOR_NAME", mAuthorName);
			signature = signature.Replace("CREATION_DATE", DateTime.Today.ToShortDateString());
			signature = signature.Replace("FILE_NAME", toName);
			toText = signature + toText;

			string toPath = string.Format("{0}/{1}", toDir, toName);
			File.WriteAllText(toPath, toText);
		}
	}
}


public class ModuleGenWindow : EditorWindow
{
    [MenuItem("Tool/ModuleCode Generator")]
    public static void Init()
    {
        EditorWindow.GetWindow<ModuleGenWindow>();
    }

    string mSystemName = string.Empty;
    string mAuthorName = string.Empty;

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.Space(5f);
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Module Name:");
        mSystemName = GUILayout.TextField(mSystemName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Author Name:");
        mAuthorName = GUILayout.TextField(mAuthorName);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.Space(5f);

        if (GUILayout.Button("Generate", GUILayout.Height(20f)))
        {
            CodeGeneratorManager.Instance.GenSystem(mSystemName, mAuthorName);
        }

        GUILayout.EndVertical();
    }
}
