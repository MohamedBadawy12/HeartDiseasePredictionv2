using Database.Entities;
using HearPrediction.Api.Data.Services.IRepository;
using HearPrediction.Api.Data.Services.Repository;
using System.Threading.Tasks;

namespace HearPrediction.Api.Data.Services
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDbContext _context;
		public IDoctorRepository Doctors { get; private set; }

		public IPatientRepository Patients { get; private set; }
		public IMedicalAnalystRepository medicalAnalyst { get; private set; }

		public IReciptionistRepository reciptionist { get; private set; }

		public IAppointmentRepository appointment { get; private set; }
		public IPrescriptionRepository prescription { get; private set; }
		public ILabRepository lab { get; private set; }

		public UnitOfWork(AppDbContext context)
		{
			_context = context;
			Patients = new PatientRepository(context);
			Doctors = new DoctorRepository(context);
			medicalAnalyst = new MedicalAnalystRepository(context);
			reciptionist = new ReceptionistRepository(context);
			lab = new LabRepository(context);
			prescription = new PrescriptionRepository(context);
			appointment = new AppointmentRepository(context);
		}

		public async Task Complete() => await _context.SaveChangesAsync();
		//public void Completes() => _context.SaveChanges();
	}
}
