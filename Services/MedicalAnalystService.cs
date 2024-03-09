using Database.Entities;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services
{
    public class MedicalAnalystService : IMedicalAnalystService
    {
        private readonly IMedicalAnalystRepository _medicalAnalystRepository;
        public MedicalAnalystService(IMedicalAnalystRepository medicalAnalystRepository)
        {
            _medicalAnalystRepository = medicalAnalystRepository;
        }

        public async Task Add(MedicalAnalyst medicalAnalyst) => await _medicalAnalystRepository.Add(medicalAnalyst);

        public Task<IEnumerable<MedicalAnalyst>> FilterMedicalAnalyst(string search) =>
            _medicalAnalystRepository.FilterMedicalAnalyst(search);

        public async Task<MedicalAnalyst> GetMedicalAnalyst(int id) => await _medicalAnalystRepository.GetMedicalAnalyst(id);

        public async Task<IEnumerable<MedicalAnalyst>> GetMedicalAnalysts() => await _medicalAnalystRepository.GetMedicalAnalysts();

        public async Task<MedicalAnalyst> GetProfile(string userId) => await _medicalAnalystRepository.GetProfile(userId);

        public MedicalAnalyst Get_MedicalAnalyst(int id) => _medicalAnalystRepository.Get_MedicalAnalyst(id);

        public void Remove(MedicalAnalyst medicalAnalyst) => _medicalAnalystRepository.Remove(medicalAnalyst);

    }
}
