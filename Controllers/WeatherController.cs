using SampleDBConn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SampleDBConn.Model;

namespace WeatherStation.Controllers
{
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private readonly IOptions<MySettingsModel> appSettings;
        private readonly IOptions<AppOptions> _options;

        public string UNITS { get; set; }
         
     

        public WeatherController(IOptions<MySettingsModel> app)
        {
            appSettings = app;
        }

     

        [HttpGet("[action]")]
        public async Task<IActionResult> City(string city)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var apikeyval = appSettings.Value.apikey;

                    var list = new List<string>();
                    client.BaseAddress = new Uri("http://api.openweathermap.org");
                    var response = await client.GetAsync($"/data/2.5/weather?q={city}&appid={apikeyval}&units=metric");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var rawWeather = JsonConvert.DeserializeObject<RootObject>(stringResult);
 var test= (new
                    {
                        Temp = string.Join("",rawWeather.weather.Select(x => x.description)),
                        Summary = string.Join(",", rawWeather.weather.Select(x => x.main)),
                        City = rawWeather.name,
                            Country = rawWeather.sys.country
 });
                    ViewBag.Temp = test.Temp;
                    ViewBag.Summary = test.Summary;
                    ViewBag.City = test.City;
                    ViewBag.Country = test.Country;
                    return View();

                }
                catch (HttpRequestException httpRequestException)
                {
                    return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
                }
            }
        }
    }

    public class OpenWeatherResponse
    {
        public string Name { get; set; }

        public IEnumerable<WeatherDescription> Weather { get; set; }

        public Main Main { get; set; }
    }

    public class WeatherDescription
    {
        public string Main { get; set; }
        public string Description { get; set; }
    }

    public class Main
    {
        public string Temp { get; set; }
    }
}