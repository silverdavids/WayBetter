angularExample.app.controller(
		"managerController",
		["$scope", "$location", "$routeParams", "upida", function ($scope, $location, $routeParams, upida) {
		   
		    $scope.BranchSummary = [];
	
		    $scope.tellers = [];
		    $scope.loadBranch= function () {
		     
		        upida.get("Manager/BranchSummary", $scope)
                .then(function (items) {
                   alert(items);
                   $scope.BranchSummary = items;
                   alert("back");
                    console.Write(BranchSummary);
                   // alert($scope.BranchSummary.totalStake);
                 
                });
		    };

		    $scope.loadTellers = function () {
		        alert("Tellers");
		        upida.get("Manager/BranchTeller", $scope)
                .then(function (items) {
               
                   // $scope.tellers = items;
                    alert("Tellersback");
                   
                });
		    };

		}]);