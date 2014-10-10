angularExample.app.controller(
		"summaryReportController",
		["$scope", "$location", "$routeParams", "upida", function ($scope, $location, $routeParams, upida) {

		    $scope.Summary = [];
		    $scope.BranchSummary = [];
		    $scope.Branches = [];
		   // alert("test");
		    $scope.loadSummary= function () {	     
		        upida.get("Admin/SummaryReports", $scope)
                .then(function (items) {
                   // alert(items);
                    $scope.Summary = items;
                    // console.Write(alert(items));
                    // alert($scope.BranchSummary.totalStake);
                });
		    };
		    $scope.LoadBranches = function () {
		        upida.get("Admin/BranchDetails/", $scope)
                .then(function (items) {
                 //   alert(items);
                    $scope.Branches = items;
                });
		    };
		    $scope.BranchReport = function (id) {
		      
		        upida.get("Admin/BranchSummaryReports/"+id, $scope)
                .then(function (items) {
                  // alert(items);
                    $scope.BranchSummary = items;
                    console.Write(items);
                    // alert($scope.BranchSummary.totalStake);
                });
		    };
		    $scope.Details = function (id,flag) {
		        rec = id.replace("/", "d");
		        rec = rec.replace("/", "d");
		       // alert(rec);
		        window.location.href = ("../../admin/TicketSales/"+rec+"/"+flag);
		       
		    };

		}]);