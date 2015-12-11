var exec = require('child_process').exec;
var spawn = require('child_process').spawn;
var edge = require('electron-edge');

//Setup Tail
var Tail = require('always-tail');
var fs = require('fs');

var worldserverLogFile = "bin/trinitycore/Server.log";
var worldserverTail = "";

var authserverLogFile = "bin/trinitycore/Auth.log";
var authserverTail = "";

function tailWorldserver() {
	if (!fs.existsSync(worldserverLogFile)) fs.writeFileSync(worldserverLogFile, "");
	worldserverTail = new Tail(worldserverLogFile, '\n', { start: 0, interval: 50 });
	worldserverTail.on('line', function(data) {
		//console.log("got line:", data);
		$("#worldserverCmdLineInner").append(data + "<br>");
		$('#worldserverCmdLineInner').scrollTop($('#worldserverCmdLineInner').prop("scrollHeight"));  
	});
	worldserverTail.on('error', function(data) {
		//console.log("error:", data);
		$("#worldserverCmdLineInner").append(data + "<br>");
		$('#worldserverCmdLineInner').scrollTop($('#worldserverCmdLineInner').prop("scrollHeight"));  

	});
	worldserverTail.watch();
	$('#worldserverCmdLineInner').scrollTop($('#worldserverCmdLineInner').prop("scrollHeight"));  
}

function tailAuthserver() {
	if (!fs.existsSync(authserverLogFile)) fs.writeFileSync(authserverLogFile, "");
	authserverTail = new Tail(authserverLogFile, '\n', { start: 0, interval: 50 });
	authserverTail.on('line', function(data) {
		//console.log("got line:", data);
		$("#authserverCmdLineInner").append(data + "<br>");
		$('#authserverCmdLineInner').scrollTop($('#authserverCmdLineInner').prop("scrollHeight"));  
	});
	authserverTail.on('error', function(data) {
		//console.log("error:", data);
		$("#authserverCmdLineInner").append(data + "<br>");
		$('#authserverCmdLineInner').scrollTop($('#authserverCmdLineInner').prop("scrollHeight"));  

	});
	authserverTail.watch();
	$('#authserverCmdLineInner').scrollTop($('#authserverCmdLineInner').prop("scrollHeight")); 
}

//worldserverTail.unwatch();
	
var checkProcess = edge.func(function () {/*
	async (input) => 
	{
		using System.Diagnostics;
		Process[] processlist = Process.GetProcesses();
		
		bool nginx_status = false;
		bool mysql_status = false;
		bool php_status = false;
		bool worldserver_status = false;
		bool authserver_status = false;
		
		foreach(Process theprocess in processlist){
			//Console.WriteLine("Process: {0} ID: {1}", theprocess.ProcessName, theprocess.Id);
			if(theprocess.ProcessName == "nginx")
			{
				nginx_status = true;
			}
			if(theprocess.ProcessName == "mysqld")
			{
				mysql_status = true;
			}		
			if(theprocess.ProcessName == "php-cgi")
			{
				php_status = true;
			}	
			if(theprocess.ProcessName == "worldserver")
			{
				worldserver_status = true;
			}
			if(theprocess.ProcessName == "authserver")
			{
				authserver_status = true;
			}	
		}	
		var result = new {
			nginx = nginx_status,
			mysql = mysql_status,
			php = php_status,
			worldserver = worldserver_status,
			authserver = authserver_status,
		};
		return result;
	}
*/});

/* --------------------------------------------------------------------------------------------------------	*/
/* Get Running processes																					*/
/* --------------------------------------------------------------------------------------------------------	*/
var nginx_status = false;
var mysql_status = false;
var php_status = false;
var worldserver_status = false;
var authserver_status = false;
		
