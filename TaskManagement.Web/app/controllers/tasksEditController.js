'use strict';
app.controller('tasksEditController', ['$scope', 'tasksService', 'authService', '$routeParams', 
	function ($scope, tasksService, authService, $routeParams) {

	$scope.task = {};

	var refreshTask = function() {
		tasksService.getTask($routeParams.id).then(function success(response) {
			$scope.task = response.data;
		}, function failure(response) {

		});
	};
	refreshTask();

	

	$scope.removeUserFromTask = function(userId) {
		for (var i = 0; i < $scope.task.UsersInTasks.length; ++i) {
			var userInTask = $scope.task.UsersInTasks[i];
			if (userInTask.UserId === userId) {
				$scope.task.UsersInTasks.splice(i, 1);
			}
		}
	}

	$scope.addUserInTask = function(userId) {
		$scope.task.UsersInTasks.push({id: userId});
	}

	$scope.updateTask = function() {
		tasksService.updateTask($scope.task).then(function success(response) {
			refreshTask();
		}, function failure(response) {

		});
	}

}]);