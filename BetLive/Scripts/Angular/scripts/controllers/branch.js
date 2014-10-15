'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:BranchCtrl
 * @description
 * # BranchCtrl
 * Controller of the bettingApp
 */
angular.module('bettingApp')
    .controller('BranchCtrl',['$scope','dataService','$routeParams','$location' ,function ($scope,betData,$routeParams,$location) {
        var url="Branch";
        $scope.companyId=$routeParams.companyId;
        $scope.message='';
        $scope.showCreate=false;
        $scope.branchRows=null;
        $scope.sortField='branchId';
        $scope.reverse=false;
        $scope.branchRow=function(id){
                this.branchId=id,
                this.companyId=null;
                this.branchName=null,
                this.branchLocation=null,
                this.branchCode=null,
                this.dateCreated=null,
                this.maxCash=0,
                this.minStake=0,
               // this.branchLocation=null,
                this.noOfTerminals=0,
                this.manager=null,
                this.terminals=[],
                this.Employees=[]

        };

        $scope.loadBranches=function(){
            betData.get("Branch/GetBranchByCompanyId?id="+$scope.companyId,$scope).then(
                function(results,status,headers,config){
                    $scope.branchesTerminals=results.terminals;
                    $scope.branchRows=new Array();
                    angular.forEach(results,function(c, i) {
                        var branchRow = new $scope.branchRow(c.branchId);
                        branchRow.branchName = c.branchName;
                        branchRow.branchLocation = c.branchLocation;
                        branchRow.branchCode = c.branchCode;
                        branchRow.dateCreated = c.dateCreated;
                        branchRow.maxCash = c.maxCash;
                        branchRow.minStake = c.minStake;
                        branchRow.noOfTerminals = c.noOfTerminals;
                        branchRow.companyId= c.companyId
                        branchRow.manager = c.manager;
                        branchRow.terminals = c.terminals;
                        $scope.branchRows.push(branchRow);
                    });

                })
        }
        $scope.$on('$routeChangeSuccess',function(){  $scope.loadBranches(); });

        $scope.branch={
            "branchId":"",
            branchName:"",
            branchCode:"",
            dateCreated:"",
            maxCash:"",
            minStake:"",
            branchLocation:"",
            //noOfTerminals:"",
            //managerId:"",
            //terminals:"",
            companyId:"",
            personId:""
        };
        $scope.onSave=function(){
            var branch={};
            branch=$scope.branch;
            branch.branchId=0;
            branch.companyId=$scope.companyId;
            betData.post('Branch',branch,$scope).then(function(){
                $scope.showCreate=false;
                $scope.message=branch.branchName+" "+ "saved successfully, you can now add terminals and employees and shifts";
                $scope.loadBranches();
                //$scope.company.companyName='';
                //$scope.company.companyLocation='';
                $scope.branch=null;
                $scope.reverse=true;
            })
        };
        $scope.onDeleteBranch=function(branchId,branchIndex){

            betData.delete('Branch/DeleteBranch?id='+branchId,$scope).then(function(result){

                var _branchIndex=$scope.branchRows.indexOf(branchIndex);
                $scope.branchRows.splice(_branchIndex,1);
                $scope.message=result.branchName+ " deleted successfully along with its branches branches";


            })
        };

        $scope.promptToCreateBranch=function(){

            $scope.showCreate=!$scope.showCreate;
        };
    }]);

