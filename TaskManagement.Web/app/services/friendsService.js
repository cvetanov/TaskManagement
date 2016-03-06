'use strict';
app.factory('friendsService', ['$http', function ($http) {

	var serviceBase = 'http://localhost:47860/api/users';
	var friendsServiceFactory = {};

	var _getFriends = function() {
		return $http.get(serviceBase + '/GetFriends').then(function (result) {
			return result;
		})
	}

	var _getUsersNonFriends = function() {
		return $http.get(serviceBase + '/GetNonFriends').then(function (result) {
			return result;
		});
	};

	var _getFriendRequests = function() {
		return $http.get(serviceBase + '/GetFriendRequests').then(function (result) {
			return result;
		});
	};

	var _acceptFriendRequest = function(data) {
		return $http.post(serviceBase + '/AcceptFriendRequest', data).then(function (result) {
			return result;
		});
	};

	var _rejectFriendRequest = function(data) {
		return $http.post(serviceBase + '/RejectFriendRequest', data).then(function (result) {
			return result;
		});
	}

	var _sendFriendRequest = function(data) {
		return $http.post((serviceBase + '/SendFriendRequest'), data).then(function (result) {
			return result;
		});
	}

	var _chunkify = function(a, n) {
	    if (n < 2)
	        return [a];

	    var len = a.length,
	            out = [],
	            i = 0,
	            size;

	    if (len % n === 0) {
	        size = Math.floor(len / n);
	        while (i < len) {
	            out.push(a.slice(i, i += size));
	        }
	    }
	    else {
	        n--;
	        size = Math.floor(len / n);
	        if (len % size === 0)
	            size--;
	        while (i < size * n) {
	            out.push(a.slice(i, i += size));
	        }
	        out.push(a.slice(size * n))

	    }
	    return out;
	}

	friendsServiceFactory.getFriends = _getFriends;
	friendsServiceFactory.getUsersNonFriends = _getUsersNonFriends;
	friendsServiceFactory.getFriendRequests = _getFriendRequests;
	friendsServiceFactory.acceptFriendRequest = _acceptFriendRequest;
	friendsServiceFactory.rejectFriendRequest = _rejectFriendRequest;
	friendsServiceFactory.sendFriendRequest = _sendFriendRequest;
	friendsServiceFactory.chunkify = _chunkify;

	return friendsServiceFactory;
}]);