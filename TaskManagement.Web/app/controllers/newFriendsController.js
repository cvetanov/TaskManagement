'use strict';
app.controller('newFriendsController', ['$scope', 'friendsService', 'toastr',
        function ($scope, friendsService, toastr) {

    $scope.nonFriendsChunks = {};
    $scope.showNonFriends = false;
    $scope.friendRequestsChunks = {};
    $scope.showFriendRequests = false;


	var getUsersNonFriends = function() {
		friendsService.getUsersNonFriends().then(function success(response) {
			$scope.showNonFriends = response.data.length > 0;
			$scope.nonFriendsChunks = friendsService.chunkify(response.data, (response.data.length - 1) / 4 + 1);
		}, function failure(response) {

		});
	}

	var getFriendRequests = function() {
		friendsService.getFriendRequests().then(function success(response) {
			if (response.data.length > 0) {
				$scope.showFriendRequests = true;
				$scope.$root.friendRequestsNotification = response.data.length;
			}
			else {
				$scope.showFriendRequests = false;
				$scope.$root.friendRequestsNotification = '';
			}
			$scope.friendRequestsChunks = friendsService.chunkify(response.data, (response.data.length - 1) / 4 + 1);
		}, function failure(response) {

		});
	}

	var refreshData = function() {
		getUsersNonFriends();
		getFriendRequests();
	}
	refreshData();

	// listener for acceptance of friend requests
	$scope.$on('refreshFriends', function() {
		refreshData();
	})
	$scope.$on('refreshFriendRequests', function() {
		refreshData();
	})

	$scope.sendFriendRequest = function(id, username) {
		var data = {
			Id: id,
			Username: username
		};
		friendsService.sendFriendRequest(data).then(function success(response) {
			refreshData();
		}, function failure(response) {

		});
	};

	$scope.acceptFriendRequest = function(id) {
		var data = {
			Id: id
		};
		friendsService.acceptFriendRequest(data).then(function success(response) {
			var username = response.data.Username;
			toastr.success("You and " + username + " are now friends!", "Congratulations");	
			refreshData();
		}, function failure(response) {

		});
	}

	$scope.rejectFriendRequest = function(id) {
		var data = {
			Id: id
		};
		friendsService.rejectFriendRequest(data).then(function success(response) {
			refreshData();
		}, function failure(response) {

		})
	}


}]);