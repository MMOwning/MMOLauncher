using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using MMOLauncher.Classes;
using MMOLauncher.Properties;
using MMOLauncher.Web.WebSocket;
using Newtonsoft.Json;
using Nowin;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MMOLauncher
{
    public partial class Form_TrayMenu : Form
    {
        public Form_TrayMenu()
        {
            InitializeComponent();

            //Check each second running processes
            var runningProcessTimer = new System.Timers.Timer(1000);
            runningProcessTimer.Elapsed += new ElapsedEventHandler(Binaries.CheckRunningProcessTimer);
            runningProcessTimer.Enabled = true;
            //GC.KeepAlive(runningProcessTimer);

            WindowState = FormWindowState.Minimized;
            if ((WindowState == FormWindowState.Minimized))
            {
                contextMenuStrip1.Show();
                Hide();
                ShowInTaskbar = false;
            }
        }

        private void RunElectron_Click(object sender, EventArgs e, string api, string command, string data)
        {
            Dictionary<string, string> electronCommand = new Dictionary<string, string>();
            electronCommand["api"] = api;
            electronCommand["command"] = command;
            electronCommand["data"] = data;
            Globals.GlobalWebsocketServer.WebSocketServices.Broadcast(JsonConvert.SerializeObject(electronCommand));
        }

        private void RunBin_Click(object source, EventArgs e, string bin, string command)
        {
            if (command == "restart")
            {
                Binaries.RunProgram(command, bin, "", "", "", false, false);
            }
            else
            {
                Binaries.RunProgram(command, bin, Globals.BinConfig[bin][command]["FileName"], Globals.BinConfig[bin][command]["Arguments"], Globals.BinConfig[bin][command]["WorkingDirectory"], Globals.BinConfig[bin][command]["ShowCmd"], false);
            }
        }

        private void ContextMenuStrip1Opening()
        {
            contextMenuStrip1.Items.Clear();

            var nginxMenu = new ToolStripMenuItem("nginx");
            contextMenuStrip1.Items.Add(nginxMenu);
            if (Globals.RunningProcesses["nginx"])
            {
                nginxMenu.Image = Resources.ok;
                nginxMenu.DropDownItems.Add("Stop", Resources.no, (sender, e) => { RunBin_Click(sender, e, "nginx", "stop"); });
                nginxMenu.DropDownItems.Add("Restart", Resources.redo, (sender, e) => { RunBin_Click(sender, e, "nginx", "restart"); });
            }
            else
            {
                nginxMenu.Image = Resources.no;
                nginxMenu.DropDownItems.Add("Start", Resources.ok, (sender, e) => { RunBin_Click(sender, e, "nginx", "start"); });
            }

            var phpMenu = new ToolStripMenuItem("php");
            contextMenuStrip1.Items.Add(phpMenu);
            if (Globals.RunningProcesses["php"])
            {
                phpMenu.Image = Resources.ok;
                phpMenu.DropDownItems.Add("Stop", Resources.no, (sender, e) => { RunBin_Click(sender, e, "php", "stop"); });
                phpMenu.DropDownItems.Add("Restart", Resources.redo, (sender, e) => { RunBin_Click(sender, e, "php", "restart"); });
            }
            else
            {
                phpMenu.Image = Resources.no;
                phpMenu.DropDownItems.Add("Start", Resources.ok, (sender, e) => { RunBin_Click(sender, e, "php", "start"); });
            }


            var mysqlMenu = new ToolStripMenuItem("MySQL");
            contextMenuStrip1.Items.Add(mysqlMenu);
            if (Globals.RunningProcesses["mysql"])
            {
                mysqlMenu.Image = Resources.ok;
                mysqlMenu.DropDownItems.Add("Stop", Resources.no, (sender, e) => { RunBin_Click(sender, e, "mysql", "stop"); });
                mysqlMenu.DropDownItems.Add("Restart", Resources.redo, (sender, e) => { RunBin_Click(sender, e, "mysql", "restart"); });
            }
            else
            {
                mysqlMenu.Image = Resources.no;
                mysqlMenu.DropDownItems.Add("Start", Resources.ok, (sender, e) => { RunBin_Click(sender, e, "mysql", "start"); });
            }

            var authserverMenu = new ToolStripMenuItem("Auhserver");
            contextMenuStrip1.Items.Add(authserverMenu);
            if (Globals.RunningProcesses["authserver"])
            {
                authserverMenu.Image = Resources.ok;
                authserverMenu.DropDownItems.Add("Stop", Resources.no, (sender, e) => { RunBin_Click(sender, e, "authserver", "stop"); });
                authserverMenu.DropDownItems.Add("Restart", Resources.redo, (sender, e) => { RunBin_Click(sender, e, "authserver", "restart"); });
            }
            else
            {
                authserverMenu.Image = Resources.no;
                authserverMenu.DropDownItems.Add("Start", Resources.ok, (sender, e) => { RunBin_Click(sender, e, "authserver", "start"); });
            }


            var worldserverMenu = new ToolStripMenuItem("Worldserver");
            contextMenuStrip1.Items.Add(worldserverMenu);
            if (Globals.RunningProcesses["worldserver"])
            {
                worldserverMenu.Image = Resources.ok;
                worldserverMenu.DropDownItems.Add("Stop", Resources.no, (sender, e) => { RunBin_Click(sender, e, "worldserver", "stop"); });
                worldserverMenu.DropDownItems.Add("Restart", Resources.redo, (sender, e) => { RunBin_Click(sender, e, "worldserver", "restart"); });
            }
            else
            {
                worldserverMenu.Image = Resources.no;
                worldserverMenu.DropDownItems.Add("Start", Resources.ok, (sender, e) => { RunBin_Click(sender, e, "worldserver", "start"); });
            }

            contextMenuStrip1.Items.Add(new ToolStripSeparator());

            //Show Electron (GUI)
            contextMenuStrip1.Items.Add("Show GUI", Resources.cancel, ShowElectron);

            var electronMenu = new ToolStripMenuItem("Electron");
            contextMenuStrip1.Items.Add(electronMenu);
            electronMenu.DropDownItems.Add("Focus", Resources.cancel, (sender, e) => { RunElectron_Click(sender, e, "BrowserWindow", "focus",""); });
            electronMenu.DropDownItems.Add("Close", Resources.cancel, (sender, e) => { RunElectron_Click(sender, e, "BrowserWindow", "close",""); });
            electronMenu.DropDownItems.Add("LoadURL", Resources.cancel, (sender, e) => { RunElectron_Click(sender, e, "BrowserWindow", "loadUrl", "http://www.google.de"); });

            contextMenuStrip1.Items.Add(new ToolStripSeparator());
            contextMenuStrip1.Items.Add("Beenden", Resources.cancel, MenuExit);
        }

        private void ShowElectron(object sender, EventArgs e)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = Globals.BasePath + "\\App\\app\\node_modules\\electron-prebuilt\\dist\\electron.exe";
            cmd.StartInfo.WorkingDirectory = Globals.BasePath + "\\App\\app\\node_modules\\electron-prebuilt\\dist";
            cmd.StartInfo.Arguments = Globals.BasePath + "\\App\\app";
            cmd.Start();
        }

        private void MenuExit(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Application.Exit();
            Environment.Exit(-1);
        }

        [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);


        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{


            /*Dictionary<string, string> FileVersion = new Dictionary<string, string>();
            FileVersion["electron"] = "focus";
            //JsonConvert.SerializeObject(FileVersion);
            Globals.GlobalWebsocketServer.WebSocketServices.Broadcast(JsonConvert.SerializeObject(FileVersion));
            //Globals.GlobalWebsocketServer.WebSocketServices.Broadcast("{\"electron\":\"focus\"}");
            */
            ContextMenuStrip1Opening();
            SetForegroundWindow(new HandleRef(this, this.Handle));
            int x = Control.MousePosition.X;
            int y = Control.MousePosition.Y;
            x = x - 10;
            y = y - 40;
            this.contextMenuStrip1.Show(x, y);
            //}
        }
    }
}
