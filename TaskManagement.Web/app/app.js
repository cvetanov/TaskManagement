var app = angular.module('TaskManagementApp', ['ngRoute', 'angularModalService', 'LocalStorageModule', 'angular-loading-bar', 'toastr']);

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

    $routeProvider.when("/tasks/:id", {
        controller: "taskController",
        templateUrl: "/app/views/task.html"
    });

    $routeProvider.when("/tasks/edit/:id", {
        controller: "tasksEditController",
        templateUrl: "/app/views/taskEdit.html",
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

app.value('baseUrl', 'http://localhost:47860/');
app.value('signalRconnection', $.hubConnection('http://localhost:47860/'));

// if user is already authenticated, connect him to server via web socket using signalR
app.run(['$rootScope', 'toastr', 'authService', 'signalRconnection', 'friendsService', 
    function($rootScope, toastr, authService, signalRconnection, friendsService) {

    var notificationHub = signalRconnection.createHubProxy('notificationHub');
    notificationHub.on('notifyAccept', function(message) {
        toastr.info(message, "You have a new friend!");
        $rootScope.$broadcast('refreshFriends');
    });
    notificationHub.on('notifyNewFriendRequest', function(message) {
        toastr.info("You have a new friend request");
        friendsService.getFriendRequests().then(function success(response) {
            if (response.data.length > 0) {
                $rootScope.friendRequestsNotification = response.data.length;
                $rootScope.$broadcast('refreshFriendRequests');
            }
            else {
                $rootScope.friendRequestsNotification = '';
            }
        });
    });
    notificationHub.on('notifyFriendRequestRejected', function(message) {
        $rootScope.$broadcast('refreshFriends');
    });
    notificationHub.on('notifyFriendshipDeleted', function(message) {
        $rootScope.$broadcast('refreshFriends');
    });
    notificationHub.on('notifyRefreshTask', function(taskId) {
        var message = {data: { taskId: taskId}};
        $rootScope.$broadcast('refreshTask', message);
    });
    notificationHub.on('notifyRefreshTasks', function(taskId) {
        var message = {data: { taskId: taskId}};
        $rootScope.$broadcast('refreshTasks', message);
    });
    

    if (authService.authentication.isAuth) { 
        signalRconnection.start()
            .done(function(){ 
                console.log('SignalR web socket now connected, connection ID = ' + signalRconnection.id); 
                notificationHub.invoke('subscribe', signalRconnection.id, authService.authentication.userName);
            })
            .fail(function(){ console.log('Could not connect'); });
    }
}]);

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

app.directive('taskDirective', function() {
  return {
    templateUrl: '/app/directives/task.html'
  };
});

app.directive('taskEditDirective', function () {
    return {
      templateUrl: '/app/directives/taskEdit.html',
      restrict: 'E',
      transclude: true,
      replace:true,
      scope:true,
    };
});

app.config(function(toastrConfig) {
  angular.extend(toastrConfig, {
    autoDismiss: false,
    containerId: 'toast-container',
    maxOpened: 0,    
    newestOnTop: true,
    positionClass: 'toast-bottom-right',
    preventDuplicates: false,
    preventOpenDuplicates: false,
    target: 'body'
  });
});

(window.angular);