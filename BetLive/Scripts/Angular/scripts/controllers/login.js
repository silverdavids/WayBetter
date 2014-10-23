'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:LoginCtrl
 * @description
 * # LoginCtrl
 * Controller of the bettingApp
 */




'use strict';
bettingApp.controller('loginCtrl', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.loginData = {
        userName: "",
        password: ""
    };

    $scope.message = "";

    $scope.login = function () {

        authService.login($scope.loginData).then(function (response) {

                $location.path('/matches');

            },
            function (err) {
                $scope.message = err.error_description;
            });
    };

}]);