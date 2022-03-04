using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace OcelotApiGetway.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {

            List<WeatherForecast> d = GetList(@"https://localhost:80/weatherforecast");
            //var rng = new Random();
            //return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            //{
            //    Date = DateTime.Now.AddDays(index),
            //    TemperatureC = rng.Next(-20, 55),
            //    Summary = Summaries[rng.Next(Summaries.Length)]
            //})
            //.ToArray();

            return d.ToList<WeatherForecast>();
            
        }


        public List<WeatherForecast> GetList(string Url)
        {
            Uri uri = new Uri(Url);
            using (HttpClientHandler httpClientHandler = new HttpClientHandler())
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return cert.NotAfter >= DateTime.Now; };
                using (HttpClient obj = new HttpClient(httpClientHandler))
                {
                    //obj.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(schema, tokeninfo);
                    //obj.Timeout = TimeSpan.FromMilliseconds(RequestTimeOutInMilliseconds);
                    //obj.MaxResponseContentBufferSize = BufferSize;
                    //FillHeaderValuesInRequest(obj);
                    HttpResponseMessage response = obj.GetAsync(uri).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == (System.Net.HttpStatusCode)210)
                        {
                            string errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                            throw new Exception("Error during remote call");
                        }
                        string content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var list = JsonConvert.DeserializeObject<List<WeatherForecast>>(content);
                        return list;
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        string errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        throw new Exception(errorMessage);
                    }
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
