'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:TerminalCtrl
 * @description
 * # TerminalCtrl
 * Controller of the bettingApp
 */








bettingApp.controller('TerminalCtrl',['$scope','dataService','$routeParams','$location' ,function ($scope,betData,$routeParams,$location) {
        var url="Terminal";
        $scope.branchId=$routeParams.branchId;
    $scope.dateFrom=null;
    $scope.dateTo=null;
        $scope.message='';
        $scope.showCreate=false;
        $scope.terminalRows=null;
        $scope.sortField='terminalId';
        $scope.reverse=false;
        $scope.terminalRow=function(id){
                this.terminalId=id,
                this.branchId=null;
                this.terminalName=null,
                this.ipAddress=null,
                this.isActive=null,
                this.dateCreated=null,
                this.shifts=null,
                this.employees=[]

        };

        $scope.loadTerminals=function(){
            betData.get("Terminal/GetTerminalsByBranchId?id="+$scope.branchId,$scope).then(
                function(results,status,headers,config){
                    $scope.companies=results;
                    $scope.terminalRows=new Array();
                    angular.forEach(results,function(c, i) {
                        var terminalRow = new $scope.terminalRow(c.terminalId);
                        terminalRow.terminalName = c.terminalName;
                        terminalRow.ipAddress = c.ipAddress;
                        terminalRow.branchId = c.branchId;
                        terminalRow.isActive = c.isActive;
                        terminalRow.dateCreated = c.dateCreated;
                        terminalRow.employees = c.branch.employees;
                        terminalRow.shifts = c.shifts;
                        $scope.terminalRows.push(terminalRow);
                    });

                })
        }
        $scope.$on('$routeChangeSuccess',function(){  $scope.loadTerminals(); });

        $scope.terminal={
            "branchId":"",
            terminalName:"",
            ipAddress:"",
            dateCreated:"",
            terminalId:""

        };
        $scope.onSave=function(){
            var terminal={};
            terminal=$scope.terminal;
            terminal.terminalId=0;
            terminal.branchId=$scope.branchId;
            betData.post('Terminal',terminal,$scope).then(function(){
                $scope.showCreate=false;
                $scope.message=terminal.terminalName+" "+ "saved successfully, you can now add terminals and employees and shifts";
                $scope.loadTerminals();
                //$scope.company.companyName='';
                //$scope.company.companyLocation='';
                $scope.terminal=null;
                $scope.reverse=true;
            })
        };
        $scope.onDeleteTerminal=function(terminalId,terminalIndex){

            betData.delete('Terminal/DeleteTerminal?id='+terminalId,$scope).then(function(result){

                var _terminalIndex=$scope.terminalRows.indexOf(terminalIndex);
                $scope.terminalRows.splice(_terminalIndex,1);
                $scope.message=result.terminalName+ " deleted successfully along with its shifts";


            })
        };

        $scope.promptToCreateTerminal=function(){

            $scope.showCreate=!$scope.showCreate;
        };
    }]);

