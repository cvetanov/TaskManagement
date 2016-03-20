'use strict';
app.controller('usersController', ['$scope', '$routeParams', 'usersService', 'friendsService', '$location', 'toastr',
        function ($scope, $routeParams, usersService, friendsService, $location, toastr) {
            $scope.user = {};
            var getUserData = function() {
                usersService.getUser($routeParams.id).then(function (response) {
                    $scope.user = response.data;
                });
                friendsService.getFriendRequests().then(function success(response) {
                    if (response.data.length > 0) {
                        $scope.$root.friendRequestsNotification = response.data.length;
                    }
                    else {
                        $scope.$root.friendRequestsNotification = '';
                    }
                });
            }

            $scope.$on('refreshFriends', function() {
                getUserData();
            })
            $scope.$on('refreshFriendRequests', function() {
                getUserData();
            })

            $scope.addFriend = function() {
                var data = {
                    Id: $routeParams.id,
                    Username: $scope.user.Username
                };
                friendsService.sendFriendRequest(data).then(function success(response) {
                    getUserData();
                }, function failure(response) {

                });
            }

            $scope.acceptFriendRequest = function() {
                var data = {
                    Id: $routeParams.id
                };
                friendsService.acceptFriendRequest(data).then(function success(response) {
                    var username = response.data.Username;
                    toastr.success("You and " + username + " are now friends!", "Congratulations"); 
                    getUserData();
                }, function failure(response) {

                });
            }

            $scope.rejectFriendRequest = function() {
                var data = {
                    Id: $routeParams.id
                };
                friendsService.rejectFriendRequest(data).then(function success(response) {
                    getUserData();
                }, function failure(response) {

                })
            }

            getUserData();

        }]);