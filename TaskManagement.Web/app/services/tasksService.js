'use strict';
app.factory('tasksService', ['$http', 'baseUrl', function ($http, baseUrl) {

    var serviceBase = baseUrl + 'api/tasks';
    var tasksServiceFactory = {};

    var _getTask = function(id) {
        return $http.get(serviceBase + '/' + id).then(function (result) {
            return result;
        });
    }

    var _getTasks = function () {
        return $http.get(serviceBase).then(function (results) {
            return results;
        });
    };

    var _createTask = function (task) {
        return $http.post(serviceBase, task).then(function (result) {
            return result;
        });
    };

    var _updateTask = function(task) {
        return $http.put(serviceBase, task).then(function (result) {
            return result;
        });
    };

    var _deleteTask = function(id) {
        return $http.delete(serviceBase + '/' + id).then(function (result) {
            return result;
        });
    }

    tasksServiceFactory.getTask = _getTask;
    tasksServiceFactory.getTasks = _getTasks;
    tasksServiceFactory.createTask = _createTask;
    tasksServiceFactory.updateTask = _updateTask;
    tasksServiceFactory.deleteTask = _deleteTask;

    return tasksServiceFactory;

}]);