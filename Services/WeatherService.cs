using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace TP2_ISI_2024.Models;
public class WeatherService
{
	private const string ApiKey = "bd79b4e7565fcb316c2437d37d6ac8d0"; // Substitua pela chave da API
	private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

	public async Task<WeatherResponse> GetWeatherDataByCoordinatesAsync(double latitude, double longitude)
	{
		using var client = new HttpClient();
		var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={ApiKey}&units=metric";
		var response = await client.GetStringAsync(url);
		return JsonConvert.DeserializeObject<WeatherResponse>(response);
	}

}
