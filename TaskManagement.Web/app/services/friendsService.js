'use strict';
app.factory('friendsService', ['$http', 'baseUrl', function ($http, baseUrl) {

	var friendsUrl = baseUrl + 'api/friends';
	var friendRequestsUrl = baseUrl + 'api/friendrequests'
	var friendsServiceFactory = {};

	var _getFriends = function() {
		return $http.get(friendsUrl + '/Get').then(function (result) {
			return result;
		})
	};

	var _getFriendsNotInTask = function(taskId) {
		return $http.get(friendsUrl + '/getFriendsNotInTask/' + taskId).then(function (result) {
			return result;
		});
	};

	var _getFriendsInTask = function(taskId) {
		return $http.get(friendsUrl + '/getFriendsInTask/' + taskId).then(function (result) {
			return result;
		});
	}

	var _getUsersNonFriends = function() {
		return $http.get(friendsUrl + '/GetNonFriends').then(function (result) {
			return result;
		});
	};

	var _removeFriend = function(data) {
		return $http.delete(friendsUrl + '/delete', {params: {friendId : data.friendId}}).then(function (result) {
			return result;
		})
	};

	var _getFriendRequests = function() {
		return $http.get(friendRequestsUrl + '/Get').then(function (result) {
			return result;
		});
	};

	var _acceptFriendRequest = function(data) {
		return $http.put(friendRequestsUrl + '/accept', data).then(function (result) {
			return result;
		});
	};

	var _rejectFriendRequest = function(data) {
		return $http.put(friendRequestsUrl + '/reject', data).then(function (result) {
			return result;
		});
	};

	var _sendFriendRequest = function(data) {
		return $http.post((friendRequestsUrl + '/create'), data).then(function (result) {
			return result;
		});
	};

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
	friendsServiceFactory.getFriendsInTask = _getFriendsInTask;
	friendsServiceFactory.getFriendsNotInTask = _getFriendsNotInTask;
	friendsServiceFactory.getUsersNonFriends = _getUsersNonFriends;
	friendsServiceFactory.getFriendRequests = _getFriendRequests;
	friendsServiceFactory.acceptFriendRequest = _acceptFriendRequest;
	friendsServiceFactory.rejectFriendRequest = _rejectFriendRequest;
	friendsServiceFactory.sendFriendRequest = _sendFriendRequest;
	friendsServiceFactory.removeFriend = _removeFriend;
	friendsServiceFactory.chunkify = _chunkify;

	return friendsServiceFactory;
}]);