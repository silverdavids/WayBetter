angularExample.app.controller(
		"recieptController",
		["$scope", "$location", "$routeParams", "upida", function ($scope, $location, $routeParams, upida) {

		    $scope.Reciepts= [];
		    $scope.ReceiptStatus = [{ StateId: 1, StateName: 'Pending' }, { StateId: 2, StateName: 'Lost' },  { StateId: 3, StateName: 'Won' }, { StateId: 4, StateName: 'Paid' }, { StateId: -1, StateName: 'Cancelled' }];

		    $scope.loadReciepts = function () {
		     
		        upida.get("Recept/GetReciepts", $scope)
                .then(function (items) {
                    $scope.Reciepts = items;             
                });
		    };
		
		    $scope.selectReceipts = function () {
		    
		        //alert($scope.StatusId);
		        upida.get("Recept/GetRecieptsByStatus/" + $scope.StatusId, $scope)
                .then(function (items) {
                    $scope.Reciepts = items;
                });
		    };

		}]);