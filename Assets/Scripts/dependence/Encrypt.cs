using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// 加密相关接口 
/// </summary>
public class Encrypt
{
	public static byte[] Des(string data)
	{
        try
        {
            // Create a MemoryStream.
            MemoryStream mStream = new MemoryStream();

            // Create a new DES object.
            DES DESalg = DES.Create();
			DESalg.Mode = CipherMode.ECB;
			DESalg.Padding = PaddingMode.None;

            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            CryptoStream cStream = new CryptoStream(mStream, 
                DESalg.CreateEncryptor(DesKey, DesIV), 
                CryptoStreamMode.Write);

            // Convert the passed string to a byte array.
            byte[] dataBytes = new ASCIIEncoding().GetBytes(data);
			int alignSize = (dataBytes.Length + 7) / 8 * 8;
			byte[] toEncrypt = new byte[alignSize];
			System.Array.Copy(dataBytes, toEncrypt, dataBytes.Length);

            // Write the byte array to the crypto stream and flush it.
            cStream.Write(toEncrypt, 0, toEncrypt.Length);
            cStream.FlushFinalBlock();
        
            // Get an array of bytes from the 
            // MemoryStream that holds the 
            // encrypted data.
            byte[] ret = mStream.ToArray();

            // Close the streams.
            cStream.Close();
            mStream.Close();

            // Return the encrypted buffer.
            return ret;
        }
        catch(CryptographicException e)
        {
            Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            return null;
        }
	}

	public static string Des(byte[] data)
	{
    	try
    	{
            // Create a new MemoryStream using the passed 
            // array of encrypted data.
            MemoryStream msDecrypt = new MemoryStream(data);

            // Create a new DES object.
            DES DESalg = DES.Create();
			DESalg.Mode = CipherMode.ECB;
			DESalg.Padding = PaddingMode.None;
			
            // Create a CryptoStream using the MemoryStream 
            // and the passed key and initialization vector (IV).
            CryptoStream csDecrypt = new CryptoStream(msDecrypt, 
                DESalg.CreateDecryptor(DesKey, DesIV), 
                CryptoStreamMode.Read);

            // Create buffer to hold the decrypted data.
            byte[] fromEncrypt = new byte[data.Length];

            // Read the decrypted data out of the crypto stream
            // and place it into the temporary buffer.
            csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

            //Convert the buffer into a string and return it.
            return new ASCIIEncoding().GetString(fromEncrypt);
        }
        catch(CryptographicException e)
        {
            Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
            return null;
        }
	}
	
	public static byte[] Md5(byte[] data)
	{
		// This is one implementation of the abstract class MD5.
		MD5 md5 = new MD5CryptoServiceProvider();

		return md5.ComputeHash(data);
	}

	private static byte[] DesKey = System.Text.ASCIIEncoding.UTF8.GetBytes("77777777");
	private static byte[] DesIV = System.Text.ASCIIEncoding.UTF8.GetBytes("19770531");
}
