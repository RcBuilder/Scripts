var module = angular.module('my-app', []);

module.controller('homeController', function ($scope, productsFactory) {
    $scope.products = [];
    
    $scope.addProduct = function () {
        var id = parseInt(Math.random() * 1000);
        var product = { id: id, name: $scope.newProduct };
        productsFactory.addProduct(product, function (response) {
            $scope.products = response;
        });
    }

    $scope.loadProducts = function () {
        productsFactory.getProducts(function (response) {
            $scope.products = response;
        });
    }

    $scope.changeProductState = function (obj, index) {
        var product = $scope.products[index];
        product.state = !product.state;
        productsFactory.updateProduct(product, function (response) {
            $scope.products = response;
        });
    }

    $scope.removeProduct = function (index) {
        var product = $scope.products[index];
        productsFactory.deleteProduct(product, function (response) {
            $scope.products = response;
        });        
    }

    $scope.loadProducts();
});

module.factory('productsFactory', function ($http) {
    var server = 'api/';
    
    var foreach = function (arr, callback) {
        for (var i = 0; i < arr.length; i++)
            callback.call(arr[i]);
    };

    var loadProducts = function (response) {
        var products = [];
        foreach(response, function () {
            products.push({
                id: this.Id,
                name: this.Name,
                state: this.State
            });
        });
        return products;
    }


    return {
        getProducts: function (onSuccess, onFailure) {
            $http.get(server)
            .success(function (response) {
                var products = loadProducts(response);
                onSuccess(products);
            })
        },
        addProduct: function (product, onSuccess, onFailure) {
            $http.put(server, product)
            .success(function (response) {
                var products = loadProducts(response);
                onSuccess(products);
            })
        },
        deleteProduct: function (product, onSuccess, onFailure) {
            $http.delete(server.concat('?Id=', product.id))
            .success(function (response) {
                var products = loadProducts(response);
                onSuccess(products);
            }).error(function (response, code) {
                alert(code);
            });
        },  
        updateProduct: function (product, onSuccess, onFailure) {
            $http.post(server, product)
            .success(function (response) {
                var products = loadProducts(response);
                onSuccess(products);
            }).error(function (response, code) {
                alert(response.ExceptionMessage);
            });
        }
    };
});