
var module = angular.module('testAvi', []);

module.controller('homeConroller', function ($scope, productsFactory) {
    (function () {
        $scope.Title = 'Hello Avi';
    })();   

    $scope.names = ['Roby', 'Avi', 'Shirly', 'Gabi', 'Bonnie', 'Barak', 'Ronen'];
    $scope.products = productsFactory.fun2();

    $scope.check = function (name, maxLength) {
        return name.length > maxLength;
    };

    productsFactory.fun1(function (result) {
        $scope.songName = result;
    });
});

module.filter('startWith', function () {
    return function (items, startWith, prefix) {
        var filtered = [];
 
        if (!startWith || startWith == '')
            return items;

        for (var i = 0; i < items.length; i++) {
            if (items[i].indexOf(startWith) == 0)
                filtered.push(prefix.concat(' ', items[i]));
        }
        return filtered;
    };
});

module.filter('myId', function () {
    return function (item) {
        if (!item)
            return item;
        return '# '.concat(item);
    };
});

/*
types:
- Element (E)
  <ng-my-directive></ng-my-directive>

- Attribute (A)
  <span ng-my-directive></span>

- CSS class (C)
  <span class="ng-my-directive: expression;"></span>

- Comment (M)
  <!-- directive: ng-my-directive expression -->
*/

module.directive('someDirective', function () {
    return {
        restrict: 'EA',
        replace: true,
        templateUrl: 'partial.html',
        scope: {},
        link: function (scope, element, attributes) {  }
    }
});

module.directive('otherDirective', function () {
    return {
        restrict: 'EA',
        replace: true,
        template: '<div><h3>{{ title }}</h3><p>{{ product.name }}</p></div>',
        scope: {            
            title: "@title", // byVal
            product: "=product", // byRef
        },
        link: function (scope, element, attributes) { }
    }
});

module.factory('productsFactory', function ($http) {

    var products = [
        { id: 10, name: 'ItemA' },
        { id: 20, name: 'ItemB' },
        { id: 30, name: 'ItemC' },
        { id: 10, name: 'ItemD' },
        { id: 20, name: 'ItemE' },
        { id: 30, name: 'ItemF' }

    ];
    
    return {
        fun1: function (onSuccess) {            
            $http.get('http://ws.audioscrobbler.com/2.0/?method=track.search&track=love&api_key=0604d2c5492dc743997cabe3fd636099&format=json&limit=10').success(function (response) {
                onSuccess(response.results.trackmatches.track[0].name);
            });
        },
        fun2: function () {
            return products;
        }
    };
})