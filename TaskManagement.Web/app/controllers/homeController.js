'use strict';
app.controller('homeController', ['$scope', 'authService', function ($scope, authService) {
	$scope.loggedIn = authService.authentication;
}]);