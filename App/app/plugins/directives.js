/**
 * CybeSystems Angular Admin Template
 *
 */


/**
 * pageTitle - Directive for set Page title - mata title
 */
function pageTitle($rootScope, $timeout) {
	return {
		link: function(scope, element) {
			var listener = function(event, toState, toParams, fromState, fromParams) {
				// Default title - load on Dashboard 1
				var title = 'CybeSystems | Angular Admin Template';
				// Create your own title pattern
				if (toState.data && toState.data.pageTitle) title = 'CybeSystems | ' + toState.data.pageTitle;
				$timeout(function() {
					element.text(title);
				});
			};
			$rootScope.$on('$stateChangeStart', listener);
		}
	}
};

/**
 * sideNavigation - Directive for run metsiMenu on sidebar navigation
 */
function sideNavigation($timeout) {
	return {
		restrict: 'A',
		link: function(scope, element) {
			// Call the metsiMenu plugin and plug it to sidebar navigation
			/*$timeout(function(){
				element.metisMenu();
			});*/
		}
	};
};

/**
 * iboxTools - Directive for iBox tools elements in right corner of ibox
 */
function iboxTools($timeout) {
	return {
		restrict: 'A',
		scope: true,
		templateUrl: 'views/common/ibox_tools.html',
		controller: function ($scope, $element) {
			// Function for collapse ibox
			$scope.showhide = function () {
				var ibox = $element.closest('div.ibox');
				var icon = $element.find('i:first');
				var content = ibox.find('div.ibox-content');
				content.slideToggle(200);
				// Toggle icon from up to down
				icon.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
				ibox.toggleClass('').toggleClass('border-bottom');
				$timeout(function () {
					ibox.resize();
					ibox.find('[id^=map-]').resize();
				}, 50);
			},
				// Function for close ibox
				$scope.closebox = function () {
					var ibox = $element.closest('div.ibox');
					ibox.remove();
				}
		}
	};
};

/**
 * minimalizaSidebar - Directive for minimalize sidebar
 */
function minimalizaSidebar($timeout) {
	return {
		restrict: 'A',
		template: '<a class="sidebar-toggle" " href="" ng-click="minimalize()"></a><span class="sr-only">Toggle navigation</span>',
		controller: function ($scope, $element) {
			$scope.minimalize = function () {
				
			var screenSizes = $.AdminLTE.options.screenSizes;
			//Enable sidebar push menu
			if ($(window).width() > (screenSizes.sm - 1)) {
			  if ($("body").hasClass('sidebar-collapse')) {
				$("body").removeClass('sidebar-collapse').trigger('expanded.pushMenu');
			  } else {
				$("body").addClass('sidebar-collapse').trigger('collapsed.pushMenu');
			  }
			}
			//Handle sidebar push menu for small screens
			else {
			  if ($("body").hasClass('sidebar-open')) {
				$("body").removeClass('sidebar-open').removeClass('sidebar-collapse').trigger('collapsed.pushMenu');
			  } else {
				$("body").addClass('sidebar-open').trigger('expanded.pushMenu');
			  }
			}				

			}
		}
	};
};

function controlSidebar($timeout) {
	return {
		restrict: 'A',
		template: '<a href="#" ng-click="showhide()" data-toggle="control-sidebar"><i class="fa fa-gears"></i></a>',
		controller: function ($scope, $element) {
			$scope.showhide = function () {
				//$.AdminLTE.controlSidebar.open(".control-sidebar");

			var sidebar = $(".control-sidebar");
			var o = $.AdminLTE.options.controlSidebarOptions;
			
		//If the sidebar is not open
		if (!sidebar.hasClass('control-sidebar-open')
				&& !$('body').hasClass('control-sidebar-open')) {
		  //Open the sidebar
		  $.AdminLTE.controlSidebar.open(sidebar, o.slide);
		} else {
		  $.AdminLTE.controlSidebar.close(sidebar, o.slide);
		}

	
			}
		}
	};
};

/**
 * iboxTools with full screen - Directive for iBox tools elements in right corner of ibox with full screen option
 */
function iboxToolsFullScreen($timeout) {
	return {
		restrict: 'A',
		scope: true,
		templateUrl: 'views/common/ibox_tools_full_screen.html',
		controller: function ($scope, $element) {
			// Function for collapse ibox
			$scope.showhide = function () {
				var ibox = $element.closest('div.ibox');
				var icon = $element.find('i:first');
				var content = ibox.find('div.ibox-content');
				content.slideToggle(200);
				// Toggle icon from up to down
				icon.toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');
				ibox.toggleClass('').toggleClass('border-bottom');
				$timeout(function () {
					ibox.resize();
					ibox.find('[id^=map-]').resize();
				}, 50);
			};
			// Function for close ibox
			$scope.closebox = function () {
				var ibox = $element.closest('div.ibox');
				ibox.remove();
			};
			// Function for full screen
			$scope.fullscreen = function () {
				var ibox = $element.closest('div.ibox');
				var button = $element.find('i.fa-expand');
				$('body').toggleClass('fullscreen-ibox-mode');
				button.toggleClass('fa-expand').toggleClass('fa-compress');
				ibox.toggleClass('fullscreen');
				setTimeout(function() {
					$(window).trigger('resize');
				}, 100);
			}
		}
	};
}



/**
 *
 * Pass all functions into module
 */
angular
	.module('cyadmin')
	.directive('pageTitle', pageTitle)
	.directive('sideNavigation', sideNavigation)
	.directive('iboxTools', iboxTools)
	.directive('minimalizaSidebar', minimalizaSidebar)
	.directive('controlSidebar', controlSidebar)
	.directive('iboxToolsFullScreen', iboxToolsFullScreen);
