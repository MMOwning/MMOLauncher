/**
 * CybeSystems Angular Admin Template
 *
 */

/**
 * MainCtrl - controller
 */
function MainCtrl($scope, $http) {
  //Backend Path get set in indem html before angular is loaded
  $scope.GetBackendPath = getBackEndPath;
  $http.get(getBackEndPath + '/getPlatform').
	success(function(data, status, headers, config) {
	  $scope.getUserName = data.username;
	  $scope.getOS = data.os;
	  $scope.getComputername = data.computername;
	  if (data.runningasadmin == true)
	  {
		  $scope.getUAC = "Administrator";
	  } else {
		  $scope.getUAC = "User";
	  }
	}).
	error(function(data, status, headers, config) {
	  $scope.getUserName = "";
	  $scope.getOS = "";
	  $scope.getComputername = "";
	  $scope.getUAC = "User";
	});	
	
	//this.userName = 'Example user';
	this.helloText = 'Welcome in SeedProject';
	this.descriptionText = 'It is an application skeleton for a typical AngularJS web app. You can use it to quickly bootstrap your angular webapp projects and dev environment for these projects.';

};


angular
	.module('cyadmin')
	.controller('MainCtrl', MainCtrl)
	.controller('DashboardCtrl', function($scope, $rootScope) {
		checkServerVersion();
		checkRunningApps();
		var checkRunningAppsInterval = setInterval(function() {
			//console.log("Start checkRunningAppsInterval");
			checkRunningApps();
		}, 1000);		
		//Reset Interval on page change
		$scope.$on('$stateChangeStart', function() {
			console.log("clear interval");
			clearInterval(checkRunningAppsInterval);		
		});
	})
	.controller('ConfigCtrl', function ($scope, $http, $interval) {
        $scope.GetBackendPath = getBackEndPath;
		$http.get(getBackEndPath + '/config')
		   .then(function (res) {
			$scope.cybeSystemsMainSettings = res.data;
		});
	
		$('#mainConfigForm').find(':input').each(function () {
			$(this).on("propertychange, change, keyup, paste, input", function () {
				event.preventDefault();
                postFormDataEncodeUriAngular("mainConfigForm", "/config/config_save", false);
			});
			$(this).on('ifChanged', function (event) {
				event.preventDefault();
                postFormDataEncodeUriAngular("mainConfigForm", "/config/config_save", false);
			});
		
		
		});
	})