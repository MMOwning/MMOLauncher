﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MMOwningLauncher.Classes
{
    class Helpers
    {

        public static void ExtractFile(string source, string destination)
        {
            string zPath = Globals.AppPath + "\\Runtime\\7zip\\7zG.exe";
            try
            {
                ProcessStartInfo pro = new ProcessStartInfo();
                //pro.WindowStyle = ProcessWindowStyle.Hidden;
                pro.FileName = zPath;
                pro.Arguments = "x \"" + source + "\" -o" + destination;
                Process x = Process.Start(pro);
                x.WaitForExit();
            }
            catch (System.Exception Ex)
            {
                //DO logic here 
            }
        }

        public static void CreateZip(string source, string destination)
        {
            //string source = @"d:\a\example.txt";
            //string destination = @"d:\a\123.zip";
            ProcessStartInfo p = new ProcessStartInfo();
            p.FileName = Globals.AppPath + "\\Runtime\\7zip\\7zG.exe";
            p.Arguments = "a -tgzip \"" + source + "\" \"" + destination + "\" -mx=9";
            //p.WindowStyle = ProcessWindowStyle.Hidden;
            Process x = Process.Start(p);
            x.WaitForExit();
        }

        public static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }





        //http://stackoverflow.com/questions/13793560/find-closest-match-to-input-string-in-a-list-of-strings
        public static int FindBestMatch(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0)
            {
                return m;
            }
            if (m == 0)
            {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        /// <summary>
        /// DirectoryCopy
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="copySubDirs"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        public static dynamic CombineJson(object object1, object object2)
        {
            JObject sourceObject = JObject.FromObject(object1);
            JObject mergeIntoSourceObject = JObject.FromObject(object2);
            sourceObject.Merge(mergeIntoSourceObject, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });

            return sourceObject;
        }

        public static dynamic JObjectToExpandoObject(object object1)
        {
            IDictionary<string, object> dictionary1 = GetKeyValueMap(object1);

            var result = new ExpandoObject();

            var d = result as IDictionary<string, object>;
            foreach (var pair in dictionary1)
            {
                d[pair.Key] = pair.Value;
            }

            return result;
        }


        public static dynamic MergeCsDictionariesAndSave(dynamic csDictionary, dynamic json, string filePath)
        {
            //Merge csDictionary and filePath
            JObject combinedDictJson = Helpers.CombineJson(csDictionary, json);
            //Switch combinedDictJson to Expando Object -> Needed to write back settings to csDictionary
            dynamic combinedDict = new Dictionary<string, dynamic>(Helpers.JObjectToExpandoObject(combinedDictJson));

            //Save config file
            string jsonToFile = JsonConvert.SerializeObject(combinedDict, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, jsonToFile);
            return combinedDict;
        }

        public static dynamic MergeCsDictionaryAndSave(dynamic csDictionary, string filePath)
        {
            //Create Config File if not exists
            if (!File.Exists(filePath))
            {
                string json = JsonConvert.SerializeObject(csDictionary, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }

            //Read Real Values
            StreamReader jsonStreamReader = new StreamReader(filePath);
            JsonTextReader jsonReader = new JsonTextReader(jsonStreamReader);
            JsonSerializer jsonSerializer = new JsonSerializer();
            dynamic binConfigFile = jsonSerializer.Deserialize(jsonReader);
            jsonStreamReader.Close();

            //Merge csDictionary and filePath
            JObject combinedDictJson = Helpers.CombineJson(csDictionary, binConfigFile);
            //Switch combinedDictJson to Expando Object -> Needed to write back settings to csDictionary
            dynamic combinedDict = new Dictionary<string, dynamic>(Helpers.JObjectToExpandoObject(combinedDictJson));
            //csDictionary = combinedDict;

            //Save config file
            string jsonToFile = JsonConvert.SerializeObject(combinedDict, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, jsonToFile);
            return combinedDict;
        }

        /// <summary>
        /// CombineDynamics
        /// Merge 2 Dictionarys into one
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        public static dynamic CombineDynamics(object object1, object object2)
        {
            IDictionary<string, object> dictionary1 = GetKeyValueMap(object1);
            IDictionary<string, object> dictionary2 = GetKeyValueMap(object2);

            var result = new ExpandoObject();

            var d = result as IDictionary<string, object>;
            foreach (var pair in dictionary1.Concat(dictionary2))
            {
                d[pair.Key] = pair.Value;
            }

            return result;
        }

        private static IDictionary<string, object> GetKeyValueMap(object values)
        {
            if (values == null)
            {
                return new Dictionary<string, object>();
            }

            var map = values as IDictionary<string, object>;
            if (map == null)
            {
                map = new Dictionary<string, object>();
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(values))
                {
                    map.Add(descriptor.Name, descriptor.GetValue(values));
                }
            }
            return map;
        }
    }
}
