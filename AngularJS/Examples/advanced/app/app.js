// define the page as application (module is a container)
var my_module = angular.module('my_application', ['ngRoute', 'ngAnimate', 'some_module']);

// add factories to the application
var factories = {};
factories.users_factory = users_factory;
factories.products_factory = products_factory;
my_module.factory(factories);

// add controllers to the application
var controllers = {};
controllers.users_controller = users_controller;
controllers.products_controller = products_controller;
my_module.controller(controllers);

// add custom filters to the application
var filters = {};
filters.toId = toId_filter;
filters.isSamsung = isSamsung_filter;
filters.StartsWith = productStartsWith_filter;
filters.NameLengthRange = productNameLengthRange_filter;
my_module.filter(filters);

// add custom directives to the application
var directives = {};
directives.ngCopyright = ngCopyright;
directives.ngProductInfo = ngProductInfo;
directives.ngSomeProduct = ngSomeProduct;
directives.ngClickMe = ngClickMe;
directives.ngColorsList = ngColorsList;
directives.ngMyDivCss = ngMyDivCss;
directives.ngEvents = ngEvents;
my_module.directive(directives);

// add routing mechanism to the application
function routing($routeProvider) {
    $routeProvider
		.when('/users/:param1', {
		    controller: 'users_controller',
		    templateUrl: 'app/views/userInfo.htm'
		})
        .when('/users', {
            controller: 'users_controller',
            templateUrl: 'app/views/users.htm'
        })
		.when('/products', {
		    controller: 'products_controller',
		    templateUrl: 'app/views/products.htm'
		})
		.otherwise({
		    redirectTo: '/'
		})
};

my_module.config(routing);