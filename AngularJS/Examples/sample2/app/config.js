app.config(function ($routeProvider) {
    $routeProvider
    .when('/test1', {
        templateUrl: 'view1.partial'
    }) // ng-template
    .when('/test2', {
        templateUrl: 'view2.partial'
    }) // ng-template
    .when('/test3', {
        template: '<h1>view3</h1>'
    }) // template
    .when('/test4', {
        templateUrl: 'app/views/view4.htm'
    }) // html partial file
    .when('/test5', {
        templateUrl: 'app/views/view5.htm'
    }) // html partial file
    .when('/test6', {
        redirectTo: '/test3'
    }) // redirect to another view
    .otherwise({
        template: 'NO MATCH!!'
    })
});

