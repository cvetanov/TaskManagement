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

    var _finishTask = function(id) {
        var data = {
            Id: id,
            Status: true
        };
        return $http.put(serviceBase, data).then(function (result) {
            return result;
        });
    }

    var _deleteTask = function(id) {
        return $http.delete(serviceBase + '/' + id).then(function (result) {
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

    tasksServiceFactory.getTask = _getTask;
    tasksServiceFactory.getTasks = _getTasks;
    tasksServiceFactory.createTask = _createTask;
    tasksServiceFactory.updateTask = _updateTask;
    tasksServiceFactory.deleteTask = _deleteTask;
    tasksServiceFactory.finishTask = _finishTask;
    tasksServiceFactory.chunkify = _chunkify;

    return tasksServiceFactory;

}]);