$(window).load(function () {
	setInterval(function(){
		checkProcess(null, function (error, result) {
			if (error) throw error;
			
			//console.log(result);
			
			if (result.nginx == true)
			{
				//$("#topboxWebserverText").text("Running");
				$("#topboxWebserverBadge").removeClass("hidden");
				$("#topboxWebserverBtn").removeClass("btn-success");
				$("#topboxWebserverBtn").addClass("btn-danger");
				$("#topboxWebserverBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxWebserver").removeClass("bg-red");
				$("#topboxWebserver").addClass("bg-green");
				if (webserver_version == "0.0.0.0")
				{
					getNginxVersion();
				}
				if (nginx_status == false)
				{
					nginx_status = true;
				}
			} else {
				//$("#topboxWebserverText").text("Not running");
				$("#topboxWebserverBadge").addClass("hidden");
				$("#topboxWebserverBtn").removeClass("btn-danger");
				$("#topboxWebserverBtn").addClass("btn-success");
				$("#topboxWebserverBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxWebserver").removeClass("bg-green");
				$("#topboxWebserver").addClass("bg-red");
				if (nginx_status == true)
				{
					nginx_status = false;
				}				
			}
			
			if (result.php == true)
			{
				//$("#topboxPhpText").text("Running");
				$("#topboxPhpBadge").removeClass("hidden");
				$("#topboxPhpBtn").removeClass("btn-success");
				$("#topboxPhpBtn").addClass("btn-danger");
				$("#topboxPhpBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxPhp").removeClass("bg-red");
				$("#topboxPhp").addClass("bg-green");
				if (getPhpVersion == "0.0.0.0")
				{
					getPhpVersion();		
				}
				if (php_status == false)
				{
					php_status = true;
				}				
			} else {
				//$("#topboxPhpText").text("Not running");
				$("#topboxPhpBadge").addClass("hidden");
				$("#topboxPhpBtn").removeClass("btn-danger");
				$("#topboxPhpBtn").addClass("btn-success");
				$("#topboxPhpBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxPhp").removeClass("bg-green");
				$("#topboxPhp").addClass("bg-red");
				if (php_status == true)
				{
					php_status = false;
				}						
			}		
			
			if (result.mysql == true)
			{
				//$("#topboxMySQLText").text("Running");
				$("#topboxMySQLBadge").removeClass("hidden");
				$("#topboxMySQLBtn").removeClass("btn-success");
				$("#topboxMySQLBtn").addClass("btn-danger");
				$("#topboxMySQLBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxMySQL").removeClass("bg-red");
				$("#topboxMySQL").addClass("bg-green");
				if (mysql_status == false)
				{
					mysql_status = true;
				}					
			} else {
				//$("#topboxMySQLText").text("Not running");
				$("#topboxMySQLBadge").addClass("hidden");
				$("#topboxMySQLBtn").removeClass("btn-danger");
				$("#topboxMySQLBtn").addClass("btn-success");
				$("#topboxMySQLBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxMySQL").removeClass("bg-green");
				$("#topboxMySQL").addClass("bg-red");
				if (mysql_version == "0.0.0.0")
				{
					getMySQLVersion();
				}
				if (mysql_status == true)
				{
					mysql_status = false;
				}					
			}		

			if (result.authserver == true)
			{
				//$("#topboxAuthServerText").text("Running");
				$("#topboxAuthServerBadge").removeClass("hidden");
				$("#topboxAuthServerBtn").removeClass("btn-success");
				$("#topboxAuthServerBtn").addClass("btn-danger");
				$("#topboxAuthServerBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxAuthServer").removeClass("bg-red");
				$("#topboxAuthServer").addClass("bg-green");
				if (authserver_status == false)
				{
					authserver_status = true;
					tailAuthserver();
					$("#authserverCmdLineInner").text("");
				}					
			} else {
				//$("#topboxAuthServerText").text("Not running");
				$("#topboxAuthServerBadge").addClass("hidden");
				$("#topboxAuthServerBtn").removeClass("btn-danger");
				$("#topboxAuthServerBtn").addClass("btn-success");
				$("#topboxAuthServerBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxAuthServer").removeClass("bg-green");
				$("#topboxAuthServer").addClass("bg-red");
				if (authserver_status == true)
				{
					authserver_status = false;
					authserverTail.unwatch();
					$("#authserverCmdLineInner").append("Authserver closed" + "<br>");
				}						
			}	
			
			if (result.worldserver == true)
			{
				//$("#topboxWorldServerText").text("Running");
				$("#topboxWorldServerBadge").removeClass("hidden");
				$("#topboxWorldServerBtn").removeClass("btn-success");
				$("#topboxWorldServerBtn").addClass("btn-danger");
				$("#topboxWorldServerBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxWorldServer").removeClass("bg-red");
				$("#topboxWorldServer").addClass("bg-green");
				if (worldserver_status == false)
				{
					worldserver_status = true;
					tailWorldserver();
					$("#worldserverCmdLineInner").text("");
				}					
			} else {
				//$("#topboxWorldServerText").text("Not running");
				$("#topboxWorldServerBadge").addClass("hidden");
				$("#topboxWorldServerBtn").removeClass("btn-danger");
				$("#topboxWorldServerBtn").addClass("btn-success");
				$("#topboxWorldServerBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxWorldServer").removeClass("bg-green");
				$("#topboxWorldServer").addClass("bg-red");
				if (worldserver_status == true)
				{
					worldserver_status = false;
					worldserverTail.unwatch();
					$("#worldserverCmdLineInner").append("Worldserver closed" + "<br>");
				}						
			}	
		
		})
	}, 1000);
});
/* --------------------------------------------------------------------------------------------------------	*/
/* Get Server Version																						*/
/* --------------------------------------------------------------------------------------------------------	*/
var mysql_version = "0.0.0.0";
var webserver_version = "0.0.0.0";
var php_version = "0.0.0.0";

