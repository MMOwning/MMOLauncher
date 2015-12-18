var express = require('express');
var app = express();
var path = require('path');

app.get('/', function (req, res) {
  //res.send('Hello World!');
  res.sendFile(path.join(__dirname + '/index.html'));
  var edge = require('edge');
});

app.get('*', express.static(__dirname));

var server = app.listen(3000, function () {
  var host = server.address().address;
  var port = server.address().port;

  console.log('Example app listening at http://%s:%s', host, port);
});



var exec = require('child_process').exec;
var spawn = require('child_process').spawn;
var edge = require('edge');
global.edge = edge;

//Setup Tail
var Tail = require('always-tail');
var fs = require('fs');

module.exports = app;