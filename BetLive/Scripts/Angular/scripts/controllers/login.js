'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:LoginCtrl
 * @description
 * # LoginCtrl
 * Controller of the bettingApp
 */




'use strict';
//bettingApp.controller('loginCtrl', ['$scope', '$location', 'authService', 'ngDialog', function ($scope, $location, authService, ngDialog) {

//    $scope.loginData = {
//        userName: "",
//        password: ""
//    };

//    $scope.message = "";

//    var loginDialog = null;
//    $scope.login = function () {
//        loginDialog= ngDialog.open({
//            template: 'login.html',
//            scope: $scope,
        
//            className: 'ngdialog-theme-default',
//            showClose: true,
//            closeByDocument: true,
//            closeByEscape:true,
//            name:'login'
//        });
       
//        $scope.signin = function () { }
//        //authService.login($scope.loginData).then(function (response) {

//        //        $location.path('/matches');

//        //    },
//        //    function (err) {
//        //        $scope.message = err.error_description;
//        //    });
//    };
//    $scope.cancel = function () {
//        ngDialog.close();
//        console.log("clicked");
//    }

//}]);

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