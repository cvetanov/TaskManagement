'use strict';
app.factory('usersService', ['$http', 'baseUrl', function ($http, baseUrl) {

	var usersUrl = baseUrl + 'api/userProfiles';
	var imagesUrl = baseUrl + 'api/images';
	var usersServiceFactory = {};

	var _getUser = function(userId) {
		return $http.get(usersUrl + '/' + userId).then(function (result) {
			return result;
		});
	}

	var _getPhoto = function(imageName) {
		return $http.get(imagesUrl + '/getPhoto?imageName=' + imageName).then(function (result) {
			return result;
		});
	}

	var _uploadPhoto = function(fd) {
		return $http.post(imagesUrl + '/uploadPhoto', fd, {
                    headers: {'Content-Type': undefined },
                    transformRequest: angular.identity
                });
	}

	usersServiceFactory.getUser = _getUser;
	usersServiceFactory.uploadPhoto = _uploadPhoto;
	usersServiceFactory.getPhoto = _getPhoto;

	return usersServiceFactory;
}]);