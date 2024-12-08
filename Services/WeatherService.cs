using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TP2_ISI_2024.Models;
public class WeatherService
{
	private readonly string _apiKey;
	private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

	public WeatherService()
	{
		var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
		_apiKey = configuration["WeatherApi:ApiKey"];
	}

	public async Task<WeatherResponse> GetWeatherDataByCoordinatesAsync(double latitude, double longitude)
	{
		using var client = new HttpClient();
		var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
		var response = await client.GetStringAsync(url);
		return JsonConvert.DeserializeObject<WeatherResponse>(response);
	}

}
