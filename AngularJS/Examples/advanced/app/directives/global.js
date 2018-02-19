function ngCopyright() {
    return {
        restrict: 'EM',
        template: '<div>&copy; copyright to <a href="http://www.rcb.co.il">RcBuilder</a></div>',
        replace: true
    };
}

function ngClickMe() {
    return {
        restrict: 'E',
        template: '<div style="cursor:pointer;">CLICK ME</div>',
        replace: true,
        link: function ($scope, element, attributes) {
            element.bind('click', function () {
                element.html('YOU CLICKED ME!');
            });
        }
    };
}

function ngColorsList($http) {
    return {
        restrict: 'A',
        template: '<span ng-repeat="color in colors" style="color:{{color}}">{{color}}&nbsp;&nbsp;</span>',
        link: function ($scope, element, attributes) {
            $scope.colors = ['red', 'green', 'blue', 'purple', 'violet', 'pink', 'brown', 'aqua'];
        }
    };
}

function ngMyDivCss() {
    return {
        restrict: 'C',
        link: function ($scope, element, attributes) {
            element.css({
                width: '300px',
                backgroundColor: 'silver',
                padding: '12px',
                color: '#fff'
            });
        }
    };
}

function ngEvents() {
    return {
        restrict: 'E',
        template: '<button ng-click="myclick1()">CLICK ME 1</button><button ng-click="myclick2()">CLICK ME 2</button>',
        scope: {
            myclick1: '&',
            myclick2: '&'
        }
    };
}