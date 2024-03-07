using HearPrediction.Api.Data.Services.IRepository;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services
{
	public interface IUnitOfWork
	{
		IDoctorRepository Doctors { get; }
		IPatientRepository Patients { get; }
		IMedicalAnalystRepository medicalAnalyst { get; }
		IReciptionistRepository reciptionist { get; }
		IAppointmentRepository appointment { get; }
		IPrescriptionRepository prescription { get; }
		ILabRepository lab { get; }
		Task Complete();
		//void Completes();
	}
}
