using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MMOwningLauncher.Classes;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MMOwningLauncher.Web.Modules
{
    public class Config : NancyModule
    {



        public string[] WriteSafeReadAllLines(String path)
        {
            using (var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }

                return file.ToArray();
            }
        }


        /********************************************************************/
        /* WS Client Testing -> Start                                       */
        /********************************************************************/

        async Task SendText(ClientWebSocket socket, string data)
        {
            var t = Encoding.UTF8.GetBytes(data);
            await socket.SendAsync(
                new ArraySegment<byte>(t),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }


        ClientWebSocket StartStaticRouteClient(string route = "/ws")
        {
            var client = new ClientWebSocket();
            client.ConnectAsync(new Uri("ws://localhost:8080" + route), CancellationToken.None).Wait();
            return client;
        }

        /********************************************************************/
        /* WS Client Testing -> End                                       */
        /********************************************************************/


        public static Dictionary<string, bool> RunningProcesses = new Dictionary<string, bool>();
        public static Dictionary<string, string> FileVersion = new Dictionary<string, string>();
        public Config()
        {
            Get["/checkRunningPrograms"] = _ =>
            {
                /*RunningProcesses["nginx_status"] = false;
                RunningProcesses["php_status"] = false;
                RunningProcesses["mysql_status"] = false;
                RunningProcesses["authserver_status"] = false;
                RunningProcesses["worldserver_status"] = false;

                Process[] processlist = Process.GetProcesses();
                foreach (Process theprocess in processlist)
                {
                    if (theprocess.ProcessName == "nginx")
                    {
                        RunningProcesses["nginx_status"] = true;
                    }
                    if (theprocess.ProcessName == "php-cgi")
                    {
                        RunningProcesses["php_status"] = true;
                    }
                    if (theprocess.ProcessName == "mysqld")
                    {
                        RunningProcesses["mysql_status"] = true;
                    }

                    if (theprocess.ProcessName == "authserver")
                    {
                        RunningProcesses["authserver_status"] = true;
                    }
                    if (theprocess.ProcessName == "worldserver")
                    {
                        RunningProcesses["worldserver_status"] = true;
                    }
                }*/
                var output = JsonConvert.SerializeObject(Globals.RunningProcesses);
                return output;
            };


            Get["/checkServerVersion"] = _ =>
            {

                FileVersion["nginx"] = "unknown";
                FileVersion["mariadb"] = "unknown";
                FileVersion["php"] = "unknown";
                FileVersion["PostgreSQL"] = "unknown";
                FileVersion["Memcached"] = "unknown";
                FileVersion["Mongodb"] = "unknown";
                FileVersion["worldserver"] = "unknown";
                FileVersion["authserver"] = "unknown";

                //FileVersionInfo.GetVersionInfo(@"D:\MMOLauncher\bin\mariadb\bin\mysqld.exe");
                if (File.Exists(Globals.RuntimePath + "\\pgsql\\bin\\pg_ctl.exe"))
                {
                    FileVersionInfo postgreSqlVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\pgsql\\bin\\pg_ctl.exe");
                    if (postgreSqlVersion.FileVersion != null)
                        FileVersion["PostgreSQL"] = postgreSqlVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\memcached\\memcached.exe"))
                {
                    FileVersionInfo memcachedVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\memcached\\memcached.exe");
                    if (memcachedVersion.FileVersion != null)
                        FileVersion["Memcached"] = memcachedVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\mongodb\\bin\\mongod.exe"))
                {
                    FileVersionInfo mongodbVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\mongodb\\bin\\mongod.exe");
                    if (mongodbVersion.FileVersion != null)
                        FileVersion["Mongodb"] = mongodbVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\nginx\\nginx.exe"))
                {
                    FileVersionInfo nginxVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\nginx\\nginx.exe");
                    if (nginxVersion.FileVersion != null)
                        FileVersion["nginx"] = nginxVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\mariadb\\bin\\mysqld.exe"))
                {
                    FileVersionInfo mariadbVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\mariadb\\bin\\mysqld.exe");
                    if (mariadbVersion.FileVersion != null)
                        FileVersion["mariadb"] = mariadbVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\php\\php-cgi.exe"))
                {
                    FileVersionInfo phpVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\php\\php-cgi.exe");
                    if (phpVersion.FileVersion != null)
                        FileVersion["php"] = phpVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\trinitycore\\worldserver.exe"))
                {
                    FileVersionInfo worldserverVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\trinitycore\\worldserver.exe");
                    if (worldserverVersion.FileVersion != null)
                        FileVersion["worldserver"] = worldserverVersion.FileVersion;
                }
                if (File.Exists(Globals.RuntimePath + "\\trinitycore\\authserver.exe"))
                {
                    FileVersionInfo authserverVersion = FileVersionInfo.GetVersionInfo(Globals.RuntimePath + "\\trinitycore\\authserver.exe");
                    if (authserverVersion.FileVersion != null)
                        FileVersion["authserver"] = authserverVersion.FileVersion;
                }

                var output = JsonConvert.SerializeObject(FileVersion);
                return output;
            };


            Post["/postBinCommand"] = _ =>
            {
                var bodyAsString = "";
                using (var rdr = new StreamReader(this.Request.Body))
                {
                    bodyAsString = rdr.ReadToEnd();
                }

                var results = JsonConvert.DeserializeObject<dynamic>(bodyAsString);
                string command = null;
                string bin = null;
                string fileName = "";
                string arguments = "";
                string workingDirectory = "";
                //bool showCmd = false;
                

                if (results.Property("command") != null) command = results.command;
                if (results.Property("bin") != null) bin = results.bin;
                if (results.Property("fileName") != null) fileName = results.fileName;
                if (results.Property("arguments") != null) arguments = results.arguments;
                if (results.Property("workingDirectory") != null) workingDirectory = results.workingDirectory;
                //if (results.Property("showCmd") != "") showCmd = results.showCmd;

                Binaries.RunProgram(command, bin, fileName, arguments, workingDirectory);

                return command + " for " + bin + " executed";
            };


            Post["/edgeOpenFile"] = _ =>
            {

                //Change later
                string LauncherPath = Globals.BasePath + "\\";
                var bodyAsString = "";
                using (var rdr = new StreamReader(this.Request.Body))
                {
                    bodyAsString = rdr.ReadToEnd();
                }

                var results = JsonConvert.DeserializeObject<dynamic>(bodyAsString);

                string command = null;
                string data = null;
                string parameter = null;
                string path = null;
                bool showcmd = false;

                if (results.Property("command") != null) command = results.command;
                if (results.Property("data") != null) data = results.data;
                if (results.Property("parameter") != null) parameter = results.parameter;
                if (results.Property("path") != null) path = results.path;
                if (results.Property("showcmd") != null) showcmd = results.showcmd;
 

                Console.WriteLine(LauncherPath + data);
                if (command == "openTextFile") {
                    Process cmd = new Process();
                    cmd.StartInfo.FileName = LauncherPath + data;
                    if (path != null) cmd.StartInfo.WorkingDirectory = path;
                    if (parameter != null) cmd.StartInfo.Arguments = parameter;
                    cmd.Start();
                }

                if (command == "start")
                {
                    Process cmd = new Process();
                    if (showcmd)
                    {
                        cmd.StartInfo.FileName = "cmd.exe";
                        if (parameter != null) cmd.StartInfo.Arguments = "/K " + data + " " + parameter;
                        else cmd.StartInfo.Arguments = "/K " + data;
                    }
                    else
                    {
                        if (data != null) cmd.StartInfo.FileName = data;
                        if (parameter != null) cmd.StartInfo.Arguments = parameter;
                    }

                    if (path != null) cmd.StartInfo.WorkingDirectory = LauncherPath + path;

                    cmd.Start();
                }

                if (command == "kill")
                {
                    if (data != null) { 
                        foreach (var process in Process.GetProcessesByName(data))
                        {
                            process.Kill();
                        }
                    }
                }

                //Console.WriteLine(results["command"]);
                //Console.WriteLine(results["data"]);

                return command + " for " + data + " executed";
            };





            Get["/getWebserverAccessLog/{maxResults}/{logLevel}"] = _ =>
            {


                JArray jsonArray = new JArray();
                dynamic aaData = new JObject();

                int maxResults = Convert.ToInt32((string)_.maxResults);
                int logLevel = (int)_.logLevel;

                string filename = Globals.BasePath + "\\Data\\" + "logs\\access.log";

                Console.WriteLine(Globals.BasePath + filename);
                if (!File.Exists(filename))
                {

                    dynamic logLine = new JObject();
                    logLine["line"] = filename + " not found";
                    jsonArray.Add(logLine);
                    aaData["aaData"] = jsonArray;
                    return JsonConvert.SerializeObject(aaData);
                }

                int countLines = 0;
                //We need other method because ReadAllLines dont work if afile is open in writable mode
                var lines = WriteSafeReadAllLines(filename);
                //var lines = File.ReadAllLines("D:\\MMOLauncher\\logs\\access.log").Reverse();

                foreach (string line in lines.Reverse())
                {
                    if (countLines == maxResults) break;

                    //Apache/Nginx log Parser
                    string logEntryPattern = "^([\\d.]+) (\\S+) (\\S+) \\[([\\w:/]+\\s[+\\-]\\d{4})\\] \"(.+?)\" (\\d{3}) (\\d+) \"([^\"]+)\" \"([^\"]+)\"";

                    Match regexMatch = Regex.Match(line, logEntryPattern);

                    /*Console.WriteLine("IP Address: " + regexMatch.Groups[1].Value);
                    Console.WriteLine("Date&Time: " + regexMatch.Groups[4].Value);
                    Console.WriteLine("Request: " + regexMatch.Groups[5].Value);
                    Console.WriteLine("Response: " + regexMatch.Groups[6].Value);
                    Console.WriteLine("Bytes Sent: " + regexMatch.Groups[7].Value);
                    if (!regexMatch.Groups[8].Value.Equals("-"))
                        Console.WriteLine("Referer: " + regexMatch.Groups[8].Value);
                    Console.WriteLine("Browser: " + regexMatch.Groups[9].Value);*/


                    //jsonArray.Add(line);
                    dynamic webServer = new JObject();
                    webServer["ip"] = regexMatch.Groups[1].Value;
                    webServer["time"] = regexMatch.Groups[4].Value;
                    webServer["request"] = regexMatch.Groups[5].Value;
                    webServer["response"] = regexMatch.Groups[6].Value;
                    webServer["bytes_sent"] = regexMatch.Groups[7].Value;
                    webServer["referer"] = regexMatch.Groups[8].Value;
                    webServer["browser"] = regexMatch.Groups[9].Value;

                    //jsonArray.Add(webServer);

                    countLines = countLines + 1;


                    
                    int responseCode = Int32.Parse(webServer["response"].ToString());
                    switch (logLevel)
                    {
                        case 0:
                            jsonArray.Add(webServer);
                            countLines = countLines + 1;
                            break;
                        case 100:
                            if (responseCode >= 100 && responseCode < 200)
                            {
                                jsonArray.Add(webServer);
                                countLines = countLines + 1;
                            }
                            break;
                        case 200:
                            if (responseCode >= 200 && responseCode < 300)
                            {
                                jsonArray.Add(webServer);
                                countLines = countLines + 1;
                            }
                            break;
                        case 300:
                            if (responseCode >= 300 && responseCode < 400)
                            {
                                jsonArray.Add(webServer);
                                countLines = countLines + 1;
                            }
                            break;
                        case 400:
                            if (responseCode >= 400 && responseCode < 500)
                            {
                                jsonArray.Add(webServer);
                                countLines = countLines + 1;
                            }
                            break;
                        case 500:
                            if (responseCode >= 500 && responseCode < 600)
                            {
                                jsonArray.Add(webServer);
                                countLines = countLines + 1;
                            }
                            break;
                    }


                }

                aaData["aaData"] = jsonArray;

                var output = JsonConvert.SerializeObject(aaData);
                return output;
            };


            Get["/getLog/{maxResults}/{filename*}"] = _ =>
            {
                JArray jsonArray = new JArray();
                dynamic aaData = new JObject();

                int maxResults = Convert.ToInt32((string)_.maxResults);
                string getFilename = (string)_.filename;
                string filename = Globals.BasePath + "\\" + getFilename.Replace("/", "\\");

                Console.WriteLine(Globals.BasePath + filename);
                if (!File.Exists(filename))
                {
                    
                    dynamic logLine = new JObject();
                    logLine["line"] = filename + " not found";
                    jsonArray.Add(logLine);
                    aaData["aaData"] = jsonArray;
                    return JsonConvert.SerializeObject(aaData);
                }

                int countLines = 0;
                var lines = WriteSafeReadAllLines(filename);

                



                foreach (string line in lines.Reverse())
                {
                    if (countLines == maxResults) break;

                    dynamic logLine = new JObject();
                    logLine["line"] = line;
                    ;

                    jsonArray.Add(logLine);
                    countLines = countLines + 1;
                }

                
                aaData["aaData"] = jsonArray;

                var output = JsonConvert.SerializeObject(aaData);
                return output;
            };


            Get["/getCurrentUserAvatar"] = _ =>
            {
                MemoryStream stream = new MemoryStream();
                Image userAvatarImage = GetUserAvatar.GetUserTile(Globals.RuntimeSettings["windows"]["currentUser"]);
                userAvatarImage.Save(stream, ImageFormat.Png);
                byte[] byteArray = stream.GetBuffer();
                return Response.FromByteArray(byteArray, "image/png");
            };

            Get["/getCurrentUserName"] = _ =>
            {
                return Globals.RuntimeSettings["windows"]["currentUser"];
            };

            Get["/getPlatform"] = _ =>
            {
                var output = JsonConvert.SerializeObject(Globals.RuntimeSettings["platform"]);
                return output;
            };

            Get["/openFile"] = _ =>
            {
                var dialog = new OpenFileDialog();
                var result = Form_TrayMenu.mainWindowThread.Invoke(new Func<OpenFileDialog, DialogResult>((d) => d.ShowDialog()), dialog);
                if ((DialogResult)result == DialogResult.OK)
                {
                    var response = (
                        File.Exists(dialog.FileName)
                        ? new StreamResponse(null, File.OpenRead(dialog.FileName).ToString())
                        : Response.AsText(string.Empty)
                        )
                        .WithHeader("X-File-Path", dialog.FileName);
                    //Console.WriteLine(dialog.FileName);
                    return dialog.FileName;
                    //return response;
                }
                return Negotiate.WithStatusCode(HttpStatusCode.ClientClosedRequest).WithReasonPhrase("");
            };


            Get["/websocketClient"] = x =>
            {

                //Testing WS Client  -> See commit above 
                var client = StartStaticRouteClient();
                var toSend = "Test Data String";
                SendText(client, toSend).Wait();
                Thread.Sleep(100);

                /*Console.WriteLine("wsroute");
                var test = new MyWebSocket();
                test.SendText(Encoding.UTF8.GetBytes("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"), true);
                var context = GlobalHost.ConnectionManager.GetConnectionContext<MyEndPoint>();
                context.Connection.Broadcast(message);*/
                return HttpStatusCode.OK;
            };

            Get["/config"] = _ =>
            {
                var output = JsonConvert.SerializeObject(Globals.MainConfig);
                return output;
            };

            Post["/config/config_save"] = _ =>
            {
                var bodyAsString = "";
                using (var rdr = new StreamReader(this.Request.Body))
                {
                    bodyAsString = rdr.ReadToEnd();
                }
                dynamic mainSettingsFile = JsonConvert.DeserializeObject(bodyAsString);

                Globals.MainConfig = Helpers.MergeCsDictionariesAndSave(Globals.MainConfig, mainSettingsFile, Globals.DataPath + "\\MainConfig.json");

                //Console.WriteLine(jsonToFile);
                return HttpStatusCode.OK;
            };

        }
    }
}
