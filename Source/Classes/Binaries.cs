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
        public static bool RunProgram(string command, string bin, string fileName = "", string arguments = "", string workingDirectory = "", bool showCmd = false, bool waitForExit = false)
        {
            //Supported commands are start and stop -> Check for others and rewrite
            if (command == "restart")
            {
                if (RunProgram("stop", bin, Globals.BinConfig[bin]["stop"]["FileName"], Globals.BinConfig[bin]["stop"]["Arguments"], Globals.BinConfig[bin]["stop"]["WorkingDirectory"], Globals.BinConfig[bin]["stop"]["ShowCmd"], true))
                {
                    RunProgram("start", bin, Globals.BinConfig[bin]["start"]["FileName"], Globals.BinConfig[bin]["start"]["Arguments"], Globals.BinConfig[bin]["start"]["WorkingDirectory"], Globals.BinConfig[bin]["start"]["ShowCmd"], true);
                }
            }

            if (command == "kill")
            {
                command = "stop";
            }

            if (command == "toggle")
            {
                if (Globals.RunningProcesses[bin])
                {
                    command = "stop";
                }
                else
                {
                    command = "start";
                }
            }

            //If RunProgram is started without arguments find parameters
            if (fileName == "")
            {
                fileName = Globals.BinConfig[bin][command]["FileName"];
            }
            if (arguments == "")
            {
                arguments = Globals.BinConfig[bin][command]["Arguments"];
            }
            if (workingDirectory == "")
            {
                workingDirectory = Globals.BinConfig[bin][command]["WorkingDirectory"];
            }

            if (command == "openTextFile")
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = Globals.BasePath + "\\" + fileName;
                if (workingDirectory != "") cmd.StartInfo.WorkingDirectory = workingDirectory;
                if (arguments != "") cmd.StartInfo.Arguments = arguments;
                cmd.Start();
            }


            if (command == "stop")
            {
                if (Globals.BinConfig[bin][command]["FileName"] == "kill")
                {
                    command = "kill";
                }
            }


            if (command == "start" || command == "stop")
            {
                Process cmd = new Process();
                if (showCmd)
                {
                    cmd.StartInfo.FileName = "cmd.exe";
                    if (arguments != "")
                    {
                        cmd.StartInfo.Arguments = "/K " + fileName + " " + arguments;
                    }
                    else
                    {
                        cmd.StartInfo.Arguments = "/K " + fileName;
                    }
                }
                else
                {
                    cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cmd.StartInfo.FileName = fileName;
                    if (arguments != "")
                    {
                        cmd.StartInfo.Arguments = arguments;
                    }
                }
                if (workingDirectory != "")
                {
                    cmd.StartInfo.WorkingDirectory = Globals.BasePath + "\\" + workingDirectory;
                }
                else
                {
                    cmd.StartInfo.WorkingDirectory = Globals.BasePath;
                }
                if (waitForExit)
                {
                    cmd.Start();
                    cmd.WaitForExit(5000);
                }
                else
                {
                    cmd.Start();
                }
                
            }

            if (command == "kill")
            {
                if (arguments != "")
                {
                    foreach (var process in Process.GetProcessesByName(arguments))
                    {
                        process.Kill();
                    }
                }
            }

            return true;

        }



        public static void CheckRunningProcessTimer(object source, ElapsedEventArgs e)
        {
            Globals.RunningProcesses["nginx"] = false;
            Globals.RunningProcesses["php"] = false;
            Globals.RunningProcesses["mysql"] = false;
            Globals.RunningProcesses["authserver"] = false;
            Globals.RunningProcesses["worldserver"] = false;

            var processList = new GetProcessesToolhelp32();
            var processListOut = processList.GetListSimple();

            if (processListOut.Contains("nginx.exe"))
            {
                Globals.RunningProcesses["nginx"] = true;
            }
            if (processListOut.Contains("php-cgi.exe"))
            {
                Globals.RunningProcesses["php"] = true;
            }
            if (processListOut.Contains("mysqld.exe"))
            {
                Globals.RunningProcesses["mysql"] = true;
            }

            if (processListOut.Contains("authserver.exe"))
            {
                Globals.RunningProcesses["authserver"] = true;
            }
            if (processListOut.Contains("worldserver.exe"))
            {
                Globals.RunningProcesses["worldserver"] = true;
            }
        }


    }
}
