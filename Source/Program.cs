using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
            Globals.RuntimePath = Globals.BasePath;
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

            if (!Directory.Exists(Globals.DataPath + "\\Cache"))
            {
                Directory.CreateDirectory(Globals.DataPath + "\\Cache");
                Directory.CreateDirectory(Globals.DataPath + "\\Cache\\Icons");
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





            Globals.MainConfig["mainConfig"] = new Dictionary<string, dynamic>();
            Globals.MainConfig["mainConfig"]["autostart"] = true;
            Globals.MainConfig["webConfig"] = new Dictionary<string, dynamic>();
            Globals.MainConfig["webConfig"]["webHost"] = "127.0.0.1";
            Globals.MainConfig["webConfig"]["webPort"] = 8888;
            Globals.MainConfig["webConfig"]["webSocketPort"] = 8889;
            Globals.MainConfig["downloadUrls"] = new Dictionary<string, dynamic>();
            Globals.MainConfig["downloadUrls"]["electron"] = "https://github.com/atom/electron/releases/download/v0.36.1/electron-v0.36.1-win32-ia32.zip";
            Globals.MainConfig["downloadUrls"]["mmowning_bin"] = "https://github.com/MMOwning/MMOLauncher/releases/download/0.5.0.0/Bin_0.5.0.0.7z";

            Globals.BinConfig["authserver"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["authserver"]["autostart"] = false;
            Globals.BinConfig["authserver"]["name"] = "Trinitycore Authserver";
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
            Globals.BinConfig["authserver"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["authserver"]["start"]["WorkingDirectory"] + "\\" + Globals.BinConfig["authserver"]["start"]["FileName"]))
            {
                Globals.BinConfig["authserver"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\authserver" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["authserver"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\authserver_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }


            Globals.BinConfig["worldserver"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["worldserver"]["autostart"] = false;
            Globals.BinConfig["worldserver"]["name"] = "Trinitycore Worldserver";
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
            Globals.BinConfig["worldserver"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["worldserver"]["start"]["WorkingDirectory"] + "\\" + Globals.BinConfig["worldserver"]["start"]["FileName"]))
            {
                Globals.BinConfig["worldserver"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\worldserver" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["worldserver"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\worldserver_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }


            Globals.BinConfig["nginx"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["nginx"]["autostart"] = false;
            Globals.BinConfig["nginx"]["name"] = "nginx";
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
            Globals.BinConfig["nginx"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["nginx"]["start"]["FileName"]))
            {
                Globals.BinConfig["nginx"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\nginx" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["nginx"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\nginx_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }



            Globals.BinConfig["mysql"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mysql"]["autostart"] = false;
            Globals.BinConfig["mysql"]["name"] = "MySQL";
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
            Globals.BinConfig["mysql"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["mysql"]["start"]["FileName"]))
            {
                Globals.BinConfig["mysql"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\mysql" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["mysql"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\mysql_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }


            Globals.BinConfig["php"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["php"]["autostart"] = false;
            Globals.BinConfig["php"]["name"] = "PHP";
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
            Globals.BinConfig["php"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["php"]["start"]["FileName"]))
            {
                Globals.BinConfig["php"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\php" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["php"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\php_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }


            Globals.BinConfig["postgres"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["postgres"]["autostart"] = false;
            Globals.BinConfig["postgres"]["name"] = "PostgreSQL";
            Globals.BinConfig["postgres"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["postgres"]["start"]["FileName"] = "Bin\\pgsql\\bin\\pg_ctl.exe";
            Globals.BinConfig["postgres"]["start"]["Arguments"] = "--pgdata /bin/pgsql/data --log /logs/postgresql.log start";
            Globals.BinConfig["postgres"]["start"]["WorkingDirectory"] = "";
            Globals.BinConfig["postgres"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["postgres"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["postgres"]["stop"]["FileName"] = "Bin\\pgsql\\bin\\pg_ctl.exe";
            Globals.BinConfig["postgres"]["stop"]["Arguments"] = "stop --pgdata /bin/pgsql/data --log /logs/postgresql.log --mode=fast -W";
            Globals.BinConfig["postgres"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["postgres"]["stop"]["ShowCmd"] = false;
            Globals.BinConfig["postgres"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["postgres"]["start"]["FileName"]))
            {
                Globals.BinConfig["postgres"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\postgres" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["postgres"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\postgres_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }

            Globals.BinConfig["mongo"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mongo"]["autostart"] = false;
            Globals.BinConfig["mongo"]["name"] = "MongoDB";
            Globals.BinConfig["mongo"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mongo"]["start"]["FileName"] = "Bin\\mongodb\\bin\\mongo.exe";
            Globals.BinConfig["mongo"]["start"]["Arguments"] = "--config /bin/mongodb/mongodb.conf --dbpath /bin/mongodb/data/db --logpath /logs/mongodb.log --rest";
            Globals.BinConfig["mongo"]["start"]["WorkingDirectory"] = "";
            Globals.BinConfig["mongo"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["mongo"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["mongo"]["stop"]["FileName"] = "Bin\\mongodb\\bin\\mongo.exe";
            Globals.BinConfig["mongo"]["stop"]["Arguments"] = "--eval \"db.getSiblingDB('admin').shutdownServer()\"";
            Globals.BinConfig["mongo"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["mongo"]["stop"]["ShowCmd"] = false;
            Globals.BinConfig["mongo"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["mongo"]["start"]["FileName"]))
            {
                Globals.BinConfig["mongo"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\mongo" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["mongo"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\mongo_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }

            Globals.BinConfig["memcached"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["memcached"]["autostart"] = false;
            Globals.BinConfig["memcached"]["name"] = "Memcached";
            Globals.BinConfig["memcached"]["start"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["memcached"]["start"]["FileName"] = "Bin\\memcached\\memcached.exe";
            Globals.BinConfig["memcached"]["start"]["Arguments"] = "";
            Globals.BinConfig["memcached"]["start"]["WorkingDirectory"] = "";
            Globals.BinConfig["memcached"]["start"]["ShowCmd"] = false;
            Globals.BinConfig["memcached"]["stop"] = new Dictionary<string, dynamic>();
            Globals.BinConfig["memcached"]["stop"]["FileName"] = "kill";
            Globals.BinConfig["memcached"]["stop"]["Arguments"] = "memcached";
            Globals.BinConfig["memcached"]["stop"]["WorkingDirectory"] = "";
            Globals.BinConfig["memcached"]["stop"]["ShowCmd"] = false;
            Globals.BinConfig["memcached"]["installed"] = false;
            if (File.Exists(Globals.RuntimePath + "\\" + Globals.BinConfig["memcached"]["start"]["FileName"]))
            {
                Globals.BinConfig["memcached"]["installed"] = true;
            }
            if (!File.Exists(Globals.Folders["datapath"] + "\\cache\\icons\\memcached" + "_16x16.png"))
            {
                try
                {
                    Bitmap bitmap_16 = ShellEx.GetBitmapFromFilePath(Globals.RuntimePath + "\\" + Globals.BinConfig["memcached"]["start"]["FileName"], ShellEx.IconSizeEnum.MediumIcon32);
                    string newFileName_16 = Globals.DataPath + "\\Cache\\Icons\\memcached_16x16.png";
                    bitmap_16.Save(newFileName_16, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                }
            }


            Globals.MainConfig = Helpers.MergeCsDictionaryAndSave(Globals.MainConfig, Globals.DataPath + "\\MainConfig.json");
            Globals.BinConfig = Helpers.MergeCsDictionaryAndSave(Globals.BinConfig, Globals.DataPath + "\\BinConfig.json");
         

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

            //Download Electron if not exist
            if (!Directory.Exists(Globals.BasePath + "\\Bin"))
            {
                Directory.CreateDirectory(Globals.AppPath + "\\Runtime\\Temp\\Download");
                Form_Download downloadForm = new Form_Download(Globals.MainConfig["downloadUrls"]["mmowning_bin"], Globals.AppPath + "\\Runtime\\Temp\\Download\\mmo_bin.zip");

                if (downloadForm.ShowDialog() == DialogResult.OK)
                {
                    Directory.CreateDirectory(Globals.AppPath + "\\Runtime\\Temp\\Extract");
                    Helpers.ExtractFile(Globals.AppPath + "\\Runtime\\Temp\\Download\\mmo_bin.zip", Globals.AppPath + "\\Runtime\\Temp\\Extract");

                    DirectoryInfo dinfo = new DirectoryInfo(Globals.AppPath + "\\Runtime\\Temp\\Extract");
                    DirectoryInfo[] directorys = dinfo.GetDirectories();

                    //Check if the file is etxracted to subfolder - Files on github includes branch name -> Correct this
                    Directory.Move(Globals.AppPath + "\\Runtime\\Temp\\Extract", Globals.BasePath + "\\Bin");
                    Directory.Delete(Globals.AppPath + "\\Runtime\\Temp\\", true);
                }
            }


            var webServer = new Web.StartUp();
            webServer.StartRun();

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
