'use strict';
app.controller('tasksEditController', ['$rootScope', '$scope', '$location', 'tasksService', 'friendsService', 'authService', 'taskId', '$element', 'toastr',
	function ($rootScope, $scope, $location, tasksService, friendsService, authService, taskId, $element, toastr) {

	$scope.title = 'New task';
	if (taskId != 0) {
		$scope.title = 'Edit task';
	}
	$scope.task = {};
	$scope.friendsInTask = [];
	$scope.friendsNotInTask = [];

	var refreshTask = function() {
		tasksService.getTask(taskId).then(function success(response) {
			$scope.task = response.data;
		});
		getFriends();
	};
	var initNewTask = function() {
        $scope.task = {
            Name: '',
            Description: ''
        };
        getFriends();
    }
    var getFriends = function() {
    	friendsService.getFriendsNotInTask(taskId).then(function success(response) {
            $scope.friendsNotInTask = response.data;
        });
		friendsService.getFriendsInTask(taskId).then(function success(response) {
            $scope.friendsInTask = response.data;
        });
    }
    if (taskId != 0) {
    	refreshTask();
    }
	else {
		initNewTask();
	}

	$scope.close = function() {
		close({}, 500);
	};

	$scope.save = function() {
		//save task
		if (taskId != 0) {
			updateTask().then(function() {
                $element.modal('hide');
                close({}, 500);
            });
		}
		else {
			createTask().then(function() {
				$element.modal('hide');
				close({}, 500);
			});
		}
	};

	$scope.cancel = function() {
		$element.modal('hide');
		close({}, 500);
	};

    $scope.removeFriendFromTask = function(friend) {
        $scope.friendsNotInTask.push(friend);

        var index = $scope.friendsInTask.indexOf(friend);
        if (index > -1) {
            $scope.friendsInTask.splice(index, 1);
        }
    }

    $scope.addFriendInTask = function(friend) {
        $scope.friendsInTask.push(friend);

        var index = $scope.friendsNotInTask.indexOf(friend);
        if (index > -1) {
            $scope.friendsNotInTask.splice(index, 1);
        }
    }


    var createTask = function () {
    	$scope.task.UsersInTask = $scope.friendsInTask;
        return tasksService.createTask($scope.task).then(function success(response) {
            toastr.success("Task created.", "Success!");
        }, function failed(response) {
            toastr.error("Something went wrong.", "Error!");
        });
    };

    var updateTask = function () {
        var taskViewModel = {
            id: $scope.task.Id,
            name: $scope.task.Name,
            description: $scope.task.Description,
            usersInTask: $scope.friendsInTask
        };

        return tasksService.updateTask(taskViewModel).then(function success(response) {
            toastr.success("Task updated.", "Success!");            
        }, function failed(response) {
            toastr.error("Something went wrong.", "Error!");
            $location.path('/tasks');
        });
    };
}]);