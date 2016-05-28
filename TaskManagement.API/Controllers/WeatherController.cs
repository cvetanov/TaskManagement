using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TaskManagement.Persistence;

namespace TaskManagement.API.Controllers
{
    [RoutePrefix("api/Weather")]
    public class WeatherController : ApiController
    {
        private UnitOfWork _uow;
        private string _apiurl = @"http://api.openweathermap.org/data/2.5/";
        private string _apikey = "4edaf534aec82236f1f36265ec2decfa";
        public WeatherController(IUnitOfWork uow)
        {
            _uow = uow as UnitOfWork;
        }

        [HttpGet]
        [Route("GetCurrent")]
        public async Task<IHttpActionResult> GetCurrentWeather(string city, string countryCode)
        {
            var fullUrl = _apiurl + "weather?q=" + city + "," + countryCode + "&APPID=" + _apikey;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsAsync<JObject>();

            var temp = content["main"]["temp"];
            var weather = content["weather"][0]["main"];
            var result = (Convert.ToDouble(temp.ToString()) - 273.15) + "°C - " + weather;

            return Ok(result);
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IHttpActionResult> GetWeatherInfo(string city, string countryCode)
        {
            var fullUrl = _apiurl + "forecast?q=" + city + "," + countryCode + "&APPID=" + _apikey;

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();            
            var content = await response.Content.ReadAsAsync<JObject>();

            var list = content["list"];
            var values = list.Select(x => new
            {
                temp = x["main"]["temp"],
                date = x["dt_txt"],
                weatherInfo = x["weather"][0]["main"]
            });

            var most = from val in values.Take(15)
                       group val by val.weatherInfo into grouped
                       orderby grouped.Count() descending
                       select grouped.Key;



            var data = new
            {
                labels = values.Select(v => v.date.ToString().Substring(5, v.date.ToString().IndexOf(":")) + "h - " + v.weatherInfo.ToString()).Take(12).ToList(),
                datasets = new[] {
                    new
                    {
                        label = "Temperatures",
                        fill = false,
                        lineTension = 0.1,
                        backgroundColor = "rgba(75,192,192,0.4)",
                        borderColor = "rgba(75,192,192,1)",
                        borderCapStyle = "butt",
                        borderDashOffset = 0.0,
                        borderJoinStyle = "miter",
                        pointBorderColor = "rgba(75,192,192,1)",
                        pointBackgroundColor = "#fff",
                        pointBorderWidth = 1,
                        pointHoverRadius = 5,
                        pointHoverBackgroundColor = "rgba(75,192,192,1)",
                        pointHoverBorderColor = "rgba(220,220,220,1)",
                        pointHoverBorderWidth = 2,
                        pointRadius = 1,
                        pointHitRadius = 10,
                        data = values.Select(v => Convert.ToDouble(v.temp.ToString()) - 273.15).Take(12).ToList()
                    }
                }
            };

            var res = new
            {
                data = data,
                info = most.First()
            };

            return Ok(res);
        }
    }
}
