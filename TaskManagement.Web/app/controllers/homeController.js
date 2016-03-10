'use strict';
app.controller('homeController', ['$scope', 'authService', 'friendsService', function ($scope, authService, friendsService) {
	$scope.loggedIn = authService.authentication;

	$scope.$root.friendRequestsNotification = '';
    var getFriendRequests = function() {
        friendsService.getFriendRequests().then(function success(response) {
            console.log(response);
            if (response.data.length > 0) {
                $scope.$root.friendRequestsNotification = response.data.length;
            }
            else {
                $scope.$root.friendRequestsNotification = '';
            }
        });
    }

    if (authService.authentication.isAuth) {
        getFriendRequests();
    }
    
}]);