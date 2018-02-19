function some_filter() {
    return function (value) {
        return '[' + value.toString() + ']';
    };
};

function ngSomeDirective() {
    return {
        restrict: 'EM',
        template: '<div>HELLO WORLD</div>',
        replace: true
    };
}

var some_module = angular.module('some_module', []);
some_module.filter('someFilter', some_filter);
some_module.directive('ngSomeDirective', ngSomeDirective);