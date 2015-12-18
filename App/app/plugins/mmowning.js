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
	
var process_status = {};
process_status['nginx_status'] = false;
process_status['mysql_status'] = false;
process_status['php_status'] = false;
process_status['worldserver_status'] = false;
process_status['authserver_status'] = false;

function checkRunningApps() {
	$.ajax({
		url : getBackEndPath + '/checkRunningPrograms',
		type : 'get',
		dataType : 'json',
		success : function(data) {
			//console.log(process_status);
			process_status = data;
			//console.log(process_status);
			//console.log(data.mysql_status)

			if (process_status['nginx_status'] == true)
			{
				//$("#topboxWebserverText").text("Running");
				$("#topboxWebserverBadge").removeClass("hidden");
				$("#topboxWebserverBtn").removeClass("btn-success");
				$("#topboxWebserverBtn").addClass("btn-danger");
				$("#topboxWebserverBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxWebserver").removeClass("bg-red");
				$("#topboxWebserver").addClass("bg-green");
			} else {
				//$("#topboxWebserverText").text("Not running");
				$("#topboxWebserverBadge").addClass("hidden");
				$("#topboxWebserverBtn").removeClass("btn-danger");
				$("#topboxWebserverBtn").addClass("btn-success");
				$("#topboxWebserverBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxWebserver").removeClass("bg-green");
				$("#topboxWebserver").addClass("bg-red");			
			}
			
			if (process_status['php_status'] == true)
			{
				//$("#topboxPhpText").text("Running");
				$("#topboxPhpBadge").removeClass("hidden");
				$("#topboxPhpBtn").removeClass("btn-success");
				$("#topboxPhpBtn").addClass("btn-danger");
				$("#topboxPhpBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxPhp").removeClass("bg-red");
				$("#topboxPhp").addClass("bg-green");			
			} else {
				//$("#topboxPhpText").text("Not running");
				$("#topboxPhpBadge").addClass("hidden");
				$("#topboxPhpBtn").removeClass("btn-danger");
				$("#topboxPhpBtn").addClass("btn-success");
				$("#topboxPhpBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxPhp").removeClass("bg-green");
				$("#topboxPhp").addClass("bg-red");						
			}		
			
			if (process_status['mysql_status'] == true)
			{
				//$("#topboxMySQLText").text("Running");
				$("#topboxMySQLBadge").removeClass("hidden");
				$("#topboxMySQLBtn").removeClass("btn-success");
				$("#topboxMySQLBtn").addClass("btn-danger");
				$("#topboxMySQLBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxMySQL").removeClass("bg-red");
				$("#topboxMySQL").addClass("bg-green");				
			} else {
				//$("#topboxMySQLText").text("Not running");
				$("#topboxMySQLBadge").addClass("hidden");
				$("#topboxMySQLBtn").removeClass("btn-danger");
				$("#topboxMySQLBtn").addClass("btn-success");
				$("#topboxMySQLBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxMySQL").removeClass("bg-green");
				$("#topboxMySQL").addClass("bg-red");				
			}		

			if (process_status['authserver_status'] == true)
			{
				//$("#topboxAuthServerText").text("Running");
				$("#topboxAuthServerBadge").removeClass("hidden");
				$("#topboxAuthServerBtn").removeClass("btn-success");
				$("#topboxAuthServerBtn").addClass("btn-danger");
				$("#topboxAuthServerBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxAuthServer").removeClass("bg-red");
				$("#topboxAuthServer").addClass("bg-green");			
			} else {
				//$("#topboxAuthServerText").text("Not running");
				$("#topboxAuthServerBadge").addClass("hidden");
				$("#topboxAuthServerBtn").removeClass("btn-danger");
				$("#topboxAuthServerBtn").addClass("btn-success");
				$("#topboxAuthServerBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxAuthServer").removeClass("bg-green");
				$("#topboxAuthServer").addClass("bg-red");				
			}	
			
			if (process_status['worldserver_status'] == true)
			{
				//$("#topboxWorldServerText").text("Running");
				$("#topboxWorldServerBadge").removeClass("hidden");
				$("#topboxWorldServerBtn").removeClass("btn-success");
				$("#topboxWorldServerBtn").addClass("btn-danger");
				$("#topboxWorldServerBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxWorldServer").removeClass("bg-red");
				$("#topboxWorldServer").addClass("bg-green");				
			} else {
				//$("#topboxWorldServerText").text("Not running");
				$("#topboxWorldServerBadge").addClass("hidden");
				$("#topboxWorldServerBtn").removeClass("btn-danger");
				$("#topboxWorldServerBtn").addClass("btn-success");
				$("#topboxWorldServerBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxWorldServer").removeClass("bg-green");
				$("#topboxWorldServer").addClass("bg-red");					
			}				
		}
	});
}

/* --------------------------------------------------------------------------------------------------------	*/
/* Get Server Version																						*/
/* --------------------------------------------------------------------------------------------------------	*/
var mysql_version = "0.0.0.0";
var webserver_version = "0.0.0.0";
var php_version = "0.0.0.0";
var worldserver_version = "0.0.0.0";
var authserver_version = "0.0.0.0";

