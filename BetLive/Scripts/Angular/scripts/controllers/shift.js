'use strict';

/**
 * @ngdoc function
 * @name bettingApp.controller:ShiftCtrl
 * @description
 * # ShiftCtrl
 * Controller of the bettingApp
 */

bettingApp.controller('ShiftCtrl',['$scope','dataService','$routeParams','$location' ,function ($scope,betData,$routeParams,$location) {
    var url="shift";
    $scope.branchId=$routeParams.branchId;
    $scope.dateFrom=$routeParams.dateFrom;
    $scope.dateTo=$routeParams.dateTo;
    $scope.message='';
    $scope.showCreate=false;
    $scope.shiftRows=null;
    $scope.sortField='shiftId';
    $scope.reverse=true;
   $scope.cashiers=[];
   $scope.stopShiftMessage=null;
   // $scope.cashiers=[{personId:1,firstName:'James'},{personId:2,firstName:'Luswaata'},{personId:3,firstName:'Nanyonga'},
       // {personId:4,firstName:'Nanyondo'},{personId:5,firstName:'Mark'},{personId:6,firstName:'Philip'}];
    $scope.cashier=function(){
        this.personId=null,
        this.name=null
    };

    $scope.shiftRow=function(id){
            this.shiftId=0,
            this.terminalId=id;
            this.terminalName=null;
            this.startTime=null,
            this.endTime=null,
            this.assignedBy=null,
            this.isClosed=null,
            this.startCash=null,
            this.cashIn=null,
            this.cashOut=null,
            this.Balance=null;
            this.personId=null
            this.Input=null;
            this.branchId=null;
            this.netCash=null;
            this.cashier=null;
            this.shiftToSave=[]
    };


    $scope.loadCashiers=function(){
        betData.get("Employee/GetPerson?id="+ $scope.branchId,$scope)
            .then(function(results,status,headers,config){
                var myArr=[];
               // $scope.cashiers=new Array();
                angular.forEach(results,function(p,i){
                    var cashier=new $scope.cashier();
                    cashier.personId= p.personId;
                    cashier.name= p.name;
                    $scope.cashiers.push(cashier);
                    myArr.push(cashier);
                });
                return  myArr;
            });
    };
    $scope.loadShifts=function(){
        betData.get("Shift/GetShiftTerminalWithCashier?id="+$scope.branchId+"&dateFrom="+$scope.dateFrom+"&dateTo="+$scope.dateTo , $scope).then(
            function(results,status,headers,config){
                $scope.loadCashiers();
                //$scope.shiftTerminals=new Array();
                //$scope.shiftTerminals= $scope.loadCashiers();
                $scope.shiftRows=new Array();


                angular.forEach(results,function(c, i) {
                   // var cas="Steve";//$scope.getCashierByPersonId(parseInt(c.personId));
                    //alert(cas);
                    var shiftRow = new $scope.shiftRow(c.terminal.terminalId);
                    var shiftToSave=new $scope.shiftToSave(c.terminal.terminalId)
                    shiftRow.terminalId = c.terminal.terminalId;
                    shiftRow.terminalName= c.terminal.terminalName;
                    shiftRow.startTime = c.startTime!=null?c.startTime:null;
                    shiftRow.endTime = c.endTime!=null?c.endTime:null;
                    shiftRow.assignedBy = c.assignedBy;
                    shiftRow.isClosed = c.isClosed;
                    shiftRow.startCash = c.startCash;
                    shiftRow.cashIn= c.cashIn;
                    shiftRow.shiftId= c.shiftId;
                    shiftRow.cashOut = c.cashOut;
                    shiftRow.personId = c.personId;
                    shiftRow.balance=c.balance;
                    shiftRow.netCash=c.netCash;
                    shiftRow.cashier= c.cashier!=null?c.cashier.firstName:null;//parseInt(c.personId)<=$scope.cashiers.length? $scope.cashiers[parseInt(c.personId)-1].firstName:null;
                   //alert( $scope.getCashierByPersonId(parseInt(c.personId)));
                    shiftRow.shiftToSave=shiftToSave;
                    $scope.shiftRows.push(shiftRow);
                });


            });
    };


    $scope.$on('$routeChangeSuccess',function(){  $scope.loadShifts(); });

    $scope.shiftToSave=function(terminalId) {
       this.shiftId= 0,
       this.startTime= "",
      this.endTime= "",
    this.assignedBy= 0,
      this.isClosed = null,
      this.startCash= 0,
        this.cashIn= 0,
       this.cashOut= 0,
       this.netCash= 0,
       this.balance= 0,
      this.personId= null,
    this.terminalId= terminalId,
       this.cashier= null,
      this.terminal= null
};
    $scope.onSave=function(terminalRowIndex){
        var shift={};
        var _terminalIndex=$scope.shiftRows.indexOf(terminalRowIndex);
        shift=$scope.shiftRows[_terminalIndex];
        var shiftName=shift.terminalName;
        shift=shift.shiftToSave;
        shift.isClosed=false;
        //shift.shiftId=0;
        //shift.companyId=$scope.companyId;
        betData.post('Shift/PostShift', shift, $scope).then(function() {
            $scope.showCreate = false;
            $scope.message = shiftName + " " + "saved successfully you can start managing the shift";
            $scope.loadShifts();

            $scope.shift = null;
        });
    };

    $scope.onCloseShift = function (shiftId, terminalRowIndex) {
        var shift = {};
        var _terminalIndex = $scope.shiftRows.indexOf(terminalRowIndex);
        shift = $scope.shiftRows[_terminalIndex];
        var shiftName = shift.terminalName;
        betData.post("Shift/stopShift?id=" + shiftId).then(function (response) {
            $scope.stopShiftMessage = true;
            $scope.message = shiftName + " " + "Stoped successfully ";

        });
    };
    $scope.onDeleteBranch=function(shiftId,shiftIndex) {

        betData.delete('Branch/DeleteBranch?id=' + shiftId, $scope).then(function(result) {

            var _shiftIndex = $scope.shiftRows.indexOf(shiftIndex);
            $scope.shiftRows.splice(_shiftIndex, 1);
            $scope.message = result.branchName + " deleted successfully along with its branches branches";


        });
    };

    $scope.promptToCreateBranch=function(){

        $scope.showCreate=!$scope.showCreate;
    };
}]);
