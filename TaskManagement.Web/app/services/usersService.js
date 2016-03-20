'use strict';
app.factory('usersService', ['$http', 'baseUrl', function ($http, baseUrl) {

	var usersUrl = baseUrl + 'api/userProfiles';
	var usersServiceFactory = {};

	var _getUser = function(userId) {
		return $http.get(usersUrl + '/' + userId).then(function (result) {
			return result;
		});
	}

	usersServiceFactory.getUser = _getUser;

	return usersServiceFactory;
}]);