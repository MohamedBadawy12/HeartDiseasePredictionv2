using Database.Entities;

namespace Services.Interfaces
{
    public interface IPrescriptionsService
    {
        Task<IEnumerable<Prescription>> GetPrescriptions();
        Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn);
        Task<IEnumerable<Prescription>> FilterPrescriptions(long search);
        Task<Prescription> GetPrescription(int id);
        Prescription Get_Prescription(int id);
        //IQueryable<Prescription> FilterPrescriptionsAsync(long patientSSN,AppointmentSearchDto searchDto);
        Task AddAsync(Prescription prescription);
        void Remove(Prescription prescription);
    }
}
