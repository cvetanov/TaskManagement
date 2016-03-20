'use strict';
app.controller('friendsController', ['$scope', '$location', 'friendsService',
        function ($scope, $location, friendsService) {

    $scope.friendsChunks = {};
    $scope.showFriends = false;
    $scope.friendsPerRow = 4;

    var getFriends = function() {
    	friendsService.getFriends().then(function success(response) {
    		$scope.showFriends = response.data.length > 0;
    		$scope.friendsChunks = friendsService.chunkify(response.data, (response.data.length - 1) / $scope.friendsPerRow + 1);
    	});
    };
    getFriends();

    // listener for acceptance of friend requests
    $scope.$on('refreshFriends', function() {
        getFriends();
    })

    $scope.showUser = function(id) {
    	$location.path('/users/' + id);
    }

    $scope.removeFriend = function(friendId) {
        var data = {
            friendId: friendId
        };
        friendsService.removeFriend(data).then(function success(response) {
            getFriends();
        });
    }

    $scope.redirectToNewFriends = function() {
        $location.path('/newfriends');
    }

}]);