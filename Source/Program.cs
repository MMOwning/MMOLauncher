using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.Owin.Builder;
using Nancy.Owin;
using System.Windows.Forms;
using MMOwningLauncher.Web;
using MMOwningLauncher.Web.WebSocket;
using MMOwningLauncher.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MMOwningLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
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


            //Create Folders if not exist
            if (!Directory.Exists(Globals.DataPath))
            {
                Directory.CreateDirectory(Globals.DataPath);
            }

            if (!Directory.Exists(Globals.BasePath + "\\Bin"))
            {
                Directory.CreateDirectory(Globals.BasePath + "\\Bin");
            }

            if (!Directory.Exists(Globals.BasePath + "\\Logs"))
            {
                Directory.CreateDirectory(Globals.BasePath + "\\Logs");
            }

            if (!Directory.Exists(Globals.BasePath + "\\Temp"))
            {
                Directory.CreateDirectory(Globals.BasePath + "\\Temp");
            }

            if (!Directory.Exists(Globals.BasePath + "\\www"))
            {
                Directory.CreateDirectory(Globals.BasePath + "\\www");
            }

            var webServer = new Web.StartUp();
            webServer.StartRun();



            Globals.MainConfig["mainConfig"] = new Dictionary<string, dynamic>();
            Globals.MainConfig["downloadUrls"] = new Dictionary<string, dynamic>();
            Globals.MainConfig["downloadUrls"]["electron"] = "https://github.com/atom/electron/releases/download/v0.36.1/electron-v0.36.1-win32-ia32.zip";

            Globals.BinConfig["authserver"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["authserver"]["autostart"] = false;
            Globals.BinConfig["authserver"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["authserver"]["start"]["FileName"] = "authserver.exe";
            Globals.BinConfig["authserver"]["start"]["Arguments"] = "-c authserver.conf";
            Globals.BinConfig["authserver"]["start"]["WorkingDirectory"] = "Bin\\trinitycore\\";
            Globals.BinConfig["authserver"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["authserver"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["authserver"]["stop"]["FileName"] = "kill";
            Globals.BinConfig["authserver"]["stop"]["Arguments"] = "authserver";
            Globals.BinConfig["authserver"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["authserver"]["stop"]["ShowCmd"] = false;

            Globals.BinConfig["worldserver"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["worldserver"]["autostart"] = false;
            Globals.BinConfig["worldserver"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["worldserver"]["start"]["FileName"] = "worldserver.exe";
            Globals.BinConfig["worldserver"]["start"]["Arguments"] = "-c worldserver.conf";
            Globals.BinConfig["worldserver"]["start"]["WorkingDirectory"] = "Bin\\trinitycore\\";
            Globals.BinConfig["worldserver"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["worldserver"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["worldserver"]["stop"]["FileName"] = "kill";
            Globals.BinConfig["worldserver"]["stop"]["Arguments"] = "worldserver";
            Globals.BinConfig["worldserver"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["worldserver"]["stop"]["ShowCmd"] = false;

            Globals.BinConfig["nginx"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["nginx"]["autostart"] = false;
            Globals.BinConfig["nginx"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["nginx"]["start"]["FileName"] = "Bin\\nginx\\nginx.exe";
            Globals.BinConfig["nginx"]["start"]["Arguments"] = "-c Bin/nginx/conf/nginx.conf";
            Globals.BinConfig["nginx"]["start"]["WorkingDirectory"] = "";
            Globals.BinConfig["nginx"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["nginx"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["nginx"]["stop"]["FileName"] = "Bin\\nginx\\nginx.exe";
            Globals.BinConfig["nginx"]["stop"]["Arguments"] = "-c Bin/nginx/conf/nginx.conf -s stop";
            Globals.BinConfig["nginx"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["nginx"]["stop"]["ShowCmd"] = false;

            Globals.BinConfig["mysql"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mysql"]["autostart"] = false;
            Globals.BinConfig["mysql"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mysql"]["start"]["FileName"] = "Bin\\mariadb\\bin\\mysqld.exe";
            Globals.BinConfig["mysql"]["start"]["Arguments"] = "";
            Globals.BinConfig["mysql"]["start"]["WorkingDirectory"] = "";
            Globals.BinConfig["mysql"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["mysql"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mysql"]["stop"]["FileName"] = "Bin\\mariadb\\bin\\mysqladmin.exe";
            Globals.BinConfig["mysql"]["stop"]["Arguments"] = "--defaults-file=Bin\\mariadb\\my.ini -uroot --password=\"\" -h127.0.0.1 --protocol=tcp shutdown";
            Globals.BinConfig["mysql"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["mysql"]["stop"]["ShowCmd"] = false;

            Globals.BinConfig["php"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["php"]["autostart"] = false;
            Globals.BinConfig["php"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["php"]["start"]["FileName"] = "Bin\\php\\php-cgi.exe";
            Globals.BinConfig["php"]["start"]["Arguments"] = "-b localhost:9100";
            Globals.BinConfig["php"]["start"]["WorkingDirectory"] = "";
            Globals.BinConfig["php"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["php"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["php"]["stop"]["FileName"] = "kill";
            Globals.BinConfig["php"]["stop"]["Arguments"] = "php-cgi";
            Globals.BinConfig["php"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["php"]["stop"]["ShowCmd"] = false;

            Helpers.MergeCsDictionaryAndSave(Globals.MainConfig, Globals.DataPath + "\\MainConfig.json");
            Helpers.MergeCsDictionaryAndSave(Globals.BinConfig, Globals.DataPath + "\\BinConfig.json");           

            //Download Electron if not exist
            if (!Directory.Exists(Globals.AppPath + "\\Runtime\\Electron"))
            {
                Directory.CreateDirectory(Globals.AppPath + "\\Runtime\\Temp\\Download");
                Form_Download downloadForm = new Form_Download(Globals.MainConfig["downloadUrls"]["electron"], Globals.AppPath + "\\Runtime\\Temp\\Download\\electron.zip");

                if (downloadForm.ShowDialog() == DialogResult.OK)
                {
                    Directory.CreateDirectory(Globals.AppPath + "\\Runtime\\Temp\\Extract");
                    Helpers.ExtractFile(Globals.AppPath + "\\Runtime\\Temp\\Download\\electron.zip", Globals.AppPath + "\\Runtime\\Temp\\Extract");

                    DirectoryInfo dinfo = new DirectoryInfo(Globals.AppPath + "\\Runtime\\Temp\\Extract");
                    DirectoryInfo[] directorys = dinfo.GetDirectories();

                    //Check if the file is etxracted to subfolder - Files on github includes branch name -> Correct this
                    Directory.Move(Globals.AppPath + "\\Runtime\\Temp\\Extract", Globals.AppPath + "\\Runtime\\Electron");
                    Directory.Delete(Globals.AppPath + "\\Runtime\\Temp\\", true);
                }
            }

            Application.Run(new Form_TrayMenu());
        }
    }



    //Define Global Variables 
    public static class Globals
    {
        public static WebSocketServer GlobalWebsocketServer;
        public static Dictionary<string, dynamic> BinConfig = new Dictionary<string, dynamic>();

        public static Dictionary<string, dynamic> AppsDictionary = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> WindowsStartMenu = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> MainConfig = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> RuntimeSettings = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> Folders = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> GamePadProfile = new Dictionary<string, dynamic>();
        public static Dictionary<string, dynamic> RemoteProfile = new Dictionary<string, dynamic>();
        //public static Dictionary<string, dynamic> RunningProcesses = new Dictionary<string, dynamic>();
        //public static string[] RunningProcesses;
        public static Dictionary<string, bool> RunningProcesses = new Dictionary<string, bool>();


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
