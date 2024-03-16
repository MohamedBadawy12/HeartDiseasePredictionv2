using Database.Entities;
using Repositories.DropDownViewModel;

namespace Repositories.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<IEnumerable<Prescription>> GetPrescriptions();
        Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn);
        Task<IEnumerable<Prescription>> FilterPrescriptions(long search);
        Task<Prescription> GetPrescription(int id);
        Prescription Get_Prescription(int id);
        Task<DoctorDropDownViewMode> GetDoctorDropDownsValues();
        Task AddAsync(Prescription prescription);
        void Remove(Prescription prescription);
    }
}