function checkServerVersion() {
	$.ajax({
		url : getBackEndPath + '/checkServerVersion',
		type : 'get',
		dataType : 'json',
		success : function(data) {
			mysql_version = data.mariadb;
			webserver_version = data.nginx;
			php_version = data.php;
			worldserver_version = data.worldserver;
			authserver_version = data.authserver;
			$("#topboxPhpVersion").html(" (Version: " + php_version + ")");
			$("#topboxMySQLVersion").html(" (Version: " + mysql_version + ")");
			//$("#topboxWorldserverVersion").html(" (Version: " + worldserver_version + ")");
			//$("#topboxAuthserverVersion").html(" (Version: " + authserver_version + ")");
			
			$("#topboxWebserverVersion").html(" (Version: " + webserver_version + ")");
		}
	});
}

//Post json data
function postJsonData(jsonData)
{
	$.ajax({
	  type: "POST",
		beforeSend: function (request)
		{
			request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');		
			//request.setRequestHeader("X-XSRFToken", getCookie("_xsrf"));
		},			  
	  url: getBackEndPath + '/edgeOpenFile',
	  cache: false,
	  data:JSON.stringify(jsonData),
	});	
}
	
function startAuthserver() {
		if (process_status['authserver_status'] == true)
		{
			postJsonData({ command: 'kill', data:'authserver'});		
		} else {
			postJsonData({ command: 'start', data:'authserver.exe', parameter:'-c authserver.conf', path:'Runtime\\trinitycore\\', showcmd: false});		
		}
};

function startWorldserver() {
		if (process_status['worldserver_status'] == true)
		{
			postJsonData({ command: 'kill', data:'worldserver'});		
		} else {
			postJsonData({ command: 'start', data:'worldserver.exe', parameter:'-c worldserver.conf', path:'Runtime\\trinitycore\\', showcmd: false});		
		}
};

function startMySQL() {
		if (process_status['mysql_status'] == true)
		{
			postJsonData({ command: 'start', data:'Runtime\\mariadb\\bin\\mysqladmin.exe', parameter:'--defaults-file=Runtime\\mariadb\\my.ini -uroot --password=\"\" -h127.0.0.1 --protocol=tcp shutdown >> C:\\output.txt', path:'', showcmd: false});		
		} else {
			postJsonData({ command: 'start', data:'Runtime\\mariadb\\bin\\mysqld.exe', parameter:'', path:'', showcmd: false});		
		}
};

function startWebserver() {
		if (process_status['nginx_status'] == true)
		{
			postJsonData({ command: 'start', data:'Runtime\\nginx\\nginx.exe', parameter:'-c bin/nginx/conf/nginx.conf -s stop', path:'', showcmd: false});		
		} else {
			postJsonData({ command: 'start', data:'Runtime\\nginx\\nginx.exe', parameter:'-c bin/nginx/conf/nginx.conf', path:'', showcmd: false});		
		}
};

function startPhp() {
		if (process_status['php_status'] == true)
		{
			postJsonData({ command: 'kill', data:'php-cgi'});		
		} else {
			postJsonData({ command: 'start', data:'Runtime\\php\\php-cgi.exe', parameter:'-b localhost:9100', path:'', showcmd: false});		
		}
};


var jsArray = {};


/*
 * LOAD SCRIPTS
 * Usage:
 * Define function = myPrettyCode ()...
 * loadScript("js/my_lovely_script.js", myPrettyCode);
 */
	function loadScript(scriptName, callback) {
	
		if (!jsArray[scriptName]) {
			jsArray[scriptName] = true;
	
			// adding the script tag to the head as suggested before
			var body = document.getElementsByTagName('body')[0],
				script = document.createElement('script');
			script.type = 'text/javascript';
			script.src = scriptName;
	
			// then bind the event to the callback function
			// there are several events for cross browser compatibility
			script.onload = callback;
	
			// fire the loading
			body.appendChild(script);
			
			// clear DOM reference
			//body = null;
			//script = null;
	
		} else if (callback) {
			// changed else to else if(callback)
			console.log("JS file already added!");
			//execute function
			callback();
		}
	
	}
/* ~ END: LOAD SCRIPTS */
/*************************************************************************************************
	Load Interval Javascripts (e.g. checkRunningApps  etc)
*************************************************************************************************/
var jsInterval = {};

function runJsInterval(script, str, delay){
	if (!jsInterval[str]) {
		//console.log("Not in array");
		jsInterval[str] = setInterval(function(){script()},delay);
	}
}

function clearJsInterval(){
    for (var key in jsInterval) {
        clearInterval(jsInterval[key]);
		jsInterval[key] = null;
		delete jsInterval[key];		
    }
}
/*************************************************************************************************
	Setup Page -> Should be called on each site
*************************************************************************************************/
function setupPage() {
	clearJsInterval();
}