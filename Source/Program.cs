using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.Owin.Builder;
using MMOLauncher.Classes;
using Nancy.Owin;
using Owin.WebSocket;
using Owin.WebSocket.Extensions;

namespace MMOLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            var webServer = new Web.StartUp();
            webServer.StartRun();

            //Add Globals
            var scriptpath = Directory.GetCurrentDirectory();

            Globals.Folders["scriptpath"] = scriptpath;
            Globals.Folders["apppath"] = scriptpath + "\\Apps";
            Globals.Folders["datapath"] = scriptpath + "\\Data";
            Globals.Folders["configpath"] = scriptpath + "\\Data\\Config";

            Globals.RuntimeSettings["platform"] = new Dictionary<string, dynamic>();
            Globals.RuntimeSettings["platform"]["os"] = Environment.OSVersion.ToString();
            Globals.RuntimeSettings["platform"]["computername"] = System.Environment.MachineName.ToString();
            Globals.RuntimeSettings["platform"]["username"] = Environment.UserName;
            var checkAdminstrator = new IsUserAdministrator();
            Globals.RuntimeSettings["platform"]["runningasadmin"] = checkAdminstrator.Check();

            Globals.MainSettings["mainConfig"] = new Dictionary<string, dynamic>();

            Globals.RuntimeSettings["windows"] = new Dictionary<string, dynamic>();
            Globals.RuntimeSettings["windows"]["currentUser"] = Environment.UserName;
            //C:\ProgramData\Microsoft\User Account Pictures\user.png or %username%.png
            Globals.RuntimeSettings["windows"]["currentUserAvatar"] = "";


            //Set Path
            //ExeFile -> C:\PortableApps\CygwinPortable\App\CygwinPortable.exe
            Globals.ExeFile = Assembly.GetExecutingAssembly().Location;
            //AppPath -> C:\PortableApps\CygwinPortable\App
            Globals.AppPath = Path.GetDirectoryName(Globals.ExeFile);
            //BasePath -> C:\PortableApps\CygwinPortable
            Globals.BasePath = Directory.GetParent(Path.GetDirectoryName(Globals.ExeFile)).FullName;
            //RootPath -> C:\
            Globals.RootPath = Path.GetPathRoot(Globals.ExeFile);
            //DataPath -> C:\PortableApps\CygwinPortable\Data
            Globals.DataPath = Globals.BasePath + "\\Data";
            //DataPath -> C:\PortableApps\CygwinPortable\Data
            Globals.RuntimePath = Globals.BasePath + "\\Runtime";
            //ConfigPath -> C:\PortableApps\CygwinPortable\Data
            Globals.ConfigPath = Globals.BasePath + "\\Data";
            //ParentBasePath -> Get Parent Folder of CygwinPortable -> C:\
            DirectoryInfo parentBasePath = new DirectoryInfo(Globals.BasePath);
            Globals.ParentBasePath = parentBasePath.Parent.FullName;
            //ParentParentBasePath -> Check if PortableApps is installed in Subfolder e.g. C:\Programs\PortableApps
            if (Globals.ParentBasePath != Globals.RootPath)
            {
                DirectoryInfo parentParentBasePath = new DirectoryInfo(Globals.ParentBasePath);
                Globals.ParentParentBasePath = parentParentBasePath.Parent.FullName;
            }




            //Blocking Console
            Console.ReadKey();
        }
    }

    //Define Global Variables 
    public static class Globals
    {
        public static Dictionary<string, dynamic> AppsDictionary = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> WindowsStartMenu = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> MainSettings = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> RuntimeSettings = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> Folders = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> GamePadProfile = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> RemoteProfile = new Dictionary<string, dynamic>();
        //public static Dictionary<string, dynamic> RunningProcesses = new Dictionary<string, dynamic>();
        //public static string[] RunningProcesses;
        public static List<string> RunningProcesses = new List<string>();
        public static List<string> RunningProcessesToolhelp32 = new List<string>();


        public static string ExeFile = "";
        public static string AppPath = "";
        public static string BasePath = "";
        public static string RootPath = "";
        public static string DataPath = "";
        public static string RuntimePath = "";
        public static string ConfigPath = "";
        public static string PortableAppsPath = "";
        public static string ParentBasePath = "";
        public static string ParentParentBasePath = "";

    }

}