function getPhpVersion() {
	cmd = spawn("bin\\php\\php.exe", ["-v"]);
	cmd.stdout.on('data', function(data) {
		process.stdout.write(data);
		//console.log(data.toString());
		var stringArray = data.toString().split(" ");
		php_version =  stringArray[1].toString();
	});
	cmd.stderr.on('data', function(data) {
		process.stderr.write(data);	
		//console.log(data.toString());
	});		
	cmd.on('exit', function(code) {
		$("#topboxPhpVersion").html(" (Version: " + php_version + ")");
	});
}

function getMySQLVersion() {
	cmd = spawn("bin\\mariadb\\bin\\mysql.exe", ["-V"]);
	cmd.stdout.on('data', function(data) {
		process.stdout.write(data);
		//console.log(data);
		var stringArray = data.toString().split(" ");
		stringArray.forEach(function(entry) {
			if (entry.match("-MariaDB,$")) {
			   mysql_version = entry.replace("-MariaDB,", "")
			   
			}		
		});
	});
	cmd.stderr.on('data', function(data) {
		process.stderr.write(data);	
		//console.log(data.toString());
	});		
	cmd.on('exit', function(code) {
		$("#topboxMySQLVersion").html(" (Version: " + mysql_version + ")");
	});
}

function getNginxVersion() {
	cmd = spawn("bin\\nginx\\nginx.exe", ["-v"]);
	cmd.stdout.on('data', function(data) {
		process.stdout.write(data);
		//console.log(data.toString());
	});
	cmd.stderr.on('data', function(data) {
		process.stderr.write(data);
		//console.log(data.toString());
		var stringArray = data.toString().split(" ");
		stringArray.forEach(function(entry) {
			if (entry.match("^nginx/")) {
			   webserver_version = entry.replace("nginx/", "")
			   
			}		
		});
	});	
	cmd.on('exit', function(code) {
		$("#topboxWebserverVersion").html(" (Version: " + webserver_version + ")");
	});
}
	
	

	
/* --------------------------------------------------------------------------------------------------------	*/
/* Start Server																								*/
/* --------------------------------------------------------------------------------------------------------	*/

var edgeRunProcess = edge.func(function () {
	/*
	async (dynamic input) => 
	{
		using System.Diagnostics;
		
		Process cmd = new Process();
		cmd.StartInfo.FileName = (string)input.exeFile;
		cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
		if ((string)input.exeWorkingDir != "") 
		{
			cmd.StartInfo.WorkingDirectory = (string)input.exeWorkingDir;
		}
		if ((string)input.exeFileParameter != "") 
		{
			cmd.StartInfo.Arguments = (string)input.exeFileParameter;
		}		
		cmd.Start();		

		return (string)input.exeFile + " executed"; 
	}
	*/
});

