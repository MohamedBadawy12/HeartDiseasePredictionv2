using Database.Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class PrescriptionsService : IPrescriptionsService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        public PrescriptionsService(IPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }
        public async Task AddAsync(Prescription prescription) => await _prescriptionRepository.AddAsync(prescription);

        public async Task<IEnumerable<Prescription>> FilterPrescriptions(long search) =>
            await _prescriptionRepository.FilterPrescriptions(search);

        public async Task<Prescription> GetPrescription(int id) => await _prescriptionRepository.GetPrescription(id);

        public async Task<IEnumerable<Prescription>> GetPrescriptions() => await _prescriptionRepository.GetPrescriptions();

        public async Task<List<Prescription>> GetPrescriptionsByUserSSN(long ssn) =>
            await _prescriptionRepository.GetPrescriptionsByUserSSN(ssn);

        public Prescription Get_Prescription(int id) =>
            _prescriptionRepository.Get_Prescription(id);

        public void Remove(Prescription prescription) => _prescriptionRepository.Remove(prescription);
    }
}
