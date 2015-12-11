using System.Diagnostics;
using System.IO;

string mySqlExePath = @"bin\mariadb\bin\mysql.exe";
string mySqlHost = "localhost";
string mySqlPort = "3306";
string mySqlUsername = "trinity";
string mySqlPassword = "trinity";

string cmakeExe = @"D:\MMOwningLauncher\devel\cmake\bin\cmake.exe";
string cmakeGenerator = "\"Visual Studio 14 2015\"";
string cmakeBuildFolder = @"D:\MMOwningLauncher\mmowning\MMOCoreBuild";
string cmakeSourceFolder = @"D:\MMOwningLauncher\mmowning\MMOCore";

string msBuildSolutionFile = "TrinityCore.sln";
string msBuildOutDir = @"D:\MMOwningLauncher\bin\trinitycore";

string develFolder = @"D:\MMOwningLauncher\devel";
string binFolder = @"D:\MMOwningLauncher\bin";

//Check if Visual Studio is installed
if (VisualStudioInstalled() == false)
{
    WriteLineHeader(ConsoleColor.Red, "Error: Visual Studio 2015 is not installed");
    Environment.Exit(0);
}

//Delete old authserver.exe and worldserver.exe
try
{ 
    File.Delete(msBuildOutDir + "\\worldserver.exe");
    File.Delete(msBuildOutDir + "\\authserver.exe");
}
catch
{
    WriteLineHeader(ConsoleColor.Red, "Error: Could not delete authserver.exe or worldserver.exe - Make sure it's not in use");
    Environment.Exit(0);
}

//Build
//We use Batch file because we want colored outpur from MSBuild
var startInfo = new ProcessStartInfo();
startInfo.WorkingDirectory = @"D:\MMOwningLauncher\mmowning";
startInfo.FileName = @"D:\MMOwningLauncher\mmowning\start_cmake.bat";
Process.Start(startInfo);


/*if (runCmake() == 0)
{
    //runMSBuild();
    File.Copy(cmakeBuildFolder + "\\bin\\Release\\worldserver.exe", msBuildOutDir + "\\worldserver.exe", true);
    File.Copy(cmakeBuildFolder + "\\bin\\Release\\authserver.exe", msBuildOutDir + "\\authserver.exe", true);
} else {
    Environment.Exit(0);
}*/
//



//Cleanup
//Directory.Delete(topPath, true);

/****************************************************************************************/
/* Functions                                                                            */
/****************************************************************************************/
public void WriteLineHeader(ConsoleColor color, string text)
{
    Console.WriteLine("");
    Console.ForegroundColor = color;
    Console.WriteLine("------------------------------------------------------------------------");
    Console.WriteLine("-- " + text.ToString());
    Console.WriteLine("------------------------------------------------------------------------");
    Console.ResetColor();
}

public bool VisualStudioInstalled()
{
    bool visualStudioInstalled = false;
    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\MSBuild\14.0\bin"))
    {
        visualStudioInstalled = true;
    }
    if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\MSBuild\14.0\bin"))
    {
        visualStudioInstalled = true;
    }
    return visualStudioInstalled;
}

public int runCmake()
{
    Directory.SetCurrentDirectory(cmakeBuildFolder);
    string pathvar = Environment.GetEnvironmentVariable("PATH");
    var process = Process.Start(
        new ProcessStartInfo
        {
            FileName = cmakeExe,
            Arguments =
                String.Format(
                    "-G {0} {1}",
                    cmakeGenerator, cmakeSourceFolder),
            ErrorDialog = false,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = cmakeBuildFolder,
        }
    );


    process.StartInfo.EnvironmentVariables["PATH"] = pathvar + ";" + develFolder + "\\OpenSSL-Win32;" + develFolder + "\\cmake\\bin;" + binFolder + "\\mariadb\\lib;" + binFolder + "\\mariadb\\include\\mysql";
    process.StartInfo.EnvironmentVariables["BOOST_ROOT"] = develFolder + "\\boost_1_59_0";
    process.StartInfo.EnvironmentVariables["OPENSSL_ROOT_DIR"] = develFolder + "\\OpenSSL -Win32";

    process.OutputDataReceived += (o, e) => { if (e.Data == null) { } else Console.Out.WriteLine(e.Data); };
    process.ErrorDataReceived += (o, e) => { if (e.Data == null) { } else Console.Error.WriteLine(e.Data); };
    process.Start();
    process.BeginErrorReadLine();
    process.BeginOutputReadLine();
    process.StandardInput.Close();
    process.WaitForExit();
    Console.ResetColor();
    return process.ExitCode;
}


public int runMSBuild()
{
    Directory.SetCurrentDirectory(cmakeBuildFolder);
    string pathvar = Environment.GetEnvironmentVariable("PATH");
    var process = Process.Start(
        new ProcessStartInfo
        {
            FileName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\MSBuild\14.0\Bin\MSBuild.exe",
            Arguments =
                String.Format(
                    //"/p:Configuration=Release;OutDir={0}",
                    "/p:Configuration=Release {0}",
                    msBuildSolutionFile),
            ErrorDialog = false,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = cmakeBuildFolder,
        }
    );


    process.StartInfo.EnvironmentVariables["PATH"] = pathvar + ";" + develFolder + "\\OpenSSL-Win32;" + develFolder + "\\cmake\\bin;" + binFolder + "\\mariadb\\lib;" + binFolder + "\\mariadb\\include\\mysql" +  @";%ProgramFiles%\MSBuild\14.0\bin;%ProgramFiles(x86)%\MSBuild\14.0\bin";
    process.StartInfo.EnvironmentVariables["BOOST_ROOT"] = develFolder + "\\boost_1_59_0";
    process.StartInfo.EnvironmentVariables["OPENSSL_ROOT_DIR"] = develFolder + "\\OpenSSL -Win32";

    process.OutputDataReceived += (o, e) => { if (e.Data == null) { } else Console.Out.WriteLine(e.Data); };
    process.ErrorDataReceived += (o, e) => { if (e.Data == null) { } else Console.Error.WriteLine(e.Data); };
    process.Start();
    process.BeginErrorReadLine();
    process.BeginOutputReadLine();
    process.StandardInput.Close();
    process.WaitForExit();
    Console.ResetColor();
    return process.ExitCode;
}
