using System.ServiceModel;
namespace TP2_ISI_2024.Interface
{
	[ServiceContract]
	public interface IWeatherSoapService
	{
		[OperationContract]
		string GetWeatherByCoordinates(double latitude, double longitude);
		[OperationContract]
		string ShouldIrrigateByCoordinates(double latitude, double longitude);
	}
}
