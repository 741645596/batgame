using System;
using System.IO;
using UnityEngine;
using System.Text;
using System.Collections.Generic;


/// <summary>
/// 文件/目录的常用接口封装 
/// </summary>
public class NDFileUtil
{
	/// <summary>
	/// 根据路径取得文件名(不包含后缀名)
	/// </summary>
	public static string FileName(string path)
	{
		return Path.GetFileNameWithoutExtension(path);
	}
		
	/// <summary>
	/// 读取文本文件内容 
	/// </summary>
	public static string ReadFile(string file)
	{
#if UNITY_WEBPLAYER
		return string.Empty;
#elif UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			WWW www = new WWW(file);
			while (! www.isDone && string.IsNullOrEmpty(www.error))
			{
				// wait...
			}
			return www.text;
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
			return string.Empty;
		}
#else
        if (File.Exists(file))
		    return File.ReadAllText(file);
        return string.Empty;
#endif
	}
	
	/// <summary>
	/// 读取资源文件内容(已经打包了)
	/// </summary>
	public static string ReadResourceFile(string file)
	{
		TextAsset ta = Resources.Load(file, typeof(TextAsset)) as TextAsset;
		if (ta == null)
		{
			Debug.Log("FileUtil.cs 读取资源文件"+file+"失败");
			return string.Empty;
		}
		
		return ta.text;
	}
	
	/// <summary>
	/// 逐行读取内容 
	/// </summary>
	public static string[] ReadLines(string file)
	{
#if UNITY_WEBPLAYER
		return new string[] {};
#elif UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			WWW www = new WWW(file);
			while (! www.isDone && string.IsNullOrEmpty(www.error))
			{
				// wait...
			}
			return GameUtil.Explode(www.text, "\n");
		}
		catch (Exception e)
		{
			Debug.LogError(e.ToString());
			return new string[] {};
		}		
#else
        if (File.Exists(file))
		    return File.ReadAllLines(file);
        else
            return new string[] {};
#endif		
	}
	
	/// <summary>
	/// 读取资源文件的内容
	/// </summary>
	public static string[] ReadResourceLines(string file)
	{
		return GameUtil.Explode(ReadResourceFile(file), "\n");
	}
	
	/// <summary>
	/// sqlite数据库文件 
	/// </summary>
	public static string SqliteFile
	{
		get { return string.Format("{0}/sqlite/config.sl3", StreamingAssetsPath); }
	}
	
	/// <summary>
	/// 本地配置文件
	/// </summary>
	public static string ConfigFile
	{
		get { return string.Format("{0}/sqlite/local.sl3", StreamingAssetsPath); }
	}

	/// <summary>
	/// 取得StreamingAssets的路径
	/// </summary>
	public static string StreamingAssetsPath
	{
		get { return Application.streamingAssetsPath; }
	}

	/// <summary>
	/// 复制目录，如果比较新才覆盖
	/// </summary>
	public static void CopyDir(string srcPath, string dstPath)
	{
		srcPath = srcPath.Replace("\\", "/");
		dstPath = dstPath.Replace("\\", "/");
		if (srcPath.EndsWith(".svn"))
			return;
		
		// 确保目标路径存在
		if (! Directory.Exists(dstPath))
			Directory.CreateDirectory(dstPath);
		
		// 取得此路径下面所有的文件并复制
		foreach (string file in Directory.GetFiles(srcPath))
		{
			if (file.EndsWith(".meta"))
				continue;
			
			string fileName = Path.GetFileName(file);
			File.Copy(file, dstPath + "/" + fileName, true);
		}
		
		// 递归子目录
		foreach (string path in Directory.GetDirectories(srcPath))
		{
			string p = path.Replace("\\", "/");
			CopyDir(p, dstPath + "/" + p.Substring(p.LastIndexOf("/") + 1));
		}
	}



	public static void GetFile(string srcPath)
	{
		// 取得此路径下面所有的文件并复制
		foreach (string file in Directory.GetFiles(srcPath))
		{
			if (file.EndsWith(".meta"))
				continue;

			Debug.Log("file name:" +file);
		}
	}

	//获取文本文件的编码方式
	public static Encoding GetEncoding(string fileName)
	{
		return GetEncoding(fileName, Encoding.Default);

	}
	public static Encoding GetEncoding(string fileName, Encoding defaultEncoding)
	{
		FileStream fs = new FileStream(fileName, FileMode.Open);
		Encoding targetEncoding = GetEncoding(fs, defaultEncoding);
		fs.Close();
		return targetEncoding;
	}
	

	public static Encoding GetEncoding(FileStream stream, Encoding defaultEncoding)
	{
		Encoding targetEncoding = defaultEncoding;
		if(stream != null && stream.Length >= 2)
		{
			//保存文件流的前4个字节
			byte byte1 = 0;
			byte byte2 = 0;
			byte byte3 = 0;
			byte byte4 = 0;
			
			//保存当前Seek位置
			long origPos = stream.Seek(0, SeekOrigin.Begin);
			stream.Seek(0, SeekOrigin.Begin);
			int nByte = stream.ReadByte();
			byte1 = Convert.ToByte(nByte);
			byte2 = Convert.ToByte(stream.ReadByte());
			
			if(stream.Length >= 3)
			{
				byte3 = Convert.ToByte(stream.ReadByte());
			}
			
			if(stream.Length >= 4)
			{
				byte4 = Convert.ToByte(stream.ReadByte());
			}
			//根据文件流的前4个字节判断Encoding
			//Unicode {0xFF, 0xFE};
			//BE-Unicode {0xFE, 0xFF};
			//UTF8 = {0xEF, 0xBB, 0xBF};
			
			if(byte1 == 0xFE && byte2 == 0xFF)//UnicodeBe
			{
				targetEncoding = Encoding.BigEndianUnicode;
			}
			
			if(byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)//Unicode
			{
				targetEncoding = Encoding.Unicode;
			}
			
			if(byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)//UTF8
			{
				targetEncoding = Encoding.UTF8;
			}
			
			//恢复Seek位置
			stream.Seek(origPos, SeekOrigin.Begin);
		} 
		return targetEncoding; 
	}



}

