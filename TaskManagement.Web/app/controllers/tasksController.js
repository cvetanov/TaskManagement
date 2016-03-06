'use strict';
app.controller('tasksController', ['$scope', 'tasksService', 'authService', '$location', 'toastr',
        function ($scope, tasksService, authService, $location, toastr) {
    $scope.tasks = {};

    var resetTableMessage = function() {
        $scope.showTable = true;
        $scope.showTableMessage = "";
    }
    resetTableMessage();

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
            if (response.data.length == 0) {
                $scope.showTable = false;
                $scope.showTableMessage = "You have no tasks. Add a new one!";
            }
            else {
                resetTableMessage();
            }
        }, function failed(response) {
            
        });
    }
    refreshTasks();

    $scope.createTask = function () {
        tasksService.createTask($scope.newTask).then(function success(response) {
            toastr.success("Task created.", "Success!");
            refreshTasks();
            initNewTask();
        }, function failed(response) {
            toastr.error("Something went wrong.", "Error!");
        });
    };

    $scope.editTask = function(id) {
        $location.path("/tasks/edit/" + id);
    };

    $scope.deleteTask = function(id) {
        tasksService.deleteTask(id).then(function success(response) {
            toastr.success("Task deleted.", "Success!");
            refreshTasks();
        }, function failure(response) {
            toastr.error("Failed to delete task.", "Error!");
        })
    };

}]);