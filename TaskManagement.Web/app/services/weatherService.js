'use strict';
app.factory('weatherService', ['$http', 'baseUrl',
	function ($http, baseUrl) {

	var serviceUrl = baseUrl + 'api/Weather';
	var weatherServiceFactory = {};

	var _getCity = function(callback, lat, lng) {
		var geocoder = new google.maps.Geocoder;
		var latlng = {
			'lat': lat,
			'lng': lng
		};
		geocoder.geocode({'location': latlng}, function(results, status) {
			var cityName = 'default';
			var countryCode = 'def';
			if (status == 'OK') {
				var address = results[0];
				var address_components = address.address_components;
				for (var i = 0; i < address_components.length; ++i) {
					var component = address_components[i];
					for (var t = 0; t < component.types.length; ++t) {
						var type = component.types[t];
						if (type == 'locality') {
							cityName = component.long_name;
						}
						else if (type == 'country') {
							countryCode = component.short_name;
						}
					}
				}
			}
			callback(cityName, countryCode);
		});		
	}

	var _getWeatherForecast = function(cityName, countryCode) {
		return $http.get(serviceUrl + '/Get?city=' + cityName + '&countryCode=' + countryCode).then(function (result) {
			return result;
		});
	}

	var _getWeather = function(cityName, countryCode) {
		return $http.get(serviceUrl + '/GetCurrent?city=' + cityName + '&countryCode=' + countryCode).then(function (result) {
			return result;
		});
	}

	weatherServiceFactory.getWeather = _getWeather;
	weatherServiceFactory.getWeatherForecast = _getWeatherForecast;
	weatherServiceFactory.getCity = _getCity;

	return weatherServiceFactory;
}]);