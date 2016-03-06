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
				$scope.$root.friendRequestsNotification = '';
			}
			$scope.friendRequestsChunks = friendsService.chunkify(response.data, (response.data.length - 1) / 4 + 1);
		}, function failure(response) {

		});
	}

	var refresh = function() {
		getUsersNonFriends();
		getFriendRequests();
	}
	refresh();

	$scope.sendFriendRequest = function(id) {
		var data = {
			Id: id
		};
		friendsService.sendFriendRequest(data).then(function success(response) {
			refresh();
		}, function failure(response) {

		});
	};

	$scope.acceptFriendRequest = function(id) {
		var data = {
			Id: id
		};
		friendsService.acceptFriendRequest(data).then(function success(response) {
			toastr.success("You and " + response.data.Username + " are now friends!", "Congratulations");
			refresh();
		}, function failure(response) {

		});
	}

	$scope.rejectFriendRequest = function(id) {
		var data = {
			Id: id
		};
		friendsService.rejectFriendRequest(data).then(function success(response) {
			toastr.warning("You rejected " + response.data.Username + "'s friend request.");
			refresh();
		}, function failure(response) {

		})
	}


}]);