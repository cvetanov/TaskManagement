'use strict';
app.factory('tasksService', ['$http', function ($http) {

    var serviceBase = 'http://localhost:47860/';
    var tasksServiceFactory = {};

    var _getTasks = function () {

        return $http.get(serviceBase + 'api/tasks').then(function (results) {
            return results;
        });
    };

    var _createTask = function (task) {
        console.log(task);
        return $http.post(serviceBase + 'api/tasks', task).then(function (result) {
            return result;
        });
    };

    tasksServiceFactory.getTasks = _getTasks;
    tasksServiceFactory.createTask = _createTask;

    return tasksServiceFactory;

}]);