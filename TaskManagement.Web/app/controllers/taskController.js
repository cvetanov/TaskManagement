'use strict';
app.controller('taskController', ['$scope', '$routeParams', 'tasksService', 'commentsService', 'authService', '$location', 'toastr',
        function ($scope, $routeParams, tasksService, commentsService, authService, $location, toastr) {
            $scope.task = {};
            $scope.newComment = '';
            $scope.username = authService.authentication.userName;

            $scope.addNewComment = function () {
                commentsService.addComment($scope.newComment, $routeParams.id).then(function () {
                    $scope.newComment = '';
                    refreshTask();
                });
            }

            $scope.finishTask = function() {
                tasksService.finishTask($routeParams.id).then(function () {
                    refreshTask();
                });
            }

            var refreshTask = function () {
                tasksService.getTask($routeParams.id).then(function success(response) {
                    console.log(response);
                    $scope.task = response.data;
                }, function failed(response) {

                });
            }
            refreshTask();

        }]);