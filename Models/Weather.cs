using Newtonsoft.Json;

namespace TP2_ISI_2024.Models
{
	public class WeatherResponse
	{
		public WeatherMain Main { get; set; }
		public WeatherRain Rain { get; set; }
	}

	public class WeatherMain
	{
		public double Temp { get; set; }
		public double Humidity { get; set; }
	}

	public class WeatherRain
	{
		[JsonProperty("1h")]
		public double OneHour { get; set; }
	}

}
