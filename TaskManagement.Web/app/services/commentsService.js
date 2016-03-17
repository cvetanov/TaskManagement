'use strict';
app.factory('commentsService', ['$http', 'baseUrl', function ($http, baseUrl) {

	var commentsUrl = baseUrl + 'api/comments';
	var commentsServiceFactory = {};

	var _addComment = function(comment, taskId) {
		var viewModel = {
			text: comment,
			taskId: taskId
		}
		return $http.post(commentsUrl, viewModel).then(function (result) {
			return result;
		})
	}

	commentsServiceFactory.addComment = _addComment;

	return commentsServiceFactory;
}]);