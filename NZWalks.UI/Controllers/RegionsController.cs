using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NZWalks.UI.Models;
using NZWalks.UI.Models.Dto;

namespace NZWalks.UI.Controllers
{
    public class RegionsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RegionsController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<RegionDto> response = new List<RegionDto>();

            try
            {
                // Get All Regions from Web API
                var client = _httpClientFactory.CreateClient(); // Creates a new Http Client, use it to consume the WEB API

                // base api appsettings.json'dan almak daha best case 
                var httpResponseMessage = await client.GetAsync("https://localhost:7220/api/regions");

                httpResponseMessage.EnsureSuccessStatusCode(); // throws exception if the Http response msg property is false

                // Deserializes JSON content into a collection of RegionDto using ReadFromJsonAsync
                response.AddRange( await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<RegionDto>>());

               // ViewBag.Response = response; // pass string data from controller to the view
            }
            catch (Exception ex)
            {
                // Log the exception
                
            }

            return View(response);
        }

        [HttpGet] // Serves the form where client can input the data
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost] // Called when the form data is submitted
        public async Task<IActionResult> Add(AddRegionViewModel model)
        {
            var client = _httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7220/api/regions"),
                // Serializes  the model(an instance of AddRegionViewModel) into a JSON string.
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json"),

			};

            // Asynchronously sends the constructed HTTP request and waits for the server's response
            var httpResponseMessage =  await client.SendAsync(httpRequestMessage);

            // Checks if the status code 200-299,otherwise throws an exception.
			httpResponseMessage.EnsureSuccessStatusCode();

            // Returns just a single object - Deserializes it into an object of type RegionDto
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response != null) 
            {
                // Redirect to Regions' controller's Index action method
                return RedirectToAction("Index", "Regions");
            }

            // Reload the form again if something goes wrong
            return View();
		}

        [HttpGet]
        // takes the id parameter from the view asp-route-id -> id has to match with the parameter
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = _httpClientFactory.CreateClient();

			// GetFromJsonAsync<T>() -> Sends an HTTP GET request to the specified URL, expects JSON response, and deserializes that JSON into an type of RegionDto
			var response = await client.GetFromJsonAsync<RegionDto>($"https://localhost:7220/api/regions/{id.ToString()}");

            if (response != null)
            {
                return View(response);
            }

            return View(null);
           
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RegionDto request)
        {
            var client = _httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7220/api/regions/{request.Id}"),
                // Serializes  the model(an instance of AddRegionViewModel) into a JSON string.
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json"),
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

			// Returns just a single object - Deserializes it into an object of type RegionDto
			var response = await httpResponseMessage.Content.ReadFromJsonAsync<RegionDto>();

            if (response != null )
            {
                return RedirectToAction("Edit", "Regions");
            }

            return View();
		}

        [HttpPost]
        public async Task<IActionResult> Delete(RegionDto request)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7220/api/regions/{request.Id}");

                httpResponseMessage?.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Regions");
            }
            catch (Exception ex)
            {
                // Console or log
                
            }

            return View("Edit");
		}
	}
}
