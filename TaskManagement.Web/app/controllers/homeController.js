'use strict';
app.controller('homeController', ['$scope', 'authService', 'friendsService', 'weatherService',
     function ($scope, authService, friendsService, weatherService) {
	$scope.loggedIn = authService.authentication;

    $scope.message = "Weather info";

	$scope.$root.friendRequestsNotification = '';
    var getFriendRequests = function() {
        friendsService.getFriendRequests().then(function success(response) {
            console.log(response);
            if (response.data.length > 0) {
                $scope.$root.friendRequestsNotification = response.data.length;
            }
            else {
                $scope.$root.friendRequestsNotification = '';
            }
        });
    }

    var getWeatherInfo = function() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(send);
        } else {
            console.log("Geolocation is not supported by this browser.");
            $scope.error = "Geolocation is not supported by this browser.";
        }
    }

    var send = function(response) {
        weatherService.getCity(function(city, country) {
            weatherService.getWeatherForecast(city, country).then(function (weatherResponse) {
                $scope.message = 'Weather mostly ' + weatherResponse.data.info;
                var ctx = document.getElementById("line").getContext("2d");
                var myChart = new Chart(ctx, {
                    type: 'line',
                    data: weatherResponse.data.data
                });
            });
        }, response.coords.latitude, response.coords.longitude);
    }

    getWeatherInfo();

    if (authService.authentication.isAuth) {
        getFriendRequests();
    }

}]);