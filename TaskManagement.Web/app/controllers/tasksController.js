'use strict';
app.controller('tasksController', ['$scope', 'tasksService', 'authService', function ($scope, tasksService, authService) {

    $scope.tasks = {};
    

    var initNewTask = function() {
        $scope.newTask = {
            Name: "",
            Description: "",
            PercentageDone: 0,
            CreatorUsername : authService.authentication.userName
        };    
    }
    initNewTask();

    var refreshTasks = function() {
        tasksService.getTasks().then(function success(response) {
            $scope.tasks = response.data;
            console.log(response.data);
        }, function failed(response) {
            console.log('error');
        });
    }
    refreshTasks();

    $scope.createTask = function () {
        tasksService.createTask($scope.newTask).then(function success(response) {
            refreshTasks();
            initNewTask();
        });
    };

}]);