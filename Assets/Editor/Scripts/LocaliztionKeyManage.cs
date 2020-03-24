using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using System.Text.RegularExpressions;

public class LocaliztionKeyManage:Editor
{
	public enum WndCode
	{
		LoginWnd = 000,
		RankWnd = 001,
	}

	private static string UIPrefabPath = Application.dataPath + "/Resources/Prefabs/UI";
	//脚本的文件夹目录
	private static string ScriptPath = Application.dataPath + "/Scripts";
	private static string OutPath = Application.dataPath +"/out.txt";
	private static string FormOutPath = Application.dataPath +"/Formatout.txt";
	private static List<string>LocalStringList = null;
	private static List<string>LocalStringForMatList = null;
	private static string staticWriteText = "";
	private static string FormatstaticWriteText = "";

	[MenuItem("Tools/导出多语言")]
	static public void ExportChinese()
	{
		LocalStringList = new List<string>();
		LocalStringForMatList = new List<string>();
		staticWriteText ="";
		FormatstaticWriteText = "";
		
		//提取Prefab上的中文
//		staticWriteText +="----------------Prefab----------------------\n";
//		LoadDiectoryPrefab(new DirectoryInfo(UIPrefabPath));
		
		//提取CS中的中文
		staticWriteText +="----------------Script----------------------\n";
		LoadDiectoryCS(new DirectoryInfo(ScriptPath));
		
		
		//最终把提取的中文生成出来
		string textPath = OutPath;
		if (System.IO.File.Exists (textPath)) 
		{
			File.Delete (textPath);
		}
		using(StreamWriter writer = new StreamWriter(textPath, false, Encoding.UTF8))
		{
			writer.Write(staticWriteText);
		}
		AssetDatabase.Refresh();

		//最终把提取的中文生成出来
		string textPath2 = FormOutPath;
		if (System.IO.File.Exists (textPath2)) 
		{
			File.Delete (textPath2);
		}
		using(StreamWriter writer = new StreamWriter(textPath2, false, Encoding.UTF8))
		{
			writer.Write(FormatstaticWriteText);
		}
		AssetDatabase.Refresh();
	}

	public string GetRandCode(WndCode sWndCode,string sWordCode)
	{
		bool reslut = Localization.Exists (sWndCode.ToString() + sWordCode);

		if(reslut)
		{
			return Localization.Get(sWndCode.ToString() + sWordCode);
		}
		else
		{
			Debug.Log("not exit code " + sWndCode + sWordCode);
			return "000000";
		}
	}
	//write a file, existed file will be overwritten if append = false
	
	public static void WriteCSV(string text,string sKeyCode,bool append) 
	{
		if(Localization.Exists(sKeyCode)) 
		{
			Debug.Log("exit code " + sKeyCode);
			return;
		}
		string filePathName = Application.dataPath + "/Resources/Localization.csv";
		StreamWriter fileWriter = new StreamWriter(filePathName,append,Encoding.UTF8); 

		List<string> list = new List<string> ();
		list.Add(sKeyCode);
		list.Add(text);

		string sLineText = "";
		foreach(string strArr in list)	
		{
			
			sLineText +=strArr+",";

		}
		sLineText = sLineText.Substring (0,sLineText.Length-1);
		sLineText += "\n";
		fileWriter.WriteLine(sLineText);
		fileWriter.Flush();

		fileWriter.Close();

	}
	static public  void  LoadDiectoryPrefab(DirectoryInfo dictoryInfo)
	{
	  if(!dictoryInfo.Exists)  
			return;
		
		
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.prefab",SearchOption.AllDirectories);
		foreach (FileInfo  files in  fileInfos)	
		{
		  string path = files.FullName;
		  string assetPath =  path.Substring(path.IndexOf("Assets/"));
	
		  GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
		
		  GameObject instance = GameObject.Instantiate(prefab) as GameObject;
		
		  SearchPrefabString(instance.transform);

		  GameObject.DestroyImmediate(instance);	
		}
	}
	//递归所有C#代码
	static public  void  LoadDiectoryCS(DirectoryInfo dictoryInfo)
	{
		
		if(!dictoryInfo.Exists)   return;
		FileInfo[] fileInfos = dictoryInfo.GetFiles("*.cs", SearchOption.AllDirectories);
		foreach (FileInfo files in fileInfos)
		{
			string path = files.FullName;
			int StartIndex = path.IndexOf("Assets\\");
			string assetPath =  path.Substring(StartIndex);
			TextAsset textAsset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
			string text = textAsset.text;
			//用正则表达式把代码里面两种字符串中间的字符串提取出来。
			Regex reg = new Regex("NGUIUtil.ShowTipWnd\\(\".*?\"");
			MatchCollection mc = reg.Matches(text);
			foreach(Match m in mc)
			{
				string format = m.Value;
				format = format.Replace("NGUIUtil.ShowTipWnd(\"","");
				format = format.Replace("\"","");
				if(!LocalStringList.Contains(format) && !string.IsNullOrEmpty(format)){
					LocalStringList.Add(format);
					staticWriteText+=format+"\n";
				}
			}
			Regex reg2 = new Regex("NGUIUtil.SetLableText\\(\".*?\"");
			MatchCollection mc2 = reg2.Matches(text);
			foreach(Match m in mc2)
			{
				string format = m.Value;
				format = format.Replace("NGUIUtil.SetLableTex(\"","");
				format = format.Replace("\"","");
				if(!LocalStringList.Contains(format) && !string.IsNullOrEmpty(format)){
					LocalStringList.Add(format);
					staticWriteText+=format+"\n";
				}
			}
			Regex reg3 = new Regex("string.Format\\(\".*?\"");
			MatchCollection mc3 = reg3.Matches(text);
			foreach(Match m in mc3)
			{
				string format = m.Value;
				format = format.Replace("string.Format(\"","");
				format = format.Replace("\"","");
				if(!LocalStringForMatList.Contains(format) && !string.IsNullOrEmpty(format)){
					LocalStringForMatList.Add(format);
					FormatstaticWriteText+=format+"\n";
				}
			}
		}
	}
	static public void SearchPrefabString(Transform root)
	{
		foreach(Transform chind in root)
		{
			//因为这里是写例子，所以我用的是UILabel
			//这里应该是写你用于图文混排的脚本。
			UILabel label = chind.GetComponent<UILabel>();
			if(label != null)
			{
				string text = label.text;
				if(!LocalStringList.Contains(text) && !string.IsNullOrEmpty(text))
				{
					LocalStringList.Add(text);
					text = text.Replace("\n",@"\n");
					staticWriteText+=text+"\n";
				}
			}
			if(chind.childCount  >0)
				SearchPrefabString(chind);
		}
	}
}

