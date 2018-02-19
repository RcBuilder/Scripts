var module = angular.module('my-app', []);

module.controller('homeController', function ($scope, tracksFactory) {
    $scope.myList = [];
    $scope.tracks = [];

    $scope.AddToList = function (index) {
        $scope.myList.push($scope.tracks[index]);
    }

    $scope.Search = function () {
        tracksFactory.searchForTracks($scope.searchPhrase, function (response) {
            $scope.tracks = response;
        });
    }

    $scope.LoadAutocomplete = function () {
        if ($scope.searchPhrase.lengh < 3) return;
        tracksFactory.getNames($scope.searchPhrase, function (response) {
            angular.element("#searchPhrase").autocomplete({
                source: response
            });
        });
    }   
});

module.factory('tracksFactory', function ($http) {
    var server = 'http://ws.audioscrobbler.com/2.0/';
    var queryBase = '?method=track.search&track={0}&api_key=0604d2c5492dc743997cabe3fd636099&format=json&limit={1}';

    var foreach = function (arr, callback) {
        for (var i = 0; i < arr.length; i++)
            callback.call(arr[i]);
    };

    return {
        getNames: function (searchPhrase, onSuccess) {
            $http.get(server.concat(queryBase.replace('{0}', searchPhrase).replace('{1}', 10)))
                .success(function (response) {
                    var trackNames = [];
                    foreach(response.results.trackmatches.track, function () {
                        trackNames.push(this.name);
                    });
                    
                    onSuccess(trackNames);
                })
        },
        searchForTracks: function (searchPhrase, onSuccess, onFailure) {
            $http.get(server.concat(queryBase.replace('{0}', searchPhrase).replace('{1}', 10)))
                .success(function (response) {
                    var tracks = [];

                    foreach(response.results.trackmatches.track, function () {
                        tracks.push({
                            name: this.name,
                            image: this.image[1]['#text'],
                            artist: this.artist,
                            url: this.url,
                            id: this.mbid
                        });
                    });                        

                    onSuccess(tracks);
                })
        }
    };
});

module.directive('ngTrack', function () {
    return {
        restrict: 'E',
        templateUrl: 'track.partial',        
        replace: true,
        scope: {
            track: '=track',
            click: '&click'
        }
    }
});
