function products_controller($scope, products_factory) {
    (function () {
        products_factory.get_products(
            function (data) {
                $scope.products = data.products;
            },
            function () {
                alert('ERROR');
            }
        );

        // custom orderBy
        $scope.orderByProductNameLength = function (product) {
            return product.name.length;
        };

        $scope.orderByRandom = function () {
            return Math.random();
        };

        $scope.foo1 = function () {
            alert(1);
        }

        $scope.foo2 = function () {
            alert(2);
        }
    })();
}