var edgeOpenFile = edge.func(function () {
	/*
	async (dynamic input) => 
	{
		using System.Diagnostics;
		
		Process cmd = new Process();
		cmd.StartInfo.FileName = (string)input.exeFile;
		if ((string)input.exeWorkingDir != "") 
		{
			cmd.StartInfo.WorkingDirectory = (string)input.exeWorkingDir;
		}
		if ((string)input.exeFileParameter != "") 
		{
			cmd.StartInfo.Arguments = (string)input.exeFileParameter;
		}		
		cmd.Start();		

		return (string)input.exeFile + " executed"; 
	}
	*/
});

var edgeRunProcessWindow = edge.func(function () {
	/*
	async (dynamic input) => 
	{
		using System.Diagnostics;
		ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe");
		if ((string)input.exeWorkingDir != "") 
		{		
			processInfo.WorkingDirectory = (string)input.exeWorkingDir;
		}
		processInfo.Arguments = "/K " + (string)input.exeFile + " " + (string)input.exeFileParameter;
		Process.Start(processInfo);		
		return (string)input.exeFile + " " + (string)input.exeFileParameter + " executed"; 
	}
	*/
});

var killProcess = edge.func(function () {/*
	async (input) => 
	{
		using System.Diagnostics;
		foreach (var process in Process.GetProcessesByName(input.ToString()))
		{
			process.Kill();
		}
		return "nginx stopped"; 
	}
*/});

function startMySQL() {
	checkProcess(null, function (error, result) {
		if (error) throw error;
		if (result.mysql == true)
		{
			edgeRunProcess({"exeFile": "bin\\mariadb\\bin\\mysqladmin.exe", "exeFileParameter": "--defaults-file=bin\\mariadb\\my.ini -uroot --password=\"\" -h127.0.0.1 --protocol=tcp shutdown >> C:\\output.txt", "exeWorkingDir": ""}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});
		} else {
			edgeRunProcess({"exeFile": "bin\\mariadb\\bin\\mysqld.exe", "exeFileParameter": "", "exeWorkingDir": ""}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});
		}
	})
};		

function startWebserver() {
	checkProcess(null, function (error, result) {
		if (error) throw error;
		if (result.nginx == true)
		{
			edgeRunProcess({"exeFile": "bin\\nginx\\nginx.exe", "exeFileParameter": "-c bin/nginx/conf/nginx.conf -s stop", "exeWorkingDir": ""}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});					
		} else {
			edgeRunProcess({"exeFile": "bin\\nginx\\nginx.exe", "exeFileParameter": "-c bin/nginx/conf/nginx.conf", "exeWorkingDir": ""}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});			
		}
	})
};			

function startPhp() {
	checkProcess(null, function (error, result) {
		if (error) throw error;
		if (result.php == true)
		{
			killProcess("php-cgi");
		} else {
			edgeRunProcess({"exeFile": "bin\\php\\php-cgi.exe", "exeFileParameter": "-b localhost:9100", "exeWorkingDir": ""}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});			
		}
	})
};

function startAuthserver() {
	checkProcess(null, function (error, result) {
		if (error) throw error;
		if (result.authserver == true)
		{
			killProcess("authserver");
		} else {
			edgeRunProcess({"exeFile": "authserver.exe", "exeFileParameter": "-c authserver.conf", "exeWorkingDir": "bin\\trinitycore"}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});			
		}
	})
};

function startWorldserver() {
	checkProcess(null, function (error, result) {
		if (error) throw error;
		if (result.worldserver == true)
		{
			killProcess("worldserver");
		} else {
			edgeRunProcess({"exeFile": "worldserver.exe", "exeFileParameter": "-c worldserver.conf", "exeWorkingDir": "bin\\trinitycore"}, function (error, result) {
				if (error) throw error;
				console.log(result);
			});			
		}
	})
};


function openTextFile(file) {
	checkProcess(null, function (error, result) {
		if (error) throw error;
		edgeOpenFile({"exeFile": file, "exeFileParameter": "", "exeWorkingDir": ""}, function (error, result) {
			if (error) throw error;
			console.log(result);
		});			
	})
};