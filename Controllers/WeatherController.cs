using Microsoft.AspNetCore.Mvc;

using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Controllers
{ 
	[ApiController]
	[Route("api")]
	public class WeatherController : ControllerBase
	{
		private readonly WeatherService _weatherService;

		public WeatherController()
		{
			_weatherService = new WeatherService();
		}

		[HttpGet("weather-by-coordinates")]
		public async Task<IActionResult> GetWeatherByCoordinates(double latitude, double longitude)
		{
			var weather = await _weatherService.GetWeatherDataByCoordinatesAsync(latitude, longitude);
			return Ok(weather);
		}

		[HttpGet("irrigation-by-coordinates")]
		public async Task<IActionResult> ShouldIrrigateByCoordinates(double latitude, double longitude)
		{
			var weather = await _weatherService.GetWeatherDataByCoordinatesAsync(latitude, longitude);

			if (weather.Rain != null && weather.Rain.OneHour > 0)
			{	
				return Ok("Não é necessário irrigar. Chuva detectada.");
			}

			if (weather.Main.Humidity < 50)
			{
				return Ok("Ligando sistema de irrigação. Baixa umidade.");
			}

			return Ok("Nenhuma irrigação necessária neste momento.");
		}
	}

}
