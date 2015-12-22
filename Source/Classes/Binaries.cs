using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MMOLauncher.Classes
{
    class Binaries
    {

        public static void RunProgram(string command, string FileName, string Arguments, string WorkingDirectory, bool ShowCmd)
        {
            if (command == "openTextFile")
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = Globals.BasePath + "\\" + FileName;
                if (WorkingDirectory != "") cmd.StartInfo.WorkingDirectory = WorkingDirectory;
                if (Arguments != "") cmd.StartInfo.Arguments = Arguments;
                cmd.Start();
            }

            if (command == "start" || command == "stop")
            {
                Process cmd = new Process();
                if (ShowCmd)
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    if (Arguments != "")
                    {
                        cmd.StartInfo.Arguments = "/K " + FileName + " " + Arguments;
                    }
                    else
                    {
                        cmd.StartInfo.Arguments = "/K " + FileName;
                    }
                }
                else
                {
                    cmd.StartInfo.FileName = FileName;
                    if (Arguments != "")
                    {
                        cmd.StartInfo.Arguments = Arguments;
                    }
                }
                if (WorkingDirectory != "")
                {
                    cmd.StartInfo.WorkingDirectory = Globals.BasePath + "\\" + WorkingDirectory;
                }
                else
                {
                    cmd.StartInfo.WorkingDirectory = Globals.BasePath;
                }
                cmd.Start();
            }

            if (command == "kill")
            {
                if (Arguments != "")
                {
                    foreach (var process in Process.GetProcessesByName(Arguments))
                    {
                        process.Kill();
                    }
                }
            }

        }



        public static void CheckRunningProcessTimer(object source, ElapsedEventArgs e)
        {
            Globals.RunningProcesses["nginx_status"] = false;
            Globals.RunningProcesses["php_status"] = false;
            Globals.RunningProcesses["mysql_status"] = false;
            Globals.RunningProcesses["authserver_status"] = false;
            Globals.RunningProcesses["worldserver_status"] = false;

            var processList = new GetProcessesToolhelp32();
            var processListOut = processList.GetListSimple();

            if (processListOut.Contains("nginx.exe"))
            {
                Globals.RunningProcesses["nginx_status"] = true;
            }
            if (processListOut.Contains("php-cgi.exe"))
            {
                Globals.RunningProcesses["php_status"] = true;
            }
            if (processListOut.Contains("mysqld.exe"))
            {
                Globals.RunningProcesses["mysql_status"] = true;
            }

            if (processListOut.Contains("authserver.exe"))
            {
                Globals.RunningProcesses["authserver_status"] = true;
            }
            if (processListOut.Contains("worldserver.exe"))
            {
                Globals.RunningProcesses["worldserver_status"] = true;
            }
        }


    }
}
