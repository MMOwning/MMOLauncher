#r "MySql.Data.dll"

using System.Diagnostics;
using MySql.Data.MySqlClient;

string mySqlExePath = @"bin\mariadb\bin\mysql.exe";
string mySqlHost = "localhost";
string mySqlPort = "3306";
string mySqlUsername = "trinity";
string mySqlPassword = "trinity";

string trinitycoreAuthTable = "auth";
string trinitycoreCharactersTable = "characters";
string trinitycoreWorldTable = "world";

string parentFolder = Directory.GetCurrentDirectory().ToString();

if (Process.GetProcessesByName("mysqld").Length == 0)
{
    WriteLineHeader(ConsoleColor.Red, "Error: MySQL is not running - Exiting");
    
    Environment.Exit(0);
}

WriteLineHeader(ConsoleColor.Green, "Importing MMO Customs -> Characters");
foreach (var file in new DirectoryInfo(parentFolder + "\\mmowning\\MMOCore\\sql\\mmo_updates_characters").GetFiles("*.sql"))
{
    Console.WriteLine("Importing " + file.FullName.ToString());
    MySqlImport(trinitycoreCharactersTable, file.FullName.ToString());
}

WriteLineHeader(ConsoleColor.Green, "Importing TDB");
foreach (var file in new DirectoryInfo(parentFolder + "\\mmowning\\TDB").GetFiles("*.sql"))
{
    Console.WriteLine("Importing " + file.FullName.ToString());
    MySqlImport(trinitycoreWorldTable, file.FullName.ToString());
}

WriteLineHeader(ConsoleColor.Green, "Importing TrinityCore -> Updates World");
foreach (var file in new DirectoryInfo(parentFolder + "\\mmowning\\MMOCore\\sql\\updates\\world").GetFiles("*.sql"))
{
    Console.WriteLine("Importing " + file.FullName.ToString());
    MySqlImport(trinitycoreWorldTable, file.FullName.ToString());
}

WriteLineHeader(ConsoleColor.Green, "Importing MMO Customs -> World");
foreach (var file in new DirectoryInfo(parentFolder + "\\mmowning\\MMOCore\\sql\\mmo_updates_world").GetFiles("*.sql"))
{
    Console.WriteLine("Importing " + file.FullName.ToString());
    MySqlImport(trinitycoreWorldTable, file.FullName.ToString());
}

WriteLineHeader(ConsoleColor.Green, "Reset AUTO_INCREMENT on WorldDB");
string query =
@"ALTER TABLE " + trinitycoreWorldTable + ".creature AUTO_INCREMENT = 9900000;"
+"ALTER TABLE " + trinitycoreWorldTable + ".gameobject AUTO_INCREMENT = 9900000;"
+"ALTER TABLE " + trinitycoreWorldTable + ".game_tele AUTO_INCREMENT = 9900000;"
+"";

MySqlExecute(trinitycoreWorldTable, query);



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

public void MySqlExecute(string database, string filename)
{
    string myConnectionString = "SERVER=" + mySqlHost + ";" +
                                "PORT=" + mySqlPort + ";" +
                                "DATABASE=" + trinitycoreWorldTable + ";" +
                                "UID=" + mySqlUsername + ";" +
                                "PASSWORD=" + mySqlPassword + ";";

    MySqlConnection connection = new MySqlConnection(myConnectionString);
    MySqlCommand command = connection.CreateCommand();
    command.CommandText = query;
    MySqlDataReader Reader;
    connection.Open();
    Reader = command.ExecuteReader();
    while (Reader.Read())
    {
        string row = "";
        for (int i = 0; i < Reader.FieldCount; i++)
            row += Reader.GetValue(i).ToString() + ", ";
        Console.WriteLine(row);
    }
    connection.Close();
}

public int MySqlImport(string database, string filename)
{
    var process = Process.Start(
        new ProcessStartInfo
        {
            FileName = mySqlExePath,
            Arguments =
                String.Format(
                    "-C -B --host={0} -P {1} --user={2} --password={3} --default_character_set={4} --database={5} -e \"\\. {6}\"",
                    mySqlHost, mySqlPort, mySqlUsername, mySqlPassword, "utf8", database, filename),
            ErrorDialog = false,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = Environment.CurrentDirectory,
        }
    );
    process.OutputDataReceived += (o, e) => { if (e.Data == null) { } else Console.Out.WriteLine(e.Data); };
    process.ErrorDataReceived += (o, e) => { if (e.Data == null) { } else { Console.ForegroundColor = ConsoleColor.Red; Console.Error.WriteLine(e.Data); } };
    process.Start();
    process.BeginErrorReadLine();
    process.BeginOutputReadLine();
    process.StandardInput.Close();
    process.WaitForExit();
    Console.ResetColor();
    return process.ExitCode;
}
