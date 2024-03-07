using HearPrediction.Api.DTO;

namespace HearPrediction.Api.Data.Services
{
	public interface IMailServices
	{
		void SendEmail(MailRequestDto mailRequestDto);
	}
}
