using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OcelotApiGetway.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace OcelotApiGetway.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
           // List<dynamic> d = GetList(@"https://localhost:5001/weatherforecast");

            return View();
        }

        public IActionResult Privacy()
        {
            //GetList(@"http://localhost:80/api/WeatherForecast");

            //GetList(@"http://localhost:22658/weatherforecast");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public List<dynamic> GetList(string Url)
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
                        List<dynamic> list = JsonConvert.DeserializeObject<List<dynamic>>(content);
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
