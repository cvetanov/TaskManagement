'use strict';
app.factory('authService', ['$http', '$rootScope', '$q', 'localStorageService', 'toastr', 'signalRconnection', 'baseUrl', 'friendsService',
    function ($http, $rootScope, $q, localStorageService, toastr, signalRconnection, baseUrl, friendsService) {

    var serviceBase = baseUrl;

    var authServiceFactory = {};

    var friendsHub = signalRconnection.createHubProxy('friendsHub');

    var _authentication = {
        isAuth: false,
        userName: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName });

            _authentication.isAuth = true;
            _authentication.userName = loginData.userName;

            //start signalR web socket connection to server
            signalRconnection.start()
                .done(function () {
                    console.log('Logged in. SignalR web socket now connected, connection ID = ' + signalRconnection.id);

                    friendsHub.invoke('subscribe', signalRconnection.id, _authentication.userName);
                })
                .fail(function () { console.log('Logged in. Could not connect web socket'); });


            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorageService.remove('authorizationData');
        
        signalRconnection.start().done(function() {

            friendsHub.invoke('unsubscribe', _authentication.userName);
            signalRconnection.stop();
            console.log('SignalR web socket disconnected.');

            _authentication.isAuth = false;
            _authentication.userName = "";
        });        
    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
        }

    }

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;

    return authServiceFactory;
}]);