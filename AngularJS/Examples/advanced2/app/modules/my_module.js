var m = angular.module('my_module', []);

// controllers
m.controller('home_controller', function ($scope, users_factory, my_consts, $location) {
    $scope.arr_numbers = [123, 12, 2, 78, 144, 41, 67];
    $scope.user = { name: 'Roby Cohen', age: 35, gender: 'male' }

    $scope.someName = 'Anonymous';
    $scope.someCssClass = '';
    $scope.randomNumber = 0;
    $scope.isEven = true;

    $scope.yourName = 'Roby Cohen';
    $scope.whoAmI = 'hello from home_controller';

    $scope.users = users_factory.get();
    $scope.isTooLong = false;
    $scope.constValue = my_consts.value1;

    var arr_names = ['Roby', 'Avi', 'Shirly', 'Ron', 'Erez', 'Galit'];   
    $scope.changeSomeName = function () {
        var rnd_index = Math.floor(Math.random() * arr_names.length);
        $scope.someName = arr_names[rnd_index];
    }
    
    $scope.getRandomNumber = function () {
        $scope.randomNumber = Math.floor(Math.random() * 100);
        $scope.isEven = $scope.randomNumber % 2 == 0;
    }

    var arr_css_classes = ['blueTitle', 'redTitle', 'greenTitle', 'purpleTitle', 'aqueTitle'];    
    $scope.changeCssClass = function () {
        var rnd_index = Math.floor(Math.random() * arr_css_classes.length);
        $scope.someCssClass = arr_css_classes[rnd_index];
    }
    
    $scope.sayHello = function () {
        return 'Hello ' + $scope.yourName;
    }
    
    $scope.$watch('someModel', function (newVal, oldVal) {
        $scope.isTooLong = newVal.length > 10;
    });

    $scope.foo1 = function () {
        alert(1);
    }

    $scope.foo2 = function () {
        alert(2);
    }

    $scope.foo3 = function (msg) {
        alert(msg);
    }

    $scope.redirectToView = function (view) {
        $location.path('/' + view);
    }

    // using jquery (jquery.min.js included)
    //$('h4').css({ border: 'solid 1px #000', color: 'blue' });
    var selector = angular.element('h4');
    selector.css({
        border: 'solid 1px #000',
        color: 'blue'
    });

});

m.controller('sub_controller', function ($scope) {
    $scope.whoAmI = 'hello from sub_controller';
    $scope.value1 = 'some value';
});

m.controller('my_controller', function ($scope, my_factory) {
    $scope.originValue = my_factory.get();
    my_factory.reverse();
    $scope.reverseValue = my_factory.get();
});

// broadcast = bubble down, emit = bubble up
// see 'angular $broadcast and $emit.txt'
m.controller('level1_controller', function ($scope) {
    $scope.$on('sayHello', function (event) {
        console.log('Hello from level1');
    });
    
    $scope.broadcast = function () {
        $scope.$broadcast('sayHello'); // down the chain 
    }

    $scope.emit = function () {
        $scope.$emit('sayHello'); // up the chain 
    }
});

m.controller('level2_controller', function ($scope) {
    $scope.$on('sayHello', function (event) {
        console.log('Hello from level2');
    });

    $scope.broadcast = function () {
        $scope.$broadcast('sayHello');
    }

    $scope.emit = function () {
        $scope.$emit('sayHello');
    }
});

m.controller('level3_controller', function ($scope) {
    $scope.$on('sayHello', function (event) {
        console.log('Hello from level3');
    });

    $scope.broadcast = function () {
        $scope.$broadcast('sayHello');
    }

    $scope.emit = function () {
        $scope.$emit('sayHello');
    }
});

// factories
m.factory('users_factory', function () {
    var friends = [
				{ name: 'RON', lname: 'buzaglo', age: 39 },
				{ name: 'LITAL', lname: 'cohen', age: 26 },
				{ name: 'EREZ', lname: 'sabag', age: 35 },
				{ name: 'MAAYAN', lname: 'aacvd', age: 34 },
				{ name: 'RICKI', lname: 'edri', age: 28 }
			];

    var factory = {};
    factory.get = function () {
        return friends;
    };

    return factory;
});

m.factory('my_factory', function () {
    var value = 'hello from factory';

    var factory = {};
    factory.get = function () {
        return value;
    };
    factory.set = function (new_value) {
        value = new_value
    };

    return factory;
});

// directives
m.directive('mydirective', function () {
    return {
        restrict: 'E',
        template: '<span>byval = {{ byval }}, byref = {{ byref }}</span>',
        scope: { byval: '@', byref: '=' }
    }
});

m.directive('somedirective', function () {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: 'somedirectiveTemplate.partial',
        scope: {},
        link: function (scope, element, attributes) {
            scope.local_var = 'some local variable';
            element.html(scope.local_var + '|' + attributes.a1 + '|' + attributes.a2);
        }
    }
});

m.directive('ngFunctions', function () {
    return {
        restrict: 'E',
        template: '<button ng-click="myclick1()">CLICK ME 1</button>' +
                  '<button ng-click="myclick2()">CLICK ME 2</button>' +
                  '<button ng-click="myclick3()">CLICK ME 3</button>' +
                  '<button ng-click="myclick4()">CLICK ME 4</button>' +
                  '<button ng-click="myclick4({yourName:\'override it!\'})">CLICK ME 5</button>' +
                  '<button ng-click="myclick4({yourName:new_yourName})">CLICK ME 6</button>',
        controller: function ($scope) {
            $scope.new_yourName = 'override your name';
        },
        scope: {
            myclick1: '&', // simple
            myclick2: '&', // simple
            myclick3: '&myclickwithparam',  // passing const param
            myclick4: '&myclickwithparam2' // passing binding param and override param (using const value or local scope variable)    
        }
    };
});

m.directive('ngWithTransclude', function () {
    return {
        restrict: 'E',
        transclude: true,
        template: '<div>' +
                    '<div ng-transclude></div>' +
                        'this is the directive template content' +
                    '<div ng-transclude></div>' +
                  '</div>'
    }
});

m.directive('ngH1', function () {
    return {
        restrict: 'E',
        template: '<h1>I AM H1</h1>',
        replace: true,
        scope: {},
        link: function (scope, element, attributes) {
            // notice! if jquery included the element refer to it, otherwise it refer to the built-in jqLite library
            element.addClass('box');
            element.bind('click', function (e) {
                element.toggleClass('box');
            });
            element.prepend('< ');
            element.append(' >');
        }
    }
});

// constants
m.constant('my_consts', {
    value1: 'some consts data',
    value2: 'another static content'
});

// decorators
m.config(function ($provide) {
    $provide.decorator('my_factory', function ($delegate) {
        $delegate.reverse = function () {
            $delegate.set($delegate.get().split('').reverse().join(''));
        };
        return $delegate;
    });
});