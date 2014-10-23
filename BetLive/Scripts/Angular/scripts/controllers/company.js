'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:CompanyCtrl
 * @description
 * # CompanyCtrl
 * Controller of the bettingApp
 */
angular.module('bettingApp')
  .controller('CompanyCtrl',['$scope','dataService','$routeParams','$location' ,function ($scope,betData,$routeParams,$location) {
        var url="Company";
        $scope.message='';
        $scope.showCreate=false;
         $scope.companyRows=null;
        $scope.sortField='companyId';
        $scope.reverse=false;
       $scope.companyRow=function(id){
                this.companyId=id,
                this.companyName=null,
                this.companyLocation=null;
                this.branches=[];
        };

      $scope.loadCompanies = function() {
          betData.get("Company", $scope).then(
              function(results, status, headers, config) {
                  //$scope.companies=results.data;
                  $scope.companyRows = new Array();
                  angular.forEach(results, function(c, i) {
                      var companyRow = new $scope.companyRow(c.companyId);
                      companyRow.companyName = c.companyName;
                      companyRow.companyLocation = c.companyLocation;
                      $scope.companyRows.push(companyRow);
                  });

              });
      };
 $scope.$on('$routeChangeSuccess',function(){  $scope.loadCompanies(); });

        $scope.company={
            "companyId":"",
                companyName:"",
                companyLocation:"",
            branches:""
        };
  $scope.onSave=function(){
      var company={};
      company=$scope.company;
      company.companyId=0;
      betData.post('Company', company, $scope).then(function() {
          $scope.showCreate = false;
          $scope.message = company.companyName + " " + "saved successfully, you can now add branches";
          $scope.loadCompanies();
          //$scope.company.companyName='';
          //$scope.company.companyLocation='';
          $scope.reverse = true;
          $scope.company = null;
      });
  };
        $scope.onDeleteCompany=function(companyId,companyIndex) {

            betData.delete('Company/DeleteCompany?id=' + companyId, $scope).then(function(result) {

                var _companyIndex = $scope.companyRows.indexOf(companyIndex);
                $scope.companyRows.splice(_companyIndex, 1);
                $scope.message = result.companyName + " deleted successfully along with its branches branches";


            });
        };

        $scope.promptToCreateCompany=function(){

           $scope.showCreate=!$scope.showCreate;
        };
    }]);
