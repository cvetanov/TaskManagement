'use strict';
app.controller('friendsController', ['$scope', 'friendsService', 'toastr',
        function ($scope, friendsService, toastr) {

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

    $scope.test = function() {
    	console.log('todo: redirect to user profile');
    }
}]);