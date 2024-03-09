using HearPrediction.Api.DTO;

namespace Services.Interfaces
{
    public interface IMailService
    {
        void SendEmail(MailRequestDto mailRequestDto);
    }
}
