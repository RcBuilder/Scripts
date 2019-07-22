var module = angular.module('my-app', []);

module.controller('homeController', function ($scope, searchesFactory) {
    $scope.searchResults = [];
    
    $scope.doSearch = function () {
        searchesFactory.getSearchResults($scope.searchPhrase, function (response) {
            $scope.searchResults = response;
        });
    }
});

module.factory('searchesFactory', function ($http) {
    var server = 'api/';
    var action = 'searches/searchTopX/{0}';

    return {
        getSearchResults: async function (phrase, onSuccess, onFailure) {
            try {
                var response = await $http.get(server.concat(action).replace('{0}', phrase));
                onSuccess(response.data.map(x => x.Title));
            }
            catch (e) {
                onFailure(e.message);
            }
        }
    }
});