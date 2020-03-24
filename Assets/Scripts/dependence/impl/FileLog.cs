using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// 文件日志的实现
/// </summary>
public class FileLog  {

	/// <summary>
	/// 写文件日志
	/// </summary>
	/// <param name="SceneID">life对象ID</param>
	/// <param name="LineText">写入的一行文本</param>
	/// <param name="append">是否追加写入</param>
	public static void write(int SceneID,string LineText,bool Append = true)
	{
		//return;
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (!Directory.Exists("d:\\log"))
			Directory.CreateDirectory("d:\\log");
		//文件路径
		string strPath="d:\\log\\"+ SceneID +".txt";
		
		//声明写入流
		StreamWriter writer;
		writer = new StreamWriter(strPath,Append);
		//写入文本行，多行可用循环
		writer.WriteLine(LineText);
		//写完数据关闭流
		writer.Close();
		writer=null;
		#endif
	}
	public static string Getnode(XmlNode parent,int tap)
	{
		string txtnode = "";
		string t = "";
		for(int i = 0; i <tap;i++)
			t += " \t ";
		foreach(XmlNode node in parent.ChildNodes)
		{

			txtnode += t + node.Name;
			if (node.HasChildNodes)
			{
				if (node.ChildNodes.Count == 1)
				{
					txtnode += " = " + node.InnerText + "\r\n";
					if (node.ChildNodes[0].HasChildNodes)
					{
						txtnode += "{ \r\n";
						txtnode += Getnode(node, tap +1);
						txtnode += "\r\n " + t +"}\r\n";
					}
				}
				else
				{
					txtnode += "{ \r\n";
					txtnode += Getnode(node, tap +1);
					txtnode += "\r\n " + t +"}\r\n";
				}
			}
			else
			{
				txtnode += " = " + node.InnerText + "\r\n";
			}
		}
		return txtnode;
	}

	//协议dump
	public static void ProtocolDump(object obj)
	{
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (obj == null) return ;
		StringWriter sw = new StringWriter();
		XmlSerializer s = new XmlSerializer(obj.GetType());
		s.Serialize(sw, obj);
		ProtocolDumpwrite(DateTime.Now.ToString());
		XmlDocument xx = new XmlDocument();
		xx.LoadXml(sw.ToString());
		//xx.l
		string txt = "";
		int tap = 0;
		foreach(XmlNode node in xx.ChildNodes)
		{
			txt += node.Name;
			if (node.HasChildNodes)
			{
				txt += "{ \r\n";
				txt += Getnode(node,tap);
				txt += "\r\n}\r\n";
			}
			else
				txt += " = " + node.InnerText + "\r\n";


		}
		//string ss = sw.ToString();
		ProtocolDumpwrite (txt);
		ProtocolDumpwriteXml(DateTime.Now.ToString());
		ProtocolDumpwriteXml(sw.ToString());
		#endif
	}
	public static string txtProtocolDump = "protocol";
	public static void ProtocolDumpwrite(string LineText,bool Append = true)
	{
		//return;
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (!Directory.Exists("d:\\log\\Protocol"))
			Directory.CreateDirectory("d:\\log\\Protocol");
		/*if (!Append)
			txtProtocolDump = GlobalTimer.GetNowTime().ToString();*/
		//文件路径
		string strPath="d:\\log\\Protocol\\"+ txtProtocolDump +".txt";
		
		//声明写入流
		StreamWriter writer;
		writer = new StreamWriter(strPath,Append);
		//写入文本行，多行可用循环
		writer.WriteLine(LineText);
		//写完数据关闭流
		writer.Close();
		writer=null;
		#endif
	}
	public static void ProtocolDumpwriteXml(string LineText,bool Append = true)
	{
		//return;
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (!Directory.Exists("d:\\log\\Protocol"))
			Directory.CreateDirectory("d:\\log\\Protocol");
		/*if (!Append)
			txtProtocolDump = GlobalTimer.GetNowTime().ToString();*/
		//文件路径
		string strPath="d:\\log\\Protocol\\"+ txtProtocolDump +".xml";
		
		//声明写入流
		StreamWriter writer;
		writer = new StreamWriter(strPath,Append);
		//写入文本行，多行可用循环
		writer.WriteLine(LineText);
		//写完数据关闭流
		writer.Close();
		writer=null;
		#endif
	}
}
