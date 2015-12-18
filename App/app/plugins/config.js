/**
 * CybeSystems Angular Admin Template
 *
 * AngularUI Router to manage routing and views
 * Each view are defined as state.
 * Initial there are written stat for all view in theme.
 *
 */
 
function config($stateProvider, $urlRouterProvider, $ocLazyLoadProvider) {
    $urlRouterProvider.otherwise("/index/main");

    $ocLazyLoadProvider.config({
        // Set to true if you want to see what and when is dynamically loaded
        debug: false
    });	
	
    $stateProvider

        .state('index', {
            abstract: true,
            url: "/index",
            templateUrl: "views/common/content.html",
        })
        .state('index.main', {
            url: "/main",
			controller: "DashboardCtrl",
            templateUrl: "views/main.html",
            data: { pageTitle: 'Example view' },
            /*resolve: {
                loadPlugin: function ($ocLazyLoad) {
                    return $ocLazyLoad.load([
                        {
							//Load MMOwning.js with ocLazyLoad -> This makes sure that DOM is ready
                            files: [ 'plugins/mmowning.js' ],
							rerun: true,
							cache: false
                        }
                    ])
					.then(function(){
						//Nothing to do yet -> We use DashboardCtrl - $ocLazyLoad.load.then is triggered only once
					});
                }
            },		*/	
        })
        .state('index.minor', {
            url: "/minor",
            templateUrl: "views/minor.html",
            data: { pageTitle: 'Example view' }
        })
        .state('index.logs_nginx', {
            url: "/logs_nginx",
            templateUrl: "views/logs_nginx.html",
            data: { pageTitle: 'Example view' }
        })		
        .state('index.logs', {
            url: "/logs",
            templateUrl: "views/logs.html",
            data: { pageTitle: 'Example view' }
        })		
		
		
}
angular
    .module('cyadmin')
    .config(config)
    .run(function($rootScope, $state) {
		//Make sure every interval setup by state pages is clear
		$rootScope.$on('$stateChangeSuccess', function () {
			setupPage();
		});		
        $rootScope.$state = $state;
    });