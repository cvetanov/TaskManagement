var app = angular.module('TaskManagementApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'toastr']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/tasks", {
        controller: "tasksController",
        templateUrl: "/app/views/tasks.html"
    });

    $routeProvider.when("/tasks/edit/:id", {
        controller: "tasksEditController",
        templateUrl: "/app/views/tasksEdit.html",
        routeParams: "id"
    });

    $routeProvider.when("/friends", {
        controller: "friendsController",
        templateUrl: "/app/views/friends.html",
    });

    $routeProvider.when("/newfriends", {
        controller: "newFriendsController",
        templateUrl: "/app/views/newFriends.html",
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.directive('friendDirective', function() {
  return {
    templateUrl: '/app/directives/friend.html'
  };
});

app.directive('userDirective', function() {
  return {
    templateUrl: '/app/directives/user.html'
  };
});

app.directive('friendRequestDirective', function() {
  return {
    templateUrl: '/app/directives/friendRequest.html'
  };
});


(window.angular);