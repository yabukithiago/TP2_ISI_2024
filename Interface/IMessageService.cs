using System.ServiceModel;
using TP2_ISI_2024.Controllers;
using TP2_ISI_2024.Models;

namespace TP2_ISI_2024.Interface
{
	[ServiceContract]
	public interface IMessageService
	{
		[OperationContract]
		List<Message> GetMessages();

		[OperationContract]
		Message GetMessage(int id);

		[OperationContract]
		Message PostMessage(MessageCreateDto messageDto);

		[OperationContract]
		void PutMessage(int id, MessageUpdateDto messageDto);

		[OperationContract]
		void DeleteMessage(int id);
	}
}
