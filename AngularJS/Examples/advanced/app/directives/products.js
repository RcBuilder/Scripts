function ngProductInfo() {
    return {
        restrict: 'EA',
        template:  '<div style="border:solid 1px #888;background-color:#ddd;width:300px;padding:6px;">' +
                       '<div><b>{{ title }}</b></div>' +
                       '<div>id : {{ product.id }}</div>' +
                       '<div>name : {{ product.name }}</div>' +
                       '<div>price : {{ product.price }}</div>' + 
                   '</div>',
        replace: true,
        scope: {
            product: "=product",
            title: "@title"
        }
    };
}

function ngSomeProduct() {
    return {
        restrict: 'EA',
        template: '<div style="border:solid 1px #888;background-color:#ddd;width:300px;padding:6px;">' +
                       '<div>id : {{ id }}</div>' +
                       '<div>name : {{ name }}</div>' +
                       '<div>price : {{ price }}</div>' +
                   '</div>',
        replace: true,
        scope: {},
        controller: function ($scope, products_factory) {
            $scope.id = 1234;
            $scope.name = 'some product';
            $scope.price = 100;
        }
    };
}