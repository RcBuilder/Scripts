function users_controller($scope, users_factory, $routeParams) {
    (function () {
        // add 'users' variable to the current scope 
        // set it's data from users factory
        $scope.users = users_factory.get_users();

        $scope.$watch('message_model', function () {
            $scope.message_model = $scope.message_model.replace('e', 'E');
        });

        $scope.userName = $routeParams.param1;
    })();

    // public (added to the scope)
    $scope.add_user = function () {
        // new_user_age and new_user_name are model defined within the view and recognize in the scope
        users_factory.add_user({
            name: $scope.new_user_name,
            age: $scope.new_user_age
        });
    }
}