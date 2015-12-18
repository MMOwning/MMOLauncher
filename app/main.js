var app = require('app');
//var BrowserWindow = require('browser-window');
const BrowserWindow = require('electron').BrowserWindow;
var WebSocket = require('ws');

const remote = require('electron').remote;


/*
var WebSocket = require('ws');
var ws = new WebSocket('ws://127.0.0.1:1415/ws');



ws.on('open', function open() {
  console.log('connected');
  ws.send(Date.now().toString(), {mask: true});
});

ws.on('close', function close() {
  console.log('disconnected');
});

ws.on('message', function message(data, flags) {
  console.log('Roundtrip time: ' + (Date.now() - parseInt(data)) + 'ms', flags);

  setTimeout(function timeout() {
    ws.send(Date.now().toString(), {mask: true});
  }, 500);
});
*/




var reconnectInterval = 3000;

var connect = function(){
	console.log('test');
    ws = new WebSocket('ws://127.0.0.1:1415/ws');
    ws.on('open', function() {
        console.log('socket open');
		//ws.send(Date.now().toString(), {mask: true});
    });
    ws.on('error', function() {
        console.log('socket error');
		setTimeout(connect, reconnectInterval);
    });
    ws.on('close', function() {
        console.log('socket close');
        setTimeout(connect, reconnectInterval);
    });
	
	ws.on('message', function message(data, flags) {
	  console.log(data);
	  if (data.id ="1")
	  {
		  //remote.getCurrentWindow().minimize();
		  //webContents.executeJavaScript(remote.getCurrentWindow().minimize());
		  //webContents.executeJavaScript(mainWindow.minimize());
		  //webContents.minimize();
		  
		  //Working
		  //https://github.com/atom/electron/blob/master/docs/api/browser-window.md
		  mainWindow.show();
		  mainWindow.focus();
		  //mainWindow.close();
		  //webContents.toggleDevTools();
	  }
	  console.log('Roundtrip time: ' + (Date.now() - parseInt(data)) + 'ms', flags);

	  setTimeout(function timeout() {
		//ws.send(Date.now().toString(), {mask: true});
	  }, 500);
	});
	
};

connect();



var mainWindow = null;
var webContents = null;

app.on('window-all-closed', function() {
 if (process.platform != 'darwin')
 app.quit();
});

app.on('ready', function() { 
 //mainWindow = new BrowserWindow({width: 1500, height: 800, frame: false}); 
 mainWindow = new BrowserWindow({width: 1500, height: 800, nodeIntegration: false}); 
 mainWindow.loadUrl('file://' + __dirname + '/index.html'); 
 webContents = mainWindow.webContents;
 mainWindow.toggleDevTools();
 mainWindow.on('closed', function() {
 mainWindow = null;
 }); 
});


