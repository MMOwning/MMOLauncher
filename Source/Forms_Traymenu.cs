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
using MMOwningLauncher.Properties;
using MMOwningLauncher.Web.WebSocket;
using MMOwningLauncher.Classes;
using Newtonsoft.Json;
using Nowin;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace MMOwningLauncher
{
    public partial class Form_TrayMenu : Form
    {

        //Define MainWindow for delegate in e.g JsonRPC
        internal static Form_TrayMenu mainWindowThread;

        public Form_TrayMenu()
        {
            InitializeComponent();
            mainWindowThread = this;
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


            //Set Menu Item
            var arrayOfAllKeys = Globals.BinConfig.Keys.ToArray();
            Array.Sort(arrayOfAllKeys);

            foreach (dynamic app in arrayOfAllKeys)
            {
                //if (Globals.BinConfig[app]["installed"]) {
                    var runtimeMenu = new ToolStripMenuItem(Globals.BinConfig[app]["name"].ToString());
                    
                    contextMenuStrip1.Items.Add(runtimeMenu);
                    if (Globals.RunningProcesses[app])
                    {
                        runtimeMenu.ForeColor = System.Drawing.Color.Green;
                        if (File.Exists(Globals.Folders["datapath"] + "\\Icons\\" + app + ".png"))
                        {
                            //Icon = new BitmapImage(new Uri(Globals.Folders["datapath"] + Globals.AppsDictionary[app]["Icon"], UriKind.Relative));
                            //runtimeMenu.Image = new Image { Width = 16, Height = 16, Source = Icon };
                        }
                        runtimeMenu.Image = Resources.ok;
                        runtimeMenu.DropDownItems.Add("Stop", Resources.no, (sender, e) => { RunBin_Click(sender, e, app, "stop"); });
                        runtimeMenu.DropDownItems.Add("Restart", Resources.redo, (sender, e) => { RunBin_Click(sender, e, app, "restart"); });
                    }
                    else
                    {
                        runtimeMenu.ForeColor = System.Drawing.Color.Red;
                        runtimeMenu.Image = Resources.no;
                        runtimeMenu.DropDownItems.Add("Start", Resources.ok, (sender, e) => { RunBin_Click(sender, e, app, "start"); });
                    }
                //}
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
            cmd.StartInfo.FileName = Globals.AppPath + "\\Runtime\\Electron\\electron.exe";
            cmd.StartInfo.WorkingDirectory = Globals.AppPath + "\\Runtime\\Electron";
            cmd.StartInfo.Arguments = Globals.AppPath + "\\app";
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
