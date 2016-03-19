'use strict';
app.controller('tasksController', ['$scope', 'tasksService', 'authService', 'friendsService', '$location', 'toastr', 'ModalService',
        function ($scope, tasksService, authService, friendsService, $location, toastr, ModalService) {
    $scope.myTasksChunks = [];
    $scope.otherTasksChunks = [];
    $scope.tasksPerRow = 3;
    $scope.noTasks = true;
    $scope.username = authService.authentication.userName;

    var resetTasks = function() {
        $scope.noTasks = false;
        $scope.noTasksMessage = "";
    }

    var refreshTasks = function() {
        tasksService.getTasks().then(function success(response) {
            $scope.myTasksChunks = tasksService.chunkify(response.data.myTasks, (response.data.myTasks.length - 1) / $scope.tasksPerRow);
            $scope.otherTasksChunks = tasksService.chunkify(response.data.otherTasks, (response.data.otherTasks.length - 1) / $scope.tasksPerRow);
            if (response.data.myTasks.length == 0 && response.data.otherTasks.length == 0) {
                $scope.noTasks = true;
                $scope.noTasksMessage = "You have no tasks. Add a new one!";
            }
            else {
                resetTasks();
            }
        }, function failed(response) {
            
        });
    }
    refreshTasks();
    $scope.$on('refreshTasks', function() {
        refreshTasks();
    });

    $scope.$on('refreshFriends', function() {
        refreshTasks();
    })

    $scope.createTask = function() {
        ModalService.showModal({
            templateUrl: '/app/views/taskEdit.html',
            controller: "tasksEditController",
            inputs: {
                taskId : 0
            }
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                refreshTasks();
            });
        });
    }

    $scope.editTask = function(id) {
        ModalService.showModal({
            templateUrl: '/app/views/taskEdit.html',
            controller: "tasksEditController",
            inputs: {
                taskId : id
            }
        }).then(function(modal) {
            modal.element.modal();
            modal.close.then(function(result) {
                refreshTasks();
            });
        });
    };

    $scope.viewTask = function(id) {
        $location.path("/tasks/" + id);
    };

    $scope.deleteTask = function(id) {
        tasksService.deleteTask(id).then(function success(response) {
            toastr.success("Task deleted.", "Success!");
            refreshTasks();
        }, function failure(response) {
            toastr.error("Failed to delete task.", "Error!");
            $location.path('/tasks');
        })
    };

}]);