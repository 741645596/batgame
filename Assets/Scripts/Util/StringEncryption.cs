using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

using UnityEngine;

public class StringEncryption{
	
	#region  方法一 C#中对字符串加密解密（对称算法）
	private static byte[] Keys = { 0xEF, 0xAB, 0x56, 0x78, 0x90, 0x34, 0xCD, 0x12 };
	/// <summary>
	/// DES加密字符串
	/// </summary>
	/// <param name="encryptString">待加密的字符串</param>
	/// <param name="encryptKey">加密密钥,要求为8位</param>
	/// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
	///

	public static string EncryptDES(string encryptString, string encryptKey)
	{
		try
		{
			byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey);
			byte[] rgbIV = Keys;
			byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
			DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
			dCSP.Mode = CipherMode.ECB;
			dCSP.Padding = PaddingMode.Zeros;
			//dCSP.KeySize=64;
			MemoryStream mStream = new MemoryStream();
			CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
			cStream.Write(inputByteArray, 0, inputByteArray.Length);
			cStream.FlushFinalBlock();
			cStream.Close();

			//return Convert.ToBase64String(mStream.ToArray());
			string t2 = BitConverter.ToString(mStream.ToArray()); 
			t2 = t2.Replace("-", ""); 
			return t2;
		}
		catch
		{
			return encryptString;
		}
	}

	/// <summary>
	/// DES解密字符串
	/// </summary>
	/// <param name="decryptString">待解密的字符串</param>
	/// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
	/// <returns>解密成功返回解密后的字符串，失败返源串</returns>
	public static string DecryptDES(string decryptString, string decryptKey)
	{
		try
		{
			byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
			byte[] rgbIV = Keys;
			byte[] inputByteArray = Convert.FromBase64String(decryptString);
			DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
			MemoryStream mStream = new MemoryStream();
			CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
			cStream.Write(inputByteArray, 0, inputByteArray.Length);
			cStream.FlushFinalBlock();
			cStream.Close();
			return Encoding.UTF8.GetString(mStream.ToArray());
		}
		catch
		{
			Debug.Log("catch");
			return decryptString;
		}
	}

	#endregion
	
	
	
	
	
	
	#region MD5不可逆加密
	//32位加密
	public string GetMD5_32(string s, string _input_charset) 
	{ 
		MD5 md5 = new MD5CryptoServiceProvider(); 
		byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s)); 
		StringBuilder sb = new StringBuilder(32); 
		for (int i = 0; i < t.Length; i++) 
		{ 
			sb.Append(t[i].ToString("x").PadLeft(2, '0')); 
		} 
		return sb.ToString(); 
	} 
	
	//16位加密 
	public static string GetMd5_16(string decryptString) 
	{ 

		MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider(); 
		string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(decryptString)), 4, 8); 
		t2 = t2.Replace("-", ""); 
		return t2; 
	}


	public static string CutString(string decryptString)
	{
		int []SelByte={0,1,2,3,4,5,6,7};
		string strMd5=GetMd5_16(decryptString);
		Debug.Log(strMd5);
		StringBuilder key = new StringBuilder(8); 
		for (int i = 0; i < 8; i++) 
		{ 
			key.Append(strMd5.Substring(SelByte[i],1));
		} 
		return key.ToString();
	}

	#endregion
}
