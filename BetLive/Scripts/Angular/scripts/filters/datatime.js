'use strict';

/**
 * @ngdoc filter
 * @name bettingApp.filter:datatime
 * @function
 * @description
 * # datatime
 * Filter in the bettingApp.
 */


bettingApp.filter("datetime", [function (utils) {
    return function (input) {
        var date = new Date(input);
        var minutes = date.getMinutes(); if(minutes < 10) minutes = "0" + minutes;
        var hours = date.getHours(); if(hours < 10) hours = "0" + hours;
        var month = date.getMonth(); if(month < 10) month = "0" + month;
        var day = date.getDate(); if(day < 10) day = "0" + day;
        return date.getFullYear() + "-" + month + "-" + day + " " + hours + ":" + day;
    };
}]);

bettingApp.filter("datetime2", [function (utils) {
    return function (input) {
        var dt = new Date(input);
       // var pattern = /Date\(([^)]+)\)/;
        //var results = pattern.exec(value);
       // var dt = new Date(parseFloat(results[1]));
        var hours = (dt.getHours() < 10) ? '0' + dt.getHours() : dt.getHours();
        var mins = (dt.getMinutes() < 10) ? '0' + dt.getMinutes() : dt.getMinutes();
        return (hours + ':' + mins);
    }
}]);