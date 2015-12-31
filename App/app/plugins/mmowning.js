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
process_status['nginx'] = false;
process_status['mysql'] = false;
process_status['php'] = false;
process_status['worldserver'] = false;
process_status['authserver'] = false;

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

			if (process_status['nginx'] == true)
			{
				$("#topboxWebserverBtnStart").addClass("hidden");
				$("#topboxWebserverBtnReStart").removeClass("hidden");				
				//$("#topboxWebserverText").text("Running");
				$("#topboxWebserverBadge").removeClass("hidden");
				$("#topboxWebserverBtn").removeClass("btn-success");
				$("#topboxWebserverBtn").addClass("btn-danger");
				$("#topboxWebserverBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxWebserver").removeClass("bg-red");
				$("#topboxWebserver").addClass("bg-green");
			} else {
				$("#topboxWebserverBtnStart").removeClass("hidden");
				$("#topboxWebserverBtnReStart").addClass("hidden");				
				//$("#topboxWebserverText").text("Not running");
				$("#topboxWebserverBadge").addClass("hidden");
				$("#topboxWebserverBtn").removeClass("btn-danger");
				$("#topboxWebserverBtn").addClass("btn-success");
				$("#topboxWebserverBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxWebserver").removeClass("bg-green");
				$("#topboxWebserver").addClass("bg-red");			
			}
			
			if (process_status['php'] == true)
			{
				$("#topboxPhpBtnStart").addClass("hidden");
				$("#topboxPhpBtnReStart").removeClass("hidden");				
				//$("#topboxPhpText").text("Running");
				$("#topboxPhpBadge").removeClass("hidden");
				$("#topboxPhpBtn").removeClass("btn-success");
				$("#topboxPhpBtn").addClass("btn-danger");
				$("#topboxPhpBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxPhp").removeClass("bg-red");
				$("#topboxPhp").addClass("bg-green");			
			} else {
				$("#topboxPhpBtnStart").removeClass("hidden");
				$("#topboxPhpBtnReStart").addClass("hidden");				
				//$("#topboxPhpText").text("Not running");
				$("#topboxPhpBadge").addClass("hidden");
				$("#topboxPhpBtn").removeClass("btn-danger");
				$("#topboxPhpBtn").addClass("btn-success");
				$("#topboxPhpBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxPhp").removeClass("bg-green");
				$("#topboxPhp").addClass("bg-red");						
			}		
			
			if (process_status['mysql'] == true)
			{
				//$("#topboxMySQLText").text("Running");
				$("#topboxMySQLBtnStart").addClass("hidden");
				$("#topboxMySQLBtnReStart").removeClass("hidden");
				$("#topboxMySQLBadge").removeClass("hidden");
				$("#topboxMySQLBtn").removeClass("btn-success");
				$("#topboxMySQLBtn").addClass("btn-danger");
				$("#topboxMySQLBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxMySQL").removeClass("bg-red");
				$("#topboxMySQL").addClass("bg-green");				
			} else {
				$("#topboxMySQLBtnStart").removeClass("hidden");
				$("#topboxMySQLBtnReStart").addClass("hidden");
				$("#topboxMySQLBadge").addClass("hidden");
				$("#topboxMySQLBtn").removeClass("btn-danger");
				$("#topboxMySQLBtn").addClass("btn-success");
				$("#topboxMySQLBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxMySQL").removeClass("bg-green");
				$("#topboxMySQL").addClass("bg-red");				
			}		

			if (process_status['authserver'] == true)
			{
				$("#topboxAuthServerBtnStart").addClass("hidden");
				$("#topboxAuthServerBtnReStart").removeClass("hidden");				
				//$("#topboxAuthServerText").text("Running");
				$("#topboxAuthServerBadge").removeClass("hidden");
				$("#topboxAuthServerBtn").removeClass("btn-success");
				$("#topboxAuthServerBtn").addClass("btn-danger");
				$("#topboxAuthServerBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxAuthServer").removeClass("bg-red");
				$("#topboxAuthServer").addClass("bg-green");			
			} else {
				$("#topboxAuthServerBtnStart").removeClass("hidden");
				$("#topboxAuthServerBtnReStart").addClass("hidden");				
				//$("#topboxAuthServerText").text("Not running");
				$("#topboxAuthServerBadge").addClass("hidden");
				$("#topboxAuthServerBtn").removeClass("btn-danger");
				$("#topboxAuthServerBtn").addClass("btn-success");
				$("#topboxAuthServerBtn").html('<i class="fa fa-play"></i> Start');
				$("#topboxAuthServer").removeClass("bg-green");
				$("#topboxAuthServer").addClass("bg-red");				
			}	
			
			if (process_status['worldserver'] == true)
			{
				$("#topboxWorldServerBtnStart").addClass("hidden");
				$("#topboxWorldServerBtnReStart").removeClass("hidden");				
				//$("#topboxWorldServerText").text("Running");
				$("#topboxWorldServerBadge").removeClass("hidden");
				$("#topboxWorldServerBtn").removeClass("btn-success");
				$("#topboxWorldServerBtn").addClass("btn-danger");
				$("#topboxWorldServerBtn").html('<i class="fa fa-stop"></i> Stop');
				$("#topboxWorldServer").removeClass("bg-red");
				$("#topboxWorldServer").addClass("bg-green");				
			} else {
				$("#topboxWorldServerBtnStart").removeClass("hidden");
				$("#topboxWorldServerBtnReStart").addClass("hidden");				
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

function postBinCommand(jsonData)
{
	$.ajax({
	  type: "POST",
		beforeSend: function (request)
		{
			request.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');		
			//request.setRequestHeader("X-XSRFToken", getCookie("_xsrf"));
		},			  
	  url: getBackEndPath + '/postBinCommand',
	  cache: false,
	  data:JSON.stringify(jsonData),
	});	
}
	
function startAuthserver() {
		if (process_status['authserver'] == true)
		{
			postJsonData({ command: 'kill', data:'authserver'});		
		} else {
			postJsonData({ command: 'start', data:'authserver.exe', parameter:'-c authserver.conf', path:'Bin\\trinitycore\\', showcmd: false});		
		}
};

function startWorldserver() {
		if (process_status['worldserver'] == true)
		{
			postJsonData({ command: 'kill', data:'worldserver'});		
		} else {
			postJsonData({ command: 'start', data:'worldserver.exe', parameter:'-c worldserver.conf', path:'Bin\\trinitycore\\', showcmd: false});		
		}
};

function startMySQL() {
		if (process_status['mysql'] == true)
		{
			postJsonData({ command: 'start', data:'Bin\\mariadb\\bin\\mysqladmin.exe', parameter:'--defaults-file=Bin\\mariadb\\my.ini -uroot --password=\"\" -h127.0.0.1 --protocol=tcp shutdown >> C:\\output.txt', path:'', showcmd: true});		
		} else {
			postJsonData({ command: 'start', data:'Bin\\mariadb\\bin\\mysqld.exe', parameter:'', path:'', showcmd: true});		
		}
};

function startWebserver() {
		if (process_status['nginx'] == true)
		{
			postJsonData({ command: 'start', data:'Bin\\nginx\\nginx.exe', parameter:'-c Bin/nginx/conf/nginx.conf -s stop', path:'', showcmd: true});		
		} else {
			postJsonData({ command: 'start', data:'Bin\\nginx\\nginx.exe', parameter:'-c Bin/nginx/conf/nginx.conf' , path:'', showcmd: true});		
		}
};

function startPhp() {
		if (process_status['php'] == true)
		{
			postJsonData({ command: 'kill', data:'php-cgi'});		
		} else {
			postJsonData({ command: 'start', data:'Bin\\php\\php-cgi.exe', parameter:'-b localhost:9100', path:'', showcmd: false});		
		}
};

function startPostgre() {
		if (process_status['postgres'] == true)
		{
			postJsonData({ command: 'start', data:'Bin\\pgsql\\bin\\pg_ctl.exe', parameter:'stop --pgdata /bin/pgsql/data --log /logs/postgresql.log --mode=fast -W', path:'', showcmd: true});		
		} else {
			postJsonData({ command: 'start', data:'Bin\\pgsql\\bin\\pg_ctl.exe', parameter:'--pgdata /bin/pgsql/data --log /logs/postgresql.log start', parameter:'', path:'', showcmd: true});		
		}
};

function startMongoDb() {
		if (process_status['mongo'] == true)
		{
			postJsonData({ command: 'start', data:'bin\\mongodb\\bin\\mongo.exe', parameter:' --eval "db.getSiblingDB(\'admin\').shutdownServer()"', path:'', showcmd: true});		
		} else {
			postJsonData({ command: 'start', data:'bin\\mongodb\\bin\\mongo.exe', parameter:'--config /bin/mongodb/mongodb.conf --dbpath /bin/mongodb/data/db --logpath /logs/mongodb.log --rest', parameter:'', path:'', showcmd: true});		
		}
};

function startMemcached() {
		if (process_status['memcached'] == true)
		{
			postJsonData({ command: 'kill', data:'memcached'});	
		} else {
			postJsonData({ command: 'start', data:'bin\\\\memcached\\memcached.exe', parameter:'', parameter:'', path:'', showcmd: true});		
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
	Page Loading (ajax, external)
*************************************************************************************************/
function loadExternal(src) {
	$('#ui-view').html('<iframe id="frame" src="' + src + '" width="100%" frameBorder="0" style="margin: -15px !important;"></iframe>');
	$('iframe#frame').load(function() {
		//alert("loaded");	
		$('#frame').css('height', $(window).height() - 55);
		/*var head = jQuery("#frame").contents().find("head");
		var css = '<style type="text/css">' +
				  '#banner{display:none}; ' +
				  '</style>';
		jQuery(head).append(css);			*/
	});	
}

function getMenuIcon(key) {
	return "/cache/icons/" + key + "_16x16.png";
	alert(key);
}

function loadURL(url, container) {
	$.ajax({
		type : "GET",
		url : url,
		dataType : 'html',
		cache : true,
		beforeSend : function() {
			container.html('<br><bt><h1 style="margin-top:10px; display:block; text-align:center"><i class="fa fa-cog fa-spin"></i> Loading...</h1>');
			if (animateAjaxLoad == true) {
				if (container[0] == $("#content")[0]) {
					$("html").animate({
						scrollTop : 0
					}, "fast");
				}
			}
		},
		success : function(data) {
			if (animateAjaxLoad == true) {
				container.css({
					opacity : '0.0'
				}).html(data).delay(50).animate({
					opacity : '1.0'
				}, 300);
			} else {
				container.html(data);
			}
			// Add Active class after success
			$("li.active").removeClass("active");
			var href = $("#menuLinks li a").filter(function() {
				if ($(this).attr('href') === url) {
					$(this).parents('li').addClass('active');
				}
			});

		},
		error : function(xhr, ajaxOptions, thrownError) {
			container.html('<br><bt><h4 style="margin-top:10px; display:block; text-align:center"><i class="fa fa-warning txt-color-orangeDark"></i> Error 404! Page not found.</h4>');
		},
		async : false
	});
}



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