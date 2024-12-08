using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using TP2_ISI_2024.Interface;
namespace TP2_ISI_2024.Models
{
	public class WeatherSoapService : IWeatherSoapService
	{
		private readonly string _apiKey;
		private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";
		
		public WeatherSoapService()
		{
			var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
			_apiKey = configuration["WeatherApi:ApiKey"];
		}
		public string GetWeatherByCoordinates(double latitude, double longitude)
		{
			try
			{
				var weather = GetWeatherDataAsync(latitude, longitude).Result;

				if (weather?.Main?.Humidity < 30)
				{
					return $"Irrigação necessária! Umidade: {weather.Main.Humidity}%";
				}

				return $"Temperatura: {weather?.Main?.Temp}°C, Umidade: {weather?.Main?.Humidity}%";
			}
			catch (Exception e)
			{
				return $"Error: {e.Message}";
			}
		}

		public string ShouldIrrigateByCoordinates(double latitude, double longitude)
		{
			try
			{
				var weather = GetWeatherDataAsync(latitude, longitude).Result;

				if (weather?.Rain != null && weather.Rain.OneHour > 0)
				{
					return "Não é necessário irrigar. Chuva detectada.";
				}

				if (weather?.Main?.Humidity < 30)
				{
					return "Irrigação necessária. Baixa umidade.";
				}

				return "Nenhuma irrigação necessária neste momento.";
			}
			catch (Exception e)
			{
				return $"Error: {e.Message}";
			}
		}

		private async Task<WeatherResponse?> GetWeatherDataAsync(double latitude, double longitude)
		{
			using var client = new HttpClient();
			var url = $"{BaseUrl}?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric";
			var response = await client.GetStringAsync(url);
			return JsonConvert.DeserializeObject<WeatherResponse>(response);
		}
	}

}
