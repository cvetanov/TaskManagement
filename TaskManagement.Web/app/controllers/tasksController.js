'use strict';
app.controller('tasksController', ['$scope', 'tasksService', 'authService', 'friendsService', '$location', 'toastr',
        function ($scope, tasksService, authService, friendsService, $location, toastr) {
    $scope.tasks = {};

    var initNewTask = function() {
        $scope.showNewTask = false;
        $scope.addFriends = false;
        $scope.newTask = {
            Name: '',
            Description: '',
            UsersInTask: []
        }
    }
    initNewTask();

    $scope.addFriendsToTask = function() {
        $scope.addFriends = true;
        friendsService.getFriends().then(function success(response) {
            $scope.friends = response.data;
        });
    }

    $scope.removeFriendFromTask = function(friend) {
        $scope.friends.push(friend);

        var index = $scope.newTask.UsersInTask.indexOf(friend);
        if (index > -1) {
            $scope.newTask.UsersInTask.splice(index, 1);
        }
    }

    $scope.addFriendInTask = function(friend) {
        $scope.newTask.UsersInTask.push(friend);

        var index = $scope.friends.indexOf(friend);
        if (index > -1) {
            $scope.friends.splice(index, 1);
        }
    }

    $scope.addNewTask = function() {
        $scope.showNewTask = true;
    }

    var resetTableMessage = function() {
        $scope.showTable = true;
        $scope.showTableMessage = "";
    }
    resetTableMessage();

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