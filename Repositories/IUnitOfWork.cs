using Repositories.Interfaces;

namespace Repositories
{
    public interface IUnitOfWork
    {
        IDoctorRepository Doctors { get; }
        IPatientRepository Patients { get; }
        IMedicalAnalystRepository medicalAnalysts { get; }
        IReciptionistRepository reciptionists { get; }
        IPrescriptionRepository prescriptions { get; }
        ILabRepository labs { get; }
        Task Complete();
    }
}
